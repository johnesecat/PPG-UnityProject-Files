using UnityEngine;

public class FreezeSyringe : SyringeBehaviour
{
	public class FreezePoisonLiquid : TemporaryBodyLiquid
	{
		public const string ID = "FREEZE POISON";

		public override string GetDisplayName()
		{
			return "Joint lock poison";
		}

		public FreezePoisonLiquid()
		{
			Color = new Color(0.8f, 1f, 1f);
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
			if ((object)circulationBehaviour == null)
			{
				return;
			}
			LimbBehaviour limb = circulationBehaviour.Limb;
			if (!(limb.SpeciesIdentity == "Android"))
			{
				float num = Mathf.Clamp01(limb.CirculationBehaviour.GetAmount(this) * 750f);
				if (Random.value < num)
				{
					limb.Frozen = true;
				}
			}
		}
	}

	public override string GetLiquidID()
	{
		return "FREEZE POISON";
	}
}
