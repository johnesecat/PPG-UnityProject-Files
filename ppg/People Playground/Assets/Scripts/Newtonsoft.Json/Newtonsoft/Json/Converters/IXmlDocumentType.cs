using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(1)]
	internal interface IXmlDocumentType : IXmlNode
	{
		string Name { get; }

		string System { get; }

		string Public { get; }

		string InternalSubset { get; }
	}
}
