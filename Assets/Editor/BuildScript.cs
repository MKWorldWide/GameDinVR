// BuildScript.cs
// Quantum-documented Unity Editor script for automated builds
//
// Feature Context:
//   - Provides static method for command-line builds
//   - Used by build_automation.ps1
//
// Dependencies:
//   - UnityEditor
//   - System.IO
//
// Usage Example:
//   Run via build_automation.ps1 or Unity batchmode
//
// Performance:
//   - Fast, minimal overhead
//
// Security:
//   - No sensitive data
//
// Changelog:
//   - v1.0.0: Initial quantum template version

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    public static void PerformBuild()
    {
        string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        string buildPath = "Builds/GameDinVR_World/GameDinVR.exe";
        if (!Directory.Exists("Builds/GameDinVR_World"))
            Directory.CreateDirectory("Builds/GameDinVR_World");
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log("Build complete: " + buildPath);
    }
}
#endif 