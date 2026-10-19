using System;
using System.IO;
using System.Text;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public static class PathUtilities
	{
		public static bool TryMakeRelative(string absoluteParentPath, string absolutePath, out string relativePath)
		{
			if (CanMakeRelative(absoluteParentPath, absolutePath))
			{
				relativePath = MakeRelative(absoluteParentPath, absolutePath);
				return true;
			}
			relativePath = null;
			return false;
		}

		public static bool CanMakeRelative(string absoluteParentPath, string absolutePath)
		{
			if (absoluteParentPath == null)
			{
				throw new ArgumentNullException("absoluteParentPath");
			}
			if (absolutePath == null)
			{
				throw new ArgumentNullException("absoluteParentPath");
			}
			absoluteParentPath = absoluteParentPath.Replace('\\', '/').Trim('/');
			absolutePath = absolutePath.Replace('\\', '/').Trim('/');
			return Path.GetPathRoot(absoluteParentPath).Equals(Path.GetPathRoot(absolutePath), StringComparison.CurrentCultureIgnoreCase);
		}

		public static string MakeRelative(string absoluteParentPath, string absolutePath)
		{
			absoluteParentPath = absoluteParentPath.TrimEnd('\\', '/');
			absolutePath = absolutePath.TrimEnd('\\', '/');
			string[] array = absoluteParentPath.Split('/', '\\');
			string[] array2 = absolutePath.Split('/', '\\');
			int num = -1;
			for (int i = 0; i < array.Length && i < array2.Length && array[i].Equals(array2[i], StringComparison.CurrentCultureIgnoreCase); i++)
			{
				num = i;
			}
			if (num == -1)
			{
				throw new InvalidOperationException("No common directory found.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (num + 1 < array.Length)
			{
				for (int j = num + 1; j < array.Length; j++)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('/');
					}
					stringBuilder.Append("..");
				}
			}
			for (int k = num + 1; k < array2.Length; k++)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append('/');
				}
				stringBuilder.Append(array2[k]);
			}
			return stringBuilder.ToString();
		}

		public static string RemoveLatestPathPart(string path, string pathDelimiter = "/")
		{
			int num = path.LastIndexOf(pathDelimiter);
			if (num >= 0)
			{
				return path.Substring(0, num);
			}
			return path;
		}
	}
}
