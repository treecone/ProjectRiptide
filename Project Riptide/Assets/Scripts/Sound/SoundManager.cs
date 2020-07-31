using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private Sound[] sounds;
    [SerializeField]
    private int numStartingSources;
    private Dictionary<string, Sound> soundDict;

    private List<AudioSource> audioSourcePool;
    private List<int> audioSourceIds;
    private void Awake()
    {
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
            Debug.Log("beginning play sound " + name);
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
                    Debug.Log("playing on source " + i);
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
                Debug.Log("no open source found, creating new source");
                return id;
            }
            //will never hit here but needs a return to compile
            return -2;
        } else
        {
            Debug.Log("no sound with name " + name);
            return -1;
        }
    }

    public void StopSound(int id)
    {
        int index = 0;
        bool found = false;
        for(int i = 0; i < audioSourceIds.Count; i++)
        {
            if(audioSourceIds[i] == id)
            {
                index = i;
                found = true;
            }
        }
        if(found)
        {
            audioSourcePool[index].Stop();
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
