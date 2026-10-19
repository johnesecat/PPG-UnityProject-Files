using System;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

internal class MonoBehaviourCallbackHooks : ComponentSingleton<MonoBehaviourCallbackHooks>
{
	internal Action<float> m_OnUpdateDelegate;

	public event Action<float> OnUpdateDelegate
	{
		add
		{
			m_OnUpdateDelegate = (Action<float>)Delegate.Combine(m_OnUpdateDelegate, value);
		}
		remove
		{
			m_OnUpdateDelegate = (Action<float>)Delegate.Remove(m_OnUpdateDelegate, value);
		}
	}

	protected override string GetGameObjectName()
	{
		return "ResourceManagerCallbacks";
	}

	internal void Update()
	{
		m_OnUpdateDelegate?.Invoke(Time.unscaledDeltaTime);
	}
}
