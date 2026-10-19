using System;
using UnityEngine;
using UnityEngine.Events;

public class ActOnDisintegrate : MonoBehaviour
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public UnityEvent Event;

	private void Awake()
	{
		PhysicalBehaviour.OnDisintegration += OnDisintegration;
	}

	private void OnDisintegration(object sender, EventArgs e)
	{
		Event?.Invoke();
	}

	private void OnDestroy()
	{
		PhysicalBehaviour.OnDisintegration -= OnDisintegration;
	}
}
