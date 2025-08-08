# setup_vrcc.ps1
# PowerShell script to bootstrap a VRChat Creator Companion project with pinned versions.
# Assumes vrcc.exe is available from the VRChat Creator Companion install.
# Usage: pwsh ./setup_vrcc.ps1 -ProjectName GameDinVR

param(
    [string]$ProjectName = "GameDinVR",
    [string]$UnityVersion = "2022.3.6f1",           # Pin Unity LTS version
    [string]$WorldsVersion = "3.6.0",               # Pin VRChat Worlds SDK version
    [string]$UdonSharpVersion = "1.1.0"             # Pin UdonSharp version
)

# Resolve vrcc.exe path; adjust if installed elsewhere
$vrccPath = Join-Path $env:LOCALAPPDATA "VRChatCreatorCompanion\\vrcc.exe"

if (!(Test-Path $vrccPath)) {
    throw "vrcc.exe not found at $vrccPath"
}

# Create project with specified Unity version
& $vrccPath create-project $ProjectName --unity-version $UnityVersion

# Add packages with explicit versions to avoid unexpected upgrades
& $vrccPath add-package $ProjectName com.vrchat.worlds@$WorldsVersion
& $vrccPath add-package $ProjectName com.vrchat.udonsharp@$UdonSharpVersion

# Generate vpm-manifest.json capturing pinned dependencies
$manifest = @{
    name = $ProjectName
    unity = $UnityVersion
    dependencies = @{
        "com.vrchat.worlds"   = $WorldsVersion
        "com.vrchat.udonsharp"= $UdonSharpVersion
    }
} | ConvertTo-Json -Depth 3

$manifestPath = Join-Path $ProjectName "vpm-manifest.json"
$manifest | Out-File -Encoding UTF8 $manifestPath

Write-Host "Initialized $ProjectName with pinned Unity and SDK versions."
