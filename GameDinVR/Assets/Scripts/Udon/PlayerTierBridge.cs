// PlayerTierBridge.cs
// Quantum-documented bridge for fetching and caching GDI tier from GameDin backend
// Modular, supports REST fetch, caching, and mock fallback
// Exposes public method to get current tier by address
// Compatible with UdonSharp or standard Unity MonoBehaviour

using UdonSharp;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// Fetches and caches a user's GDI tier from the GameDin backend via REST.
/// Supports mock fallback and exposes a public method to get the current tier by address.
/// </summary>
public class PlayerTierBridge : UdonSharpBehaviour
{
    [Header("Backend Settings")]
    [Tooltip("Base URL for GDI tier API (e.g., https://api.gamedin.world)")]
    public string apiBaseUrl = "https://api.gamedin.world";
    [Tooltip("Enable debug logs.")]
    public bool IsDebugMode = false;

    [Header("Mock Fallback")]
    [Tooltip("If true, use mock tier if fetch fails or for testing.")]
    public bool useMockTier = true;
    [Tooltip("Mock tier to use if backend is unavailable.")]
    public string mockTier = "Initiate";

    // --- Internal cache: address -> tier ---
    private Dictionary<string, string> tierCache = new Dictionary<string, string>();
    private HashSet<string> fetching = new HashSet<string>();

    /// <summary>
    /// Public method to get the current tier for a given address.
    /// If not cached, triggers a fetch and returns mock or null until complete.
    /// </summary>
    public string GetTier(string address)
    {
        if (string.IsNullOrEmpty(address)) return useMockTier ? mockTier : null;
        if (tierCache.ContainsKey(address)) return tierCache[address];
        if (!fetching.Contains(address))
        {
            StartCoroutine(FetchTierCoroutine(address));
        }
        return useMockTier ? mockTier : null;
    }

    /// <summary>
    /// Coroutine to fetch tier from backend and cache it.
    /// </summary>
    private IEnumerator FetchTierCoroutine(string address)
    {
        fetching.Add(address);
        string url = $"{apiBaseUrl}/api/gdi/tier/{address}";
        if (IsDebugMode) Debug.Log($"[PlayerTierBridge] Fetching tier for {address} from {url}");
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                if (IsDebugMode) Debug.LogWarning($"[PlayerTierBridge] Tier fetch failed for {address}: {req.error}");
                if (useMockTier) tierCache[address] = mockTier;
            }
            else
            {
                string tier = req.downloadHandler.text.Trim();
                tierCache[address] = tier;
                if (IsDebugMode) Debug.Log($"[PlayerTierBridge] Tier for {address}: {tier}");
            }
        }
        fetching.Remove(address);
    }

    /// <summary>
    /// Optionally clear the cache (for testing or refresh).
    /// </summary>
    public void ClearCache()
    {
        tierCache.Clear();
        fetching.Clear();
    }
} 