using System;
using UnityEngine;

public class DisintegrationCounterBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public int DisintegrationCount;

	private PhysicalBehaviour[] children;

	private void Start()
	{
		children = GetComponentsInChildren<PhysicalBehaviour>();
		for (int i = 0; i < children.Length; i++)
		{
			PhysicalBehaviour physicalBehaviour = children[i];
			if ((bool)physicalBehaviour)
			{
				physicalBehaviour.OnDisintegration += OnChildDisintegrated;
			}
		}
	}

	private void OnDestroy()
	{
		if (children == null)
		{
			return;
		}
		for (int i = 0; i < children.Length; i++)
		{
			PhysicalBehaviour physicalBehaviour = children[i];
			if ((bool)physicalBehaviour)
			{
				physicalBehaviour.OnDisintegration -= OnChildDisintegrated;
			}
		}
	}

	public void RegisterPseudoChild(PhysicalBehaviour phys)
	{
		Array.Resize(ref children, children.Length + 1);
		children[children.Length - 1] = phys;
		if ((bool)phys)
		{
			phys.OnDisintegration += OnChildDisintegrated;
		}
	}

	private void OnChildDisintegrated(object sender, EventArgs e)
	{
		DisintegrationCount++;
		int num = 0;
		if (children != null)
		{
			for (int i = 0; i < children.Length; i++)
			{
				if ((bool)children[i])
				{
					num++;
				}
			}
		}
		if (DisintegrationCount >= num)
		{
			UnityEngine.Object.Destroy(base.transform.root.gameObject);
		}
	}
}
