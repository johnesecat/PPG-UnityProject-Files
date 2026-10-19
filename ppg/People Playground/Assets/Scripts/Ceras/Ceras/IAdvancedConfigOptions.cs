using System;

namespace Ceras
{
	public interface IAdvancedConfigOptions
	{
		Action<object> DiscardObjectMethod { get; set; }

		ReadonlyFieldHandling ReadonlyFieldHandling { get; set; }

		bool EmbedChecksum { get; set; }

		bool PersistTypeCache { get; set; }

		bool SealTypesWhenUsingKnownTypes { get; set; }

		bool SkipCompilerGeneratedFields { get; set; }

		ITypeBinder TypeBinder { get; set; }

		ISizeLimitsConfig SizeLimits { get; }

		DelegateSerializationFlags DelegateSerialization { get; set; }

		bool UseReinterpretFormatter { get; set; }

		bool RespectNonSerializedAttribute { get; set; }

		BitmapMode BitmapMode { get; set; }

		AotMode AotMode { get; set; }
	}
}
