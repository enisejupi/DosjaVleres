# ============================================================================
# Kosovo Dogana - Database Troubleshooting Script
# ============================================================================
# Diagnoses and fixes common database issues in production
# ============================================================================

param(
    [string]$DeployPath = "C:\inetpub\wwwroot\DoganaDosjaVleres",
    [string]$AppPoolName = "DoganaDosjaVleres",
    [switch]$FixPermissions,
    [switch]$RunMigrations,
    [switch]$CreateDatabase
)

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Troubleshooting Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
$isAdmin = $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "? WARNING: Not running as Administrator" -ForegroundColor Yellow
    Write-Host "Some operations require admin privileges." -ForegroundColor Yellow
    Write-Host ""
}

# ============================================================================
# 1. CHECK DATABASE FILE
# ============================================================================
Write-Host "[1] Database File Check" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

$dbPath = Join-Path $DeployPath "KosovaDoganaModerne.db"
Write-Host "Database Path: $dbPath" -ForegroundColor White

if (Test-Path $dbPath) {
    $dbFile = Get-Item $dbPath
    Write-Host "? Database file exists" -ForegroundColor Green
    Write-Host "  Size: $([Math]::Round($dbFile.Length/1MB, 2)) MB" -ForegroundColor White
    Write-Host "  Last Modified: $(Get-Date $dbFile.LastWriteTime -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
    Write-Host "  Created: $(Get-Date $dbFile.CreationTime -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
} else {
    Write-Host "? Database file NOT FOUND" -ForegroundColor Red
    
    if ($CreateDatabase) {
        Write-Host ""
        Write-Host "Attempting to create database by running migrations..." -ForegroundColor Yellow
        $RunMigrations = $true
    } else {
        Write-Host ""
        Write-Host "Database will be created automatically on first application run," -ForegroundColor Yellow
        Write-Host "or you can create it manually with:" -ForegroundColor Yellow
        Write-Host "  .\Troubleshoot-Database.ps1 -CreateDatabase" -ForegroundColor Cyan
    }
}
Write-Host ""

# ============================================================================
# 2. CHECK DATABASE PERMISSIONS
# ============================================================================
Write-Host "[2] Database Permissions" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

