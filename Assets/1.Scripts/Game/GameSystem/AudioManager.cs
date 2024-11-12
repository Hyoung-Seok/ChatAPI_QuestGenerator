using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public enum ESoundType
{
    BGM,
    EFFECT,
}

public class AudioManager : MonoBehaviour
{
    // audio Source
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioSource> effectSource;
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Sound Clip")]
    [SerializeField] private AudioClip[] gunSoundList;
    [SerializeField] private AudioClip[] playerHitVoice;
    [SerializeField] private AudioClip[] playerHitSounds;
    [SerializeField] private AudioClip[] bgmSource;
    
    // path
    private const string SOUND_PATH = "Sound/";
    private const string BGM_PATH = "BGM/";
    private const string GUNSOUND_PATH = "Enemy/HeadShot/";
    private const string HITVOICE_PATH = "Player/HitVoice/";
    private const string HITSOUND_PATH = "Player/HitSound/";

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
            effectSource[i].spatialBlend = 1;
        }
        
        gunSoundList = Resources.LoadAll<AudioClip>(Path.Combine(SOUND_PATH, GUNSOUND_PATH));
        playerHitVoice = Resources.LoadAll<AudioClip>(Path.Combine(SOUND_PATH, HITVOICE_PATH));
        playerHitSounds = Resources.LoadAll<AudioClip>(Path.Combine(SOUND_PATH, HITSOUND_PATH));
        bgmSource = Resources.LoadAll<AudioClip>(Path.Combine(SOUND_PATH, BGM_PATH));
        
        _clipDic.Add("HeadShot", gunSoundList);
        _clipDic.Add("HitVoice", playerHitVoice);
        _clipDic.Add("HitSound", playerHitSounds);
        _clipDic.Add("BGM", bgmSource);

        bgmAudioSource.clip = _clipDic["BGM"].GetValue(1) as AudioClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void PlaySound(ESoundType type, string key, bool is3DSound, Vector3 pos = default, int index = -1)
    {
        if (_clipDic.TryGetValue(key, out var clips) == false)
        {
            return;
        }

        switch (type)
        {
            case ESoundType.BGM:
                break;
            
            case ESoundType.EFFECT when pos == Vector3.zero:
                PlayRandomSound(clips[Random.Range(0, clips.Length)], is3DSound);
                break;
            
            case ESoundType.EFFECT:
                PlayRandomSound(clips[Random.Range(0, clips.Length)], pos, is3DSound);
                break;
            
            default:
                return;
        }
    }

    public AudioClip[] GetAudioClips(string key)
    {
        return _clipDic.GetValueOrDefault(key);
    }

    private void PlayBackGroundSource()
    {
        
    }

    private void PlayRandomSound(AudioClip clip, bool is3D)
    {
        var source = effectSource.FirstOrDefault(x => x.isPlaying == false);

        source = (source == null) ? effectSource[0] : source;
        source.spatialBlend = (is3D) ? 1 : 0;
        source.clip = clip;
        source.Play();
    }
    
    private void PlayRandomSound(AudioClip clip, Vector3 pos, bool is3D)
    {
        var source = effectSource.FirstOrDefault(x => x.isPlaying == false);
        
        source = (source == null) ? effectSource[0] : source;
        source.spatialBlend = (is3D) ? 1 : 0;
        source.transform.position = pos;
        source.clip = clip;
        source.Play();
    }
}
