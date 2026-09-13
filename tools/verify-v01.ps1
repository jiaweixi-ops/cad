[CmdletBinding()]
param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]

function Require-Path([string]$Path) {
    $fullPath = Join-Path $Root $Path
    if (-not (Test-Path -LiteralPath $fullPath)) {
        $failures.Add("Missing: $Path")
        return $false
    }

    return $true
}

function Require-Text([string]$Path, [string]$Text) {
    $fullPath = Join-Path $Root $Path
    if (-not (Test-Path -LiteralPath $fullPath)) {
        return
    }

    $content = Get-Content -Raw -LiteralPath $fullPath
    if ($content.IndexOf($Text, [System.StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Missing text '$Text' in $Path")
    }
}

function Get-SourceText([string]$Directory, [string[]]$Extensions) {
    if (-not (Test-Path -LiteralPath $Directory)) {
        return ''
    }

    $items = Get-ChildItem -LiteralPath $Directory -Recurse -File |
        Where-Object {
            ($Extensions -contains $_.Extension) -and
            ($_.FullName -notmatch '\\bin\\') -and
            ($_.FullName -notmatch '\\obj\\')
        }

    return (($items | ForEach-Object { Get-Content -Raw -LiteralPath $_.FullName }) -join "`n")
}

Require-Path 'CADProjectManager.sln' | Out-Null
Require-Path 'src\CADProjectManager.Core\CADProjectManager.Core.csproj' | Out-Null
Require-Path 'src\CADProjectManager.Infrastructure\CADProjectManager.Infrastructure.csproj' | Out-Null
$hasAutoCadProject = Require-Path 'src\CADProjectManager.AutoCAD\CADProjectManager.AutoCAD.csproj'
Require-Path 'src\CADProjectManager.AutoCAD\PluginEntry.cs' | Out-Null
Require-Path 'src\CADProjectManager.AutoCAD\GlobalExceptionBoundary.cs' | Out-Null
Require-Text 'src\CADProjectManager.AutoCAD\PluginEntry.cs' 'CADPM_HEALTH'
Require-Text 'src\CADProjectManager.AutoCAD\PluginEntry.cs' 'CADPM_INFO'
Require-Text 'src\CADProjectManager.AutoCAD\PluginEntry.cs' 'IExtensionApplication'

if ($hasAutoCadProject) {
    $autoCadProjectPath = Join-Path $Root 'src\CADProjectManager.AutoCAD\CADProjectManager.AutoCAD.csproj'
    $autoCadProject = [xml](Get-Content -Raw -LiteralPath $autoCadProjectPath)
    $requiredReferences = @('AcMgd', 'AcDbMgd', 'AcCoreMgd')
    foreach ($referenceName in $requiredReferences) {
        $reference = $autoCadProject.Project.ItemGroup.Reference |
            Where-Object { $_.Include -eq $referenceName } |
            Select-Object -First 1

        if ($null -eq $reference) {
            $failures.Add("Missing AutoCAD reference: $referenceName")
            continue
        }

        if ([string]$reference.Private -ne 'false') {
            $failures.Add("AutoCAD reference must set Private=false: $referenceName")
        }
    }
}

$allSource = Get-SourceText (Join-Path $Root 'src') @('.cs', '.csproj')
foreach ($forbidden in @('System.Net.', 'HttpClient', 'FileSystemWatcher', 'System.Threading.Timer', 'SendKeys', 'Process.Kill', 'Environment.Exit', 'Application.Quit')) {
    if ($allSource.IndexOf($forbidden, [System.StringComparison]::Ordinal) -ge 0) {
        $failures.Add("Forbidden V0.1 pattern present: $forbidden")
    }
}

$coreFiles = Get-SourceText (Join-Path $Root 'src\CADProjectManager.Core') @('.cs', '.csproj')
foreach ($autocadReference in @('AcMgd', 'AcDbMgd', 'AcCoreMgd', 'Autodesk.AutoCAD')) {
    if ($coreFiles.IndexOf($autocadReference, [System.StringComparison]::Ordinal) -ge 0) {
        $failures.Add("Core references AutoCAD API: $autocadReference")
    }
}

$outputRoot = Join-Path $Root 'src\CADProjectManager.AutoCAD\bin'
if (Test-Path -LiteralPath $outputRoot) {
    foreach ($dll in @('AcMgd.dll', 'AcDbMgd.dll', 'AcCoreMgd.dll')) {
        $copied = Get-ChildItem -LiteralPath $outputRoot -Recurse -File -Filter $dll -ErrorAction SilentlyContinue
        if ($copied) {
            $paths = ($copied | ForEach-Object { $_.FullName }) -join '; '
            $failures.Add("Autodesk Managed DLL copied to plugin output: $dll => $paths")
        }
    }
}

if ($failures.Count -gt 0) {
    foreach ($failure in $failures) {
        Write-Host "FAIL: $failure"
    }
    exit 1
}

Write-Output 'V0.1 static checks passed.'
Write-Output 'Verified: project skeleton, commands, Core isolation, per-reference Private=false, forbidden V0.1 patterns, and no Autodesk Managed DLLs in existing build output.'
Write-Output 'This does not replace a real AutoCAD 2025 GUI NETLOAD and lifecycle acceptance test.'
exit 0
