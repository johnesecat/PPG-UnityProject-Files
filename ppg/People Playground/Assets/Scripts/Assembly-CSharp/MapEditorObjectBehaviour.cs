using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorObjectBehaviour : MonoBehaviour
{
	[NonSerialized]
	public MapEditorSelectable Selectable;

	[NonSerialized]
	public List<ContextMenuButton> ContextMenuButtons = new List<ContextMenuButton>();

	private void Awake()
	{
		Selectable = GetComponent<MapEditorSelectable>();
	}

	public void Delete()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
