using UnityEngine;

public class WaterBreathingSyringe : SyringeBehaviour
{
	public class WaterBreathingSerum : Liquid
	{
		public const string ID = "WATER BREATHING SERUM";

		public override string GetDisplayName()
		{
			return "Water breathing serum";
		}

		public WaterBreathingSerum()
		{
			Color = new Color(0.21f, 0.57f, 0.96f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
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
				LimbBehaviour limb = circulationBehaviour.Limb;
				if (!(limb.SpeciesIdentity == "Android") && limb.RoughClassification == LimbBehaviour.BodyPart.Head)
				{
					limb.Person.OxygenLevel = 1f;
				}
			}
		}
	}

	public override string GetLiquidID()
	{
		return "WATER BREATHING SERUM";
	}
}
