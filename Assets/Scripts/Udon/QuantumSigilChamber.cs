// QuantumSigilChamber.cs
// Quantum-documented, modular system for sacred Citadel chambers sealed by memory orbs
// Unlocks when a specific set of GDIMemoryOrbs are activated by the player
// Supports tier override, door animation, VFX/SFX, and UnityEditor debugging
// Ready for multiplayer sync, UI, and future expansion

using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// QuantumSigilChamber: Sealed Citadel chamber that opens when required memory orbs are unlocked
/// </summary>
public class QuantumSigilChamber : UdonSharpBehaviour
{
    [Header("Required Orbs")]
    [Tooltip("List of GDIMemoryOrbs (by GameObject) required to unlock this chamber.")]
    public GDIMemoryOrb[] requiredOrbs;
    [Tooltip("Optional: Orb IDs for more advanced setups.")]
    public string[] requiredOrbIDs;

    [Header("Tier Override")]
    [Tooltip("If set, this tier can bypass orb check and unlock chamber directly.")]
    public string overrideTier = "";
    [Tooltip("Current player tier (set by session/auth system)")]
    public string playerTier = "Wanderer";

    [Header("Chamber Door & Effects")]
    [Tooltip("Door GameObject or Animator to open on unlock.")]
    public Animator doorAnimator;
    [Tooltip("Animation trigger name to open door.")]
    public string openTrigger = "Open";
    [Tooltip("Sigil VFX to play on unlock.")]
    public GameObject sigilVFX;
    [Tooltip("Resonance SFX to play on unlock.")]
    public AudioClip resonanceSFX;
    [Tooltip("Optional: UnityEvent for story/UI triggers.")]
    public UnityEvent onChamberUnlocked;

    [Header("Debug & Editor Tools")]
    [Tooltip("Show debug info in the Editor.")]
    public bool showDebug = true;
    [Tooltip("Editor-only: Tag for visual debugging.")]
    public string chamberTag = "QuantumSigilChamber";

    // --- Internal state ---
    private bool unlocked = false;

    // --- Tier mapping for override ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    /// <summary>
    /// Call to check if the chamber should unlock (e.g., after orb unlock or tier change)
    /// </summary>
    public void CheckUnlock()
    {
        if (unlocked) return;
        if (!string.IsNullOrEmpty(overrideTier) && MeetsTier(playerTier, overrideTier))
        {
            UnlockChamber();
            return;
        }
        if (requiredOrbs != null && requiredOrbs.Length > 0)
        {
            foreach (var orb in requiredOrbs)
            {
                if (!orb || !orb.IsUnlocked())
                {
                    if (showDebug) Debug.Log($"[QuantumSigilChamber] Orb not yet unlocked: {orb}");
                    return;
                }
            }
        }
        // (Optional) Check requiredOrbIDs if used
        // All orbs unlocked!
        UnlockChamber();
    }

    /// <summary>
    /// Unlocks the chamber: animates door, plays VFX/SFX, triggers events
    /// </summary>
    private void UnlockChamber()
    {
        if (unlocked) return;
        unlocked = true;
        if (showDebug) Debug.Log($"[QuantumSigilChamber] Chamber unlocked!");
        if (doorAnimator && !string.IsNullOrEmpty(openTrigger))
            doorAnimator.SetTrigger(openTrigger);
        if (sigilVFX) sigilVFX.SetActive(true);
        if (resonanceSFX) AudioSource.PlayClipAtPoint(resonanceSFX, transform.position);
        onChamberUnlocked?.Invoke();
    }

    /// <summary>
    /// Helper: Check if player meets or exceeds a tier
    /// </summary>
    private bool MeetsTier(string playerTier, string requiredTier)
    {
        int p = GDILevels.ContainsKey(playerTier) ? GDILevels[playerTier] : 0;
        int r = GDILevels.ContainsKey(requiredTier) ? GDILevels[requiredTier] : 0;
        return p >= r;
    }

    /// <summary>
    /// Editor-only: Tag for visual debugging
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        Gizmos.color = unlocked ? Color.green : Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 3f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, chamberTag);
    }
} 