using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class StreamingAssetsLoader
	{
		public static CoroutineWithData<TType> LoadJson<TType>(string filePathStreamingAssetFolderRelative, MonoBehaviour coroutineRunner)
		{
			return new CoroutineWithData<TType>(coroutineRunner, ParseJsonResultToType<TType>(filePathStreamingAssetFolderRelative, coroutineRunner));
		}

		private static IEnumerator ParseJsonResultToType<TType>(string filePathStreamingAssetFolderRelative, MonoBehaviour coroutineRunner)
		{
			CoroutineWithData<string> loadText = LoadText(filePathStreamingAssetFolderRelative, coroutineRunner);
			yield return loadText.Coroutine;
			yield return JsonUtility.FromJson<TType>(loadText.Result);
		}

		public static CoroutineWithData<string> LoadText(string filePathStreamingAssetFolderRelative, MonoBehaviour coroutineRunner)
		{
			string text = Path.Combine(Application.streamingAssetsPath, filePathStreamingAssetFolderRelative);
			if (Application.platform == RuntimePlatform.Android)
			{
				return new CoroutineWithData<string>(coroutineRunner, GetStreamingAssetTextAndroid(text));
			}
			return new CoroutineWithData<string>(coroutineRunner, GetStreamingAssetTextStandard(text.Replace("\\", "/")));
		}

		private static IEnumerator GetStreamingAssetTextStandard(string filePath)
		{
			yield return File.ReadAllText(filePath);
		}

		private static IEnumerator GetStreamingAssetTextAndroid(string filePath)
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
			yield return webRequest.SendWebRequest();
			yield return webRequest.downloadHandler.text;
		}
	}
}
