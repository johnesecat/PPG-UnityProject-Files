using UnityEngine;

public class CoagulationSyringe : SyringeBehaviour
{
	public class CoagulationLiquid : TemporaryBodyLiquid
	{
		public const string ID = "COAGULATION SERUM";

		public override string GetDisplayName()
		{
			return "Coagulation serum";
		}

		public CoagulationLiquid()
		{
			Color = new Color(1f, 1f, 0f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
		}

		public override void OnUpdate(BloodContainer c)
		{
			base.OnUpdate(c);
			CirculationBehaviour circulationBehaviour = c as CirculationBehaviour;
			if ((object)circulationBehaviour != null)
			{
				LimbBehaviour limb = circulationBehaviour.Limb;
				if (!(limb.SpeciesIdentity == "Android"))
				{
					limb.CirculationBehaviour.HealBleeding();
				}
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "COAGULATION SERUM";
	}
}
