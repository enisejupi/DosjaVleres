# Install ASP.NET Core 8.0 Hosting Bundle for IIS
# This is REQUIRED for IIS to host ASP.NET Core apps
# Run as Administrator

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  ASP.NET Core 8.0 Hosting Bundle Installer" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

# Check current status
Write-Host "Checking installation status..." -ForegroundColor Yellow
$moduleExists = Test-Path "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"

if ($moduleExists) {
    Write-Host "Hosting Bundle is already installed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Running iisreset to ensure it's loaded..." -ForegroundColor Yellow
    iisreset
    Write-Host ""
    Write-Host "Done! Try deploying your app now: .\Deploy-IIS.ps1" -ForegroundColor Green
    Read-Host "Press Enter to exit"
    exit 0
}

Write-Host "Hosting Bundle NOT found. Will install now..." -ForegroundColor Red
Write-Host ""

# Download URL for ASP.NET Core 8.0.11 Hosting Bundle
$url = "https://download.visualstudio.microsoft.com/download/pr/b8cf881b-32d1-4b7e-b5e7-4e98dca27e92/0192c3c49cbbb3d7a4528d0bdd3e1291/dotnet-hosting-8.0.11-win.exe"
$installerPath = "$env:TEMP\dotnet-hosting-8.0.11-win.exe"

Write-Host "Step 1: Downloading installer..." -ForegroundColor Cyan
Write-Host "URL: $url" -ForegroundColor Gray
Write-Host ""

try {
    # Try with system proxy settings
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12 -bor [System.Net.SecurityProtocolType]::Tls13
    $webClient = New-Object System.Net.WebClient
    $webClient.DownloadFile($url, $installerPath)
    
    Write-Host "Download successful!" -ForegroundColor Green
    Write-Host "Saved to: $installerPath" -ForegroundColor Gray
    Write-Host ""
}
catch {
    Write-Host "Download failed with WebClient, trying Invoke-WebRequest..." -ForegroundColor Yellow
    
    try {
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $url -OutFile $installerPath -UseBasicParsing
        Write-Host "Download successful!" -ForegroundColor Green
        Write-Host ""
    }
    catch {
        Write-Host ""
        Write-Host "ERROR: Automatic download failed!" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please download manually:" -ForegroundColor Yellow
        Write-Host "  1. Open this URL in your browser:" -ForegroundColor White
        Write-Host "     https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "  2. Look for 'Hosting Bundle' under ASP.NET Core Runtime 8.0" -ForegroundColor White
        Write-Host "  3. Download and run the installer as Administrator" -ForegroundColor White
        Write-Host "  4. After installation, run: iisreset" -ForegroundColor White
        Write-Host ""
        Write-Host "Alternative: Use the direct link in your browser:" -ForegroundColor Yellow
        Write-Host "  $url" -ForegroundColor Cyan
        Write-Host ""
        
        # Try to open browser
        $openBrowser = Read-Host "Open download page in browser? (Y/N)"
        if ($openBrowser -eq 'Y' -or $openBrowser -eq 'y') {
            Start-Process "https://dotnet.microsoft.com/download/dotnet/8.0"
        }
        
        Read-Host "Press Enter to exit"
        exit 1
    }
}

# Check if file was downloaded
if (-not (Test-Path $installerPath)) {
    Write-Host "ERROR: Installer file not found at $installerPath" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Step 2: Installing Hosting Bundle..." -ForegroundColor Cyan
Write-Host "This will take a few minutes. Please wait..." -ForegroundColor Yellow
Write-Host ""

try {
    $process = Start-Process -FilePath $installerPath -ArgumentList "/install", "/quiet", "/norestart" -Wait -PassThru
    
    if ($process.ExitCode -eq 0 -or $process.ExitCode -eq 3010) {
        Write-Host "Installation completed successfully!" -ForegroundColor Green
        Write-Host ""
        
        # Restart IIS
        Write-Host "Step 3: Restarting IIS..." -ForegroundColor Cyan
        iisreset
        Write-Host ""
        
        # Verify
        Start-Sleep -Seconds 2
        $moduleExists = Test-Path "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
        
        if ($moduleExists) {
            Write-Host "================================================" -ForegroundColor Green
            Write-Host "  SUCCESS! Hosting Bundle is now installed!" -ForegroundColor Green
            Write-Host "================================================" -ForegroundColor Green
            Write-Host ""
            Write-Host "Next step: Deploy your application" -ForegroundColor Yellow
            Write-Host "  Run: .\Deploy-IIS.ps1" -ForegroundColor White
            Write-Host ""
        } else {
            Write-Host "WARNING: Installation completed but module not found" -ForegroundColor Yellow
            Write-Host "You may need to restart your computer" -ForegroundColor White
            Write-Host ""
        }
    } else {
        Write-Host "ERROR: Installation failed with exit code $($process.ExitCode)" -ForegroundColor Red
        Write-Host ""
        Write-Host "Common solutions:" -ForegroundColor Yellow
        Write-Host "  1. Make sure you're running as Administrator" -ForegroundColor White
        Write-Host "  2. Close all browsers and try again" -ForegroundColor White
        Write-Host "  3. Restart your computer and run this script again" -ForegroundColor White
        Write-Host ""
    }
}
catch {
    Write-Host "ERROR: Failed to run installer" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
}

# Cleanup
if (Test-Path $installerPath) {
    Write-Host "Cleaning up installer file..." -ForegroundColor Gray
    Remove-Item $installerPath -Force -ErrorAction SilentlyContinue
}

Write-Host ""
Read-Host "Press Enter to exit"
