using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;


    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] float maxTravelDistance = 7f;
    [SerializeField] float spinDuration = 2;
    [SerializeField] float spinGravity = 1f;
    [SerializeField] float hitCoolDown;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;


    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bouncingSpeed;


    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 lauchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float FreezeTimeDuration;
    [SerializeField] private float returningSpeed;
    private Vector2 finalDirection;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool timeStopUnlock { get; private set; }
    public bool vulnerableUnlock { get; private set; }
    

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;      //So luong dot muon co
    [SerializeField] private float spaceBetweenDots;  //Khoang cach giua cac dot
    [SerializeField] private GameObject dotPrefab;  // Hinh anh cua dot
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();
        GenerateDots();
        SetUpGravity();
        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
    }

    protected override void Update()
    {
        if (PlayerInputManager.instance.isAim == false) // Nếu nhả nút thì lấy vị trí cuối cùng của con chuột
        {
            finalDirection = new Vector2(AimDirection().normalized.x * lauchForce.x,
                AimDirection().normalized.y * lauchForce.y);
        }

        if (PlayerInputManager.instance.isAim) // Khi đang ấn giữ nút chuột thì cập nhật vị trí cho từng dấu chấm
        {
            //for (int i = 0; i < dots.Length; i++)
            //{
            //    dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            //}
        }
    }
    #region Unlock Region

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockSpinSword();
        UnlockPierceSword();
        UnlockTimeStop();
        UnlockVulnerable();
    }
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlock = true;
    }

    private void UnlockVulnerable()
    {
        if (vulnerableUnlockButton.unlocked)
            vulnerableUnlock = true;
    }
    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }

    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }
    #endregion
    public void CreateSword() // Tạo ra cây kiếm khi người chơi ném nó
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetUpBounce(true, bounceAmount,bouncingSpeed);

        else if (swordType == SwordType.Pierce)
            newSwordScript.SetUpPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetUpSpin(true, maxTravelDistance, spinDuration,hitCoolDown);

        newSwordScript.SetUpSword(finalDirection, swordGravity, player,FreezeTimeDuration,returningSpeed);
        player.AssignNewSword(newSword);
        DotsActive(false);
    }

    private void SetUpGravity()
    {
        if(swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if(swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        else if(swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
    }

    #region Aim region

    public Vector2 AimDirection()// Xác định hướng ném cây kiếm dựa trên hướng và vị trí player đứng
    {
        //Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = new Vector2(player.transform.position.x * player.facingDirection, 2);
        Vector2 direction = mousePosition/* - playerPosition*/;
        return direction;
    }

    public void DotsActive(bool _isActive) //Bật hình ảnh của các dấu chấm
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }
    private void GenerateDots() // Sản sinh ra những dấu chấm đưa vào trong game, nhưng nó sẽ được ẩn đi
    {
        dots = new GameObject[numberOfDots]; //Khai bao so luong dot trong mang dots
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t) // Xác định vị trí những dấu chấm, dựa vào vt player, hướng player trái hay phải,
                                          // lực ném, gravity...
    {
        Vector2 position = (Vector2)player.transform.position
            + new Vector2(AimDirection().normalized.x * lauchForce.x,
                            AimDirection().normalized.y * lauchForce.y)
            * t + .5f * (Physics2D.gravity * swordGravity)
            * (t * t);
        return position;
    }
    #endregion
}
