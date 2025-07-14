// GDIAccessGate.cs
// Quantum-documented access control for sacred Citadel zones
// Enforces GDI tier-based entry using BoxCollider triggers
// Now supports live GDI tier sync from backend (MKZenith) using UnityWebRequest
// Modular, extendable, and ready for WalletConnect/Web3
// Compatible with UdonSharp (VRChat) or standard Unity C#

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Enforces access control based on a player's GDI Tier, synced live from backend.
/// Attach to BoxCollider (isTrigger) in sacred Citadel areas.
/// </summary>
public class GDIAccessGate : UdonSharpBehaviour
{
    // --- Public Fields ---
    [Header("Access Control")]
    [Tooltip("Minimum GDI Tier required to enter (Wanderer, Initiate, Radiant, Sovereign)")]
    public string requiredTier = "Radiant";

    [Header("Backend Integration")]
    [Tooltip("Player wallet or address to check against backend.")]
    public string playerAddress = "0x000...";
    [Tooltip("If true, auto-fetch tier on Start.")]
    public bool autoResolveOnStart = true;
    [Tooltip("Enable debug logs.")]
    public bool IsDebugMode = false;

    [Header("Feedback FX/SFX")]
    public GameObject successVFX;
    public GameObject denialVFX;
    public AudioClip denialSound;

    // --- Internal Tier Mapping ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    // --- Cached Tier ---
    private string cachedTier = null;
    private bool isFetching = false;
    private bool fetchFailed = false;

    // --- Unity Start: Optionally auto-fetch tier ---
    private void Start()
    {
        if (autoResolveOnStart && !string.IsNullOrEmpty(playerAddress))
        {
            StartCoroutine(FetchTierCoroutine(playerAddress));
        }
    }

    // --- OnTriggerEnter: Enforce Access with live tier ---
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (string.IsNullOrEmpty(playerAddress))
        {
            ShowFloatingMessage("No player address set");
            if (IsDebugMode) Debug.LogWarning("[GDIAccessGate] No player address set.");
            return;
        }
        if (isFetching)
        {
            ShowFloatingMessage("Checking access...");
            if (IsDebugMode) Debug.Log("[GDIAccessGate] Still fetching tier...");
            return;
        }
        if (cachedTier == null && !fetchFailed)
        {
            StartCoroutine(FetchTierCoroutine(playerAddress, () => CheckAccess(player)));
            ShowFloatingMessage("Checking access...");
            return;
        }
        CheckAccess(player);
    }

    // --- Check Access Logic ---
    private void CheckAccess(VRCPlayerApi player)
    {
        string playerTier = cachedTier ?? "Wanderer";
        int playerLevel = GDILevels.ContainsKey(playerTier) ? GDILevels[playerTier] : 0;
        int requiredLevel = GDILevels.ContainsKey(requiredTier) ? GDILevels[requiredTier] : 0;
        if (IsDebugMode) Debug.Log($"[GDIAccessGate] Player tier: {playerTier} (lvl {playerLevel}), Required: {requiredTier} (lvl {requiredLevel})");
        if (playerLevel >= requiredLevel)
        {
            PlaySuccessFX();
        }
        else
        {
            DenyEntry(player, playerTier);
        }
    }

    // --- Coroutine: Fetch Tier from Backend ---
    private IEnumerator FetchTierCoroutine(string address, System.Action onComplete = null)
    {
        isFetching = true;
        string url = $"/api/gdi/tier/{address}";
        if (IsDebugMode) Debug.Log($"[GDIAccessGate] Fetching GDI tier from: {url}");
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                if (IsDebugMode) Debug.LogWarning($"[GDIAccessGate] Tier fetch failed: {req.error}");
                fetchFailed = true;
                ShowFloatingMessage("Tier fetch failed");
                isFetching = false;
                yield break;
            }
            // Assume backend returns plain tier string (e.g., "Radiant")
            cachedTier = req.downloadHandler.text.Trim();
            if (IsDebugMode) Debug.Log($"[GDIAccessGate] Tier fetched: {cachedTier}");
        }
        isFetching = false;
        onComplete?.Invoke();
    }

    // --- Deny Entry: Push back, play SFX/VFX, show message ---
    private void DenyEntry(VRCPlayerApi player, string playerTier)
    {
        if (player.isLocal)
        {
            Vector3 pushDir = (player.GetPosition() - transform.position).normalized;
            player.TeleportTo(player.GetPosition() + pushDir * 2f, player.GetRotation());
        }
        if (denialVFX) denialVFX.SetActive(true);
        if (denialSound)
        {
            AudioSource.PlayClipAtPoint(denialSound, transform.position);
        }
        ShowFloatingMessage($"Access Denied – {requiredTier} Required (You: {playerTier})");
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

    // --- Future: Replace with WalletConnect/Web3 auth, real GDI API, and world-space UI ---
} 