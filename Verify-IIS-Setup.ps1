# Kosovo Dogana - IIS Verification & Troubleshooting Guide
# ============================================================================
# Run these commands to verify and troubleshoot your IIS deployment
# ============================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "IIS Verification & Troubleshooting" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Import IIS module
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "? WebAdministration module loaded" -ForegroundColor Green
} catch {
    Write-Host "? ERROR: WebAdministration module not available" -ForegroundColor Red
    Write-Host "  Please ensure IIS is installed and management tools are enabled" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# ============================================================================
# 1. CHECK IIS SERVICE STATUS
# ============================================================================
Write-Host "[1] IIS Service Status" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    $w3svc = Get-Service W3SVC
    Write-Host "Service Name:    $($w3svc.Name)" -ForegroundColor White
    Write-Host "Display Name:    $($w3svc.DisplayName)" -ForegroundColor White
    Write-Host "Status:          $($w3svc.Status)" -ForegroundColor $(if ($w3svc.Status -eq 'Running') { 'Green' } else { 'Red' })
    Write-Host "Startup Type:    $($w3svc.StartType)" -ForegroundColor White
    
    if ($w3svc.Status -ne 'Running') {
        Write-Host ""
        Write-Host "? WARNING: IIS is not running!" -ForegroundColor Yellow
        Write-Host "Start it with: Start-Service W3SVC" -ForegroundColor Cyan
    }
} catch {
    Write-Host "? ERROR: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# ============================================================================
# 2. CHECK APPLICATION POOL STATUS
# ============================================================================
Write-Host "[2] Application Pool Status" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

$AppPoolName = "DoganaDosjaVleres"

try {
    $pool = Get-Item "IIS:\AppPools\$AppPoolName" -ErrorAction Stop
    $state = Get-WebAppPoolState -Name $AppPoolName
    
    Write-Host "Application Pool: $AppPoolName" -ForegroundColor White
    Write-Host "State:            $($state.Value)" -ForegroundColor $(if ($state.Value -eq 'Started') { 'Green' } else { 'Red' })
    Write-Host "Runtime Version:  $($pool.managedRuntimeVersion)" -ForegroundColor White
    Write-Host "Pipeline Mode:    $($pool.managedPipelineMode)" -ForegroundColor White
    Write-Host "Identity:         $($pool.processModel.identityType)" -ForegroundColor White
    Write-Host "Start Mode:       $($pool.startMode)" -ForegroundColor White
    Write-Host "Enable 32-bit:    $($pool.enable32BitAppOnWin64)" -ForegroundColor White
    
    if ($state.Value -ne 'Started') {
        Write-Host ""
        Write-Host "? WARNING: Application pool is not started!" -ForegroundColor Yellow
        Write-Host "Start it with: Start-WebAppPool -Name '$AppPoolName'" -ForegroundColor Cyan
    }
} catch {
    Write-Host "? ERROR: Application pool '$AppPoolName' not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Create it by running: .\Setup-IIS-Production.ps1" -ForegroundColor Cyan
}
Write-Host ""

# ============================================================================
# 3. CHECK WEB APPLICATION CONFIGURATION
# ============================================================================
Write-Host "[3] Web Application Configuration" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

$SiteName = "Default Web Site"
$AppName = "DoganaDosjaVleres"
$DeployPath = "C:\inetpub\wwwroot\DoganaDosjaVleres"

try {
    $app = Get-WebApplication -Site $SiteName -Name $AppName -ErrorAction Stop
    
    Write-Host "Application Name:  $($app.path)" -ForegroundColor White
    Write-Host "Physical Path:     $($app.physicalPath)" -ForegroundColor White
    Write-Host "Application Pool:  $($app.applicationPool)" -ForegroundColor White
    Write-Host "Enabled Protocols: $($app.enabledProtocols)" -ForegroundColor White
    
    # Check if physical path exists
    if (Test-Path $app.physicalPath) {
        Write-Host "? Physical path exists" -ForegroundColor Green
        
        # Check for required files
        $requiredFiles = @("web.config", "KosovaDoganaModerne.dll", "appsettings.json", "appsettings.Production.json")
        Write-Host ""
        Write-Host "Required Files:" -ForegroundColor Gray
        foreach ($file in $requiredFiles) {
            $filePath = Join-Path $app.physicalPath $file
            if (Test-Path $filePath) {
                Write-Host "  ? $file" -ForegroundColor Green
            } else {
                Write-Host "  ? $file (MISSING)" -ForegroundColor Red
            }
        }
        
        # Check for database file
        Write-Host ""
        Write-Host "Database Files:" -ForegroundColor Gray
        $dbPath = Join-Path $app.physicalPath "KosovaDoganaModerne.db"
        if (Test-Path $dbPath) {
            $dbSize = (Get-Item $dbPath).Length
            Write-Host "  ? KosovaDoganaModerne.db (Size: $([Math]::Round($dbSize/1MB, 2)) MB)" -ForegroundColor Green
            
            # Check database permissions
            $appPoolIdentity = "IIS AppPool\$AppPoolName"
            $acl = Get-Acl $dbPath
            $hasPermission = $acl.Access | Where-Object { 
                $_.IdentityReference -eq $appPoolIdentity -and 
                $_.FileSystemRights -match "Modify" 
            }
            
            if ($hasPermission) {
                Write-Host "    ? IIS AppPool has Modify permissions" -ForegroundColor Green
            } else {
                Write-Host "    ? IIS AppPool lacks Modify permissions!" -ForegroundColor Red
                Write-Host "    Fix with: icacls '$dbPath' /grant 'IIS AppPool\$AppPoolName`:(M)'" -ForegroundColor Cyan
            }
        } else {
            Write-Host "  ? KosovaDoganaModerne.db (NOT FOUND - will be created on first run)" -ForegroundColor Yellow
        }
        
        # Check logs directory
        Write-Host ""
        Write-Host "Logs Directory:" -ForegroundColor Gray
        $logsPath = Join-Path $app.physicalPath "logs"
        if (Test-Path $logsPath) {
            Write-Host "  ? logs directory exists" -ForegroundColor Green
            
            $logFiles = Get-ChildItem $logsPath -Filter "*.log" -ErrorAction SilentlyContinue
            if ($logFiles) {
                Write-Host "    Found $($logFiles.Count) log file(s)" -ForegroundColor White
                $latestLog = $logFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
                Write-Host "    Latest: $($latestLog.Name) ($(Get-Date $latestLog.LastWriteTime -Format 'yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
            }
            
            # Check logs directory permissions
            $acl = Get-Acl $logsPath
            $hasPermission = $acl.Access | Where-Object { 
                $_.IdentityReference -eq $appPoolIdentity -and 
                $_.FileSystemRights -match "Modify" 
            }
            
            if ($hasPermission) {
                Write-Host "    ? IIS AppPool has Modify permissions on logs" -ForegroundColor Green
            } else {
                Write-Host "    ? IIS AppPool lacks Modify permissions on logs!" -ForegroundColor Red
                Write-Host "    Fix with: icacls '$logsPath' /grant 'IIS AppPool\$AppPoolName`:(OI)(CI)M' /T" -ForegroundColor Cyan
            }
        } else {
            Write-Host "  ? logs directory NOT FOUND" -ForegroundColor Yellow
            Write-Host "    Create with: New-Item -ItemType Directory -Path '$logsPath' -Force" -ForegroundColor Cyan
        }
        
    } else {
        Write-Host "? Physical path does not exist!" -ForegroundColor Red
        Write-Host "  Deploy files to: $($app.physicalPath)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "? ERROR: Application '$AppName' not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Create it by running: .\Setup-IIS-Production.ps1" -ForegroundColor Cyan
}
Write-Host ""

# ============================================================================
# 4. CHECK SITE BINDINGS
# ============================================================================
Write-Host "[4] Site Bindings" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    $site = Get-Website -Name $SiteName -ErrorAction Stop
    $bindings = Get-WebBinding -Name $SiteName
    
    Write-Host "Site: $SiteName" -ForegroundColor White
    Write-Host "State: $($site.state)" -ForegroundColor $(if ($site.state -eq 'Started') { 'Green' } else { 'Red' })
    Write-Host ""
    Write-Host "Bindings:" -ForegroundColor White
    
    foreach ($binding in $bindings) {
        $protocol = $binding.protocol
        $bindingInfo = $binding.bindingInformation
        Write-Host "  $protocol`://$bindingInfo" -ForegroundColor Cyan
    }
    
    # Check if our expected binding exists
    $expectedIP = "10.10.173.154"
    $expectedPort = "80"
    $hasExpectedBinding = $bindings | Where-Object { 
        $_.protocol -eq "http" -and 
        $_.bindingInformation -like "*$expectedPort*" 
    }
    
    if ($hasExpectedBinding) {
        Write-Host ""
        Write-Host "? HTTP binding on port $expectedPort exists" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "? WARNING: No HTTP binding found on port $expectedPort" -ForegroundColor Yellow
        Write-Host "Add it with: New-WebBinding -Name '$SiteName' -Protocol http -Port $expectedPort -IPAddress '$expectedIP'" -ForegroundColor Cyan
    }
} catch {
    Write-Host "? ERROR: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# ============================================================================
# 5. TEST DATABASE CONNECTIVITY (via Diagnostic endpoint)
# ============================================================================
Write-Host "[5] Database Connectivity Test" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    $healthUrl = "http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health"
    Write-Host "Testing: $healthUrl" -ForegroundColor Gray
    
    $response = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    $health = $response.Content | ConvertFrom-Json
    
    Write-Host "Overall Status:      $($health.OverallStatus)" -ForegroundColor $(if ($health.OverallStatus -eq 'Healthy') { 'Green' } else { 'Red' })
    Write-Host "Database Connected:  $($health.DatabaseConnectivity.CanConnect)" -ForegroundColor $(if ($health.DatabaseConnectivity.CanConnect) { 'Green' } else { 'Red' })
    Write-Host "Database File:       $($health.DatabaseFile.Status)" -ForegroundColor $(if ($health.DatabaseFile.Status -eq 'Healthy') { 'Green' } else { 'Red' })
    Write-Host "Pending Migrations:  $($health.Migrations.PendingCount)" -ForegroundColor $(if ($health.Migrations.PendingCount -eq 0) { 'Green' } else { 'Yellow' })
    Write-Host "Product Count:       $($health.DataAccess.ProductCount)" -ForegroundColor White
    
} catch {
    Write-Host "? Cannot reach diagnostic endpoint" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "  This usually means the application is not running or has startup errors" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Check application logs:" -ForegroundColor Yellow
    Write-Host "  Get-Content '$DeployPath\logs\*.log' -Tail 50" -ForegroundColor Cyan
}
Write-Host ""

# ============================================================================
# 6. CHECK FIREWALL RULES
# ============================================================================
Write-Host "[6] Firewall Status" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    $firewallRule = Get-NetFirewallRule -DisplayName "World Wide Web Services (HTTP Traffic-In)" -ErrorAction SilentlyContinue
    
    if ($firewallRule) {
        Write-Host "Firewall Rule:    $($firewallRule.DisplayName)" -ForegroundColor White
        Write-Host "Enabled:          $($firewallRule.Enabled)" -ForegroundColor $(if ($firewallRule.Enabled) { 'Green' } else { 'Red' })
        Write-Host "Direction:        $($firewallRule.Direction)" -ForegroundColor White
        Write-Host "Action:           $($firewallRule.Action)" -ForegroundColor White
    } else {
        Write-Host "? WARNING: HTTP firewall rule not found" -ForegroundColor Yellow
        Write-Host "You may need to create a firewall rule for port $Port" -ForegroundColor Yellow
    }
} catch {
    Write-Host "? Could not check firewall: $($_.Exception.Message)" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# 7. CHECK EVENT LOGS
# ============================================================================
Write-Host "[7] Recent Event Log Errors" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    # Check Application logs for recent errors
    $appErrors = Get-EventLog -LogName Application -Source "IIS*" -EntryType Error -Newest 5 -ErrorAction SilentlyContinue
    
    if ($appErrors) {
        Write-Host "Recent IIS errors in Application log:" -ForegroundColor Red
        foreach ($error in $appErrors) {
            Write-Host "  [$($error.TimeGenerated)] $($error.Message.Substring(0, [Math]::Min(100, $error.Message.Length)))..." -ForegroundColor Yellow
        }
    } else {
        Write-Host "? No recent IIS errors in Application log" -ForegroundColor Green
    }
    
    Write-Host ""
    
    # Check System logs
    $sysErrors = Get-EventLog -LogName System -Source "W3SVC*" -EntryType Error -Newest 5 -ErrorAction SilentlyContinue
    
    if ($sysErrors) {
        Write-Host "Recent W3SVC errors in System log:" -ForegroundColor Red
        foreach ($error in $sysErrors) {
            Write-Host "  [$($error.TimeGenerated)] $($error.Message.Substring(0, [Math]::Min(100, $error.Message.Length)))..." -ForegroundColor Yellow
        }
    } else {
        Write-Host "? No recent W3SVC errors in System log" -ForegroundColor Green
    }
} catch {
    Write-Host "? Could not check event logs: $($_.Exception.Message)" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# 8. CHECK .NET HOSTING BUNDLE
# ============================================================================
Write-Host "[8] .NET Hosting Bundle" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    # Check if ASP.NET Core module is installed
    $aspNetCoreModule = Get-WebGlobalModule | Where-Object { $_.name -like "*AspNetCore*" }
    
    if ($aspNetCoreModule) {
        Write-Host "? ASP.NET Core Module installed:" -ForegroundColor Green
        foreach ($module in $aspNetCoreModule) {
            Write-Host "  - $($module.name)" -ForegroundColor White
        }
    } else {
        Write-Host "? ASP.NET Core Module NOT found!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Download and install .NET 10 Hosting Bundle:" -ForegroundColor Yellow
        Write-Host "  https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "After installing, run: iisreset" -ForegroundColor Cyan
    }
    
    # Check installed .NET runtimes
    Write-Host ""
    Write-Host "Installed .NET Runtimes:" -ForegroundColor White
    
    $runtimePath = "C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App"
    if (Test-Path $runtimePath) {
        $runtimes = Get-ChildItem $runtimePath | Select-Object -ExpandProperty Name | Sort-Object -Descending
        foreach ($runtime in $runtimes) {
            if ($runtime -like "10.*") {
                Write-Host "  ? $runtime" -ForegroundColor Green
            } else {
                Write-Host "  - $runtime" -ForegroundColor Gray
            }
        }
    } else {
        Write-Host "  ? .NET runtime path not found" -ForegroundColor Red
    }
} catch {
    Write-Host "? Could not check .NET hosting bundle: $($_.Exception.Message)" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# SUMMARY & QUICK ACTIONS
# ============================================================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Quick Action Commands" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Restart IIS:" -ForegroundColor Yellow
Write-Host "  iisreset" -ForegroundColor Cyan
Write-Host ""

Write-Host "Restart Application Pool:" -ForegroundColor Yellow
Write-Host "  Restart-WebAppPool -Name 'DoganaDosjaVleres'" -ForegroundColor Cyan
Write-Host ""

Write-Host "View Application Logs:" -ForegroundColor Yellow
Write-Host "  Get-Content 'C:\inetpub\wwwroot\DoganaDosjaVleres\logs\application-*.log' -Tail 50" -ForegroundColor Cyan
Write-Host ""

Write-Host "View Error Logs:" -ForegroundColor Yellow
Write-Host "  Get-Content 'C:\inetpub\wwwroot\DoganaDosjaVleres\logs\errors-*.log' -Tail 50" -ForegroundColor Cyan
Write-Host ""

Write-Host "Fix Database Permissions:" -ForegroundColor Yellow
Write-Host "  icacls 'C:\inetpub\wwwroot\DoganaDosjaVleres\KosovaDoganaModerne.db' /grant 'IIS AppPool\DoganaDosjaVleres:(M)'" -ForegroundColor Cyan
Write-Host ""

Write-Host "Fix Directory Permissions:" -ForegroundColor Yellow
Write-Host "  icacls 'C:\inetpub\wwwroot\DoganaDosjaVleres' /grant 'IIS AppPool\DoganaDosjaVleres:(OI)(CI)M' /T" -ForegroundColor Cyan
Write-Host ""

Write-Host "Test Endpoints:" -ForegroundColor Yellow
Write-Host "  Application: http://10.10.173.154/DoganaDosjaVleres" -ForegroundColor Cyan
Write-Host "  Health:      http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health" -ForegroundColor Cyan
Write-Host "  System Info: http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Index" -ForegroundColor Cyan
Write-Host "  View Logs:   http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Logs" -ForegroundColor Cyan
Write-Host ""

Write-Host "Deploy Application:" -ForegroundColor Yellow
Write-Host "  .\Deploy-ToIIS.ps1" -ForegroundColor Cyan
Write-Host ""
