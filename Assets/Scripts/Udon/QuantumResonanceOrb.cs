// QuantumResonanceOrb.cs
// Quantum-documented resonance orb system for The Citadel of Resonance
// Creates interactive orbs that respond to player proximity and GDI tier
// Features visual effects, audio resonance, and tier-based interactions
// Modular, extendable, and ready for VRChat multiplayer sync
// Compatible with UdonSharp (VRChat) or standard Unity C#

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interactive resonance orbs that respond to player proximity and GDI tier.
/// Creates immersive environmental interactions with visual and audio feedback.
/// </summary>
public class QuantumResonanceOrb : UdonSharpBehaviour
{
    // --- Resonance Configuration ---
    [Header("Resonance Settings")]
    [Tooltip("Base resonance frequency (affects audio pitch)")]
    public float baseFrequency = 440f; // A4 note
    [Tooltip("Resonance intensity multiplier")]
    public float resonanceIntensity = 1.0f;
    [Tooltip("Maximum resonance range")]
    public float maxResonanceRange = 10f;
    [Tooltip("Resonance falloff curve")]
    public AnimationCurve resonanceFalloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Tier Interaction")]
    [Tooltip("Minimum GDI tier required for full resonance")]
    public string minimumTier = "Wanderer";
    [Tooltip("Tier-based resonance multiplier")]
    public float tierResonanceMultiplier = 1.5f;
    [Tooltip("Enable tier-based color changes")]
    public bool enableTierColors = true;

    // --- Visual Effects ---
    [Header("Visual Effects")]
    [Tooltip("Orb mesh renderer")]
    public Renderer orbRenderer;
    [Tooltip("Resonance particle system")]
    public ParticleSystem resonanceParticles;
    [Tooltip("Glow material for tier-based coloring")]
    public Material glowMaterial;
    [Tooltip("Pulse animation speed")]
    public float pulseSpeed = 2f;
    [Tooltip("Pulse intensity")]
    public float pulseIntensity = 0.3f;

    // --- Audio Effects ---
    [Header("Audio Effects")]
    [Tooltip("Resonance audio source")]
    public AudioSource resonanceAudio;
    [Tooltip("Base resonance tone")]
    public AudioClip resonanceTone;
    [Tooltip("Tier-based harmonic overlay")]
    public AudioClip tierHarmonic;
    [Tooltip("Audio spatial blend (0=2D, 1=3D)")]
    public float spatialBlend = 1.0f;
    [Tooltip("Maximum audio volume")]
    public float maxVolume = 0.8f;

    // --- Interaction ---
    [Header("Interaction")]
    [Tooltip("Enable player interaction")]
    public bool enableInteraction = true;
    [Tooltip("Interaction cooldown in seconds")]
    public float interactionCooldown = 1.0f;
    [Tooltip("Enable haptic feedback")]
    public bool enableHaptics = true;
    [Tooltip("Haptic intensity")]
    public float hapticIntensity = 0.5f;

    // --- Events ---
    [Header("Unity Events")]
    [Tooltip("Triggered when player enters resonance range")]
    public UnityEvent onPlayerEntered;
    [Tooltip("Triggered when player exits resonance range")]
    public UnityEvent onPlayerExited;
    [Tooltip("Triggered on resonance interaction")]
    public UnityEvent onResonanceActivated;
    [Tooltip("Triggered when tier requirement is met")]
    public UnityEvent onTierRequirementMet;

    // --- Internal State ---
    private bool isResonating = false;
    private bool isInteracting = false;
    private float currentResonance = 0f;
    private float lastInteractionTime = 0f;
    private VRCPlayerApi currentPlayer;
    private string currentPlayerTier = "Wanderer";
    private Vector3 originalScale;
    private Color originalColor;
    private Coroutine pulseCoroutine;
    private Coroutine resonanceCoroutine;

    // --- Tier Level Mapping ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    // --- Unity Start: Initialize Resonance Orb ---
    private void Start()
    {
        InitializeResonanceOrb();
    }

