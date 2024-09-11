using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;

public class EntityFx : MonoBehaviour
{
    private SpriteRenderer sr;

    private Player player;

    [Header("Pop up Text fx")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("Screen shake fx")]
     private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMutiplier;
    [SerializeField] public Vector3 shakeSwordCatchPower;
    [SerializeField] public Vector3 shakeHighDamagedPower;

    [Header("After image fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;

    [Header("Flash Fx")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;


    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("Hit fx")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject hitFx_critical;

    [Header("Dust fx")]
    [SerializeField] private ParticleSystem dustFx;
    private void Start()
    {
        player = PlayerManager.instance.player;
        screenShake =  GetComponent<CinemachineImpulseSource>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(3,5);

        Vector3 positionOffset = new Vector3(randomX, randomY, 0);
        GameObject newText = Instantiate(popUpTextPrefab, transform.position, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
        
    }

    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDirection, _shakePower.y) * shakeMutiplier;
        screenShake.GenerateImpulse();
    }
    public void CreateAfterImage()
    {
        if(afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab,transform.position,transform.rotation);

            Debug.Log("Sinh ra image");
            newAfterImage.GetComponent<AfterImage>().SetupAfterImage(sr.sprite,colorLooseRate);
        }
    }

    private IEnumerator FlashFx()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
        sr.color = Color.white;
    }

    public void IgniteFxFor(float _second)
    {
        igniteFx.Play();
        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange",_second);
    }

    public void ChillFxFor(float _second)
    {
        chillFx.Play();
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _second);
    }

    public void ShockFxFor(float _second)
    {
        shockFx.Play();
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _second);
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    public void CreateHitFx(Transform _target,bool _critical)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);
        GameObject hitPrefab = hitFx;
        if (_critical)
        {
            hitPrefab = hitFx_critical;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDirection == -1)
                yRotation = 180;

            hitFxRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFx = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity,_target);

        newHitFx.transform.Rotate(hitFxRotation);
        
        Destroy(newHitFx, .5f);
    }

    public void PlayDustFx()
    {
        if(dustFx != null)
        {
            dustFx.Play();
        }
    }
}
