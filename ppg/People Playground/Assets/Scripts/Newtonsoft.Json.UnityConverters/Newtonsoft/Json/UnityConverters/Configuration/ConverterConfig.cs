using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Newtonsoft.Json.UnityConverters.Configuration
{
	[Serializable]
	[DebuggerDisplay("{converterName}, enabled={enabled}")]
	public struct ConverterConfig : IEquatable<ConverterConfig>
	{
		public bool enabled;

		public string converterName;

		public List<KeyedConfig> settings;

		public override string ToString()
		{
			return $"{{enabled={enabled}, converterName={converterName}, settings=[{settings?.Count ?? 0}]}}";
		}

		public override bool Equals(object obj)
		{
			if (obj is ConverterConfig)
			{
				ConverterConfig other = (ConverterConfig)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals(ConverterConfig other)
		{
			if (enabled == other.enabled && converterName == other.converterName)
			{
				return EqualityComparer<List<KeyedConfig>>.Default.Equals(settings, other.settings);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((1016066258 * -1521134295 + enabled.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(converterName)) * -1521134295 + EqualityComparer<List<KeyedConfig>>.Default.GetHashCode(settings);
		}

		public static bool operator ==(ConverterConfig left, ConverterConfig right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ConverterConfig left, ConverterConfig right)
		{
			return !(left == right);
		}
	}
}
