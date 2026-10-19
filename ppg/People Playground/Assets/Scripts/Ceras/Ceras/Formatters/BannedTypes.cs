using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Security.Principal;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	internal static class BannedTypes
	{
		private struct BannedType
		{
			public readonly Type Type;

			public readonly string FullName;

			public readonly string BanReason;

			public readonly bool AlsoCheckInherit;

			public readonly Func<Type, bool> CustomIsBannedCheck;

			public BannedType(Type type, string banReason, bool alsoCheckInherit)
			{
				Type = type;
				FullName = null;
				CustomIsBannedCheck = null;
				BanReason = banReason;
				AlsoCheckInherit = alsoCheckInherit;
			}

			public BannedType(string fullName, string banReason, bool alsoCheckInherit)
			{
				Type = null;
				FullName = fullName;
				CustomIsBannedCheck = null;
				BanReason = banReason;
				AlsoCheckInherit = alsoCheckInherit;
			}

			public BannedType(Func<Type, bool> customCheck, string banReason)
			{
				Type = null;
				FullName = null;
				CustomIsBannedCheck = customCheck;
				BanReason = banReason;
				AlsoCheckInherit = false;
			}
		}

		private static List<BannedType> _bannedTypes;

		static BannedTypes()
		{
			_bannedTypes = new List<BannedType>();
			BanBase(typeof(IEnumerator), "Enumerators are potentially infinite, and also most likely have no way to be instantiated at deserialization-time. If you think this is a mistake, report it as a github issue or provide a custom IFormatter for this case.");
			string reason = "This type can be exploited when deserializing malicious data";
			Ban(typeof(FileSystemInfo), reason);
			Ban(typeof(DataSet), reason);
			Ban("System.Management.IWbemClassObjectFreeThreaded", reason);
			BanBase(typeof(ClaimsIdentity), reason);
			Ban(typeof(WindowsIdentity), reason);
			Ban(typeof(TempFileCollection), reason);
			Ban(typeof(HashMembershipCondition), reason);
			BanCustom(IsUnityNativeType, "Native Unity objects cannot be serialized because they're not real C#/.NET objects. It is possible to create specialized formatters for some situations so Ceras can handle those objects, but it's not really possible to do this in a general and reliable way.");
		}

		private static bool IsUnityNativeType(Type t)
		{
			while (t != null)
			{
				if (t.FullName == "UnityEngine.Object")
				{
					return true;
				}
				t = t.BaseType;
			}
			return false;
		}

		private static void Ban(Type type, string reason)
		{
			_bannedTypes.Add(new BannedType(type, reason, alsoCheckInherit: false));
		}

		private static void Ban(string fullName, string reason)
		{
			_bannedTypes.Add(new BannedType(fullName, reason, alsoCheckInherit: false));
		}

		private static void BanBase(Type type, string reason)
		{
			_bannedTypes.Add(new BannedType(type, reason, alsoCheckInherit: true));
		}

		private static void BanBase(string fullName, string reason)
		{
			_bannedTypes.Add(new BannedType(fullName, reason, alsoCheckInherit: true));
		}

		private static void BanCustom(Func<Type, bool> isBanned, string reason)
		{
			_bannedTypes.Add(new BannedType(isBanned, reason));
		}

		internal static void ThrowIfBanned(Type type)
		{
			for (int i = 0; i < _bannedTypes.Count; i++)
			{
				BannedType ban = _bannedTypes[i];
				bool flag = false;
				if (ban.Type != null)
				{
					if (ban.AlsoCheckInherit)
					{
						if (ban.Type.IsAssignableFrom(type))
						{
							flag = true;
						}
					}
					else if (type == ban.Type)
					{
						flag = true;
					}
				}
				else if (ban.FullName != null)
				{
					if (ban.AlsoCheckInherit)
					{
						Type type2 = type;
						while (type2 != null)
						{
							if (type2.FullName == ban.FullName)
							{
								flag = true;
								break;
							}
							if (type2.GetInterfaces().Any((Type x) => x.FullName == ban.FullName))
							{
								flag = true;
								break;
							}
							type2 = type2.BaseType;
						}
					}
					else if (type.FullName == ban.FullName)
					{
						flag = true;
					}
				}
				else if (ban.CustomIsBannedCheck != null && ban.CustomIsBannedCheck(type))
				{
					flag = true;
				}
				if (flag)
				{
					throw new BannedTypeException("The type '" + type.FullName + "' cannot be serialized, please mark the field/property that caused this Type to be included with the [Exclude] attribute or filter it out using the 'ShouldSerialize' callback. Specific reason for this type being banned: \"" + ban.BanReason + "\". You should open an issue on GitHub or join the Discord server for support.");
				}
			}
		}

		internal static void ThrowIfNonspecific(Type type)
		{
			if (type.IsAbstract() || type.IsInterface || type.ContainsGenericParameters)
			{
				throw new InvalidOperationException("Can only generate code for specific types. The type " + type.FriendlyName(fullName: true) + " is abstract, or an interface, or an open generic.");
			}
		}
	}
}
