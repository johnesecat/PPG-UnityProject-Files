using System;
using System.Collections;
using UnityEngine;

[Obsolete]
public class AcidPoison : PoisonSpreadBehaviour
{
	public override float SpreadSpeed => 5f;

	public override float Lifespan => 60f;

	public override void Start()
	{
		if (!(Limb.SpeciesIdentity == "Gorse"))
		{
			StartCoroutine(Acid());
		}
	}

	private IEnumerator Acid()
	{
		while (Limb.SkinMaterialHandler.AcidProgress < 0.99f)
		{
			Limb.SkinMaterialHandler.AcidProgress += Time.deltaTime * 0.2f;
			Limb.Wince(Limb.SkinMaterialHandler.AcidProgress * 150f);
			if (Limb.SkinMaterialHandler.AcidProgress > 0.5f)
			{
				Limb.BreakBone();
			}
			yield return new WaitForSeconds(0.03f);
		}
		Limb.Health = 0f;
		Limb.CirculationBehaviour.IsPump = false;
		Limb.CirculationBehaviour.ForceSetAllLiquid(0f);
		Limb.BreakingThreshold = 0f;
	}
}
