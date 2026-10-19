using System.Collections.Generic;
using UnityEngine;

public class LavaBehaviour : MonoBehaviour
{
	private class RefBool
	{
		public bool Marked;
	}

	internal static List<LavaBehaviour> lavas = new List<LavaBehaviour>();

	public float LavaTemperature = 975f;

	public Collider2D Trigger;

	public ContactFilter2D ContactFilter;

	public float TransferRate = 0.01f;

	[Range(0f, 1f)]
	public float Dampening = 0.05f;

	public float Buoyancy = 0.05f;

	public GameObject ImpactPrefab;

	private const int bufferSize = 1024;

	private static readonly Collider2D[] victimBuffer = new Collider2D[1024];

	private float lastImpactSplashTime;

	private readonly Dictionary<PhysicalBehaviour, RefBool> previousVictims = new Dictionary<PhysicalBehaviour, RefBool>();

	private readonly Stack<PhysicalBehaviour> shouldRemove = new Stack<PhysicalBehaviour>();

	private void FixedUpdate()
	{
		int num = Trigger.OverlapCollider(ContactFilter, victimBuffer);
		foreach (KeyValuePair<PhysicalBehaviour, RefBool> previousVictim in previousVictims)
		{
			previousVictim.Value.Marked = false;
		}
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = victimBuffer[i];
			if (!collider2D || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) || value.rigidbody.bodyType != 0)
			{
				continue;
			}
			if (previousVictims.TryGetValue(value, out var value2))
			{
				value2.Marked = true;
			}
			else
			{
				previousVictims.Add(value, new RefBool
				{
					Marked = true
				});
			}
			int num2 = value.LocalGridPointLength;
			for (int j = 0; j < value.LocalColliderGridPoints.Length; j++)
			{
				Vector2 vector = value.LocalColliderGridPoints[j];
				Vector3 vector2 = value.transform.TransformPoint(vector);
				if (Trigger.OverlapPoint(vector2))
				{
					Vector2 pointVelocity = value.rigidbody.GetPointVelocity(vector2);
					value.rigidbody.AddForceAtPosition(Dampening * Mathf.Clamp01(value.rigidbody.mass) * -pointVelocity, vector2, ForceMode2D.Force);
					float num3 = 0.995f * Mathf.Clamp(Mathf.Pow(value.rigidbody.mass, 0.1f), 0.8f, 1f);
					value.rigidbody.velocity *= num3;
					value.rigidbody.angularVelocity *= num3;
					if (Buoyancy > float.Epsilon)
					{
						value.rigidbody.AddForceAtPosition(-1f * Buoyancy * value.rigidbody.mass * Physics2D.gravity / value.LocalGridPointLength, vector2, ForceMode2D.Force);
					}
					num2--;
				}
			}
			if (num2 == 0)
			{
				value.rigidbody.velocity *= 0.95f;
				value.rigidbody.angularVelocity *= 0.95f;
			}
			float num4 = 1f - (float)(num2 / value.LocalGridPointLength);
			bool isInLava = value.IsInLava;
			value.IsInLava = true;
			if (!isInLava)
			{
				DoSplash(value.transform.position, Mathf.Sqrt(value.ObjectArea));
			}
			if (value.SimulateTemperature)
			{
				value.Ignite();
				value.Temperature = Mathf.Lerp(value.Temperature, LavaTemperature, TransferRate / value.GetHeatCapacity() * num4);
			}
		}
		foreach (KeyValuePair<PhysicalBehaviour, RefBool> previousVictim2 in previousVictims)
		{
			if (!previousVictim2.Value.Marked)
			{
				if (!previousVictim2.Key)
				{
					shouldRemove.Push(previousVictim2.Key);
					continue;
				}
				PhysicalBehaviour key = previousVictim2.Key;
				key.IsInLava = false;
				DoSplash(key.transform.position, Mathf.Sqrt(key.ObjectArea));
				shouldRemove.Push(key);
			}
		}
		while (shouldRemove.Count != 0)
		{
			previousVictims.Remove(shouldRemove.Pop());
		}
	}

	public void DoSplash(Vector2 point, float size)
	{
		if (Time.time - lastImpactSplashTime > 0.25f || (double)Random.value > 0.9)
		{
			lastImpactSplashTime = Time.time;
			ParticleSystem.ShapeModule shape = Object.Instantiate(ImpactPrefab, Trigger.ClosestPoint(point), Quaternion.identity).GetComponent<ParticleSystem>().shape;
			shape.radius = size;
		}
	}

	private void Awake()
	{
		lavas.Add(this);
	}

	private void OnDestroy()
	{
		lavas.Remove(this);
	}

	public static bool IsPointInLava(Vector3 point)
	{
		for (int i = 0; i < lavas.Count; i++)
		{
			LavaBehaviour lavaBehaviour = lavas[i];
			if (lavaBehaviour.gameObject.activeInHierarchy && lavaBehaviour.Trigger.OverlapPoint(point))
			{
				return true;
			}
		}
		return false;
	}

	public static LavaBehaviour GetLavaAtPoint(Vector3 point)
	{
		for (int i = 0; i < lavas.Count; i++)
		{
			LavaBehaviour lavaBehaviour = lavas[i];
			if (lavaBehaviour.gameObject.activeInHierarchy && lavaBehaviour.Trigger.OverlapPoint(point))
			{
				return lavaBehaviour;
			}
		}
		return null;
	}

	public static bool TryGetLavaAtPoint(Vector3 point, out LavaBehaviour lava)
	{
		lava = GetLavaAtPoint(point);
		return lava;
	}
}
