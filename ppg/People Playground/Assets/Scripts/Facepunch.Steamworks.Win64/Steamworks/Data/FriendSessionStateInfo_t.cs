using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct FriendSessionStateInfo_t
	{
		internal uint IOnlineSessionInstances;

		internal byte IPublishedToFriendsSessionInstance;
	}
}
