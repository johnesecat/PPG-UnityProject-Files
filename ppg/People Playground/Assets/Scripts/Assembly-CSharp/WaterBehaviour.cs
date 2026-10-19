using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour, IManagedBehaviour
{
	internal static List<WaterBehaviour> waters = new List<WaterBehaviour>();

	public float Drag;

	public float Buoyancy;

	public float LocalSurfaceLevel;

	public HashSet<GameObject> Ignore = new HashSet<GameObject>();

	public GameObject[] ObjectsToBeActiveIfWater;

	public GameObject Splash;

	public GameObject BoilingWaterSound;

	public SpriteRenderer SpriteRenderer;

	public Collider2D Trigger;

	public LayerMask Layers;

	public PhysicalBehaviour Ice;

	public float FreezingPointCelsius;

	public float BoilingPointCelsius = 100f;

	private const int currentlyInWaterIncrementSize = 32;

	private PhysicalBehaviour[] currentlyInWater;

	private int currentlyInWaterCount;

	private bool wasFrozen;

	private AudioClip[] waterSizzles;

	private HashSet<PhysicalBehaviour> frozenInIce = new HashSet<PhysicalBehaviour>();

	private Material mat;

	private static List<ParticleSystem> buffer = new List<ParticleSystem>(4);

	public bool IsEvaporated => PhysicalBehaviour.AmbientTemperature > BoilingPointCelsius + 100f;

	public bool IsFrozen => PhysicalBehaviour.AmbientTemperature < FreezingPointCelsius;

	public float GetGlobalSurfaceLevel()
	{
		return base.transform.TransformPoint(new Vector3(0f, LocalSurfaceLevel)).y;
	}

	public IEnumerable<PhysicalBehaviour> GetSubmergedObjects()
	{
		if (currentlyInWater == null || currentlyInWater.Length == 0)
		{
			yield break;
		}
		for (int i = 0; i < currentlyInWaterCount; i++)
		{
			PhysicalBehaviour physicalBehaviour = currentlyInWater[i];
			if ((bool)physicalBehaviour)
			{
				yield return physicalBehaviour;
			}
		}
	}

	private void Awake()
	{
		waters.Add(this);
	}

	private void OnDestroy()
	{
		waters.Remove(this);
	}

	private void Start()
	{
		SpriteRenderer = GetComponent<SpriteRenderer>();
		if (base.gameObject.activeInHierarchy)
		{
			waterSizzles = Resources.LoadAll<AudioClip>("Audio/water_sizzle");
		}
		UpdateTemperatureState();
	}

	public void IgnoreObject(GameObject obj)
	{
		if (Ignore.Add(obj))
		{
			Collider2D[] componentsInChildren = obj.GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Physics2D.IgnoreCollision(componentsInChildren[i], Trigger, ignore: true);
			}
		}
	}

	public void StopIgnoringObject(GameObject obj)
	{
		if (Ignore.Remove(obj))
		{
			Collider2D[] componentsInChildren = obj.GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Physics2D.IgnoreCollision(componentsInChildren[i], Trigger, ignore: false);
			}
		}
	}

	public static bool IsPointUnderWater(Vector3 point)
	{
		for (int i = 0; i < waters.Count; i++)
		{
			WaterBehaviour waterBehaviour = waters[i];
			if (waterBehaviour.gameObject.activeInHierarchy && !waterBehaviour.IsEvaporated && !waterBehaviour.IsFrozen && waterBehaviour.IsPointInsideWater(point))
			{
				return true;
			}
		}
		return false;
	}

	public static WaterBehaviour GetWaterAtPoint(Vector3 point)
	{
		for (int i = 0; i < waters.Count; i++)
		{
			WaterBehaviour waterBehaviour = waters[i];
			if (waterBehaviour.gameObject.activeInHierarchy && waterBehaviour.IsPointInsideWater(point))
			{
				return waterBehaviour;
			}
		}
		return null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!IsEvaporated && !IsFrozen)
		{
			DoSplash(collision);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!IsEvaporated && !IsFrozen)
		{
			DoSplash(collision);
		}
	}

	private void DoSplash(Collider2D collision)
	{
		if (!UserPreferenceManager.Current.FancyEffects || collision.isTrigger || Ignore.Contains(collision.gameObject) || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value))
		{
			return;
		}
		Rigidbody2D rigidbody = value.rigidbody;
		Vector2 velocity = rigidbody.velocity;
		if (Mathf.Abs(velocity.y) < 0.75f)
		{
			return;
		}
		GameObject gameObject = (Splash.CompareTag("Poolable") ? PoolGenerator.Instance.RequestPrefab(Splash, Trigger.bounds.ClosestPoint(collision.transform.position)) : UnityEngine.Object.Instantiate(Splash, Trigger.bounds.ClosestPoint(collision.transform.position), Quaternion.identity));
		if (!gameObject)
		{
			return;
		}
		velocity.y = 0f - Mathf.Abs(velocity.y);
		gameObject.transform.up = Vector2.Reflect(velocity, Vector2.up);
		float num = Mathf.Min(collision.bounds.extents.x, 4f);
		float num2 = Mathf.Clamp(rigidbody.velocity.magnitude * 0.5f * rigidbody.mass, 0.2f, 25f);
		gameObject.GetComponent<AudioSource>().volume = num2 / 15f;
		gameObject.GetComponentsInChildren(buffer);
		for (int i = 0; i < buffer.Count; i++)
		{
			ParticleSystem particleSystem = buffer[i];
			if ((bool)particleSystem)
			{
				ParticleSystem.ShapeModule shape = particleSystem.shape;
				ParticleSystem.MainModule main = particleSystem.main;
				shape.radius = num;
				main.startSizeMultiplier = num * 2.5f * UnityEngine.Random.Range(1f, 1.4f);
				main.startSpeedMultiplier = num2;
			}
		}
	}

	private void UpdateObjectsToBeActiveIfWater(bool active)
	{
		for (int i = 0; i < ObjectsToBeActiveIfWater.Length; i++)
		{
			ObjectsToBeActiveIfWater[i].SetActive(active);
		}
		SpriteRenderer.enabled = active;
		GetComponent<SpriteMask>().enabled = active;
	}

	public void UpdateTemperatureState()
	{
		Ice.gameObject.SetActive(value: false);
		if (IsFrozen)
		{
			SetFrozen();
			return;
		}
		if (wasFrozen)
		{
			foreach (PhysicalBehaviour item in frozenInIce)
			{
				if ((bool)item && item.TryGetComponent<FrozenInIceBehaviour>(out var component))
				{
					UnityEngine.Object.Destroy(component);
					FreezeStackController.RequestUnfreeze(item.rigidbody);
				}
			}
		}
		wasFrozen = false;
		frozenInIce.Clear();
		if (IsEvaporated)
		{
			SetEvaporated();
			return;
		}
		BoilingWaterSound.SetActive(PhysicalBehaviour.AmbientTemperature >= BoilingPointCelsius);
		UpdateObjectsToBeActiveIfWater(active: true);
	}

	private void SetEvaporated()
	{
		BoilingWaterSound.SetActive(value: false);
		UpdateObjectsToBeActiveIfWater(active: false);
	}

	private void SetFrozen()
	{
		BoilingWaterSound.SetActive(value: false);
		wasFrozen = true;
		LayerMask mask = LayerMask.GetMask("Bounds");
		for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[i];
			if (Ignore.Contains(physicalBehaviour.gameObject) || mask.HasLayer(physicalBehaviour.gameObject.layer) || frozenInIce.Contains(physicalBehaviour))
			{
				continue;
			}
			int num = 0;
			int num2 = physicalBehaviour.LocalColliderGridPoints.Length;
			for (int j = 0; j < physicalBehaviour.LocalColliderGridPoints.Length; j++)
			{
				Vector2 vector = physicalBehaviour.LocalColliderGridPoints[j];
				Vector3 vector2 = physicalBehaviour.transform.TransformPoint(vector);
				if (IsPointInsideWater(vector2))
				{
					num++;
					if ((float)num / (float)num2 >= 0.05f)
					{
						physicalBehaviour.rigidbody.velocity = default(Vector2);
						physicalBehaviour.rigidbody.angularVelocity = 0f;
						FreezeStackController.RequestFreeze(physicalBehaviour.rigidbody);
						frozenInIce.Add(physicalBehaviour);
						physicalBehaviour.gameObject.AddComponent<FrozenInIceBehaviour>();
						physicalBehaviour.Temperature = Mathf.Min(physicalBehaviour.Temperature, PhysicalBehaviour.AmbientTemperature);
						break;
					}
				}
			}
		}
		UpdateObjectsToBeActiveIfWater(active: false);
		SpriteRenderer.enabled = true;
		Ice.gameObject.SetActive(value: true);
	}

	public void ManagedFixedUpdate()
	{
		currentlyInWaterCount = 0;
		if (IsFrozen)
		{
			Ice.Temperature = PhysicalBehaviour.AmbientTemperature;
			bool flag = Time.frameCount % 2 == 0;
			foreach (PhysicalBehaviour item in frozenInIce)
			{
				if (!item)
				{
					continue;
				}
				if (item.OnFire)
				{
					item.Extinguish();
				}
				item.Wetness = 1f;
				if (!flag)
				{
					continue;
				}
				bool flag2 = false;
				for (int i = 0; i < item.LocalColliderGridPoints.Length; i++)
				{
					Vector2 vector = item.LocalColliderGridPoints[i];
					Vector3 vector2 = item.transform.TransformPoint(vector);
					if (IsPointInsideWater(vector2))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2 && item.TryGetComponent<FrozenInIceBehaviour>(out var component))
				{
					UnityEngine.Object.Destroy(component);
					FreezeStackController.RequestUnfreeze(item.rigidbody);
				}
			}
		}
		else
		{
			if (IsEvaporated)
			{
				return;
			}
			Vector3 vector3 = Vector3.up * Buoyancy;
			float globalSurfaceLevel = GetGlobalSurfaceLevel();
			float num = Mathf.Min(PhysicalBehaviour.AmbientTemperature, BoilingPointCelsius);
			bool flag3 = PhysicalBehaviour.AmbientTemperature >= BoilingPointCelsius;
			for (int j = 0; j < Global.main.PhysicalObjectsInWorld.Count; j++)
			{
				PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[j];
				if (!physicalBehaviour || Ignore.Contains(physicalBehaviour.gameObject))
				{
					continue;
				}
				float num2 = Mathf.Clamp(Mathf.Pow(Mathf.Abs(physicalBehaviour.transform.position.y - globalSurfaceLevel), 0.25f), 1f, 8f);
				float num3 = Mathf.Clamp01((num2 - 1f) / 7f) + 1f;
				Vector2 force = physicalBehaviour.rigidbody.mass / (float)physicalBehaviour.LocalGridPointLength * num2 * (physicalBehaviour.Properties.Buoyancy * physicalBehaviour.BuoyancyModifier) * vector3;
				bool flag4 = false;
				physicalBehaviour.CurrentWaterSurfaceLevel = globalSurfaceLevel;
				for (int k = 0; k < physicalBehaviour.LocalColliderGridPoints.Length; k++)
				{
					Vector2 vector4 = physicalBehaviour.LocalColliderGridPoints[k];
					Vector3 vector5 = physicalBehaviour.transform.TransformPoint(vector4);
					if (IsPointInsideWater(vector5))
					{
						flag4 = true;
						if (!flag3)
						{
							physicalBehaviour.rigidbody.AddForceAtPosition(force, vector5);
						}
						Vector2 pointVelocity = physicalBehaviour.rigidbody.GetPointVelocity(vector5);
						physicalBehaviour.rigidbody.AddForceAtPosition(0.4f * num3 * GetDragFactor(pointVelocity) * physicalBehaviour.rigidbody.mass * -pointVelocity, vector5);
					}
				}
				bool flag5 = !physicalBehaviour.IsUnderWater;
				if (flag4)
				{
					physicalBehaviour.underwaterMarkings++;
				}
				if (!flag4)
				{
					continue;
				}
				if (currentlyInWater == null || currentlyInWater.Length <= currentlyInWaterCount + 1)
				{
					if (currentlyInWater == null)
					{
						currentlyInWater = new PhysicalBehaviour[32];
					}
					else
					{
						Array.Resize(ref currentlyInWater, currentlyInWater.Length + 32);
					}
				}
				currentlyInWater[currentlyInWaterCount++] = physicalBehaviour;
				if (flag3 && AliveBehaviour.AliveByTransform.TryGetValue(physicalBehaviour.transform.root, out var value))
				{
					physicalBehaviour.BurnProgress = Mathf.Max(physicalBehaviour.BurnProgress, 0.5f);
					value.SendMessage("AddPain", 15f, SendMessageOptions.DontRequireReceiver);
				}
				physicalBehaviour.Temperature = Mathf.Lerp(physicalBehaviour.Temperature, num, 0.05f * physicalBehaviour.Properties.HeatTransferSpeedMultiplier);
				if (flag5 && physicalBehaviour.Temperature >= 100f && physicalBehaviour.Temperature > num * 2f)
				{
					physicalBehaviour.PlayClipOnce(waterSizzles.PickRandom());
					physicalBehaviour.Sizzle(withSound: false);
				}
				physicalBehaviour.Wetness += 0.1f;
				physicalBehaviour.SendMessage("WaterImpact", physicalBehaviour.rigidbody.velocity.magnitude, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private float GetDragFactor(Vector2 velocity)
	{
		float num = velocity.sqrMagnitude * 0.2f;
		return Mathf.Clamp(Mathf.Pow(num + 1f, num * Drag), 0.6f, 1f);
	}

	private void OnWillRenderObject()
	{
		if (!mat)
		{
			mat = SpriteRenderer.material;
		}
		if ((bool)mat)
		{
			mat.SetFloat(ShaderProperties.Get("_Temperature"), Utils.MapRange(FreezingPointCelsius, BoilingPointCelsius, 0f, 100f, PhysicalBehaviour.AmbientTemperature));
		}
	}

	private bool IsPointInsideWater(Vector2 point)
	{
		return Trigger.OverlapPoint(point);
	}

	public void ManagedUpdate()
	{
	}

	public void ManagedLateUpdate()
	{
	}

	public bool ShouldUpdate()
	{
		if (base.gameObject.activeInHierarchy)
		{
			return base.enabled;
		}
		return false;
	}
}
