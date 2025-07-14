// GDIMemoryOrb.cs
// Quantum-documented, modular prefab component for GDI Memory Orbs in the Citadel
// Each orb hovers, rotates, and unlocks lore/secrets/energy based on the player's GDI tier
// Integrates with TieredMemoryUnlocker and QuantumSessionVerifier for immersive, tiered narrative
// Supports SFX, VFX, haptics, and floating subtitle/lore message
// Ready for VR interaction, achievements, and future expansion

using UdonSharp;
using UnityEngine;
using TMPro;

/// <summary>
/// GDI Memory Orb: Interactive, tiered lore orb for the Citadel of Resonance
/// Hovers, rotates, and reveals secrets based on player GDI tier
/// </summary>
public class GDIMemoryOrb : UdonSharpBehaviour
{
    [Header("Orb Visuals & Animation")]
    [Tooltip("The glowing orb mesh or root GameObject.")]
    public GameObject orbVisual;
    [Tooltip("Rotation speed (degrees/sec)")]
    public float rotationSpeed = 20f;
    [Tooltip("Hover amplitude (units)")]
    public float hoverAmplitude = 0.2f;
    [Tooltip("Hover frequency (Hz)")]
    public float hoverFrequency = 0.5f;

    [Header("Lore & Tier Unlocks")]
    [Tooltip("TieredMemoryUnlocker for this orb's secrets.")]
    public TieredMemoryUnlocker tierUnlocker;
    [Tooltip("Optional: QuantumSessionVerifier for live tier injection.")]
    public QuantumSessionVerifier sessionVerifier;
    [Tooltip("Lore/secret text to display per tier.")]
    public string wandererLore = "A faint memory stirs...";
    public string initiateLore = "The path of Initiate glimmers.";
    public string radiantLore = "Radiant truth pulses within.";
    public string sovereignLore = "Sovereign power awakens the core.";

    [Header("Feedback & Effects")]
    public AudioClip unlockSFX;
    public GameObject unlockVFX;
    [Tooltip("Optional: Haptic feedback (future VR)")]
    public bool playHaptics = false;

    [Header("Subtitle/Lore UI")]
    [Tooltip("3D Text or Canvas for floating lore message.")]
    public TextMeshPro subtitleText;
    [Tooltip("Show subtitle on unlock?")]
    public bool showSubtitleOnUnlock = true;

    [Header("Interaction")]
    [Tooltip("Auto-trigger on proximity (true) or require OnInteract() call (false)")]
    public bool autoTrigger = true;
    [Tooltip("Minimum tier required to interact (optional)")]
    public string minInteractTier = "Wanderer";

    // --- Internal state ---
    private Vector3 basePosition;
    private bool unlocked = false;

    // --- Quantum Documentation: Initialization ---
    private void Start()
    {
        if (orbVisual) basePosition = orbVisual.transform.position;
        if (autoTrigger) TryUnlock();
    }

    // --- Quantum Documentation: Hover & Rotate Animation ---
    private void Update()
    {
        if (orbVisual)
        {
            float hover = Mathf.Sin(Time.time * hoverFrequency * 2 * Mathf.PI) * hoverAmplitude;
            orbVisual.transform.position = basePosition + Vector3.up * hover;
            orbVisual.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    /// <summary>
    /// Attempt to unlock the orb's secrets based on player tier.
    /// </summary>
    public void TryUnlock()
    {
        if (unlocked) return;
        string playerTier = tierUnlocker ? tierUnlocker.playerTier : "Wanderer";
        // Optionally fetch from QuantumSessionVerifier if present
        if (sessionVerifier && sessionVerifier.isSessionValid)
        {
            playerTier = sessionVerifier.GetPlayerAddress(); // Replace with actual tier fetch if available
        }
        // Check if player meets minInteractTier
        if (!MeetsTier(playerTier, minInteractTier)) return;
        // Unlock content for this tier
        if (tierUnlocker) tierUnlocker.playerTier = playerTier;
        if (tierUnlocker) tierUnlocker.UnlockForTier();
        // Play feedback
        PlayUnlockFX();
        // Show subtitle/lore
        if (showSubtitleOnUnlock && subtitleText)
        {
            subtitleText.text = GetLoreForTier(playerTier);
            subtitleText.gameObject.SetActive(true);
        }
        unlocked = true;
    }

    /// <summary>
    /// OnInteract: Call to trigger unlock (for VR or manual interaction)
    /// </summary>
    public override void Interact()
    {
        TryUnlock();
    }

    // --- Helper: Play SFX/VFX/Haptics ---
    private void PlayUnlockFX()
    {
        if (unlockSFX) AudioSource.PlayClipAtPoint(unlockSFX, transform.position);
        if (unlockVFX) unlockVFX.SetActive(true);
        if (playHaptics) { /* TODO: Integrate VR haptics */ }
    }

    // --- Helper: Get Lore Text for Tier ---
    private string GetLoreForTier(string tier)
    {
        switch (tier)
        {
            case "Sovereign": return sovereignLore;
            case "Radiant": return radiantLore;
            case "Initiate": return initiateLore;
            default: return wandererLore;
        }
    }

    // --- Helper: Tier comparison ---
    private bool MeetsTier(string playerTier, string requiredTier)
    {
        var map = new System.Collections.Generic.Dictionary<string, int> { {"Wanderer",0},{"Initiate",1},{"Radiant",2},{"Sovereign",3} };
        int p = map.ContainsKey(playerTier) ? map[playerTier] : 0;
        int r = map.ContainsKey(requiredTier) ? map[requiredTier] : 0;
        return p >= r;
    }
} 