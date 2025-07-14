// QuantumAudioController.cs
// Quantum-documented dynamic audio system for The Citadel of Resonance
// Provides tier-based music, environmental audio, and interactive soundscapes
// Features adaptive music, spatial audio, and event-driven sound effects
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
/// Dynamic audio controller that provides adaptive music and environmental audio.
/// Creates immersive soundscapes that respond to player actions and tier levels.
/// </summary>
public class QuantumAudioController : UdonSharpBehaviour
{
    // --- Audio Configuration ---
    [Header("Audio Sources")]
    [Tooltip("Main ambient music source")]
    public AudioSource ambientMusicSource;
    [Tooltip("Tier-based music source")]
    public AudioSource tierMusicSource;
    [Tooltip("Environmental effects source")]
    public AudioSource environmentalSource;
    [Tooltip("Interactive effects source")]
    public AudioSource interactiveSource;

    [Header("Music Tracks")]
    [Tooltip("Wanderer tier ambient music")]
    public AudioClip wandererAmbient;
    [Tooltip("Initiate tier ambient music")]
    public AudioClip initiateAmbient;
    [Tooltip("Radiant tier ambient music")]
    public AudioClip radiantAmbient;
    [Tooltip("Sovereign tier ambient music")]
    public AudioClip sovereignAmbient;
    [Tooltip("Portal activation music")]
    public AudioClip portalActivationMusic;
    [Tooltip("Resonance effect music")]
    public AudioClip resonanceMusic;
    [Tooltip("Chamber unlock music")]
    public AudioClip chamberUnlockMusic;

    [Header("Environmental Audio")]
    [Tooltip("Day ambient sounds")]
    public AudioClip dayAmbient;
    [Tooltip("Night ambient sounds")]
    public AudioClip nightAmbient;
    [Tooltip("Storm weather sounds")]
    public AudioClip stormSounds;
    [Tooltip("Aurora weather sounds")]
    public AudioClip auroraSounds;
    [Tooltip("Wind ambient sounds")]
    public AudioClip windSounds;

    [Header("Interactive Audio")]
    [Tooltip("Portal activation sound")]
    public AudioClip portalActivationSound;
    [Tooltip("Portal denial sound")]
    public AudioClip portalDenialSound;
    [Tooltip("Resonance orb interaction sound")]
    public AudioClip resonanceInteractionSound;
    [Tooltip("Tier unlock sound")]
    public AudioClip tierUnlockSound;
    [Tooltip("Chamber unlock sound")]
    public AudioClip chamberUnlockSound;

    [Header("Audio Settings")]
    [Tooltip("Master volume")]
    public float masterVolume = 0.8f;
    [Tooltip("Music volume")]
    public float musicVolume = 0.6f;
    [Tooltip("Environmental volume")]
    public float environmentalVolume = 0.4f;
    [Tooltip("Interactive volume")]
    public float interactiveVolume = 0.7f;
    [Tooltip("Crossfade duration")]
    public float crossfadeDuration = 2f;
    [Tooltip("Spatial blend (0=2D, 1=3D)")]
    public float spatialBlend = 0.5f;

    [Header("Tier-Based Audio")]
    [Tooltip("Enable tier-based music changes")]
    public bool enableTierMusic = true;
    [Tooltip("Tier music transition duration")]
    public float tierTransitionDuration = 3f;
    [Tooltip("Tier-based volume multiplier")]
    public float tierVolumeMultiplier = 1.2f;

    [Header("Environmental Responses")]
    [Tooltip("Enable day/night audio changes")]
    public bool enableDayNightAudio = true;
    [Tooltip("Enable weather audio effects")]
    public bool enableWeatherAudio = true;
    [Tooltip("Enable portal audio responses")]
    public bool enablePortalAudio = true;
    [Tooltip("Enable resonance audio responses")]
    public bool enableResonanceAudio = true;

    // --- Events ---
    [Header("Unity Events")]
    [Tooltip("Triggered when music changes to a new tier")]
    public UnityEvent onTierMusicChanged;
    [Tooltip("Triggered when environmental audio changes")]
    public UnityEvent onEnvironmentalAudioChanged;
    [Tooltip("Triggered when interactive audio plays")]
    public UnityEvent onInteractiveAudioPlayed;
    [Tooltip("Triggered when audio system initializes")]
    public UnityEvent onAudioSystemInitialized;

    // --- Internal State ---
    private string currentTier = "Wanderer";
    private string currentEnvironment = "day";
    private string currentWeather = "clear";
    private AudioClip currentAmbientTrack;
    private AudioClip targetAmbientTrack;
    private bool isTransitioning = false;
    private Coroutine musicTransitionCoroutine;
    private Coroutine environmentalCoroutine;
    private Coroutine interactiveCoroutine;

