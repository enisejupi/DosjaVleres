# PowerShell script to deploy ASP.NET Core application to IIS
# Run this script as Administrator

# Configuration
$appName = "KosovaDoganaModerne"
$appPoolName = "KosovaDoganaModerneAppPool"
$siteName = "KosovaDoganaModerne"
$publishPath = "C:\inetpub\KosovaDoganaModerne"
$port = 80
$hostName = ""  # Leave empty for all hostnames, or set to specific hostname like "dogana.internal"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  IIS Deployment Script - Kosovo Dogana Moderne" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Please right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    exit 1
}

# Import IIS module
Import-Module WebAdministration -ErrorAction Stop

Write-Host "Step 1: Checking ASP.NET Core Hosting Bundle..." -ForegroundColor Green
$aspNetCoreModule = Get-WebGlobalModule | Where-Object { $_.Name -eq "AspNetCoreModuleV2" }
if (-not $aspNetCoreModule) {
    Write-Host "ERROR: ASP.NET Core Module V2 is not installed!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install the ASP.NET Core Hosting Bundle:" -ForegroundColor Yellow
    Write-Host "  1. Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
    Write-Host "  2. Look for 'Hosting Bundle' under ASP.NET Core Runtime" -ForegroundColor White
    Write-Host "  3. Install it and restart IIS using: iisreset" -ForegroundColor White
    Write-Host ""
    exit 1
}
Write-Host "✓ ASP.NET Core Module V2 is installed" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Publishing application to IIS folder..." -ForegroundColor Green
# Stop existing site and app pool if they exist
if (Test-Path "IIS:\Sites\$siteName") {
    Write-Host "  Stopping existing website..." -ForegroundColor Yellow
    Stop-Website -Name $siteName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}
if (Test-Path "IIS:\AppPools\$appPoolName") {
    Write-Host "  Stopping existing application pool..." -ForegroundColor Yellow
    Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

# Create publish directory if it doesn't exist
if (-not (Test-Path $publishPath)) {
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
}

# Publish the application
Write-Host "  Running: dotnet publish -c Release -o $publishPath" -ForegroundColor Yellow
$currentPath = Get-Location
try {
    Set-Location $PSScriptRoot
    dotnet publish -c Release -o $publishPath --no-self-contained
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Publishing failed!" -ForegroundColor Red
        exit 1
    }
} finally {
    Set-Location $currentPath
}

Write-Host "✓ Application published successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3: Creating Application Pool..." -ForegroundColor Green
# Remove existing app pool if exists
if (Test-Path "IIS:\AppPools\$appPoolName") {
    Write-Host "  Removing existing application pool..." -ForegroundColor Yellow
    Remove-WebAppPool -Name $appPoolName
}

# Create new application pool
New-WebAppPool -Name $appPoolName
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "enable32BitAppOnWin64" -Value $false
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "startMode" -Value "AlwaysRunning"

Write-Host "✓ Application Pool '$appPoolName' created successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 4: Setting up IIS Website..." -ForegroundColor Green
# Remove existing website if exists
if (Test-Path "IIS:\Sites\$siteName") {
    Write-Host "  Removing existing website..." -ForegroundColor Yellow
    Remove-Website -Name $siteName
}

# Create new website
$binding = "*:${port}:"
if ($hostName) {
    $binding = "*:${port}:${hostName}"
}

New-Website -Name $siteName `
    -PhysicalPath $publishPath `
    -ApplicationPool $appPoolName `
    -Port $port `
    -HostHeader $hostName `
    -Force

Write-Host "✓ Website '$siteName' created successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 5: Setting permissions..." -ForegroundColor Green
# Set permissions for application pool identity
$acl = Get-Acl $publishPath
$identity = "IIS AppPool\$appPoolName"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($identity, "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($accessRule)
Set-Acl $publishPath $acl

Write-Host "✓ Permissions set for '$identity'" -ForegroundColor Green
Write-Host ""

Write-Host "Step 6: Creating logs directory..." -ForegroundColor Green
$logsPath = Join-Path $publishPath "logs"
if (-not (Test-Path $logsPath)) {
    New-Item -ItemType Directory -Path $logsPath -Force | Out-Null
}
# Set permissions for logs directory
$logsAcl = Get-Acl $logsPath
$logsAcl.SetAccessRule($accessRule)
Set-Acl $logsPath $logsAcl

Write-Host "✓ Logs directory created" -ForegroundColor Green
Write-Host ""

Write-Host "Step 7: Starting Application Pool and Website..." -ForegroundColor Green
Start-WebAppPool -Name $appPoolName
Start-Website -Name $siteName
Start-Sleep -Seconds 3

Write-Host "✓ Application Pool and Website started" -ForegroundColor Green
Write-Host ""

Write-Host "Step 8: Checking status..." -ForegroundColor Green
$appPoolState = (Get-WebAppPoolState -Name $appPoolName).Value
$siteState = (Get-Website -Name $siteName).State

Write-Host "  Application Pool State: $appPoolState" -ForegroundColor $(if ($appPoolState -eq "Started") { "Green" } else { "Red" })
Write-Host "  Website State: $siteState" -ForegroundColor $(if ($siteState -eq "Started") { "Green" } else { "Red" })
Write-Host ""

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Deployment Complete!" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Access the application at:" -ForegroundColor Yellow

# Get server IP addresses
$ipAddresses = Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.PrefixOrigin -ne "WellKnown" } | Select-Object -ExpandProperty IPAddress

Write-Host "  - http://localhost:$port" -ForegroundColor White
foreach ($ip in $ipAddresses) {
    Write-Host "  - http://${ip}:$port" -ForegroundColor White
}

if ($hostName) {
    Write-Host "  - http://${hostName}:$port" -ForegroundColor White
}

Write-Host ""
Write-Host "Default Admin Credentials:" -ForegroundColor Yellow
Write-Host "  Username: shaban.ejupi@dogana-rks.org" -ForegroundColor White
Write-Host "  Password: Admin@123" -ForegroundColor White
Write-Host ""

Write-Host "IMPORTANT NOTES:" -ForegroundColor Cyan
Write-Host "  1. The application is now hosted directly at C:\inetpub\KosovaDoganaModerne" -ForegroundColor White
Write-Host "  2. No external ports are used - accessible only within your network" -ForegroundColor White
Write-Host "  3. The database will be created automatically on first run" -ForegroundColor White
Write-Host "  4. Check IIS logs if you encounter any issues: $logsPath" -ForegroundColor White
Write-Host "  5. If you see errors, check Event Viewer: Windows Logs > Application" -ForegroundColor White
Write-Host ""

Write-Host "Troubleshooting commands:" -ForegroundColor Yellow
Write-Host "  - Check logs: Get-Content '$logsPath\stdout*.log' -Tail 50" -ForegroundColor White
Write-Host "  - Restart app: Restart-WebAppPool -Name '$appPoolName'" -ForegroundColor White
Write-Host "  - Restart IIS: iisreset" -ForegroundColor White
Write-Host ""
