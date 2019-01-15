# .NET Installer class + RollBar deploy
MSI .NET Installer class Custom action + RollBar deploy

use Debug predefined variable to show action in log:
* event Commit __DEBUGVAR_Commit__
* sub-event Committed __DEBUGVAR_Committed__
* sub-event Committing __DEBUGVAR_Committing__
* event Install __DEBUGVAR_Install__
* event Uninstall __DEBUGVAR_Uninstall__
* event Rollback __DEBUGVAR_Rollback__
* event Deploy RollBar (POST data) __DEBUGVAR_Deploy__
