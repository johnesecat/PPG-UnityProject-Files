using UnityEngine;

public class MendingSyringe : SyringeBehaviour
{
	public class MendingSerum : TemporaryBodyLiquid
	{
		public const string ID = "MENDING SERUM";

		public override float RemovalChancePerSecond => base.RemovalChancePerSecond * 0.8f;

		public override string GetDisplayName()
		{
			return "Mending serum";
		}

		public MendingSerum()
		{
			Color = new Color(0f, 0.67f, 0.5f, 1f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			if (limb.SpeciesIdentity == "Android")
			{
				return;
			}
			if (limb.HasBrain)
			{
				limb.Person.Braindead = false;
				if (Random.value > 0.995f)
				{
					limb.Person.BrainDamaged = false;
				}
			}
			limb.Health = limb.InitialHealth;
			limb.CirculationBehaviour.BleedingRate *= 0.8f;
			limb.CirculationBehaviour.InternalBleedingIntensity *= 0.8f;
			limb.SkinMaterialHandler.RottenProgress *= 0.8f;
			limb.LungsPunctured = false;
			limb.HealBone();
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "MENDING SERUM";
	}
}
