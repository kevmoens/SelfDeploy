$currentDirectory = $PSScriptRoot
$sourceDirectory = Join-Path (Split-Path $currentDirectory -Parent) "SelfDeployUI"
$zipFilePath = Join-Path $currentDirectory "UI.zip"
Add-Type -A 'System.IO.Compression.FileSystem'
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
if (Test-Path $zipFilePath) {
    Remove-Item $zipFilePath -Force
}
[System.IO.Compression.ZipFile]::CreateFromDirectory($sourceDirectory, $zipFilePath, $compressionLevel, $false)