    // --- Tier Level Mapping ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    // --- Unity Start: Initialize Audio System ---
    private void Start()
    {
        InitializeAudioSystem();
    }

    /// <summary>
    /// Initialize the audio system.
    /// </summary>
    private void InitializeAudioSystem()
    {
        // Setup audio sources if not assigned
        SetupAudioSources();

        // Set initial volumes
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetEnvironmentalVolume(environmentalVolume);
        SetInteractiveVolume(interactiveVolume);

        // Set initial ambient track
        currentAmbientTrack = wandererAmbient;
        if (ambientMusicSource != null && currentAmbientTrack != null)
        {
            ambientMusicSource.clip = currentAmbientTrack;
            ambientMusicSource.Play();
        }

        // Start environmental audio if enabled
        if (enableDayNightAudio)
        {
            StartEnvironmentalAudio();
        }

        onAudioSystemInitialized?.Invoke();
    }

    /// <summary>
    /// Setup audio sources with proper configuration.
    /// </summary>
    private void SetupAudioSources()
    {
        // Setup ambient music source
        if (ambientMusicSource == null)
        {
            ambientMusicSource = gameObject.AddComponent<AudioSource>();
        }
        ambientMusicSource.loop = true;
        ambientMusicSource.spatialBlend = spatialBlend;
        ambientMusicSource.volume = musicVolume;

        // Setup tier music source
        if (tierMusicSource == null)
        {
            tierMusicSource = gameObject.AddComponent<AudioSource>();
        }
        tierMusicSource.loop = true;
        tierMusicSource.spatialBlend = spatialBlend;
        tierMusicSource.volume = 0f; // Start silent

        // Setup environmental source
        if (environmentalSource == null)
        {
            environmentalSource = gameObject.AddComponent<AudioSource>();
        }
        environmentalSource.loop = true;
        environmentalSource.spatialBlend = spatialBlend;
        environmentalSource.volume = environmentalVolume;

        // Setup interactive source
        if (interactiveSource == null)
        {
            interactiveSource = gameObject.AddComponent<AudioSource>();
        }
        interactiveSource.loop = false;
        interactiveSource.spatialBlend = spatialBlend;
        interactiveSource.volume = interactiveVolume;
    }

    /// <summary>
    /// Start environmental audio system.
    /// </summary>
    private void StartEnvironmentalAudio()
    {
        if (environmentalCoroutine != null)
        {
            StopCoroutine(environmentalCoroutine);
        }
        environmentalCoroutine = StartCoroutine(EnvironmentalAudioCoroutine());
    }

    /// <summary>
    /// Environmental audio coroutine for day/night and weather changes.
    /// </summary>
    private IEnumerator EnvironmentalAudioCoroutine()
    {
        while (enableDayNightAudio)
        {
            // Simulate day/night cycle (simplified)
            float timeOfDay = (Time.time % 300f) / 300f; // 5-minute cycle
            string newEnvironment = timeOfDay > 0.5f ? "night" : "day";
            
            if (newEnvironment != currentEnvironment)
            {
                currentEnvironment = newEnvironment;
                UpdateEnvironmentalAudio();
            }
            
            yield return new WaitForSeconds(10f);
        }
    }

    /// <summary>
    /// Update environmental audio based on current conditions.
    /// </summary>
    private void UpdateEnvironmentalAudio()
    {
        AudioClip newEnvironmentalClip = null;
        
        switch (currentEnvironment)
        {
            case "day":
                newEnvironmentalClip = dayAmbient;
                break;
            case "night":
                newEnvironmentalClip = nightAmbient;
                break;
        }
        
        if (newEnvironmentalClip != null && environmentalSource != null)
        {
            StartCoroutine(CrossfadeEnvironmentalAudio(newEnvironmentalClip));
        }
        
        onEnvironmentalAudioChanged?.Invoke();
    }

    /// <summary>
    /// Crossfade environmental audio.
    /// </summary>
    private IEnumerator CrossfadeEnvironmentalAudio(AudioClip newClip)
    {
        if (environmentalSource == null) yield break;
        
        float startVolume = environmentalSource.volume;
        float elapsed = 0f;
        
        // Fade out current
        while (elapsed < crossfadeDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (crossfadeDuration * 0.5f);
            environmentalSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }
        
        // Change clip
        environmentalSource.clip = newClip;
        environmentalSource.Play();
        
        // Fade in new
        elapsed = 0f;
        while (elapsed < crossfadeDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (crossfadeDuration * 0.5f);
            environmentalSource.volume = Mathf.Lerp(0f, environmentalVolume, t);
            yield return null;
        }
        
        environmentalSource.volume = environmentalVolume;
    }

