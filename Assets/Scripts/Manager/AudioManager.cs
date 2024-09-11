using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinimumDistance;
    [SerializeField] private AudioSource[] sfx; //sound effect
    [SerializeField] private AudioSource[] bgm; //background music

    public bool playBGM;
    [SerializeField] private int bgmIndex;

    private bool canPlaySFX;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);

        Invoke("AllowSFX", 1);
    }

    private void Update()
    {
        if(!playBGM)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                PlayBGM(bgmIndex);
            }
        }
    }

    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        //if (sfx[_sfxIndex].isPlaying)
        //{
        //    return;
        //}

        if (canPlaySFX == false)
            return;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position,_source.position) > sfxMinimumDistance)
        {
            return;
        }
        if(_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 0.99f);
            sfx[_sfxIndex].Play();
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void StopSFX(int _index) => sfx[_index].Stop();

    public void StopAllSFX()
    {
        for(int i=0;i<sfx.Length;i++)
        {
            sfx[i].Stop();
        }
    }
    public void StopSFXWithTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;
        StopAllBGM();

        bgm[bgmIndex].Play();
    }

    public void TurnBGM()
    {
        if (playBGM)
            playBGM = false;
        else if(playBGM == false)
            playBGM = true;
    }

    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.25f);
            if(_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void AllowSFX() => canPlaySFX = true;   
}
