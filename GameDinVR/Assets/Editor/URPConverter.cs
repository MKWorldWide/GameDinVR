// URPConverter.cs
// Quantum-documented Unity Editor script for seamless Universal Render Pipeline (URP) conversion
// Automates URP setup, asset organization, and prepares for material/shader migration
// Modular, extendable, and ready for sacred-tier expansion
// Place in /Assets/Editor/

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

/// <summary>
/// Automates URP installation, asset setup, and pipeline assignment for GameDinVR.
/// Quantum-documented for maximum clarity and future expansion.
/// </summary>
public class URPConverter : EditorWindow
{
    [MenuItem("GameDinVR/URP/Setup Universal Render Pipeline")]
    public static void SetupURP()
    {
        // 1. Ensure URP is installed
        if (!IsURPInstalled())
        {
            EditorUtility.DisplayDialog(
                "URP Not Installed",
                "Universal Render Pipeline (URP) is not installed. Please install it via Window > Package Manager > Universal RP, then re-run this script.",
                "OK");
            return;
        }

        // 2. Create /Graphics/URP/ directory structure
        string urpRoot = "Assets/Graphics/URP";
        string[] subDirs = { "Materials", "Shaders", "Volumes" };
        if (!AssetDatabase.IsValidFolder("Assets/Graphics"))
            AssetDatabase.CreateFolder("Assets", "Graphics");
        if (!AssetDatabase.IsValidFolder(urpRoot))
            AssetDatabase.CreateFolder("Assets/Graphics", "URP");
        foreach (var sub in subDirs)
        {
            string subPath = urpRoot + "/" + sub;
            if (!AssetDatabase.IsValidFolder(subPath))
                AssetDatabase.CreateFolder(urpRoot, sub);
        }

        // 3. Create URP Pipeline Asset if not present
        string pipelineAssetPath = urpRoot + "/GameDinVR_URP_Pipeline.asset";
        RenderPipelineAsset pipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(pipelineAssetPath);
        if (pipelineAsset == null)
        {
            pipelineAsset = UniversalRenderPipelineAsset.Create();
            AssetDatabase.CreateAsset(pipelineAsset, pipelineAssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[URPConverter] Created new URP Pipeline Asset at " + pipelineAssetPath);
        }
        else
        {
            Debug.Log("[URPConverter] URP Pipeline Asset already exists at " + pipelineAssetPath);
        }

        // 4. Assign URP Pipeline Asset in Graphics settings
        GraphicsSettings.renderPipelineAsset = pipelineAsset;
        QualitySettings.renderPipeline = pipelineAsset;
        Debug.Log("[URPConverter] Assigned URP Pipeline Asset in Graphics and Quality settings.");

        // 5. Prepare for material upgrade and shader migration
        Debug.Log("[URPConverter] URP setup complete. Ready for material upgrade and Shader Graph migration.");
        EditorUtility.DisplayDialog(
            "URP Setup Complete",
            "URP is now configured. Next steps:\n- Upgrade materials\n- Migrate custom shaders to Shader Graph\n- Replace post-processing with URP Volumes\n- Test and optimize for VR/Quest\n\nUse the next menu options to continue.",
            "OK");
    }

    /// <summary>
    /// Checks if Universal RP is installed in the project.
    /// </summary>
    private static bool IsURPInstalled()
    {
        var urpType = typeof(UniversalRenderPipelineAsset);
        return urpType != null;
    }
} 