namespace Ceras.Formatters
{
	internal interface ISchemaTaintedFormatter
	{
		void OnSchemaChanged(TypeMetaData meta);
	}
}
