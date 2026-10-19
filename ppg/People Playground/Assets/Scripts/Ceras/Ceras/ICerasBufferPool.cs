namespace Ceras
{
	public interface ICerasBufferPool
	{
		byte[] RentBuffer(int minimumSize);

		void Return(byte[] buffer);
	}
}
