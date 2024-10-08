using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private Rig aimRig;
    [SerializeField] private AimTarget aimTarget;

    [Header("Rig Value")] 
    [SerializeField] private float maxWeight;

    private IEnumerator _weightLerpRoutine;
    
    private void Start()
    {
        aimRig.weight = 0.0f;
        aimTarget.gameObject.SetActive(false);
    }
    
    public void StartAim()
    {
        aimTarget.gameObject.SetActive(true);
        aimRig.weight = maxWeight;
    }

    public void StopAim()
    {
        aimTarget.gameObject.SetActive(false);
        aimRig.weight = 0.0f;
    }
}
