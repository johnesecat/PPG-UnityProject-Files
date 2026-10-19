using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(1)]
	internal interface IXmlElement : IXmlNode
	{
		bool IsEmpty { get; }

		void SetAttributeNode(IXmlNode attribute);

		string GetPrefixOfNamespace(string namespaceUri);
	}
}
