// QuantumLightingController.cs
// Quantum-documented dynamic lighting system for The Citadel of Resonance
// Provides tier-based lighting, environmental responses, and atmospheric effects
// Features color transitions, intensity modulation, and event-driven lighting
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
/// Dynamic lighting controller that responds to player actions, tier levels, and environmental events.
/// Creates immersive atmospheric lighting for The Citadel of Resonance.
/// </summary>
public class QuantumLightingController : UdonSharpBehaviour
{
    // --- Lighting Configuration ---
    [Header("Lighting Setup")]
    [Tooltip("Primary directional light")]
    public Light mainLight;
    [Tooltip("Ambient point lights")]
    public Light[] ambientLights;
    [Tooltip("Tier-based accent lights")]
    public Light[] tierAccentLights;
    [Tooltip("Environmental effect lights")]
    public Light[] effectLights;

    [Header("Tier-Based Lighting")]
    [Tooltip("Enable tier-based color changes")]
    public bool enableTierColors = true;
    [Tooltip("Tier color transition duration")]
    public float colorTransitionDuration = 2f;
    [Tooltip("Tier-based intensity multiplier")]
    public float tierIntensityMultiplier = 1.2f;

    [Header("Environmental Effects")]
    [Tooltip("Enable day/night cycle")]
    public bool enableDayNightCycle = true;
    [Tooltip("Day/night cycle duration in seconds")]
    public float dayNightCycleDuration = 300f; // 5 minutes
    [Tooltip("Weather effect intensity")]
    public float weatherEffectIntensity = 0.5f;
    [Tooltip("Enable dynamic shadows")]
    public bool enableDynamicShadows = true;

    [Header("Event Responses")]
    [Tooltip("Portal activation lighting effect")]
    public bool enablePortalLighting = true;
    [Tooltip("Resonance orb lighting effect")]
    public bool enableResonanceLighting = true;
    [Tooltip("Chamber unlock lighting effect")]
    public bool enableChamberLighting = true;

    // --- Color Schemes ---
    [Header("Color Schemes")]
    [Tooltip("Wanderer tier color")]
    public Color wandererColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    [Tooltip("Initiate tier color")]
    public Color initiateColor = new Color(0.2f, 0.8f, 0.2f, 1.0f);
    [Tooltip("Radiant tier color")]
    public Color radiantColor = new Color(0.8f, 0.8f, 0.2f, 1.0f);
    [Tooltip("Sovereign tier color")]
    public Color sovereignColor = new Color(0.8f, 0.2f, 0.8f, 1.0f);
    [Tooltip("Portal activation color")]
    public Color portalActivationColor = new Color(0.2f, 0.8f, 1.0f, 1.0f);
    [Tooltip("Resonance effect color")]
    public Color resonanceColor = new Color(1.0f, 0.6f, 0.2f, 1.0f);

    // --- Events ---
    [Header("Unity Events")]
    [Tooltip("Triggered when lighting changes to a new tier")]
    public UnityEvent onTierLightingChanged;
    [Tooltip("Triggered when day/night cycle completes")]
    public UnityEvent onDayNightCycleComplete;
    [Tooltip("Triggered when environmental effect starts")]
    public UnityEvent onEnvironmentalEffectStart;
    [Tooltip("Triggered when environmental effect ends")]
    public UnityEvent onEnvironmentalEffectEnd;

    // --- Internal State ---
    private string currentTier = "Wanderer";
    private Color currentMainColor;
    private Color targetMainColor;
    private float currentCycleTime = 0f;
    private bool isTransitioning = false;
    private Coroutine colorTransitionCoroutine;
    private Coroutine dayNightCoroutine;
    private Coroutine effectCoroutine;

    // --- Tier Level Mapping ---
    private static readonly Dictionary<string, int> GDILevels = new Dictionary<string, int>
    {
        {"Wanderer", 0},
        {"Initiate", 1},
        {"Radiant", 2},
        {"Sovereign", 3}
    };

    // --- Unity Start: Initialize Lighting System ---
    private void Start()
    {
        InitializeLightingSystem();
    }

    /// <summary>
    /// Initialize the lighting system.
    /// </summary>
    private void InitializeLightingSystem()
    {
        // Set initial colors
        currentMainColor = wandererColor;
        targetMainColor = wandererColor;

        // Setup main light
        if (mainLight != null)
        {
            mainLight.color = currentMainColor;
            mainLight.shadows = enableDynamicShadows ? LightShadows.Soft : LightShadows.None;
        }

        // Setup ambient lights
        foreach (var light in ambientLights)
        {
            if (light != null)
            {
                light.color = currentMainColor * 0.5f;
                light.intensity = light.intensity * 0.8f;
            }
        }

        // Setup tier accent lights
        foreach (var light in tierAccentLights)
        {
            if (light != null)
            {
                light.color = currentMainColor;
                light.intensity = light.intensity * 0.6f;
            }
        }

        // Start day/night cycle if enabled
        if (enableDayNightCycle)
        {
            StartDayNightCycle();
        }
    }

