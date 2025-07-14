// QuantumGatekeeper.cs
// Quantum-documented advanced tiered teleportation and access control system
// Features portal VFX/SFX, destination previews, tier glyphs, and multiplayer sync preparation
// Modular, extendable, and ready for future VRChat networking integration
// Compatible with UdonSharp (VRChat) or standard Unity C#

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// Advanced tiered teleportation and access control system for The Citadel of Resonance.
/// Features portal VFX/SFX, destination previews, tier glyphs, and multiplayer sync preparation.
/// </summary>
public class QuantumGatekeeper : UdonSharpBehaviour
{
    // --- Tier System ---
    public enum GDI_Tier
    {
        Wanderer = 0,
        Initiate = 1,
        Radiant = 2,
        Sovereign = 3
    }

    // --- Core Configuration ---
    [Header("Access Control")]
    [Tooltip("Minimum GDI Tier required to use this portal")]
    public GDI_Tier requiredTier = GDI_Tier.Radiant;
    
    [Header("Destination Settings")]
    [Tooltip("Choose between Transform (in-scene) or Scene Name")]
    public bool useSceneName = false;
    [Tooltip("Destination Transform (when useSceneName is false)")]
    public Transform destinationTransform;
    [Tooltip("Destination Scene Name (when useSceneName is true)")]
    public string destinationSceneName = "TheCitadel_InnerSanctum";

    // --- Auto-Detection & Manual Override ---
    [Header("Tier Detection")]
    [Tooltip("Auto-detect QuantumSessionVerifier in scene")]
    public bool autoDetectSessionVerifier = true;
    [Tooltip("Manual QuantumSessionVerifier reference")]
    public QuantumSessionVerifier sessionVerifier;
    [Tooltip("Manual PlayerTierBridge reference")]
    public PlayerTierBridge tierBridge;
    [Tooltip("Manual player tier override for testing")]
    public string manualPlayerTier = "";

    // --- Portal Visual Effects ---
    [Header("Portal VFX")]
    [Tooltip("Portal VFX prefab to instantiate on valid entry")]
    public GameObject portalVFXPrefab;
    [Tooltip("Portal particle system (alternative to prefab)")]
    public ParticleSystem portalParticles;
    [Tooltip("Portal glow material (for tier-based coloring)")]
    public Material portalGlowMaterial;
    [Tooltip("Portal activation animation trigger")]
    public string portalAnimTrigger = "Activate";

    // --- Audio Effects ---
    [Header("Portal SFX")]
    [Tooltip("Portal activation sound (hum, resonance)")]
    public AudioClip portalActivationSound;
    [Tooltip("Portal denial sound (low hum, glyph rejection)")]
    public AudioClip portalDenialSound;
    [Tooltip("Voice whispers on activation")]
    public AudioClip voiceWhispersSound;
    [Tooltip("Audio source for portal sounds")]
    public AudioSource portalAudioSource;

    // --- UI & Feedback ---
    [Header("UI & Feedback")]
    [Tooltip("Tier-locked UI message prefab")]
    public GameObject tierLockedUIPrefab;
    [Tooltip("Tier glyph icon that glows when player qualifies")]
    public GameObject tierGlyphIcon;
    [Tooltip("Preview/hologram of destination")]
    public RenderTexture destinationPreview;
    [Tooltip("Preview plane with shader-based display")]
    public GameObject previewPlane;
    [Tooltip("Lore subtitle text (e.g., 'The Chamber Awaits...')")]
    public string loreSubtitle = "The Chamber Awaits...";

    // --- Animation & Transitions ---
    [Header("Animation & Transitions")]
    [Tooltip("Portal/gate animator component")]
    public Animator portalAnimator;
    [Tooltip("Fade to black on teleport")]
    public bool fadeToBlack = true;
    [Tooltip("Fade duration in seconds")]
    public float fadeDuration = 1.5f;
    [Tooltip("Reset position on invalid entry")]
    public bool resetPositionOnDenial = true;

    // --- Developer & Debug ---
    [Header("Developer & Debug")]
    [Tooltip("Developer tier override for testing")]
    public GDI_Tier devTierOverride = GDI_Tier.Wanderer;
    [Tooltip("Enable debug mode (logs, gizmos, tier bypass)")]
    public bool debugMode = false;
    [Tooltip("Bypass tier requirements in debug mode")]
    public bool bypassTierInDebug = false;

    // --- Multiplayer Support (Future) ---
    [Header("Multiplayer Support (Future)")]
    [Tooltip("Minimum players required to activate portal")]
    public int minPlayersRequired = 1;
    [Tooltip("Minimum tier required for any player")]
    public GDI_Tier minTierForAnyPlayer = GDI_Tier.Wanderer;
    [Tooltip("Sync portal state across clients")]
    public bool syncAcrossClients = false;

