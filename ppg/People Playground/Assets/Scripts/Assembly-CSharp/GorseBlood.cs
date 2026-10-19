using System.Collections;
using UnityEngine;

public class GorseBlood : Blood
{
	public new const string ID = "GORSE BLOOD";

	public override string GetDisplayName()
	{
		return "Gorse blood";
	}

	public GorseBlood()
	{
		Color = new Color(0.03f, 0.57f, 0f);
	}

	public override void OnEnterContainer(BloodContainer container)
	{
	}

	public override void OnExitContainer(BloodContainer container)
	{
	}

	public override void OnEnterLimb(LimbBehaviour limb)
	{
		base.OnEnterLimb(limb);
		if (!(limb.SpeciesIdentity == "Gorse"))
		{
			limb.CirculationBehaviour.StartCoroutine(AcidRoutine(limb));
		}
	}

	private IEnumerator AcidRoutine(LimbBehaviour limb)
	{
		while (limb.SkinMaterialHandler.AcidProgress < 0.99f)
		{
			float concentration = Mathf.Clamp01(limb.CirculationBehaviour.GetAmount(this) * 500f);
			if (limb.SpeciesIdentity == "Android")
			{
				concentration *= 0.1f;
			}
			limb.SkinMaterialHandler.AcidProgress += 0.002f * concentration;
			limb.Wince(150f * concentration * limb.SkinMaterialHandler.AcidProgress);
			if (limb.SkinMaterialHandler.AcidProgress > 0.5f)
			{
				if (limb.NodeBehaviour.IsConnectedToRoot)
				{
					limb.Person.AddPain(0.1f * concentration * limb.SkinMaterialHandler.AcidProgress);
				}
				limb.CirculationBehaviour.InternalBleedingIntensity += 0.005f * concentration;
				if (limb.SkinMaterialHandler.AcidProgress > 0.9f)
				{
					limb.BreakBone();
				}
			}
			yield return new WaitForSeconds(0.03f);
			if (concentration <= float.Epsilon)
			{
				yield break;
			}
		}
		limb.Health = 0f;
		limb.CirculationBehaviour.IsPump = false;
		limb.BreakingThreshold = 0f;
	}
}
