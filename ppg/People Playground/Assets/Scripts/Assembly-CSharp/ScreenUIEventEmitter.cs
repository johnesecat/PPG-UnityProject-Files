using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenUIEventEmitter : MonoBehaviour
{
	private static HashSet<ScreenUIEventEmitter> screenUIEventEmitters = new HashSet<ScreenUIEventEmitter>();

	public string MouseDownControlCode = "drag";

	public bool IncludeChildren;

	[Space]
	public UnityEvent OnMouseEnter;

	public UnityEvent OnMouseExit;

	public UnityEvent OnMouseDown;

	[NonSerialized]
	public bool IsMouseInside;

	[NonSerialized]
	public bool IsBeingDragged;

	private Camera cam;

	private Collider2D[] colliders;

	private void Awake()
	{
		screenUIEventEmitters.Add(this);
		colliders = (IncludeChildren ? GetComponentsInChildren<Collider2D>() : GetComponents<Collider2D>());
		cam = Camera.main;
	}

	private void OnDestroy()
	{
		screenUIEventEmitters.Remove(this);
	}

	public static bool IsAny(Func<ScreenUIEventEmitter, bool> predicate)
	{
		foreach (ScreenUIEventEmitter screenUIEventEmitter in screenUIEventEmitters)
		{
			if ((bool)screenUIEventEmitter && predicate(screenUIEventEmitter))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMouseOverAny()
	{
		foreach (ScreenUIEventEmitter screenUIEventEmitter in screenUIEventEmitters)
		{
			if ((bool)screenUIEventEmitter && screenUIEventEmitter.gameObject.activeInHierarchy && screenUIEventEmitter.IsMouseInside)
			{
				return true;
			}
		}
		return false;
	}

	private void LateUpdate()
	{
		Vector3 vector = cam.ScreenToWorldPoint(Input.mousePosition);
		bool isMouseInside = IsMouseInside;
		IsMouseInside = false;
		if (!IsMouseOverAny())
		{
			Collider2D[] array = colliders;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].OverlapPoint(vector))
				{
					IsMouseInside = true;
					break;
				}
			}
		}
		if (!isMouseInside && IsMouseInside)
		{
			OnMouseEnter?.Invoke();
		}
		if (isMouseInside && !IsMouseInside)
		{
			OnMouseExit?.Invoke();
		}
		if (IsMouseInside && InputSystem.Down(MouseDownControlCode))
		{
			IsBeingDragged = true;
			OnMouseDown?.Invoke();
		}
		if (IsBeingDragged && InputSystem.Up(MouseDownControlCode))
		{
			IsBeingDragged = false;
		}
	}

	private void OnDisable()
	{
		IsBeingDragged = false;
		if (IsMouseInside)
		{
			OnMouseExit?.Invoke();
		}
		IsMouseInside = false;
	}
}
