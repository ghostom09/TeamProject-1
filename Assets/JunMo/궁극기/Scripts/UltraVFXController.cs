using System;
using UnityEngine;

using UnityEngine.InputSystem;

public class UltraVFXController : MonoBehaviour
{
    [SerializeField]private ParadoxController paradoxController;

    public static bool usingUltra = false;
    
    
    private JobType jobType = JobType.Gun;

    void Job(JobType jobType)
    {
        this.jobType = jobType;
    }

    void Start()
    {
        paradoxController.ResetUltra();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            PressUltra();
        }
    }

    void PressUltra()
    {
        paradoxController.SetVolumeActive(true);
        if (!usingUltra)
        {
            UseUltra();
        }
        else
        {
            OffUltra();
        }
    }

    private void UseUltra()
    {
        if (jobType == JobType.Gun)
        {
            paradoxController.SetVolumeActive(true);
        }
        else
        {
            
        }
        usingUltra = true;
    }

    private void OffUltra()
    {
        paradoxController.SetVolumeActive(false);
        usingUltra = false;
    }
}
