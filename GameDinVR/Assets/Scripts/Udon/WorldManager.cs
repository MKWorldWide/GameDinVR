// WorldManager.cs
// UdonSharp script for world initialization and global logic
// Controls startup flow, lighting, and future expansion hooks

using UdonSharp;
using UnityEngine;

/// <summary>
/// Manages world startup, lighting, and global state.
/// Expand to control music, events, and world logic.
/// </summary>
public class WorldManager : UdonSharpBehaviour
{
    [Header("Lighting")]
    public Light mainLight;
    public Color divineColor = new Color(0.8f, 0.9f, 1f, 1f);
    public Color shadowColor = new Color(0.1f, 0.1f, 0.2f, 1f);

    private void Start()
    {
        // Set initial lighting mood
        if (mainLight != null)
        {
            mainLight.color = divineColor;
            mainLight.intensity = 1.2f;
        }
        RenderSettings.ambientLight = shadowColor;
    }
} 