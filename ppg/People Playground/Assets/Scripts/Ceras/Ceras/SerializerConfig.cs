using System;
using System.Collections.Generic;
using System.Reflection;
using Ceras.Resolvers;

namespace Ceras
{
	public class SerializerConfig : IAdvancedConfigOptions, ISizeLimitsConfig, IVersionToleranceConfig
	{
		private bool _isSealed;

		private VersionToleranceMode _versionToleranceMode;

		private Dictionary<Type, TypeConfig> _configEntries = new Dictionary<Type, TypeConfig>();

		private Dictionary<Type, TypeConfig> _staticConfigEntries = new Dictionary<Type, TypeConfig>();

		private Action<TypeConfig> _onConfigNewType;

		internal bool IsSealed => _isSealed;

		public List<Type> KnownTypes { get; internal set; } = new List<Type>();


		public IExternalObjectResolver ExternalObjectResolver { get; set; }

		public Action<IExternalRootObject> OnExternalObject { get; set; }

		public List<FormatterResolverCallback> OnResolveFormatter { get; } = new List<FormatterResolverCallback>();


		public bool PreserveReferences { get; set; } = true;


		public TargetMember DefaultTargets { get; set; } = TargetMember.AllPublic;


		public IVersionToleranceConfig VersionTolerance => this;

		VersionToleranceMode IVersionToleranceConfig.Mode
		{
			get
			{
				return _versionToleranceMode;
			}
			set
			{
				if (_versionToleranceMode == VersionToleranceMode.Disabled && value != 0)
				{
					Advanced.UseReinterpretFormatter = false;
				}
				_versionToleranceMode = value;
			}
		}

		bool IVersionToleranceConfig.VerifySizes { get; set; }

		public Action<TypeConfig> OnConfigNewType
		{
			get
			{
				return _onConfigNewType;
			}
			set
			{
				if (_onConfigNewType == null)
				{
					_onConfigNewType = value;
					return;
				}
				throw new InvalidOperationException("OnConfigNewType is already set. Multiple type configuration callbacks would overwrite each others changes, you must collect all the callbacks into one function to maintain detailed control over how each Type gets configured.");
			}
		}

		public IAdvancedConfigOptions Advanced => this;

		ISizeLimitsConfig IAdvancedConfigOptions.SizeLimits => this;

		uint ISizeLimitsConfig.MaxStringLength { get; set; } = uint.MaxValue;


		uint ISizeLimitsConfig.MaxArraySize { get; set; } = uint.MaxValue;


		uint ISizeLimitsConfig.MaxByteArraySize { get; set; } = uint.MaxValue;


		uint ISizeLimitsConfig.MaxCollectionSize { get; set; } = uint.MaxValue;


		Action<object> IAdvancedConfigOptions.DiscardObjectMethod { get; set; }

		ReadonlyFieldHandling IAdvancedConfigOptions.ReadonlyFieldHandling { get; set; }

		bool IAdvancedConfigOptions.EmbedChecksum { get; set; }

		bool IAdvancedConfigOptions.PersistTypeCache { get; set; }

		bool IAdvancedConfigOptions.SealTypesWhenUsingKnownTypes { get; set; } = true;


		bool IAdvancedConfigOptions.SkipCompilerGeneratedFields { get; set; } = true;


		ITypeBinder IAdvancedConfigOptions.TypeBinder { get; set; } = new SimpleTypeBinder();


		DelegateSerializationFlags IAdvancedConfigOptions.DelegateSerialization { get; set; }

		bool IAdvancedConfigOptions.UseReinterpretFormatter { get; set; } = true;


		bool IAdvancedConfigOptions.RespectNonSerializedAttribute { get; set; } = true;


		BitmapMode IAdvancedConfigOptions.BitmapMode { get; set; }

		AotMode IAdvancedConfigOptions.AotMode { get; set; }

		internal void Seal()
		{
			_isSealed = true;
		}

		private TypeConfig GetTypeConfigForConfiguration(Type type, bool isStatic = false)
		{
			Dictionary<Type, TypeConfig> dictionary = (isStatic ? _staticConfigEntries : _configEntries);
			if (dictionary.TryGetValue(type, out var value))
			{
				return value;
			}
			if (type.ContainsGenericParameters)
			{
				throw new InvalidOperationException("You can not configure 'open' types (like List<>)! Only 'closed' types (like 'List<int>') can be configured statically. For dynamic configuration (which is what you are trying to do) use the 'OnConfigNewType' callback. It will be called for every fully instantiated type.");
			}
			value = (TypeConfig)Activator.CreateInstance(typeof(TypeConfig<>).MakeGenericType(type), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[2] { this, isStatic }, null);
			dictionary.Add(type, value);
			return value;
		}

		internal TypeConfig GetTypeConfig(Type type, bool isStatic)
		{
			Dictionary<Type, TypeConfig> dictionary = (isStatic ? _staticConfigEntries : _configEntries);
			if (dictionary.TryGetValue(type, out var value))
			{
				return value;
			}
			if (type.ContainsGenericParameters)
			{
				return null;
			}
			value = (TypeConfig)Activator.CreateInstance(typeof(TypeConfig<>).MakeGenericType(type), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[2] { this, isStatic }, null);
			OnConfigNewType?.Invoke(value);
			dictionary.Add(type, value);
			return value;
		}

		public TypeConfig ConfigType(Type type)
		{
			return GetTypeConfigForConfiguration(type);
		}

		public TypeConfig ConfigStaticType(Type type)
		{
			return GetTypeConfigForConfiguration(type, isStatic: true);
		}

		public TypeConfig<T> ConfigType<T>()
		{
			return (TypeConfig<T>)GetTypeConfigForConfiguration(typeof(T));
		}
	}
}
