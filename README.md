# .NET Installer class + RollBar deploy
MSI .NET Installer class: Custom action + RollBar deploy

use Debug predefined variable to show action in log:
* event Commit __DEBUGVAR_Commit__
* sub-event Committed __DEBUGVAR_Committed__
* sub-event Committing __DEBUGVAR_Committing__
* event Install __DEBUGVAR_Install__
* event Uninstall __DEBUGVAR_Uninstall__
* event Rollback __DEBUGVAR_Rollback__
* event Deploy RollBar (POST data) __DEBUGVAR_Deploy__

## Location:

`BaseProjectDir/MyApp/Properties/AssemblyInfo.tt` - you application project  
`BaseProjectDir/MyApp/Properties/AssemblyRollBarId.cs` - you application project  
`BaseProjectDir/AppInstaller/` - you __msi__ installer VS project (.vdproj)  
`BaseProjectDir/AppInstaller/UnInstallDeployApp/UnInstallDeployApp.cs` - you uninstall project  
`BaseProjectDir/AppInstaller/InstallerDeployLib/InstallerDeployLib.cs` - you InstallerDeploy action library  

## Editing all project:

1. Edit `AssemblyInfo.tt` and modify you RollBar Id and other assembly `MyApp` setings
2. Insert in `AppInstaller` project, all `CustomActionData` fields: `/xTargetDir="[TARGETDIR]\"`
2. Insert in `AppInstaller` project, shortcut UninstallYouApp actions `Arguments` fields: `/u={xxx}`
4. Run `AssemblyInfo.tt` in you `MyApp` project
5. Add `AssemblyInfo1.cs` in you `MyApp` project, and remove old `AssemblyInfo.cs`
6. Add `AppVersionInfo.cs` in you `InstallerDeployLib` project
7. Build `MyApp` project
8. Build `UnInstallDeployApp` project
9. Build `InstallerDeployLib` project library
10. Build `AppInstaller` to create __msi__ setup
11. End! :)

## License

_MIT_
