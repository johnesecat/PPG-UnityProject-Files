using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Activations
{
	public static class LegacyActivationCache
	{
		public class CachedResult
		{
			public bool Use;

			public bool UseContinuous;

			public bool IsolatedActivation;

			public bool IsolatedContinuousActivation;
		}

		private static readonly Dictionary<Type, CachedResult> cache = new Dictionary<Type, CachedResult>();

		private static List<Component> results = new List<Component>();

		public static void ProcessObject(GameObject gm, in ActivationEvent activation)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			gm.GetComponents(results);
			foreach (Component result in results)
			{
				CachedResult legacyActivationMethods = GetLegacyActivationMethods(result);
				flag |= legacyActivationMethods.Use;
				flag2 |= legacyActivationMethods.UseContinuous;
				flag3 |= legacyActivationMethods.IsolatedActivation;
				flag4 |= legacyActivationMethods.IsolatedContinuousActivation;
			}
			results.Clear();
		}

		public static CachedResult GetLegacyActivationMethods(Component component)
		{
			Type type = component.GetType();
			if (cache.TryGetValue(type, out var value))
			{
				return value;
			}
			CachedResult cachedResult = new CachedResult
			{
				Use = CanImplementInterface(type, typeof(Messages.IUse)),
				UseContinuous = CanImplementInterface(type, typeof(Messages.IUseContinuous)),
				IsolatedActivation = CanImplementInterface(type, typeof(Messages.IIsolatedActivation)),
				IsolatedContinuousActivation = CanImplementInterface(type, typeof(Messages.IIsolatedContinuousActivation))
			};
			cache.Add(type, cachedResult);
			return cachedResult;
		}

		private static bool CanImplementInterface(Type targetType, Type interfaceType)
		{
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException("Provided interfaceType must be an interface.");
			}
			if (targetType.IsInterface || targetType.IsAbstract)
			{
				return false;
			}
			if (interfaceType.IsAssignableFrom(targetType))
			{
				return true;
			}
			MethodInfo[] methods = interfaceType.GetMethods();
			foreach (MethodInfo interfaceMethod in methods)
			{
				if (!(from m in targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					where m.Name == interfaceMethod.Name && m.ReturnType == interfaceMethod.ReturnType && (from p in m.GetParameters()
						select p.ParameterType).SequenceEqual(from p in interfaceMethod.GetParameters()
						select p.ParameterType)
					select m).Any())
				{
					return false;
				}
			}
			return true;
		}
	}
}
