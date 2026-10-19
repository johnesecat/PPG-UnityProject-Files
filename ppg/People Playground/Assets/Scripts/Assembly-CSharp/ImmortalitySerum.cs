using UnityEngine;

public class ImmortalitySerum : Liquid
{
	public const string ID = "IMMORTALITY SERUM";

	public override string GetDisplayName()
	{
		return "Immortality serum";
	}

	public ImmortalitySerum()
	{
		Color = new Color(0.87f, 1f, 0f, 1f);
	}

	public override void OnEnterContainer(BloodContainer container)
	{
	}

	public override void OnEnterLimb(LimbBehaviour limb)
	{
		if (limb.SpeciesIdentity == "Android")
		{
			if (Random.value > 0.95f)
			{
				ExplosionCreator.Explode(limb.transform.position, 5f);
			}
			return;
		}
		limb.Health = limb.InitialHealth;
		limb.HealBone();
		limb.DiscomfortingHeatTemperature = float.MaxValue;
		limb.IsLethalToBreak = false;
		limb.Vitality = 0f;
		limb.CirculationBehaviour.HealBleeding();
		limb.LungsPunctured = false;
		limb.ShotDamageMultiplier = 0f;
		limb.ImpactPainMultiplier = 0f;
		limb.InternalTemperature = limb.BodyTemperature;
		limb.BreakingThreshold = float.MaxValue;
		limb.Numbness = 0f;
		limb.SkinMaterialHandler.AcidProgress -= 0.5f;
		limb.RegenerationSpeed = 0.5f;
		limb.CirculationBehaviour.BleedingRate = 0f;
		limb.CirculationBehaviour.IsPump = limb.CirculationBehaviour.WasInitiallyPumping;
		float amountOfBlood = limb.CirculationBehaviour.GetAmountOfBlood();
		if (amountOfBlood < 1f)
		{
			limb.CirculationBehaviour.AddLiquid(limb.GetOriginalBloodType(), 1f - amountOfBlood);
		}
		if (limb.HasBrain)
		{
			limb.Person.PainLevel = 0f;
			limb.Person.ShockLevel = 0f;
			limb.Person.Consciousness = 1f;
			limb.Person.OxygenLevel = 1f;
			limb.Person.Braindead = false;
			limb.Person.BrainDamaged = false;
		}
	}

	public override void OnExitContainer(BloodContainer container)
	{
	}

	public override void OnUpdate(BloodContainer c)
	{
		base.OnUpdate(c);
		CirculationBehaviour circulationBehaviour = c as CirculationBehaviour;
		if ((object)circulationBehaviour != null)
		{
			OnEnterLimb(circulationBehaviour.Limb);
		}
	}
}
