using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private Sound[] sounds;

    private Dictionary<string, Sound> soundDict;
    
    private void Awake()
    {
        
        soundDict = new Dictionary<string, Sound>();
        for(int i = 0; i < sounds.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.name = sounds[i].name;
            source.volume = sounds[i].volume;
            source.pitch = sounds[i].pitch;
            source.clip = sounds[i].audioClip;
            sounds[i].audioSource = source;

            soundDict[sounds[i].name] = sounds[i];
        }

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlaySound("cannon");
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlaySound("tripleShot");
        }
    }

    public void PlaySound(string name)
    {
        Sound sound = soundDict[name];
        AudioSource source = soundDict[name].audioSource;
        source.volume = sound.volume * (1f + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
        source.pitch = sound.pitch * (1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
        source.Play();
    }
}
