using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(2)]
	[Nullable(0)]
	public class JsonObjectContract : JsonContainerContract
	{
		internal bool ExtensionDataIsJToken;

		private bool? _hasRequiredOrDefaultValueProperties;

		[Nullable(new byte[] { 2, 1 })]
		private ObjectConstructor<object> _overrideCreator;

		[Nullable(new byte[] { 2, 1 })]
		private ObjectConstructor<object> _parameterizedCreator;

		private JsonPropertyCollection _creatorParameters;

		private Type _extensionDataValueType;

		public MemberSerialization MemberSerialization { get; set; }

		public MissingMemberHandling? MissingMemberHandling { get; set; }

		public Required? ItemRequired { get; set; }

		public NullValueHandling? ItemNullValueHandling { get; set; }

		[Nullable(1)]
		[field: Nullable(1)]
		public JsonPropertyCollection Properties
		{
			[NullableContext(1)]
			get;
		}

		[Nullable(1)]
		public JsonPropertyCollection CreatorParameters
		{
			[NullableContext(1)]
			get
			{
				if (_creatorParameters == null)
				{
					_creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
				}
				return _creatorParameters;
			}
		}

		[Nullable(new byte[] { 2, 1 })]
		public ObjectConstructor<object> OverrideCreator
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get
			{
				return _overrideCreator;
			}
			[param: Nullable(new byte[] { 2, 1 })]
			set
			{
				_overrideCreator = value;
			}
		}

		[Nullable(new byte[] { 2, 1 })]
		internal ObjectConstructor<object> ParameterizedCreator
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get
			{
				return _parameterizedCreator;
			}
			[param: Nullable(new byte[] { 2, 1 })]
			set
			{
				_parameterizedCreator = value;
			}
		}

		public ExtensionDataSetter ExtensionDataSetter { get; set; }

		public ExtensionDataGetter ExtensionDataGetter { get; set; }

		public Type ExtensionDataValueType
		{
			get
			{
				return _extensionDataValueType;
			}
			set
			{
				_extensionDataValueType = value;
				ExtensionDataIsJToken = value != null && typeof(JToken).IsAssignableFrom(value);
			}
		}

		[Nullable(new byte[] { 2, 1, 1 })]
		[field: Nullable(new byte[] { 2, 1, 1 })]
		public Func<string, string> ExtensionDataNameResolver
		{
			[return: Nullable(new byte[] { 2, 1, 1 })]
			get;
			[param: Nullable(new byte[] { 2, 1, 1 })]
			set;
		}

		internal bool HasRequiredOrDefaultValueProperties
		{
			get
			{
				if (!_hasRequiredOrDefaultValueProperties.HasValue)
				{
					_hasRequiredOrDefaultValueProperties = false;
					if (ItemRequired.GetValueOrDefault(Required.Default) != 0)
					{
						_hasRequiredOrDefaultValueProperties = true;
					}
					else
					{
						foreach (JsonProperty property in Properties)
						{
							if (property.Required != 0 || ((uint?)property.DefaultValueHandling & 2u) == 2)
							{
								_hasRequiredOrDefaultValueProperties = true;
								break;
							}
						}
					}
				}
				return _hasRequiredOrDefaultValueProperties.GetValueOrDefault();
			}
		}

		[NullableContext(1)]
		public JsonObjectContract(Type underlyingType)
			: base(underlyingType)
		{
			ContractType = JsonContractType.Object;
			Properties = new JsonPropertyCollection(base.UnderlyingType);
		}

		[NullableContext(1)]
		[SecuritySafeCritical]
		internal object GetUninitializedObject()
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, NonNullableUnderlyingType));
			}
			return FormatterServices.GetUninitializedObject(NonNullableUnderlyingType);
		}
	}
}
