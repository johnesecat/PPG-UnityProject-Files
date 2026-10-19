namespace Steamworks
{
	internal enum NetIdentityType
	{
		Invalid = 0,
		SteamID = 0x10,
		XboxPairwiseID = 17,
		IPAddress = 1,
		GenericString = 2,
		GenericBytes = 3,
		UnknownType = 4,
		Force32bit = int.MaxValue
	}
}
