using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEditor.Build;
#endif

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
/// 
/// Hello there :)
///
/// Unfortunately if you've come across this project someone has probably cracked your game, which
/// is extremely frustrating as a game developer. The point of this project is to give you some basic
/// protection against trivial attacks, which will stop most crackers. However any game can be cracked
/// given enough time and enough willpower. We just want to provide all Unity game developers some
/// simple defense against these DLL substitution attacks. This should be good "enough" to prevent
/// most automated forms of attacks.
///
/// Some assumptions we're making in this class:
///  - You are building for Windows, Mac OSX or Linux. Mobile or web builds are not officially supported right now.
///  - All of your plugins are within the Assets/Plugins directory (or subdirectories therein)
///
/// How to use this class:
///  - Call CheckIntegrity() at startup and during semi-regular intervals to make sure no new malicious DLLs
///         have been loaded. Choose what you want to do here: it could make it more obvious to a cracker that
///         you are aware of them if you close the application right away so think about what the right strategy
///         is for your game.
///  - We recommend keeping computeHashesOnBuild enabled, so that hashes are computed automatically during build.
///  - We recommend keeping strictChecks enabled, although it can cause false positives if you are loading custom
///         DLLs outside of the build.
///  - We STRONGLY recommend disabling printHashErrors in builds - printing hash errors will make it really easy
///         for a cracker to see where you are checking for validity.
///
/// HIGHLY Recommended asset to pair with this:
///  - BeeByte's obfuscator: https://assetstore.unity.com/packages/tools/utilities/obfuscator-48919
///
/// If you use BeeByte's obfuscator make sure you enable class/method/property renaming for this class. Also
/// make sure to configure the obfuscator to generate additional "garbage" methods (~500+/class is good).
/// 
/// Other helpful assets:
///  - ACT: https://assetstore.unity.com/packages/tools/utilities/anti-cheat-toolkit-152334
///
/// My only request is that if you use this script, do no harm to the user's machine who is running your
/// cracked software. Most people who download cracked software/games wouldn't have paid for your software/game
/// anyway and doing harm to them or their hardware won't help the problem of game piracy. Also consider a large
/// portion of people downloading cracked software are children who have no money.
public class DLLValidityCheck : ScriptableObject
#if UNITY_EDITOR
    , IPreprocessBuildWithReport
