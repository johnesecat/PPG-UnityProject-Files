using System;
using System.Collections;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

public abstract class ApplicationConfigBase<T> : ApplicationConfigBaseNonGeneric
{
	public const string ApplicationSettingsStreamingAssetsFilePath = "Config/application-settings.json";

	private static T _instance;

	protected static bool _isLoading;

	public static T Instance => _instance;

	public static void Get(MonoBehaviour coroutineRunner, Action<T> onSettingsResolved)
	{
		coroutineRunner.StartCoroutine(GetCoroutine(coroutineRunner, onSettingsResolved));
	}

	private static IEnumerator GetCoroutine(MonoBehaviour coroutineRunner, Action<T> onSettingsResolved)
	{
		do
		{
			yield return null;
		}
		while (_isLoading);
		if (_instance == null)
		{
			_isLoading = true;
			Debug.Log("ApplicationSettings: loading starts");
			CoroutineWithData<T> applicationSettingsCd = StreamingAssetsLoader.LoadJson<T>("Config/application-settings.json", coroutineRunner);
			yield return applicationSettingsCd.Coroutine;
			_instance = applicationSettingsCd.Result;
			ApplicationConfigBaseNonGeneric.IsInitialized = true;
			Debug.Log($"ApplicationSettings: loaded, result: {_instance}");
			try
			{
				ApplicationConfigBaseNonGeneric.InvokeSettingsInitializedEvent();
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error during {0} event. ex: {1}", "SettingsInitialized", arg));
			}
			_isLoading = false;
		}
		if (onSettingsResolved != null)
		{
			onSettingsResolved?.Invoke(_instance);
		}
	}
}
