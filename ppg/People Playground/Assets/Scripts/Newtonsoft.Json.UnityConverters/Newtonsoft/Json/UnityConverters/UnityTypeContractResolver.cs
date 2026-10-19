using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters
{
	public class UnityTypeContractResolver : DefaultContractResolver
	{
		protected override List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			List<MemberInfo> serializableMembers = base.GetSerializableMembers(objectType);
			serializableMembers.AddRange(GetMissingMembers(objectType, serializableMembers));
			return serializableMembers;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
			if (member.GetCustomAttribute<SerializeField>() != null)
			{
				jsonProperty.Ignored = false;
				jsonProperty.Writable = CanWriteMemberWithSerializeField(member);
				jsonProperty.Readable = CanReadMemberWithSerializeField(member);
				jsonProperty.HasMemberAttribute = true;
			}
			return jsonProperty;
		}

		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			return base.CreateProperties(type, memberSerialization);
		}

		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract jsonObjectContract = base.CreateObjectContract(objectType);
			if (typeof(ScriptableObject).IsAssignableFrom(objectType))
			{
				jsonObjectContract.DefaultCreator = () => ScriptableObject.CreateInstance(objectType);
			}
			return jsonObjectContract;
		}

		private static IEnumerable<MemberInfo> GetMissingMembers(Type type, List<MemberInfo> alreadyAdded)
		{
			return from o in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Cast<MemberInfo>().Concat(type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
				where o.GetCustomAttribute<SerializeField>() != null && !alreadyAdded.Contains(o)
				select o;
		}

		private static bool CanReadMemberWithSerializeField(MemberInfo member)
		{
			return (member as PropertyInfo)?.CanRead ?? true;
		}

		private static bool CanWriteMemberWithSerializeField(MemberInfo member)
		{
			return (member as PropertyInfo)?.CanWrite ?? true;
		}
	}
}
