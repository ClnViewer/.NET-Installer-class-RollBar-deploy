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
`BaseProjectDir/AppInstaller/UnInstallDeployApp/UnInstallDeployApp.cs` - you uninstall project
`BaseProjectDir/AppInstaller/InstallerDeployLib/InstallerDeployLib.cs` - you InstallerDeploy action library

## Editing all project:

1. Insert in `AppInstaller` project, all `CustomActionData` fields: `/xTargetDir="[TARGETDIR]\"`
2. Insert in `AppInstaller` project, shortcut UninstallYouApp actions `Arguments` fields: `/u={xxx}`
3. Run `AssemblyInfo.tt` in you `MyApp` project
4. Add `AssemblyInfo1.cs` in you `MyApp` project, and remove old `AssemblyInfo.cs`
5. Build `MyApp` project
6. Build `UnInstallDeployApp` project
7. Build `InstallerDeployLib` project library
8. Build `AppInstaller` to create __msi__ setup
9. End! :)

## License

_MIT_
