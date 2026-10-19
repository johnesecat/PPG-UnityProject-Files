using System;
using System.Text;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapControl : ICloneable
	{
		private static RespControlVector registeredControls;

		private RfcControl control;

		public virtual string ID => new StringBuilder(control.ControlType.stringValue()).ToString();

		public virtual bool Critical => control.Criticality.booleanValue();

		internal static RespControlVector RegisteredControls => registeredControls;

		internal virtual RfcControl Asn1Object => control;

		[CLSCompliant(false)]
		public LdapControl(string oid, bool critical, sbyte[] values)
		{
			if (oid == null)
			{
				throw new ArgumentException("An OID must be specified");
			}
			if (values == null)
			{
				control = new RfcControl(new RfcLdapOID(oid), new Asn1Boolean(critical));
			}
			else
			{
				control = new RfcControl(new RfcLdapOID(oid), new Asn1Boolean(critical), new Asn1OctetString(values));
			}
		}

		protected internal LdapControl(RfcControl control)
		{
			this.control = control;
		}

		public object Clone()
		{
			LdapControl ldapControl;
			try
			{
				ldapControl = (LdapControl)MemberwiseClone();
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
			sbyte[] value = getValue();
			sbyte[] array = null;
			if (value != null)
			{
				array = new sbyte[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					array[i] = value[i];
				}
				ldapControl.control = new RfcControl(new RfcLdapOID(ID), new Asn1Boolean(Critical), new Asn1OctetString(array));
			}
			return ldapControl;
		}

		[CLSCompliant(false)]
		public virtual sbyte[] getValue()
		{
			sbyte[] result = null;
			Asn1OctetString controlValue = control.ControlValue;
			if (controlValue != null)
			{
				result = controlValue.byteValue();
			}
			return result;
		}

		[CLSCompliant(false)]
		protected internal virtual void setValue(sbyte[] controlValue)
		{
			control.ControlValue = new Asn1OctetString(controlValue);
		}

		public static void register(string oid, Type controlClass)
		{
			registeredControls.registerResponseControl(oid, controlClass);
		}

		static LdapControl()
		{
			registeredControls = new RespControlVector(5, 5);
		}
	}
}
