using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct FreezeStackController
{
	private static Dictionary<Rigidbody2D, RefInt> stack = new Dictionary<Rigidbody2D, RefInt>();

	public static void Clear()
	{
		stack.Clear();
	}

	public static void RequestFreeze(Rigidbody2D a)
	{
		if ((bool)a)
		{
			if (stack.TryGetValue(a, out var value))
			{
				value.Increment();
				return;
			}
			value = new RefInt(1);
			stack.Add(a, value);
			a.bodyType = RigidbodyType2D.Static;
		}
	}

	public static void RequestUnfreeze(Rigidbody2D a)
	{
		if (!a || !stack.TryGetValue(a, out var value))
		{
			return;
		}
		value.Decrement();
		if (value.Value <= 0)
		{
			stack.Remove(a);
			if ((bool)a)
			{
				a.bodyType = RigidbodyType2D.Dynamic;
			}
		}
	}
}
