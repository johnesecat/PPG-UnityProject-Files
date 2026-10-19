using UnityEngine;

public class TargetLineRenderer : MonoBehaviour
{
	public LineRenderer LineRenderer;

	public Transform Target;

	private void Awake()
	{
		LineRenderer.useWorldSpace = false;
		LineRenderer.positionCount = 2;
		LineRenderer.SetPosition(0, Vector2.zero);
		Update();
	}

	private void Update()
	{
		LineRenderer.SetPosition(1, base.transform.InverseTransformPoint(Target.position));
	}
}
