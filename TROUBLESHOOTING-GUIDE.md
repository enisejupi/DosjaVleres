# Kosovo Dogana - Production Troubleshooting Guide
# ============================================================================
# Quick Reference for Diagnosing and Fixing Production Issues
# ============================================================================

## ?? IMMEDIATE ACTIONS FOR ERRORS

### 1. Check Application Health
Open in browser:
```
http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health
```

This endpoint shows:
- Database connectivity status
- Database file existence and permissions
- Pending migrations
- Overall application health

### 2. View Recent Logs
Open in browser:
```
http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Logs
http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Logs?type=error&lines=100
```

Or via PowerShell:
```powershell
# Application logs
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\application-*.log -Tail 50

# Error logs only
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\errors-*.log -Tail 50

# Startup logs
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\startup-*.log -Tail 50

# IIS stdout logs
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\stdout*.log -Tail 50
```

### 3. View System Information
Open in browser:
```
http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Index
```

Shows:
- PathBase configuration
- Current environment
- Connection string
- File paths

---

## ?? COMMON ISSUES & FIXES

### Issue 1: HTTP 404 - Page Not Found

**Symptom**: All pages return 404 errors

**Causes**:
- PathBase misconfiguration
- Middleware ordering issue
- IIS virtual directory not created

**Fix**:
```powershell
# 1. Verify IIS setup
.\Verify-IIS-Setup.ps1

# 2. Check PathBase in appsettings.Production.json
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\appsettings.Production.json | Select-String "PathBase"
# Should show: "PathBase": "/DoganaDosjaVleres"

# 3. Restart app pool
Restart-WebAppPool -Name DoganaDosjaVleres
```

### Issue 2: Database Errors / No Data Showing

**Symptom**: Application loads but no tables or data appear

**Causes**:
- Database file missing
- Database file permissions incorrect
- Pending migrations not applied

**Fix**:
```powershell
# Run comprehensive database troubleshooting
.\Troubleshoot-Database.ps1 -FixPermissions

# If database doesn't exist, create it
.\Troubleshoot-Database.ps1 -CreateDatabase -FixPermissions

# Check database via API
Invoke-WebRequest http://10.10.173.154/DoganaDosjaVleres/Diagnostic/TestDatabase
```

### Issue 3: Application Won't Start / Generic Error Page

**Symptom**: Shows "An error occurred while processing your request"

**Causes**:
- Startup error in Program.cs
- Missing dependencies
- Database initialization failure

**Fix**:
```powershell
# 1. Check startup logs
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\startup-*.log -Tail 100

# 2. Check stdout logs
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\stdout*.log -Tail 100

# 3. Check error logs
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\errors-*.log -Tail 100

# 4. Restart app pool
Restart-WebAppPool -Name DoganaDosjaVleres

# 5. Check IIS event logs
Get-EventLog -LogName Application -Source "IIS*" -EntryType Error -Newest 10
```

### Issue 4: HTTPS Not Working

**Symptom**: HTTPS connections fail or redirect to HTTP

**Causes**:
- No SSL certificate in IIS
- HTTPS binding not configured
- HTTPS redirection disabled

**Fix**:
```powershell
# 1. Check IIS bindings
Get-WebBinding -Name "Default Web Site"

# 2. Add HTTPS binding (if SSL certificate installed)
New-WebBinding -Name "Default Web Site" -Protocol https -Port 443 -IPAddress 10.10.173.154

# 3. Enable HTTPS redirection in appsettings.Production.json
# Change "UseHttpsRedirection": false to true

# 4. Restart app pool
Restart-WebAppPool -Name DoganaDosjaVleres
```

### Issue 5: Permission Denied Errors

**Symptom**: Logs show "Access denied" or "Permission denied"

**Causes**:
- IIS AppPool lacks file system permissions
- Database file not accessible
- Logs directory not writable

**Fix**:
```powershell
# Fix all permissions automatically
.\Troubleshoot-Database.ps1 -FixPermissions

# Or manually:
icacls "C:\inetpub\wwwroot\DoganaDosjaVleres" /grant "IIS AppPool\DoganaDosjaVleres:(OI)(CI)M" /T

# For database specifically:
icacls "C:\inetpub\wwwroot\DoganaDosjaVleres\KosovaDoganaModerne.db" /grant "IIS AppPool\DoganaDosjaVleres:(M)"

# For logs directory:
icacls "C:\inetpub\wwwroot\DoganaDosjaVleres\logs" /grant "IIS AppPool\DoganaDosjaVleres:(OI)(CI)M" /T
```

---

## ?? DEPLOYMENT SCRIPTS

### Full Setup (First Time)
```powershell
# Run as Administrator
.\Setup-IIS-Production.ps1
```

Creates:
- IIS Application Pool
- IIS Virtual Directory
- Configures all settings

### Deploy Application
```powershell
# Build and deploy
dotnet publish -c Release
.\Deploy-ToIIS.ps1
```

Or with MSBuild:
```powershell
dotnet build -c Release /p:DeployOnBuild=true
```

### Verify Deployment
```powershell
# Check all IIS configuration
.\Verify-IIS-Setup.ps1

# Check database specifically
.\Troubleshoot-Database.ps1
```

---

## ?? DIAGNOSTIC ENDPOINTS

All diagnostic endpoints are available at:
- **Base**: `http://10.10.173.154/DoganaDosjaVleres/Diagnostic/`

### Available Endpoints:

