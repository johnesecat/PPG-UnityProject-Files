using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DestroyWhenAllChildrenDestroyed : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject[] Exclude;

	private Transform[] children;

	private int l;

	private void Awake()
	{
		children = GetComponentsInChildren<Transform>();
		l = children.Length;
		Transform[] array = children;
		foreach (Transform transform in array)
		{
			if (Exclude.Contains(transform.gameObject))
			{
				l--;
				continue;
			}
			ActOnDestroy orAddComponent = transform.gameObject.GetOrAddComponent<ActOnDestroy>();
			if (orAddComponent.Event == null)
			{
				orAddComponent.Event = new UnityEvent();
			}
			orAddComponent.Event.AddListener(Check);
		}
	}

	private void Check()
	{
		l--;
		if (l <= 1)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
