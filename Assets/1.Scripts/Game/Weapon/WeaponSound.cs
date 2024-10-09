using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [Header("Sound List")] 
    [SerializeField] private List<AudioClip> fireSound;

    public void PlayFireSound()
    {
        audioSource.clip = fireSound[Random.Range(0, fireSound.Count)];
        audioSource.Play();
    }
}
