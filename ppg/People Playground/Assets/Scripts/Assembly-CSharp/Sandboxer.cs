using System;
using System.IO;
using System.Security;
using System.Security.Permissions;

[Obsolete]
public class Sandboxer
{
	public static AppDomain ModDomain;

	public static void Initialise()
	{
		PermissionSet permissionSet = new PermissionSet(PermissionState.None);
		permissionSet.AddPermission(new FileIOPermission(PermissionState.None)
		{
			AllFiles = FileIOPermissionAccess.NoAccess
		});
		permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
		permissionSet.AddPermission(new FileDialogPermission(FileDialogPermissionAccess.None));
		AppDomainSetup appDomainSetup = new AppDomainSetup();
		appDomainSetup.ApplicationBase = Path.GetFullPath("CompiledModAssemblies");
		ModDomain = AppDomain.CreateDomain("Mod domain", null, appDomainSetup, permissionSet);
	}
}
