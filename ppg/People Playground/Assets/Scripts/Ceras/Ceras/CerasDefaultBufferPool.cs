using System.Buffers;

namespace Ceras
{
	public class CerasDefaultBufferPool : ICerasBufferPool
	{
		public byte[] RentBuffer(int minimumSize)
		{
			return ArrayPool<byte>.Shared.Rent(minimumSize);
		}

		public void Return(byte[] buffer)
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}
}
