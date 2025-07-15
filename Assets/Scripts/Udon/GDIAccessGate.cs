// GDIAccessGate.cs
// Quantum-documented UdonSharp script for basic tier-based access control
//
// Feature Context:
//   - Restricts access to sacred zones based on player tier
//   - Used in lore chambers, council rooms, etc.
//
// Dependencies:
//   - PlayerTierBridge
//   - UnityEngine, UdonSharp
//
// Usage Example:
//   Attach to zone collider or trigger
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

public class GDIAccessGate : UdonSharpBehaviour
{
    [Header("Required Tier")]
    public int requiredTier = 2;
    private PlayerTierBridge tierBridge;

    void Start()
    {
        tierBridge = GetComponent<PlayerTierBridge>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tierBridge != null && tierBridge.GetPlayerTier() < requiredTier)
        {
            // Deny access: teleport out or show feedback
            other.gameObject.SetActive(false); // Example: disable player object
        }
    }
} 