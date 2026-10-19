using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class XmlNodeWrapper : IXmlNode
	{
		[Nullable(1)]
		private readonly XmlNode _node;

		[Nullable(new byte[] { 2, 1 })]
		private List<IXmlNode> _childNodes;

		[Nullable(new byte[] { 2, 1 })]
		private List<IXmlNode> _attributes;

		public object WrappedNode => _node;

		public XmlNodeType NodeType => _node.NodeType;

		public virtual string LocalName => _node.LocalName;

		[Nullable(1)]
		public List<IXmlNode> ChildNodes
		{
			[NullableContext(1)]
			get
			{
				if (_childNodes == null)
				{
					if (!_node.HasChildNodes)
					{
						_childNodes = XmlNodeConverter.EmptyChildNodes;
					}
					else
					{
						_childNodes = new List<IXmlNode>(_node.ChildNodes.Count);
						foreach (XmlNode childNode in _node.ChildNodes)
						{
							_childNodes.Add(WrapNode(childNode));
						}
					}
				}
				return _childNodes;
			}
		}

		protected virtual bool HasChildNodes => _node.HasChildNodes;

		[Nullable(1)]
		public List<IXmlNode> Attributes
		{
			[NullableContext(1)]
			get
			{
				if (_attributes == null)
				{
					if (!HasAttributes)
					{
						_attributes = XmlNodeConverter.EmptyChildNodes;
					}
					else
					{
						_attributes = new List<IXmlNode>(_node.Attributes.Count);
						foreach (XmlAttribute attribute in _node.Attributes)
						{
							_attributes.Add(WrapNode(attribute));
						}
					}
				}
				return _attributes;
			}
		}

		private bool HasAttributes
		{
			get
			{
				XmlElement xmlElement = _node as XmlElement;
				if (xmlElement != null)
				{
					return xmlElement.HasAttributes;
				}
				XmlAttributeCollection attributes = _node.Attributes;
				if (attributes == null)
				{
					return false;
				}
				return attributes.Count > 0;
			}
		}

		public IXmlNode ParentNode
		{
			get
			{
				XmlAttribute xmlAttribute = _node as XmlAttribute;
				XmlNode xmlNode = ((xmlAttribute != null) ? xmlAttribute.OwnerElement : _node.ParentNode);
				if (xmlNode == null)
				{
					return null;
				}
				return WrapNode(xmlNode);
			}
		}

		public string Value
		{
			get
			{
				return _node.Value;
			}
			set
			{
				_node.Value = value;
			}
		}

		public string NamespaceUri => _node.NamespaceURI;

		[NullableContext(1)]
		public XmlNodeWrapper(XmlNode node)
		{
			_node = node;
		}

		[NullableContext(1)]
		internal static IXmlNode WrapNode(XmlNode node)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
				return new XmlElementWrapper((XmlElement)node);
			case XmlNodeType.XmlDeclaration:
				return new XmlDeclarationWrapper((XmlDeclaration)node);
			case XmlNodeType.DocumentType:
				return new XmlDocumentTypeWrapper((XmlDocumentType)node);
			default:
				return new XmlNodeWrapper(node);
			}
		}

		[NullableContext(1)]
		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			_node.AppendChild(xmlNodeWrapper._node);
			_childNodes = null;
			_attributes = null;
			return newChild;
		}
	}
}
