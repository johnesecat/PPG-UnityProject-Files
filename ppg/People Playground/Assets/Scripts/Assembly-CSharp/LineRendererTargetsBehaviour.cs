using UnityEngine;

public class LineRendererTargetsBehaviour : MonoBehaviour
{
	public LineRenderer LineRenderer;

	public Transform[] Targets;

	private Vector3[] points;

	private void Update()
	{
		if (!LineRenderer || Targets == null || Targets.Length < 2)
		{
			return;
		}
		if (LineRenderer.positionCount != Targets.Length)
		{
			LineRenderer.positionCount = Targets.Length;
		}
		if (points == null || points.Length != Targets.Length)
		{
			points = new Vector3[Targets.Length];
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			Vector3 vector = Targets[i].position;
			if (!LineRenderer.useWorldSpace)
			{
				vector = LineRenderer.transform.InverseTransformPoint(vector);
			}
			points[i] = vector;
		}
		LineRenderer.SetPositions(points);
	}
}