    // --- Events ---
    [Header("Unity Events")]
    [Tooltip("Triggered on successful portal activation")]
    public UnityEvent onPortalActivated;
    [Tooltip("Triggered on portal access denied")]
    public UnityEvent onPortalDenied;
    [Tooltip("Triggered when player enters trigger zone")]
    public UnityEvent onPlayerEntered;
    [Tooltip("Triggered when player exits trigger zone")]
    public UnityEvent onPlayerExited;

    // --- Internal State ---
    private bool isPortalActive = false;
    private bool isPlayerInTrigger = false;
    private VRCPlayerApi currentPlayer;
    private string currentPlayerTier = "";
    private bool isCheckingAccess = false;
    private GameObject activeVFX;
    private AudioSource activeAudioSource;

    // --- Tier Level Mapping ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    // --- Unity Start: Initialize Components ---
    private void Start()
    {
        InitializeComponents();
        UpdatePortalVisuals();
    }

    /// <summary>
    /// Initialize and auto-detect required components.
    /// </summary>
    private void InitializeComponents()
    {
        // Auto-detect session verifier
        if (autoDetectSessionVerifier && sessionVerifier == null)
        {
            sessionVerifier = FindObjectOfType<QuantumSessionVerifier>();
            if (debugMode && sessionVerifier != null)
                Debug.Log("[QuantumGatekeeper] Auto-detected QuantumSessionVerifier");
        }

        // Auto-detect tier bridge
        if (tierBridge == null)
        {
            tierBridge = FindObjectOfType<PlayerTierBridge>();
            if (debugMode && tierBridge != null)
                Debug.Log("[QuantumGatekeeper] Auto-detected PlayerTierBridge");
        }

        // Setup audio source
        if (portalAudioSource == null)
        {
            portalAudioSource = GetComponent<AudioSource>();
            if (portalAudioSource == null)
            {
                portalAudioSource = gameObject.AddComponent<AudioSource>();
                portalAudioSource.spatialBlend = 1.0f; // 3D audio
                portalAudioSource.volume = 0.8f;
            }
        }

        // Setup preview plane
        if (previewPlane != null && destinationPreview != null)
        {
            var renderer = previewPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = destinationPreview;
            }
        }
    }

    /// <summary>
    /// Update portal visuals based on current state and tier requirements.
    /// </summary>
    private void UpdatePortalVisuals()
    {
        // Update tier glyph glow
        if (tierGlyphIcon != null)
        {
            var glyphRenderer = tierGlyphIcon.GetComponent<Renderer>();
            if (glyphRenderer != null)
            {
                Color glyphColor = GetTierColor(requiredTier);
                glyphColor.a = isPortalActive ? 1.0f : 0.3f;
                glyphRenderer.material.color = glyphColor;
                
                // Pulse effect when active
                if (isPortalActive)
                {
                    StartCoroutine(PulseGlyph(glyphRenderer));
                }
            }
        }

        // Update portal glow material
        if (portalGlowMaterial != null)
        {
            Color glowColor = GetTierColor(requiredTier);
            glowColor.a = isPortalActive ? 0.8f : 0.2f;
            portalGlowMaterial.SetColor("_EmissionColor", glowColor);
        }
    }

    /// <summary>
    /// Get color associated with a GDI tier.
    /// </summary>
    private Color GetTierColor(GDI_Tier tier)
    {
        switch (tier)
        {
            case GDI_Tier.Wanderer: return new Color(0.5f, 0.5f, 0.5f, 1.0f); // Gray
            case GDI_Tier.Initiate: return new Color(0.2f, 0.8f, 0.2f, 1.0f); // Green
            case GDI_Tier.Radiant: return new Color(0.8f, 0.8f, 0.2f, 1.0f); // Gold
            case GDI_Tier.Sovereign: return new Color(0.8f, 0.2f, 0.8f, 1.0f); // Purple
            default: return Color.white;
        }
    }

    /// <summary>
    /// Pulse effect for tier glyph when portal is active.
    /// </summary>
    private IEnumerator PulseGlyph(Renderer glyphRenderer)
    {
        while (isPortalActive)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 2f);
            Color color = glyphRenderer.material.color;
            color.a = pulse;
            glyphRenderer.material.color = color;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // --- Trigger Events ---
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (debugMode) Debug.Log($"[QuantumGatekeeper] Player {player.displayName} entered trigger");
        
        currentPlayer = player;
        isPlayerInTrigger = true;
        onPlayerEntered?.Invoke();
        
        if (player.isLocal)
        {
            CheckPlayerAccess();
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (debugMode) Debug.Log($"[QuantumGatekeeper] Player {player.displayName} exited trigger");
        
        isPlayerInTrigger = false;
        onPlayerExited?.Invoke();
        
        if (player.isLocal)
        {
            StopPortalEffects();
        }
    }

    /// <summary>
    /// Check if the current player has access to use this portal.
    /// </summary>
    private void CheckPlayerAccess()
    {
        if (isCheckingAccess) return;
        
        isCheckingAccess = true;
        
        // Use debug override if enabled
        if (debugMode && bypassTierInDebug)
        {
            ActivatePortal();
            isCheckingAccess = false;
            return;
        }

        // Get player tier
        string playerTier = GetPlayerTier();
        
        if (debugMode) Debug.Log($"[QuantumGatekeeper] Player tier: {playerTier}, Required: {requiredTier}");
        
        // Check tier access
        if (HasTierAccess(playerTier, requiredTier))
        {
            ActivatePortal();
        }
        else
        {
            DenyPortalAccess(playerTier);
        }
        
        isCheckingAccess = false;
    }

    /// <summary>
    /// Get the current player's tier using various methods.
    /// </summary>
    private string GetPlayerTier()
    {
        // Manual override
        if (!string.IsNullOrEmpty(manualPlayerTier))
        {
            return manualPlayerTier;
        }

        // Debug override
        if (debugMode)
        {
            return devTierOverride.ToString();
        }

        // Session verifier + tier bridge
        if (sessionVerifier != null && tierBridge != null)
        {
            string playerAddress = sessionVerifier.GetPlayerAddress();
            if (!string.IsNullOrEmpty(playerAddress))
            {
                return tierBridge.GetTier(playerAddress);
            }
        }

        // Fallback
        return "Wanderer";
    }

    /// <summary>
    /// Check if player tier meets required tier.
    /// </summary>
    private bool HasTierAccess(string playerTier, GDI_Tier requiredTier)
    {
        int playerLevel = GDILevels.ContainsKey(playerTier) ? GDILevels[playerTier] : 0;
        int requiredLevel = (int)requiredTier;
        return playerLevel >= requiredLevel;
    }

    /// <summary>
    /// Activate the portal and handle teleportation.
    /// </summary>
    private void ActivatePortal()
    {
        if (debugMode) Debug.Log("[QuantumGatekeeper] Activating portal");
        
        isPortalActive = true;
        currentPlayerTier = GetPlayerTier();
        
        // Play activation effects
        PlayPortalActivationEffects();
        
        // Trigger events
        onPortalActivated?.Invoke();
        
        // Handle teleportation
        StartCoroutine(TeleportPlayer());
    }

    /// <summary>
    /// Deny portal access and show feedback.
    /// </summary>
    private void DenyPortalAccess(string playerTier)
    {
        if (debugMode) Debug.Log($"[QuantumGatekeeper] Access denied - Player: {playerTier}, Required: {requiredTier}");
        
        // Play denial effects
        PlayPortalDenialEffects();
        
        // Show tier-locked UI
        ShowTierLockedUI(playerTier);
        
        // Reset position if enabled
        if (resetPositionOnDenial && currentPlayer != null && currentPlayer.isLocal)
        {
            Vector3 pushDir = (currentPlayer.GetPosition() - transform.position).normalized;
            currentPlayer.TeleportTo(currentPlayer.GetPosition() + pushDir * 2f, currentPlayer.GetRotation());
        }
        
        // Trigger events
        onPortalDenied?.Invoke();
    }

    /// <summary>
    /// Play portal activation VFX and SFX.
    /// </summary>
    private void PlayPortalActivationEffects()
    {
        // Portal VFX
        if (portalVFXPrefab != null)
        {
            activeVFX = Instantiate(portalVFXPrefab, transform.position, transform.rotation);
            Destroy(activeVFX, 5f); // Auto-destroy after 5 seconds
        }
        else if (portalParticles != null)
        {
            portalParticles.Play();
        }

        // Portal animation
        if (portalAnimator != null)
        {
            portalAnimator.SetTrigger(portalAnimTrigger);
        }

        // Portal SFX
        if (portalAudioSource != null)
        {
            if (portalActivationSound != null)
            {
                portalAudioSource.PlayOneShot(portalActivationSound);
            }
            
            if (voiceWhispersSound != null)
            {
                StartCoroutine(PlayDelayedSound(voiceWhispersSound, 1.0f));
            }
        }
    }

    /// <summary>
    /// Play portal denial VFX and SFX.
    /// </summary>
    private void PlayPortalDenialEffects()
    {
        // Denial SFX
        if (portalAudioSource != null && portalDenialSound != null)
        {
            portalAudioSource.PlayOneShot(portalDenialSound);
        }
    }

    /// <summary>
    /// Stop all portal effects.
    /// </summary>
    private void StopPortalEffects()
    {
        isPortalActive = false;
        
        if (portalParticles != null)
        {
            portalParticles.Stop();
        }
        
        if (activeVFX != null)
        {
            Destroy(activeVFX);
            activeVFX = null;
        }
    }

    /// <summary>
    /// Show tier-locked UI message.
    /// </summary>
    private void ShowTierLockedUI(string playerTier)
    {
        if (tierLockedUIPrefab != null)
        {
            GameObject uiMessage = Instantiate(tierLockedUIPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(uiMessage, 3f); // Auto-destroy after 3 seconds
        }
        
        // Log message for now (replace with proper UI system)
        string message = $"Access Denied – {requiredTier} Required (You: {playerTier})";
        Debug.Log($"[QuantumGatekeeper] {message}");
        
        if (!string.IsNullOrEmpty(loreSubtitle))
        {
            Debug.Log($"[QuantumGatekeeper] {loreSubtitle}");
        }
    }

    /// <summary>
    /// Handle player teleportation to destination.
    /// </summary>
    private IEnumerator TeleportPlayer()
    {
        if (currentPlayer == null || !currentPlayer.isLocal) yield break;
        
        // Fade to black if enabled
        if (fadeToBlack)
        {
            yield return StartCoroutine(FadeToBlack());
        }
        
        // Teleport to destination
        if (useSceneName)
        {
            // Load new scene (requires VRChat scene management)
            if (debugMode) Debug.Log($"[QuantumGatekeeper] Loading scene: {destinationSceneName}");
            // VRC_SceneDescriptor.LoadScene(destinationSceneName); // Future VRChat API
        }
        else if (destinationTransform != null)
        {
            if (debugMode) Debug.Log($"[QuantumGatekeeper] Teleporting to: {destinationTransform.position}");
            currentPlayer.TeleportTo(destinationTransform.position, destinationTransform.rotation);
        }
        
        // Fade back in
        if (fadeToBlack)
        {
            yield return StartCoroutine(FadeFromBlack());
        }
    }

    /// <summary>
    /// Fade to black effect.
    /// </summary>
    private IEnumerator FadeToBlack()
    {
        // Simple fade implementation (replace with proper UI system)
        if (debugMode) Debug.Log("[QuantumGatekeeper] Fading to black");
        yield return new WaitForSeconds(fadeDuration);
    }

    /// <summary>
    /// Fade from black effect.
    /// </summary>
    private IEnumerator FadeFromBlack()
    {
        // Simple fade implementation (replace with proper UI system)
        if (debugMode) Debug.Log("[QuantumGatekeeper] Fading from black");
        yield return new WaitForSeconds(fadeDuration);
    }

    /// <summary>
    /// Play sound with delay.
    /// </summary>
    private IEnumerator PlayDelayedSound(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (portalAudioSource != null)
        {
            portalAudioSource.PlayOneShot(clip);
        }
    }

    // --- Public Methods for External Control ---
    
    /// <summary>
    /// Public method to manually activate the portal (for testing or external triggers).
    /// </summary>
    public void ManualActivatePortal()
    {
        if (debugMode) Debug.Log("[QuantumGatekeeper] Manual portal activation");
        ActivatePortal();
    }

    /// <summary>
    /// Public method to set the required tier dynamically.
    /// </summary>
    public void SetRequiredTier(GDI_Tier newTier)
    {
        requiredTier = newTier;
        UpdatePortalVisuals();
        if (debugMode) Debug.Log($"[QuantumGatekeeper] Required tier set to: {newTier}");
    }

    /// <summary>
    /// Public method to check if portal is currently active.
    /// </summary>
    public bool IsPortalActive()
    {
        return isPortalActive;
    }

    // --- Gizmos for Editor Visualization ---
    private void OnDrawGizmos()
    {
        if (!debugMode) return;
        
        // Draw trigger zone
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = isPortalActive ? Color.green : Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.bounds.center - transform.position, col.bounds.size);
        }
        
        // Draw destination line
        if (destinationTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, destinationTransform.position);
            Gizmos.DrawWireSphere(destinationTransform.position, 0.5f);
        }
    }

    // --- Future Multiplayer Sync Methods ---
    
    /// <summary>
    /// Check if minimum player requirements are met for multiplayer activation.
    /// </summary>
    private bool CheckMultiplayerRequirements()
    {
        // Future implementation for VRChat networking
        // This would check nearby players and their tiers
        return true; // Placeholder
    }

    /// <summary>
    /// Sync portal state across all clients.
    /// </summary>
    private void SyncPortalState()
    {
        // Future implementation for VRChat networking
        // This would sync the portal activation state
    }
} 