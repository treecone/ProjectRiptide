using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip audioClip;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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
}
