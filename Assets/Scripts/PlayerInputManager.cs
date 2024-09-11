using UnityEngine;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }



    public bool canJump;
    public bool isAim;
    public bool isDragging;
    public bool isDash;
    public bool isAttack;
    public bool isParry;
    public bool isCrystal;
    public bool isBlackHole;
    public bool isUseFlask;

    [Header("Drop UI and Button")]
    [SerializeField] Button dropButton;
    public bool isPressDrop;
    Color dropColorDefault;
    Image dropImage;

    private void Start()
    {
        dropImage = dropButton.GetComponent<Image>();
        dropColorDefault = dropImage.color;
    }
    public void OnJump(bool isJump)
    {
        canJump = isJump;
    }

    public void OnAim(bool _isAim)
    {
        isAim = _isAim;
        //isDragging = _isDragging;
    }


    public void OnDash(bool _isDash)
    {
        isDash = _isDash;
    }

    public void OnAttack(bool _isAttack)
    {
        isAttack = _isAttack;
    }

    ///

    public void OnParry(bool _isParry)
    {
        isParry = _isParry;
    }
    public void OnCrystal(bool _isCrystal)
    {
        isCrystal = _isCrystal;
    }
    public void OnBlackHole(bool _isBlackHole)
    {
        isBlackHole = _isBlackHole;
    }
    public void OnUseFlask(bool _isUseFlask)
    {
        isUseFlask = _isUseFlask;
    }

    public void OnDrop(bool _isPressDrop)
    {
        isPressDrop = _isPressDrop;

        if (_isPressDrop)
        {
            dropImage.color = new Color(1, 0, 0, 1);
            Debug.Log(dropImage.color);
        }
        else if (_isPressDrop == false)
        {
            dropImage.color = dropColorDefault;
            Debug.Log(dropImage.color);
        }

    }
}

