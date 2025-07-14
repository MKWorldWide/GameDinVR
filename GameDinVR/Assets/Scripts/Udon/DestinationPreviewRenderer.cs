// DestinationPreviewRenderer.cs
// Quantum-documented destination preview capture system for QuantumGatekeeper
// Renders destination areas to RenderTexture for portal previews
// Modular, extendable, and optimized for VRChat performance
// Compatible with UdonSharp (VRChat) or standard Unity C#

using UdonSharp;
using UnityEngine;
using System.Collections;

/// <summary>
/// Captures destination previews for QuantumGatekeeper portal system.
/// Renders destination areas to RenderTexture for portal previews.
/// </summary>
public class DestinationPreviewRenderer : UdonSharpBehaviour
{
    // --- Preview Settings ---
    [Header("Preview Settings")]
    [Tooltip("RenderTexture for destination preview")]
    public RenderTexture previewTexture;
    [Tooltip("Preview camera for capturing destination")]
    public Camera previewCamera;
    [Tooltip("Preview resolution width")]
    public int previewWidth = 512;
    [Tooltip("Preview resolution height")]
    public int previewHeight = 512;
    [Tooltip("Preview camera field of view")]
    public float previewFOV = 60f;
    [Tooltip("Preview camera near clip plane")]
    public float previewNearClip = 0.1f;
    [Tooltip("Preview camera far clip plane")]
    public float previewFarClip = 100f;

    [Header("Destination Setup")]
    [Tooltip("Destination area to preview")]
    public Transform destinationArea;
    [Tooltip("Preview camera position offset from destination")]
    public Vector3 cameraOffset = new Vector3(0f, 2f, -5f);
    [Tooltip("Preview camera look target")]
    public Transform lookTarget;
    [Tooltip("Auto-capture preview on start")]
    public bool autoCaptureOnStart = true;

    [Header("Rendering")]
    [Tooltip("Rendering layer for preview objects")]
    public int previewLayer = 8;
    [Tooltip("Background color for preview")]
    public Color backgroundColor = Color.black;
    [Tooltip("Enable post-processing effects in preview")]
    public bool enablePostProcessing = false;

    // --- Internal State ---
    private bool isPreviewReady = false;
    private bool isCapturing = false;
    private RenderTexture originalTargetTexture;

    // --- Unity Start: Initialize Preview System ---
    private void Start()
    {
        InitializePreviewSystem();
        
        if (autoCaptureOnStart)
        {
            StartCoroutine(CapturePreviewDelayed());
        }
    }

    /// <summary>
    /// Initialize the preview rendering system.
    /// </summary>
    private void InitializePreviewSystem()
    {
        // Create preview camera if not assigned
        if (previewCamera == null)
        {
            CreatePreviewCamera();
        }

        // Create RenderTexture if not assigned
        if (previewTexture == null)
        {
            CreatePreviewTexture();
        }

        // Setup preview camera
        SetupPreviewCamera();

        // Setup destination area
        SetupDestinationArea();
    }

    /// <summary>
    /// Create a preview camera for capturing destination.
    /// </summary>
    private void CreatePreviewCamera()
    {
        GameObject cameraObj = new GameObject("DestinationPreviewCamera");
        cameraObj.transform.SetParent(transform);
        previewCamera = cameraObj.AddComponent<Camera>();
        
        // Position camera
        if (destinationArea != null)
        {
            cameraObj.transform.position = destinationArea.position + cameraOffset;
            if (lookTarget != null)
            {
                cameraObj.transform.LookAt(lookTarget);
            }
            else
            {
                cameraObj.transform.LookAt(destinationArea);
            }
        }
    }

    /// <summary>
    /// Create RenderTexture for preview capture.
    /// </summary>
    private void CreatePreviewTexture()
    {
        previewTexture = new RenderTexture(previewWidth, previewHeight, 24);
        previewTexture.name = "DestinationPreview";
        previewTexture.Create();
    }

    /// <summary>
    /// Setup preview camera settings.
    /// </summary>
    private void SetupPreviewCamera()
    {
        if (previewCamera == null) return;

        // Basic camera settings
        previewCamera.targetTexture = previewTexture;
        previewCamera.fieldOfView = previewFOV;
        previewCamera.nearClipPlane = previewNearClip;
        previewCamera.farClipPlane = previewFarClip;
        previewCamera.backgroundColor = backgroundColor;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.cullingMask = 1 << previewLayer; // Only render preview layer
        previewCamera.enabled = false; // Disable by default, only enable when capturing

        // Disable audio listener if present
        AudioListener audioListener = previewCamera.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
    }

