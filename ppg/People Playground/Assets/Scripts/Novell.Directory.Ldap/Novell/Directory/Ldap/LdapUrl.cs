using System;
using System.Collections;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapUrl : ICloneable
	{
		private static readonly int DEFAULT_SCOPE;

		private bool secure;

		private bool ipV6;

		private string host;

		private int port;

		private string dn;

		private string[] attrs;

		private string filter;

		private int scope;

		private string[] extensions;

		public virtual string[] AttributeArray => attrs;

		public virtual IEnumerator Attributes => new ArrayEnumeration(attrs);

		public virtual string[] Extensions => extensions;

		public virtual string Filter => filter;

		public virtual string Host => host;

		public virtual int Port
		{
			get
			{
				if (port == 0)
				{
					return 389;
				}
				return port;
			}
		}

		public virtual int Scope => scope;

		public virtual bool Secure => secure;

		private void InitBlock()
		{
			scope = DEFAULT_SCOPE;
		}

		public LdapUrl(string url)
		{
			InitBlock();
			parseURL(url);
		}

		public LdapUrl(string host, int port, string dn)
		{
			InitBlock();
			this.host = host;
			this.port = port;
			this.dn = dn;
		}

		public LdapUrl(string host, int port, string dn, string[] attrNames, int scope, string filter, string[] extensions)
		{
			InitBlock();
			this.host = host;
			this.port = port;
			this.dn = dn;
			attrs = new string[attrNames.Length];
			attrNames.CopyTo(attrs, 0);
			this.scope = scope;
			this.filter = filter;
			this.extensions = new string[extensions.Length];
			extensions.CopyTo(this.extensions, 0);
		}

		public LdapUrl(string host, int port, string dn, string[] attrNames, int scope, string filter, string[] extensions, bool secure)
		{
			InitBlock();
			this.host = host;
			this.port = port;
			this.dn = dn;
			attrs = attrNames;
			this.scope = scope;
			this.filter = filter;
			this.extensions = new string[extensions.Length];
			extensions.CopyTo(this.extensions, 0);
			this.secure = secure;
		}

		public object Clone()
		{
			try
			{
				return MemberwiseClone();
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
		}

		public static string decode(string URLEncoded)
		{
			int startIndex = 0;
			int num = URLEncoded.IndexOf("%", startIndex);
			if (num < 0)
			{
				return URLEncoded;
			}
			int num2 = 0;
			int length = URLEncoded.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			while (true)
			{
				if (num > length - 3)
				{
					throw new UriFormatException("LdapUrl.decode: must be two hex characters following escape character '%'");
				}
				if (num < 0)
				{
					num = length;
				}
				stringBuilder.Append(URLEncoded.Substring(num2, num - num2));
				num++;
				if (num >= length)
				{
					break;
				}
				num2 = num + 2;
				try
				{
					stringBuilder.Append((char)Convert.ToInt32(URLEncoded.Substring(num, num2 - num), 16));
				}
				catch (FormatException ex)
				{
					throw new UriFormatException("LdapUrl.decode: error converting hex characters to integer \"" + ex.Message + "\"");
				}
				startIndex = num2;
				if (startIndex == length)
				{
					break;
				}
				num = URLEncoded.IndexOf("%", startIndex);
			}
			return stringBuilder.ToString();
		}

		public static string encode(string toEncode)
		{
			StringBuilder stringBuilder = new StringBuilder(toEncode.Length);
			foreach (char c in toEncode)
			{
				if (c <= '\u001f' || c == '\u007f' || (c >= '\u0080' && c <= 'ÿ') || c == '<' || c == '>' || c == '"' || c == '#' || c == '%' || c == '{' || c == '}' || c == '|' || c == '\\' || c == '^' || c == '~' || c == '[' || c == '\'' || c == ';' || c == '/' || c == '?' || c == ':' || c == '@' || c == '=' || c == '&')
				{
					string text = Convert.ToString(c, 16);
					if (text.Length == 1)
					{
						stringBuilder.Append("%0" + text);
					}
					else
					{
						stringBuilder.Append("%" + Convert.ToString(c, 16));
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public virtual string getDN()
		{
			return dn;
		}

		internal virtual void setDN(string dn)
		{
			this.dn = dn;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (secure)
			{
				stringBuilder.Append("ldaps://");
			}
			else
			{
				stringBuilder.Append("ldap://");
			}
			if (ipV6)
			{
				stringBuilder.Append("[" + host + "]");
			}
			else
			{
				stringBuilder.Append(host);
			}
			if (port != 0)
			{
				stringBuilder.Append(":" + port);
			}
			if (dn == null && attrs == null && scope == DEFAULT_SCOPE && filter == null && extensions == null)
			{
				return stringBuilder.ToString();
			}
			stringBuilder.Append("/");
			if (dn != null)
			{
				stringBuilder.Append(dn);
			}
			if (attrs == null && scope == DEFAULT_SCOPE && filter == null && extensions == null)
			{
				return stringBuilder.ToString();
			}
			stringBuilder.Append("?");
			if (attrs != null)
			{
				for (int i = 0; i < attrs.Length; i++)
				{
					stringBuilder.Append(attrs[i]);
					if (i < attrs.Length - 1)
					{
						stringBuilder.Append(",");
					}
				}
			}
			if (scope == DEFAULT_SCOPE && filter == null && extensions == null)
			{
				return stringBuilder.ToString();
			}
			stringBuilder.Append("?");
			if (scope != DEFAULT_SCOPE)
			{
				if (scope == 1)
				{
					stringBuilder.Append("one");
				}
				else
				{
					stringBuilder.Append("sub");
				}
			}
			if (filter == null && extensions == null)
			{
				return stringBuilder.ToString();
			}
			if (filter == null)
			{
				stringBuilder.Append("?");
			}
			else
			{
				stringBuilder.Append("?" + Filter);
			}
			if (extensions == null)
			{
				return stringBuilder.ToString();
			}
			stringBuilder.Append("?");
			if (extensions != null)
			{
				for (int j = 0; j < extensions.Length; j++)
				{
					stringBuilder.Append(extensions[j]);
					if (j < extensions.Length - 1)
					{
						stringBuilder.Append(",");
					}
				}
			}
			return stringBuilder.ToString();
		}

		private string[] parseList(string listStr, char delimiter, int listStart, int listEnd)
		{
			if (listEnd - listStart < 1)
			{
				return null;
			}
			int num = listStart;
			int num2 = 0;
			while (num > 0)
			{
				num2++;
				int num3 = listStr.IndexOf(delimiter, num);
				if (num3 <= 0 || num3 >= listEnd)
				{
					break;
				}
				num = num3 + 1;
			}
			num = listStart;
			string[] array = new string[num2];
			num2 = 0;
			while (num > 0)
			{
				int num3 = listStr.IndexOf(delimiter, num);
				if (num > listEnd)
				{
					break;
				}
				if (num3 < 0)
				{
					num3 = listEnd;
				}
				if (num3 > listEnd)
				{
					num3 = listEnd;
				}
				array[num2] = listStr.Substring(num, num3 - num);
				num = num3 + 1;
				num2++;
			}
			return array;
		}

		private void parseURL(string url)
		{
			int num = 0;
			int num2 = url.Length;
			if (url == null)
			{
				throw new UriFormatException("LdapUrl: URL cannot be null");
			}
			if (url[num] == '<')
			{
				if (url[num2 - 1] != '>')
				{
					throw new UriFormatException("LdapUrl: URL bad enclosure");
				}
				num++;
				num2--;
			}
			if (url.Substring(num, num + 4 - num).ToUpper().Equals("URL:".ToUpper()))
			{
				num += 4;
			}
			if (url.Substring(num, num + 7 - num).ToUpper().Equals("ldap://".ToUpper()))
			{
				num += 7;
				port = 389;
			}
			else
			{
				if (!url.Substring(num, num + 8 - num).ToUpper().Equals("ldaps://".ToUpper()))
				{
					throw new UriFormatException("LdapUrl: URL scheme is not ldap");
				}
				secure = true;
				num += 8;
				port = 636;
			}
			int num3 = url.IndexOf("/", num);
			int num4 = num2;
			bool flag = false;
			if (num3 < 0)
			{
				num3 = url.IndexOf("?", num);
				if (num3 > 0)
				{
					if (url[num3 + 1] == '?')
					{
						num4 = num3;
						num3++;
						flag = true;
					}
					else
					{
						num3 = -1;
					}
				}
			}
			else
			{
				num4 = num3;
			}
			int num5 = num4;
			if (url[num] == '[')
			{
				num5 = url.IndexOf(']', num + 1);
				if (num5 >= num4 || num5 == -1)
				{
					throw new UriFormatException("LdapUrl: \"]\" is missing on IPV6 host name");
				}
				host = url.Substring(num + 1, num5 - (num + 1));
				int num6 = url.IndexOf(":", num5);
				if (num6 < num4 && num6 != -1)
				{
					port = int.Parse(url.Substring(num6 + 1, num4 - (num6 + 1)));
				}
			}
			else
			{
				int num6 = url.IndexOf(":", num);
				if (num6 < 0 || num6 > num4)
				{
					host = url.Substring(num, num4 - num);
				}
				else
				{
					host = url.Substring(num, num6 - num);
					port = int.Parse(url.Substring(num6 + 1, num4 - (num6 + 1)));
				}
			}
			num = num4 + 1;
			if (num >= num2 || num3 < 0)
			{
				return;
			}
			num = num3 + 1;
			int num7 = url.IndexOf('?', num);
			if (num7 < 0)
			{
				dn = url.Substring(num, num2 - num);
			}
			else
			{
				dn = url.Substring(num, num7 - num);
			}
			num = num7 + 1;
			if (num >= num2 || num7 < 0 || flag)
			{
				return;
			}
			int num8 = url.IndexOf('?', num);
			if (num8 < 0)
			{
				num8 = num2 - 1;
			}
			attrs = parseList(url, ',', num7 + 1, num8);
			num = num8 + 1;
			if (num >= num2)
			{
				return;
			}
			int num9 = url.IndexOf('?', num);
			string text = ((num9 >= 0) ? url.Substring(num, num9 - num) : url.Substring(num, num2 - num));
			if (text.ToUpper().Equals("".ToUpper()))
			{
				scope = 0;
			}
			else if (text.ToUpper().Equals("base".ToUpper()))
			{
				scope = 0;
			}
			else if (text.ToUpper().Equals("one".ToUpper()))
			{
				scope = 1;
			}
			else
			{
				if (!text.ToUpper().Equals("sub".ToUpper()))
				{
					throw new UriFormatException("LdapUrl: URL invalid scope");
				}
				scope = 2;
			}
			num = num9 + 1;
			if (num >= num2 || num9 < 0)
			{
				return;
			}
			num = num9 + 1;
			int num10 = url.IndexOf('?', num);
			string text2 = ((num10 >= 0) ? url.Substring(num, num10 - num) : url.Substring(num, num2 - num));
			if (!text2.Equals(""))
			{
				filter = text2;
			}
			num = num10 + 1;
			if (num < num2 && num10 >= 0)
			{
				if (url.IndexOf('?', num) > 0)
				{
					throw new UriFormatException("LdapUrl: URL has too many ? fields");
				}
				extensions = parseList(url, ',', num, num2);
			}
		}
	}
}
