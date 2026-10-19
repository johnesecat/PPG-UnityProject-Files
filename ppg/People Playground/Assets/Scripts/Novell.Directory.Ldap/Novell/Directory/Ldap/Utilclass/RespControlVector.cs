using System;
using System.Collections;

namespace Novell.Directory.Ldap.Utilclass
{
	public class RespControlVector : ArrayList
	{
		private class RegisteredControl
		{
			private RespControlVector enclosingInstance;

			public string myOID;

			public Type myClass;

			public RespControlVector Enclosing_Instance => enclosingInstance;

			private void InitBlock(RespControlVector enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public RegisteredControl(RespControlVector enclosingInstance, string oid, Type controlClass)
			{
				InitBlock(enclosingInstance);
				myOID = oid;
				myClass = controlClass;
			}
		}

		public RespControlVector(int cap, int incr)
			: base(cap)
		{
		}

		public void registerResponseControl(string oid, Type controlClass)
		{
			lock (this)
			{
				Add(new RegisteredControl(this, oid, controlClass));
			}
		}

		public Type findResponseControl(string searchOID)
		{
			lock (this)
			{
				RegisteredControl registeredControl = null;
				for (int i = 0; i < Count; i++)
				{
					if ((registeredControl = (RegisteredControl)this[i]) == null)
					{
						throw new FieldAccessException();
					}
					if (registeredControl.myOID.CompareTo(searchOID) == 0)
					{
						return registeredControl.myClass;
					}
				}
				return null;
			}
		}
	}
}
