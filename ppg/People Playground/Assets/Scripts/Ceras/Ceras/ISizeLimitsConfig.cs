namespace Ceras
{
	public interface ISizeLimitsConfig
	{
		uint MaxStringLength { get; set; }

		uint MaxArraySize { get; set; }

		uint MaxByteArraySize { get; set; }

		uint MaxCollectionSize { get; set; }
	}
}
