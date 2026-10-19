namespace Ceras
{
	internal sealed class NullPool : ICerasBufferPool
	{
		internal static readonly NullPool Instance = new NullPool();

		private NullPool()
		{
		}

		public byte[] RentBuffer(int minimumSize)
		{
			return new byte[minimumSize];
		}

		public void Return(byte[] buffer)
		{
		}
	}
}
