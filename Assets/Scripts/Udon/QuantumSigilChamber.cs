// QuantumSigilChamber.cs
// Quantum-documented UdonSharp script for unlockable chambers by tier
//
// Feature Context:
//   - Unlocks special rooms or content for high-tier players
//   - Used for lore, secrets, or rewards
//
// Dependencies:
//   - PlayerTierBridge
//   - UnityEngine, UdonSharp
//
// Usage Example:
//   Attach to chamber door or trigger
//
// Performance:
//   - Minimal overhead; checks on trigger
//
// Security:
//   - Local only; no sensitive data
//
// Changelog:
//   - v1.0.0: Initial quantum template version

using UdonSharp;
using UnityEngine;

public class QuantumSigilChamber : UdonSharpBehaviour
{
    [Header("Required Tier")]
    public int requiredTier = 3;
    public GameObject chamberDoor;
    private PlayerTierBridge tierBridge;

    void Start()
    {
        tierBridge = GetComponent<PlayerTierBridge>();
    }

    public void TryUnlock()
    {
        if (tierBridge != null && tierBridge.GetPlayerTier() >= requiredTier)
        {
            if (chamberDoor != null) chamberDoor.SetActive(false); // Open door
        }
    }
} 