    /// <summary>
    /// Update music based on player tier.
    /// </summary>
    public void UpdateTierMusic(string playerTier)
    {
        if (!enableTierMusic) return;

        currentTier = playerTier;
        AudioClip newTrack = GetTierMusic(playerTier);
        
        if (newTrack != targetAmbientTrack)
        {
            targetAmbientTrack = newTrack;
            StartMusicTransition();
        }
    }

    /// <summary>
    /// Get music track associated with a GDI tier.
    /// </summary>
    private AudioClip GetTierMusic(string tier)
    {
        switch (tier)
        {
            case "Wanderer": return wandererAmbient;
            case "Initiate": return initiateAmbient;
            case "Radiant": return radiantAmbient;
            case "Sovereign": return sovereignAmbient;
            default: return wandererAmbient;
        }
    }

    /// <summary>
    /// Start music transition animation.
    /// </summary>
    private void StartMusicTransition()
    {
        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        musicTransitionCoroutine = StartCoroutine(MusicTransitionCoroutine());
    }

    /// <summary>
    /// Music transition coroutine with crossfade.
    /// </summary>
    private IEnumerator MusicTransitionCoroutine()
    {
        isTransitioning = true;
        
        if (ambientMusicSource == null || tierMusicSource == null) yield break;
        
        // Setup tier music source
        tierMusicSource.clip = targetAmbientTrack;
        tierMusicSource.Play();
        
        // Crossfade between sources
        float elapsed = 0f;
        while (elapsed < tierTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / tierTransitionDuration;
            
            ambientMusicSource.volume = Mathf.Lerp(musicVolume, 0f, t);
            tierMusicSource.volume = Mathf.Lerp(0f, musicVolume * tierVolumeMultiplier, t);
            
            yield return null;
        }
        
        // Swap sources
        ambientMusicSource.clip = targetAmbientTrack;
        ambientMusicSource.volume = musicVolume * tierVolumeMultiplier;
        ambientMusicSource.Play();
        
        tierMusicSource.Stop();
        tierMusicSource.volume = 0f;
        
        currentAmbientTrack = targetAmbientTrack;
        isTransitioning = false;
        
        onTierMusicChanged?.Invoke();
    }

    /// <summary>
    /// Play portal activation audio.
    /// </summary>
    public void PlayPortalActivationAudio()
    {
        if (!enablePortalAudio) return;

        StartCoroutine(PlayPortalAudioSequence());
    }

    /// <summary>
    /// Portal audio sequence coroutine.
    /// </summary>
    private IEnumerator PlayPortalAudioSequence()
    {
        // Play activation sound
        if (portalActivationSound != null && interactiveSource != null)
        {
            interactiveSource.PlayOneShot(portalActivationSound);
            yield return new WaitForSeconds(portalActivationSound.length * 0.5f);
        }
        
        // Play activation music overlay
        if (portalActivationMusic != null && tierMusicSource != null)
        {
            tierMusicSource.clip = portalActivationMusic;
            tierMusicSource.volume = musicVolume * 0.5f;
            tierMusicSource.Play();
            
            yield return new WaitForSeconds(portalActivationMusic.length);
            
            tierMusicSource.Stop();
            tierMusicSource.volume = 0f;
        }
        
        onInteractiveAudioPlayed?.Invoke();
    }

    /// <summary>
    /// Play portal denial audio.
    /// </summary>
    public void PlayPortalDenialAudio()
    {
        if (!enablePortalAudio) return;

        if (portalDenialSound != null && interactiveSource != null)
        {
            interactiveSource.PlayOneShot(portalDenialSound);
            onInteractiveAudioPlayed?.Invoke();
        }
    }

    /// <summary>
    /// Play resonance interaction audio.
    /// </summary>
    public void PlayResonanceAudio(float intensity)
    {
        if (!enableResonanceAudio) return;

        StartCoroutine(PlayResonanceAudioSequence(intensity));
    }

    /// <summary>
    /// Resonance audio sequence coroutine.
    /// </summary>
    private IEnumerator PlayResonanceAudioSequence(float intensity)
    {
        // Play interaction sound
        if (resonanceInteractionSound != null && interactiveSource != null)
        {
            interactiveSource.PlayOneShot(resonanceInteractionSound);
        }
        
        // Play resonance music overlay
        if (resonanceMusic != null && tierMusicSource != null)
        {
            tierMusicSource.clip = resonanceMusic;
            tierMusicSource.volume = musicVolume * intensity * 0.3f;
            tierMusicSource.Play();
            
            yield return new WaitForSeconds(resonanceMusic.length * 0.5f);
            
            tierMusicSource.Stop();
            tierMusicSource.volume = 0f;
        }
        
        onInteractiveAudioPlayed?.Invoke();
    }

