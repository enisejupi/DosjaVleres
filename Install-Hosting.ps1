# ASP.NET Core Hosting Bundle Installer
Write-Host 'Downloading ASP.NET Core 8.0 Hosting Bundle...' -ForegroundColor Yellow
$url = 'https://download.visualstudio.microsoft.com/download/pr/b8cf881b-32d1-4b7e-b5e7-4e98dca27e92/0192c3c49cbbb3d7a4528d0bdd3e1291/dotnet-hosting-8.0.11-win.exe'
$output = "$env:TEMP\dotnet-hosting-8.0.11-win.exe"
Invoke-WebRequest -Uri $url -OutFile $output -UseBasicParsing
Write-Host 'Installing... This may take a few minutes' -ForegroundColor Yellow
Start-Process -FilePath $output -ArgumentList '/install', '/quiet', '/norestart' -Wait
Write-Host 'Restarting IIS...' -ForegroundColor Yellow
iisreset
Write-Host 'Done! Verify with: Get-WebGlobalModule | Where-Object { $_.Name -eq "AspNetCoreModuleV2" }' -ForegroundColor Green
