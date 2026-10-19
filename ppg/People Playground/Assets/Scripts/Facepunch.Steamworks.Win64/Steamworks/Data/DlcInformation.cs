using System.Runtime.CompilerServices;

namespace Steamworks.Data
{
	public struct DlcInformation
	{
		public AppId AppId
		{
			[IsReadOnly]
			get;
			internal set; }

		public string Name
		{
			[IsReadOnly]
			get;
			internal set; }

		public bool Available
		{
			[IsReadOnly]
			get;
			internal set; }
	}
}
