using UnityEngine;

public class HealthGunBehaviour : EffectGunBehaviour
{
	protected override void Affect(Collider2D coll)
	{
		if (!TryGetLimb(coll, out var limb) || limb.IsAndroid)
		{
			return;
		}
		limb.HealBone();
		limb.Health = limb.InitialHealth;
		limb.CirculationBehaviour.HealBleeding();
		limb.SkinMaterialHandler.AcidProgress *= 0.25f;
		limb.SkinMaterialHandler.RottenProgress = 0f;
		limb.PhysicalBehaviour.BurnProgress *= 0.8f;
		limb.Numbness = 0f;
		limb.CirculationBehaviour.IsPump = limb.CirculationBehaviour.WasInitiallyPumping;
		limb.CirculationBehaviour.BloodFlow = 1f;
		limb.CirculationBehaviour.InternalBleedingIntensity *= 0.5f;
		limb.CirculationBehaviour.ClearLiquid();
		limb.CirculationBehaviour.AddLiquid(limb.GetOriginalBloodType(), 1f);
		limb.Frozen = false;
		if (limb.RoughClassification == LimbBehaviour.BodyPart.Head)
		{
			limb.Person.ShockLevel = 0f;
			limb.Person.PainLevel = 0f;
			limb.Person.AdrenalineLevel = 1f;
			limb.Person.Consciousness += 0.5f;
			limb.Person.Braindead = false;
			if (Random.value > 0.95f)
			{
				limb.Person.BrainDamaged = false;
			}
		}
	}
}
