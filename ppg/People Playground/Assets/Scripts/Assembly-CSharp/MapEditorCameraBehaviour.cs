using UnityEngine;

public class MapEditorCameraBehaviour : MonoBehaviour
{
	public float ZoomSpeed = 1f;

	public float MinZoom = 0.2f;

	public float MaxZoom = 1000f;

	public Bounds BoundingBox = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(25f, 25f, 0f));

	public float KeyboardPanSpeedMultiplier = 4f;

	public float KeyboardZoomSpeedMultiplier = 10f;

	private Camera Camera;

	private Vector2 oldWorldMousePos;

	private Vector3 targetPos;

	private float targetZoom = 5f;

	private void Awake()
	{
		Camera = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		targetPos = base.transform.position;
		targetZoom = Camera.orthographicSize;
		MapProperties mapProperties = MapEditorGlobal.Instance.MapProperties;
		ProcessMouseInput();
		ProcessKeyboardInput();
		Vector2 maxInUnits = mapProperties.MapBounds.GetMaxInUnits();
		Vector2 minInUnits = mapProperties.MapBounds.GetMinInUnits();
		Vector3 vector = BoundingBox.size * 15f;
		targetPos.x = Mathf.Clamp(targetPos.x, minInUnits.x - vector.x, maxInUnits.x + vector.x);
		targetPos.y = Mathf.Clamp(targetPos.y, minInUnits.y - vector.y, maxInUnits.y + vector.y);
		base.transform.position = new Vector3(targetPos.x, targetPos.y, -10f);
		Camera.orthographicSize = Mathf.Clamp(targetZoom, MinZoom, MaxZoom);
	}

	private void ProcessMouseInput()
	{
		if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f)
		{
			ZoomCamera(Input.mouseScrollDelta.y);
		}
		if (InputSystem.Down("pan"))
		{
			oldWorldMousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
		}
		if (InputSystem.Held("pan"))
		{
			targetPos += (Vector3)oldWorldMousePos - Camera.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	private void ProcessKeyboardInput()
	{
		float num = Time.unscaledDeltaTime * KeyboardPanSpeedMultiplier * targetZoom;
		if (InputSystem.Held("panLeft"))
		{
			targetPos += Vector3.left * num;
		}
		if (InputSystem.Held("panRight"))
		{
			targetPos += Vector3.right * num;
		}
		if (InputSystem.Held("panUp"))
		{
			targetPos += Vector3.up * num;
		}
		if (InputSystem.Held("panDown"))
		{
			targetPos += Vector3.down * num;
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

	private void ZoomCamera(float i, bool cursorAsOrigin = true)
	{
		float num = i * ZoomSpeed * 0.1f * UserPreferenceManager.Current.ZoomSensitivity;
		targetZoom -= num * Camera.orthographicSize;
		if (targetZoom < MaxZoom && targetZoom > MinZoom && cursorAsOrigin)
		{
			Vector3 vector = Camera.ScreenToWorldPoint(Input.mousePosition) - base.transform.position;
			targetPos += vector * num;
		}
	}
}
