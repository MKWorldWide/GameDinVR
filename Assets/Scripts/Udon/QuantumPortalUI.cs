// QuantumPortalUI.cs
// Quantum-documented UI system for QuantumGatekeeper portal feedback
// Handles tier-locked messages, destination previews, and portal status display
// Modular, extendable, and ready for VRChat world-space UI integration
// Compatible with UdonSharp (VRChat) or standard Unity C#

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UI system for QuantumGatekeeper portal feedback and status display.
/// Handles tier-locked messages, destination previews, and portal status.
/// </summary>
public class QuantumPortalUI : UdonSharpBehaviour
{
    // --- UI Components ---
    [Header("Tier Locked UI")]
    [Tooltip("Main tier-locked message panel")]
    public GameObject tierLockedPanel;
    [Tooltip("Tier requirement text")]
    public TextMeshProUGUI tierRequirementText;
    [Tooltip("Player's current tier text")]
    public TextMeshProUGUI playerTierText;
    [Tooltip("Lore subtitle text")]
    public TextMeshProUGUI loreSubtitleText;
    [Tooltip("Tier glyph icon")]
    public Image tierGlyphImage;
    [Tooltip("Access denied icon")]
    public Image accessDeniedIcon;

    [Header("Portal Status UI")]
    [Tooltip("Portal status panel")]
    public GameObject portalStatusPanel;
    [Tooltip("Portal status text")]
    public TextMeshProUGUI portalStatusText;
    [Tooltip("Portal activation progress bar")]
    public Slider activationProgressBar;
    [Tooltip("Portal ready indicator")]
    public Image portalReadyIndicator;

    [Header("Destination Preview")]
    [Tooltip("Destination preview panel")]
    public GameObject previewPanel;
    [Tooltip("Destination preview image")]
    public RawImage previewImage;
    [Tooltip("Destination name text")]
    public TextMeshProUGUI destinationNameText;
    [Tooltip("Preview loading indicator")]
    public GameObject previewLoadingIndicator;

    [Header("Animation & Effects")]
    [Tooltip("UI animator component")]
    public Animator uiAnimator;
    [Tooltip("Fade in/out duration")]
    public float fadeDuration = 0.5f;
    [Tooltip("Auto-hide delay for messages")]
    public float autoHideDelay = 3.0f;

    // --- Tier Colors ---
    [Header("Tier Colors")]
    [Tooltip("Wanderer tier color")]
    public Color wandererColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    [Tooltip("Initiate tier color")]
    public Color initiateColor = new Color(0.2f, 0.8f, 0.2f, 1.0f);
    [Tooltip("Radiant tier color")]
    public Color radiantColor = new Color(0.8f, 0.8f, 0.2f, 1.0f);
    [Tooltip("Sovereign tier color")]
    public Color sovereignColor = new Color(0.8f, 0.2f, 0.8f, 1.0f);

    // --- Internal State ---
    private bool isUIVisible = false;
    private Coroutine autoHideCoroutine;
    private CanvasGroup mainCanvasGroup;

    // --- Unity Start: Initialize UI ---
    private void Start()
    {
        InitializeUI();
    }

