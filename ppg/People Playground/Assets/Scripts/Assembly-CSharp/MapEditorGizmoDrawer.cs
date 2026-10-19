using UnityEngine;

public class MapEditorGizmoDrawer : MonoBehaviour
{
	public MapEditorManipulator Manipulator;

	public GameObject MoveGizmo;

	private Material fadeInDistance;

	private Material simpleColored;

	private void Start()
	{
		fadeInDistance = new Material(Shader.Find("Hidden/MapEditorGizmoShader"));
		simpleColored = new Material(Shader.Find("Hidden/Internal-Colored"));
	}

	private void OnPostRender()
	{
		GL.PushMatrix();
		if (fadeInDistance.SetPass(0))
		{
			GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
			DrawMapBounds();
			DrawGrid();
			if (simpleColored.SetPass(0))
			{
				DrawTransformGizmoLines();
				DrawSelectionOutlines();
				GL.PopMatrix();
			}
		}
	}

	private void DrawTransformGizmoLines()
	{
		if (MoveGizmo.activeSelf)
		{
			DrawLine(MoveGizmo.transform.position, Manipulator.CenterOfSelection, Color.white);
		}
	}

	private void DrawSelectionOutlines()
	{
		Color color = Color.yellow;
		foreach (MapEditorSelectable item in MapEditorSelectionManager.Instance.CurrentlySelected)
		{
			foreach (Collider2D collider in item.GetColliders())
			{
				drawCollider(collider, color);
			}
		}
		foreach (MapEditorSelectable item2 in MapEditorSelectionManager.Instance.CurrentlySelected)
		{
			DrawBounds(item2, Color.white);
		}
		if (!MapEditorGlobal.Instance.UiLock && (bool)MapEditorSelectionManager.Instance.TopMostHovering)
		{
			DrawBounds(MapEditorSelectionManager.Instance.TopMostHovering, Color.green);
		}
		void drawCollider(Collider2D collider, Color colorToDrawWith)
		{
			if ((bool)collider)
			{
				BoxCollider2D boxCollider2D = collider as BoxCollider2D;
				if ((object)boxCollider2D == null)
				{
					PolygonCollider2D polygonCollider2D = collider as PolygonCollider2D;
					if ((object)polygonCollider2D == null)
					{
						EdgeCollider2D edgeCollider2D = collider as EdgeCollider2D;
						if ((object)edgeCollider2D == null)
						{
							CircleCollider2D circleCollider2D = collider as CircleCollider2D;
							if ((object)circleCollider2D != null)
							{
								_ = circleCollider2D.transform.position;
								float x = circleCollider2D.radius * Mathf.Max(Mathf.Abs(circleCollider2D.transform.lossyScale.x), Mathf.Abs(circleCollider2D.transform.lossyScale.y));
								GL.Begin(2);
								GL.Color(colorToDrawWith);
								GL.Vertex(new Vector3(x, 0f) + circleCollider2D.transform.TransformPoint(circleCollider2D.offset));
								for (int i = 1; i < 64; i++)
								{
									GL.Vertex(Utils.Rotate(new Vector2(x, 0f), 5.625f * (float)i) + circleCollider2D.transform.TransformPoint(circleCollider2D.offset));
								}
								GL.Vertex(new Vector3(x, 0f) + circleCollider2D.transform.TransformPoint(circleCollider2D.offset));
								GL.End();
							}
						}
						else
						{
							Vector2[] points = edgeCollider2D.points;
							for (int j = 0; j < edgeCollider2D.pointCount - 1; j++)
							{
								DrawLine((Vector4)edgeCollider2D.transform.position + collider.transform.localToWorldMatrix * points[j], (Vector4)edgeCollider2D.transform.position + collider.transform.localToWorldMatrix * points[j + 1], color);
							}
						}
					}
					else
					{
						for (int k = 0; k < polygonCollider2D.pathCount; k++)
						{
							Vector4 vector = polygonCollider2D.transform.position;
							Vector2[] path = polygonCollider2D.GetPath(k);
							for (int l = 0; l < path.Length - 1; l++)
							{
								DrawLine(vector + collider.transform.localToWorldMatrix * (path[l] + polygonCollider2D.offset), vector + collider.transform.localToWorldMatrix * (path[l + 1] + polygonCollider2D.offset), color);
							}
							DrawLine(vector + collider.transform.localToWorldMatrix * (path[path.Length - 1] + polygonCollider2D.offset), vector + collider.transform.localToWorldMatrix * (path[0] + polygonCollider2D.offset), color);
						}
					}
				}
				else
				{
					Vector4 vector2 = boxCollider2D.transform.position;
					Vector2 vector3 = boxCollider2D.size / 2f;
					Vector2 vector4 = boxCollider2D.offset + new Vector2(0f - vector3.x, vector3.y);
					Vector2 vector5 = boxCollider2D.offset + new Vector2(vector3.x, vector3.y);
					Vector2 vector6 = boxCollider2D.offset + new Vector2(vector3.x, 0f - vector3.y);
					Vector2 vector7 = boxCollider2D.offset + new Vector2(0f - vector3.x, 0f - vector3.y);
					if ((bool)collider)
					{
						GL.Begin(1);
						GL.Color(colorToDrawWith);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector4);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector5);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector5);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector6);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector6);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector7);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector7);
						GL.Vertex(vector2 + collider.transform.localToWorldMatrix * vector4);
						GL.End();
					}
				}
			}
		}
	}

	private static void DrawBounds(MapEditorSelectable item, Color color)
	{
		GL.PushMatrix();
		GL.MultMatrix(item.transform.localToWorldMatrix);
		GL.Begin(1);
		GL.Color(color);
		Vector2 vector = new Vector2(item.LocalBounds.min.x, item.LocalBounds.min.y);
		Vector2 vector2 = new Vector2(item.LocalBounds.max.x, item.LocalBounds.min.y);
		Vector2 vector3 = new Vector2(item.LocalBounds.min.x, item.LocalBounds.max.y);
		Vector2 vector4 = new Vector2(item.LocalBounds.max.x, item.LocalBounds.max.y);
		GL.Vertex(vector);
		GL.Vertex(vector2);
		GL.Vertex(vector2);
		GL.Vertex(vector4);
		GL.Vertex(vector4);
		GL.Vertex(vector3);
		GL.Vertex(vector3);
		GL.Vertex(vector);
		GL.End();
		GL.PopMatrix();
	}

	private static void DrawLine(Vector2 a, Vector2 b, Color color)
	{
		GL.Begin(1);
		GL.Color(color);
		GL.Vertex(a);
		GL.Vertex(b);
		GL.End();
	}

	private void DrawGrid()
	{
		MapBounds mapBounds = MapEditorGlobal.Instance.MapProperties.MapBounds;
		Vector2 centerInUnits = mapBounds.GetCenterInUnits();
		Vector2 sizeInUnits = mapBounds.GetSizeInUnits();
		Vector2 vector = centerInUnits - sizeInUnits / 2f;
		Vector2 vector2 = centerInUnits + sizeInUnits / 2f;
		GL.Begin(1);
		GL.Color(new Color(1f, 1f, 1f, 0.05f));
		int x = mapBounds.BoundsSizeInMeters.x;
		int y = mapBounds.BoundsSizeInMeters.y;
		for (int i = 0; i < x; i++)
		{
			float x2 = (float)i / (220f / 267f) + centerInUnits.x - sizeInUnits.x / 2f;
			GL.Vertex3(x2, vector2.y, 0f);
			GL.Vertex3(x2, vector.y, 0f);
		}
		for (int j = 0; j < y; j++)
		{
			float y2 = (float)j / (220f / 267f) + centerInUnits.y - sizeInUnits.y / 2f;
			GL.Vertex3(vector2.x, y2, 0f);
			GL.Vertex3(vector.x, y2, 0f);
		}
		GL.End();
	}

	private static void DrawMapBounds()
	{
		MapBounds mapBounds = MapEditorGlobal.Instance.MapProperties.MapBounds;
		Vector2 maxInUnits = mapBounds.GetMaxInUnits();
		Vector2 minInUnits = mapBounds.GetMinInUnits();
		GL.Begin(2);
		GL.Color(new Color(1f, 1f, 1f, 1f));
		GL.Vertex3(minInUnits.x, minInUnits.y, 0f);
		GL.Vertex3(maxInUnits.x, minInUnits.y, 0f);
		GL.Vertex3(maxInUnits.x, maxInUnits.y, 0f);
		GL.Vertex3(minInUnits.x, maxInUnits.y, 0f);
		GL.Vertex3(minInUnits.x, minInUnits.y, 0f);
		GL.End();
	}
}
