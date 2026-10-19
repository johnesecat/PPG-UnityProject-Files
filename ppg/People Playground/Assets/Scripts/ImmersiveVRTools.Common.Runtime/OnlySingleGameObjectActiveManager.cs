using System.Collections.Generic;
using System.Linq;
using ImmersiveVRTools.Runtime.Common.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class OnlySingleGameObjectActiveManager : MonoBehaviour
{
	[SerializeField]
	[FormerlySerializedAs("_allGameObjects")]
	private List<Transform> _allTransforms;

	[SerializeField]
	private Transform _defaultActive;

	public void SetActiveByName(string name)
	{
		bool flag = false;
		foreach (Transform allTransform in _allTransforms)
		{
			if (allTransform.name == name)
			{
				allTransform.gameObject.SetActive(value: true);
				flag = true;
			}
			else
			{
				allTransform.gameObject.SetActive(value: false);
			}
		}
		if (!flag)
		{
			Debug.LogWarning("GameObject named: '" + name + "' was not found. Default one was be activated.");
			_defaultActive.gameObject.SetActive(value: true);
		}
	}

	private void Awake()
	{
		foreach (Transform allTransform in _allTransforms)
		{
			allTransform.gameObject.SetActive(allTransform == _defaultActive);
		}
	}

	private void Reset()
	{
		_allTransforms = base.transform.GetAllChildren().ToList();
	}
}
