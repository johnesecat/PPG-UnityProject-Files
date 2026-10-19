using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourManager<T> : MonoBehaviour where T : IManagedBehaviour
{
	protected abstract IList<T> GetCollection();

	protected virtual void Update()
	{
		IList<T> collection = GetCollection();
		for (int i = 0; i < collection.Count; i++)
		{
			T val = collection[i];
			if (val != null && val.ShouldUpdate())
			{
				val.ManagedUpdate();
			}
		}
	}

	protected virtual void FixedUpdate()
	{
		IList<T> collection = GetCollection();
		for (int i = 0; i < collection.Count; i++)
		{
			T val = collection[i];
			if (val != null && val.ShouldUpdate())
			{
				val.ManagedFixedUpdate();
			}
		}
	}

	protected virtual void LateUpdate()
	{
		IList<T> collection = GetCollection();
		for (int i = 0; i < collection.Count; i++)
		{
			T val = collection[i];
			if (val != null && val.ShouldUpdate())
			{
				val.ManagedLateUpdate();
			}
		}
	}
}
