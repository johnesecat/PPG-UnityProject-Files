using System;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras.Resolvers
{
	public class DynamicObjectFormatterResolver : IFormatterResolver
	{
		private CerasSerializer _ceras;

		private VersionToleranceMode _versionToleranceMode;

		public DynamicObjectFormatterResolver(CerasSerializer ceras)
		{
			_ceras = ceras;
			_versionToleranceMode = ceras.Config.VersionTolerance.Mode;
		}

		public IFormatter GetFormatter(Type type)
		{
			if (_ceras.Config.Advanced.AotMode == AotMode.Enabled)
			{
				throw new InvalidOperationException("No formatter for the Type '" + type.FullName + "' was found. Ceras is trying to fall back to the DynamicFormatter, but that formatter will never work in on AoT compiled platforms. Use the code generator tool to automatically generate a formatter for this type.");
			}
			TypeMetaData typeMetaData = _ceras.GetTypeMetaData(type);
			if (typeMetaData.IsPrimitive)
			{
				throw new InvalidOperationException("DynamicFormatter is not allowed to serialize serialization-primitives.");
			}
			if ((_versionToleranceMode == VersionToleranceMode.Standard && !typeMetaData.IsFrameworkType) || (_versionToleranceMode == VersionToleranceMode.Extended && typeMetaData.IsFrameworkType))
			{
				return (IFormatter)Activator.CreateInstance(typeof(SchemaDynamicFormatter<>).MakeGenericType(type), _ceras, typeMetaData.PrimarySchema, false);
			}
			return (IFormatter)Activator.CreateInstance(typeof(DynamicFormatter<>).MakeGenericType(type), _ceras, false);
		}
	}
}