    /// <summary>
    /// Play tier unlock audio.
    /// </summary>
    public void PlayTierUnlockAudio()
    {
        if (tierUnlockSound != null && interactiveSource != null)
        {
            interactiveSource.PlayOneShot(tierUnlockSound);
            onInteractiveAudioPlayed?.Invoke();
        }
    }

    /// <summary>
    /// Play chamber unlock audio.
    /// </summary>
    public void PlayChamberUnlockAudio()
    {
        StartCoroutine(PlayChamberUnlockAudioSequence());
    }

    /// <summary>
    /// Chamber unlock audio sequence coroutine.
    /// </summary>
    private IEnumerator PlayChamberUnlockAudioSequence()
    {
        // Play unlock sound
        if (chamberUnlockSound != null && interactiveSource != null)
        {
            interactiveSource.PlayOneShot(chamberUnlockSound);
        }
        
        // Play unlock music overlay
        if (chamberUnlockMusic != null && tierMusicSource != null)
        {
            tierMusicSource.clip = chamberUnlockMusic;
            tierMusicSource.volume = musicVolume * 0.7f;
            tierMusicSource.Play();
            
            yield return new WaitForSeconds(chamberUnlockMusic.length);
            
            tierMusicSource.Stop();
            tierMusicSource.volume = 0f;
        }
        
        onInteractiveAudioPlayed?.Invoke();
    }

    /// <summary>
    /// Trigger weather audio effect.
    /// </summary>
    public void TriggerWeatherAudio(string weatherType)
    {
        if (!enableWeatherAudio) return;

        currentWeather = weatherType;
        AudioClip weatherClip = GetWeatherAudio(weatherType);
        
        if (weatherClip != null)
        {
            StartCoroutine(PlayWeatherAudio(weatherClip));
        }
    }

    /// <summary>
    /// Get weather audio clip.
    /// </summary>
    private AudioClip GetWeatherAudio(string weatherType)
    {
        switch (weatherType.ToLower())
        {
            case "storm": return stormSounds;
            case "aurora": return auroraSounds;
            case "wind": return windSounds;
            default: return null;
        }
    }

    /// <summary>
    /// Play weather audio effect.
    /// </summary>
    private IEnumerator PlayWeatherAudio(AudioClip weatherClip)
    {
        if (tierMusicSource == null) yield break;
        
        // Play weather overlay
        tierMusicSource.clip = weatherClip;
        tierMusicSource.volume = environmentalVolume * 0.5f;
        tierMusicSource.Play();
        
        yield return new WaitForSeconds(weatherClip.length);
        
        tierMusicSource.Stop();
        tierMusicSource.volume = 0f;
        
        onEnvironmentalAudioChanged?.Invoke();
    }

    // --- Public Control Methods ---

    /// <summary>
    /// Public method to set master volume.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = masterVolume;
    }

    /// <summary>
    /// Public method to set music volume.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (ambientMusicSource != null)
        {
            ambientMusicSource.volume = musicVolume;
        }
    }

    /// <summary>
    /// Public method to set environmental volume.
    /// </summary>
    public void SetEnvironmentalVolume(float volume)
    {
        environmentalVolume = Mathf.Clamp01(volume);
        if (environmentalSource != null)
        {
            environmentalSource.volume = environmentalVolume;
        }
    }

    /// <summary>
    /// Public method to set interactive volume.
    /// </summary>
    public void SetInteractiveVolume(float volume)
    {
        interactiveVolume = Mathf.Clamp01(volume);
        if (interactiveSource != null)
        {
            interactiveSource.volume = interactiveVolume;
        }
    }

    /// <summary>
    /// Public method to toggle environmental audio.
    /// </summary>
    public void ToggleEnvironmentalAudio()
    {
        enableDayNightAudio = !enableDayNightAudio;
        
        if (enableDayNightAudio)
        {
            StartEnvironmentalAudio();
        }
        else if (environmentalCoroutine != null)
        {
            StopCoroutine(environmentalCoroutine);
        }
    }

    /// <summary>
    /// Public method to get current tier.
    /// </summary>
    public string GetCurrentTier()
    {
        return currentTier;
    }

    /// <summary>
    /// Public method to get current environment.
    /// </summary>
    public string GetCurrentEnvironment()
    {
        return currentEnvironment;
    }

    /// <summary>
    /// Public method to check if audio is transitioning.
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    /// <summary>
    /// Public method to get current ambient track.
    /// </summary>
    public AudioClip GetCurrentAmbientTrack()
    {
        return currentAmbientTrack;
    }

    // --- Gizmos for Editor Visualization ---
    private void OnDrawGizmos()
    {
        // Draw audio influence zones
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 15f);
        
        // Draw audio source positions
        if (ambientMusicSource != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ambientMusicSource.transform.position, 2f);
        }
        
        if (environmentalSource != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(environmentalSource.transform.position, 2f);
        }
    }
} 