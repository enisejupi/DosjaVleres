# Automatic Deployment to IIS

## Overview
This project is configured to automatically deploy to IIS after every successful build. The application is deployed to `C:\inetpub\wwwroot\DoganaDosjaVleres` (or your system's equivalent path).

## How It Works

### Automatic Process
When you run `dotnet build`, the following happens automatically:

1. **Build** - The project is compiled
2. **Publish** - The application is published to `publish\output` folder
3. **Stop IIS** - The application pool is stopped to release file locks
4. **Copy Files** - All files are copied to `C:\inetpub\wwwroot\DoganaDosjaVleres`
5. **Start IIS** - The application pool is restarted

### Build Commands
```powershell
# Standard build (triggers automatic deployment)
dotnet build

# Release build (also triggers deployment)
dotnet build -c Release
```

## Configuration Details

The automatic deployment is configured in `KosovaDoganaModerne.csproj` using MSBuild targets with **dynamic paths** that work across different machines:

### Default Settings
- **IIS Root**: Automatically detected from system drive (`$(SystemDrive)\inetpub\wwwroot`)
- **App Name**: `DoganaDosjaVleres`
- **Full Path**: `C:\inetpub\wwwroot\DoganaDosjaVleres` (on C: drive)
- **App Pool**: `DefaultAppPool`

### Customizing for Different PCs

You can override the defaults by setting MSBuild properties:

```powershell
# Deploy to a different app name
dotnet build /p:IISAppName=MyCustomAppName

# Use a different application pool
dotnet build /p:AppPoolName=MyAppPool

# Use a completely different IIS root path
dotnet build /p:IISRootPath=D:\inetpub\wwwroot
```

### Permanent Customization

Create a `Directory.Build.props` file in your project root:

```xml
<Project>
  <PropertyGroup>
    <IISAppName>MyCustomAppName</IISAppName>
    <AppPoolName>MyAppPool</AppPoolName>
  </PropertyGroup>
</Project>
```

## Requirements

1. IIS must be installed and running
2. Run Visual Studio or terminal as Administrator for app pool operations
3. The deployment path will be automatically created if it doesn't exist

## Cross-Machine Compatibility

✓ Uses `$(SystemDrive)` variable - works on any drive letter  
✓ Dynamic path construction - no hardcoded paths  
✓ Customizable via MSBuild properties  
✓ Can be overridden per-machine without changing code  

## Troubleshooting

### Permission Issues
If you get permission errors, ensure you're running as Administrator:
```powershell
# Right-click PowerShell or Visual Studio
# Select "Run as Administrator"
```

### Files Locked
If files are locked during deployment:
1. The deployment script automatically stops the app pool
2. Waits 2 seconds for files to be released
3. If issues persist, manually stop: `Stop-WebAppPool -Name 'DefaultAppPool'`

### Deployment Not Running
To see deployment messages:
```powershell
# Build with detailed logging
dotnet build -v detailed | Select-String "Deploy"
```

### Disable Automatic Deployment
To temporarily disable automatic deployment, comment out the target in `KosovaDoganaModerne.csproj`:

```xml
<!-- Disable by commenting this section -->
<!--
<Target Name="DeployToIIS" AfterTargets="Build">
  ...
</Target>
-->
```

Or skip the target during build:
```powershell
dotnet build /p:SkipDeployToIIS=true
```

## Benefits

✓ No manual copying of files  
✓ Always in sync with latest build  
✓ Saves time during development  
✓ Reduces deployment errors  
✓ Works with any build command  
✓ **Cross-machine compatible - no hardcoded paths**  
✓ **Easy to customize per environment**  

## Deployment Location

After a successful build, your application will be available at:
- **File System**: `C:\inetpub\wwwroot\DoganaDosjaVleres` (or your system drive)
- **IIS Manager**: Configure your IIS site to point to this folder
- **Default URL**: `http://localhost/DoganaDosjaVleres` (if configured in IIS)

## Note

The automatic deployment:
- ✓ Runs after every successful build
- ✓ Works in Debug and Release configurations
- ✗ Does NOT run during design-time builds
- ✗ Does NOT run during IntelliSense operations
- ✗ Does NOT run on file save without explicit build
