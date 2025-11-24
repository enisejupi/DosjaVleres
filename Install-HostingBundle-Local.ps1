# Install ASP.NET Core Hosting Bundle from local file or Downloads folder
# Run as Administrator

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  ASP.NET Core Hosting Bundle Installer" -ForegroundColor Cyan
Write-Host "  (Local File)" -ForegroundColor Cyan
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

# Check if already installed
$moduleExists = Test-Path "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
if ($moduleExists) {
    Write-Host "Hosting Bundle is already installed!" -ForegroundColor Green
    Write-Host "Running iisreset..." -ForegroundColor Yellow
    iisreset
    Write-Host ""
    Write-Host "Done!" -ForegroundColor Green
    Read-Host "Press Enter to exit"
    exit 0
}

Write-Host "Searching for Hosting Bundle installer..." -ForegroundColor Yellow
Write-Host ""

# Search for installer in common locations
$searchPaths = @(
    "$env:USERPROFILE\Downloads\dotnet-hosting*.exe",
    "$env:TEMP\dotnet-hosting*.exe",
    ".\dotnet-hosting*.exe",
    "$PWD\dotnet-hosting*.exe"
)

$installerPath = $null
foreach ($path in $searchPaths) {
    $found = Get-ChildItem -Path $path -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) {
        $installerPath = $found.FullName
        Write-Host "Found installer: $installerPath" -ForegroundColor Green
        break
    }
}

# If not found, ask user for path
if (-not $installerPath) {
    Write-Host "No installer found in common locations." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please provide the full path to the installer file," -ForegroundColor White
    Write-Host "or press Enter to download manually from Microsoft." -ForegroundColor White
    Write-Host ""
    Write-Host "Example: C:\Users\YourName\Downloads\dotnet-hosting-8.0.11-win.exe" -ForegroundColor Gray
    Write-Host ""
    $installerPath = Read-Host "Installer path"
    
    if ([string]::IsNullOrWhiteSpace($installerPath)) {
        Write-Host ""
        Write-Host "Opening download page..." -ForegroundColor Yellow
        Start-Process "https://dotnet.microsoft.com/download/dotnet/8.0"
        Write-Host ""
        Write-Host "Please download 'Hosting Bundle' for ASP.NET Core 8.0" -ForegroundColor White
        Write-Host "Then run this script again." -ForegroundColor White
        Write-Host ""
        Read-Host "Press Enter to exit"
        exit 0
    }
}

# Verify file exists
if (-not (Test-Path $installerPath)) {
    Write-Host ""
    Write-Host "ERROR: File not found: $installerPath" -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Installing from: $installerPath" -ForegroundColor Cyan
Write-Host "This may take a few minutes..." -ForegroundColor Yellow
Write-Host ""

try {
    # Run installer
    $process = Start-Process -FilePath $installerPath -ArgumentList "/install", "/quiet", "/norestart" -Wait -PassThru
    
    if ($process.ExitCode -eq 0 -or $process.ExitCode -eq 3010) {
        Write-Host "Installation completed!" -ForegroundColor Green
        Write-Host ""
        
        # Restart IIS
        Write-Host "Restarting IIS..." -ForegroundColor Yellow
        iisreset
        Write-Host ""
        
        # Verify installation
        Start-Sleep -Seconds 2
        $moduleExists = Test-Path "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
        
        if ($moduleExists) {
            Write-Host "================================================" -ForegroundColor Green
            Write-Host "  SUCCESS!" -ForegroundColor Green
            Write-Host "  Hosting Bundle is now installed!" -ForegroundColor Green
            Write-Host "================================================" -ForegroundColor Green
            Write-Host ""
            
            # Verify in IIS
            Import-Module WebAdministration -ErrorAction SilentlyContinue
            $module = Get-WebGlobalModule | Where-Object { $_.Name -eq "AspNetCoreModuleV2" }
            if ($module) {
                Write-Host "Module registered in IIS: $($module.Name)" -ForegroundColor Green
            } else {
                Write-Host "WARNING: Module installed but not yet registered in IIS" -ForegroundColor Yellow
                Write-Host "Try restarting your computer if issues persist" -ForegroundColor White
            }
            
            Write-Host ""
            Write-Host "Next step: Deploy your application" -ForegroundColor Cyan
            Write-Host "  Run: .\Deploy-IIS.ps1" -ForegroundColor White
            Write-Host ""
        } else {
            Write-Host "WARNING: Module file not found after installation" -ForegroundColor Yellow
            Write-Host "Please restart your computer and check again" -ForegroundColor White
            Write-Host ""
        }
    }
    elseif ($process.ExitCode -eq 1638) {
        Write-Host "INFO: A newer version is already installed" -ForegroundColor Yellow
        Write-Host "Running iisreset..." -ForegroundColor Yellow
        iisreset
        Write-Host ""
    }
    else {
        Write-Host "ERROR: Installation failed with exit code: $($process.ExitCode)" -ForegroundColor Red
        Write-Host ""
        Write-Host "Common solutions:" -ForegroundColor Yellow
        Write-Host "  1. Make sure you're running PowerShell as Administrator" -ForegroundColor White
        Write-Host "  2. Close all browsers and IIS Manager" -ForegroundColor White
        Write-Host "  3. Try restarting your computer first" -ForegroundColor White
        Write-Host ""
    }
}
catch {
    Write-Host "ERROR: Failed to run installer" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
}

Write-Host ""
Read-Host "Press Enter to exit"
