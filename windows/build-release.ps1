param(
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "Snoopy.Windows/Snoopy.Windows.csproj"
$outDir = Join-Path $root "release/$Runtime"
$zipPath = Join-Path $root "release/Snoopy.Windows-$Runtime.zip"

Write-Host "[1/3] publish => $outDir"
dotnet publish $project `
  -c Release `
  -r $Runtime `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true `
  -o $outDir

Write-Host "[2/3] zip => $zipPath"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path "$outDir/*" -DestinationPath $zipPath -Force

Write-Host "[3/3] done"
Write-Host "Release package: $zipPath"