    /// <summary>
    /// Start the day/night cycle.
    /// </summary>
    private void StartDayNightCycle()
    {
        if (dayNightCoroutine != null)
        {
            StopCoroutine(dayNightCoroutine);
        }
        dayNightCoroutine = StartCoroutine(DayNightCycleCoroutine());
    }

    /// <summary>
    /// Day/night cycle coroutine.
    /// </summary>
    private IEnumerator DayNightCycleCoroutine()
    {
        while (enableDayNightCycle)
        {
            currentCycleTime += Time.deltaTime;
            float cycleProgress = (currentCycleTime % dayNightCycleDuration) / dayNightCycleDuration;
            
            // Calculate day/night lighting
            float dayIntensity = Mathf.Sin(cycleProgress * Mathf.PI * 2f) * 0.5f + 0.5f;
            Color dayColor = Color.Lerp(Color.blue * 0.3f, Color.white, dayIntensity);
            
            // Apply day/night effect
            ApplyEnvironmentalLighting(dayColor, dayIntensity);
            
            // Trigger cycle completion event
            if (cycleProgress >= 0.99f)
            {
                onDayNightCycleComplete?.Invoke();
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Apply environmental lighting effects.
    /// </summary>
    private void ApplyEnvironmentalLighting(Color color, float intensity)
    {
        if (mainLight != null)
        {
            mainLight.color = Color.Lerp(mainLight.color, color, 0.1f);
            mainLight.intensity = Mathf.Lerp(mainLight.intensity, intensity * 1.5f, 0.1f);
        }

        foreach (var light in ambientLights)
        {
            if (light != null)
            {
                light.color = Color.Lerp(light.color, color * 0.5f, 0.1f);
                light.intensity = Mathf.Lerp(light.intensity, intensity * 0.8f, 0.1f);
            }
        }
    }

    /// <summary>
    /// Update lighting based on player tier.
    /// </summary>
    public void UpdateTierLighting(string playerTier)
    {
        if (!enableTierColors) return;

        currentTier = playerTier;
        Color newColor = GetTierColor(playerTier);
        
        if (newColor != targetMainColor)
        {
            targetMainColor = newColor;
            StartColorTransition();
        }
    }

    /// <summary>
    /// Get color associated with a GDI tier.
    /// </summary>
    private Color GetTierColor(string tier)
    {
        switch (tier)
        {
            case "Wanderer": return wandererColor;
            case "Initiate": return initiateColor;
            case "Radiant": return radiantColor;
            case "Sovereign": return sovereignColor;
            default: return wandererColor;
        }
    }

    /// <summary>
    /// Start color transition animation.
    /// </summary>
    private void StartColorTransition()
    {
        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        colorTransitionCoroutine = StartCoroutine(ColorTransitionCoroutine());
    }

    /// <summary>
    /// Color transition coroutine.
    /// </summary>
    private IEnumerator ColorTransitionCoroutine()
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = currentMainColor;

        while (elapsed < colorTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / colorTransitionDuration;
            
            currentMainColor = Color.Lerp(startColor, targetMainColor, t);
            ApplyMainLighting(currentMainColor);
            
            yield return null;
        }

        currentMainColor = targetMainColor;
        ApplyMainLighting(currentMainColor);
        isTransitioning = false;
        
        onTierLightingChanged?.Invoke();
    }

    /// <summary>
    /// Apply main lighting color to all lights.
    /// </summary>
    private void ApplyMainLighting(Color color)
    {
        if (mainLight != null)
        {
            mainLight.color = color;
        }

        foreach (var light in ambientLights)
        {
            if (light != null)
            {
                light.color = color * 0.5f;
            }
        }

        foreach (var light in tierAccentLights)
        {
            if (light != null)
            {
                light.color = color;
                light.intensity = light.intensity * tierIntensityMultiplier;
            }
        }
    }

    /// <summary>
    /// Trigger portal activation lighting effect.
    /// </summary>
    public void TriggerPortalLightingEffect()
    {
        if (!enablePortalLighting) return;

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(PortalLightingEffect());
    }

    /// <summary>
    /// Portal activation lighting effect coroutine.
    /// </summary>
    private IEnumerator PortalLightingEffect()
    {
        onEnvironmentalEffectStart?.Invoke();
        
        Color originalColor = currentMainColor;
        float originalIntensity = mainLight != null ? mainLight.intensity : 1f;
        
        // Flash effect
        for (int i = 0; i < 3; i++)
        {
            ApplyMainLighting(portalActivationColor);
            if (mainLight != null) mainLight.intensity = originalIntensity * 2f;
            yield return new WaitForSeconds(0.2f);
            
            ApplyMainLighting(originalColor);
            if (mainLight != null) mainLight.intensity = originalIntensity;
            yield return new WaitForSeconds(0.2f);
        }
        
        onEnvironmentalEffectEnd?.Invoke();
    }

    /// <summary>
    /// Trigger resonance lighting effect.
    /// </summary>
    public void TriggerResonanceLightingEffect(float intensity)
    {
        if (!enableResonanceLighting) return;

        StartCoroutine(ResonanceLightingEffect(intensity));
    }

    /// <summary>
    /// Resonance lighting effect coroutine.
    /// </summary>
    private IEnumerator ResonanceLightingEffect(float intensity)
    {
        Color originalColor = currentMainColor;
        Color resonanceColor = this.resonanceColor * intensity;
        
        // Blend resonance color
        Color blendedColor = Color.Lerp(originalColor, resonanceColor, 0.3f);
        ApplyMainLighting(blendedColor);
        
        yield return new WaitForSeconds(1f);
        
        // Return to original
        ApplyMainLighting(originalColor);
    }

    /// <summary>
    /// Trigger chamber unlock lighting effect.
    /// </summary>
    public void TriggerChamberLightingEffect()
    {
        if (!enableChamberLighting) return;

        StartCoroutine(ChamberLightingEffect());
    }

    /// <summary>
    /// Chamber unlock lighting effect coroutine.
    /// </summary>
    private IEnumerator ChamberLightingEffect()
    {
        onEnvironmentalEffectStart?.Invoke();
        
        Color originalColor = currentMainColor;
        float originalIntensity = mainLight != null ? mainLight.intensity : 1f;
        
        // Dramatic lighting change
        ApplyMainLighting(sovereignColor);
        if (mainLight != null) mainLight.intensity = originalIntensity * 1.5f;
        
        // Pulse effect
        for (int i = 0; i < 5; i++)
        {
            if (mainLight != null) mainLight.intensity = originalIntensity * 1.5f;
            yield return new WaitForSeconds(0.3f);
            
            if (mainLight != null) mainLight.intensity = originalIntensity * 0.8f;
            yield return new WaitForSeconds(0.3f);
        }
        
        // Return to original
        ApplyMainLighting(originalColor);
        if (mainLight != null) mainLight.intensity = originalIntensity;
        
        onEnvironmentalEffectEnd?.Invoke();
    }

    /// <summary>
    /// Trigger weather effect lighting.
    /// </summary>
    public void TriggerWeatherEffect(string weatherType)
    {
        StartCoroutine(WeatherLightingEffect(weatherType));
    }

    /// <summary>
    /// Weather lighting effect coroutine.
    /// </summary>
    private IEnumerator WeatherLightingEffect(string weatherType)
    {
        onEnvironmentalEffectStart?.Invoke();
        
        Color originalColor = currentMainColor;
        Color weatherColor = originalColor;
        
        switch (weatherType.ToLower())
        {
            case "storm":
                weatherColor = Color.gray * 0.5f;
                break;
            case "aurora":
                weatherColor = Color.green * 0.8f;
                break;
            case "sunset":
                weatherColor = Color.orange * 0.7f;
                break;
            case "moonlight":
                weatherColor = Color.blue * 0.3f;
                break;
        }
        
        // Apply weather effect
        ApplyMainLighting(weatherColor);
        if (mainLight != null) mainLight.intensity *= weatherEffectIntensity;
        
        yield return new WaitForSeconds(10f);
        
        // Return to original
        ApplyMainLighting(originalColor);
        if (mainLight != null) mainLight.intensity /= weatherEffectIntensity;
        
        onEnvironmentalEffectEnd?.Invoke();
    }

    // --- Public Control Methods ---

    /// <summary>
    /// Public method to set main lighting color.
    /// </summary>
    public void SetMainLightingColor(Color color)
    {
        targetMainColor = color;
        StartColorTransition();
    }

    /// <summary>
    /// Public method to set lighting intensity.
    /// </summary>
    public void SetLightingIntensity(float intensity)
    {
        if (mainLight != null)
        {
            mainLight.intensity = intensity;
        }

        foreach (var light in ambientLights)
        {
            if (light != null)
            {
                light.intensity = intensity * 0.8f;
            }
        }
    }

    /// <summary>
    /// Public method to toggle day/night cycle.
    /// </summary>
    public void ToggleDayNightCycle()
    {
        enableDayNightCycle = !enableDayNightCycle;
        
        if (enableDayNightCycle)
        {
            StartDayNightCycle();
        }
        else if (dayNightCoroutine != null)
        {
            StopCoroutine(dayNightCoroutine);
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
    /// Public method to check if lighting is transitioning.
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    /// <summary>
    /// Public method to get current main color.
    /// </summary>
    public Color GetCurrentMainColor()
    {
        return currentMainColor;
    }

    // --- Gizmos for Editor Visualization ---
    private void OnDrawGizmos()
    {
        // Draw lighting influence zones
        if (mainLight != null)
        {
            Gizmos.color = currentMainColor;
            Gizmos.DrawWireSphere(transform.position, 20f);
        }

        // Draw ambient light positions
        foreach (var light in ambientLights)
        {
            if (light != null)
            {
                Gizmos.color = light.color;
                Gizmos.DrawWireSphere(light.transform.position, 5f);
            }
        }
    }
} 