if (Test-Path $dbPath) {
    $appPoolIdentity = "IIS AppPool\$AppPoolName"
    Write-Host "Checking permissions for: $appPoolIdentity" -ForegroundColor White
    
    try {
        $acl = Get-Acl $dbPath
        $permissions = $acl.Access | Where-Object { $_.IdentityReference -eq $appPoolIdentity }
        
        if ($permissions) {
            Write-Host "? IIS AppPool has permissions" -ForegroundColor Green
            foreach ($perm in $permissions) {
                $color = if ($perm.FileSystemRights -match "Modify|FullControl") { "Green" } else { "Yellow" }
                Write-Host "  - Rights: $($perm.FileSystemRights)" -ForegroundColor $color
                Write-Host "  - Type: $($perm.AccessControlType)" -ForegroundColor White
            }
            
            $hasModify = $permissions | Where-Object { $_.FileSystemRights -match "Modify|FullControl" }
            if (-not $hasModify) {
                Write-Host ""
                Write-Host "? WARNING: AppPool needs Modify permissions" -ForegroundColor Yellow
                
                if ($FixPermissions) {
                    Write-Host "Fixing permissions..." -ForegroundColor Yellow
                    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
                        $appPoolIdentity, "Modify", "Allow"
                    )
                    $acl.SetAccessRule($rule)
                    Set-Acl -Path $dbPath -AclObject $acl
                    Write-Host "? Permissions updated" -ForegroundColor Green
                } else {
                    Write-Host "Run with -FixPermissions to fix automatically" -ForegroundColor Cyan
                }
            }
        } else {
            Write-Host "? No permissions found for IIS AppPool" -ForegroundColor Red
            
            if ($FixPermissions) {
                Write-Host "Adding permissions..." -ForegroundColor Yellow
                $rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
                    $appPoolIdentity, "Modify", "Allow"
                )
                $acl.AddAccessRule($rule)
                Set-Acl -Path $dbPath -AclObject $acl
                Write-Host "? Permissions added" -ForegroundColor Green
            } else {
                Write-Host "Fix with: icacls '$dbPath' /grant 'IIS AppPool\$AppPoolName`:(M)'" -ForegroundColor Cyan
                Write-Host "Or run: .\Troubleshoot-Database.ps1 -FixPermissions" -ForegroundColor Cyan
            }
        }
    } catch {
        Write-Host "? Error checking permissions: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "? Database file not found - skipping permission check" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# 3. CHECK DIRECTORY PERMISSIONS
# ============================================================================
Write-Host "[3] Directory Permissions" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

if (Test-Path $DeployPath) {
    $appPoolIdentity = "IIS AppPool\$AppPoolName"
    Write-Host "Checking directory: $DeployPath" -ForegroundColor White
    
    try {
        $acl = Get-Acl $DeployPath
        $permissions = $acl.Access | Where-Object { 
            $_.IdentityReference -eq $appPoolIdentity -and
            $_.FileSystemRights -match "Modify|FullControl"
        }
        
        if ($permissions) {
            Write-Host "? Directory has proper permissions" -ForegroundColor Green
        } else {
            Write-Host "? Directory may lack proper permissions" -ForegroundColor Yellow
            
            if ($FixPermissions) {
                Write-Host "Setting directory permissions..." -ForegroundColor Yellow
                $rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
                    $appPoolIdentity,
                    "Modify",
                    "ContainerInherit,ObjectInherit",
                    "None",
                    "Allow"
                )
                $acl.SetAccessRule($rule)
                Set-Acl -Path $DeployPath -AclObject $acl
                Write-Host "? Directory permissions set" -ForegroundColor Green
            } else {
                Write-Host "Fix with: icacls '$DeployPath' /grant 'IIS AppPool\$AppPoolName`:(OI)(CI)M' /T" -ForegroundColor Cyan
                Write-Host "Or run: .\Troubleshoot-Database.ps1 -FixPermissions" -ForegroundColor Cyan
            }
        }
    } catch {
        Write-Host "? Error checking directory permissions: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

# ============================================================================
# 4. CHECK LOGS DIRECTORY
# ============================================================================
Write-Host "[4] Logs Directory" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

$logsPath = Join-Path $DeployPath "logs"

if (Test-Path $logsPath) {
    Write-Host "? Logs directory exists: $logsPath" -ForegroundColor Green
    
    # Check permissions
    $appPoolIdentity = "IIS AppPool\$AppPoolName"
    try {
        $acl = Get-Acl $logsPath
        $permissions = $acl.Access | Where-Object { 
            $_.IdentityReference -eq $appPoolIdentity -and
            $_.FileSystemRights -match "Modify|FullControl"
        }
        
        if ($permissions) {
            Write-Host "? Logs directory has proper permissions" -ForegroundColor Green
        } else {
            Write-Host "? Logs directory may lack proper permissions" -ForegroundColor Yellow
            
            if ($FixPermissions) {
                Write-Host "Setting logs directory permissions..." -ForegroundColor Yellow
                $rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
                    $appPoolIdentity,
                    "Modify",
                    "ContainerInherit,ObjectInherit",
                    "None",
                    "Allow"
                )
                $acl.SetAccessRule($rule)
                Set-Acl -Path $logsPath -AclObject $acl
                Write-Host "? Logs directory permissions set" -ForegroundColor Green
            }
        }
    } catch {
        Write-Host "? Could not check logs permissions: $($_.Exception.Message)" -ForegroundColor Yellow
    }
    
    # List recent log files
    $logFiles = Get-ChildItem $logsPath -Filter "*.log" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
    if ($logFiles) {
        Write-Host ""
        Write-Host "Recent log files:" -ForegroundColor White
        foreach ($log in $logFiles | Select-Object -First 5) {
            Write-Host "  - $($log.Name) ($(Get-Date $log.LastWriteTime -Format 'yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "? Logs directory NOT FOUND" -ForegroundColor Red
    Write-Host "Creating logs directory..." -ForegroundColor Yellow
    
    try {
        New-Item -ItemType Directory -Path $logsPath -Force | Out-Null
        Write-Host "? Logs directory created" -ForegroundColor Green
        
        if ($FixPermissions) {
            Write-Host "Setting permissions on new logs directory..." -ForegroundColor Yellow
            $appPoolIdentity = "IIS AppPool\$AppPoolName"
            $acl = Get-Acl $logsPath
            $rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
                $appPoolIdentity,
                "Modify",
                "ContainerInherit,ObjectInherit",
                "None",
                "Allow"
            )
            $acl.SetAccessRule($rule)
            Set-Acl -Path $logsPath -AclObject $acl
            Write-Host "? Permissions set" -ForegroundColor Green
        }
    } catch {
        Write-Host "? Failed to create logs directory: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

# ============================================================================
# 5. CHECK CONNECTION STRING
# ============================================================================
Write-Host "[5] Connection String Configuration" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

$appsettingsPath = Join-Path $DeployPath "appsettings.Production.json"

if (Test-Path $appsettingsPath) {
    try {
        $appsettings = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
        $connString = $appsettings.ConnectionStrings.DefaultConnection
        
        Write-Host "? Connection string found" -ForegroundColor Green
        Write-Host "  $connString" -ForegroundColor White
        
        # Parse connection string to get database path
        if ($connString -match "Data Source=([^;]+)") {
            $configDbPath = $matches[1]
            Write-Host ""
            Write-Host "Configured database path: $configDbPath" -ForegroundColor White
            
            if ([System.IO.Path]::IsPathRooted($configDbPath)) {
                Write-Host "  (Absolute path)" -ForegroundColor Gray
            } else {
                Write-Host "  (Relative path - will be resolved from application root)" -ForegroundColor Gray
                $resolvedPath = Join-Path $DeployPath $configDbPath
                Write-Host "  Resolves to: $resolvedPath" -ForegroundColor Gray
            }
        }
    } catch {
        Write-Host "? Error reading appsettings: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "? appsettings.Production.json NOT FOUND" -ForegroundColor Red
}
Write-Host ""

# ============================================================================
# 6. RUN MIGRATIONS (if requested)
# ============================================================================
if ($RunMigrations) {
    Write-Host "[6] Running Database Migrations" -ForegroundColor Yellow
    Write-Host "----------------------------------------" -ForegroundColor Gray
    
    $dllPath = Join-Path $DeployPath "KosovaDoganaModerne.dll"
    
    if (Test-Path $dllPath) {
        Write-Host "Application DLL: $dllPath" -ForegroundColor White
        
        try {
            Write-Host "Running migrations..." -ForegroundColor Yellow
            
            # Change to deployment directory
            Push-Location $DeployPath
            
            # Set environment to Production
            $env:ASPNETCORE_ENVIRONMENT = "Production"
            
            # Check if dotnet ef is installed
            $efToolCheck = & dotnet tool list --global 2>&1 | Select-String "dotnet-ef"
            
            if (-not $efToolCheck) {
                Write-Host "  Installing dotnet-ef tool..." -ForegroundColor Yellow
                & dotnet tool install --global dotnet-ef 2>&1 | Out-Null
                Write-Host "  ? dotnet-ef tool installed" -ForegroundColor Green
            }
            
            Write-Host "  Applying pending migrations to database..." -ForegroundColor Gray
            
            # Run migrations using dotnet ef database update
            # For deployed applications, we use the DLL as the assembly and specify the startup project explicitly
            # The --no-build flag is important since this is a published app
            $output = & dotnet ef database update --assembly $dllPath --startup-project $dllPath --no-build --verbose 2>&1
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ? Migrations completed successfully" -ForegroundColor Green
                
                # Show applied migrations summary
                Write-Host ""
                Write-Host "  Applied migrations:" -ForegroundColor White
                $output | Select-String "Applying migration" | ForEach-Object {
                    Write-Host "    $_" -ForegroundColor Gray
                }
                
                # Verify database was updated
                if (Test-Path $dbPath) {
                    $dbFile = Get-Item $dbPath
                    Write-Host ""
                    Write-Host "  Database updated:" -ForegroundColor White
                    Write-Host "    Size: $([Math]::Round($dbFile.Length/1MB, 2)) MB" -ForegroundColor Gray
                    Write-Host "    Last Modified: $(Get-Date $dbFile.LastWriteTime -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
                }
            } else {
                Write-Host "  ? Migrations failed (exit code: $LASTEXITCODE)" -ForegroundColor Red
                Write-Host ""
                Write-Host "  Error details:" -ForegroundColor Yellow
                $output | Write-Host -ForegroundColor Red
                
                Write-Host ""
                Write-Host "  Troubleshooting tips:" -ForegroundColor Yellow
                Write-Host "    1. Ensure the database file is not locked by another process" -ForegroundColor Gray
                Write-Host "    2. Check that IIS AppPool has proper permissions" -ForegroundColor Gray
                Write-Host "    3. Verify connection string in appsettings.Production.json" -ForegroundColor Gray
                Write-Host "    4. Try stopping IIS: Stop-WebAppPool -Name '$AppPoolName'" -ForegroundColor Gray
                Write-Host "    5. The app must be built from source for migrations - cannot run on deployed DLLs" -ForegroundColor Gray
            }
            
            Pop-Location
        } catch {
            Write-Host "  ? Error running migrations: $($_.Exception.Message)" -ForegroundColor Red
            Write-Host ""
            Write-Host "  Note: For deployed applications, migrations should be run during deployment," -ForegroundColor Yellow
            Write-Host "        not on the deployed binaries. Use the Deploy-ToIIS.ps1 script which" -ForegroundColor Yellow
            Write-Host "        runs migrations before deployment." -ForegroundColor Yellow
            Pop-Location
        }
    } else {
        Write-Host "? Application DLL not found: $dllPath" -ForegroundColor Red
        Write-Host "  Make sure the application is deployed first" -ForegroundColor Yellow
        Write-Host "  Run: .\Deploy-ToIIS.ps1" -ForegroundColor Cyan
    }
    Write-Host ""
}

# ============================================================================
# 7. TEST DATABASE ACCESS
# ============================================================================
Write-Host "[7] Test Database Access" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

if (Test-Path $dbPath) {
    Write-Host "Testing file read access..." -ForegroundColor Gray
    try {
        $stream = [System.IO.File]::OpenRead($dbPath)
        $stream.Close()
        Write-Host "? Can read database file" -ForegroundColor Green
    } catch {
        Write-Host "? Cannot read database file: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Write-Host "Testing file write access..." -ForegroundColor Gray
    try {
        $stream = [System.IO.File]::OpenWrite($dbPath)
        $stream.Close()
        Write-Host "? Can write to database file" -ForegroundColor Green
    } catch {
        Write-Host "? Cannot write to database file: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

# ============================================================================
# SUMMARY
# ============================================================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary & Recommendations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (Test-Path $dbPath) {
    Write-Host "? Database file exists" -ForegroundColor Green
} else {
    Write-Host "? Database file missing" -ForegroundColor Red
    Write-Host "  Create it with: .\Troubleshoot-Database.ps1 -CreateDatabase" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Quick Fix Commands:" -ForegroundColor Yellow
Write-Host ""

Write-Host "Fix all permissions:" -ForegroundColor White
Write-Host "  .\Troubleshoot-Database.ps1 -FixPermissions" -ForegroundColor Cyan
Write-Host ""

Write-Host "Run database migrations:" -ForegroundColor White
Write-Host "  .\Troubleshoot-Database.ps1 -RunMigrations" -ForegroundColor Cyan
Write-Host ""

Write-Host "Create database and fix permissions:" -ForegroundColor White
Write-Host "  .\Troubleshoot-Database.ps1 -CreateDatabase -FixPermissions" -ForegroundColor Cyan
Write-Host ""

Write-Host "Manual permission fix:" -ForegroundColor White
Write-Host "  icacls '$DeployPath' /grant 'IIS AppPool\$AppPoolName`:(OI)(CI)M' /T" -ForegroundColor Cyan
Write-Host ""

Write-Host "Check application logs:" -ForegroundColor White
Write-Host "  Get-Content '$logsPath\application-*.log' -Tail 50" -ForegroundColor Cyan
Write-Host ""

Write-Host "Test application health:" -ForegroundColor White
Write-Host "  Invoke-WebRequest http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health" -ForegroundColor Cyan
Write-Host ""
