using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace Novell.Directory.Ldap.Utilclass
{
	public class ResourcesHandler
	{
		private static ResourceManager defaultResultCodes;

		private static ResourceManager defaultMessages;

		private static string pkg;

		private static CultureInfo defaultLocale;

		private ResourcesHandler()
		{
		}

		public static string getMessage(string messageOrKey, object[] arguments)
		{
			return getMessage(messageOrKey, arguments, null);
		}

		public static string getMessage(string messageOrKey, object[] arguments, CultureInfo locale)
		{
			if (defaultMessages == null)
			{
				defaultMessages = new ResourceManager("ExceptionMessages", Assembly.GetExecutingAssembly());
			}
			if (defaultLocale == null)
			{
				defaultLocale = Thread.CurrentThread.CurrentUICulture;
			}
			if (locale == null)
			{
				locale = defaultLocale;
			}
			if (messageOrKey == null)
			{
				messageOrKey = "";
			}
			string text;
			try
			{
				text = defaultMessages.GetString(messageOrKey, locale);
			}
			catch (MissingManifestResourceException)
			{
				text = messageOrKey;
			}
			if (arguments != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat(text, arguments);
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static string getResultString(int code)
		{
			return getResultString(code, null);
		}

		public static string getResultString(int code, CultureInfo locale)
		{
			if (defaultResultCodes == null)
			{
				defaultResultCodes = new ResourceManager("ResultCodeMessages", Assembly.GetExecutingAssembly());
			}
			if (defaultLocale == null)
			{
				defaultLocale = Thread.CurrentThread.CurrentUICulture;
			}
			if (locale == null)
			{
				locale = defaultLocale;
			}
			try
			{
				return defaultResultCodes.GetString(Convert.ToString(code), defaultLocale);
			}
			catch (ArgumentNullException)
			{
				return getMessage("UNKNOWN_RESULT", new object[1] { code }, locale);
			}
		}

		static ResourcesHandler()
		{
			defaultResultCodes = null;
			defaultMessages = null;
			pkg = "Novell.Directory.Ldap.Utilclass.";
			defaultLocale = Thread.CurrentThread.CurrentUICulture;
		}
	}
}
