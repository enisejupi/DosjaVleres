# Script to help install ASP.NET Core Hosting Bundle
# Run as Administrator

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  ASP.NET Core Hosting Bundle Installation" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    exit 1
}

Write-Host "Checking current installation status..." -ForegroundColor Yellow
Write-Host ""

# Check if module exists
$moduleExists = Test-Path "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
Import-Module WebAdministration -ErrorAction SilentlyContinue
$moduleRegistered = $null
if (Get-Module WebAdministration) {
    $moduleRegistered = Get-WebGlobalModule | Where-Object { $_.Name -eq "AspNetCoreModuleV2" }
}

if ($moduleExists -and $moduleRegistered) {
    Write-Host "ASP.NET Core Module V2 is already installed and registered!" -ForegroundColor Green
    Write-Host ""
    Write-Host "If you are still having issues, try:" -ForegroundColor Yellow
    Write-Host "  1. Run: iisreset" -ForegroundColor White
    Write-Host "  2. Redeploy your application: .\Deploy-IIS.ps1" -ForegroundColor White
    exit 0
}

Write-Host "ASP.NET Core Module V2 is NOT properly installed" -ForegroundColor Red
Write-Host ""

# Try to download the hosting bundle
$hostingBundleUrl = "https://download.visualstudio.microsoft.com/download/pr/b8cf881b-32d1-4b7e-b5e7-4e98dca27e92/0192c3c49cbbb3d7a4528d0bdd3e1291/dotnet-hosting-8.0.11-win.exe"
$downloadPath = "$env:TEMP\dotnet-hosting-8.0.11-win.exe"

Write-Host "Downloading ASP.NET Core 8.0 Hosting Bundle..." -ForegroundColor Yellow
Write-Host "URL: $hostingBundleUrl" -ForegroundColor Gray
Write-Host "Saving to: $downloadPath" -ForegroundColor Gray
Write-Host ""

try {
    # Download the file
    $ProgressPreference = 'SilentlyContinue'
    Invoke-WebRequest -Uri $hostingBundleUrl -OutFile $downloadPath -UseBasicParsing
    Write-Host "Download complete!" -ForegroundColor Green
    Write-Host ""
    
    # Install
    Write-Host "Installing Hosting Bundle..." -ForegroundColor Yellow
    Write-Host "This may take a few minutes..." -ForegroundColor Gray
    Write-Host ""
    
    $installProcess = Start-Process -FilePath $downloadPath -ArgumentList "/install", "/quiet", "/norestart" -Wait -PassThru
    
    if ($installProcess.ExitCode -eq 0) {
        Write-Host "Installation completed successfully!" -ForegroundColor Green
        Write-Host ""
        
        # Restart IIS
        Write-Host "Restarting IIS..." -ForegroundColor Yellow
        iisreset | Out-Null
        Write-Host "IIS restarted" -ForegroundColor Green
        Write-Host ""
        
        # Verify installation
        Start-Sleep -Seconds 2
        Import-Module WebAdministration -Force
        $moduleRegistered = Get-WebGlobalModule | Where-Object { $_.Name -eq "AspNetCoreModuleV2" }
        
        if ($moduleRegistered) {
            Write-Host "================================================" -ForegroundColor Cyan
            Write-Host "  Installation Successful!" -ForegroundColor Green
            Write-Host "================================================" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "You can now deploy your application:" -ForegroundColor Yellow
            Write-Host "  .\Deploy-IIS.ps1" -ForegroundColor White
            Write-Host ""
        } else {
            Write-Host "WARNING: Module installed but not registered in IIS" -ForegroundColor Yellow
            Write-Host "Try restarting your computer and running iisreset again" -ForegroundColor White
        }
    } else {
        Write-Host "ERROR: Installation failed with exit code: $($installProcess.ExitCode)" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please try manual installation:" -ForegroundColor Yellow
        Write-Host "  1. Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
        Write-Host "  2. Look for Hosting Bundle under ASP.NET Core Runtime 8.0" -ForegroundColor White
        Write-Host "  3. Run the installer as Administrator" -ForegroundColor White
        Write-Host "  4. After installation, run: iisreset" -ForegroundColor White
    }
    
    # Cleanup
    if (Test-Path $downloadPath) {
        Remove-Item $downloadPath -Force -ErrorAction SilentlyContinue
    }
}
catch {
    Write-Host "ERROR: Failed to download or install the hosting bundle" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install manually:" -ForegroundColor Yellow
    Write-Host "  1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
    Write-Host "  2. Download Hosting Bundle for ASP.NET Core 8.0" -ForegroundColor White
    Write-Host "  3. Run the installer as Administrator" -ForegroundColor White
    Write-Host "  4. After installation, run: iisreset" -ForegroundColor White
    Write-Host ""
}
