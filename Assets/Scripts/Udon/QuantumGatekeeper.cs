// QuantumGatekeeper.cs
// Quantum-documented UdonSharp script for tiered portal logic in GameDinVR
//
// Feature Context:
//   - Controls portal activation, tier checks, and VFX
//   - Central to world navigation and access control
//
// Dependencies:
//   - PlayerTierBridge (for tier data)
//   - QuantumSessionVerifier (for authentication)
//   - UnityEngine, UdonSharp
//
// Usage Example:
//   Attach to portal prefab. Configure tier requirements in Inspector.
//
// Performance:
//   - Optimized for minimal per-frame checks
//
// Security:
//   - All checks are local; no sensitive data stored
//
// Changelog:
//   - v1.0.0: Initial quantum template version

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class QuantumGatekeeper : UdonSharpBehaviour
{
    [Header("Tier Requirement")]
    public int requiredTier = 1;
    [Header("Portal VFX")] public GameObject portalVFX;
    [Header("Denied VFX")] public GameObject deniedVFX;
    [Header("Destination")] public Transform destination;

    private PlayerTierBridge tierBridge;
    private QuantumSessionVerifier sessionVerifier;

    void Start()
    {
        tierBridge = GetComponent<PlayerTierBridge>();
        sessionVerifier = GetComponent<QuantumSessionVerifier>();
    }

    public override void Interact()
    {
        if (sessionVerifier != null && !sessionVerifier.IsSessionValid())
        {
            DenyAccess();
            return;
        }
        if (tierBridge != null && tierBridge.GetPlayerTier() >= requiredTier)
        {
            AllowAccess();
        }
        else
        {
            DenyAccess();
        }
    }

    private void AllowAccess()
    {
        if (portalVFX != null) portalVFX.SetActive(true);
        if (destination != null && Networking.LocalPlayer != null)
        {
            Networking.LocalPlayer.TeleportTo(destination.position, destination.rotation);
        }
    }

    private void DenyAccess()
    {
        if (deniedVFX != null) deniedVFX.SetActive(true);
        // Optionally play denial sound or feedback
    }
} 