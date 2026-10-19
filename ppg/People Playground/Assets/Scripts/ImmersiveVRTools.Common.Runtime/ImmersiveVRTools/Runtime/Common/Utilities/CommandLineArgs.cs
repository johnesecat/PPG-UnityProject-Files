using System;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public static class CommandLineArgs
	{
		public static string GetNamed(string name)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i] == name && commandLineArgs.Length > i + 1)
				{
					return commandLineArgs[i + 1];
				}
			}
			return null;
		}
	}
}
