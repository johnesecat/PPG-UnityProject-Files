namespace Ceras.Formatters
{
	public delegate void SerializeDelegate<T>(ref byte[] buffer, ref int offset, T value);
}
