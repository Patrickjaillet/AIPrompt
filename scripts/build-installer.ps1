#!/usr/bin/env pwsh
<#
Publishes AIPrompt.App as a self-contained win-x64 build and compiles it
into a Windows installer with Inno Setup 6, producing dist\AIPromptSetup_x64.exe.
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$versionFilePath = Join-Path $repoRoot 'VERSION'
$appProjectPath = Join-Path $repoRoot 'src\AIPrompt.App\AIPrompt.App.csproj'
$publishDir = Join-Path $repoRoot 'publish\AIPrompt.App'
$issPath = Join-Path $repoRoot 'installer\AIPromptSetup.iss'

if (-not (Test-Path $versionFilePath)) {
    throw "VERSION file not found at '$versionFilePath'."
}
$version = (Get-Content -Path $versionFilePath -Raw).Trim()

if (Test-Path $publishDir) {
    Remove-Item -Path $publishDir -Recurse -Force
}

Write-Host "Publishing AIPrompt.App $version ($Configuration, win-x64, self-contained)..."
dotnet publish $appProjectPath -c $Configuration -r win-x64 --self-contained true `
    -p:PublishSingleFile=false -o $publishDir "/p:ProductVersionFull=$version"
if ($LASTEXITCODE -ne 0) {
    throw 'dotnet publish failed.'
}

$iscc = Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'
if (-not (Test-Path $iscc)) {
    $iscc = Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'
}
if (-not (Test-Path $iscc)) {
    throw 'Inno Setup 6 (ISCC.exe) not found. Install it from https://jrsoftware.org/isinfo.php.'
}

Write-Host "Compiling installer with Inno Setup ($version)..."
& $iscc "/DMyAppVersion=$version" $issPath
if ($LASTEXITCODE -ne 0) {
    throw 'ISCC compilation failed.'
}

Write-Host "Installer built: dist\AIPromptSetup_x64.exe (version $version)"
