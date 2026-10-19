using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class XAttributeWrapper : XObjectWrapper
	{
		[Nullable(1)]
		private XAttribute Attribute
		{
			[NullableContext(1)]
			get
			{
				return (XAttribute)base.WrappedNode;
			}
		}

		public override string Value
		{
			get
			{
				return Attribute.Value;
			}
			set
			{
				Attribute.Value = value;
			}
		}

		public override string LocalName => Attribute.Name.LocalName;

		public override string NamespaceUri => Attribute.Name.NamespaceName;

		public override IXmlNode ParentNode
		{
			get
			{
				if (Attribute.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(Attribute.Parent);
			}
		}

		[NullableContext(1)]
		public XAttributeWrapper(XAttribute attribute)
			: base(attribute)
		{
		}
	}
}
