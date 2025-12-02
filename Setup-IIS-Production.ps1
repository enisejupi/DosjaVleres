# ============================================================================
# Kosovo Dogana IIS Production Setup Script
# ============================================================================
# This script configures IIS for production deployment of the Kosovo Dogana
# application at http://10.10.173.154/DoganaDosjaVleres
# ============================================================================

<#
.SYNOPSIS
    Configures IIS for Kosovo Dogana production deployment
.DESCRIPTION
    This script performs comprehensive checks and configuration for IIS,
    including prerequisite validation, automatic feature installation,
    and complete application setup.
.NOTES
    Requires Administrator privileges
    Windows Server 2016+ or Windows 10+ with IIS features
#>

# Stop on any error
$ErrorActionPreference = "Stop"

# ============================================================================
# PREREQUISITE VALIDATION FUNCTIONS
# ============================================================================

function Test-Administrator {
    <#
    .SYNOPSIS
        Checks if script is running with Administrator privileges
    #>
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Test-IISInstalled {
    <#
    .SYNOPSIS
        Checks if IIS is installed on the system
    #>
    try {
        $iisService = Get-Service -Name W3SVC -ErrorAction SilentlyContinue
        return ($null -ne $iisService)
    } catch {
        return $false
    }
}

function Test-WindowsFeature {
    <#
    .SYNOPSIS
        Checks if a Windows feature is installed
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureName
    )
    
    try {
        # Try Server method first (Windows Server)
        $feature = Get-WindowsFeature -Name $FeatureName -ErrorAction SilentlyContinue
        if ($null -ne $feature) {
            return $feature.Installed
        }
        
        # Try Desktop method (Windows 10/11)
        $feature = Get-WindowsOptionalFeature -Online -FeatureName $FeatureName -ErrorAction SilentlyContinue
        if ($null -ne $feature) {
            return ($feature.State -eq "Enabled")
        }
        
        return $false
    } catch {
        return $false
    }
}

function Get-OSType {
    <#
    .SYNOPSIS
        Determines if running on Windows Server or Desktop
    #>
    $os = Get-CimInstance -ClassName Win32_OperatingSystem
    if ($os.ProductType -eq 1) {
        return "Desktop" # Windows 10/11
    } else {
        return "Server"  # Windows Server
    }
}

function Install-IISFeatures {
    <#
    .SYNOPSIS
        Installs required IIS features based on OS type
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$OSType
    )
    
    Write-Host ""
    Write-Host "Installing IIS features..." -ForegroundColor Yellow
    Write-Host "This may take several minutes. Please wait..." -ForegroundColor Yellow
    Write-Host ""
    
    try {
        if ($OSType -eq "Server") {
            # Windows Server installation
            Write-Host "  Installing IIS on Windows Server..." -ForegroundColor White
            
            $features = @(
                "Web-Server",
                "Web-WebServer",
                "Web-Common-Http",
                "Web-Default-Doc",
                "Web-Dir-Browsing",
                "Web-Http-Errors",
                "Web-Static-Content",
                "Web-Health",
                "Web-Http-Logging",
                "Web-Performance",
                "Web-Stat-Compression",
                "Web-Security",
                "Web-Filtering",
                "Web-App-Dev",
                "Web-Net-Ext45",
                "Web-Asp-Net45",
                "Web-ISAPI-Ext",
                "Web-ISAPI-Filter",
                "Web-Mgmt-Tools",
                "Web-Mgmt-Console",
                "Web-Scripting-Tools"
            )
            
            foreach ($feature in $features) {
                Write-Host "    Installing $feature..." -ForegroundColor Gray
                Install-WindowsFeature -Name $feature -IncludeManagementTools -ErrorAction SilentlyContinue | Out-Null
            }
            
        } else {
            # Windows Desktop installation
            Write-Host "  Installing IIS on Windows Desktop..." -ForegroundColor White
            
            $features = @(
                "IIS-WebServerRole",
                "IIS-WebServer",
                "IIS-CommonHttpFeatures",
                "IIS-HttpErrors",
                "IIS-ApplicationDevelopment",
                "IIS-NetFxExtensibility45",
                "IIS-HealthAndDiagnostics",
                "IIS-HttpLogging",
                "IIS-Security",
                "IIS-RequestFiltering",
                "IIS-Performance",
                "IIS-HttpCompressionStatic",
                "IIS-WebServerManagementTools",
                "IIS-ManagementConsole",
                "IIS-ManagementScriptingTools",
                "IIS-StaticContent",
                "IIS-DefaultDocument",
                "IIS-DirectoryBrowsing",
                "IIS-ISAPIExtensions",
                "IIS-ISAPIFilter",
                "IIS-ASPNET45"
            )
            
            foreach ($feature in $features) {
                Write-Host "    Enabling $feature..." -ForegroundColor Gray
                Enable-WindowsOptionalFeature -Online -FeatureName $feature -All -NoRestart -ErrorAction SilentlyContinue | Out-Null
            }
        }
        
        Write-Host ""
        Write-Host "  ? IIS features installed successfully" -ForegroundColor Green
        Write-Host ""
        
        # Start IIS service
        Write-Host "  Starting IIS service..." -ForegroundColor White
        Start-Service W3SVC -ErrorAction Stop
        Set-Service W3SVC -StartupType Automatic
        
        Write-Host "  ? IIS service started" -ForegroundColor Green
        Write-Host ""
        
        return $true
        
    } catch {
        Write-Host ""
        Write-Host "  ? ERROR installing IIS features: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return $false
    }
}

