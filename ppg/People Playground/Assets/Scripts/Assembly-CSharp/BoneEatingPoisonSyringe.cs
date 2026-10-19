using UnityEngine;

public class BoneEatingPoisonSyringe : SyringeBehaviour
{
	public class BoneHurtingJuiceLiquid : TemporaryBodyLiquid
	{
		public const string ID = "BONE EATING POISON";

		public override string GetDisplayName()
		{
			return "Bone eating poison";
		}

		public BoneHurtingJuiceLiquid()
		{
			Color = new Color(1f, 0.55f, 0.22f);
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
				float num = Mathf.Clamp01(limb.CirculationBehaviour.GetAmount(this) * 250f);
				if (Random.value < num)
				{
					limb.BreakBone();
				}
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "BONE EATING POISON";
	}
}
