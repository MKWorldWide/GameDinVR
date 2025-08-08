# build_automation.ps1
# Automates Unity batch builds and optional world uploads for GameDinVR
# Requires Unity editor and VRChat CLI (vrchat-cli.exe) installed locally

param(
    [string]$UnityPath = "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.6f1\\Editor\\Unity.exe", # pinned Unity version
    [string]$ProjectPath = (Get-Location).Path,
    [string]$OutputPath = "$((Get-Location).Path)\\Builds\\GameDinVR_World",
    [string]$VRCliPath = "$env:LOCALAPPDATA\\VRChatCreatorCompanion\\VRChat CLI\\vrchat-cli.exe",
    [string]$WorldId = "" # optional existing world ID for uploads
)

# Ensure output directory exists
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

# Build the world using Unity's batch mode for CI-friendly operation
& $UnityPath -batchmode -nographics -quit -projectPath $ProjectPath `
    -executeMethod BuildScript.PerformBuild -buildTarget StandaloneWindows64 `
    -logFile "$OutputPath\\build.log"

# If a world ID is supplied, upload via VRChat CLI
if ($WorldId -ne "" -and (Test-Path $VRCliPath)) {
    & $VRCliPath build-upload `
        --project-path $ProjectPath `
        --world-id $WorldId `
        --output "$OutputPath"
} else {
    Write-Host "Skipping VRChat upload; provide -WorldId to enable." -ForegroundColor Yellow
}
