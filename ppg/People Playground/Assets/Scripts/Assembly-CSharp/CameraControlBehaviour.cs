using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlBehaviour : MonoBehaviour
{
	public float ZoomSpeed = 1f;

	private Vector3 oldMousePos;

	private Vector3 oldCamPos;

	private float targetZoom;

	private Vector3 targetPos;

	public float MinZoom = 0.2f;

	public float MaxZoom = 1000f;

	public Bounds BoundingBox = new Bounds(new Vector3(595f, 497f, 0f), new Vector3(1200f, 990f, 0f));

	public float Extend = 50f;

	public float KeyboardPanSpeedMultiplier = 4f;

	public float KeyboardZoomSpeedMultiplier = 10f;

	public List<PhysicalBehaviour> CurrentlyFollowing = new List<PhysicalBehaviour>();

	private FollowingViewBehaviour followingViewBehaviour;

	internal Vector3 MovementDelta;

	internal Vector3 LastPos;

	public event EventHandler PostRender;

	private void Start()
	{
		targetZoom = Global.main.camera.orthographicSize;
		followingViewBehaviour = UnityEngine.Object.FindObjectOfType<FollowingViewBehaviour>();
	}

	private void LateUpdate()
	{
		if (CurrentlyFollowing.Count > 0)
		{
			targetPos = GetAveragePosition();
		}
		targetPos.z = -10f;
		Vector3 mousePosition = Input.mousePosition;
		bool flag = mousePosition.x > 0f && mousePosition.y > 0f && mousePosition.x < (float)Screen.width && mousePosition.y < (float)Screen.height;
		bool flag2 = !Global.main.GetPausedMenu() && Application.isFocused && flag;
		Camera camera = Global.main.camera;
		targetZoom = Mathf.Clamp(targetZoom, MinZoom, MaxZoom);
		camera.orthographicSize = targetZoom;
		Global.CameraPosition = targetPos;
		if (!DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock && flag2)
		{
			if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f && !Global.main.UILock)
			{
				ZoomCamera(Input.mouseScrollDelta.y);
			}
			if (InputSystem.Down("pan"))
			{
				oldMousePos = Global.main.MousePosition;
				oldCamPos = Global.CameraPosition;
				CurrentlyFollowing.Clear();
			}
			if (InputSystem.Held("pan"))
			{
				Global.CameraPosition = oldCamPos;
				Vector3 vector = oldMousePos - camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Global.CameraPosition.z));
				Global.CameraPosition = oldCamPos + new Vector3(vector.x, vector.y, 0f);
				targetPos = Global.CameraPosition;
			}
			if (InputSystem.Held("panLeft"))
			{
				CurrentlyFollowing.Clear();
				targetPos += KeyboardPanSpeedMultiplier * targetZoom * Time.unscaledDeltaTime * Vector3.left;
				targetPos = clampPos(targetPos);
			}
			if (InputSystem.Held("panRight"))
			{
				CurrentlyFollowing.Clear();
				targetPos += KeyboardPanSpeedMultiplier * targetZoom * Time.unscaledDeltaTime * Vector3.right;
				targetPos = clampPos(targetPos);
			}
			if (InputSystem.Held("panUp"))
			{
				CurrentlyFollowing.Clear();
				targetPos += KeyboardPanSpeedMultiplier * targetZoom * Time.unscaledDeltaTime * Vector3.up;
				targetPos = clampPos(targetPos);
			}
			if (InputSystem.Held("panDown"))
			{
				CurrentlyFollowing.Clear();
				targetPos += KeyboardPanSpeedMultiplier * targetZoom * Time.unscaledDeltaTime * Vector3.down;
				targetPos = clampPos(targetPos);
			}
			if (InputSystem.Held("zoomIn"))
			{
				ZoomCamera(Time.unscaledDeltaTime * KeyboardZoomSpeedMultiplier, cursorAsOrigin: false);
			}
			if (InputSystem.Held("zoomOut"))
			{
				ZoomCamera((0f - Time.unscaledDeltaTime) * KeyboardZoomSpeedMultiplier, cursorAsOrigin: false);
			}
		}
		targetZoom = Mathf.Clamp(targetZoom, MinZoom, MaxZoom);
		followingViewBehaviour.gameObject.SetActive(CurrentlyFollowing.Count > 0);
		Global.CameraPosition = clampPos(Global.CameraPosition);
		Vector3 clampPos(Vector3 v)
		{
			return new Vector3(Mathf.Clamp(v.x, BoundingBox.center.x - BoundingBox.extents.x - Extend, BoundingBox.center.x + BoundingBox.extents.x + Extend), Mathf.Clamp(v.y, BoundingBox.center.y - BoundingBox.extents.y - Extend, BoundingBox.center.y + BoundingBox.extents.y + Extend), -10f);
		}
	}

	private void ZoomCamera(float i, bool cursorAsOrigin = true)
	{
		if (!DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock)
		{
			float num = i * ZoomSpeed * 0.1f * UserPreferenceManager.Current.ZoomSensitivity;
			targetZoom -= num * Global.main.camera.orthographicSize;
			if (targetZoom < MaxZoom && targetZoom > MinZoom && cursorAsOrigin)
			{
				Vector2 vector = Global.main.MousePosition - Global.CameraPosition;
				targetPos += (Vector3)vector * num;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!Global.main.Paused)
		{
			Vector3 position = base.transform.position;
			MovementDelta = position - LastPos;
			LastPos = position;
		}
	}

	private void Update()
	{
		if (Global.main.Paused)
		{
			MovementDelta = default(Vector3);
		}
	}

	private Vector3 GetAveragePosition()
	{
		Vector3 zero = Vector3.zero;
		int num = CurrentlyFollowing.Count;
		for (int i = 0; i < CurrentlyFollowing.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = CurrentlyFollowing[i];
			if (!physicalBehaviour || physicalBehaviour.isDisintegrated)
			{
				num--;
			}
			else
			{
				zero += physicalBehaviour.transform.position;
			}
		}
		if (num == 0)
		{
			CurrentlyFollowing.Clear();
			return targetPos;
		}
		return zero / num;
	}

	private void OnPostRender()
	{
		this.PostRender?.Invoke(this, EventArgs.Empty);
	}
}
