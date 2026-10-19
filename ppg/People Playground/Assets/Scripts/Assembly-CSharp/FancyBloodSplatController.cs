using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FancyBloodSplatController : MonoBehaviour, Messages.IOnPoolableReinitialised
{
	public enum DirectionModes
	{
		LocalDirected,
		GlobalDirected,
		Circle
	}

	private class Particle
	{
		public float VelocityX;

		public float VelocityY;

		public float PositionX;

		public float PositionY;

		public float Life;

		public Vector2 Velocity
		{
			get
			{
				return new Vector2(VelocityX, VelocityY);
			}
			set
			{
				VelocityX = value.x;
				VelocityY = value.y;
			}
		}

		public Vector2 Position
		{
			get
			{
				return new Vector2(PositionX, PositionY);
			}
			set
			{
				PositionX = value.x;
				PositionY = value.y;
			}
		}
	}

	public bool UnparentAtStart = true;

	public DirectionModes DirectionMode;

	public bool Continuous;

	[HideIf("IsCircleMode")]
	public Vector2 Direction = Vector2.right;

	[ShowIf("Continuous")]
	[Range(0f, 1f)]
	public float SpawningChance = 0.1f;

	[MinMaxSlider(0f, 32f)]
	[HideIf("Continuous")]
	public Vector2 AmountRange = new Vector2(1f, 4f);

	[Range(0f, 360f)]
	public float AngleRandomisation = 15f;

	[Header("Particle settings")]
	[MinMaxSlider(0f, 5f)]
	public Vector2 LifetimeInSeconds = new Vector2(1f, 2f);

	[MinMaxSlider(0f, 5f)]
	public Vector2 EjectionForce = new Vector2(1f, 5f);

	[Header("Decal settings")]
	public DecalDescriptor Decal;

	public Color DecalColourMultiplier = Color.white;

	public float Size = 0.2f;

	public LayerMask CollisionLayers;

	public bool DestroyWhenNoParticles = true;

	public bool TriggerOnCollide;

	private readonly HashSet<Particle> particles = new HashSet<Particle>();

	private Transform initialParent;

	public event Action<Collider2D> OnCollide;

	private void Start()
	{
		initialParent = base.transform.parent;
		if (UnparentAtStart)
		{
			base.transform.SetParent(null);
		}
		int num = Mathf.RoundToInt(VectorToRandom(AmountRange));
		particles.Clear();
		if (!Continuous)
		{
			for (int i = 0; i < num; i++)
			{
				CreateParticle(i);
			}
		}
	}

	public void CreateParticle(int index = 0)
	{
		Particle item = new Particle
		{
			Life = VectorToRandom(LifetimeInSeconds),
			Position = initialParent.position,
			Velocity = 0.15f * GetEjectionForce(index)
		};
		particles.Add(item);
	}

	private void FixedUpdate()
	{
		particles.RemoveWhere((Particle p) => p.Life <= 0f);
		if (Continuous)
		{
			if (UnityEngine.Random.value <= SpawningChance)
			{
				CreateParticle();
			}
		}
		else if (particles.Count == 0 && DestroyWhenNoParticles)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		foreach (Particle particle in particles)
		{
			particle.PositionX += particle.VelocityX;
			particle.PositionY += particle.VelocityY;
			particle.VelocityX += Physics2D.gravity.x * fixedDeltaTime / 10f;
			particle.VelocityY += Physics2D.gravity.y * fixedDeltaTime / 10f;
			particle.Life -= fixedDeltaTime;
			Collider2D collider2D = Physics2D.OverlapPoint(particle.Position, CollisionLayers);
			if (DecalColourMultiplier.a > 0.05f && (bool)collider2D && (bool)initialParent && collider2D.transform.root != initialParent.root)
			{
				collider2D.SendMessage("Decal", new DecalInstruction(Decal, particle.Position, Size)
				{
					colourMultiplier = DecalColourMultiplier
				}, SendMessageOptions.DontRequireReceiver);
				particle.Life = -1f;
				if (TriggerOnCollide)
				{
					this.OnCollide?.Invoke(collider2D);
				}
			}
		}
	}

	private Vector2 GetEjectionForce(int index)
	{
		switch (DirectionMode)
		{
		default:
			return GetRandomisedAngle() * base.transform.TransformDirection(Direction) * VectorToRandom(EjectionForce);
		case DirectionModes.GlobalDirected:
			return GetRandomisedAngle() * Direction * VectorToRandom(EjectionForce);
		case DirectionModes.Circle:
		{
			int num = (int)Mathf.Max(AmountRange.x, AmountRange.y);
			float f = (float)(360 / num) * (float)index * ((float)Math.PI / 180f);
			return GetRandomisedAngle() * new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * VectorToRandom(EjectionForce);
		}
		}
	}

	private Quaternion GetRandomisedAngle()
	{
		return Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, UnityEngine.Random.value * 360f), AngleRandomisation / 360f);
	}

	private float VectorToRandom(Vector2 v)
	{
		return UnityEngine.Random.Range(v.x, v.y);
	}

	private bool IsCircleMode()
	{
		return DirectionMode == DirectionModes.Circle;
	}

	public void OnPoolableReinitialised(ObjectPoolBehaviour pool)
	{
		if (UnparentAtStart)
		{
			base.transform.SetParent(initialParent);
			base.transform.localPosition = Vector3.zero;
		}
		Start();
	}
}
