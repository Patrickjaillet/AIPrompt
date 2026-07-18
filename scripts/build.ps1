#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [switch]$NoIncrement
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$versionFilePath = Join-Path $repoRoot 'VERSION'
$solutionPath = Join-Path $repoRoot 'AIPrompt.sln'

if (-not (Test-Path $versionFilePath)) {
    throw "VERSION file not found at '$versionFilePath'."
}

$currentVersion = (Get-Content -Path $versionFilePath -Raw).Trim()
if ($currentVersion -notmatch '^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(-(?<prerelease>[0-9A-Za-z.]+))?$') {
    throw "VERSION file content '$currentVersion' is not a valid MAJOR.MINOR.PATCH[-PRERELEASE] version."
}

$major = $Matches.major
$minor = $Matches.minor
$patch = [int]$Matches.patch
$prerelease = $Matches.prerelease

if (-not $NoIncrement) {
    $patch = $patch + 1
}

$newVersion = "$major.$minor.$patch"
if ($prerelease) {
    $newVersion = "$newVersion-$prerelease"
}

if (-not $NoIncrement) {
    Set-Content -Path $versionFilePath -Value $newVersion -NoNewline
    Write-Host "Version bumped: $currentVersion -> $newVersion"
} else {
    Write-Host "Building version $newVersion (PATCH not incremented, -NoIncrement set)"
}

dotnet build $solutionPath -c $Configuration "/p:ProductVersionFull=$newVersion"
