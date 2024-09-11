using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class BlackHole_Skill_Controller : MonoBehaviour
{
    [SerializeField] GameObject hotKeyPrefab;
    [SerializeField] List<KeyCode> keyCodeList;
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();
    //Tránh việc đang cast skill thì 1 con khác bước vô lỗ đen =>dẫn đến việc hiện hotkey trên con đó
    private bool canCreateHotKey = true; 
    
    private bool canAttack;
    private int amountOfAttack = 4;
    private float cloneAttackCooldown = .3f; // thời gian nó xuất hiện để chém mỗi con enemy
    private float cloneAttackTimer;
    private float blackHoleTimer;
    public bool playerCanExitState {  get; private set; }
    public bool playerCanDisappear = true;

    public void SetUpBlackHole(float _maxSize, float _growSpeed,float _shrinkSpeed,int _amountOfAttack,float _CloneAttackCooldown,float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttack = _amountOfAttack;
        cloneAttackCooldown = _CloneAttackCooldown;
        blackHoleTimer = _blackHoleDuration;
        if (SkillManager.instance.clone.crystalInsteadOfClone)
            playerCanDisappear = false;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackHoleTimer -= Time.deltaTime;
        if(blackHoleTimer < 0)
        {
            blackHoleTimer = Mathf.Infinity;
            if(targets.Count > 0)  
                ReleaseCloneAttack();       
            else
                FinishBlackHoleAbility();
        }



        if (Input.GetKeyDown(KeyCode.R)) //Sau khi nhấn hết hotkey và ấn r nó sẽ tấn công enemy
        {
            ReleaseCloneAttack(); // Thực hiện tấn công enemy ( chủ yếu bật canAttack == true)
        }

        CloneAttackLogic(); // Logic để clone xuất hiện và tấn công enemy (canAttack phải == true)

        if (canGrow && !canShrink) //Mở hoạt ảnh hố đen
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        if (canShrink) // Tắt hoạt ảnh hố đen
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;
        if (playerCanDisappear)
        {
            playerCanDisappear = false;
            PlayerManager.instance.player.fx.MakeTransparent(true);
        }
        DestroyHotKey(); // Xóa hết toàn bộ gameObject HotKey
        canAttack = true;
        canCreateHotKey = false; //cấm việc tạo hotkey khi đang tấn công thì enemy khác bước vào lỗ đen
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && canAttack && amountOfAttack >0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);
            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 1;
            else xOffset = -1;

            if (SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomEnemy();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));

            }

            amountOfAttack--;

            if (amountOfAttack <= 0) //Chém đủ số lần thì kết thúc
            {
                //Kết thúc việc chém xong
                Invoke("FinishBlackHoleAbility", 1f);
            }

        }
    }


    private void FinishBlackHoleAbility()
    {
        DestroyHotKey();
        playerCanExitState = true;
        //Cho nó thoát khỏi BlackHoleState ngay sau khi đánh xong, kích hoạt qua hàm trong player
        
        canShrink = true; // thu hồi hố đen
        canAttack = false; // true thì mới vô CloneAttackLogic được
        
    }

    public void DestroyHotKey()
    {
        if (createdHotKey.Count <= 0)
            return;
        for(int i = 0;i < createdHotKey.Count; i++)
            Destroy(createdHotKey[i]);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Kich hoat trigger");
        if(collision.GetComponent<Enemy>() != null) //lỗ đen nếu nó dò thấy enemy trong vùng đen
        {
            Debug.Log("Tim thay enemy");
            collision.GetComponent<Enemy>().FreezeTime(true); //Enemy trong lỗ đen bị đứng yên
                                                             
            CreateHotKey(collision); //Tạo hotkey trên đầu cho từng Enemy mà nó dò được

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null) //lỗ đen nếu nó dò thấy enemy trong vùng đen
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0) //Check coi minh co cai dat phim trong list chua
        {
            Debug.Log("Khong du hotKey trong keyCodeList");
            return;
        }

        if (!canCreateHotKey)
            return;
        
        // spawn prefab cac hotkey trên đầu quái vật => + thêm vector để nằm nhích trên đầu nó
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);

        createdHotKey.Add(newHotKey);//add hotKey vào 1 list để dùng skill xong thì xóa object

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)]; // chọn random 1 phím bất kỳ từ danh sách hot key mình đã lưu

        keyCodeList.Remove(choosenKey); //Loại bỏ cái phím mà nó đã random ra.

        //=> remove hết thì sẽ vô cái if trên kia

        BlackHole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<BlackHole_HotKey_Controller>();

        //Set thuộc tính cho từng HotKey được sinh ra ( mỗi hotKey nó có dữ liệu riêng , vị trí quái, nút bấm của nó)
        newHotKeyScript.SetHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform)
    {
        targets.Add(_enemyTransform); 
    }
}