    /// <summary>
    /// Setup destination area for preview capture.
    /// </summary>
    private void SetupDestinationArea()
    {
        if (destinationArea == null) return;

        // Move destination area to preview layer
        SetLayerRecursively(destinationArea.gameObject, previewLayer);

        // Ensure destination area is active for preview
        destinationArea.gameObject.SetActive(true);
    }

    /// <summary>
    /// Set layer recursively for all child objects.
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    /// <summary>
    /// Capture preview with delay to ensure setup is complete.
    /// </summary>
    private IEnumerator CapturePreviewDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        CapturePreview();
    }

    /// <summary>
    /// Capture destination preview.
    /// </summary>
    public void CapturePreview()
    {
        if (isCapturing || previewCamera == null || previewTexture == null)
        {
            Debug.LogWarning("[DestinationPreviewRenderer] Cannot capture preview - system not ready");
            return;
        }

        StartCoroutine(CapturePreviewCoroutine());
    }

    /// <summary>
    /// Coroutine to capture preview asynchronously.
    /// </summary>
    private IEnumerator CapturePreviewCoroutine()
    {
        isCapturing = true;

        // Store original camera settings
        Camera originalCamera = Camera.main;
        if (originalCamera != null)
        {
            originalTargetTexture = originalCamera.targetTexture;
        }

        // Enable preview camera
        previewCamera.enabled = true;

        // Wait for camera to render
        yield return new WaitForEndOfFrame();

        // Capture the frame
        previewCamera.Render();

        // Disable preview camera
        previewCamera.enabled = false;

        // Restore original camera settings
        if (originalCamera != null)
        {
            originalCamera.targetTexture = originalTargetTexture;
        }

        isCapturing = false;
        isPreviewReady = true;

        Debug.Log("[DestinationPreviewRenderer] Preview captured successfully");
    }

    /// <summary>
    /// Get the preview texture.
    /// </summary>
    public RenderTexture GetPreviewTexture()
    {
        return isPreviewReady ? previewTexture : null;
    }

    /// <summary>
    /// Check if preview is ready.
    /// </summary>
    public bool IsPreviewReady()
    {
        return isPreviewReady;
    }

    /// <summary>
    /// Update preview camera position and rotation.
    /// </summary>
    public void UpdatePreviewCamera(Vector3 position, Vector3 lookAt)
    {
        if (previewCamera == null) return;

        previewCamera.transform.position = position;
        previewCamera.transform.LookAt(lookAt);
    }

    /// <summary>
    /// Update preview camera with offset from destination.
    /// </summary>
    public void UpdatePreviewCameraWithOffset(Vector3 offset)
    {
        if (previewCamera == null || destinationArea == null) return;

        cameraOffset = offset;
        Vector3 newPosition = destinationArea.position + offset;
        previewCamera.transform.position = newPosition;
        
        if (lookTarget != null)
        {
            previewCamera.transform.LookAt(lookTarget);
        }
        else
        {
            previewCamera.transform.LookAt(destinationArea);
        }
    }

    /// <summary>
    /// Set destination area for preview.
    /// </summary>
    public void SetDestinationArea(Transform newDestination)
    {
        destinationArea = newDestination;
        SetupDestinationArea();
        
        // Update camera position
        if (previewCamera != null && destinationArea != null)
        {
            previewCamera.transform.position = destinationArea.position + cameraOffset;
            if (lookTarget != null)
            {
                previewCamera.transform.LookAt(lookTarget);
            }
            else
            {
                previewCamera.transform.LookAt(destinationArea);
            }
        }
    }

    /// <summary>
    /// Set look target for preview camera.
    /// </summary>
    public void SetLookTarget(Transform newLookTarget)
    {
        lookTarget = newLookTarget;
        if (previewCamera != null && lookTarget != null)
        {
            previewCamera.transform.LookAt(lookTarget);
        }
    }

    // --- Public Methods for External Control ---

    /// <summary>
    /// Public method to force refresh preview.
    /// </summary>
    public void RefreshPreview()
    {
        isPreviewReady = false;
        CapturePreview();
    }

    /// <summary>
    /// Public method to get preview texture as Texture2D (for UI).
    /// </summary>
    public Texture2D GetPreviewAsTexture2D()
    {
        if (!isPreviewReady || previewTexture == null) return null;

        // Create Texture2D from RenderTexture
        Texture2D texture = new Texture2D(previewWidth, previewHeight, TextureFormat.RGB24, false);
        RenderTexture.active = previewTexture;
        texture.ReadPixels(new Rect(0, 0, previewWidth, previewHeight), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        return texture;
    }

    /// <summary>
    /// Public method to check if system is capturing.
    /// </summary>
    public bool IsCapturing()
    {
        return isCapturing;
    }

    // --- Cleanup ---
    private void OnDestroy()
    {
        if (previewTexture != null)
        {
            previewTexture.Release();
        }
    }
} 