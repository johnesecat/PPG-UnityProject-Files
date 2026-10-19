using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SteamTags
{
	public static readonly string[] Tags = new string[8] { "Machine", "Vehicle", "Building", "Destructible", "Realistic", "Fun", "Electronics", "Utility" };
}
