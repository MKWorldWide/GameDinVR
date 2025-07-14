// TieredMemoryUnlocker.cs
// Quantum-documented Unity/UdonSharp component for tier-based content unlocking
// Activates lore objects, story paths, or hidden rooms based on the player's verified GDI tier
// Modular, extendable, and ready for future expansion (e.g., dynamic content, achievements)

using UdonSharp;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Activates lore objects, story paths, or hidden rooms based on the player's verified GDI tier.
/// Attach to a GameObject and assign unlockable objects per tier.
/// </summary>
public class TieredMemoryUnlocker : UdonSharpBehaviour
{
    [Header("Tier Unlock Mapping")]
    [Tooltip("Objects unlocked for Wanderer tier and above.")]
    public GameObject[] wandererUnlocks;
    [Tooltip("Objects unlocked for Initiate tier and above.")]
    public GameObject[] initiateUnlocks;
    [Tooltip("Objects unlocked for Radiant tier and above.")]
    public GameObject[] radiantUnlocks;
    [Tooltip("Objects unlocked for Sovereign tier and above.")]
    public GameObject[] sovereignUnlocks;

    [Header("Tier Source")]
    [Tooltip("Verified player GDI tier (set by auth/session system)")]
    public string playerTier = "Wanderer";

    // --- Internal tier mapping ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    /// <summary>
    /// Call this to unlock content based on the current player tier.
    /// </summary>
    public void UnlockForTier()
    {
        int tierLevel = GDILevels.ContainsKey(playerTier) ? GDILevels[playerTier] : 0;
        // Wanderer and above
        SetActiveForArray(wandererUnlocks, tierLevel >= 0);
        // Initiate and above
        SetActiveForArray(initiateUnlocks, tierLevel >= 1);
        // Radiant and above
        SetActiveForArray(radiantUnlocks, tierLevel >= 2);
        // Sovereign only
        SetActiveForArray(sovereignUnlocks, tierLevel >= 3);
    }

    /// <summary>
    /// Helper to activate/deactivate all objects in an array.
    /// </summary>
    private void SetActiveForArray(GameObject[] arr, bool active)
    {
        if (arr == null) return;
        foreach (var go in arr)
        {
            if (go) go.SetActive(active);
        }
    }

    /// <summary>
    /// Optionally call this on Start or after player tier is set/verified.
    /// </summary>
    private void Start()
    {
        UnlockForTier();
    }
} 