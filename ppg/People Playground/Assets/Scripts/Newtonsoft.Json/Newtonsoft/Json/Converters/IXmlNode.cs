using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	internal interface IXmlNode
	{
		XmlNodeType NodeType { get; }

		string LocalName { get; }

		[Nullable(1)]
		List<IXmlNode> ChildNodes
		{
			[NullableContext(1)]
			get;
		}

		[Nullable(1)]
		List<IXmlNode> Attributes
		{
			[NullableContext(1)]
			get;
		}

		IXmlNode ParentNode { get; }

		string Value { get; set; }

		string NamespaceUri { get; }

		object WrappedNode { get; }

		[NullableContext(1)]
		IXmlNode AppendChild(IXmlNode newChild);
	}
}
