using System;
using System.Collections.Generic;
using UnityEngine;

public class ModDebugDrawer
{
	internal Stack<Action> Actions;

	public ModDebugDrawer(Stack<Action> actions)
	{
		Actions = actions;
	}

	public void Line(Vector3 from, Vector3 to)
	{
		Actions.Push(delegate
		{
			GL.Begin(1);
			GL.Vertex(from);
			GL.Vertex(to);
			GL.End();
		});
	}

	public void Rect(Vector3 center, Vector3 size)
	{
		Vector3 vector = size / 2f;
		Vector3 topleft = center + new Vector3(0f - vector.x, vector.y);
		Vector3 topright = center + new Vector3(vector.x, vector.y);
		Vector3 bottomright = center + new Vector3(vector.x, 0f - vector.y);
		Vector3 bottomleft = center + new Vector3(0f - vector.x, 0f - vector.y);
		Actions.Push(delegate
		{
			GL.Begin(1);
			GL.Vertex(topleft);
			GL.Vertex(topright);
			GL.Vertex(topright);
			GL.Vertex(bottomright);
			GL.Vertex(bottomright);
			GL.Vertex(bottomleft);
			GL.Vertex(bottomleft);
			GL.Vertex(topleft);
			GL.End();
		});
	}

	public void Circle(Vector3 center, float radius)
	{
		Actions.Push(delegate
		{
			GL.Begin(2);
			GL.Vertex(new Vector3(radius, 0f) + center);
			for (int i = 1; i < 64; i++)
			{
				GL.Vertex(Utils.Rotate(new Vector2(radius, 0f), 5.625f * (float)i) + center);
			}
			GL.Vertex(new Vector3(radius, 0f) + center);
			GL.End();
		});
	}

	public void Collider(Collider2D collider)
	{
		if (!collider)
		{
			return;
		}
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
					if ((object)circleCollider2D == null)
					{
						return;
					}
					_ = circleCollider2D.transform.position;
					float radius = circleCollider2D.radius * Mathf.Max(Mathf.Abs(circleCollider2D.transform.lossyScale.x), Mathf.Abs(circleCollider2D.transform.lossyScale.y));
					Actions.Push(delegate
					{
						GL.Begin(2);
						GL.Vertex(new Vector3(radius, 0f) + circleCollider2D.transform.TransformPoint(circleCollider2D.offset));
						for (int l = 1; l < 64; l++)
						{
							GL.Vertex(Utils.Rotate(new Vector2(radius, 0f), 5.625f * (float)l) + circleCollider2D.transform.TransformPoint(circleCollider2D.offset));
						}
						GL.Vertex(new Vector3(radius, 0f) + circleCollider2D.transform.TransformPoint(circleCollider2D.offset));
						GL.End();
					});
				}
				else
				{
					Vector2[] points = edgeCollider2D.points;
					for (int i = 0; i < edgeCollider2D.pointCount - 1; i++)
					{
						Line((Vector4)edgeCollider2D.transform.position + collider.transform.localToWorldMatrix * points[i], (Vector4)edgeCollider2D.transform.position + collider.transform.localToWorldMatrix * points[i + 1]);
					}
				}
				return;
			}
			for (int j = 0; j < polygonCollider2D.pathCount; j++)
			{
				Vector4 vector = polygonCollider2D.transform.position;
				Vector2[] path = polygonCollider2D.GetPath(j);
				for (int k = 0; k < path.Length - 1; k++)
				{
					Line(vector + collider.transform.localToWorldMatrix * (path[k] + polygonCollider2D.offset), vector + collider.transform.localToWorldMatrix * (path[k + 1] + polygonCollider2D.offset));
				}
				Line(vector + collider.transform.localToWorldMatrix * (path[path.Length - 1] + polygonCollider2D.offset), vector + collider.transform.localToWorldMatrix * (path[0] + polygonCollider2D.offset));
			}
			return;
		}
		Vector4 pos = boxCollider2D.transform.position;
		Vector2 vector2 = boxCollider2D.size / 2f;
		Vector2 topleft = boxCollider2D.offset + new Vector2(0f - vector2.x, vector2.y);
		Vector2 topright = boxCollider2D.offset + new Vector2(vector2.x, vector2.y);
		Vector2 bottomright = boxCollider2D.offset + new Vector2(vector2.x, 0f - vector2.y);
		Vector2 bottomleft = boxCollider2D.offset + new Vector2(0f - vector2.x, 0f - vector2.y);
		Actions.Push(delegate
		{
			if ((bool)collider)
			{
				GL.Begin(1);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * topleft);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * topright);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * topright);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * bottomright);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * bottomright);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * bottomleft);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * bottomleft);
				GL.Vertex(pos + collider.transform.localToWorldMatrix * topleft);
				GL.End();
			}
		});
	}
}
