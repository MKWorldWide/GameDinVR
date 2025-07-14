# GameDinVR: The Citadel of Resonance

Welcome to the sacred beginning of the GameDin VR world—**The Citadel of Resonance**—a high-fantasy + sci-fi VRChat realm designed for lore, governance, and connection.

---

## ✨ World Vision
- **Central floating bar** (hub of activity)
- **Lore chambers** (Council, Genesis Core, Arcade)
- **GDI Hologram Panel** (HUD-style, token & login display)
- **Portals** to future areas
- **Divine yet shadowy lighting**
- **Modular, reusable UdonSharp components**

---

## 🏗️ Project Structure
```
/Assets/
  /Scenes/                # Main Unity scenes
  /Prefabs/                # Modular prefabs (bar, pads, panels)
  /Scripts/Udon/           # UdonSharp scripts (teleport, UI, world logic)
  /Materials/              # Placeholder materials
  /UI/                     # GDI Hologram Canvas prefab
  /SDKPackages/            # Downloaded UnityPackages (UdonSharp, etc.)
```

---

## 🚀 Quick Start
1. **Clone this repo:**
   ```sh
   git clone https://github.com/M-K-World-Wide/GameDinVR.git
   ```
2. **Open in Unity 2022 LTS**
3. **Import VRChat SDK3 (Worlds) via [VCC](https://vcc.docs.vrchat.com/)**
4. **Import UdonSharp:**
   - Double-click `Assets/SDKPackages/UdonSharp.unitypackage` in Unity
5. **Open `Assets/Scenes/GameDin_Citadel.unity`**
6. **Build & test in VRChat!**

---

## 🛠️ Automated Build & Deployment (Vision)
- **Goal:** One-command build from Cursor → Unity → VRChat
- **Scripts planned:**
  - Download/update SDKs & UdonSharp
  - Auto-import UnityPackages
  - Scene layout automation
  - VRChat world upload (via CLI/API if available)
- **API Needs:**
  - VRChat does not currently offer a public world-upload API; automation is limited to local build/import steps.
  - If/when VRChat exposes a CLI or API for world upload, we will integrate it here.

---

## 🤖 Contributing
- Fork, branch, and PR as usual
- All scripts must be quantum-documented (see `/Scripts/Udon/` for examples)
- Keep `/docs/` and `/UI/` up to date
- See [CONTRIBUTING.md](CONTRIBUTING.md) for details (coming soon)

---

## 🧩 Credits & License
- Built by the GameDin community
- Powered by Unity, VRChat SDK3, UdonSharp
- MIT License (see LICENSE)

---

## 💡 Next Steps
- [ ] Automate UnityPackage import & scene setup
- [ ] Add dynamic lighting/music triggers
- [ ] Integrate real wallet/token data
- [ ] Expand lore chambers & social features

---

**With infinite love and dedication, your perfect AI assistant.** 