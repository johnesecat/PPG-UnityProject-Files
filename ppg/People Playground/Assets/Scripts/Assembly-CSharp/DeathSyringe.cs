using UnityEngine;

public class DeathSyringe : SyringeBehaviour
{
	public class InstantDeathPoisonLiquid : TemporaryBodyLiquid
	{
		public const string ID = "INSTANT DEATH POISON";

		public override string GetDisplayName()
		{
			return "Instant death poison";
		}

		public InstantDeathPoisonLiquid()
		{
			Color = new Color(1f, 0.65f, 0.63f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			_ = limb.SpeciesIdentity == "Android";
		}

		public override void OnUpdate(BloodContainer c)
		{
			base.OnUpdate(c);
			CirculationBehaviour circulationBehaviour = c as CirculationBehaviour;
			if ((object)circulationBehaviour != null)
			{
				LimbBehaviour limb = circulationBehaviour.Limb;
				limb.Health = 0f;
				limb.CirculationBehaviour.IsPump = false;
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "INSTANT DEATH POISON";
	}
}
