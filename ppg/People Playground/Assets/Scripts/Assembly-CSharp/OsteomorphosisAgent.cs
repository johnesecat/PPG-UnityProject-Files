using UnityEngine;

public class OsteomorphosisAgent : TemporaryBodyLiquid
{
	public const string ID = "OSTEOMORPHOSIS AGENT";

	public override bool ShouldCallOnEnterEveryUpdate => true;

	public override float RemovalChancePerSecond => base.RemovalChancePerSecond * 0.1f;

	public override string GetDisplayName()
	{
		return "Osteomorphosis agent";
	}

	public OsteomorphosisAgent()
	{
		Color = new Color(1f, 0.84f, 0.56f, 1f);
	}

	public override void OnEnterContainer(BloodContainer container)
	{
	}

	public override void OnEnterLimb(LimbBehaviour limb)
	{
		if (!(limb.SpeciesIdentity == "Android"))
		{
			limb.SkinMaterialHandler.AcidProgress += 0.1f * Random.value;
			if (limb.NodeBehaviour.IsConnectedToRoot)
			{
				limb.Person.AddPain(5f);
			}
		}
	}

	public override void OnExitContainer(BloodContainer container)
	{
	}
}
