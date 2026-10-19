using UnityEngine;

public class UltraStrengthSyringe : SyringeBehaviour
{
	public class UltraStrengthSerumLiquid : TemporaryBodyLiquid
	{
		public const string ID = "ULTRA STRENGTH SERUM";

		public override string GetDisplayName()
		{
			return "Ultra strength serum";
		}

		public UltraStrengthSerumLiquid()
		{
			Color = new Color(0.7604228f, 182f / 255f, 1f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			if (!(limb.SpeciesIdentity == "Android"))
			{
				limb.Vitality *= 0.1f;
				limb.RegenerationSpeed += 8f;
				limb.ImpactPainMultiplier *= 0.1f;
				limb.InitialHealth += 350f;
				limb.Health = limb.InitialHealth;
				limb.BreakingThreshold += 20f;
				limb.BaseStrength = Mathf.Min(15f, limb.BaseStrength + 5f);
				limb.ShotDamageMultiplier *= 0.1f;
				limb.CirculationBehaviour.HealBleeding();
				limb.Wince(10f);
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "ULTRA STRENGTH SERUM";
	}
}
