using System;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (!(UnityEngine.Object)_instance)
			{
				_instance = InitSingleton();
			}
			try
			{
				if (!_instance.gameObject.activeInHierarchy)
				{
					Debug.Log("T is disabled, enabling");
					_instance.gameObject.SetActive(value: true);
				}
			}
			catch (Exception)
			{
			}
			return _instance ?? (_instance = InitSingleton());
		}
	}

	private static T InitSingleton()
	{
		if ((UnityEngine.Object)_instance != (UnityEngine.Object)null)
		{
			Debug.LogWarning("Too many T - deleting this instance: '" + _instance.name + "'");
			_instance.enabled = false;
			UnityEngine.Object.DestroyImmediate(_instance);
			return null;
		}
		_instance = UnityEngine.Object.FindObjectOfType<T>();
		if (!(UnityEngine.Object)_instance)
		{
			_instance = new GameObject(typeof(T).Name).AddComponent<T>();
		}
		return _instance;
	}

	public T EnsureInitialized()
	{
		return Instance;
	}
}
