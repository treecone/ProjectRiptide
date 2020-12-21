using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private int numStartingSources;
    private Dictionary<string, Sound> soundDict;

    private float globalVolume = 0.5f;

    [SerializeField]
    private List<iAudioSource> audioSourcePool;
    [System.Serializable]
    private class iAudioSource
    {
        public AudioSource audioSource;
        public int id;

        public iAudioSource(AudioSource audioSource, int id)
        {
            this.audioSource = audioSource;
            this.id = id;
        }
    }
    private void Awake()
    {
        Sound[] sounds = Resources.LoadAll<Sound>("ScriptableObjectInstances/Sounds");
        Debug.Log(sounds.Length);
        audioSourcePool = new List<iAudioSource>();
        soundDict = new Dictionary<string, Sound>();
        foreach(Sound s in sounds)
        {
            soundDict[s.name] = s;
        }
        for (int i = 0; i < numStartingSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(new iAudioSource(source, -1));
        }

        instance = this;
    }

    public int PlaySound(string name)
    {
        if(soundDict.ContainsKey(name))
        {
            Sound sound = soundDict[name];

            bool foundSource = false;
            for(int i = 0; i < audioSourcePool.Count; i++)
            {
                AudioSource source = audioSourcePool[i].audioSource;
                if (!source.isPlaying)
                {
                    foundSource = true;
                    source.clip = sound.audioClip;
                    source.volume = sound.volume * (1f + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f)) * globalVolume;
                    source.pitch = sound.pitch * (1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
                    source.Play();
                    audioSourcePool[i].id = Random.Range(0, int.MaxValue);
                    //Debug.Log("playing sound on audiosource " + i + ", id " + audioSourcePool[i].id);
                    return audioSourcePool[i].id;
                }
            }
            if (!foundSource)
            {
                //create new audio source
                AudioSource source = gameObject.AddComponent<AudioSource>();

                source.clip = sound.audioClip;
                source.volume = sound.volume * (1f + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f)) * globalVolume;
                source.pitch = sound.pitch * (1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
                source.Play();

                int id = Random.Range(0, int.MaxValue);
                audioSourcePool.Add(new iAudioSource(source, id));
                return id;
            }
            //will never hit here but needs a return to compile
            return -2;
        } else
        {
            return -1;
        }
    }

    public void StopSound(int id)
    {
        int index = 0;
        bool found = false;
        //Debug.Log("stopping " + id);
        for(int i = 0; i < audioSourcePool.Count; i++)
        {
            if(audioSourcePool[i].id == id)
            {
                index = i;
                found = true;
            }
        }
        if(found)
        {
            //Debug.Log("found");
            audioSourcePool[index].audioSource.Stop();
            audioSourcePool[index].id = -1;
        }
    }

    public int PlaySound(params string[] list)
    {
        return PlaySound(list[Random.Range(0,list.Length)]);
    }

    private void ClearOutSources()
    {
        for(int i = 0; i < audioSourcePool.Count; i++)
        {
            if(!audioSourcePool[i].audioSource.isPlaying && audioSourcePool.Count > numStartingSources)
            {
                audioSourcePool.RemoveAt(i);
                i--;
            }
        }
    }

    public void SetGlobalVolume(float amount)
    {
        float oldGlobalVolume = globalVolume;
        globalVolume = amount;
        foreach(iAudioSource source in audioSourcePool)
        {
            source.audioSource.volume *= (globalVolume / oldGlobalVolume);
        }
    }

    public float GetGlobalVolume()
    {
        return globalVolume;
    }
}
