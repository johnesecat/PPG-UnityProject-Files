namespace Steamworks
{
	internal enum AppType
	{
		Invalid = 0,
		Game = 1,
		Application = 2,
		Tool = 4,
		Demo = 8,
		Media_DEPRECATED = 0x10,
		DLC = 0x20,
		Guide = 0x40,
		Driver = 0x80,
		Config = 0x100,
		Hardware = 0x200,
		Franchise = 0x400,
		Video = 0x800,
		Plugin = 0x1000,
		MusicAlbum = 0x2000,
		Series = 0x4000,
		Comic_UNUSED = 0x8000,
		Beta = 0x10000,
		Shortcut = 0x40000000,
		DepotOnly = int.MinValue
	}
}
