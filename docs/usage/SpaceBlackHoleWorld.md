# Space Black Hole World Setup

Quantum-documented walkthrough for constructing a space-themed VRChat world featuring a distant black hole.

For overarching design principles, see [ARCHITECTURE.md](../../ARCHITECTURE.md). General project onboarding lives in the [root README](../../README.md), while narrative context resides in [THE_LAST_WHISPER.md](../../THE_LAST_WHISPER.md).

## Prerequisites
- Windows 10/11 with VRChat Creator Companion (VCC)
- Unity version matching VRChat's current LTS recommendation
- Packages: **VRChat SDK3 – Worlds**, **UdonSharp**, **CyanEmu**

## Step-by-Step
1. **Generate Project**
   - Via VCC GUI or PowerShell:
     ```powershell
     vrcc.exe create-project GameDinVR.SpaceWorld
     ```
2. **Install Packages**
   - Open VCC → *Manage Project* → add **SDK3 – Worlds**, **UdonSharp**, **CyanEmu**.
3. **Scene Creation**
   - Create scene `Assets/GameDinVR/Scenes/SpaceWorld.unity`.
   - Import a skybox material with starfield imagery.
   - Drop a large sphere far from origin for the black hole; apply emissive texture.
4. **Black Hole Logic**
   - Add an empty GameObject `BlackHoleController` at origin.
   - Attach `BlackHoleAttractor.cs`.
   - Assign the black hole sphere's transform to `eventHorizon`.
5. **Gemini Orb Integration**
   - Reuse `GeminiOrb.prefab` and `OSCResponseRouter` from project root.
   - Position near spawn for quick interaction.
6. **Build & Publish**
   - VRChat SDK → *Build & Publish for Windows*.
   - Provide world title, description, and preview image.

## PowerShell Automation
```powershell
# setup.ps1 – reproducible project generation
vrcc.exe create-project GameDinVR.SpaceWorld
vrcc.exe add-package GameDinVR.SpaceWorld vrcsdk3-worlds
vrcc.exe add-package GameDinVR.SpaceWorld udonsharp
```

## Further Enhancements
- Add shader-based accretion disk for realism.
- Pipe OSC voice transcriptions into Gemini for dynamic lore.
- Emit Prometheus metrics from Node bridge for session analytics.
