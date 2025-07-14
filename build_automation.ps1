# build_automation.ps1
# Quantum-documented PowerShell script to automate GameDinVR build pipeline
# - Downloads/updates UdonSharp
# - Prompts for VRChat SDK3 via VCC
# - Provides hooks for future automation (scene layout, UnityPackage import, VRChat upload)
# - To be run from project root

<##
Quantum Documentation:
- This script is the foundation for a fully automated build pipeline from Cursor to VRChat.
- It currently automates UdonSharp download and provides guidance for VRChat SDK3 setup.
- Future hooks: Unity CLI automation, scene layout, VRChat world upload (when API/CLI is available).
- Cross-reference: See README.md for vision and usage.
##>

# --- CONFIG ---
$udonSharpUrl = "https://github.com/MerlinVR/UdonSharp/releases/latest/download/UdonSharp.unitypackage"
$udonSharpDest = "Assets/SDKPackages/UdonSharp.unitypackage"

# --- UdonSharp Download/Update ---
Write-Host "[GameDinVR] Downloading latest UdonSharp..."
Invoke-WebRequest -Uri $udonSharpUrl -OutFile $udonSharpDest -UseBasicParsing
Write-Host "[GameDinVR] UdonSharp downloaded to $udonSharpDest"

# --- VRChat SDK3 Guidance ---
Write-Host "[GameDinVR] Please use the VRChat Creator Companion (VCC) to add the latest VRChat SDK3 (Worlds) to this project."
Write-Host "[GameDinVR] See: https://vcc.docs.vrchat.com/ for instructions."

# --- Future Automation Hooks ---
# TODO: Unity CLI scene layout automation
# TODO: UnityPackage auto-import (via Unity Editor scripting)
# TODO: VRChat world upload (when API/CLI is available)

Write-Host "[GameDinVR] Build automation script complete. Expand as VRChat/Unity APIs allow." 