# QuantumGatekeeper System Documentation

## Overview

The **QuantumGatekeeper** is an advanced tiered teleportation and access control system for The Citadel of Resonance VRChat world. It provides sophisticated portal management with visual effects, audio feedback, destination previews, and multiplayer sync preparation.

## 🧬 Core Features

### Tier System
- **GDI_Tier Enum**: Wanderer (0), Initiate (1), Radiant (2), Sovereign (3)
- **Automatic Tier Detection**: Integrates with QuantumSessionVerifier and PlayerTierBridge
- **Manual Override**: Developer testing with tier bypass options
- **Tier-based Visual Feedback**: Color-coded glyphs and portal effects

### Destination Management
- **Transform Teleportation**: Teleport to in-scene transforms
- **Scene Loading**: Load new VRChat scenes (future API support)
- **Destination Previews**: RenderTexture-based preview system
- **Lore Integration**: Customizable subtitle text for each portal

### Visual & Audio Effects
- **Portal VFX**: Configurable particle systems and prefabs
- **Tier Glyphs**: Glowing, pulsing tier requirement indicators
- **Audio Feedback**: Portal activation, denial, and voice whispers
- **Animation Support**: Animator trigger integration
- **Fade Transitions**: Smooth teleportation with fade effects

## 🔐 Developer & Debug Features

### Debug Mode
- **Tier Bypass**: Override tier requirements for testing
- **Developer Override**: Set custom tier for development
- **Debug Logging**: Comprehensive logging system
- **Gizmo Visualization**: Scene view helpers for portal zones

### Inspector Controls
- **Manual Tier Override**: Set player tier manually
- **Component Auto-Detection**: Automatic session verifier and tier bridge detection
- **Event System**: UnityEvents for portal activation/denial
- **Real-time Updates**: Dynamic tier and destination changes

## 🌀 Symbolic Features

### Tier Glyph System
- **Color-coded Icons**: Each tier has unique color (Gray, Green, Gold, Purple)
- **Pulse Animation**: Active portals pulse with tier color
- **Dynamic Opacity**: Brightness based on portal state

### Destination Preview System
- **RenderTexture Capture**: Real-time destination previews
- **Shader-based Display**: Preview planes with custom materials
- **Loading States**: Visual feedback during preview generation
- **Camera Positioning**: Automatic preview camera setup

### Lore Integration
- **Custom Subtitles**: "The Chamber Awaits..." style messages
- **Tier-specific Messages**: Different lore for each access level
- **Dynamic Text**: Real-time tier requirement display

## 🌐 Multiplayer Support (Future)

### Planned Features
- **Minimum Player Requirements**: Require X players of Y tier
- **Cross-client Sync**: Portal state synchronization
- **Network Events**: Photon/Udon networking integration
- **Shared Activation**: Multiplayer portal activation

## 📖 Setup Instructions

### 1. Basic Portal Setup

```csharp
// Add QuantumGatekeeper to a GameObject with trigger collider
GameObject portal = new GameObject("MyPortal");
portal.AddComponent<BoxCollider>().isTrigger = true;
portal.AddComponent<QuantumGatekeeper>();

// Configure required tier
var gatekeeper = portal.GetComponent<QuantumGatekeeper>();
gatekeeper.requiredTier = QuantumGatekeeper.GDI_Tier.Radiant;
```

### 2. Auto-Detection Setup

```csharp
// The system automatically detects these components:
// - QuantumSessionVerifier (for player authentication)
// - PlayerTierBridge (for tier fetching)

// Manual assignment is also supported:
gatekeeper.sessionVerifier = FindObjectOfType<QuantumSessionVerifier>();
gatekeeper.tierBridge = FindObjectOfType<PlayerTierBridge>();
```

### 3. Visual Effects Setup

```csharp
// Portal VFX
gatekeeper.portalVFXPrefab = yourVFXPrefab;
gatekeeper.portalParticles = yourParticleSystem;
gatekeeper.portalGlowMaterial = yourGlowMaterial;

// Tier Glyph
gatekeeper.tierGlyphIcon = yourGlyphGameObject;

// Preview System
gatekeeper.destinationPreview = yourRenderTexture;
gatekeeper.previewPlane = yourPreviewPlane;
```

### 4. Audio Setup

```csharp
// Portal Sounds
gatekeeper.portalActivationSound = yourActivationClip;
gatekeeper.portalDenialSound = yourDenialClip;
gatekeeper.voiceWhispersSound = yourWhispersClip;

// Audio Source (auto-created if not assigned)
gatekeeper.portalAudioSource = yourAudioSource;
```

## 🎮 Usage Examples

### Basic Portal Configuration

```csharp
// Create a Radiant-tier portal to a specific transform
var portal = CreatePortal("RadiantPortal", Vector3.zero);
var gatekeeper = portal.GetComponent<QuantumGatekeeper>();

gatekeeper.requiredTier = QuantumGatekeeper.GDI_Tier.Radiant;
gatekeeper.destinationTransform = someTransform;
gatekeeper.useSceneName = false;
gatekeeper.loreSubtitle = "The Radiant Chamber Awaits...";
```

### Scene Loading Portal

```csharp
// Create a portal that loads a new scene
var portal = CreatePortal("ScenePortal", Vector3.zero);
var gatekeeper = portal.GetComponent<QuantumGatekeeper>();

gatekeeper.requiredTier = QuantumGatekeeper.GDI_Tier.Sovereign;
gatekeeper.useSceneName = true;
gatekeeper.destinationSceneName = "TheCitadel_InnerSanctum";
gatekeeper.loreSubtitle = "The Sovereign Sanctum Beckons...";
```

