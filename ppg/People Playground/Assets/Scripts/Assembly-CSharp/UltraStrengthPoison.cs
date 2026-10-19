using System;
using UnityEngine;

[Obsolete]
public class UltraStrengthPoison : PoisonSpreadBehaviour
{
	public override float SpreadSpeed => 1.4f;

	public override void Start()
	{
		AddStrengthBonus();
	}

	private void AddStrengthBonus()
	{
		Limb.Vitality *= 0.1f;
		Limb.RegenerationSpeed += 5f;
		Limb.ImpactPainMultiplier *= 0.1f;
		Limb.InitialHealth += 150f;
		Limb.Health = Limb.InitialHealth;
		Limb.BreakingThreshold += 10f;
		Limb.BaseStrength = Mathf.Min(15f, Limb.BaseStrength + 5f);
		Limb.ShotDamageMultiplier *= 0.1f;
		Limb.CirculationBehaviour.HealBleeding();
		Limb.Wince(10f);
	}
}
