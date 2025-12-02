# ============================================================================
# Kosovo Dogana - Complete Build & Deploy Script
# ============================================================================
# This script handles the complete build and deployment process
# ============================================================================

param(
    [switch]$SkipBuild,
    [switch]$Verify,
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Kosovo Dogana - Build & Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ProjectDir = $PSScriptRoot
$ProjectFile = Join-Path $ProjectDir "KosovaDoganaModerne.csproj"
$DeployPath = "C:\inetpub\wwwroot\DoganaDosjaVleres"
$AppPoolName = "DoganaDosjaVleres"

# Check if project file exists
if (-not (Test-Path $ProjectFile)) {
    Write-Host "? ERROR: Project file not found: $ProjectFile" -ForegroundColor Red
    exit 1
}

Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Project:      KosovaDoganaModerne" -ForegroundColor White
Write-Host "  Build Config: $Configuration" -ForegroundColor White
Write-Host "  Deploy To:    $DeployPath" -ForegroundColor White
Write-Host "  App Pool:     $AppPoolName" -ForegroundColor White
Write-Host ""

# ============================================================================
# STEP 0: Verify Setup (Optional)
# ============================================================================
if ($Verify) {
    Write-Host "[Optional] Running verification checks..." -ForegroundColor Yellow
    Write-Host ""
    
    $verifyScript = Join-Path $ProjectDir "Verify-IIS-Setup.ps1"
    if (Test-Path $verifyScript) {
        & $verifyScript
        
        Write-Host ""
        $continue = Read-Host "Continue with deployment? (Y/N)"
        if ($continue -ne "Y" -and $continue -ne "y") {
            Write-Host "Deployment cancelled by user." -ForegroundColor Yellow
            exit 0
        }
        Write-Host ""
    }
}

# ============================================================================
# STEP 1: Clean Previous Build
# ============================================================================
Write-Host "[Step 1/5] Cleaning previous build..." -ForegroundColor Yellow

try {
    $binPath = Join-Path $ProjectDir "bin"
    $objPath = Join-Path $ProjectDir "obj"
    
    if (Test-Path $binPath) {
        Remove-Item -Path $binPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ? Cleaned bin folder" -ForegroundColor Green
    }
    
    if (Test-Path $objPath) {
        Remove-Item -Path $objPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ? Cleaned obj folder" -ForegroundColor Green
    }
} catch {
    Write-Host "  ? Warning: Could not clean all folders: $($_.Exception.Message)" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# STEP 2: Restore NuGet Packages
# ============================================================================
if (-not $SkipBuild) {
    Write-Host "[Step 2/5] Restoring NuGet packages..." -ForegroundColor Yellow
    
    try {
        $restoreOutput = dotnet restore $ProjectFile 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ? Packages restored successfully" -ForegroundColor Green
        } else {
            Write-Host "  ? ERROR: Package restore failed" -ForegroundColor Red
            Write-Host $restoreOutput
            exit 1
        }
    } catch {
        Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""
}

# ============================================================================
# STEP 3: Build Project
# ============================================================================
if (-not $SkipBuild) {
    Write-Host "[Step 3/5] Building project..." -ForegroundColor Yellow
    
    try {
        $buildOutput = dotnet build $ProjectFile -c $Configuration --no-restore 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ? Build completed successfully" -ForegroundColor Green
        } else {
            Write-Host "  ? ERROR: Build failed" -ForegroundColor Red
            Write-Host $buildOutput
            exit 1
        }
    } catch {
        Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""
} else {
    Write-Host "[Step 2-3/5] Skipping build (using existing binaries)..." -ForegroundColor Yellow
    Write-Host ""
}

# ============================================================================
# STEP 4: Publish Application
# ============================================================================
if (-not $SkipBuild) {
    Write-Host "[Step 4/5] Publishing application..." -ForegroundColor Yellow
    
    $publishDir = Join-Path $ProjectDir "bin\$Configuration\net10.0\publish"
    
    try {
        $publishOutput = dotnet publish $ProjectFile -c $Configuration --no-build -o $publishDir 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ? Application published to: $publishDir" -ForegroundColor Green
        } else {
            Write-Host "  ? ERROR: Publish failed" -ForegroundColor Red
            Write-Host $publishOutput
            exit 1
        }
    } catch {
        Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""
} else {
    Write-Host "[Step 4/5] Using existing published files..." -ForegroundColor Yellow
    Write-Host ""
}

# ============================================================================
# STEP 5: Deploy to IIS
# ============================================================================
Write-Host "[Step 5/5] Deploying to IIS..." -ForegroundColor Yellow

$deployScript = Join-Path $ProjectDir "Deploy-ToIIS.ps1"
if (Test-Path $deployScript) {
    Write-Host "  Executing Deploy-ToIIS.ps1..." -ForegroundColor White
    Write-Host ""
    
    & $deployScript -AppPoolName $AppPoolName -DeployPath $DeployPath
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "  ? Deployment script reported issues" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ? Deploy-ToIIS.ps1 not found, attempting manual deployment..." -ForegroundColor Yellow
    
    $publishDir = Join-Path $ProjectDir "bin\$Configuration\net10.0\publish"
    
    if (-not (Test-Path $publishDir)) {
        Write-Host "  ? ERROR: Publish directory not found: $publishDir" -ForegroundColor Red
        exit 1
    }
    
    # Stop app pool
    try {
        Import-Module WebAdministration -ErrorAction SilentlyContinue
        if (Get-Command "Stop-WebAppPool" -ErrorAction SilentlyContinue) {
            Stop-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
            Start-Sleep -Seconds 2
        }
    } catch {
        Write-Host "  ? Could not stop app pool" -ForegroundColor Yellow
    }
    
    # Copy files
    try {
        if (-not (Test-Path $DeployPath)) {
            New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
        }
        
        Copy-Item -Path "$publishDir\*" -Destination $DeployPath -Recurse -Force
        Write-Host "  ? Files copied to $DeployPath" -ForegroundColor Green
    } catch {
        Write-Host "  ? ERROR: Failed to copy files: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    
    # Start app pool
    try {
        if (Get-Command "Start-WebAppPool" -ErrorAction SilentlyContinue) {
            Start-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
            Start-Sleep -Seconds 2
        }
    } catch {
        Write-Host "  ? Could not start app pool" -ForegroundColor Yellow
    }
}

Write-Host ""

# ============================================================================
# SUMMARY
# ============================================================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host ""

Write-Host "1. Test your application:" -ForegroundColor White
Write-Host "   Local:   http://localhost/DoganaDosjaVleres" -ForegroundColor Cyan
Write-Host "   Network: http://10.10.173.154/DoganaDosjaVleres" -ForegroundColor Cyan
Write-Host ""

Write-Host "2. Verify application pool is running:" -ForegroundColor White
Write-Host "   Get-WebAppPoolState -Name '$AppPoolName'" -ForegroundColor Cyan
Write-Host ""

Write-Host "3. Check for errors:" -ForegroundColor White
Write-Host "   Get-Content '$DeployPath\logs\*.log' -Tail 50" -ForegroundColor Cyan
Write-Host ""

Write-Host "4. Monitor Event Viewer for any issues:" -ForegroundColor White
Write-Host "   Get-EventLog -LogName Application -Source 'IIS*' -EntryType Error -Newest 5" -ForegroundColor Cyan
Write-Host ""

Write-Host "Troubleshooting:" -ForegroundColor Yellow
Write-Host "  - If app pool stops: Check stdout logs and Event Viewer" -ForegroundColor White
Write-Host "  - If 502.5 error: Ensure .NET 10 Hosting Bundle is installed" -ForegroundColor White
Write-Host "  - If 404 error: Verify PathBase in appsettings.Production.json" -ForegroundColor White
Write-Host "  - Run: .\Verify-IIS-Setup.ps1 for detailed diagnostics" -ForegroundColor White
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
