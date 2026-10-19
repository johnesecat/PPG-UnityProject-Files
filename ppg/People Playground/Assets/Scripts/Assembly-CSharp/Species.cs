using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Species
{
	public const string Human = "Human";

	public const string Gorse = "Gorse";

	public const string Android = "Android";

	public static readonly string[] AllSpecies = new string[3] { "Human", "Gorse", "Android" };
}
