
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public enum ESoundType
{
    BGM,
    EFFECT
}

public class AudioManager : MonoBehaviour
{
    // audio Source
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioSource> effectSource;
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Sound Clip")]
    [SerializeField] private AudioClip[] gunSoundList;
    
    // path
    private const string SOUND_PATH = "Sound/";
    private const string GUNSOUND_PATH = "Enemy/HeadShot/";

    private Dictionary<string, AudioClip[]> _clipDic;
    
    public void Init(int createCount)
    {
        _clipDic = new Dictionary<string, AudioClip[]>();
        audioMixer = Resources.Load<AudioMixer>(Path.Combine(SOUND_PATH, "AudioMixer"));

        bgmAudioSource = gameObject.AddComponent<AudioSource>(); 
        bgmAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
        
        effectSource = new List<AudioSource>();
        var mixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
        for (var i = 0; i < createCount; ++i)
        {
            effectSource.Add(gameObject.AddComponent<AudioSource>());
            effectSource[i].outputAudioMixerGroup = mixerGroup;
        }
        
        gunSoundList = Resources.LoadAll<AudioClip>(Path.Combine(SOUND_PATH, GUNSOUND_PATH));
        
        _clipDic.Add("HeadShot", gunSoundList);
    }

    public void PlaySound(ESoundType type, string key, int index = -1)
    {
        if (_clipDic.TryGetValue(key, out var clips) == false)
        {
            return;
        }

        switch (type)
        {
            case ESoundType.BGM:
                break;
            
            case ESoundType.EFFECT:
                PlayRandomSound(clips[Random.Range(0, clips.Length)]);
                break;
            
            default:
                return;
        }
    }

    private void PlayRandomSound(AudioClip clip)
    {
        var source = effectSource.FirstOrDefault(x => x.isPlaying == false);

        source = (source == null) ? effectSource[0] : source;
        source.clip = clip;
        source.Play();
    }
}
