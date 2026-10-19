using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableBehaviour : MonoBehaviour, Messages.IOnFragmentHit, Messages.IShot, Messages.IBreak
{
	[SkipSerialisation]
	public float OverallChance = 1f;

	[SkipSerialisation]
	public float ShotBreakChance = 1f;

	[Space]
	[SkipSerialisation]
	public float MinimumImpactForce = 5f;

	[SkipSerialisation]
	public float MinimumShotDamage = 15f;

	[SkipSerialisation]
	public float MinimumExplosionFragmentDamage = 2f;

	[SkipSerialisation]
	public GameObject DebrisPrefab;

	[SkipSerialisation]
	public Rigidbody2D VelocityAncestor;

	[SkipSerialisation]
	public bool ReactToCollision = true;

	[SkipSerialisation]
	public bool ReactToBullets = true;

	[SkipSerialisation]
	public bool ReactToExplosions = true;

	[SkipSerialisation]
	public float RestoreIncomingVelocityWhenDestroyedRatio = 0.5f;

	[SkipSerialisation]
	public float TemperatureLimit = float.PositiveInfinity;

	[SkipSerialisation]
	public bool DestroyWhenBroken = true;

	[SkipSerialisation]
	public bool TransferUndoRegistry = true;

	private PhysicalBehaviour phys;

	private bool broken;

	private static readonly ContactPoint2D[] buffer = new ContactPoint2D[8];

	public UnityEvent OnBreak = new UnityEvent();

	protected virtual void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	protected virtual void Update()
	{
		if ((bool)phys && phys.SimulateTemperature && phys.Temperature >= TemperatureLimit && (!TryGetComponent<ExplosiveBehaviour>(out var component) || !(component.TemperatureLimit >= phys.Temperature)))
		{
			Break();
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		EvaluateCollision(collision);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		EvaluateCollision(collision);
	}

	private void EvaluateCollision(Collision2D collision)
	{
		if (!ReactToCollision)
		{
			return;
		}
		int contacts = collision.GetContacts(buffer);
		if (Utils.GetMinImpulse(buffer, contacts) < MinimumImpactForce)
		{
			return;
		}
		if (TryGetComponent<ExplosiveBehaviour>(out var component) && component.ImpactForceThreshold > 0f)
		{
			component.ForceImmediateExplosion();
			return;
		}
		if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.collider.transform, out var value) && value.rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			value.rigidbody.velocity = Vector2.Lerp(value.rigidbody.velocity, value.GetPreviousVel(), RestoreIncomingVelocityWhenDestroyedRatio);
		}
		Break(collision.otherRigidbody.velocity - collision.relativeVelocity);
	}

	public void OnFragmentHit(float force)
	{
		if (ReactToExplosions && !(force < MinimumExplosionFragmentDamage))
		{
			if (TryGetComponent<ExplosiveBehaviour>(out var component) && component.ExplodesOnFragmentHit)
			{
				component.ForceImmediateExplosion();
			}
			else
			{
				Break();
			}
		}
	}

	public void Shot(Shot shot)
	{
		if (ReactToBullets && !(shot.damage < MinimumShotDamage) && !(ShotBreakChance < Random.value))
		{
			if (TryGetComponent<ExplosiveBehaviour>(out var component) && component.ShotExplodeChance > float.Epsilon)
			{
				component.ForceImmediateExplosion();
			}
			else
			{
				StartCoroutine(WaitAFrame());
			}
		}
		IEnumerator WaitAFrame()
		{
			yield return new WaitForSeconds(0.05f);
			Break(shot.normal * (0f - MinimumShotDamage) / 20f);
		}
	}

	public void Break(Vector2 velocity = default(Vector2))
	{
		if (!broken && !(OverallChance < Random.value))
		{
			broken = true;
			GameObject createdDebris = Object.Instantiate(DebrisPrefab, base.transform.position, base.transform.rotation);
			OnDebrisCreated(createdDebris, velocity);
			OnBreak?.Invoke();
		}
	}

	protected virtual void OnDebrisCreated(GameObject createdDebris, Vector2 velocity)
	{
		createdDebris.GetOrAddComponent<DebrisComponent>();
		createdDebris.transform.localScale = base.transform.lossyScale;
		ApplyAncestorProperties(VelocityAncestor, phys, velocity, createdDebris);
		if (DestroyWhenBroken)
		{
			Object.Destroy(base.gameObject);
			if (TransferUndoRegistry)
			{
				ReplaceUndoEntryWith(base.transform, createdDebris);
			}
		}
		else
		{
			base.gameObject.AddComponent<ActOnDestroy>().DestroyWith = new GameObject[1] { createdDebris };
		}
	}

	internal static void ReplaceUndoEntryWith(Transform originalParent, GameObject createdDebris)
	{
		if (!UndoControllerBehaviour.FindRelevantAction(originalParent.root.gameObject, out var result))
		{
			return;
		}
		ObjectCreationAction objectCreationAction = result as ObjectCreationAction;
		if (objectCreationAction == null)
		{
			PasteLoadAction pasteLoadAction = result as PasteLoadAction;
			if (pasteLoadAction == null)
			{
				return;
			}
			int num = pasteLoadAction.RelevantObjects.IndexOf(originalParent.root.gameObject);
			if (num == -1)
			{
				Debug.LogWarning("Could not find Destroyable entry in pasted object collection");
				return;
			}
			pasteLoadAction.RelevantObjects.RemoveAt(num);
			pasteLoadAction.RelevantObjects.Insert(num, createdDebris);
			foreach (Transform item in createdDebris.transform)
			{
				pasteLoadAction.RelevantObjects.Insert(num, item.gameObject);
			}
			return;
		}
		objectCreationAction.RelevantObjects.Clear();
		objectCreationAction.RelevantObjects.Add(createdDebris);
		foreach (Transform item2 in createdDebris.transform)
		{
			objectCreationAction.RelevantObjects.Add(item2.gameObject);
		}
	}

	internal static void ApplyAncestorProperties(Rigidbody2D velocityAncestor, PhysicalBehaviour originalPhysicalBehaviour, Vector2 velocity, GameObject debris)
	{
		if (!velocityAncestor)
		{
			return;
		}
		Rigidbody2D[] componentsInChildren = debris.GetComponentsInChildren<Rigidbody2D>();
		foreach (Rigidbody2D rigidbody2D in componentsInChildren)
		{
			Vector2 vector2 = (rigidbody2D.velocity = Vector2.ClampMagnitude(velocityAncestor.GetPointVelocity(rigidbody2D.transform.position) + velocity, velocityAncestor.velocity.magnitude));
			rigidbody2D.angularVelocity = velocityAncestor.angularVelocity * 0.2f;
			rigidbody2D.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
		}
		PhysicalBehaviour component = originalPhysicalBehaviour.GetComponent<PhysicalBehaviour>();
		if (!component)
		{
			return;
		}
		PhysicalBehaviour[] componentsInChildren2 = debris.GetComponentsInChildren<PhysicalBehaviour>();
		PhysicalBehaviour[] array = componentsInChildren2;
		foreach (PhysicalBehaviour physicalBehaviour in array)
		{
			physicalBehaviour.BurnProgress = component.BurnProgress;
			if (component.OnFire)
			{
				physicalBehaviour.Ignite(ignoreFlammability: true);
			}
			physicalBehaviour.charge = component.charge / (float)Mathf.Max(1, componentsInChildren2.Length / 4);
			physicalBehaviour.burnIntensity = component.burnIntensity / (float)componentsInChildren2.Length;
			if (physicalBehaviour.SimulateTemperature && component.SimulateTemperature)
			{
				physicalBehaviour.Temperature = component.Temperature;
			}
		}
	}
}
