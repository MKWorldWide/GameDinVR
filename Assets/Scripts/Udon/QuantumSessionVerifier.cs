// QuantumSessionVerifier.cs
// Quantum-documented Unity authentication module for GameDin session validation
// Validates a player's session token against the backend, fetches address, and triggers UnityEvents
// Modular, extendable, and ready for future auth expansion
// Compatible with UdonSharp or standard Unity MonoBehaviour

using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Validates a player's GameDin session token against the backend.
/// Exposes public methods to verify token, fetch address, and trigger UnityEvents for onSuccess/onFailure.
/// </summary>
public class QuantumSessionVerifier : UdonSharpBehaviour
{
    [Header("Backend Settings")]
    [Tooltip("Base URL for GameDin session API (e.g., https://api.gamedin.world)")]
    public string apiBaseUrl = "https://api.gamedin.world";
    [Tooltip("Enable debug logs.")]
    public bool IsDebugMode = false;

    [Header("Session State")]
    [Tooltip("Current session token to verify.")]
    public string sessionToken;
    [Tooltip("Fetched player address after verification.")]
    public string playerAddress;
    [Tooltip("Last verification result.")]
    public bool isSessionValid = false;

    [Header("Events")]
    public UnityEvent onSuccess;
    public UnityEvent onFailure;

    /// <summary>
    /// Public method to verify the current session token.
    /// </summary>
    public void VerifySession()
    {
        if (string.IsNullOrEmpty(sessionToken))
        {
            if (IsDebugMode) Debug.LogWarning("[QuantumSessionVerifier] No session token set.");
            onFailure?.Invoke();
            return;
        }
        StartCoroutine(VerifySessionCoroutine(sessionToken));
    }

    /// <summary>
    /// Public method to fetch the player address (after successful verification).
    /// </summary>
    public string GetPlayerAddress()
    {
        return playerAddress;
    }

    /// <summary>
    /// Coroutine to verify session token and fetch address from backend.
    /// </summary>
    private IEnumerator VerifySessionCoroutine(string token)
    {
        string url = $"{apiBaseUrl}/api/gdi/session/verify";
        if (IsDebugMode) Debug.Log($"[QuantumSessionVerifier] Verifying session at: {url}");
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes($"{\"token\":\"{token}\"}");
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            if (IsDebugMode) Debug.LogWarning($"[QuantumSessionVerifier] Session verification failed: {req.error}");
            isSessionValid = false;
            playerAddress = null;
            onFailure?.Invoke();
        }
        else
        {
            // Assume backend returns JSON: { "valid": true, "address": "0x..." }
            string json = req.downloadHandler.text;
            bool valid = false;
            string address = null;
            try
            {
                var parsed = JsonUtility.FromJson<SessionResponse>(json);
                valid = parsed.valid;
                address = parsed.address;
            }
            catch
            {
                if (IsDebugMode) Debug.LogWarning($"[QuantumSessionVerifier] Failed to parse session response: {json}");
            }
            isSessionValid = valid;
            playerAddress = address;
            if (valid)
            {
                if (IsDebugMode) Debug.Log($"[QuantumSessionVerifier] Session valid. Address: {address}");
                onSuccess?.Invoke();
            }
            else
            {
                if (IsDebugMode) Debug.LogWarning("[QuantumSessionVerifier] Session invalid.");
                onFailure?.Invoke();
            }
        }
    }

    // --- Helper class for JSON parsing ---
    [System.Serializable]
    private class SessionResponse
    {
        public bool valid;
        public string address;
    }
} 