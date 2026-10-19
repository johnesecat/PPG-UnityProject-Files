using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class JointSerialisationHelper : MonoBehaviour, Messages.IOnAfterDeserialise
{
	[SkipSerialisation]
	public Joint2D Joint;

	[ReadOnly]
	public bool Broken;

	private void Start()
	{
		if (Broken)
		{
			Object.Destroy(Joint);
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (joint == Joint)
		{
			Broken = true;
		}
	}

	public void OnAfterDeserialise(List<GameObject> gameObjects)
	{
		if (Broken)
		{
			Object.Destroy(Joint);
		}
	}
}
