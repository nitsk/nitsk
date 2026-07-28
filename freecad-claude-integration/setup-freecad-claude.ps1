<#
.SYNOPSIS
    Automates the scriptable parts of connecting Claude Desktop to FreeCAD via
    the freecad-mcp add-on (https://github.com/neka-nat/freecad-mcp).

.DESCRIPTION
    - Installs uv/uvx if not already present.
    - Downloads the freecad-mcp add-on and installs it into your FreeCAD Mod folder.
    - Adds (or updates) the "freecad" entry in claude_desktop_config.json, preserving
      any other MCP servers already configured.

    Steps that require clicking through a GUI (starting the RPC server inside
    FreeCAD, approving the FreeCAD/Claude connectors, quitting Claude from the
    tray) are NOT automated here -- see README.md for those.

.PARAMETER FreeCADVersion
    The version subfolder under %APPDATA%\FreeCAD used for the Mod directory.
    Defaults to "v1-1" (FreeCAD 1.1). For FreeCAD versions older than 1.1, pass
    "" (empty string) since the Mod folder sits directly in %APPDATA%\FreeCAD.

.PARAMETER TextOnly
    Configure the MCP server with --only-text-feedback (no viewport screenshots,
    fewer tokens per operation). Omit for the default (screenshot + text) mode.

.EXAMPLE
    .\setup-freecad-claude.ps1
    .\setup-freecad-claude.ps1 -TextOnly
    .\setup-freecad-claude.ps1 -FreeCADVersion ""   # FreeCAD < 1.1

.NOTES
    Run this from an Administrator PowerShell window.
    Close FreeCAD completely before running (it needs to pick up the add-on fresh).
#>

param(
    [string]$FreeCADVersion = "v1-1",
    [switch]$TextOnly
)

$ErrorActionPreference = "Stop"

function Write-Step($msg) {
    Write-Host ""
    Write-Host "==> $msg" -ForegroundColor Cyan
}

# 1. Install uvx (uv) if missing --------------------------------------------
Write-Step "Checking for uvx"
if (-not (Get-Command uvx -ErrorAction SilentlyContinue)) {
    Write-Host "uvx not found. Installing uv..."
    powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
    Write-Host ""
    Write-Host "uv/uvx was just installed. PATH changes only apply to a fresh shell." -ForegroundColor Yellow
    Write-Host "Close this PowerShell window, reopen it as Administrator, and re-run this script." -ForegroundColor Yellow
    exit 0
}
Write-Host "uvx found: $(uvx --version)"

# 2. Download and install the freecad-mcp add-on ----------------------------
Write-Step "Installing the freecad-mcp add-on"

$modDir = if ($FreeCADVersion) {
    Join-Path $env:APPDATA "FreeCAD\$FreeCADVersion\Mod"
} else {
    Join-Path $env:APPDATA "FreeCAD\Mod"
}
New-Item -ItemType Directory -Path $modDir -Force | Out-Null
Write-Host "Mod folder: $modDir"

$tmpZip = Join-Path $env:TEMP "freecad-mcp.zip"
$tmpExtract = Join-Path $env:TEMP "freecad-mcp-extract"

Write-Host "Downloading neka-nat/freecad-mcp..."
Invoke-WebRequest -Uri "https://github.com/neka-nat/freecad-mcp/archive/refs/heads/master.zip" -OutFile $tmpZip

if (Test-Path $tmpExtract) { Remove-Item $tmpExtract -Recurse -Force }
Expand-Archive -Path $tmpZip -DestinationPath $tmpExtract -Force

$addonSrc = Get-ChildItem -Path $tmpExtract -Recurse -Directory -Filter "freecad-mcp" |
    Where-Object { $_.FullName -match "[\\/]addon[\\/]freecad-mcp$" } |
    Select-Object -First 1

if (-not $addonSrc) {
    throw "Could not find the addon/freecad-mcp folder inside the downloaded repo. Install manually per README.md step 4."
}

$addonDest = Join-Path $modDir "freecad-mcp"
if (Test-Path $addonDest) {
    Write-Host "Removing existing add-on at $addonDest"
    Remove-Item $addonDest -Recurse -Force
}
Copy-Item -Path $addonSrc.FullName -Destination $addonDest -Recurse

Remove-Item $tmpZip -Force -ErrorAction SilentlyContinue
Remove-Item $tmpExtract -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Add-on installed to $addonDest"

# 3. Update claude_desktop_config.json ---------------------------------------
Write-Step "Updating claude_desktop_config.json"

$claudeConfigDir = Join-Path $env:APPDATA "Claude"
New-Item -ItemType Directory -Path $claudeConfigDir -Force | Out-Null
$configPath = Join-Path $claudeConfigDir "claude_desktop_config.json"

if (Test-Path $configPath) {
    $raw = Get-Content $configPath -Raw
    $config = if ([string]::IsNullOrWhiteSpace($raw)) { [PSCustomObject]@{} } else { $raw | ConvertFrom-Json }
} else {
    $config = [PSCustomObject]@{}
}

if (-not ($config.PSObject.Properties.Name -contains "mcpServers")) {
    $config | Add-Member -NotePropertyName "mcpServers" -NotePropertyValue ([PSCustomObject]@{})
}

$args = if ($TextOnly) { @("freecad-mcp", "--only-text-feedback") } else { @("freecad-mcp") }
$freecadServer = [PSCustomObject]@{
    command = "uvx"
    args    = $args
}

if ($config.mcpServers.PSObject.Properties.Name -contains "freecad") {
    $config.mcpServers.freecad = $freecadServer
} else {
    $config.mcpServers | Add-Member -NotePropertyName "freecad" -NotePropertyValue $freecadServer
}

if (Test-Path $configPath) {
    Copy-Item $configPath "$configPath.bak" -Force
    Write-Host "Backed up existing config to $configPath.bak"
}

$config | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8
Write-Host "Wrote $configPath"

# 4. Final instructions -------------------------------------------------------
Write-Step "Automated steps are done. Now do this by hand:"
Write-Host "  1. Quit Claude Desktop from the system tray (not just the window)."
Write-Host "  2. Open FreeCAD -> workbench dropdown -> select the MCP add-on."
Write-Host "  3. Click 'Start RPC Server' (tick 'Autostart Server' so this isn't needed again)."
Write-Host "  4. Reopen Claude Desktop -> '+' -> Connectors -> confirm 'freecad' is listed and on."
Write-Host ""
Write-Host "See README.md in this folder for troubleshooting and a prompt library to test with." -ForegroundColor Green