function Test-WebAdministrationModule {
    <#
    .SYNOPSIS
        Checks if WebAdministration module can be loaded
    #>
    try {
        Import-Module WebAdministration -ErrorAction Stop
        
        # Verify IIS drive is accessible
        if (Test-Path "IIS:\") {
            return $true
        }
        return $false
    } catch {
        return $false
    }
}

# ============================================================================
# MAIN SCRIPT START
# ============================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Kosovo Dogana IIS Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# PREREQUISITE CHECK 1: Administrator Privileges
# ============================================================================
Write-Host "[Prerequisite 1/4] Checking Administrator privileges..." -ForegroundColor Yellow

if (-not (Test-Administrator)) {
    Write-Host "  ? ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Press any key to exit..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "  ? Running with Administrator privileges" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PREREQUISITE CHECK 2: Operating System Type
# ============================================================================
Write-Host "[Prerequisite 2/4] Detecting operating system..." -ForegroundColor Yellow

$OSType = Get-OSType
$osInfo = Get-CimInstance -ClassName Win32_OperatingSystem

Write-Host "  OS: $($osInfo.Caption)" -ForegroundColor White
Write-Host "  Type: $OSType" -ForegroundColor White
Write-Host "  Version: $($osInfo.Version)" -ForegroundColor White
Write-Host "  ? Operating system detected" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PREREQUISITE CHECK 3: IIS Installation
# ============================================================================
Write-Host "[Prerequisite 3/4] Checking IIS installation..." -ForegroundColor Yellow

$iisInstalled = Test-IISInstalled

if (-not $iisInstalled) {
    Write-Host "  ? IIS is not installed on this system" -ForegroundColor Red
    Write-Host ""
    Write-Host "IIS (Internet Information Services) is required to run this application." -ForegroundColor Yellow
    Write-Host ""
    
    # Ask user if they want to install IIS
    $response = Read-Host "Would you like to install IIS now? (Y/N)"
    
    if ($response -eq "Y" -or $response -eq "y") {
        $installResult = Install-IISFeatures -OSType $OSType
        
        if (-not $installResult) {
            Write-Host ""
            Write-Host "Failed to install IIS. Please install manually:" -ForegroundColor Red
            Write-Host ""
            if ($OSType -eq "Server") {
                Write-Host "  Install-WindowsFeature -Name Web-Server -IncludeManagementTools" -ForegroundColor Cyan
            } else {
                Write-Host "  Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All" -ForegroundColor Cyan
            }
            Write-Host ""
            Write-Host "Press any key to exit..." -ForegroundColor Gray
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
            exit 1
        }
        
        # Verify IIS is now installed
        $iisInstalled = Test-IISInstalled
        if (-not $iisInstalled) {
            Write-Host "  ? IIS installation verification failed" -ForegroundColor Red
            Write-Host ""
            Write-Host "Please restart PowerShell and run this script again." -ForegroundColor Yellow
            Write-Host ""
            Write-Host "Press any key to exit..." -ForegroundColor Gray
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
            exit 1
        }
        
    } else {
        Write-Host ""
        Write-Host "IIS installation cancelled by user." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Please install IIS manually before running this script:" -ForegroundColor Yellow
        Write-Host ""
        if ($OSType -eq "Server") {
            Write-Host "  1. Open Server Manager" -ForegroundColor White
            Write-Host "  2. Click 'Add Roles and Features'" -ForegroundColor White
            Write-Host "  3. Select 'Web Server (IIS)'" -ForegroundColor White
            Write-Host "  4. Complete the installation" -ForegroundColor White
            Write-Host ""
            Write-Host "Or use PowerShell:" -ForegroundColor White
            Write-Host "  Install-WindowsFeature -Name Web-Server -IncludeManagementTools" -ForegroundColor Cyan
        } else {
            Write-Host "  1. Open Control Panel" -ForegroundColor White
            Write-Host "  2. Go to 'Programs and Features'" -ForegroundColor White
            Write-Host "  3. Click 'Turn Windows features on or off'" -ForegroundColor White
            Write-Host "  4. Check 'Internet Information Services'" -ForegroundColor White
            Write-Host "  5. Expand and check required features" -ForegroundColor White
            Write-Host "  6. Click OK to install" -ForegroundColor White
            Write-Host ""
            Write-Host "Or use PowerShell:" -ForegroundColor White
            Write-Host "  Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All" -ForegroundColor Cyan
        }
        Write-Host ""
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
}

Write-Host "  ? IIS is installed" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PREREQUISITE CHECK 4: WebAdministration Module
# ============================================================================
Write-Host "[Prerequisite 4/4] Loading WebAdministration module..." -ForegroundColor Yellow

$moduleLoaded = Test-WebAdministrationModule

if (-not $moduleLoaded) {
    Write-Host "  ? ERROR: WebAdministration module could not be loaded" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "  1. Ensure IIS Management Scripts and Tools are installed" -ForegroundColor White
    Write-Host "  2. Restart PowerShell as Administrator" -ForegroundColor White
    Write-Host "  3. Try manual import: Import-Module WebAdministration" -ForegroundColor White
    Write-Host ""
    
    if ($OSType -eq "Server") {
        Write-Host "Install management tools with:" -ForegroundColor Yellow
        Write-Host "  Install-WindowsFeature -Name Web-Scripting-Tools" -ForegroundColor Cyan
    } else {
        Write-Host "Enable management tools with:" -ForegroundColor Yellow
        Write-Host "  Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementScriptingTools" -ForegroundColor Cyan
    }
    Write-Host ""
    Write-Host "Press any key to exit..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "  ? WebAdministration module loaded successfully" -ForegroundColor Green
Write-Host "  ? IIS:\\ PowerShell drive is accessible" -ForegroundColor Green
Write-Host ""

# ============================================================================
# All prerequisites met - proceed with configuration
# ============================================================================

Write-Host "========================================" -ForegroundColor Green
Write-Host "All prerequisites met!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Configuration Variables
$AppPoolName = "DoganaDosjaVleres"
$SiteName = "Default Web Site"
$AppName = "DoganaDosjaVleres"
$PhysicalPath = "C:\inetpub\wwwroot\DoganaDosjaVleres"
$IPAddress = "10.10.173.154"
$HttpPort = 80
$HttpsPort = 443

Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Application Pool: $AppPoolName" -ForegroundColor White
Write-Host "  Site: $SiteName" -ForegroundColor White
Write-Host "  Application: $AppName" -ForegroundColor White
Write-Host "  Physical Path: $PhysicalPath" -ForegroundColor White
Write-Host "  IP Address: $IPAddress" -ForegroundColor White
Write-Host "  HTTP Port: $HttpPort" -ForegroundColor White
Write-Host ""

# ============================================================================
# STEP 1: Create Application Pool
# ============================================================================
Write-Host "[Step 1/5] Creating Application Pool..." -ForegroundColor Green

try {
    # Check if app pool exists
    $existingPool = Get-Item "IIS:\AppPools\$AppPoolName" -ErrorAction SilentlyContinue
    
    if ($existingPool) {
        Write-Host "  Application pool '$AppPoolName' already exists" -ForegroundColor Yellow
        Write-Host "  Stopping existing pool..." -ForegroundColor Yellow
        
        $poolState = Get-WebAppPoolState -Name $AppPoolName
        if ($poolState.Value -ne "Stopped") {
            Stop-WebAppPool -Name $AppPoolName -ErrorAction Stop
            Start-Sleep -Seconds 2
            Write-Host "  ? Existing pool stopped" -ForegroundColor Green
        }
    } else {
        Write-Host "  Creating new application pool..." -ForegroundColor White
        New-WebAppPool -Name $AppPoolName -Force | Out-Null
        Write-Host "  ? Application pool created" -ForegroundColor Green
    }
    
    # Configure application pool settings
    Write-Host "  Configuring application pool settings..." -ForegroundColor White
    
    # No Managed Code (for .NET Core/10)
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "managedRuntimeVersion" -Value ""
    
    # Integrated pipeline mode
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "managedPipelineMode" -Value "Integrated"
    
    # Always running (don't shut down when idle)
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "startMode" -Value "AlwaysRunning"
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "processModel.idleTimeout" -Value ([TimeSpan]::FromMinutes(0))
    
    # Application Pool Identity (most secure)
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
    
    # 32-bit applications (usually false for .NET Core)
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "enable32BitAppOnWin64" -Value $false
    
    # Recycling settings
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "recycling.periodicRestart.time" -Value ([TimeSpan]::FromHours(0))
    
    Write-Host "  ? Application pool configured successfully" -ForegroundColor Green
    
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Verify IIS is running: Get-Service W3SVC" -ForegroundColor White
    Write-Host "  - Check IIS AppPools path: Test-Path 'IIS:\AppPools'" -ForegroundColor White
    Write-Host "  - Try restarting IIS: iisreset" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to exit..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host ""

# ============================================================================
# STEP 2: Create Physical Directory and Set Permissions
# ============================================================================
Write-Host "[Step 2/5] Setting up physical directory..." -ForegroundColor Green

try {
    # Create directory if it doesn't exist
    if (-not (Test-Path $PhysicalPath)) {
        Write-Host "  Creating directory: $PhysicalPath" -ForegroundColor White
        New-Item -ItemType Directory -Path $PhysicalPath -Force | Out-Null
        Write-Host "  ? Directory created" -ForegroundColor Green
    } else {
        Write-Host "  Directory already exists: $PhysicalPath" -ForegroundColor Yellow
    }
    
    # Set permissions for IIS App Pool Identity
    Write-Host "  Setting directory permissions..." -ForegroundColor White
    
    $identity = "IIS AppPool\$AppPoolName"
    $acl = Get-Acl $PhysicalPath
    
    # Check if permission already exists
    $existingRule = $acl.Access | Where-Object { $_.IdentityReference -eq $identity }
    
    if ($existingRule) {
        Write-Host "  Permissions already exist for $identity" -ForegroundColor Yellow
    } else {
        $fileSystemRights = [System.Security.AccessControl.FileSystemRights]::Modify
        $inheritanceFlags = [System.Security.AccessControl.InheritanceFlags]::ContainerInherit -bor [System.Security.AccessControl.InheritanceFlags]::ObjectInherit
        $propagationFlags = [System.Security.AccessControl.PropagationFlags]::None
        $accessControlType = [System.Security.AccessControl.AccessControlType]::Allow
        $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($identity, $fileSystemRights, $inheritanceFlags, $propagationFlags, $accessControlType)
        
        $acl.SetAccessRule($accessRule)
        Set-Acl $PhysicalPath $acl
        
        Write-Host "  ? Permissions granted to $identity" -ForegroundColor Green
    }
    
    Write-Host "  ? Directory setup complete" -ForegroundColor Green
    
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Verify you have permissions to create directories in C:\inetpub\wwwroot" -ForegroundColor White
    Write-Host "  - Check if directory is locked by another process" -ForegroundColor White
    Write-Host "  - Ensure the application pool exists: Get-WebAppPool -Name '$AppPoolName'" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to exit..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host ""

# ============================================================================
# STEP 3: Create Virtual Directory/Application
# ============================================================================
Write-Host "[Step 3/5] Creating Web Application..." -ForegroundColor Green

try {
    # Verify Default Web Site exists
    $site = Get-Website -Name $SiteName -ErrorAction SilentlyContinue
    if (-not $site) {
        Write-Host "  ? ERROR: '$SiteName' does not exist" -ForegroundColor Red
        Write-Host ""
        Write-Host "Available sites:" -ForegroundColor Yellow
        Get-Website | Format-Table Name, State, PhysicalPath
        Write-Host ""
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
    
    # Check if application exists
    $existingApp = Get-WebApplication -Site $SiteName -Name $AppName -ErrorAction SilentlyContinue
    
    if ($existingApp) {
        Write-Host "  Application '$AppName' already exists" -ForegroundColor Yellow
        Write-Host "  Removing existing application..." -ForegroundColor Yellow
        Remove-WebApplication -Site $SiteName -Name $AppName
        Write-Host "  ? Existing application removed" -ForegroundColor Green
    }
    
    Write-Host "  Creating new web application..." -ForegroundColor White
    New-WebApplication -Name $AppName `
                       -Site $SiteName `
                       -PhysicalPath $PhysicalPath `
                       -ApplicationPool $AppPoolName `
                       -Force | Out-Null
    
    # Verify application was created
    $verifyApp = Get-WebApplication -Site $SiteName -Name $AppName
    if ($verifyApp) {
        Write-Host "  ? Web application created successfully" -ForegroundColor Green
        Write-Host "    Path: /$AppName" -ForegroundColor Gray
        Write-Host "    Physical Path: $PhysicalPath" -ForegroundColor Gray
        Write-Host "    Application Pool: $AppPoolName" -ForegroundColor Gray
    } else {
        throw "Application creation verification failed"
    }
    
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Verify 'Default Web Site' exists: Get-Website" -ForegroundColor White
    Write-Host "  - Check if application path is unique" -ForegroundColor White
    Write-Host "  - Ensure physical path exists: Test-Path '$PhysicalPath'" -ForegroundColor White
    Write-Host "  - Verify app pool exists: Get-WebAppPool -Name '$AppPoolName'" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to exit..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host ""

# ============================================================================
# STEP 4: Configure Site Bindings
# ============================================================================
Write-Host "[Step 4/5] Configuring site bindings..." -ForegroundColor Green

try {
    # Get all current bindings
    $currentBindings = Get-WebBinding -Name $SiteName
    
    # Check HTTP binding
    $httpBinding = $currentBindings | Where-Object { 
        $_.protocol -eq "http" -and 
        $_.bindingInformation -like "*:$HttpPort`:*" 
    }
    
    if (-not $httpBinding) {
        Write-Host "  Adding HTTP binding on $IPAddress`:$HttpPort..." -ForegroundColor White
        
        try {
            New-WebBinding -Name $SiteName -Protocol "http" -Port $HttpPort -IPAddress $IPAddress | Out-Null
            Write-Host "  ? HTTP binding added" -ForegroundColor Green
        } catch {
            # If specific IP fails, try with all IPs
            Write-Host "  Could not bind to specific IP, trying with all IPs (*:$HttpPort)..." -ForegroundColor Yellow
            New-WebBinding -Name $SiteName -Protocol "http" -Port $HttpPort -IPAddress "*" | Out-Null
            Write-Host "  ? HTTP binding added (all IPs)" -ForegroundColor Green
        }
    } else {
        Write-Host "  HTTP binding already exists on port $HttpPort" -ForegroundColor Yellow
        Write-Host "    Binding: $($httpBinding.bindingInformation)" -ForegroundColor Gray
    }
    
    # Display all current bindings
    Write-Host "  Current site bindings:" -ForegroundColor Gray
    $currentBindings | ForEach-Object {
        Write-Host "    $($_.protocol)://$($_.bindingInformation)" -ForegroundColor DarkGray
    }
    
    # Optional: HTTPS binding (commented out - uncomment if you have SSL certificate)
    <#
    Write-Host "  HTTPS binding configuration is disabled" -ForegroundColor Gray
    Write-Host "  To enable HTTPS:" -ForegroundColor Gray
    Write-Host "    1. Obtain an SSL certificate" -ForegroundColor DarkGray
    Write-Host "    2. Import certificate to IIS" -ForegroundColor DarkGray
    Write-Host "    3. Uncomment HTTPS section in this script" -ForegroundColor DarkGray
    
    $httpsBinding = Get-WebBinding -Name $SiteName -Protocol "https" -Port $HttpsPort -IPAddress $IPAddress -ErrorAction SilentlyContinue
    if (-not $httpsBinding) {
        Write-Host "  Adding HTTPS binding on port $HttpsPort..." -ForegroundColor White
        New-WebBinding -Name $SiteName -Protocol "https" -Port $HttpsPort -IPAddress $IPAddress -SslFlags 0
        Write-Host "  ? HTTPS binding added (you need to bind SSL certificate manually)" -ForegroundColor Green
    }
    #>
    
    Write-Host "  ? Binding configuration complete" -ForegroundColor Green
    
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Check if port $HttpPort is already in use: netstat -ano | findstr :$HttpPort" -ForegroundColor White
    Write-Host "  - Verify IP address $IPAddress is valid on this machine: ipconfig" -ForegroundColor White
    Write-Host "  - Check existing bindings: Get-WebBinding -Name '$SiteName'" -ForegroundColor White
    Write-Host ""
    Write-Host "Note: The application may still work on existing bindings" -ForegroundColor Yellow
    Write-Host ""
}

Write-Host ""

# ============================================================================
# STEP 5: Start Application Pool and Verify
# ============================================================================
Write-Host "[Step 5/5] Starting Application Pool..." -ForegroundColor Green

try {
    $poolState = Get-WebAppPoolState -Name $AppPoolName
    
    if ($poolState.Value -ne "Started") {
        Start-WebAppPool -Name $AppPoolName -ErrorAction Stop
        Start-Sleep -Seconds 2
        
        $poolState = Get-WebAppPoolState -Name $AppPoolName
    }
    
    if ($poolState.Value -eq "Started") {
        Write-Host "  ? Application pool started successfully" -ForegroundColor Green
        Write-Host "    State: $($poolState.Value)" -ForegroundColor Gray
    } else {
        Write-Host "  ? Application pool state: $($poolState.Value)" -ForegroundColor Yellow
        Write-Host "  Attempting to start..." -ForegroundColor Yellow
        Start-WebAppPool -Name $AppPoolName -ErrorAction Stop
        Start-Sleep -Seconds 2
        Write-Host "  ? Application pool started" -ForegroundColor Green
    }
    
} catch {
    Write-Host "  ? ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Check Event Viewer for IIS errors" -ForegroundColor White
    Write-Host "  - Verify .NET hosting bundle is installed" -ForegroundColor White
    Write-Host "  - Try manual start: Start-WebAppPool -Name '$AppPoolName'" -ForegroundColor White
    Write-Host "  - Check application pool settings: Get-Item 'IIS:\AppPools\$AppPoolName'" -ForegroundColor White
    Write-Host ""
    Write-Host "You can try starting it manually later." -ForegroundColor Yellow
    Write-Host ""
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Display summary
Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "  ? Application Pool: $AppPoolName" -ForegroundColor White

$finalPoolState = Get-WebAppPoolState -Name $AppPoolName -ErrorAction SilentlyContinue
if ($finalPoolState) {
    Write-Host "    State: $($finalPoolState.Value)" -ForegroundColor Gray
}

Write-Host "  ? Physical Path: $PhysicalPath" -ForegroundColor White
Write-Host "  ? Application: /$AppName" -ForegroundColor White

$finalBindings = Get-WebBinding -Name $SiteName -ErrorAction SilentlyContinue
if ($finalBindings) {
    Write-Host "  ? Site Bindings:" -ForegroundColor White
    $finalBindings | ForEach-Object {
        Write-Host "    $($_.protocol)://$($_.bindingInformation)" -ForegroundColor Gray
    }
}

Write-Host ""

# ============================================================================
# NEXT STEPS AND VERIFICATION
# ============================================================================

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Deploy your application files to: $PhysicalPath" -ForegroundColor White
Write-Host "     Command: dotnet publish -c Release -o $PhysicalPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "  2. Ensure .NET 10 Hosting Bundle is installed" -ForegroundColor White
Write-Host "     Download: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Cyan
Write-Host ""
Write-Host "  3. Verify web.config exists and is configured for Production" -ForegroundColor White
Write-Host "     Should contain: <aspNetCore processPath='dotnet' ... />" -ForegroundColor Gray
Write-Host ""
Write-Host "  4. Verify appsettings.Production.json has correct PathBase" -ForegroundColor White
Write-Host "     Should contain: `"PathBase`": `"/$AppName`"" -ForegroundColor Gray
Write-Host ""
Write-Host "  5. Test your application:" -ForegroundColor White
Write-Host "     Local: http://localhost/$AppName" -ForegroundColor Cyan
Write-Host "     Network: http://$IPAddress/$AppName" -ForegroundColor Cyan
Write-Host ""

Write-Host "Verification Commands:" -ForegroundColor Yellow
Write-Host "  # Check application pool status" -ForegroundColor Gray
Write-Host "  Get-WebAppPoolState -Name '$AppPoolName'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Check application configuration" -ForegroundColor Gray
Write-Host "  Get-WebApplication -Site '$SiteName' -Name '$AppName'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Check site bindings" -ForegroundColor Gray
Write-Host "  Get-WebBinding -Name '$SiteName'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Test network connectivity" -ForegroundColor Gray
Write-Host "  Test-NetConnection -ComputerName $IPAddress -Port $HttpPort" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # View application pool details" -ForegroundColor Gray
Write-Host "  Get-Item 'IIS:\AppPools\$AppPoolName' | Select-Object *" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Check IIS service status" -ForegroundColor Gray
Write-Host "  Get-Service W3SVC" -ForegroundColor Cyan
Write-Host ""

Write-Host "Troubleshooting Resources:" -ForegroundColor Yellow
Write-Host "  - Event Viewer: Application and System logs" -ForegroundColor White
Write-Host "  - IIS Logs: C:\inetpub\logs\LogFiles" -ForegroundColor White
Write-Host "  - Application Logs: Check $PhysicalPath\logs (if configured)" -ForegroundColor White
Write-Host "  - Restart IIS: iisreset" -ForegroundColor White
Write-Host "  - Restart App Pool: Restart-WebAppPool -Name '$AppPoolName'" -ForegroundColor White
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Script completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
