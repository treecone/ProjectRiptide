using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip audioClip;

    private AudioSource _audioSource;
    private float startVolume;
    private float currentVolumeMod;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        startVolume = _audioSource.volume;
        currentVolumeMod = 0.5f;
        instance = this;
    }

    private void Start()
    {
        StartMusic();
    }

    public void StartMusic()
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    public void SetVolume(float amount)
    {
        currentVolumeMod = amount;
        _audioSource.volume = startVolume * currentVolumeMod;
    }
    public float GetVolume()
    {
        return currentVolumeMod;
    }
}
