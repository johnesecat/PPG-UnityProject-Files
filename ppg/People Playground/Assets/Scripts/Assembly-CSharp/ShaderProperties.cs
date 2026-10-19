using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ShaderProperties
{
	private static readonly Dictionary<string, int> ids = new Dictionary<string, int>();

	public static int Get(string name)
	{
		if (ids.TryGetValue(name, out var value))
		{
			return value;
		}
		value = Shader.PropertyToID(name);
		ids.Add(name, value);
		return value;
	}
}
