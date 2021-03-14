using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is just an example script, feel free to edit it and configure it for your
/// own use.
/// </summary>
public class DLLCheckerExample : MonoBehaviour
{
    [SerializeField] DLLValidityCheck dllValidity;
    
    void Start()
    {
        PCheck.E($"Validity: {dllValidity.CheckIntegrity()}");
        Invoke(nameof(CheckInterval), 10.0f);
    }
    
    void CheckInterval()
    {
        PCheck.E($"Validity checked: {dllValidity.CheckIntegrity()}");
    }
}
