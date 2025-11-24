# Script to install ASP.NET Core Hosting Bundle using winget
# Run as Administrator

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  ASP.NET Core Hosting Bundle Installation" -ForegroundColor Cyan
Write-Host "  Using Windows Package Manager (winget)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    exit 1
}

# Check if winget is available
$wingetPath = Get-Command winget -ErrorAction SilentlyContinue
if (-not $wingetPath) {
    Write-Host "ERROR: winget is not installed on this system" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please use one of these alternatives:" -ForegroundColor Yellow
    Write-Host "  1. Install winget from Microsoft Store (App Installer)" -ForegroundColor White
    Write-Host "  2. Download the Hosting Bundle manually from:" -ForegroundColor White
    Write-Host "     https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
    Write-Host "  3. Or use chocolatey:" -ForegroundColor White
    Write-Host "     choco install dotnet-hosting" -ForegroundColor Cyan
    Write-Host ""
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

# Install using winget
Write-Host "Installing ASP.NET Core Runtime & Hosting Bundle..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Gray
Write-Host ""

try {
    # Install ASP.NET Core Runtime 8.0
    Write-Host "Installing ASP.NET Core Runtime 8.0..." -ForegroundColor Cyan
    winget install Microsoft.DotNet.AspNetCore.8 --silent --accept-package-agreements --accept-source-agreements
    
    Write-Host ""
    Write-Host "Installation completed!" -ForegroundColor Green
    Write-Host ""
    
    # Restart IIS
    Write-Host "Restarting IIS..." -ForegroundColor Yellow
    iisreset | Out-Null
    Write-Host "IIS restarted" -ForegroundColor Green
    Write-Host ""
    
    # Verify installation
    Start-Sleep -Seconds 3
    Import-Module WebAdministration -Force -ErrorAction SilentlyContinue
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
        Write-Host "WARNING: Module may not be registered yet" -ForegroundColor Yellow
        Write-Host "Try restarting your computer and running iisreset again" -ForegroundColor White
        Write-Host ""
    }
}
catch {
    Write-Host "ERROR: Installation failed" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please try manual installation:" -ForegroundColor Yellow
    Write-Host "  1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
    Write-Host "  2. Download 'Hosting Bundle' for Windows" -ForegroundColor White
    Write-Host "  3. Run the installer as Administrator" -ForegroundColor White
    Write-Host "  4. After installation, run: iisreset" -ForegroundColor White
    Write-Host ""
}
