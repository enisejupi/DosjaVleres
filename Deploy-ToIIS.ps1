# ============================================================================
# Kosovo Dogana - Quick Deployment Script
# ============================================================================
# Deploys the application to IIS without rebuilding
# Use this after running: dotnet build -c Release
# ============================================================================

param(
    [string]$AppPoolName = "DoganaDosjaVleres",
    [string]$DeployPath = "C:\inetpub\wwwroot\DoganaDosjaVleres"
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Kosovo Dogana Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
$isAdmin = $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "? WARNING: Not running as Administrator" -ForegroundColor Yellow
    Write-Host "Some operations may fail without admin privileges." -ForegroundColor Yellow
    Write-Host ""
}

# Find the publish directory
$projectDir = $PSScriptRoot
$publishDir = Join-Path $projectDir "bin\Release\net10.0\publish"

Write-Host "Project Directory: $projectDir" -ForegroundColor White
Write-Host "Publish Directory: $publishDir" -ForegroundColor White
Write-Host "Deploy Target: $DeployPath" -ForegroundColor White
Write-Host ""

# Check if publish directory exists
if (-not (Test-Path $publishDir)) {
    Write-Host "? ERROR: Publish directory not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please build the project first:" -ForegroundColor Yellow
    Write-Host "  dotnet build -c Release" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or publish it:" -ForegroundColor Yellow
    Write-Host "  dotnet publish -c Release" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

Write-Host "? Publish directory found" -ForegroundColor Green
Write-Host ""

# Check for pending migrations
Write-Host "[Pre-Check] Checking for pending migrations..." -ForegroundColor Yellow
try {
    Push-Location $projectDir
    
    # Check if dotnet ef is available
    $efToolCheck = & dotnet tool list --global 2>&1 | Select-String "dotnet-ef"
    
    if ($efToolCheck) {
        # Set environment to Production for migration check
        $env:ASPNETCORE_ENVIRONMENT = "Production"
        
        # Check for pending migrations
        $pendingOutput = & dotnet ef migrations list --no-build 2>&1
        $pendingMigrations = $pendingOutput | Select-String "\(Pending\)"
        
        if ($pendingMigrations) {
            Write-Host ""
            Write-Host "? WARNING: Pending migrations detected!" -ForegroundColor Yellow
            Write-Host ""
            Write-Host "The following migrations have not been applied to the database:" -ForegroundColor Yellow
            $pendingMigrations | ForEach-Object {
                Write-Host "  $_" -ForegroundColor Red
            }
            Write-Host ""
            Write-Host "Recommendations:" -ForegroundColor Yellow
            Write-Host "  1. After deployment, run migrations using:" -ForegroundColor White
            Write-Host "     .\Troubleshoot-Database.ps1 -RunMigrations" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "  2. Or manually in the deployment directory:" -ForegroundColor White
            Write-Host "     cd $DeployPath" -ForegroundColor Cyan
            Write-Host "     `$env:ASPNETCORE_ENVIRONMENT='Production'" -ForegroundColor Cyan
            Write-Host "     dotnet ef database update --no-build" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "Press any key to continue with deployment (Ctrl+C to cancel)..." -ForegroundColor Yellow
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
            Write-Host ""
        } else {
            Write-Host "? No pending migrations" -ForegroundColor Green
        }
    } else {
        Write-Host "? dotnet-ef tool not installed - skipping migration check" -ForegroundColor Yellow
        Write-Host "  Install with: dotnet tool install --global dotnet-ef" -ForegroundColor Cyan
    }
    
    Pop-Location
} catch {
    Write-Host "? Could not check migrations: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "  Continuing with deployment..." -ForegroundColor Gray
    Pop-Location
}
Write-Host ""

# Step 1: Stop Application Pool
Write-Host "[Step 1/5] Stopping application pool..." -ForegroundColor Yellow
try {
    Import-Module WebAdministration -ErrorAction SilentlyContinue
    
    if (Get-Command "Stop-WebAppPool" -ErrorAction SilentlyContinue) {
        $pool = Get-Item "IIS:\AppPools\$AppPoolName" -ErrorAction SilentlyContinue
        if ($pool) {
            $state = Get-WebAppPoolState -Name $AppPoolName
            if ($state.Value -ne "Stopped") {
                Stop-WebAppPool -Name $AppPoolName -ErrorAction Stop
                Start-Sleep -Seconds 3
                Write-Host "  ? Application pool stopped" -ForegroundColor Green
            } else {
                Write-Host "  Application pool already stopped" -ForegroundColor Gray
            }
        } else {
            Write-Host "  Application pool '$AppPoolName' not found (may not exist yet)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  IIS module not available - skipping pool stop" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ? Could not stop app pool: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "  Continuing anyway..." -ForegroundColor Yellow
}
Write-Host ""

# Step 2: Create deployment directory
Write-Host "[Step 2/5] Preparing deployment directory..." -ForegroundColor Yellow
try {
    if (-not (Test-Path $DeployPath)) {
        New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
        Write-Host "  ? Created directory: $DeployPath" -ForegroundColor Green
    } else {
        Write-Host "  ? Directory exists: $DeployPath" -ForegroundColor Green
    }
    
    # Create logs directory
    $logsPath = Join-Path $DeployPath "logs"
    if (-not (Test-Path $logsPath)) {
        New-Item -ItemType Directory -Path $logsPath -Force | Out-Null
        Write-Host "  ? Created logs directory" -ForegroundColor Green
    }
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 3: Copy files
Write-Host "[Step 3/5] Copying files..." -ForegroundColor Yellow
try {
    # Use robocopy for reliable copying
    # /MIR = Mirror directory tree (removes files that don't exist in source)
    # /XO = Exclude older files (don't overwrite newer files)
    # /R:2 = Retry 2 times on failed copies
    # /W:3 = Wait 3 seconds between retries
    # /NP = No progress (cleaner output)
    
    $robocopyArgs = @(
        "`"$publishDir`"",
        "`"$DeployPath`"",
        "/MIR",
        "/R:2",
        "/W:3",
        "/NP"
    )
    
    $result = Start-Process -FilePath "robocopy.exe" `
                            -ArgumentList $robocopyArgs `
                            -Wait `
                            -PassThru `
                            -NoNewWindow
    
    # Robocopy exit codes: 0-7 are success, 8+ are errors
    if ($result.ExitCode -le 7) {
        Write-Host "  ? Files copied successfully (exit code: $($result.ExitCode))" -ForegroundColor Green
    } else {
        throw "Robocopy failed with exit code $($result.ExitCode)"
    }
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "  Falling back to Copy-Item..." -ForegroundColor Yellow
    try {
        Copy-Item -Path "$publishDir\*" -Destination $DeployPath -Recurse -Force
        Write-Host "  ? Files copied using Copy-Item" -ForegroundColor Green
    } catch {
        Write-Host "  ? Copy-Item also failed: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}
Write-Host ""

# Step 4: Set permissions for database and logs
Write-Host "[Step 4/5] Setting file permissions..." -ForegroundColor Yellow
try {
    $appPoolIdentity = "IIS AppPool\$AppPoolName"
    
    # Set permissions on deployment directory
    Write-Host "  Setting directory permissions..." -ForegroundColor Gray
    $acl = Get-Acl $DeployPath
    $permission = New-Object System.Security.AccessControl.FileSystemAccessRule(
        $appPoolIdentity,
        "Modify",
        "ContainerInherit,ObjectInherit",
        "None",
        "Allow"
    )
    $acl.SetAccessRule($permission)
    Set-Acl -Path $DeployPath -AclObject $acl
    Write-Host "  ? Directory permissions set" -ForegroundColor Green
    
    # Set permissions on database file if it exists
    $dbPath = Join-Path $DeployPath "KosovaDoganaModerne.db"
    if (Test-Path $dbPath) {
        Write-Host "  Setting database file permissions..." -ForegroundColor Gray
        $acl = Get-Acl $dbPath
        $permission = New-Object System.Security.AccessControl.FileSystemAccessRule(
            $appPoolIdentity,
            "Modify",
            "Allow"
        )
        $acl.SetAccessRule($permission)
        Set-Acl -Path $dbPath -AclObject $acl
        Write-Host "  ? Database file permissions set" -ForegroundColor Green
    } else {
        Write-Host "  ? Database file not found (will be created on first run)" -ForegroundColor Yellow
    }
    
    # Set permissions on logs directory
    $logsPath = Join-Path $DeployPath "logs"
    if (Test-Path $logsPath) {
        Write-Host "  Setting logs directory permissions..." -ForegroundColor Gray
        $acl = Get-Acl $logsPath
        $permission = New-Object System.Security.AccessControl.FileSystemAccessRule(
            $appPoolIdentity,
            "Modify",
            "ContainerInherit,ObjectInherit",
            "None",
            "Allow"
        )
        $acl.SetAccessRule($permission)
        Set-Acl -Path $logsPath -AclObject $acl
        Write-Host "  ? Logs directory permissions set" -ForegroundColor Green
    }
    
} catch {
    Write-Host "  ? Could not set permissions: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "  You may need to set permissions manually" -ForegroundColor Yellow
}
Write-Host ""

# Step 5: Start Application Pool
Write-Host "[Step 5/5] Starting application pool..." -ForegroundColor Yellow
try {
    if (Get-Command "Start-WebAppPool" -ErrorAction SilentlyContinue) {
        $pool = Get-Item "IIS:\AppPools\$AppPoolName" -ErrorAction SilentlyContinue
        if ($pool) {
            Start-WebAppPool -Name $AppPoolName -ErrorAction Stop
            Start-Sleep -Seconds 2
            
            $state = Get-WebAppPoolState -Name $AppPoolName
            Write-Host "  ? Application pool started (State: $($state.Value))" -ForegroundColor Green
        } else {
            Write-Host "  Application pool '$AppPoolName' not found" -ForegroundColor Yellow
            Write-Host "  Run Setup-IIS-Production.ps1 to create it" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  IIS module not available - skipping pool start" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ? Could not start app pool: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "  You may need to start it manually" -ForegroundColor Yellow
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Application deployed to: $DeployPath" -ForegroundColor White
Write-Host ""
Write-Host "Test your application:" -ForegroundColor Yellow
Write-Host "  Local:   http://localhost/DoganaDosjaVleres" -ForegroundColor Cyan
Write-Host "  Network: http://10.10.173.154/DoganaDosjaVleres" -ForegroundColor Cyan
Write-Host ""
Write-Host "Diagnostic endpoints:" -ForegroundColor Yellow
Write-Host "  Health Check: http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health" -ForegroundColor Cyan
Write-Host "  System Info:  http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Index" -ForegroundColor Cyan
Write-Host "  View Logs:    http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Logs" -ForegroundColor Cyan
Write-Host ""
Write-Host "Troubleshooting commands:" -ForegroundColor Yellow
Write-Host "  # Check app pool status" -ForegroundColor Gray
Write-Host "  Get-WebAppPoolState -Name '$AppPoolName'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Restart app pool" -ForegroundColor Gray
Write-Host "  Restart-WebAppPool -Name '$AppPoolName'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Check application" -ForegroundColor Gray
Write-Host "  Get-WebApplication -Site 'Default Web Site' -Name 'DoganaDosjaVleres'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # View logs" -ForegroundColor Gray
Write-Host "  Get-Content '$DeployPath\logs\*.log' -Tail 50" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Check database permissions" -ForegroundColor Gray
Write-Host "  icacls '$DeployPath\KosovaDoganaModerne.db'" -ForegroundColor Cyan
Write-Host ""
