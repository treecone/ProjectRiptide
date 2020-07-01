using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;
    public float volume;
    public float pitch;
    public float volumeVariance;
    public float pitchVariance;

    [HideInInspector]
    public AudioSource audioSource;
}
