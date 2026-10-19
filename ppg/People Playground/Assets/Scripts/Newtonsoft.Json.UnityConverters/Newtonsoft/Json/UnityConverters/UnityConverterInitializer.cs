using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.UnityConverters.Configuration;
using Newtonsoft.Json.UnityConverters.Helpers;
using Newtonsoft.Json.UnityConverters.Utility;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters
{
	public static class UnityConverterInitializer
	{
		private static bool _shouldAddConvertsToDefaultSettings = true;

		public static JsonSerializerSettings defaultUnityConvertersSettings { get; set; } = CreateJsonSettingsWithFreslyLoadedConfig();


		public static bool shouldAddConvertsToDefaultSettings
		{
			get
			{
				return _shouldAddConvertsToDefaultSettings;
			}
			set
			{
				_shouldAddConvertsToDefaultSettings = value;
				UpdateDefaultSettings();
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		internal static void Init()
		{
			UpdateDefaultSettings();
		}

		private static void UpdateDefaultSettings()
		{
			if (shouldAddConvertsToDefaultSettings)
			{
				if (JsonConvert.DefaultSettings == null)
				{
					JsonConvert.DefaultSettings = GetExistingDefaultUnitySettings;
				}
			}
			else if (JsonConvert.DefaultSettings == new Func<JsonSerializerSettings>(GetExistingDefaultUnitySettings))
			{
				JsonConvert.DefaultSettings = null;
			}
		}

		public static void RefreshSettingsFromConfig()
		{
			defaultUnityConvertersSettings = CreateJsonSettingsWithFreslyLoadedConfig();
		}

		internal static JsonSerializerSettings GetExistingDefaultUnitySettings()
		{
			return defaultUnityConvertersSettings;
		}

		private static JsonSerializerSettings CreateJsonSettingsWithFreslyLoadedConfig()
		{
			UnityConvertersConfig unityConvertersConfig = Resources.Load<UnityConvertersConfig>("Newtonsoft.Json-for-Unity.Converters");
			if (!unityConvertersConfig)
			{
				unityConvertersConfig = ScriptableObject.CreateInstance<UnityConvertersConfig>();
			}
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
			{
				Converters = CreateConverters(unityConvertersConfig)
			};
			if (unityConvertersConfig.useUnityContractResolver)
			{
				jsonSerializerSettings.ContractResolver = new UnityTypeContractResolver();
			}
			return jsonSerializerSettings;
		}

		private static List<JsonConverter> CreateConverters(UnityConvertersConfig config)
		{
			return new List<JsonConverter>(from type in FindFilteredCustomConverters(config).Concat(FindFilteredUnityConverters(config)).Concat(FindFilteredJsonNetConverters(config))
				select CreateConverter(type) into o
				where o != null
				select o);
		}

		private static IEnumerable<Type> FindFilteredCustomConverters(UnityConvertersConfig config)
		{
			return ApplyConfigFilter(FindCustomConverters(), config.useAllOutsideConverters, config.outsideConverters);
		}

		internal static IEnumerable<Type> FindCustomConverters()
		{
			return FilterToJsonConvertersAndOrder((from dll in AppDomain.CurrentDomain.GetAssemblies()
				select dll.GetLoadableTypes().Where(delegate(Type type)
				{
					string @namespace = type.Namespace;
					return @namespace == null || !@namespace.StartsWith("Newtonsoft.Json");
				})).SelectMany((IEnumerable<Type> types) => types));
		}

		private static IEnumerable<Type> FindFilteredUnityConverters(UnityConvertersConfig config)
		{
			return ApplyConfigFilter(FindUnityConverters(), config.useAllUnityConverters, config.unityConverters);
		}

		internal static IEnumerable<Type> FindUnityConverters()
		{
			return FilterToJsonConvertersAndOrder(from type in (from dll in AppDomain.CurrentDomain.GetAssemblies()
					select from type in dll.GetLoadableTypes()
						where type.Namespace?.StartsWith("Newtonsoft.Json.UnityConverters") ?? false
						select type).SelectMany((IEnumerable<Type> types) => types)
				orderby type.FullName
				select type);
		}

		private static IEnumerable<Type> FindFilteredJsonNetConverters(UnityConvertersConfig config)
		{
			return ApplyConfigFilter(FindJsonNetConverters(), config.useAllJsonNetConverters, config.jsonNetConverters);
		}

		internal static IEnumerable<Type> FindJsonNetConverters()
		{
			return FilterToJsonConvertersAndOrder(typeof(JsonConverter).Assembly.GetTypes());
		}

		private static IEnumerable<Type> FilterToJsonConvertersAndOrder(IEnumerable<Type> types)
		{
			return from type in types
				where typeof(JsonConverter).IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericTypeDefinition && type.GetConstructor(Array.Empty<Type>()) != null
				orderby type.FullName
				select type;
		}

		private static IEnumerable<Type> ApplyConfigFilter(IEnumerable<Type> types, bool useAll, IEnumerable<ConverterConfig> configs)
		{
			if (useAll)
			{
				return types;
			}
			HashSet<Type> second = new HashSet<Type>(from o in configs
				where o.enabled
				select TypeCache.FindType(o.converterName) into o
				where o != null
				select o);
			return types.Intersect(second);
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		private static JsonConverter CreateConverter(Type jsonConverterType)
		{
			try
			{
				return (JsonConverter)Activator.CreateInstance(jsonConverterType);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Cannot create JsonConverter '{0}':\n{1}", jsonConverterType.FullName, ex);
			}
			return null;
		}
	}
}
