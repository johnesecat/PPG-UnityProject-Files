using System;
using System.Diagnostics;
using UnityEngine;

public class URLOpenBehaviour : MonoBehaviour
{
	public void OpenURL(string url)
	{
		Utils.OpenURL(url);
	}

	public void OpenLocal(string path)
	{
		Process.Start(Environment.CurrentDirectory + path);
	}
}
