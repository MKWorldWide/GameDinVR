// GDIAccessGate.cs
// Quantum-documented access control for sacred Citadel zones
// Enforces GDI tier-based entry using BoxCollider triggers
// Modular, extendable, and ready for real GDI backend integration
// Compatible with UdonSharp (VRChat) or standard Unity C#

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;

/// <summary>
/// Enforces access control based on a player's GDI Tier.
/// Attach to BoxCollider (isTrigger) in sacred Citadel areas.
/// </summary>
public class GDIAccessGate : UdonSharpBehaviour
{
    // --- Public Fields ---
    [Header("Access Control")]
    [Tooltip("Minimum GDI Tier required to enter (Wanderer, Initiate, Radiant, Sovereign)")]
    public string requiredTier = "Radiant";

    [Header("Mock Player Tier (for testing)")]
    [Tooltip("Simulated player GDI tier. Replace with real user data in production.")]
    public string mockPlayerTier = "Initiate";

    [Header("Feedback FX/SFX")]
    public GameObject successVFX;
    public GameObject denialVFX;
    public AudioClip denialSound;

    // --- Internal Tier Mapping ---
    private static readonly Dictionary<string, int> tierStrength = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    // --- OnTriggerEnter: Enforce Access ---
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // --- Simulate player tier (replace with real lookup later) ---
        string playerTier = mockPlayerTier;
        int playerLevel = tierStrength.ContainsKey(playerTier) ? tierStrength[playerTier] : 0;
        int requiredLevel = tierStrength.ContainsKey(requiredTier) ? tierStrength[requiredTier] : 0;

        if (playerLevel >= requiredLevel)
        {
            // --- Access granted ---
            PlaySuccessFX();
            // (Extend: allow teleport, open gate, etc)
        }
        else
        {
            // --- Access denied ---
            DenyEntry(player);
        }
    }

    // --- Deny Entry: Push back, play SFX/VFX, show message ---
    private void DenyEntry(VRCPlayerApi player)
    {
        // Push player back slightly (if local)
        if (player.isLocal)
        {
            Vector3 pushDir = (player.GetPosition() - transform.position).normalized;
            player.TeleportTo(player.GetPosition() + pushDir * 2f, player.GetRotation());
        }
        // Play denial VFX
        if (denialVFX) denialVFX.SetActive(true);
        // Play denial sound
        if (denialSound)
        {
            AudioSource.PlayClipAtPoint(denialSound, transform.position);
        }
        // Show floating message (mock)
        ShowFloatingMessage($"Access Denied: {requiredTier} Tier Required");
    }

    // --- Play Success VFX (optional) ---
    private void PlaySuccessFX()
    {
        if (successVFX) successVFX.SetActive(true);
        // (Extend: play sound, open gate, etc)
    }

    // --- Show Floating Message (mock, replace with UI system) ---
    private void ShowFloatingMessage(string msg)
    {
        // (Mock: log to console, replace with in-world UI popup)
        Debug.Log($"[GDIAccessGate] {msg}");
    }

    // --- Future: Replace mockPlayerTier with real GDI API lookup ---
    // e.g., query MKZenith backend for authenticated user tier
} 