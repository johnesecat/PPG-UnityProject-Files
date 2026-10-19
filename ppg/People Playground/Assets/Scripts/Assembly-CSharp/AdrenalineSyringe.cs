using UnityEngine;

public class AdrenalineSyringe : SyringeBehaviour
{
	public class AdrenalineLiquid : TemporaryBodyLiquid
	{
		public const string ID = "ADRENALINE";

		public override string GetDisplayName()
		{
			return "Adrenaline";
		}

		public AdrenalineLiquid()
		{
			Color = new Color(0.45f, 0.27f, 0.77f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			PushAdrenaline(limb);
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
				PushAdrenaline(circulationBehaviour.Limb);
			}
		}

		private void PushAdrenaline(LimbBehaviour limb)
		{
			if (!(limb.SpeciesIdentity == "Android"))
			{
				float num = Mathf.Clamp01(limb.CirculationBehaviour.GetAmount(this) * 450f);
				limb.Numbness = 0f;
				if (num > 1.4f)
				{
					limb.Damage(10f * Random.value);
				}
				if (limb.RoughClassification == LimbBehaviour.BodyPart.Head)
				{
					limb.Person.AdrenalineLevel += 1f * num;
					limb.Person.Consciousness = Mathf.Max(num, limb.Person.Consciousness);
					limb.Person.PainLevel = Mathf.Max(0f, limb.Person.PainLevel - num);
					limb.Person.ShockLevel = Mathf.Max(0f, limb.Person.ShockLevel - num);
				}
			}
		}
	}

	public override string GetLiquidID()
	{
		return "ADRENALINE";
	}
}
