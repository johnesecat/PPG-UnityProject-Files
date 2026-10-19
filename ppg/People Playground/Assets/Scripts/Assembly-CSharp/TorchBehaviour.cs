using System;
using System.Collections.Generic;
using UnityEngine;

public class TorchBehaviour : MonoBehaviour, Messages.IOnInsideVacuum
{
	[Serializable]
	public struct ParticleSystemRateMutliplier
	{
		public ParticleSystem System;

		public float Multiplier;

		public ParticleSystemRateMutliplier(ParticleSystem item1, float item2)
		{
			System = item1;
			Multiplier = item2;
		}

		public override bool Equals(object obj)
		{
			if (obj is ParticleSystemRateMutliplier)
			{
				ParticleSystemRateMutliplier particleSystemRateMutliplier = (ParticleSystemRateMutliplier)obj;
				if (EqualityComparer<ParticleSystem>.Default.Equals(System, particleSystemRateMutliplier.System))
				{
					return Multiplier == particleSystemRateMutliplier.Multiplier;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (-1030903623 * -1521134295 + EqualityComparer<ParticleSystem>.Default.GetHashCode(System)) * -1521134295 + Multiplier.GetHashCode();
		}

		public void Deconstruct(out ParticleSystem item1, out float item2)
		{
			item1 = System;
			item2 = Multiplier;
		}

		public static implicit operator (ParticleSystem, float)(ParticleSystemRateMutliplier value)
		{
			return (value.System, value.Multiplier);
		}

		public static implicit operator ParticleSystemRateMutliplier((ParticleSystem, float) value)
		{
			return new ParticleSystemRateMutliplier(value.Item1, value.Item2);
		}
	}

	private HashSet<PhysicalBehaviour> currentlyColliding = new HashSet<PhysicalBehaviour>();

	public float TouchTemperature = 400f;

	public float TransferSpeed = 0.01f;

	[SkipSerialisation]
	public FireSoundEmitter FireSoundEmitter;

	[SkipSerialisation]
	public ParticleSystemRateMutliplier[] FlameParticles;

	private float torchIntensity = 1f;

	private void OnEnable()
	{
		ContactPoint2D[] array = new ContactPoint2D[4];
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			int contacts = componentsInChildren[i].GetContacts(array);
			for (int j = 0; j < contacts; j++)
			{
				Register(array[j].collider, array[j].point);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (base.enabled && (bool)collision.rigidbody)
		{
			Register(collision.collider, collision.GetContact(0).point);
		}
	}

	private void Register(Collider2D c, Vector2 contactPoint)
	{
		if ((bool)c.gameObject && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(c.transform, out var value))
		{
			currentlyColliding.Add(value);
			value.Ignite(ignoreFlammability: true, contactPoint);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (base.enabled && (bool)collision.rigidbody && (bool)collision.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value))
		{
			currentlyColliding.Remove(value);
		}
	}

	private void FixedUpdate()
	{
		currentlyColliding.RemoveWhere((PhysicalBehaviour a) => !a);
		foreach (PhysicalBehaviour item in currentlyColliding)
		{
			item.Temperature = Mathf.Lerp(item.Temperature, TouchTemperature, TransferSpeed);
		}
		if (torchIntensity < 1f)
		{
			torchIntensity += Time.fixedDeltaTime;
		}
		else
		{
			torchIntensity = 1f;
		}
		if ((bool)FireSoundEmitter)
		{
			FireSoundEmitter.Intensity = torchIntensity * 0.2f;
		}
		if (FlameParticles != null)
		{
			for (int i = 0; i < FlameParticles.Length; i++)
			{
				ParticleSystem.EmissionModule emission = FlameParticles[i].System.emission;
				emission.rateOverTimeMultiplier = torchIntensity * FlameParticles[i].Multiplier;
			}
		}
	}

	public void OnInsideVacuum()
	{
		if (torchIntensity > -1f)
		{
			torchIntensity -= Time.fixedDeltaTime * 2f;
		}
	}
}
