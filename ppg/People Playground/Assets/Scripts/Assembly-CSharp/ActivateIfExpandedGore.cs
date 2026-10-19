using System;
using UnityEngine;

[Obsolete]
public class ActivateIfExpandedGore : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}
}
