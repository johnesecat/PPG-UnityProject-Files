namespace Ceras.Formatters
{
	public delegate void DeserializeDelegate<T>(byte[] buffer, ref int offset, ref T value);
}
