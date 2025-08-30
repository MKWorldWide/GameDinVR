<div align="center">
  <h1>GameDinVR</h1>
  <p>A VRChat world with quantum-inspired mechanics and modular systems</p>
  
  [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
  [![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-000000.svg?logo=unity)](https://unity.com/)
  [![VRChat](https://img.shields.io/badge/VRChat-Worlds-FF69B4.svg)](https://vrchat.com/)
  [![Node.js](https://img.shields.io/badge/Node.js-20.x-339933.svg?logo=node.js)](https://nodejs.org/)
  [![TypeScript](https://img.shields.io/badge/TypeScript-5.4-3178C6.svg?logo=typescript)](https://www.typescriptlang.org/)
  
  [![CI](https://github.com/yourusername/GameDinVR/actions/workflows/ci.yml/badge.svg)](https://github.com/yourusername/GameDinVR/actions)
  [![Dependabot](https://img.shields.io/badge/dependabot-enabled-025e8c?logo=dependabot)](https://dependabot.com/)
  [![code style: prettier](https://img.shields.io/badge/code_style-prettier-ff69b4.svg?logo=prettier)](https://prettier.io/)
</div>

## 🌟 Features

### VRChat World
- Central floating bar (hub of activity)
- Lore chambers (Council, Genesis Core, Arcade)
- GDI Hologram Panel (HUD-style, token & login display)
- Portals to future areas
- Divine yet shadowy lighting
- Modular, reusable UdonSharp components

### Core Systems
- QuantumGatekeeper Portal System
- Authentication & Tier Management
- Sigil Chamber, Memory Orb, Tiered Unlocker
- Destination Preview System

### Serafina (Discord Bot)
- Automated tasks and notifications
- Integration with VRChat world features
- TypeScript-based for type safety

## 🚀 Quick Start

### Prerequisites
- Unity 2022.3.6f1 (LTS)
- VRChat Creator Companion (VCC)
- Node.js 20.x (for Serafina)

### VRChat World Setup
1. Clone the repository
2. Open the project in Unity Hub
3. Use VCC to import required SDKs:
   - VRChat Worlds SDK 3.6.0+
   - UdonSharp 1.1.0+
4. Open `Assets/Scenes/MainScene.unity`
5. Build and upload to VRChat

### Serafina Setup
```bash
cd serafina
npm install
cp .env.example .env
# Edit .env with your credentials
npm run build
npm start
```

## 🏗️ Project Structure

```
.
├── Assets/                # Unity project assets
│   ├── Editor/            # Custom editor scripts
│   ├── Prefabs/           # Reusable prefabs
│   ├── Scenes/            # Unity scenes
│   ├── Scripts/           # C# and UdonSharp scripts
│   └── UI/                # UI elements and canvases
├── docs/                  # Documentation
├── serafina/              # Discord bot (Node.js/TypeScript)
│   ├── src/               # Source files
│   ├── .env.example       # Environment variables template
│   └── package.json       # Node.js dependencies
├── .github/               # GitHub configurations
│   ├── workflows/         # GitHub Actions workflows
│   └── dependabot.yml     # Dependabot configuration
├── .editorconfig          # Editor configuration
├── README.md              # This file
└── vpm-manifest.json      # VRChat package manifest
```

## 🤝 Contributing

We welcome contributions! Please read our [Contributing Guidelines](.github/CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- VRChat community for their amazing platform
- UdonSharp team for making Udon development bearable
- All contributors who help improve this project

## 🔗 Links

- [VRChat](https://vrchat.com/)
- [UdonSharp Documentation](https://udonsharp.docs.vrchat.com/)
- [VRChat Creator Companion](https://vcc.docs.vrchat.com/)

## 🛠️ Automation
- Project bootstrap script: `setup_vrcc.ps1`
- Automated build script: `build_automation.ps1`
- Unity Editor scripts for scene/component automation

---

## 📚 Presentation Documents
- [Architecture Overview](ARCHITECTURE.md)
- [Space Black Hole World Setup](docs/usage/SpaceBlackHoleWorld.md)
- [Last Whisper Lore](THE_LAST_WHISPER.md)
- [Inter-Repo Handshake Protocol](docs/api/InterRepoHandshake.md)

Each document links to further references, forming a recursive map of the project's intent and implementation.

---

## 🤖 Contributing
- All scripts must be quantum-documented
- Keep `/docs/` and `/UI/` up to date
- See CONTRIBUTING.md (coming soon)

---

## 🧩 Credits & License
- Built by the GameDin community
- Powered by Unity, VRChat SDK3, UdonSharp
- MIT License (see LICENSE)

---

**With infinite love and dedication, your perfect AI assistant.** 