#endif
{
    [Serializable]
    public struct DLLDefinition
    {
        public string dllName; // The filename of the DLL
        public string dllSha; // The SHA-256 hash of the DLL
    }

    readonly string[] dllEndings =
    {
        ".dll", ".winmd", ".so", ".jar", ".aar", ".xex", ".def", ".suprx", ".prx", ".sprx",
        ".rpl", ".cpp", ".cc", ".c", ".h", ".jslib", ".jspre", ".bc", ".a", ".m", ".mm",
        ".swift", ".xib", ".dylib"
    };

    /// <summary>
    /// Any 3rd party DLL that is not included in the build should be included here, otherwise you
    /// will get a false-positive when the DLL is loaded.
    ///
    /// The ignored DLLs here must be ignored because the code that checks hashes will be compiled
    /// into one of these DLLs and its not clear right now if its possible to check their integrity.
    /// </summary>
    readonly string[] ignoreDLLs =
    {
        "UnityEngine.dll",
        "GameAssembly.dll",
    };

#if UNITY_EDITOR
    [Tooltip("Whether or not to automatically compute hashes when building a development build.")]
    [SerializeField] bool computeHashesForDevelopmentBuilds = true;
    [Tooltip("Whether or not to automatically compute hashes when building a standalone executable.")]
    [SerializeField] bool computeHashesOnBuild = true;
#endif
    
    [Tooltip("Strict checks will make sure no suspicious DLLs have been loaded that were not present during build.")]
    [SerializeField] bool strictChecks = true;
    [Tooltip("The list of DLLs - this will be recalculated during build if computeHashesOnBuild is set.")]
    [SerializeField] List<DLLDefinition> dlls = new List<DLLDefinition>();
    [Tooltip("This should only be used for debugging - NEVER enable this in a real build.")]
    [SerializeField] bool printHashErrors = false;
    
    /// <summary>
    /// You should call this function during startup.
    ///
    /// We strongly, strongly recommend buying the recommended obfuscator plugin to try to hide this function. If
    /// a cracker is able to rewrite this function, we lose all of the protection it provides.
    /// </summary>
    /// <returns>True if it appears the DLLs are okay, false if it appears there are suspicious or unverified DLLs.</returns>
    public bool CheckIntegrity()
    {
        if (Application.isEditor) return true;
        
        // Lets first check to make sure any DLLs that *could* be loaded are ok
        foreach (var dllPath in FindFiles(Application.dataPath, dllEndings))
        {
            var dllName = Path.GetFileName(dllPath);
            if (ignoreDLLs.Contains(dllName)) continue;
            var verified = false;
            foreach (var dllDef in dlls)
            {
                if (!dllName.ToLower().Equals(dllDef.dllName)) continue;
                if (GetSha256(dllPath) == dllDef.dllSha)
                    verified = true;
                else
                {
                    // This is the case where the DLL has been found, but has been replaced by another
                    // DLL (DLL substitution attack).
                    if(printHashErrors) Debug.LogError($"Hashes did not patch: {dllPath}");
                    return false;
                }
            }

            // We have never seen this DLL before, could be a 3rd party DLL or it could be someone
            // injecting a DLL here for cracking, modding, etc.
            if (!verified && strictChecks)
            {
                if(printHashErrors) Debug.LogError($"Module Location: {dllPath}");
                return false;
            }
        }

        return true;
    }

    IEnumerable<string> FindFiles(string path, string[] endings)
    {
        if (!Directory.Exists(path)) yield break;
        foreach (var file in Directory.GetFiles(path))
        {
            if (!endings.Any(a => file.ToLower().EndsWith(a))) continue;
            yield return file;
        }

        foreach (var directory in Directory.GetDirectories(path))
        foreach (var result in FindFiles(directory, endings))
            yield return result;
    }

    /// <summary>
    /// This is just a simple SHA-256 computation function.
    /// </summary>
    /// <param name="filePath">The file to hash.</param>
    /// <returns>The hash of the file</returns>
    string GetSha256(string filePath)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        var sb = new StringBuilder();

        using (var sha256 = new SHA256Managed())
        {
            var hash = sha256.ComputeHash(fileBytes);
            foreach (var b in hash)
                sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }

#if UNITY_EDITOR
    public int callbackOrder { get; }
    public void OnPreprocessBuild(BuildReport report)
    {
        if(!computeHashesForDevelopmentBuilds && ((uint)report.summary.options & (uint)BuildOptions.Development) == 1) return;
        if (!computeHashesOnBuild) return;
        RefreshAllDLLHashes();
    }

    /// <summary>
    /// Call this if you just want to refresh hashes for all DLLs we can find.
    /// </summary>
    public void RefreshAllDLLHashes()
    {
        dlls.Clear();
        var pluginsDirectory = Path.Combine(Application.dataPath, "Plugins");
        foreach (var dllPath in FindFiles(pluginsDirectory, dllEndings))
        {
            var sha256 = GetSha256(dllPath);
            dlls.Add(new DLLDefinition
            {
                dllName = Path.GetFileName(dllPath),
                dllSha = sha256,
            });
        }

        EditorUtility.SetDirty(this);
    }
    
    [CustomEditor(typeof(DLLValidityCheck))]
    public class DLLValidityCheckerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var checker = (DLLValidityCheck) target;

            if(checker.dlls == null || checker.dlls.Count == 0)
                GUILayout.Label("No DLLs have been hashed.");
            else GUILayout.Label($"Hashed DLLs: {checker.dlls.Count}");
            
            if (GUILayout.Button("Populate DLLs"))
                checker.RefreshAllDLLHashes();
        }
    }
#endif
}