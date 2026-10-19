using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class XmlDocumentWrapper : XmlNodeWrapper, IXmlDocument, IXmlNode
	{
		private readonly XmlDocument _document;

		[Nullable(2)]
		public IXmlElement DocumentElement
		{
			[NullableContext(2)]
			get
			{
				if (_document.DocumentElement == null)
				{
					return null;
				}
				return new XmlElementWrapper(_document.DocumentElement);
			}
		}

		public XmlDocumentWrapper(XmlDocument document)
			: base(document)
		{
			_document = document;
		}

		public IXmlNode CreateComment([Nullable(2)] string data)
		{
			return new XmlNodeWrapper(_document.CreateComment(data));
		}

		public IXmlNode CreateTextNode([Nullable(2)] string text)
		{
			return new XmlNodeWrapper(_document.CreateTextNode(text));
		}

		public IXmlNode CreateCDataSection([Nullable(2)] string data)
		{
			return new XmlNodeWrapper(_document.CreateCDataSection(data));
		}

		public IXmlNode CreateWhitespace([Nullable(2)] string text)
		{
			return new XmlNodeWrapper(_document.CreateWhitespace(text));
		}

		public IXmlNode CreateSignificantWhitespace([Nullable(2)] string text)
		{
			return new XmlNodeWrapper(_document.CreateSignificantWhitespace(text));
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XmlDeclarationWrapper(_document.CreateXmlDeclaration(version, encoding, standalone));
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XmlDocumentTypeWrapper(_document.CreateDocumentType(name, publicId, systemId, null));
		}

		public IXmlNode CreateProcessingInstruction(string target, [Nullable(2)] string data)
		{
			return new XmlNodeWrapper(_document.CreateProcessingInstruction(target, data));
		}

		public IXmlElement CreateElement(string elementName)
		{
			return new XmlElementWrapper(_document.CreateElement(elementName));
		}

		public IXmlElement CreateElement(string qualifiedName, string namespaceUri)
		{
			return new XmlElementWrapper(_document.CreateElement(qualifiedName, namespaceUri));
		}

		public IXmlNode CreateAttribute(string name, [Nullable(2)] string value)
		{
			return new XmlNodeWrapper(_document.CreateAttribute(name))
			{
				Value = value
			};
		}

		public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, [Nullable(2)] string value)
		{
			return new XmlNodeWrapper(_document.CreateAttribute(qualifiedName, namespaceUri))
			{
				Value = value
			};
		}
	}
}
