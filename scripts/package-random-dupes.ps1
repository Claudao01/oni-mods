[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$publicRoot = (Resolve-Path "$PSScriptRoot\..").Path
$modRoot = Join-Path $publicRoot "mods\random-dupes"
$project = Join-Path $modRoot "CLD01_RandomDupes.csproj"
$releaseDirectory = Join-Path $modRoot "release"
$artifactDirectory = Join-Path $publicRoot "artifacts"
$stagingRoot = Join-Path $artifactDirectory "staging-random-dupes"
$stagingMod = Join-Path $stagingRoot "random-dupes"

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
    throw "Falha ao compilar Random Dupes."
}

$compiledDll = Join-Path $modRoot "bin\Release\CLD01_RandomDupes.dll"
$dllVersion = [System.Reflection.AssemblyName]::GetAssemblyName($compiledDll).Version.ToString(3)
if ($dllVersion -ne $version) {
    throw "Versao da DLL divergente: dll=$dllVersion, esperada=$version"
}

New-Item -ItemType Directory -Force -Path $releaseDirectory, $artifactDirectory | Out-Null
Copy-Item -LiteralPath $compiledDll -Destination (Join-Path $releaseDirectory "CLD01_RandomDupes.dll") -Force

if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $stagingMod | Out-Null

@("mod.yaml", "mod_info.yaml", "README.md", "CHANGELOG.md") | ForEach-Object {
    Copy-Item -LiteralPath (Join-Path $modRoot $_) -Destination $stagingMod
}
Copy-Item -LiteralPath (Join-Path $releaseDirectory "CLD01_RandomDupes.dll") -Destination $stagingMod

$zipPath = Join-Path $artifactDirectory "random-dupes-v$version.zip"
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}
Compress-Archive -Path $stagingMod -DestinationPath $zipPath -CompressionLevel Optimal
Remove-Item -LiteralPath $stagingRoot -Recurse -Force

Write-Host "Pacote criado: $zipPath" -ForegroundColor Green
Write-Host "DLL versionada: $(Join-Path $releaseDirectory 'CLD01_RandomDupes.dll')" -ForegroundColor Green
