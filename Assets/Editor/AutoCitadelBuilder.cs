// AutoCitadelBuilder.cs
// Quantum-documented Unity Editor script for auto-building the Citadel of Resonance scene
// Inspired by Skyrim x Infinity Blade cathedral, for VRChat + UdonSharp
// Adds menu: GameDinVR > Build Citadel Scene
// Modular, extensible, and deeply commented for rapid iteration

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Auto-builds the Citadel of Resonance: floor, temple, throne, floating stairs, crystal pillars, rings, islands, and placeholders.
/// Assigns placeholder materials and UdonSharp scripts. Modular for future expansion.
/// Now includes: Audio assignment, GDI tier gates, scene validation, and menu for validation.
/// </summary>
public class AutoCitadelBuilder : EditorWindow
{
    // --- AudioClip fields for smart assignment ---
    public AudioClip ambientSFXClip;
    public AudioClip reverbMusicClip;

    [MenuItem("GameDinVR/Build Citadel Scene")]
    public static void BuildCitadelScene()
    {
        // --- Scene Setup ---
        string scenePath = "Assets/Scenes/CitadelOfResonance.unity";
        if (!File.Exists(scenePath))
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(newScene, scenePath);
        }
        EditorSceneManager.OpenScene(scenePath);

        // --- Materials ---
        Material marble = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/StoneGlow.mat");
        Material crystal = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/NeonCrystal.mat");

        // --- Floor ---
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        floor.name = "CathedralFloor";
        floor.transform.localScale = new Vector3(20, 0.5f, 20);
        if (marble) floor.GetComponent<Renderer>().sharedMaterial = marble;

        // --- Central Temple ---
        GameObject temple = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temple.name = "CentralTemple";
        temple.transform.position = new Vector3(0, 2, 0);
        temple.transform.localScale = new Vector3(8, 8, 8);
        if (marble) temple.GetComponent<Renderer>().sharedMaterial = marble;

        // --- High Throne Area ---
        GameObject throneBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        throneBase.name = "ThroneBase";
        throneBase.transform.position = new Vector3(0, 7, -6);
        throneBase.transform.localScale = new Vector3(3, 1, 3);
        if (crystal) throneBase.GetComponent<Renderer>().sharedMaterial = crystal;
        // Add throne seat
        GameObject throne = GameObject.CreatePrimitive(PrimitiveType.Cube);
        throne.name = "Throne";
        throne.transform.position = new Vector3(0, 9, -6);
        throne.transform.localScale = new Vector3(1.5f, 3, 1);
        if (crystal) throne.GetComponent<Renderer>().sharedMaterial = crystal;

        // --- Floating Stairs ---
        for (int i = 0; i < 10; i++)
        {
            GameObject stair = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stair.name = $"FloatingStair_{i}";
            stair.transform.position = new Vector3(Mathf.Sin(i * 0.4f) * 6, 1.5f + i * 0.7f, Mathf.Cos(i * 0.4f) * 6);
            stair.transform.localScale = new Vector3(2, 0.3f, 1);
            if (marble) stair.GetComponent<Renderer>().sharedMaterial = marble;
        }

        // --- Crystal Pillars ---
        for (int i = 0; i < 6; i++)
        {
            float angle = i * Mathf.PI * 2 / 6;
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = $"CrystalPillar_{i}";
            pillar.transform.position = new Vector3(Mathf.Cos(angle) * 10, 4, Mathf.Sin(angle) * 10);
            pillar.transform.localScale = new Vector3(0.7f, 8, 0.7f);
            if (crystal) pillar.GetComponent<Renderer>().sharedMaterial = crystal;
        }

