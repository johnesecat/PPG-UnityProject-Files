using System;

namespace Novell.Directory.Ldap
{
	public class InterThreadException : LdapException
	{
		private Message request;

		internal virtual int MessageID
		{
			get
			{
				if (request == null)
				{
					return -1;
				}
				return request.MessageID;
			}
		}

		internal virtual int ReplyType
		{
			get
			{
				if (request == null)
				{
					return -1;
				}
				int messageType = request.MessageType;
				int result = -1;
				switch (messageType)
				{
				case 0:
					result = 1;
					break;
				case 2:
					result = -1;
					break;
				case 3:
					result = 5;
					break;
				case 6:
					result = 7;
					break;
				case 8:
					result = 9;
					break;
				case 10:
					result = 11;
					break;
				case 12:
					result = 13;
					break;
				case 14:
					result = 15;
					break;
				case 16:
					result = -1;
					break;
				case 23:
					result = 24;
					break;
				}
				return result;
			}
		}

		internal InterThreadException(string message, object[] arguments, int resultCode, Exception rootException, Message request)
			: base(message, arguments, resultCode, null, rootException)
		{
			this.request = request;
		}
	}
}
