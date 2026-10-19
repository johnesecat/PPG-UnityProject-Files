using System.Collections.Generic;
using UnityEngine;

public abstract class Liquid
{
	public Color Color;

	public const float LiquidUnitToLiter = 5f / 14f;

	public const float LiterToLiquidUnit = 2.8f;

	internal static readonly Dictionary<string, Liquid> Registry = new Dictionary<string, Liquid>();

	internal static readonly HashSet<Liquid> LiquidSet = new HashSet<Liquid>();

	public abstract void OnEnterLimb(LimbBehaviour limb);

	public abstract void OnEnterContainer(BloodContainer container);

	public abstract void OnExitContainer(BloodContainer container);

	public virtual void OnUpdate(BloodContainer container)
	{
	}

	public virtual string GetDisplayName()
	{
		return GetIdentity(this) ?? GetType().Name;
	}

	public static bool HasID(string id)
	{
		if (id != null)
		{
			return Registry.ContainsKey(id);
		}
		return false;
	}

	public static bool HasLiquid(Liquid liq)
	{
		if (liq == null)
		{
			return false;
		}
		return LiquidSet.Contains(liq);
	}

	public static Liquid GetLiquid(string identity)
	{
		if (Registry.TryGetValue(identity, out var value))
		{
			return value;
		}
		Debug.LogErrorFormat("There is no liquid in the registry that matches \"{0}\"", identity);
		return null;
	}

	public static string GetIdentity(Liquid instance)
	{
		foreach (KeyValuePair<string, Liquid> item in Registry)
		{
			if (item.Value == instance)
			{
				return item.Key;
			}
		}
		Debug.LogErrorFormat("There is no liquid in the registry that matches \"{0}\"", instance.GetType());
		return null;
	}

	public static void Register(string identity, Liquid liquid)
	{
		if (Registry.ContainsKey(identity))
		{
			Debug.LogErrorFormat("A liquid that matches \"{0}\" is already registered.", identity);
		}
		else
		{
			Registry.Add(identity, liquid);
			LiquidSet.Add(liquid);
		}
	}

	internal static void ClearLiquidRegistry()
	{
		Registry.Clear();
		LiquidSet.Clear();
	}

	public IEnumerable<Liquid> GetAll()
	{
		foreach (Liquid item in LiquidSet)
		{
			yield return item;
		}
	}
}
