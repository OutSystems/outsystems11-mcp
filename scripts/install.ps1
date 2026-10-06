<#
.SYNOPSIS
    Installs (or updates) the servicestudio-mcp-oml skill into the local
    Claude Code skills directory.

.PARAMETER Source
    Path to a local checkout of this repository to copy from, instead of
    cloning the latest version from GitHub.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File scripts\install.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File scripts\install.ps1 -Source .
#>
[CmdletBinding()]
param(
    [string]$Source
)

$ErrorActionPreference = "Stop"

$RepoUrl = "https://github.com/OutSystems/outsystems11-mcp.git"
$SkillName = "servicestudio-mcp-oml"
$SkillsDir = Join-Path $env:USERPROFILE ".claude\skills"

$TmpDir = $null
try {
    if ($Source) {
        $RepoDir = $Source
    } else {
        if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
            throw "git is required but not found on PATH."
        }
        $TmpDir = Join-Path ([System.IO.Path]::GetTempPath()) ([System.Guid]::NewGuid())
        New-Item -ItemType Directory -Path $TmpDir | Out-Null
        Write-Host "Cloning $RepoUrl..."
        git clone --depth 1 $RepoUrl (Join-Path $TmpDir "repo")
        $RepoDir = Join-Path $TmpDir "repo"
    }

    $SourceSkillPath = Join-Path $RepoDir $SkillName
    if (-not (Test-Path $SourceSkillPath)) {
        throw "'$SkillName' not found under $RepoDir"
    }

    $Target = Join-Path $SkillsDir $SkillName
    New-Item -ItemType Directory -Path $SkillsDir -Force | Out-Null
    if (Test-Path $Target) {
        Remove-Item -Recurse -Force $Target
    }
    Copy-Item -Recurse -Force $SourceSkillPath $Target

    Write-Host "Installed '$SkillName' to $Target"
    Write-Host "Start a new Claude Code session for the skill to load."
}
finally {
    if ($TmpDir -and (Test-Path $TmpDir)) {
        Remove-Item -Recurse -Force $TmpDir
    }
}