    /// <summary>
    /// Initialize UI components and set initial state.
    /// </summary>
    private void InitializeUI()
    {
        // Get canvas group for fade effects
        mainCanvasGroup = GetComponent<CanvasGroup>();
        if (mainCanvasGroup == null)
        {
            mainCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Hide all panels initially
        if (tierLockedPanel != null) tierLockedPanel.SetActive(false);
        if (portalStatusPanel != null) portalStatusPanel.SetActive(false);
        if (previewPanel != null) previewPanel.SetActive(false);

        // Set initial alpha
        mainCanvasGroup.alpha = 0f;
        isUIVisible = false;
    }

    /// <summary>
    /// Show tier-locked message with player and required tier information.
    /// </summary>
    public void ShowTierLockedMessage(string playerTier, string requiredTier, string loreSubtitle = "")
    {
        if (tierLockedPanel == null) return;

        // Update text components
        if (tierRequirementText != null)
        {
            tierRequirementText.text = $"{requiredTier} Required";
            tierRequirementText.color = GetTierColor(requiredTier);
        }

        if (playerTierText != null)
        {
            playerTierText.text = $"Your Tier: {playerTier}";
            playerTierText.color = GetTierColor(playerTier);
        }

        if (loreSubtitleText != null && !string.IsNullOrEmpty(loreSubtitle))
        {
            loreSubtitleText.text = loreSubtitle;
        }

        // Update tier glyph
        if (tierGlyphImage != null)
        {
            tierGlyphImage.color = GetTierColor(requiredTier);
        }

        // Show panel with animation
        ShowPanel(tierLockedPanel);
        
        // Auto-hide after delay
        StartAutoHide();
    }

    /// <summary>
    /// Show portal status with activation progress.
    /// </summary>
    public void ShowPortalStatus(string status, float progress = 0f)
    {
        if (portalStatusPanel == null) return;

        // Update status text
        if (portalStatusText != null)
        {
            portalStatusText.text = status;
        }

        // Update progress bar
        if (activationProgressBar != null)
        {
            activationProgressBar.value = progress;
        }

        // Update ready indicator
        if (portalReadyIndicator != null)
        {
            portalReadyIndicator.color = progress >= 1f ? Color.green : Color.yellow;
        }

        // Show panel
        ShowPanel(portalStatusPanel);
    }

    /// <summary>
    /// Show destination preview with image and name.
    /// </summary>
    public void ShowDestinationPreview(Texture previewTexture, string destinationName)
    {
        if (previewPanel == null) return;

        // Update preview image
        if (previewImage != null && previewTexture != null)
        {
            previewImage.texture = previewTexture;
        }

        // Update destination name
        if (destinationNameText != null)
        {
            destinationNameText.text = destinationName;
        }

        // Hide loading indicator
        if (previewLoadingIndicator != null)
        {
            previewLoadingIndicator.SetActive(false);
        }

        // Show panel
        ShowPanel(previewPanel);
    }

    /// <summary>
    /// Show loading state for destination preview.
    /// </summary>
    public void ShowPreviewLoading()
    {
        if (previewPanel == null) return;

        // Show loading indicator
        if (previewLoadingIndicator != null)
        {
            previewLoadingIndicator.SetActive(true);
        }

        // Clear preview image
        if (previewImage != null)
        {
            previewImage.texture = null;
        }

        // Show panel
        ShowPanel(previewPanel);
    }

    /// <summary>
    /// Hide all UI panels with fade effect.
    /// </summary>
    public void HideAllUI()
    {
        StopAutoHide();
        StartCoroutine(FadeOutUI());
    }

    /// <summary>
    /// Show a specific panel with fade effect.
    /// </summary>
    private void ShowPanel(GameObject panel)
    {
        if (panel == null) return;

        // Hide other panels
        if (tierLockedPanel != null && tierLockedPanel != panel) tierLockedPanel.SetActive(false);
        if (portalStatusPanel != null && portalStatusPanel != panel) portalStatusPanel.SetActive(false);
        if (previewPanel != null && previewPanel != panel) previewPanel.SetActive(false);

        // Show target panel
        panel.SetActive(true);

        // Fade in UI
        if (!isUIVisible)
        {
            StartCoroutine(FadeInUI());
        }

        // Trigger animation if available
        if (uiAnimator != null)
        {
            uiAnimator.SetTrigger("Show");
        }
    }

    /// <summary>
    /// Get color associated with a tier name.
    /// </summary>
    private Color GetTierColor(string tierName)
    {
        switch (tierName.ToLower())
        {
            case "wanderer": return wandererColor;
            case "initiate": return initiateColor;
            case "radiant": return radiantColor;
            case "sovereign": return sovereignColor;
            default: return Color.white;
        }
    }

    /// <summary>
    /// Start auto-hide timer.
    /// </summary>
    private void StartAutoHide()
    {
        StopAutoHide();
        autoHideCoroutine = StartCoroutine(AutoHideCoroutine());
    }

    /// <summary>
    /// Stop auto-hide timer.
    /// </summary>
    private void StopAutoHide()
    {
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
    }

    /// <summary>
    /// Auto-hide coroutine.
    /// </summary>
    private IEnumerator AutoHideCoroutine()
    {
        yield return new WaitForSeconds(autoHideDelay);
        HideAllUI();
    }

    /// <summary>
    /// Fade in UI effect.
    /// </summary>
    private IEnumerator FadeInUI()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            mainCanvasGroup.alpha = alpha;
            yield return null;
        }
        mainCanvasGroup.alpha = 1f;
        isUIVisible = true;
    }

    /// <summary>
    /// Fade out UI effect.
    /// </summary>
    private IEnumerator FadeOutUI()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mainCanvasGroup.alpha = alpha;
            yield return null;
        }
        mainCanvasGroup.alpha = 0f;
        isUIVisible = false;

        // Hide all panels
        if (tierLockedPanel != null) tierLockedPanel.SetActive(false);
        if (portalStatusPanel != null) portalStatusPanel.SetActive(false);
        if (previewPanel != null) previewPanel.SetActive(false);
    }

    // --- Public Methods for External Control ---

    /// <summary>
    /// Public method to show a custom message.
    /// </summary>
    public void ShowCustomMessage(string message, float duration = 3f)
    {
        if (portalStatusText != null)
        {
            portalStatusText.text = message;
        }
        ShowPanel(portalStatusPanel);
        
        // Auto-hide after custom duration
        StartCoroutine(CustomAutoHide(duration));
    }

    /// <summary>
    /// Custom auto-hide with specified duration.
    /// </summary>
    private IEnumerator CustomAutoHide(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideAllUI();
    }

    /// <summary>
    /// Public method to check if UI is currently visible.
    /// </summary>
    public bool IsUIVisible()
    {
        return isUIVisible;
    }

    /// <summary>
    /// Public method to force hide UI immediately.
    /// </summary>
    public void ForceHideUI()
    {
        StopAutoHide();
        mainCanvasGroup.alpha = 0f;
        isUIVisible = false;
        
        if (tierLockedPanel != null) tierLockedPanel.SetActive(false);
        if (portalStatusPanel != null) portalStatusPanel.SetActive(false);
        if (previewPanel != null) previewPanel.SetActive(false);
    }
} 