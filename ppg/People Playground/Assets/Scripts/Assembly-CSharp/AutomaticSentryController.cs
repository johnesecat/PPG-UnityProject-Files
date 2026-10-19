using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[SkipSerialisation]
public class AutomaticSentryController
{
	public float SightRange = 20f;

	public UnityEvent OnShoot = new UnityEvent();

	public UnityEvent OnSight = new UnityEvent();

	public LayerMask LayerMask;

	public float AimFuzziness = 0.0005f;

	private readonly List<LimbBehaviour> targetStack = new List<LimbBehaviour>();

	private static readonly RaycastHit2D[] losBuffer = new RaycastHit2D[5];

	private readonly Collider2D[] buffer = new Collider2D[256];

	private LimbBehaviour currentTarget;

	private float t;

	private int previousPersonID;

	private const float interval = 0.5f;

	public float MaxAngle = 180f;

	public bool HasTarget => currentTarget;

	public LimbBehaviour Target => currentTarget;

	public AutomaticSentryController()
	{
		t = 0f;
	}

	public void GetTargetMotorSpeed(Transform transform, Vector2 worldspaceBarrelPosition, Vector2 worldspaceBarrelDirection, out float motorSpeed)
	{
		motorSpeed = 0f;
		if (t > 0.5f && !currentTarget)
		{
			t = 0f;
			for (int i = 0; i < Physics2D.OverlapCircleNonAlloc(worldspaceBarrelPosition, SightRange, buffer, LayerMask); i++)
			{
				Collider2D collider2D = buffer[i];
				if (!(collider2D.transform.root == transform.root) && collider2D.TryGetComponent<LimbBehaviour>(out var component) && !targetStack.Contains(component) && component.IsConsideredAlive && HasLineOfSight(component, transform, worldspaceBarrelPosition, worldspaceBarrelDirection))
				{
					if (component.RoughClassification == LimbBehaviour.BodyPart.Torso)
					{
						targetStack.Add(component);
					}
					else
					{
						targetStack.Insert(0, component);
					}
				}
			}
		}
		else
		{
			t += Time.deltaTime;
		}
		if (targetStack.Count == 0)
		{
			return;
		}
		if (!currentTarget)
		{
			currentTarget = targetStack.Last();
			if (!currentTarget)
			{
				skip();
				return;
			}
		}
		if (currentTarget.IsConsideredAlive && Vector2.Distance(currentTarget.transform.position, transform.position) < SightRange)
		{
			if (!HasLineOfSight(currentTarget, transform, worldspaceBarrelPosition, worldspaceBarrelDirection))
			{
				skip();
			}
			else
			{
				motorSpeed = GetMotorSpeed(transform, worldspaceBarrelDirection);
			}
		}
		else
		{
			skip();
		}
		void skip()
		{
			if ((bool)currentTarget)
			{
				Debug.DrawLine(currentTarget.transform.position, transform.position, Color.red);
			}
			targetStack.RemoveAt(targetStack.Count - 1);
			currentTarget = null;
		}
	}

	public void ResetTargets()
	{
		currentTarget = null;
		targetStack.Clear();
	}

	private bool HasLineOfSight(LimbBehaviour t, Transform transform, Vector2 worldspacePos, Vector2 barrelDir)
	{
		Vector2 vector = (Vector2)t.transform.position - worldspacePos;
		Debug.DrawLine(worldspacePos, worldspacePos + vector * 10f, Color.yellow);
		if (Vector2.Angle(vector, barrelDir) > MaxAngle)
		{
			return false;
		}
		int num = Physics2D.LinecastNonAlloc(worldspacePos, t.transform.position, losBuffer, LayerMask);
		if (num == 0)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit2D raycastHit2D = losBuffer[i];
			if (raycastHit2D.transform.root != t.transform.root && raycastHit2D.transform.root != transform.root && Global.main.PhysicalObjectsInWorldByTransform[raycastHit2D.transform].AbsorbsLasers)
			{
				return false;
			}
		}
		return true;
	}

	private float GetMotorSpeed(Transform transform, Vector2 worldspaceDir)
	{
		int instanceID = currentTarget.Person.GetInstanceID();
		if (previousPersonID != instanceID)
		{
			OnSight?.Invoke();
			previousPersonID = instanceID;
		}
		Vector2 vector = (currentTarget.transform.position - transform.position).normalized;
		if (Vector2.Dot(vector, worldspaceDir) > 1f - AimFuzziness)
		{
			OnShoot?.Invoke();
		}
		return Mathf.Clamp(0f - Vector2.SignedAngle(worldspaceDir, vector), -120f, 120f);
	}
}
