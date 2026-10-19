using UnityEngine;

public class KnockoutSyringe : SyringeBehaviour
{
	public class KnockoutPoisonLiquid : TemporaryBodyLiquid
	{
		public const string ID = "KNOCKOUT POISON";

		public override string GetDisplayName()
		{
			return "Knockout poison";
		}

		public KnockoutPoisonLiquid()
		{
			Color = new Color(0.68f, 0.62f, 0.541f);
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
			if ((object)circulationBehaviour == null)
			{
				return;
			}
			LimbBehaviour limb = circulationBehaviour.Limb;
			if (!(limb.SpeciesIdentity == "Android"))
			{
				float num = Mathf.Clamp01(limb.CirculationBehaviour.GetAmount(this) * 350f);
				limb.Numbness = Mathf.Max(num, limb.Numbness);
				if (limb.RoughClassification == LimbBehaviour.BodyPart.Head)
				{
					limb.Person.Consciousness = Mathf.Min(1f - num, limb.Person.Consciousness);
				}
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "KNOCKOUT POISON";
	}
}
