using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Obsolete]
public class LifePoison : PoisonSpreadBehaviour
{
	public override void Start()
	{
		StopAllCoroutines();
		GeneralHealing();
		StartCoroutine(HealBurn(Limb));
		StartCoroutine(HealWounds(Limb));
		StartCoroutine(HealRottenFlesh(Limb));
		StartCoroutine(HealAcidFlesh(Limb));
	}

	private void GeneralHealing()
	{
		Limb.HealBone();
		Limb.Health = Limb.InitialHealth;
		Limb.Numbness = 0f;
		Limb.CirculationBehaviour.HealBleeding();
		Limb.CirculationBehaviour.IsPump = Limb.CirculationBehaviour.WasInitiallyPumping;
		Limb.CirculationBehaviour.BloodFlow = 5f;
		Limb.CirculationBehaviour.AddLiquid(Limb.GetOriginalBloodType(), 5f);
		Limb.BruiseCount = 0;
		Limb.CirculationBehaviour.GunshotWoundCount = 0;
		Limb.CirculationBehaviour.StabWoundCount = 0;
		if (Limb.RoughClassification == LimbBehaviour.BodyPart.Head)
		{
			Limb.Person.Consciousness = 1f;
			Limb.Person.ShockLevel = 0f;
			Limb.Person.PainLevel = 0f;
			Limb.Person.OxygenLevel = 1f;
			Limb.Person.AdrenalineLevel = 1f;
		}
	}

	private IEnumerator HealBurn(LimbBehaviour limb)
	{
		limb.PhysicalBehaviour.Temperature = limb.BodyTemperature;
		while (limb.PhysicalBehaviour.BurnProgress > 0.1f)
		{
			limb.PhysicalBehaviour.BurnProgress -= Time.deltaTime * 0.16f;
			yield return new WaitForEndOfFrame();
		}
		limb.PhysicalBehaviour.BurnProgress = 0f;
	}

	private IEnumerator HealAcidFlesh(LimbBehaviour limb)
	{
		while (limb.SkinMaterialHandler.AcidProgress > 0.1f)
		{
			limb.SkinMaterialHandler.AcidProgress -= Time.deltaTime * 0.8f;
			yield return new WaitForEndOfFrame();
		}
		limb.SkinMaterialHandler.AcidProgress = 0f;
	}

	private IEnumerator HealRottenFlesh(LimbBehaviour limb)
	{
		while (limb.SkinMaterialHandler.RottenProgress > 0.1f)
		{
			limb.SkinMaterialHandler.RottenProgress -= Time.deltaTime * 0.8f;
			yield return new WaitForEndOfFrame();
		}
		limb.SkinMaterialHandler.RottenProgress = 0f;
	}

	private IEnumerator HealWounds(LimbBehaviour limb)
	{
		while (((IEnumerable<Vector4>)limb.SkinMaterialHandler.damagePoints).Max((Func<Vector4, float>)intensity) > 0f)
		{
			for (int i = 0; i < limb.SkinMaterialHandler.damagePoints.Length; i++)
			{
				limb.SkinMaterialHandler.damagePoints[i].z -= Time.deltaTime * 0.5f;
			}
			limb.SkinMaterialHandler.Sync();
			yield return new WaitForEndOfFrame();
		}
		float intensity(Vector4 c)
		{
			return c.z;
		}
	}
}