1. **Health Check** (JSON)
   ```
   /Diagnostic/Health
   ```
   Shows: Database connectivity, file existence, migrations, overall status

2. **System Information** (JSON)
   ```
   /Diagnostic/Index
   ```
   Shows: Configuration, paths, environment, routing info

3. **Database Test** (JSON)
   ```
   /Diagnostic/TestDatabase
   ```
   Shows: Database connection status, sample products

4. **Routes** (JSON)
   ```
   /Diagnostic/Routes
   ```
   Shows: All registered routes with full URLs

5. **View Logs** (JSON)
   ```
   /Diagnostic/Logs?type=application&lines=100
   /Diagnostic/Logs?type=error&lines=50
   /Diagnostic/Logs?type=startup&lines=50
   ```
   Shows: Recent log entries

6. **Test Specific Product** (JSON)
   ```
   /Diagnostic/TestHistoria/1
   ```
   Shows: Whether product ID exists and has history

---

## ?? POWERSHELL QUICK COMMANDS

### Check Application Status
```powershell
# App pool status
Get-WebAppPoolState -Name DoganaDosjaVleres

# IIS service status
Get-Service W3SVC

# Test network connectivity
Test-NetConnection -ComputerName 10.10.173.154 -Port 80
```

### Restart Services
```powershell
# Restart app pool only (recommended)
Restart-WebAppPool -Name DoganaDosjaVleres

# Restart entire IIS (if necessary)
iisreset
```

### Check Logs
```powershell
# Latest 50 lines from application log
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\application-*.log -Tail 50 -Wait

# Latest 50 lines from error log
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\errors-*.log -Tail 50 -Wait

# Monitor logs in real-time (live tail)
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\application-*.log -Wait -Tail 10
```

### Test Endpoints
```powershell
# Test home page
Invoke-WebRequest http://10.10.173.154/DoganaDosjaVleres

# Test health endpoint
$health = Invoke-WebRequest http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health | ConvertFrom-Json
$health | ConvertTo-Json -Depth 10

# Test database
Invoke-WebRequest http://10.10.173.154/DoganaDosjaVleres/Diagnostic/TestDatabase
```

---

## ?? LOG FILE LOCATIONS

All logs are in: `C:\inetpub\wwwroot\DoganaDosjaVleres\logs\`

- `application-YYYYMMDD.log` - General application logs (Info level)
- `errors-YYYYMMDD.log` - Error-only logs (Error level)
- `startup-YYYYMMDD.log` - Application startup logs
- `stdout-YYYYMMDD.log` - IIS stdout capture (ASP.NET Core Module)

Logs are automatically:
- Rotated daily
- Limited to 50MB per file
- Retained for 30 days (application) or 90 days (errors)

---

## ?? TROUBLESHOOTING WORKFLOW

### Step 1: Identify the Issue
1. Try accessing: `http://10.10.173.154/DoganaDosjaVleres`
2. Note the exact error or behavior
3. Check health endpoint: `http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health`

### Step 2: Check Logs
```powershell
# View recent errors
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\errors-*.log -Tail 50

# View startup issues
Get-Content C:\inetpub\wwwroot\DoganaDosjaVleres\logs\startup-*.log -Tail 50
```

### Step 3: Verify Configuration
```powershell
# Run verification script
.\Verify-IIS-Setup.ps1
```

### Step 4: Check Database
```powershell
# Run database troubleshooting
.\Troubleshoot-Database.ps1
```

### Step 5: Fix Issues
Based on findings:
```powershell
# Fix permissions
.\Troubleshoot-Database.ps1 -FixPermissions

# Create/update database
.\Troubleshoot-Database.ps1 -CreateDatabase -FixPermissions

# Redeploy if needed
.\Deploy-ToIIS.ps1
```

### Step 6: Restart and Test
```powershell
# Restart app pool
Restart-WebAppPool -Name DoganaDosjaVleres

# Wait a few seconds
Start-Sleep -Seconds 5

# Test health
Invoke-WebRequest http://10.10.173.154/DoganaDosjaVleres/Diagnostic/Health
```

---

## ?? SUPPORT INFORMATION

### Key Configuration Files:
- `appsettings.Production.json` - Production configuration (PathBase, connection string, logging)
- `web.config` - IIS/ASP.NET Core Module configuration
- `Program.cs` - Application startup and middleware configuration

### Key Settings:
- **PathBase**: `/DoganaDosjaVleres` (must match IIS virtual directory)
- **Connection String**: `Data Source=KosovaDoganaModerne.db` (relative to application root)
- **App Pool**: `DoganaDosjaVleres` (No Managed Code, AlwaysRunning)
- **Physical Path**: `C:\inetpub\wwwroot\DoganaDosjaVleres`

### Environment Variables:
- `ASPNETCORE_ENVIRONMENT=Production` (set in web.config)
- `ASPNETCORE_DETAILEDERRORS=true` (temporarily enabled for debugging)

---

## ?? PRODUCTION NOTES

### Current Configuration:
- **Detailed errors are ENABLED in Production** (for debugging)
- This is controlled by `ShowDetailedErrors: true` in `appsettings.Production.json`
- **DISABLE THIS** after resolving issues by setting to `false`

### Security:
- Detailed errors should not be shown in production
- After fixing all issues, set `ShowDetailedErrors: false` in `appsettings.Production.json`
- Diagnostic endpoints should be restricted or removed in final production

---

Last Updated: 2025-12-01
Version: 1.0
