using System;
using UnityEngine;

[Serializable]
public class BellClip
{
	public string Name;

	public PianoKey Key;

	public int Octave = 1;

	[Space]
	public AudioClip Clip;

	public string GetDisplayName(bool shorten = false)
	{
		if (Name.Contains("Sharp"))
		{
			return shorten ? Key.ToString().Substring(0, 6).Replace("Sharp", "♯\n") : Key.ToString().Replace("Sharp", "♯\n").Replace("Flat", "♭");
		}
		return Key.ToString().Trim();
	}
}
