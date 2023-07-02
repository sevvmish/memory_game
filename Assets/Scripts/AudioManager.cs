using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioPack audioPack;

    private int priority = 0;

    // Start is called before the first frame update
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }
       
    public void PlaySound_Click()
    {
        playStandartSound(0, audioPack.SimpleClick);
        /*
        if ((priority > 0 && _audio.isPlaying) || (_audio.clip == audioPack.SimpleClick && _audio.isPlaying)) return;

        priority = 0;
        _audio.Stop();
        _audio.clip = audioPack.SimpleClick;
        _audio.Play();*/
    }

    public void PlaySound_BackRotate()
    {
        playStandartSound(0, audioPack.ReverseClick);
        /*
        if (priority > 0 && _audio.isPlaying) return;

        priority = 0;
        _audio.Stop();
        _audio.clip = audioPack.ReverseClick;
        _audio.Play();*/
    }

    public void PlaySound_Success()
    {
        playStandartSound(1, audioPack.Happy01);
        /*
        if (_audio.clip == audioPack.Happy01 && _audio.isPlaying) return;
        priority = 1;
        _audio.Stop();
        _audio.clip = audioPack.Happy01;
        _audio.Play();*/
    }

    private void playStandartSound(int _priority, AudioClip clip)
    {
        if ((priority > _priority && _audio.isPlaying) || (_audio.clip == clip && _audio.isPlaying)) return;

        priority = _priority;
        _audio.Stop();
        _audio.clip = clip;
        _audio.Play();
    }
}
