using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class JukeboxSongLoader : MonoBehaviour
{
	public static AudioClip[] CustomSongs;

	public string Path = "Jukebox";

	public string[] AllowedExtensions = new string[2] { ".wav", ".mp3" };

	public UnityEvent OnLoadStart;

	private static bool hasLoaded;

	private void Start()
	{
		OnLoadStart.Invoke();
		if (!hasLoaded)
		{
			hasLoaded = true;
			if (!Directory.Exists(Path))
			{
				Debug.Log(Path + " not found. Can't load custom jukebox files.");
				CustomSongs = new AudioClip[0];
			}
			else
			{
				FileInfo[] files = LoadFiles();
				CustomSongs = ExtractData(files);
			}
		}
	}

	private AudioClip[] ExtractData(FileInfo[] files)
	{
		List<AudioClip> list = new List<AudioClip>();
		foreach (FileInfo fileInfo in files)
		{
			try
			{
				AudioClip item = Utils.FileToAudioClip(fileInfo.FullName);
				list.Add(item);
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Could not load audio file: " + fileInfo.FullName);
				Debug.LogException(exception);
			}
		}
		return list.ToArray();
	}

	private FileInfo[] LoadFiles()
	{
		return (from f in new DirectoryInfo(Path).GetFiles()
			where AllowedExtensions.Contains(f.Extension)
			select f).ToArray();
	}
}