### Debug Testing Portal

```csharp
// Create a debug portal for testing
var portal = CreatePortal("DebugPortal", Vector3.zero);
var gatekeeper = portal.GetComponent<QuantumGatekeeper>();

gatekeeper.debugMode = true;
gatekeeper.bypassTierInDebug = true;
gatekeeper.devTierOverride = QuantumGatekeeper.GDI_Tier.Sovereign;
```

## 🔧 Integration with Other Systems

### QuantumSessionVerifier Integration

```csharp
// The QuantumGatekeeper automatically uses the session verifier
// to get the player's address and validate their session

// Manual integration:
var sessionVerifier = FindObjectOfType<QuantumSessionVerifier>();
sessionVerifier.onSuccess.AddListener(() => {
    // Portal access granted
});
sessionVerifier.onFailure.AddListener(() => {
    // Portal access denied
});
```

### PlayerTierBridge Integration

```csharp
// The QuantumGatekeeper automatically uses the tier bridge
// to fetch the player's GDI tier from the backend

// Manual integration:
var tierBridge = FindObjectOfType<PlayerTierBridge>();
string playerTier = tierBridge.GetTier(playerAddress);
```

### UnityEvents Integration

```csharp
// Connect to portal events
gatekeeper.onPortalActivated.AddListener(() => {
    Debug.Log("Portal activated!");
});

gatekeeper.onPortalDenied.AddListener(() => {
    Debug.Log("Portal access denied!");
});

gatekeeper.onPlayerEntered.AddListener(() => {
    Debug.Log("Player entered portal zone!");
});
```

## 🎨 Customization

### Custom Tier Colors

```csharp
// Override default tier colors in QuantumPortalUI
var portalUI = FindObjectOfType<QuantumPortalUI>();
portalUI.wandererColor = Color.gray;
portalUI.initiateColor = Color.green;
portalUI.radiantColor = Color.yellow;
portalUI.sovereignColor = Color.purple;
```

### Custom Portal Effects

```csharp
// Create custom portal VFX
var vfxPrefab = CreatePortalVFX();
gatekeeper.portalVFXPrefab = vfxPrefab;

// Create custom tier glyph
var glyphPrefab = CreateTierGlyph();
gatekeeper.tierGlyphIcon = glyphPrefab;
```

### Custom Audio Setup

```csharp
// Setup custom portal audio
var audioSource = portal.AddComponent<AudioSource>();
audioSource.spatialBlend = 1.0f; // 3D audio
audioSource.volume = 0.8f;
audioSource.maxDistance = 10f;

gatekeeper.portalAudioSource = audioSource;
```

## 🚀 Performance Considerations

### Optimization Tips

1. **RenderTexture Management**: Use appropriate resolution for previews
2. **Particle System Limits**: Limit particle count for portal VFX
3. **Audio Source Pooling**: Reuse audio sources for multiple portals
4. **Layer-based Rendering**: Use preview layers for destination capture
5. **Event Cleanup**: Properly unsubscribe from UnityEvents

### Memory Management

```csharp
// Clean up RenderTextures
private void OnDestroy()
{
    if (previewTexture != null)
    {
        previewTexture.Release();
    }
}

// Clean up event listeners
private void OnDisable()
{
    onPortalActivated.RemoveAllListeners();
    onPortalDenied.RemoveAllListeners();
}
```

## 🔮 Future Enhancements

### Planned Features

1. **WalletConnect Integration**: Direct Web3 wallet authentication
2. **Advanced VFX**: Shader-based portal effects
3. **VR Haptics**: Haptic feedback for portal interactions
4. **World-space UI**: In-world portal status displays
5. **Automated VRChat Upload**: CI/CD pipeline integration

### Multiplayer Sync

```csharp
// Future multiplayer implementation
private void SyncPortalState()
{
    // Sync portal activation across all clients
    // Using VRChat networking or Photon
}

private bool CheckMultiplayerRequirements()
{
    // Check if minimum players with required tier are present
    // Return true if requirements are met
}
```

## 📋 Troubleshooting

### Common Issues

1. **Portal Not Activating**: Check tier requirements and session verification
2. **Preview Not Showing**: Verify RenderTexture setup and preview camera
3. **Audio Not Playing**: Check AudioSource configuration and clip assignment
4. **VFX Not Displaying**: Verify particle system or VFX prefab assignment

### Debug Commands

```csharp
// Force portal activation
gatekeeper.ManualActivatePortal();

// Check portal status
bool isActive = gatekeeper.IsPortalActive();

// Set tier dynamically
gatekeeper.SetRequiredTier(QuantumGatekeeper.GDI_Tier.Initiate);
```

## 📚 Related Systems

- **QuantumSessionVerifier**: Player authentication and session management
- **PlayerTierBridge**: GDI tier fetching and caching
- **QuantumPortalUI**: Portal feedback and status display
- **DestinationPreviewRenderer**: Destination preview capture system
- **GDIAccessGate**: Basic tier-based access control
- **QuantumSigilChamber**: Advanced chamber unlocking system

---

*This documentation is part of the GameDinVR project for The Citadel of Resonance VRChat world. For more information, see the main README.md file.* 