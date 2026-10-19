using UnityEngine;

public class Oil : Liquid
{
	public const string ID = "OIL";

	public override string GetDisplayName()
	{
		return "Oil";
	}

	public Oil()
	{
		Color = new Color(0f, 0.04689861f, 0.1803922f);
	}

	public override void OnEnterContainer(BloodContainer container)
	{
	}

	public override void OnEnterLimb(LimbBehaviour limb)
	{
		if (limb.SpeciesIdentity != "Android")
		{
			limb.Numbness += 0.5f;
			limb.CirculationBehaviour.IsPump = false;
		}
	}

	public override void OnExitContainer(BloodContainer container)
	{
	}

	public override void OnUpdate(BloodContainer container)
	{
		CirculationBehaviour circulationBehaviour = container as CirculationBehaviour;
		if ((object)circulationBehaviour != null)
		{
			OnEnterLimb(circulationBehaviour.Limb);
		}
	}
}
