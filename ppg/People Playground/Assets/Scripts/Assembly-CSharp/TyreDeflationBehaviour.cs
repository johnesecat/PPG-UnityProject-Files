using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

public class TyreDeflationBehaviour : MonoBehaviour, Messages.IShot, Messages.IOnFragmentHit, Messages.IUnstabbed, Messages.IRepair, Messages.IBreak
{
	[Serializable]
	public struct CircleSettings
	{
		public float Radius;

		public Vector2 Offset;
	}

	public bool Deflated;

	public bool Indestructible;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	[SkipSerialisation]
	public CircleCollider2D Collider;

	[SkipSerialisation]
	public CircleSettings DeflatedColliderSettings;

	[SkipSerialisation]
	public float TemperatureThreshold = 398f;

	[SkipSerialisation]
	public float ShotDamageThreshold = 8f;

	[SkipSerialisation]
	public float FragmentForceThreshold = 15f;

	[SkipSerialisation]
	public AudioClip[] TyreDeflationSound;

	[SkipSerialisation]
	public Material SharedDeflatedMaterial;

	[SkipSerialisation]
	public float VisualSagIntensity = 5f;

	private Material previousMat;

	private CircleSettings initial;

	private MaterialPropertyBlock props;

	private Vector2 sagVector;

	private GameObject pooledDeflationParticles;

	public const string DeflationParticlePrefabPath = "Prefabs/Tyre deflation effect poolable";

	private void Awake()
	{
		initial = new CircleSettings
		{
			Offset = Collider.offset,
			Radius = Collider.radius
		};
	}

	private void Start()
	{
		previousMat = Phys.spriteRenderer.sharedMaterial;
		props = new MaterialPropertyBlock();
		Phys.spriteRenderer.GetPropertyBlock(props);
		if (Deflated)
		{
			Collider.radius = DeflatedColliderSettings.Radius;
			Collider.offset = DeflatedColliderSettings.Offset;
			if ((bool)SharedDeflatedMaterial)
			{
				Phys.spriteRenderer.material = SharedDeflatedMaterial;
			}
		}
		List<ContextMenuButton> buttons = Phys.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("repairMachinery", "Repair", "Repair broken machinery", delegate
		{
			Repair();
		})
		{
			Condition = () => Deflated
		};
		buttons.Add(item);
		Phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("machineryIndestructible", () => (!Indestructible) ? "Make indestructible" : "Make destructible", "Toggle indestructibility", delegate
		{
			Indestructible = !Indestructible;
			if (Indestructible)
			{
				Repair();
			}
		}));
	}

	private void FixedUpdate()
	{
		if (!Indestructible && !Deflated && Phys.SimulateTemperature && Phys.Temperature >= TemperatureThreshold)
		{
			PopTyre((Vector2)base.transform.position + UnityEngine.Random.insideUnitCircle * Collider.radius);
		}
	}

	private void Update()
	{
		if (!Deflated || !SharedDeflatedMaterial)
		{
			return;
		}
		ContactPoint2D[] array = ArrayPool<ContactPoint2D>.Shared.Rent(1);
		int contacts = Phys.rigidbody.GetContacts(array);
		if (contacts == 0)
		{
			sagVector = Utils.SmoothApproach(sagVector, Vector2.zero, 12f, Time.deltaTime);
		}
		else
		{
			for (int i = 0; i < contacts; i++)
			{
				ContactPoint2D contactPoint2D = array[i];
				Vector3 vector = base.transform.InverseTransformPoint(contactPoint2D.point);
				float magnitude = vector.magnitude;
				float num = Mathf.Max(0f, initial.Radius - magnitude);
				sagVector = Utils.SmoothApproach(sagVector, vector * (VisualSagIntensity * num), 12f, Time.deltaTime);
				Utils.DrawCross(contactPoint2D.point, sagVector.magnitude + 0.2f, Color.magenta, Time.deltaTime);
			}
		}
		ArrayPool<ContactPoint2D>.Shared.Return(array);
	}

	public void PopTyre(Vector2? point = null)
	{
		if (!Indestructible && !Deflated)
		{
			if (point.HasValue)
			{
				Vector2 vector = Collider.ClosestPoint(point.Value);
				GameObject obj = (pooledDeflationParticles = PoolGenerator.Instance.RequestPrefab(Resources.Load<GameObject>("Prefabs/Tyre deflation effect poolable"), vector));
				obj.transform.right = (vector - (Vector2)base.transform.position).normalized;
				obj.transform.SetParent(base.transform);
			}
			Phys.PlayClipOnce(TyreDeflationSound.PickRandom());
			Collider.radius = DeflatedColliderSettings.Radius;
			Collider.offset = DeflatedColliderSettings.Offset;
			Deflated = true;
			if ((bool)SharedDeflatedMaterial)
			{
				Phys.spriteRenderer.material = SharedDeflatedMaterial;
			}
		}
	}

	public void Shot(Shot shot)
	{
		if (!Indestructible && shot.damage >= ShotDamageThreshold)
		{
			PopTyre(shot.point);
		}
	}

	public void OnFragmentHit(float fragmentForce)
	{
		if (!Indestructible && fragmentForce >= FragmentForceThreshold)
		{
			PopTyre();
		}
	}

	public void Unstabbed(Stabbing stabbing)
	{
		PopTyre(stabbing.point);
	}

	public void Repair()
	{
		Deflated = false;
		Collider.radius = initial.Radius;
		Collider.offset = initial.Offset;
		if ((bool)SharedDeflatedMaterial)
		{
			Phys.spriteRenderer.material = previousMat;
		}
	}

	private void OnDestroy()
	{
		if ((bool)pooledDeflationParticles)
		{
			pooledDeflationParticles.transform.SetParent(null);
			if (PoolGenerator.Instance.GetFor(Resources.Load<GameObject>("Prefabs/Tyre deflation effect poolable"), out var pool))
			{
				pool.Return(pooledDeflationParticles);
			}
		}
	}

	private void OnWillRenderObject()
	{
		if (Deflated && (bool)SharedDeflatedMaterial)
		{
			props.SetVector(ShaderProperties.Get("_Direction"), sagVector);
			Phys.spriteRenderer.SetPropertyBlock(props);
		}
	}

	public void Break(Vector2 impactVelocity)
	{
		PopTyre();
	}
}
