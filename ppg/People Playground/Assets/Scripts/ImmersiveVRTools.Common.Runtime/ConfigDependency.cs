using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class ConfigDependency : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour _dependentComponent;

	[SerializeField]
	private bool _enableOnlyOnceConfigResolved;

	[SerializeField]
	private bool _disableWholeGameObject;

	private void Awake()
	{
		if (!_dependentComponent)
		{
			Debug.LogWarning("No _dependentComponent for ConfigDependency. (this may not be intentional when running on server)");
			return;
		}
		if (!ApplicationConfigBaseNonGeneric.IsInitialized)
		{
			ApplicationConfigBaseNonGeneric.SettingsInitialized += delegate
			{
				HandleSettingsInitialized();
			};
		}
		else
		{
			StartCoroutine(HandleSettingsInitializedDelayed());
		}
		if (_enableOnlyOnceConfigResolved)
		{
			_dependentComponent.enabled = false;
		}
		if (_disableWholeGameObject)
		{
			_dependentComponent.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator HandleSettingsInitializedDelayed()
	{
		yield return null;
		HandleSettingsInitialized();
	}

	private void HandleSettingsInitialized()
	{
		if (_enableOnlyOnceConfigResolved)
		{
			_dependentComponent.enabled = true;
		}
		if (_disableWholeGameObject)
		{
			_dependentComponent.gameObject.SetActive(value: true);
		}
	}
}
