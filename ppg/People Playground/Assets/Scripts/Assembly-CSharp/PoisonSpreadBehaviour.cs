using System;
using System.Collections;
using UnityEngine;

[Obsolete]
public abstract class PoisonSpreadBehaviour : MonoBehaviour
{
	public bool Spreads = true;

	public LimbBehaviour Limb;

	public float LifeTime;

	public bool HasSpread;

	public float[] Fingerprint;

	private int fingerprintReadPos;

	public virtual float SpreadSpeed => 1f;

	public virtual float Lifespan => 5f;

	public virtual bool AllowMultipleActivations => true;

	public abstract void Start();

	protected virtual void Update()
	{
		LifeTime += Time.deltaTime;
		Spread();
		DestroyIfExpired();
	}

	private void Spread()
	{
		if (HasSpread || !Spreads || !(LifeTime > 1f / Mathf.Max(0.0001f, SpreadSpeed)))
		{
			return;
		}
		CirculationBehaviour source = Limb.CirculationBehaviour.Source;
		if (!Limb.CirculationBehaviour.IsDisconnected && (bool)source && shouldPoison(source.Limb))
		{
			Poison(source.Limb);
		}
		CirculationBehaviour[] pushesTo = Limb.CirculationBehaviour.PushesTo;
		foreach (CirculationBehaviour circulationBehaviour in pushesTo)
		{
			if ((bool)circulationBehaviour && !circulationBehaviour.IsDisconnected && shouldPoison(circulationBehaviour.Limb))
			{
				Poison(circulationBehaviour.Limb);
			}
		}
		HasSpread = true;
		bool shouldPoison(LimbBehaviour cir)
		{
			return !cir.PhysicalBehaviour.isDisintegrated;
		}
	}

	protected float GetFingerprintValue()
	{
		float result = Fingerprint[fingerprintReadPos];
		fingerprintReadPos++;
		if (fingerprintReadPos >= Fingerprint.Length)
		{
			fingerprintReadPos = 0;
		}
		return result;
	}

	private void DestroyIfExpired()
	{
		if (LifeTime > Lifespan)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public void Poison(LimbBehaviour limb)
	{
		PoisonSpreadBehaviour poisonSpreadBehaviour = limb.gameObject.GetComponent(GetType()) as PoisonSpreadBehaviour;
		if (!poisonSpreadBehaviour)
		{
			poisonSpreadBehaviour = limb.gameObject.AddComponent(GetType()) as PoisonSpreadBehaviour;
			poisonSpreadBehaviour.Limb = limb;
			poisonSpreadBehaviour.Fingerprint = Fingerprint;
			OnPoisonOther(poisonSpreadBehaviour);
		}
		else if (AllowMultipleActivations)
		{
			poisonSpreadBehaviour.Start();
		}
	}

	protected virtual void OnPoisonOther(PoisonSpreadBehaviour other)
	{
	}

	public IEnumerator WaitAndDelete(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		UnityEngine.Object.Destroy(this);
	}
}
