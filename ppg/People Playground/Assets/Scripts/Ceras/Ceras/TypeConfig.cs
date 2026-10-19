using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Ceras.Exceptions;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	public abstract class TypeConfig
	{
		private bool _isSealed;

		protected TypeConstruction _typeConstruction;

		internal Dictionary<ParameterInfo, MemberInfo> ParameterMap;

		protected FormatterResolverCallback _customResolver;

		protected Ceras.Formatters.IFormatter _overrideFormatter;

		protected ReadonlyFieldHandling? _readonlyOverride;

		protected TargetMember? _targetMembers;

		internal readonly List<MemberConfig> _allMembers;

		public Type Type { get; }

		public SerializerConfig Config { get; }

		public bool IsStatic { get; }

		public TypeConstruction TypeConstruction
		{
			get
			{
				return _typeConstruction;
			}
			set
			{
				ThrowIfSealed();
				_typeConstruction = value;
				if (value != null)
				{
					TypeConstruction.TypeConfig = this;
					TypeConstruction.VerifyReturnType();
				}
			}
		}

		public FormatterResolverCallback CustomResolver
		{
			get
			{
				return _customResolver;
			}
			set
			{
				ThrowIfSealed();
				if (_overrideFormatter != null)
				{
					ThrowCantSetResolverAndFormatter();
				}
				_customResolver = value;
			}
		}

		public Ceras.Formatters.IFormatter CustomFormatter
		{
			get
			{
				return _overrideFormatter;
			}
			set
			{
				ThrowIfSealed();
				if (_customResolver != null)
				{
					ThrowCantSetResolverAndFormatter();
				}
				if (value != null)
				{
					FormatterHelper.ThrowOnMismatch(value, Type);
					_overrideFormatter = value;
				}
				else
				{
					_overrideFormatter = null;
				}
			}
		}

		public ReadonlyFieldHandling? ReadonlyFieldOverride
		{
			get
			{
				return _readonlyOverride;
			}
			set
			{
				ThrowIfSealed();
				_readonlyOverride = value;
			}
		}

		public TargetMember? TargetMembers
		{
			get
			{
				return _targetMembers;
			}
			set
			{
				ThrowIfSealed();
				_targetMembers = value;
			}
		}

		public IEnumerable<MemberConfig> Members => _allMembers.Where((MemberConfig m) => !m.IsCompilerGenerated);

		private void ThrowCantSetResolverAndFormatter()
		{
			throw new InvalidOperationException("You can only set a custom resolver or a custom formatter instance, not both.");
		}

		protected TypeConfig(SerializerConfig config, Type type, bool isStatic)
		{
			Config = config;
			Type = type;
			IsStatic = isStatic;
			Type configType = typeof(MemberConfig<>).MakeGenericType(type);
			IEnumerable<MemberConfig> source = from m in isStatic ? type.GetAllStaticDataMembers() : type.GetAllDataMembers()
				let a = new object[2] { this, m }
				select (MemberConfig)Activator.CreateInstance(configType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, a, null);
			_allMembers = source.ToList();
			TypeConfigDefaults.ApplyTypeAttributes(this);
			TypeConfigDefaults.ApplySpecializedDefaults(this);
			foreach (MemberConfig allMember in _allMembers)
			{
				TypeConfigDefaults.ApplyMemberAttributes(allMember);
			}
		}

		internal MemberConfig GetMemberConfig(MemberInfo memberInfo)
		{
			for (int i = 0; i < _allMembers.Count; i++)
			{
				MemberConfig memberConfig = _allMembers[i];
				if (memberConfig.Member == memberInfo)
				{
					return memberConfig;
				}
			}
			throw new InvalidOperationException("Cannot find member '" + memberInfo.DeclaringType.FriendlyName() + "." + memberInfo.Name + "' in type '" + Type.FriendlyName(fullName: true) + "'");
		}

		public IEnumerable<MemberConfig> UnsafeGetAllMembersIncludingCompilerGenerated()
		{
			return _allMembers;
		}

		public bool TryConfigureLikeDataContractSerializer(bool force = false)
		{
			if (Type.GetCustomAttribute<DataContractAttribute>() == null && !force)
			{
				return false;
			}
			foreach (MemberConfig member in Members)
			{
				if (member.Member.GetCustomAttribute<IgnoreDataMemberAttribute>(inherit: true) != null)
				{
					member.ExcludeWithReason("User has called 'TryConfigureLikeDataContractSerializer', and the member has the [IgnoreDataMember] attribute.");
					continue;
				}
				DataMemberAttribute customAttribute = member.Member.GetCustomAttribute<DataMemberAttribute>(inherit: true);
				if (customAttribute != null)
				{
					if (!string.IsNullOrWhiteSpace(customAttribute.Name))
					{
						member.PersistentName = customAttribute.Name;
					}
					member.SetIncludeWithReason(SerializationOverride.ForceInclude, "User has called 'TryConfigureLikeDataContractSerializer', and the member has the [DataMember] attribute.");
				}
			}
			return true;
		}

		internal void Seal()
		{
			_isSealed = true;
		}

		internal void VerifyConstructionMethod()
		{
			if (TypeConstruction == null)
			{
				throw new CerasException("You have not configured a construction mode for the type '" + Type.FullName + "' and it has no parameterless constructor. There are many ways Ceras can handle this, select one of the methods in the TypeConfig ('config.ConfigType<YourType>().ConstructBy(...)')");
			}
			TypeConstruction.VerifyReturnType();
			TypeConstruction.VerifyParameterMapping();
		}

		internal void ThrowIfSealed()
		{
			if (_isSealed)
			{
				throw new ConfigurationSealedException("The configuration for this Type or Member is already sealed because the SerializationSchema has been instantiated (which means dynamically emitted code relies on it not changing anymore). All changes to the type or member configuration must be made before the the configuration is used in a 'CerasSerializer' instance, except for config callbacks like OnConfigNewType)");
			}
		}
	}
}
