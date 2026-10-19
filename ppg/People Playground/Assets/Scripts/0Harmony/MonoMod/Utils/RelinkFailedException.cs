using System;
using System.Text;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal class RelinkFailedException : Exception
	{
		public const string DefaultMessage = "MonoMod failed relinking";

		public IMetadataTokenProvider MTP;

		public IMetadataTokenProvider Context;

		public RelinkFailedException(IMetadataTokenProvider mtp, IMetadataTokenProvider context = null)
			: this(_Format("MonoMod failed relinking", mtp, context), mtp, context)
		{
		}

		public RelinkFailedException(string message, IMetadataTokenProvider mtp, IMetadataTokenProvider context = null)
			: base(message)
		{
			MTP = mtp;
			Context = context;
		}

		public RelinkFailedException(string message, Exception innerException, IMetadataTokenProvider mtp, IMetadataTokenProvider context = null)
			: base(message ?? _Format("MonoMod failed relinking", mtp, context), innerException)
		{
			MTP = mtp;
			Context = context;
		}

		protected static string _Format(string message, IMetadataTokenProvider mtp, IMetadataTokenProvider context)
		{
			if (mtp == null && context == null)
			{
				return message;
			}
			StringBuilder stringBuilder = new StringBuilder(message);
			stringBuilder.Append(" ");
			if (mtp != null)
			{
				stringBuilder.Append(mtp.ToString());
			}
			if (context != null)
			{
				stringBuilder.Append(" ");
			}
			if (context != null)
			{
				stringBuilder.Append("(context: ").Append(context.ToString()).Append(")");
			}
			return stringBuilder.ToString();
		}
	}
}
