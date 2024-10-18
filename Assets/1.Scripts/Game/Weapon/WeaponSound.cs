using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSound : MonoBehaviour
{
    [Header("AudioSource")] 
    [SerializeField] private AudioSource gunSoundSource;
    [SerializeField] private AudioSource headShotSoundSource;
    
    [Header("Sound List")] 
    [SerializeField] private List<AudioClip> fireSound;
    [SerializeField] private List<AudioClip> headShotSound;
    

    public void PlayFireSound()
    {
        gunSoundSource.clip = fireSound[Random.Range(0, fireSound.Count)];
        gunSoundSource.Play();
    }
    
    public void PlayHeadShotSound()
    {
        headShotSoundSource.clip = headShotSound[Random.Range(0, headShotSound.Count)];
        headShotSoundSource.Play();
    }
}
