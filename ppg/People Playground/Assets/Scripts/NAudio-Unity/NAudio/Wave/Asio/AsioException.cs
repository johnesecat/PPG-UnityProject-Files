using System;

namespace NAudio.Wave.Asio
{
	internal class AsioException : global::System.Exception
	{
		private AsioError error;

		public AsioError Error
		{
			get
			{
				return error;
			}
			set
			{
				error = value;
				((global::System.Exception)this).get_Data().set_Item((object)"ASIOError", (object)error);
			}
		}

		public AsioException()
		{
		}

		public AsioException(string message)
			: base(message)
		{
		}

		public AsioException(string message, global::System.Exception innerException)
			: base(message, innerException)
		{
		}

		public static string getErrorName(AsioError error)
		{
			return global::System.Enum.GetName(typeof(AsioError), (object)error);
		}
	}
}
