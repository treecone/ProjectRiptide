using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private int numStartingSources;
    private Dictionary<string, Sound> soundDict;

    private List<AudioSource> audioSourcePool;
    private List<int> audioSourceIds;
    private void Awake()
    {
        Sound[] sounds = Resources.LoadAll<Sound>("ScriptableObjectInstances/Sounds");
        Debug.Log(sounds.Length);
        audioSourcePool = new List<AudioSource>();
        audioSourceIds = new List<int>();
        soundDict = new Dictionary<string, Sound>();
        foreach(Sound s in sounds)
        {
            soundDict[s.name] = s;
        }
        for (int i = 0; i < numStartingSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(source);

            audioSourceIds.Add(-1);
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
                AudioSource source = audioSourcePool[i];
                if (!source.isPlaying)
                {
                    foundSource = true;
                    source.clip = sound.audioClip;
                    source.volume = sound.volume * (1f + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
                    source.pitch = sound.pitch * (1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
                    source.Play();
                    audioSourceIds[i] = Random.Range(0, int.MaxValue);
                    return audioSourceIds[i];
                }
            }
            if (!foundSource)
            {
                //create new audio source
                AudioSource source = gameObject.AddComponent<AudioSource>();

                source.clip = sound.audioClip;
                source.volume = sound.volume * (1f + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
                source.pitch = sound.pitch * (1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
                source.Play();
                audioSourcePool.Add(source);
                int id = Random.Range(0, int.MaxValue);
                audioSourceIds.Add(id);
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
        Debug.Log("stopping " + id);
        for(int i = 0; i < audioSourceIds.Count; i++)
        {
            Debug.Log(audioSourceIds[i]);
            if(audioSourceIds[i] == id)
            {
                index = i;
                found = true;
            }
        }
        if(found)
        {
            Debug.Log("found");
            audioSourcePool[index].Stop();
            audioSourceIds[index] = -1;
        }
    }

    private void ClearOutSources()
    {
        for(int i = 0; i < audioSourcePool.Count; i++)
        {
            if(!audioSourcePool[i].isPlaying && audioSourcePool.Count > numStartingSources)
            {
                audioSourcePool.RemoveAt(i);
                audioSourceIds.RemoveAt(i);
                i--;
            }
        }
    }
}
