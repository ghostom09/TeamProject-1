using System;
using UnityEngine;

using UnityEngine.InputSystem;

public class UltraVFXController : MonoBehaviour
{
    [SerializeField]private ParadoxController paradoxController;

    public static bool usingUltra = false;

    private jobType jobType;
    

    public void Job(jobType jobType)
    {
        this.jobType = jobType;
    }

    void Start()
    {
        paradoxController.ResetUltra();
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
        if (jobType == jobType.Gun)
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
