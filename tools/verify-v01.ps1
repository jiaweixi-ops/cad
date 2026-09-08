[CmdletBinding()]
param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]

function Require-Path([string]$Path) {
    if (-not (Test-Path -LiteralPath (Join-Path $Root $Path))) {
        $failures.Add("Missing: $Path")
    }
}

function Require-Text([string]$Path, [string]$Text) {
    $fullPath = Join-Path $Root $Path
    $content = Get-Content -Raw -LiteralPath $fullPath
    if ($content.IndexOf($Text, [System.StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Missing text '$Text' in $Path")
    }
}

Require-Path 'CADProjectManager.sln'
Require-Path 'src\CADProjectManager.Core\CADProjectManager.Core.csproj'
Require-Path 'src\CADProjectManager.Infrastructure\CADProjectManager.Infrastructure.csproj'
Require-Path 'src\CADProjectManager.AutoCAD\CADProjectManager.AutoCAD.csproj'
Require-Path 'src\CADProjectManager.AutoCAD\PluginEntry.cs'
Require-Path 'src\CADProjectManager.AutoCAD\GlobalExceptionBoundary.cs'
Require-Text 'src\CADProjectManager.AutoCAD\PluginEntry.cs' 'CADPM_HEALTH'
Require-Text 'src\CADProjectManager.AutoCAD\PluginEntry.cs' 'CADPM_INFO'
Require-Text 'src\CADProjectManager.AutoCAD\PluginEntry.cs' 'IExtensionApplication'
Require-Text 'src\CADProjectManager.AutoCAD\CADProjectManager.AutoCAD.csproj' '<Private>false</Private>'

$allSource = Get-ChildItem -LiteralPath (Join-Path $Root 'src') -Recurse -File -Include *.cs,*.csproj | Get-Content -Raw
foreach ($forbidden in @('System.Net.', 'HttpClient', 'FileSystemWatcher', 'System.Threading.Timer', 'SendKeys', 'Process.Kill', 'Environment.Exit', 'Application.Quit', 'AcDbMgd.dll</HintPath>')) {
    if ($allSource.Contains($forbidden)) {
        $failures.Add("Forbidden V0.1 pattern present: $forbidden")
    }
}

$coreFiles = Get-ChildItem -LiteralPath (Join-Path $Root 'src\CADProjectManager.Core') -Recurse -File -Include *.cs,*.csproj | Get-Content -Raw
foreach ($autocadReference in @('AcMgd', 'AcDbMgd', 'AcCoreMgd', 'Autodesk.AutoCAD')) {
    if ($coreFiles.Contains($autocadReference)) {
        $failures.Add("Core references AutoCAD API: $autocadReference")
    }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output 'V0.1 static checks passed.'
Write-Output 'This does not replace Visual Studio/MSBuild or real AutoCAD NETLOAD acceptance.'
