using System;
using System.Collections;

namespace Novell.Directory.Ldap.Utilclass
{
	public class AttributeQualifier
	{
		internal string name;

		internal ArrayList values;

		public virtual string Name => name;

		public virtual string[] Values
		{
			get
			{
				string[] array = null;
				if (values.Count > 0)
				{
					array = new string[values.Count];
					for (int i = 0; i < values.Count; i++)
					{
						array[i] = (string)values[i];
					}
				}
				return array;
			}
		}

		public AttributeQualifier(string name, string[] value_Renamed)
		{
			if (name == null || value_Renamed == null)
			{
				throw new ArgumentException("A null name or value was passed in for a schema definition qualifier");
			}
			this.name = name;
			values = new ArrayList(5);
			for (int i = 0; i < value_Renamed.Length; i++)
			{
				values.Add(value_Renamed[i]);
			}
		}
	}
}
