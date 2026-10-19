using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(1)]
	internal interface IXmlDocument : IXmlNode
	{
		[Nullable(2)]
		IXmlElement DocumentElement
		{
			[NullableContext(2)]
			get;
		}

		IXmlNode CreateComment([Nullable(2)] string text);

		IXmlNode CreateTextNode([Nullable(2)] string text);

		IXmlNode CreateCDataSection([Nullable(2)] string data);

		IXmlNode CreateWhitespace([Nullable(2)] string text);

		IXmlNode CreateSignificantWhitespace([Nullable(2)] string text);

		[NullableContext(2)]
		[return: Nullable(1)]
		IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone);

		[NullableContext(2)]
		[return: Nullable(1)]
		IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset);

		IXmlNode CreateProcessingInstruction(string target, [Nullable(2)] string data);

		IXmlElement CreateElement(string elementName);

		IXmlElement CreateElement(string qualifiedName, string namespaceUri);

		IXmlNode CreateAttribute(string name, [Nullable(2)] string value);

		IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, [Nullable(2)] string value);
	}
}
