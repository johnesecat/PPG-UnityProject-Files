namespace Ceras
{
	public interface IVersionToleranceConfig
	{
		VersionToleranceMode Mode { get; set; }

		bool VerifySizes { get; set; }
	}
}
