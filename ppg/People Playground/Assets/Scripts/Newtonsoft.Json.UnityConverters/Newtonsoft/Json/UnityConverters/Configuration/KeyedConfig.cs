using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Configuration
{
	[Serializable]
	public struct KeyedConfig : IEquatable<KeyedConfig>
	{
		public string key;

		public ConfigType type;

		public bool boolean;

		public int integer;

		public float number;

		public string text;

		public override bool Equals(object obj)
		{
			if (obj is KeyedConfig)
			{
				KeyedConfig other = (KeyedConfig)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals(KeyedConfig other)
		{
			if (type != other.type)
			{
				return false;
			}
			switch (type)
			{
			case ConfigType.Boolean:
				return boolean == other.boolean;
			case ConfigType.Integer:
				return integer == other.integer;
			case ConfigType.Number:
				return Mathf.Approximately(number, other.number);
			case ConfigType.Text:
				return text == other.text;
			default:
				return false;
			}
		}

		public override int GetHashCode()
		{
			return (((((910641971 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(key)) * -1521134295 + type.GetHashCode()) * -1521134295 + boolean.GetHashCode()) * -1521134295 + integer.GetHashCode()) * -1521134295 + number.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(text);
		}

		public static bool operator ==(KeyedConfig left, KeyedConfig right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(KeyedConfig left, KeyedConfig right)
		{
			return !(left == right);
		}
	}
}
