using UnityEngine;

public class Gasoline : Liquid
{
	public const string ID = "GASOLINE";

	public override string GetDisplayName()
	{
		return "Gasoline";
	}

	public Gasoline()
	{
		Color = new Color(77f / 255f, 11f / 255f, 0f);
	}

	public override void OnEnterContainer(BloodContainer container)
	{
	}

	public override void OnExitContainer(BloodContainer container)
	{
	}

	public override void OnEnterLimb(LimbBehaviour limb)
	{
		limb.Color = new Color(79f / 85f, 13f / 15f, 218f / 255f);
		limb.CirculationBehaviour.IsPump = false;
	}

	public override void OnUpdate(BloodContainer container)
	{
		CirculationBehaviour circulationBehaviour = container as CirculationBehaviour;
		if ((object)circulationBehaviour != null && circulationBehaviour.Limb.SpeciesIdentity != "Android" && container.GetAmount(this) > 0.28f)
		{
			circulationBehaviour.Limb.Damage(10f * Random.value);
		}
	}
}
