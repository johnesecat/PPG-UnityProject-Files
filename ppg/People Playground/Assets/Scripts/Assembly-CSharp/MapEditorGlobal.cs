using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditorGlobal : MonoBehaviour
{
	public static MapEditorGlobal Instance;

	public GameObject PauseMenu;

	public GameObject Catalog;

	public EventSystem EventSystem;

	public MapEditorContextMenuBehaviour ContextMenu;

	public MapProperties MapProperties;

	private int uiLockingRegistrations;

	public bool IsPauseMenuOpen => PauseMenu.activeSelf;

	public bool IsCatalogOpen => Catalog.activeSelf;

	public Vector2 WorldMousePos { get; private set; }

	public Camera Camera { get; private set; }

	public bool UiLock
	{
		get
		{
			if (uiLockingRegistrations == 0 && !EventSystem.IsPointerOverGameObject() && !DialogBox.IsAnyDialogboxOpen)
			{
				return ScreenUIEventEmitter.IsMouseOverAny();
			}
			return true;
		}
	}

	public void Awake()
	{
		Instance = this;
		InputSystem.Load();
		InputSystem.CurrentUniverse = ActionUniverse.MapEditor;
	}

	private void Start()
	{
		Camera = Camera.main;
	}

	private void Update()
	{
		if (!IsPauseMenuOpen)
		{
			WorldMousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
			if (InputSystem.Down("pause"))
			{
				PauseMenu.SetActive(value: true);
			}
			if (InputSystem.Down("toybox"))
			{
				Catalog.SetActive(!IsCatalogOpen);
			}
		}
	}

	public void RegisterUILock()
	{
		uiLockingRegistrations++;
	}

	public void DeregisterUILock()
	{
		uiLockingRegistrations--;
		if (uiLockingRegistrations < 0)
		{
			throw new Exception("Somehow deregistered an unregistered UI lock");
		}
	}
}
