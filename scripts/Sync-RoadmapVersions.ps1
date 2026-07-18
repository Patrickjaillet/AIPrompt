#!/usr/bin/env pwsh
<#
Stamps each fully-completed Phase in ROADMAP.md with the git tag actually
reached for it, next to its "Version cible" line, e.g.:

    **Version cible : `0.9.9`**
    **Version atteinte : `v0.9.9`** (2026-07-17)

A phase is "fully completed" when every `- [ ]` task line under it is `- [x]`.
Idempotent: re-running updates the stamp in place instead of duplicating it.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$roadmapPath = Join-Path $repoRoot 'ROADMAP.md'

if (-not (Test-Path $roadmapPath)) {
    throw "ROADMAP.md not found at '$roadmapPath'."
}

$tags = git -C $repoRoot tag --list 'v*' 2>$null
if (-not $tags) {
    Write-Warning 'No git tags found; nothing to sync.'
    return
}

function Get-TagDate([string]$tag) {
    $raw = git -C $repoRoot log -1 --format=%ad --date=short $tag 2>$null
    if ($LASTEXITCODE -eq 0 -and $raw) { return $raw.Trim() }
    return $null
}

$lines = Get-Content -Path $roadmapPath
$output = New-Object System.Collections.Generic.List[string]

$phaseTargetVersion = $null
$phaseStartIndex = -1
$phaseLines = New-Object System.Collections.Generic.List[string]

function Flush-Phase {
    param($targetVersion, $lines)

    if (-not $targetVersion) { return $lines }

    $taskLines = $lines | Where-Object { $_ -match '^\s*- \[[ x]\]' }
    if ($taskLines.Count -eq 0) { return $lines }

    $allChecked = -not ($taskLines | Where-Object { $_ -match '^\s*- \[ \]' })
    if (-not $allChecked) { return $lines }

    $tagName = "v$targetVersion"
    if (-not ($tags -contains $tagName)) { return $lines }

    $date = Get-TagDate $tagName
    $stamp = if ($date) { "**Version atteinte : ``$tagName``** ($date)" } else { "**Version atteinte : ``$tagName``**" }

    $result = New-Object System.Collections.Generic.List[string]
    $stampWritten = $false
    foreach ($line in $lines) {
        if ($line -match '^\*\*Version atteinte') {
            if (-not $stampWritten) {
                $result.Add($stamp)
                $stampWritten = $true
            }
            continue
        }
        $result.Add($line)
        if (-not $stampWritten -and $line -match '^\*\*Version cible') {
            $result.Add($stamp)
            $stampWritten = $true
        }
    }
    return $result
}

foreach ($line in $lines) {
    if ($line -match '^### Phase \d+') {
        foreach ($l in (Flush-Phase $phaseTargetVersion $phaseLines)) { $output.Add($l) }
        $phaseLines = New-Object System.Collections.Generic.List[string]
        $phaseTargetVersion = $null
        $output.Add($line)
        continue
    }

    if ($line -match '^\*\*Version cible : `([^`]+)`\*\*') {
        $phaseTargetVersion = $Matches[1]
    }

    $phaseLines.Add($line)
}
foreach ($l in (Flush-Phase $phaseTargetVersion $phaseLines)) { $output.Add($l) }

Set-Content -Path $roadmapPath -Value $output
Write-Host "ROADMAP.md synced with achieved git tags."
