# GameDinVR Quantum Architecture

## System Overview
This project is a modular, quantum-documented Unity VRChat world template. It features:
- Centralized scene management
- Tiered portal and access systems
- Modular UdonSharp scripts
- Automated build and deployment
- Quantum-level documentation throughout

## Core Systems
- **QuantumGatekeeper**: Tiered portal logic, VFX, and access control
- **QuantumSessionVerifier**: Player authentication and session validation
- **PlayerTierBridge**: Backend GDI tier fetching and caching
- **GDIAccessGate**: Basic tier-based access for sacred zones
- **QuantumSigilChamber**: Unlockable chambers based on player tier
- **GDIMemoryOrb**: Lore and secret unlocks
- **TieredMemoryUnlocker**: Content activation by tier
- **DestinationPreviewRenderer**: Real-time portal previews

## Directory Structure
- `/Assets/Scenes/`: Main Unity scenes
- `/Assets/Prefabs/`: Modular prefabs
- `/Assets/Scripts/Udon/`: UdonSharp scripts
- `/Assets/Materials/`: Materials
- `/Assets/UI/`: UI prefabs
- `/Assets/Editor/`: Editor automation scripts
- `/docs/`: Quantum documentation

## Dependency Map
- All core systems are modular and can be reused or extended
- UdonSharp is required for all scripts
- VRChat SDK3 (Worlds) is required for world upload

## System Diagram
```
flowchart TD
    Player -->|Interacts| QuantumGatekeeper
    QuantumGatekeeper -->|Checks| QuantumSessionVerifier
    QuantumSessionVerifier -->|Fetches| PlayerTierBridge
    PlayerTierBridge -->|Returns| QuantumGatekeeper
    QuantumGatekeeper -->|Controls| Portals
    QuantumGatekeeper -->|Triggers| QuantumSigilChamber
    QuantumSigilChamber -->|Unlocks| GDIMemoryOrb
    GDIMemoryOrb -->|Activates| TieredMemoryUnlocker
    QuantumGatekeeper -->|Renders| DestinationPreviewRenderer
```

## Performance & Security
- All systems are optimized for VRChat world constraints
- Tier and session checks are local (no external API calls at runtime)
- Security: No sensitive data stored; all logic is client-side

## Change History
- See CHANGELOG.md for updates 