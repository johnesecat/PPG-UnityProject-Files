using UnityEngine;

public class PainKillerSyringe : SyringeBehaviour
{
	public class PainKillerLiquid : TemporaryBodyLiquid
	{
		public const string ID = "PAIN KILLER";

		public override string GetDisplayName()
		{
			return "Painkiller";
		}

		public PainKillerLiquid()
		{
			Color = new Color(0.71f, 0.75f, 0.65f, 0.1f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			KillPain(limb);
		}

		public override void OnUpdate(BloodContainer c)
		{
			base.OnUpdate(c);
			CirculationBehaviour circulationBehaviour = c as CirculationBehaviour;
			if ((object)circulationBehaviour != null)
			{
				LimbBehaviour limb = circulationBehaviour.Limb;
				KillPain(limb);
			}
		}

		private void KillPain(LimbBehaviour limb)
		{
			if (limb.SpeciesIdentity == "Android")
			{
				return;
			}
			limb.Numbness += 0.1f;
			if (!limb.NodeBehaviour.IsConnectedToRoot)
			{
				return;
			}
			limb.Person.PainLevel *= 0.1f;
			limb.Person.ShockLevel *= 0.1f;
			limb.Person.SeizureTime *= 0.5f;
			float amount = limb.CirculationBehaviour.GetAmount(this);
			if (amount > 0.4f)
			{
				limb.InfluenceMotorSpeed(0f);
				limb.Person.Wince(Random.value * 300f);
				if (amount > 0.5f)
				{
					limb.SkinMaterialHandler.AcidProgress += 0.05f;
					limb.Damage(limb.Health * 0.5f + 0.1f);
				}
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "PAIN KILLER";
	}
}
