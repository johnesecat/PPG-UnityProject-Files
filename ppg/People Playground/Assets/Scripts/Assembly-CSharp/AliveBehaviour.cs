using System.Collections.Generic;
using UnityEngine;

public abstract class AliveBehaviour : MonoBehaviour
{
	public static Dictionary<Transform, AliveBehaviour> AliveByTransform = new Dictionary<Transform, AliveBehaviour>();

	public abstract bool IsAlive();

	public void AddToDictionary()
	{
		AliveByTransform.Add(base.transform, this);
	}

	public void RemoveFromDictionary()
	{
		AliveByTransform.Remove(base.transform);
	}
}
