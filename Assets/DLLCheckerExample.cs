using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// MIT License
///
/// Copyright (c) 2021 Boppy Games, LLC
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.

/// <summary>
/// This is just an example script, feel free to edit it and configure it for your
/// own use.
/// </summary>
public class DLLCheckerExample : MonoBehaviour
{
    [SerializeField] DLLValidityCheck dllValidity;
    
    void Start()
    {
        var validity = dllValidity.CheckIntegrity();
        
        // Stop the game randomly 3 or 10 minutes from now. This would drive most crackers insane.
        if(!validity) Invoke(nameof(StopGame), UnityEngine.Random.Range(3, 10) * 60.0f);
    }

    void StopGame()
    {
        // We give a non-zero error code here so it looks like a crash
        Application.Quit(1);
    }
}
