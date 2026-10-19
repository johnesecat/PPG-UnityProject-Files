using System;
using System.Collections;

namespace Novell.Directory.Ldap
{
	public class LdapConstraints : ICloneable
	{
		private int msLimit;

		private int hopLimit = 10;

		private bool doReferrals;

		private LdapReferralHandler refHandler;

		private LdapControl[] controls;

		private static object nameLock;

		private static int lConsNum;

		private string name;

		private Hashtable properties;

		public virtual int HopLimit
		{
			get
			{
				return hopLimit;
			}
			set
			{
				hopLimit = value;
			}
		}

		internal virtual Hashtable Properties
		{
			get
			{
				return properties;
			}
			set
			{
				properties = (Hashtable)value.Clone();
			}
		}

		public virtual bool ReferralFollowing
		{
			get
			{
				return doReferrals;
			}
			set
			{
				doReferrals = value;
			}
		}

		public virtual int TimeLimit
		{
			get
			{
				return msLimit;
			}
			set
			{
				msLimit = value;
			}
		}

		public LdapConstraints()
		{
		}

		public LdapConstraints(int msLimit, bool doReferrals, LdapReferralHandler handler, int hop_limit)
		{
			this.msLimit = msLimit;
			this.doReferrals = doReferrals;
			refHandler = handler;
			hopLimit = hop_limit;
		}

		public virtual LdapControl[] getControls()
		{
			return controls;
		}

		public virtual object getProperty(string name)
		{
			if (properties == null)
			{
				return null;
			}
			return properties[name];
		}

		internal virtual LdapReferralHandler getReferralHandler()
		{
			return refHandler;
		}

		public virtual void setControls(LdapControl control)
		{
			if (control == null)
			{
				controls = null;
				return;
			}
			controls = new LdapControl[1];
			controls[0] = (LdapControl)control.Clone();
		}

		public virtual void setControls(LdapControl[] controls)
		{
			if (controls == null || controls.Length == 0)
			{
				this.controls = null;
				return;
			}
			this.controls = new LdapControl[controls.Length];
			for (int i = 0; i < controls.Length; i++)
			{
				this.controls[i] = (LdapControl)controls[i].Clone();
			}
		}

		public virtual void setProperty(string name, object value_Renamed)
		{
			if (properties == null)
			{
				properties = new Hashtable();
			}
			SupportClass.PutElement(properties, name, value_Renamed);
		}

		public virtual void setReferralHandler(LdapReferralHandler handler)
		{
			refHandler = handler;
		}

		public object Clone()
		{
			try
			{
				object obj = MemberwiseClone();
				if (controls != null)
				{
					((LdapConstraints)obj).controls = new LdapControl[controls.Length];
					controls.CopyTo(((LdapConstraints)obj).controls, 0);
				}
				if (properties != null)
				{
					((LdapConstraints)obj).properties = (Hashtable)properties.Clone();
				}
				return obj;
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
		}

		static LdapConstraints()
		{
			nameLock = new object();
		}
	}
}
