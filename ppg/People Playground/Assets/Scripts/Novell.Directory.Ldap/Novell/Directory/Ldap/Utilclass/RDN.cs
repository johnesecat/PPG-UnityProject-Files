using System;
using System.Collections;

namespace Novell.Directory.Ldap.Utilclass
{
	public class RDN
	{
		private ArrayList types;

		private ArrayList values;

		private string rawValue;

		protected internal virtual string RawValue => rawValue;

		public virtual string Type => (string)types[0];

		public virtual string[] Types
		{
			get
			{
				string[] array = new string[types.Count];
				for (int i = 0; i < types.Count; i++)
				{
					array[i] = (string)types[i];
				}
				return array;
			}
		}

		public virtual string Value => (string)values[0];

		public virtual string[] Values
		{
			get
			{
				string[] array = new string[values.Count];
				for (int i = 0; i < values.Count; i++)
				{
					array[i] = (string)values[i];
				}
				return array;
			}
		}

		public virtual bool Multivalued
		{
			get
			{
				if (values.Count <= 1)
				{
					return false;
				}
				return true;
			}
		}

		public RDN(string rdn)
		{
			rawValue = rdn;
			ArrayList rDNs = new DN(rdn).RDNs;
			if (rDNs.Count != 1)
			{
				throw new ArgumentException("Invalid RDN: see API documentation");
			}
			RDN rDN = (RDN)rDNs[0];
			types = rDN.types;
			values = rDN.values;
			rawValue = rDN.rawValue;
		}

		public RDN()
		{
			types = new ArrayList();
			values = new ArrayList();
			rawValue = "";
		}

		[CLSCompliant(false)]
		public virtual bool equals(RDN rdn)
		{
			if (values.Count != rdn.values.Count)
			{
				return false;
			}
			for (int i = 0; i < values.Count; i++)
			{
				int j;
				for (j = 0; j < values.Count && (!((string)values[i]).ToUpper().Equals(((string)rdn.values[j]).ToUpper()) || !equalAttrType((string)types[i], (string)rdn.types[j])); j++)
				{
				}
				if (j >= rdn.values.Count)
				{
					return false;
				}
			}
			return true;
		}

		private bool equalAttrType(string attr1, string attr2)
		{
			if (char.IsDigit(attr1[0]) ^ char.IsDigit(attr2[0]))
			{
				throw new ArgumentException("OID numbers are not currently compared to attribute names");
			}
			return attr1.ToUpper().Equals(attr2.ToUpper());
		}

		public virtual void add(string attrType, string attrValue, string rawValue)
		{
			types.Add(attrType);
			values.Add(attrValue);
			this.rawValue += rawValue;
		}

		public override string ToString()
		{
			return toString(noTypes: false);
		}

		[CLSCompliant(false)]
		public virtual string toString(bool noTypes)
		{
			int count = types.Count;
			string text = "";
			if (count < 1)
			{
				return null;
			}
			if (!noTypes)
			{
				text = string.Concat(types[0], "=");
			}
			text += values[0];
			for (int i = 1; i < count; i++)
			{
				text += "+";
				if (!noTypes)
				{
					text = string.Concat(text, types[i], "=");
				}
				text += values[i];
			}
			return text;
		}

		public virtual string[] explodeRDN(bool noTypes)
		{
			int count = types.Count;
			if (count < 1)
			{
				return null;
			}
			string[] array = new string[types.Count];
			if (!noTypes)
			{
				array[0] = string.Concat(types[0], "=");
			}
			array[0] += values[0];
			for (int i = 1; i < count; i++)
			{
				if (!noTypes)
				{
					ref string reference = ref array[i];
					reference = string.Concat(reference, types[i], "=");
				}
				array[i] += values[i];
			}
			return array;
		}
	}
}