        // --- Platform Rings ---
        for (int i = 0; i < 3; i++)
        {
            float radius = 14 + i * 3;
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Torus);
            ring.name = $"PlatformRing_{i}";
            ring.transform.position = new Vector3(0, 2 + i * 2, 0);
            ring.transform.localScale = new Vector3(radius, 0.2f, radius);
            if (crystal) ring.GetComponent<Renderer>().sharedMaterial = crystal;
        }

        // --- Floating Islands ---
        for (int i = 0; i < 4; i++)
        {
            float angle = i * Mathf.PI * 2 / 4;
            GameObject island = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            island.name = $"FloatingIsland_{i}";
            island.transform.position = new Vector3(Mathf.Cos(angle) * 25, 8, Mathf.Sin(angle) * 25);
            island.transform.localScale = new Vector3(6, 2, 6);
            if (marble) island.GetComponent<Renderer>().sharedMaterial = marble;
        }

        // --- Placeholders ---
        GameObject gdiPanel = new GameObject("GDIHologramPanel");
        gdiPanel.transform.position = new Vector3(3, 3, 0);
        // Add TokenStatusDisplay script if available
        var tokenStatusType = GetTypeByName("TokenStatusDisplay");
        if (tokenStatusType != null)
            gdiPanel.AddComponent(tokenStatusType);

        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = "CelestialSpiritsTavern";
        bar.transform.position = new Vector3(-6, 2, 0);
        bar.transform.localScale = new Vector3(4, 2, 4);
        if (marble) bar.GetComponent<Renderer>().sharedMaterial = marble;

        // --- Portals ---
        string[] portalNames = { "GenesisCorePortal", "CouncilRoomPortal", "ArcadePortal" };
        Vector3[] portalPositions = { new Vector3(0, 2, 18), new Vector3(-16, 2, 0), new Vector3(16, 2, 0) };
        for (int i = 0; i < portalNames.Length; i++)
        {
            GameObject portal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            portal.name = portalNames[i];
            portal.transform.position = portalPositions[i];
            portal.transform.localScale = new Vector3(2, 0.2f, 2);
            if (crystal) portal.GetComponent<Renderer>().sharedMaterial = crystal;
            // Add TeleportTrigger script if available
            var teleportType = GetTypeByName("TeleportTrigger");
            if (teleportType != null)
                portal.AddComponent(teleportType);
        }

        // --- Lighting ---
        GameObject mainLight = new GameObject("MainDirectionalLight");
        var light = mainLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.8f, 0.9f, 1f, 1f); // Divine blue
        light.intensity = 1.2f;
        mainLight.transform.rotation = Quaternion.Euler(50, 30, 0);

        GameObject glowLight = new GameObject("DivineGlowLight");
        var glow = glowLight.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = new Color(1f, 0.85f, 0.4f, 1f); // Golden glow
        glow.intensity = 8f;
        glow.range = 30f;
        glowLight.transform.position = new Vector3(0, -2, 0);

        // --- Music Reverb & Ambient SFX (with smart assignment) ---
        AssignOrCreateAudioZone("AmbientSFXZone", "AmbientSFXLoop", typeof(AudioSource), GetAudioClip("ambientSFXClip"));
        AssignOrCreateAudioZone("ReverbMusicZone", "MusicReverbZone", typeof(AudioReverbZone), GetAudioClip("reverbMusicClip"));

        // --- WorldManager (assign in Unity) ---
        GameObject worldManager = new GameObject("WorldManager");
        // Add WorldManager script if available
        var worldManagerType = GetTypeByName("WorldManager");
        if (worldManagerType != null)
            worldManager.AddComponent(worldManagerType);

        // --- GDI Tier Collider Zones ---
        CreateGDIAccessGate("RadiantAccessZone", new Vector3(-10, 2, 10), "Radiant");
        CreateGDIAccessGate("SovereignAccessZone", new Vector3(10, 2, 10), "Sovereign");
        CreateGDIAccessGate("GenesisCoreGate", new Vector3(0, 2, -18), "GenesisCore");

        // --- Save Scene ---
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[GameDinVR] Citadel of Resonance scene auto-built!");
        ValidateCitadelScene();
    }

    /// <summary>
    /// Utility to get a Type by name from loaded assemblies (for UdonSharp scripts).
    /// </summary>
    private static Type GetTypeByName(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);
    }

    // --- Helper: Assign or Create Audio Zone ---
    private static void AssignOrCreateAudioZone(string zoneName, string fallbackName, Type componentType, AudioClip clip)
    {
        GameObject zone = GameObject.Find(zoneName) ?? GameObject.Find(fallbackName);
        if (!zone)
        {
            zone = new GameObject(zoneName);
            if (componentType == typeof(AudioReverbZone))
                zone.AddComponent<AudioReverbZone>();
        }
        var audioSource = zone.GetComponent<AudioSource>() ?? zone.AddComponent<AudioSource>();
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
        }
    }

    // --- Helper: Get AudioClip from EditorWindow fields ---
    private static AudioClip GetAudioClip(string fieldName)
    {
        var window = GetWindow<AutoCitadelBuilder>();
        var field = typeof(AutoCitadelBuilder).GetField(fieldName);
        return field != null ? field.GetValue(window) as AudioClip : null;
    }

    // --- Helper: Create GDI Tier Access Gate ---
    private static void CreateGDIAccessGate(string name, Vector3 pos, string tier)
    {
        GameObject gate = new GameObject(name);
        gate.transform.position = pos;
        var collider = gate.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        gate.tag = "GDIAccessGate";
        var accessType = GetTypeByName("GDIAccessGate");
        if (accessType != null)
        {
            var script = gate.AddComponent(accessType);
            var tierField = accessType.GetField("requiredTier");
            if (tierField != null)
                tierField.SetValue(script, tier);
        }
    }

    // --- Scene Validation ---
    [MenuItem("GameDinVR/Validate Scene Integrity")]
    public static void ValidateCitadelScene()
    {
        int portalCount = GameObject.FindObjectsOfType<GameObject>().Count(go => go.GetComponent(GetTypeByName("TeleportTrigger")) != null);
        int tokenDisplayCount = GameObject.FindObjectsOfType<GameObject>().Count(go => go.GetComponent(GetTypeByName("TokenStatusDisplay")) != null);
        int throneCount = GameObject.FindObjectsOfType<GameObject>().Count(go => go.name.ToLower().Contains("throne"));
        int gateCount = GameObject.FindObjectsOfType<GameObject>().Count(go => go.CompareTag("GDIAccessGate"));
        Debug.Log($"[GameDinVR] Scene Validation Report:\n Portals with TeleportTrigger: {portalCount}\n TokenStatusDisplay objects: {tokenDisplayCount}\n Thrones: {throneCount}\n GDI Tier Gates: {gateCount}");
        // Warnings for missing essentials
        if (portalCount < 3) Debug.LogWarning("[GameDinVR] Warning: Fewer than 3 portals with TeleportTrigger found.");
        if (tokenDisplayCount < 1) Debug.LogWarning("[GameDinVR] Warning: No TokenStatusDisplay found.");
        if (throneCount < 1) Debug.LogWarning("[GameDinVR] Warning: No throne found.");
        if (gateCount < 3) Debug.LogWarning("[GameDinVR] Warning: Fewer than 3 GDI Tier Gates found.");
    }
} 