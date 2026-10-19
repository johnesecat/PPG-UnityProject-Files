using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorSelectable : MonoBehaviour
{
	internal static HashSet<MapEditorSelectable> Selectables = new HashSet<MapEditorSelectable>();

	public int Depth;

	[NonSerialized]
	public Bounds LocalBounds;

	[NonSerialized]
	public bool IsSelected;

	private Collider2D[] colliders;

	internal int selectionAge;

	private void Awake()
	{
		colliders = GetComponentsInChildren<Collider2D>();
		Selectables.Add(this);
	}

	private void Start()
	{
		MapEditorSelectionManager.Instance.UpdateSortedSelectables();
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 localScale = base.transform.localScale;
		base.transform.SetPositionAndRotation(default(Vector3), Quaternion.identity);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		LocalBounds = new Bounds(base.transform.position, default(Vector3));
		Collider2D[] array = colliders;
		foreach (Collider2D collider2D in array)
		{
			LocalBounds.Encapsulate(collider2D.bounds);
		}
		LocalBounds.center = default(Vector3);
		base.transform.SetPositionAndRotation(position, rotation);
		base.transform.localScale = localScale;
	}

	public bool MouseOverlaps()
	{
		Vector2 worldMousePos = MapEditorGlobal.Instance.WorldMousePos;
		Collider2D[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].OverlapPoint(worldMousePos))
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerable<Collider2D> GetColliders()
	{
		return colliders;
	}

	private void OnDestroy()
	{
		Selectables.Remove(this);
		MapEditorSelectionManager instance = MapEditorSelectionManager.Instance;
		instance.CurrentlySelected.Remove(this);
		instance.OnDeselect?.Invoke(this);
		instance.UpdateSortedSelectables();
	}

	public void OnSelect()
	{
	}

	public void OnDeselect()
	{
		selectionAge = 0;
	}
}
