using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ceras.Helpers;

namespace Ceras
{
	public abstract class MemberConfig
	{
		private string _persistentNameOverride;

		internal bool HasNonSerialized;

		protected ReadonlyFieldHandling? _readonlyOverride;

		private string _explicitInclusionReason = "Error: no inclusion/exclusion override is set for this member";

		private SerializationOverride _serializationOverride;

		public TypeConfig TypeConfig { get; }

		public Type DeclaringType => TypeConfig.Type;

		public MemberInfo Member { get; }

		public Type MemberType
		{
			get
			{
				FieldInfo fieldInfo = Member as FieldInfo;
				if ((object)fieldInfo == null)
				{
					return ((PropertyInfo)Member).PropertyType;
				}
				return fieldInfo.FieldType;
			}
		}

		public string PersistentName
		{
			get
			{
				return _persistentNameOverride ?? Member.Name;
			}
			set
			{
				TypeConfig.ThrowIfSealed();
				_persistentNameOverride = value;
			}
		}

		public bool IsCompilerGenerated { get; }

		public bool IsReadonlyField { get; }

		public bool IsComputedProperty { get; }

		public ReadonlyFieldHandling? ReadonlyFieldHandling
		{
			get
			{
				return _readonlyOverride;
			}
			set
			{
				TypeConfig.ThrowIfSealed();
				_readonlyOverride = value;
			}
		}

		public int WriteBackOrder { get; set; }

		public string IncludeExcludeReason
		{
			get
			{
				InclusionExclusionResult inclusionExclusionResult = ComputeFinalInclusionResult(Member, needReason: true);
				return (inclusionExclusionResult.IsIncluded ? "Included: " : "Excluded: ") + inclusionExclusionResult.Reason;
			}
		}

		public SerializationOverride SerializationOverride
		{
			get
			{
				return _serializationOverride;
			}
			set
			{
				SetIncludeWithReason(value, "User has explicitly set member.SerializationOverride");
			}
		}

		internal void ExcludeWithReason(string reason)
		{
			SetIncludeWithReason(SerializationOverride.ForceSkip, reason);
		}

		internal void SetIncludeWithReason(SerializationOverride serializationOverride, string reason)
		{
			TypeConfig.ThrowIfSealed();
			if (serializationOverride == SerializationOverride.NoOverride)
			{
				throw new InvalidOperationException("Explicitly setting 'NoOverride', this must be a bug, please report it on GitHub!");
			}
			if (string.IsNullOrWhiteSpace(reason))
			{
				throw new InvalidOperationException("Missing reason in SetIncludeWithReason, this must be a bug, please report it on GitHub!");
			}
			_serializationOverride = serializationOverride;
			_explicitInclusionReason = reason;
		}

		protected MemberConfig(TypeConfig typeConfig, MemberInfo member)
		{
			TypeConfig = typeConfig;
			Member = member;
			IsCompilerGenerated = member.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
			IsReadonlyField = (member as FieldInfo)?.IsInitOnly ?? false;
			PropertyInfo propertyInfo = member as PropertyInfo;
			IsComputedProperty = (object)propertyInfo != null && propertyInfo.GetSetMethod(nonPublic: true) == null;
		}

		public InclusionExclusionResult ComputeFinalInclusion()
		{
			return ComputeFinalInclusionResult(Member, needReason: true);
		}

		internal bool ComputeFinalInclusionFast()
		{
			return ComputeFinalInclusionResult(Member, needReason: false).IsIncluded;
		}

		private InclusionExclusionResult ComputeFinalInclusionResult(MemberInfo memberInfo, bool needReason)
		{
			if (_serializationOverride != 0)
			{
				return new InclusionExclusionResult(_serializationOverride == SerializationOverride.ForceInclude, _explicitInclusionReason);
			}
			if (HasNonSerialized && TypeConfig.Config.Advanced.RespectNonSerializedAttribute)
			{
				return new InclusionExclusionResult(isIncluded: false, "Member has [NonSerializedAttribute] and 'RespectNonSerializedAttribute' is set in the config.");
			}
			if (IsReadonlyField)
			{
				if (_readonlyOverride.HasValue)
				{
					if (_readonlyOverride.Value == Ceras.ReadonlyFieldHandling.ExcludeFromSerialization)
					{
						return new InclusionExclusionResult(isIncluded: false, "Field is readonly and has [ReadonlyConfig] set to exclude.");
					}
				}
				else if (TypeConfig.ReadonlyFieldOverride.HasValue)
				{
					if (TypeConfig.ReadonlyFieldOverride.Value == Ceras.ReadonlyFieldHandling.ExcludeFromSerialization)
					{
						return new InclusionExclusionResult(isIncluded: false, "Field is readonly and the declaring type has a [MemberConfig] which excludes it");
					}
				}
				else if (TypeConfig.Config.Advanced.ReadonlyFieldHandling == Ceras.ReadonlyFieldHandling.ExcludeFromSerialization)
				{
					return new InclusionExclusionResult(isIncluded: false, "Field is readonly and the global default in the SerializerConfig is set to exclude");
				}
			}
			TargetMember targetMember = ComputeMemberTargetMask(memberInfo);
			if (TypeConfig.TargetMembers.HasValue)
			{
				if ((TypeConfig.TargetMembers.Value & targetMember) != 0)
				{
					return new InclusionExclusionResult(isIncluded: true, needReason ? $"Member is '{targetMember.Singular()}', which is included through the configuration '{TypeConfig.TargetMembers.Value}' of the declared Type '{DeclaringType.Name}'" : null);
				}
				return new InclusionExclusionResult(isIncluded: false, needReason ? $"Member is '{targetMember.Singular()}', which is excluded through the configuration '{TypeConfig.TargetMembers.Value}' of the declared Type '{DeclaringType.Name}'" : null);
			}
			if ((TypeConfig.Config.DefaultTargets & targetMember) != 0)
			{
				return new InclusionExclusionResult(isIncluded: true, needReason ? ("Member is '" + targetMember.Singular() + "', which is included by the 'DefaultTargets' configuration in the SerializerConfig") : null);
			}
			return new InclusionExclusionResult(isIncluded: false, needReason ? ("Member is '" + targetMember.Singular() + "', which is excluded by the 'DefaultTargets' configuration in the SerializerConfig") : null);
		}

		private static TargetMember ComputeMemberTargetMask(MemberInfo member)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if ((object)fieldInfo != null)
			{
				if (fieldInfo.IsPublic)
				{
					return TargetMember.PublicFields;
				}
				return TargetMember.PrivateFields;
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if ((object)propertyInfo != null)
			{
				if (propertyInfo.GetGetMethod(nonPublic: true).IsPublic)
				{
					return TargetMember.PublicProperties;
				}
				return TargetMember.PrivateProperties;
			}
			throw new ArgumentOutOfRangeException();
		}

		internal ReadonlyFieldHandling ComputeReadonlyHandling()
		{
			if (_readonlyOverride.HasValue)
			{
				return _readonlyOverride.Value;
			}
			if (TypeConfig.ReadonlyFieldOverride.HasValue)
			{
				return TypeConfig.ReadonlyFieldOverride.Value;
			}
			return TypeConfig.Config.Advanced.ReadonlyFieldHandling;
		}

		public override string ToString()
		{
			return TypeConfig.Type.FriendlyName() + "." + Member.Name + " (" + (ComputeFinalInclusionFast() ? "Included" : "Excluded") + ")";
		}
	}
}
