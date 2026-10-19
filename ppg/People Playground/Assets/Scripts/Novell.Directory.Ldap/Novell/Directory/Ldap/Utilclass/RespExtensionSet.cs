using System;
using System.Collections;

namespace Novell.Directory.Ldap.Utilclass
{
	public class RespExtensionSet : SupportClass.AbstractSetSupport
	{
		private Hashtable map;

		public override int Count => map.Count;

		public RespExtensionSet()
		{
			map = new Hashtable();
		}

		public void registerResponseExtension(string oid, Type extClass)
		{
			lock (this)
			{
				if (!map.ContainsKey(oid))
				{
					map.Add(oid, extClass);
				}
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return map.Values.GetEnumerator();
		}

		public Type findResponseExtension(string searchOID)
		{
			lock (this)
			{
				if (map.ContainsKey(searchOID))
				{
					return (Type)map[searchOID];
				}
				return null;
			}
		}
	}
}
