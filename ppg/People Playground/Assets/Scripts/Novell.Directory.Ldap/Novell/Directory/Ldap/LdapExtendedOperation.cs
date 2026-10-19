using System;

namespace Novell.Directory.Ldap
{
	public class LdapExtendedOperation : ICloneable
	{
		private string oid;

		private sbyte[] vals;

		[CLSCompliant(false)]
		public LdapExtendedOperation(string oid, sbyte[] vals)
		{
			this.oid = oid;
			this.vals = vals;
		}

		public object Clone()
		{
			try
			{
				object obj = MemberwiseClone();
				Array.Copy(vals, 0, ((LdapExtendedOperation)obj).vals, 0, vals.Length);
				return obj;
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
		}

		public virtual string getID()
		{
			return oid;
		}

		[CLSCompliant(false)]
		public virtual sbyte[] getValue()
		{
			return vals;
		}

		[CLSCompliant(false)]
		protected internal virtual void setValue(sbyte[] newVals)
		{
			vals = newVals;
		}

		protected internal virtual void setID(string newoid)
		{
			oid = newoid;
		}
	}
}
