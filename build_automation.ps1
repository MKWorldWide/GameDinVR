# build_automation.ps1
# Quantum-documented build automation script for GameDinVR Quantum Template
# This script automates Unity project build steps for VRChat world development.

# ---
# Step 1: Set Unity executable path (update as needed)
$unityPath = "C:\Program Files\Unity\Hub\Editor\2022.3.0f1\Editor\Unity.exe"

# Step 2: Set project path (current directory)
$projectPath = Get-Location

# Step 3: Set build output path
$outputPath = "$projectPath\Builds\GameDinVR_World"

# Step 4: Run Unity batch mode build (update scene name as needed)
& $unityPath -batchmode -nographics -quit -projectPath $projectPath -executeMethod BuildScript.PerformBuild -buildTarget StandaloneWindows64 -logFile "$outputPath\build.log"

# ---
# Usage:
# 1. Update $unityPath if your Unity install is elsewhere.
# 2. Ensure BuildScript.cs exists in Assets/Editor/ with a static PerformBuild method.
# 3. Run this script from the project root:
#    pwsh ./build_automation.ps1
# --- 