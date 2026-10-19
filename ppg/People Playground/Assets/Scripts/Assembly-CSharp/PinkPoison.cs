using System;
using UnityEngine;

[Obsolete]
public class PinkPoison : PoisonSpreadBehaviour
{
	public override float SpreadSpeed => 1.4f;

	public override void Start()
	{
		WhatTheFuck();
	}

	private void WhatTheFuck()
	{
		CirculationBehaviour circulationBehaviour = Limb.CirculationBehaviour;
		SkinMaterialHandler skinMaterialHandler = Limb.SkinMaterialHandler;
		PhysicalBehaviour physicalBehaviour = Limb.PhysicalBehaviour;
		Limb.BalanceMuscleMovement += GetFingerprintValue() * 0.5f;
		Limb.BloodMuscleStrengthRatio += GetFingerprintValue() * 0.5f;
		Limb.DoBalanceJerk ^= GetFingerprintValue() > 0.7f;
		Limb.DoStumble ^= GetFingerprintValue() > 0.7f;
		Limb.Numbness += GetFingerprintValue();
		Limb.ShotDamageMultiplier += GetFingerprintValue() * 5f;
		if (GetFingerprintValue() > 0.95f)
		{
			Limb.BreakBone();
		}
		if (GetFingerprintValue() > 0.95f)
		{
			Limb.Crush();
		}
		skinMaterialHandler.AcidProgress += GetFingerprintValue() * 0.05f;
		skinMaterialHandler.intensityMultiplier += GetFingerprintValue() * 0.5f;
		skinMaterialHandler.RottenProgress += GetFingerprintValue() * 0.05f;
		if (GetFingerprintValue() > 0.9f)
		{
			circulationBehaviour.Cut(Limb.transform.position, UnityEngine.Random.insideUnitCircle);
		}
		circulationBehaviour.BleedingRate *= Mathf.Abs(GetFingerprintValue());
		circulationBehaviour.BloodLossRateMultiplier += GetFingerprintValue() * 0.2f;
		circulationBehaviour.IsPump ^= GetFingerprintValue() > 0.9f;
		circulationBehaviour.WasInitiallyPumping ^= GetFingerprintValue() > 0.7f;
		physicalBehaviour.AbsorbsLasers ^= GetFingerprintValue() > 0f;
		physicalBehaviour.ReflectsLasers ^= GetFingerprintValue() > 0.7f;
		physicalBehaviour.rigidbody.mass += GetFingerprintValue() * physicalBehaviour.rigidbody.mass * 0.5f;
		if (GetFingerprintValue() > 0.99f)
		{
			float num = GetFingerprintValue() * 25f;
			physicalBehaviour.Charge += num * num * num;
		}
		if (GetFingerprintValue() > 0.95f)
		{
			physicalBehaviour.Ignite(ignoreFlammability: true);
		}
		if (GetFingerprintValue() > 0.9f)
		{
			physicalBehaviour.transform.localScale += new Vector3(GetFingerprintValue(), GetFingerprintValue()) * 0.2f;
		}
		physicalBehaviour.rigidbody.gravityScale += ((GetFingerprintValue() > 0.9f) ? GetFingerprintValue() : 0f);
		Limb.Vitality *= GetFingerprintValue() * 8f;
		Limb.RegenerationSpeed += GetFingerprintValue() * 8f;
		Limb.ImpactPainMultiplier += GetFingerprintValue();
		Limb.InitialHealth += GetFingerprintValue();
		Limb.Health = Limb.InitialHealth;
		Limb.BreakingThreshold += GetFingerprintValue();
		Limb.BaseStrength += GetFingerprintValue() * 8f;
		if (GetFingerprintValue() > 0.99f)
		{
			ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(Limb.transform.position), new ExplosionCreator.ExplosionParameters(32u, Limb.transform.position, Mathf.Abs(GetFingerprintValue()) * 8f, GetFingerprintValue() * 25f, GetFingerprintValue() < 0.5f, GetFingerprintValue() > 0.9f));
		}
		Limb.Frozen = GetFingerprintValue() > 0.95f;
		Limb.Wince(GetFingerprintValue());
	}
}
