using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Custom Assets/Sound")]
public class Sound : ScriptableObject
{
    public string name;

    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume;
    [Range(0, 2)]
    public float pitch;
    [Range(0, 1)]
    public float volumeVariance;
    [Range(0, 1)]
    public float pitchVariance;
    public bool looping;

    void Reset()
    {
        volume = 1;
        pitch = 1;
        volumeVariance = 0.1f;
        pitchVariance = 0.1f;
        looping = false;
    }
}
