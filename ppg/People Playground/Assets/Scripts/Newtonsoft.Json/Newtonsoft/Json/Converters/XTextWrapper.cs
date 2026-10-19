using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class XTextWrapper : XObjectWrapper
	{
		[Nullable(1)]
		private XText Text
		{
			[NullableContext(1)]
			get
			{
				return (XText)base.WrappedNode;
			}
		}

		public override string Value
		{
			get
			{
				return Text.Value;
			}
			set
			{
				Text.Value = value;
			}
		}

		public override IXmlNode ParentNode
		{
			get
			{
				if (Text.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(Text.Parent);
			}
		}

		[NullableContext(1)]
		public XTextWrapper(XText text)
			: base(text)
		{
		}
	}
}
