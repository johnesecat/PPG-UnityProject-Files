namespace Ceras.Formatters
{
	public delegate void SerializeDelegate2<TFormatterContainer, TValue>(TFormatterContainer formatters, ref byte[] buffer, ref int offset, TValue value);
}
