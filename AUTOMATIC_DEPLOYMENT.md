# Automatic Deployment to IIS

## Overview
This project is configured to automatically deploy to IIS (`C:\inetpub\wwwroot`) after every successful build.

## How It Works

### Automatic Process
When you run `dotnet build`, the following happens automatically:

1. **Build** - The project is compiled
2. **Publish** - The application is published to `publish\output` folder
3. **Stop IIS** - The DefaultAppPool is stopped to release file locks
4. **Copy Files** - All files are copied to `C:\inetpub\wwwroot`
5. **Start IIS** - The DefaultAppPool is restarted

### Build Commands
```powershell
# Standard build (triggers automatic deployment)
dotnet build

# Release build (also triggers deployment)
dotnet build -c Release
```

## Configuration Details

The automatic deployment is configured in `KosovaDoganaModerne.csproj` using MSBuild targets:

- **Target Name**: `DeployToIIS`
- **Trigger**: Runs after `Build` target
- **Destination**: `C:\inetpub\wwwroot`
- **App Pool**: DefaultAppPool

## Requirements

1. IIS must be installed and running
2. Run Visual Studio or terminal as Administrator for app pool operations
3. Ensure `C:\inetpub\wwwroot` directory exists and is writable

## Troubleshooting

### Permission Issues
If you get permission errors, ensure you're running as Administrator:
```powershell
# Right-click PowerShell or Visual Studio
# Select "Run as Administrator"
```

### Files Locked
If files are locked during deployment:
1. Manually stop the app pool: `Stop-WebAppPool -Name 'DefaultAppPool'`
2. Build again
3. The deployment script now handles this automatically

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

## Benefits

✓ No manual copying of files  
✓ Always in sync with latest build  
✓ Saves time during development  
✓ Reduces deployment errors  
✓ Works with any build command  

## Note

The automatic deployment only runs when building with MSBuild/dotnet CLI. It does NOT run during:
- Design-time builds in IDE
- IntelliSense operations
- File saves without explicit build
