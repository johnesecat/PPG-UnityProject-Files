namespace Steamworks
{
	internal enum FriendFlags
	{
		None = 0,
		Blocked = 1,
		FriendshipRequested = 2,
		Immediate = 4,
		ClanMember = 8,
		OnGameServer = 0x10,
		RequestingFriendship = 0x80,
		RequestingInfo = 0x100,
		Ignored = 0x200,
		IgnoredFriend = 0x400,
		ChatMember = 0x1000,
		All = 0xFFFF
	}
}
