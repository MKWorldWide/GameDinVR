// BuildScript.cs
// Quantum-documented Unity build automation for GameDinVR
// Enables command-line builds for CI/CD, VRChat upload, or local automation
// Usage:
// Unity.exe -batchmode -projectPath "C:/Path/To/GameDinVR" -executeMethod BuildScript.PerformBuild -quit

using UnityEditor;
using System.IO;

/// <summary>
/// Static build script for automated Unity builds (CLI, CI/CD, VRChat prep).
/// </summary>
public static class BuildScript
{
    /// <summary>
    /// Performs a build of the Citadel of Resonance world for Windows Standalone.
    /// Output: Builds/CitadelOfResonance.exe
    /// </summary>
    public static void PerformBuild()
    {
        // --- Quantum Documentation ---
        // - This method is called from the Unity CLI for automated builds.
        // - Adjust buildTarget and outputPath as needed for VRChat or other platforms.
        // - Extend for asset bundles, VRChat SDK, or custom post-processing.

        string[] scenes = { "Assets/Scenes/CitadelOfResonance.unity" };
        string outputPath = "Builds/CitadelOfResonance.exe";
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        BuildOptions buildOptions = BuildOptions.None;

        // Ensure output directory exists
        Directory.CreateDirectory("Builds");

        // Run the build
        BuildReport report = BuildPipeline.BuildPlayer(scenes, outputPath, buildTarget, buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log($"[BuildScript] Build succeeded: {summary.totalSize} bytes at {outputPath}");
        }
        else
        {
            UnityEngine.Debug.LogError($"[BuildScript] Build failed: {summary.result}");
        }
    }
} 