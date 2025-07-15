// GDIMemoryOrb.cs
// Quantum-documented UdonSharp script for lore/secret unlocks
//
// Feature Context:
//   - Unlocks lore or secrets when interacted with
//   - Used for worldbuilding and player engagement
//
// Dependencies:
//   - UnityEngine, UdonSharp
//
// Usage Example:
//   Attach to orb prefab; configure lore text
//
// Performance:
//   - Lightweight; triggers on interact
//
// Security:
//   - No sensitive data
//
// Changelog:
//   - v1.0.0: Initial quantum template version

using UdonSharp;
using UnityEngine;

public class GDIMemoryOrb : UdonSharpBehaviour
{
    [Header("Lore Text")]
    public string loreText = "Secret Lore";
    public GameObject visualEffect;

    public override void Interact()
    {
        if (visualEffect != null) visualEffect.SetActive(true);
        // Display lore text to player (UI integration needed)
        Debug.Log($"Lore Unlocked: {loreText}");
    }
} 