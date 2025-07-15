// DestinationPreviewRenderer.cs
// Quantum-documented UdonSharp script for real-time portal destination previews
//
// Feature Context:
//   - Renders a preview of the portal destination for the player
//   - Used by QuantumGatekeeper and portal prefabs
//
// Dependencies:
//   - UnityEngine, UdonSharp
//
// Usage Example:
//   Attach to portal prefab; assign preview camera and render texture
//
// Performance:
//   - Uses Unity's RenderTexture for efficiency
//
// Security:
//   - No sensitive data
//
// Changelog:
//   - v1.0.0: Initial quantum template version

using UdonSharp;
using UnityEngine;

public class DestinationPreviewRenderer : UdonSharpBehaviour
{
    [Header("Preview Camera")]
    public Camera previewCamera;
    [Header("Render Texture")]
    public RenderTexture previewTexture;
    [Header("Preview UI")]
    public GameObject previewUI;

    void Start()
    {
        if (previewCamera != null && previewTexture != null)
        {
            previewCamera.targetTexture = previewTexture;
        }
    }

    public void ShowPreview()
    {
        if (previewUI != null) previewUI.SetActive(true);
    }

    public void HidePreview()
    {
        if (previewUI != null) previewUI.SetActive(false);
    }
} 