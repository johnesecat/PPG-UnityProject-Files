using System;
using System.Reflection;
using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapMessage
	{
		public const int BIND_REQUEST = 0;

		public const int BIND_RESPONSE = 1;

		public const int UNBIND_REQUEST = 2;

		public const int SEARCH_REQUEST = 3;

		public const int SEARCH_RESPONSE = 4;

		public const int SEARCH_RESULT = 5;

		public const int MODIFY_REQUEST = 6;

		public const int MODIFY_RESPONSE = 7;

		public const int ADD_REQUEST = 8;

		public const int ADD_RESPONSE = 9;

		public const int DEL_REQUEST = 10;

		public const int DEL_RESPONSE = 11;

		public const int MODIFY_RDN_REQUEST = 12;

		public const int MODIFY_RDN_RESPONSE = 13;

		public const int COMPARE_REQUEST = 14;

		public const int COMPARE_RESPONSE = 15;

		public const int ABANDON_REQUEST = 16;

		public const int SEARCH_RESULT_REFERENCE = 19;

		public const int EXTENDED_REQUEST = 23;

		public const int EXTENDED_RESPONSE = 24;

		public const int INTERMEDIATE_RESPONSE = 25;

		protected internal RfcLdapMessage message;

		private int imsgNum = -1;

		private int messageType = -1;

		private string stringTag;

		internal virtual LdapMessage RequestingMessage => message.RequestingMessage;

		public virtual LdapControl[] Controls
		{
			get
			{
				LdapControl[] array = null;
				RfcControls controls = message.Controls;
				if (controls != null)
				{
					array = new LdapControl[controls.size()];
					for (int i = 0; i < controls.size(); i++)
					{
						RfcControl obj = (RfcControl)controls.get_Renamed(i);
						string oid = obj.ControlType.stringValue();
						sbyte[] value_Renamed = obj.ControlValue.byteValue();
						bool critical = obj.Criticality.booleanValue();
						array[i] = controlFactory(oid, critical, value_Renamed);
					}
				}
				return array;
			}
		}

		public virtual int MessageID
		{
			get
			{
				if (imsgNum == -1)
				{
					imsgNum = message.MessageID;
				}
				return imsgNum;
			}
		}

		public virtual int Type
		{
			get
			{
				if (messageType == -1)
				{
					messageType = message.Type;
				}
				return messageType;
			}
		}

		public virtual bool Request => message.isRequest();

		internal virtual RfcLdapMessage Asn1Object => message;

		private string Name
		{
			get
			{
				switch (Type)
				{
				case 4:
					return "LdapSearchResponse";
				case 5:
					return "LdapSearchResult";
				case 3:
					return "LdapSearchRequest";
				case 6:
					return "LdapModifyRequest";
				case 7:
					return "LdapModifyResponse";
				case 8:
					return "LdapAddRequest";
				case 9:
					return "LdapAddResponse";
				case 10:
					return "LdapDelRequest";
				case 11:
					return "LdapDelResponse";
				case 12:
					return "LdapModifyRDNRequest";
				case 13:
					return "LdapModifyRDNResponse";
				case 14:
					return "LdapCompareRequest";
				case 15:
					return "LdapCompareResponse";
				case 0:
					return "LdapBindRequest";
				case 1:
					return "LdapBindResponse";
				case 2:
					return "LdapUnbindRequest";
				case 16:
					return "LdapAbandonRequest";
				case 19:
					return "LdapSearchResultReference";
				case 23:
					return "LdapExtendedRequest";
				case 24:
					return "LdapExtendedResponse";
				case 25:
					return "LdapIntermediateResponse";
				default:
					throw new SystemException("LdapMessage: Unknown Type " + Type);
				}
			}
		}

		public virtual string Tag
		{
			get
			{
				if (stringTag != null)
				{
					return stringTag;
				}
				if (Request)
				{
					return null;
				}
				return RequestingMessage?.stringTag;
			}
			set
			{
				stringTag = value;
			}
		}

		internal LdapMessage()
		{
		}

		internal LdapMessage(int type, RfcRequest op, LdapControl[] controls)
		{
			messageType = type;
			RfcControls rfcControls = null;
			if (controls != null)
			{
				rfcControls = new RfcControls();
				for (int i = 0; i < controls.Length; i++)
				{
					rfcControls.add(controls[i].Asn1Object);
				}
			}
			message = new RfcLdapMessage(op, rfcControls);
		}

		protected internal LdapMessage(RfcLdapMessage message)
		{
			this.message = message;
		}

		internal LdapMessage Clone(string dn, string filter, bool reference)
		{
			return new LdapMessage((RfcLdapMessage)message.dupMessage(dn, filter, reference));
		}

		private LdapControl controlFactory(string oid, bool critical, sbyte[] value_Renamed)
		{
			RespControlVector registeredControls = LdapControl.RegisteredControls;
			try
			{
				Type type = registeredControls.findResponseControl(oid);
				if (type == null)
				{
					return new LdapControl(oid, critical, value_Renamed);
				}
				Type[] types = new Type[3]
				{
					typeof(string),
					typeof(bool),
					typeof(sbyte[])
				};
				object[] parameters = new object[3] { oid, critical, value_Renamed };
				try
				{
					ConstructorInfo constructor = type.GetConstructor(types);
					try
					{
						return (LdapControl)constructor.Invoke(parameters);
					}
					catch (UnauthorizedAccessException)
					{
					}
					catch (TargetInvocationException)
					{
					}
					catch (Exception)
					{
					}
				}
				catch (MethodAccessException)
				{
				}
			}
			catch (FieldAccessException)
			{
			}
			return new LdapControl(oid, critical, value_Renamed);
		}

		public override string ToString()
		{
			return Name + "(" + MessageID + "): " + message.ToString();
		}
	}
}
