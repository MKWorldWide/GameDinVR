// TokenStatusDisplay.cs
// UdonSharp script for simulating wallet/token status on the GDI Hologram Panel
// Replace with real data integration later

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simulates token count and login state for the GDI Hologram Panel.
/// Replace with real wallet integration in the future.
/// </summary>
public class TokenStatusDisplay : UdonSharpBehaviour
{
    [Header("UI References")]
    public Text tokenCountText;
    public Text loginStateText;

    [Header("Simulated Data")]
    public int fakeTokenCount = 42;
    public string fakeLoginState = "Guest";

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (tokenCountText != null)
            tokenCountText.text = $"Tokens: {fakeTokenCount}";
        if (loginStateText != null)
            loginStateText.text = $"Status: {fakeLoginState}";
    }
} 