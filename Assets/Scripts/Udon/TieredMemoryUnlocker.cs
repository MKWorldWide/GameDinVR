// TieredMemoryUnlocker.cs
// Quantum-documented UdonSharp script for tier-based content activation
//
// Feature Context:
//   - Activates hidden content for players of sufficient tier
//   - Used for secrets, rewards, or advanced lore
//
// Dependencies:
//   - PlayerTierBridge
//   - UnityEngine, UdonSharp
//
// Usage Example:
//   Attach to content root; set required tier
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

public class TieredMemoryUnlocker : UdonSharpBehaviour
{
    [Header("Required Tier")]
    public int requiredTier = 2;
    public GameObject hiddenContent;
    private PlayerTierBridge tierBridge;

    void Start()
    {
        tierBridge = GetComponent<PlayerTierBridge>();
    }

    public void TryUnlock()
    {
        if (tierBridge != null && tierBridge.GetPlayerTier() >= requiredTier)
        {
            if (hiddenContent != null) hiddenContent.SetActive(true);
        }
    }
} 