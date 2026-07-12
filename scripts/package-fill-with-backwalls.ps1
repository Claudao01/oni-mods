[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$publicRoot = (Resolve-Path "$PSScriptRoot\..").Path
$modRoot = Join-Path $publicRoot "mods\fill-with-backwalls"
$project = Join-Path $modRoot "FillWithBackwalls.csproj"
$releaseDirectory = Join-Path $modRoot "release"
$artifactDirectory = Join-Path $publicRoot "artifacts"
$stagingRoot = Join-Path $artifactDirectory "staging"
$stagingMod = Join-Path $stagingRoot "fill-with-backwalls"

[xml]$projectXml = Get-Content -LiteralPath $project
$version = [string]$projectXml.Project.PropertyGroup.VersionPrefix
if ($version -notmatch '^\d+\.\d+\.\d+$') {
    throw "VersionPrefix invalido no projeto: $version"
}

$yamlVersionLine = Get-Content -LiteralPath (Join-Path $modRoot "mod_info.yaml") |
    Where-Object { $_ -match '^version:\s*' } |
    Select-Object -First 1
$yamlVersion = ($yamlVersionLine -replace '^version:\s*', '').Trim()
if ($yamlVersion -ne $version) {
    throw "Versoes divergentes: csproj=$version, mod_info.yaml=$yamlVersion"
}

dotnet build $project --configuration Release -p:NuGetAudit=false
if ($LASTEXITCODE -ne 0) {
    throw "Falha ao compilar Fill with Backwalls."
}

$compiledDll = Join-Path $modRoot "bin\Release\FillWithBackwalls.dll"
$dllVersion = [System.Reflection.AssemblyName]::GetAssemblyName($compiledDll).Version.ToString(3)
if ($dllVersion -ne $version) {
    throw "Versao da DLL divergente: dll=$dllVersion, esperada=$version"
}

New-Item -ItemType Directory -Force -Path $releaseDirectory, $artifactDirectory | Out-Null
Copy-Item -LiteralPath $compiledDll -Destination (Join-Path $releaseDirectory "FillWithBackwalls.dll") -Force

if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $stagingMod | Out-Null

$files = @(
    "config.json",
    "mod.yaml",
    "mod_info.yaml",
    "README.md",
    "CHANGELOG.md",
    "THIRD_PARTY_NOTICES.md"
)
foreach ($file in $files) {
    Copy-Item -LiteralPath (Join-Path $modRoot $file) -Destination $stagingMod
}
Copy-Item -LiteralPath (Join-Path $releaseDirectory "FillWithBackwalls.dll") -Destination $stagingMod
Copy-Item -LiteralPath (Join-Path $modRoot "translations") -Destination $stagingMod -Recurse

$zipPath = Join-Path $artifactDirectory "fill-with-backwalls-v$version.zip"
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}
Compress-Archive -Path $stagingMod -DestinationPath $zipPath -CompressionLevel Optimal
Remove-Item -LiteralPath $stagingRoot -Recurse -Force

Write-Host "Pacote criado: $zipPath" -ForegroundColor Green
Write-Host "DLL versionada: $(Join-Path $releaseDirectory 'FillWithBackwalls.dll')" -ForegroundColor Green