    /// <summary>
    /// Initialize the resonance orb system.
    /// </summary>
    private void InitializeResonanceOrb()
    {
        // Store original properties
        if (orbRenderer != null)
        {
            originalScale = orbRenderer.transform.localScale;
            originalColor = orbRenderer.material.color;
        }

        // Setup audio source
        if (resonanceAudio == null)
        {
            resonanceAudio = GetComponent<AudioSource>();
            if (resonanceAudio == null)
            {
                resonanceAudio = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configure audio source
        resonanceAudio.spatialBlend = spatialBlend;
        resonanceAudio.volume = 0f;
        resonanceAudio.loop = true;
        resonanceAudio.clip = resonanceTone;
        resonanceAudio.pitch = baseFrequency / 440f; // Normalize to A4

        // Start ambient pulse
        StartAmbientPulse();
    }

    /// <summary>
    /// Start ambient pulse animation.
    /// </summary>
    private void StartAmbientPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        pulseCoroutine = StartCoroutine(AmbientPulseCoroutine());
    }

    /// <summary>
    /// Ambient pulse coroutine for idle state.
    /// </summary>
    private IEnumerator AmbientPulseCoroutine()
    {
        while (true)
        {
            float pulse = 1f + pulseIntensity * 0.1f * Mathf.Sin(Time.time * pulseSpeed * 0.5f);
            if (orbRenderer != null)
            {
                orbRenderer.transform.localScale = originalScale * pulse;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // --- Player Proximity Detection ---
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!enableInteraction) return;

        currentPlayer = player;
        onPlayerEntered?.Invoke();

        if (player.isLocal)
        {
            StartResonance(player);
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!enableInteraction) return;

        onPlayerExited?.Invoke();

        if (player.isLocal)
        {
            StopResonance();
        }

        currentPlayer = null;
    }

    /// <summary>
    /// Start resonance interaction with player.
    /// </summary>
    private void StartResonance(VRCPlayerApi player)
    {
        if (isResonating) return;

        isResonating = true;
        GetPlayerTier(player);
        StartCoroutine(ResonanceCoroutine(player));
    }

    /// <summary>
    /// Stop resonance interaction.
    /// </summary>
    private void StopResonance()
    {
        isResonating = false;
        
        if (resonanceCoroutine != null)
        {
            StopCoroutine(resonanceCoroutine);
            resonanceCoroutine = null;
        }

        // Fade out audio
        StartCoroutine(FadeAudioCoroutine(0f, 1f));
        
        // Reset visual effects
        ResetVisualEffects();
    }

    /// <summary>
    /// Get player's GDI tier for resonance calculation.
    /// </summary>
    private void GetPlayerTier(VRCPlayerApi player)
    {
        // Try to get tier from existing systems
        var sessionVerifier = FindObjectOfType<QuantumSessionVerifier>();
        var tierBridge = FindObjectOfType<PlayerTierBridge>();
        
        if (sessionVerifier != null && tierBridge != null)
        {
            string playerAddress = sessionVerifier.GetPlayerAddress();
            if (!string.IsNullOrEmpty(playerAddress))
            {
                currentPlayerTier = tierBridge.GetTier(playerAddress);
            }
        }
        
        // Check if tier requirement is met
        CheckTierRequirement();
    }

    /// <summary>
    /// Check if player meets tier requirement for enhanced resonance.
    /// </summary>
    private void CheckTierRequirement()
    {
        int playerLevel = GDILevels.ContainsKey(currentPlayerTier) ? GDILevels[currentPlayerTier] : 0;
        int requiredLevel = GDILevels.ContainsKey(minimumTier) ? GDILevels[minimumTier] : 0;
        
        if (playerLevel >= requiredLevel)
        {
            onTierRequirementMet?.Invoke();
            UpdateTierColors(true);
        }
        else
        {
            UpdateTierColors(false);
        }
    }

    /// <summary>
    /// Update visual colors based on tier interaction.
    /// </summary>
    private void UpdateTierColors(bool tierMet)
    {
        if (!enableTierColors || orbRenderer == null) return;

        Color tierColor = GetTierColor(currentPlayerTier);
        if (tierMet)
        {
            tierColor *= tierResonanceMultiplier;
        }
        
        orbRenderer.material.color = Color.Lerp(originalColor, tierColor, 0.7f);
        
        if (glowMaterial != null)
        {
            glowMaterial.SetColor("_EmissionColor", tierColor * (tierMet ? 1.5f : 0.5f));
        }
    }

    /// <summary>
    /// Get color associated with a GDI tier.
    /// </summary>
    private Color GetTierColor(string tier)
    {
        switch (tier)
        {
            case "Wanderer": return new Color(0.5f, 0.5f, 0.5f, 1.0f); // Gray
            case "Initiate": return new Color(0.2f, 0.8f, 0.2f, 1.0f); // Green
            case "Radiant": return new Color(0.8f, 0.8f, 0.2f, 1.0f); // Gold
            case "Sovereign": return new Color(0.8f, 0.2f, 0.8f, 1.0f); // Purple
            default: return Color.white;
        }
    }

    /// <summary>
    /// Main resonance coroutine for player interaction.
    /// </summary>
    private IEnumerator ResonanceCoroutine(VRCPlayerApi player)
    {
        while (isResonating && player != null)
        {
            // Calculate distance-based resonance
            float distance = Vector3.Distance(transform.position, player.GetPosition());
            float normalizedDistance = Mathf.Clamp01(distance / maxResonanceRange);
            float resonance = resonanceFalloff.Evaluate(1f - normalizedDistance);
            
            // Apply tier multiplier
            int playerLevel = GDILevels.ContainsKey(currentPlayerTier) ? GDILevels[currentPlayerTier] : 0;
            int requiredLevel = GDILevels.ContainsKey(minimumTier) ? GDILevels[minimumTier] : 0;
            float tierMultiplier = playerLevel >= requiredLevel ? tierResonanceMultiplier : 1f;
            
            currentResonance = resonance * resonanceIntensity * tierMultiplier;
            
            // Update visual effects
            UpdateVisualEffects(currentResonance);
            
            // Update audio effects
            UpdateAudioEffects(currentResonance);
            
            // Trigger haptics
            if (enableHaptics && currentResonance > 0.1f)
            {
                player.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, 0.1f, hapticIntensity * currentResonance, hapticIntensity * currentResonance);
                player.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, 0.1f, hapticIntensity * currentResonance, hapticIntensity * currentResonance);
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Update visual effects based on resonance intensity.
    /// </summary>
    private void UpdateVisualEffects(float resonance)
    {
        // Update orb scale
        if (orbRenderer != null)
        {
            float scaleMultiplier = 1f + pulseIntensity * resonance;
            orbRenderer.transform.localScale = originalScale * scaleMultiplier;
        }
        
        // Update particle system
        if (resonanceParticles != null)
        {
            var emission = resonanceParticles.emission;
            emission.rateOverTime = resonance * 50f; // Scale particle emission
            
            var main = resonanceParticles.main;
            main.startSpeed = resonance * 5f; // Scale particle speed
        }
        
        // Update glow intensity
        if (glowMaterial != null)
        {
            Color currentEmission = glowMaterial.GetColor("_EmissionColor");
            glowMaterial.SetColor("_EmissionColor", currentEmission * (1f + resonance));
        }
    }

    /// <summary>
    /// Update audio effects based on resonance intensity.
    /// </summary>
    private void UpdateAudioEffects(float resonance)
    {
        if (resonanceAudio == null) return;
        
        // Update volume
        resonanceAudio.volume = resonance * maxVolume;
        
        // Update pitch based on resonance
        float pitchVariation = 1f + (resonance - 0.5f) * 0.2f;
        resonanceAudio.pitch = (baseFrequency / 440f) * pitchVariation;
        
        // Start audio if not playing
        if (!resonanceAudio.isPlaying && resonance > 0.1f)
        {
            resonanceAudio.Play();
        }
    }

    /// <summary>
    /// Reset visual effects to default state.
    /// </summary>
    private void ResetVisualEffects()
    {
        if (orbRenderer != null)
        {
            orbRenderer.transform.localScale = originalScale;
            orbRenderer.material.color = originalColor;
        }
        
        if (glowMaterial != null)
        {
            glowMaterial.SetColor("_EmissionColor", Color.black);
        }
        
        if (resonanceParticles != null)
        {
            var emission = resonanceParticles.emission;
            emission.rateOverTime = 0f;
        }
    }

    /// <summary>
    /// Fade audio volume over time.
    /// </summary>
    private IEnumerator FadeAudioCoroutine(float targetVolume, float duration)
    {
        float startVolume = resonanceAudio.volume;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            resonanceAudio.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }
        
        resonanceAudio.volume = targetVolume;
        
        if (targetVolume <= 0f)
        {
            resonanceAudio.Stop();
        }
    }

    // --- Public Interaction Methods ---
    
    /// <summary>
    /// Public method to manually activate resonance.
    /// </summary>
    public void ManualActivateResonance()
    {
        if (Time.time - lastInteractionTime < interactionCooldown) return;
        
        lastInteractionTime = Time.time;
        onResonanceActivated?.Invoke();
        
        // Trigger enhanced resonance effect
        StartCoroutine(EnhancedResonanceEffect());
    }

    /// <summary>
    /// Enhanced resonance effect for manual activation.
    /// </summary>
    private IEnumerator EnhancedResonanceEffect()
    {
        float originalResonance = currentResonance;
        currentResonance = 1f;
        
        UpdateVisualEffects(currentResonance);
        UpdateAudioEffects(currentResonance);
        
        yield return new WaitForSeconds(2f);
        
        currentResonance = originalResonance;
    }

    /// <summary>
    /// Public method to set resonance frequency.
    /// </summary>
    public void SetResonanceFrequency(float frequency)
    {
        baseFrequency = frequency;
        if (resonanceAudio != null)
        {
            resonanceAudio.pitch = baseFrequency / 440f;
        }
    }

    /// <summary>
    /// Public method to set resonance intensity.
    /// </summary>
    public void SetResonanceIntensity(float intensity)
    {
        resonanceIntensity = intensity;
    }

    /// <summary>
    /// Public method to get current resonance level.
    /// </summary>
    public float GetCurrentResonance()
    {
        return currentResonance;
    }

    /// <summary>
    /// Public method to check if orb is resonating.
    /// </summary>
    public bool IsResonating()
    {
        return isResonating;
    }

    // --- Gizmos for Editor Visualization ---
    private void OnDrawGizmos()
    {
        // Draw resonance range
        Gizmos.color = isResonating ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxResonanceRange);
        
        // Draw resonance intensity
        if (isResonating)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, currentResonance * maxResonanceRange);
        }
    }
} 