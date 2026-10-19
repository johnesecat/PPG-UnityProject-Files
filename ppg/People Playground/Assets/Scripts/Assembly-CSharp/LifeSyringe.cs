using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeSyringe : SyringeBehaviour
{
	public class LifeSerumLiquid : TemporaryBodyLiquid
	{
		public const string ID = "LIFE SERUM";

		public override string GetDisplayName()
		{
			return "Life serum";
		}

		public LifeSerumLiquid()
		{
			Color = new Color(143f / 212f, 1f, 0.7830077f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			if (!(limb.SpeciesIdentity == "Android"))
			{
				if (limb.HasBrain)
				{
					limb.Person.Braindead = false;
					limb.Person.BrainDamaged = false;
					limb.Person.BrainDamagedTime = 0f;
					limb.Person.SeizureTime = 0f;
				}
				GeneralHealing(limb);
				limb.CirculationBehaviour.StartCoroutine(HealBurn(limb));
				limb.CirculationBehaviour.StartCoroutine(HealWounds(limb));
				limb.CirculationBehaviour.StartCoroutine(HealRottenFlesh(limb));
				limb.CirculationBehaviour.StartCoroutine(HealAcidFlesh(limb));
				limb.LungsPunctured = false;
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}

		private void GeneralHealing(LimbBehaviour limb)
		{
			limb.HealBone();
			limb.Health = limb.InitialHealth;
			limb.Numbness = 0f;
			limb.CirculationBehaviour.HealBleeding();
			limb.CirculationBehaviour.IsPump = limb.CirculationBehaviour.WasInitiallyPumping;
			limb.CirculationBehaviour.BloodFlow = 1f;
			limb.CirculationBehaviour.AddLiquid(limb.GetOriginalBloodType(), Mathf.Max(0f, 1f - limb.CirculationBehaviour.GetAmount(limb.GetOriginalBloodType())));
			limb.BruiseCount = 0;
			limb.CirculationBehaviour.GunshotWoundCount = 0;
			limb.CirculationBehaviour.StabWoundCount = 0;
			if (limb.RoughClassification == LimbBehaviour.BodyPart.Head)
			{
				limb.Person.Consciousness = 1f;
				limb.Person.ShockLevel = 0f;
				limb.Person.PainLevel = 0f;
				limb.Person.OxygenLevel = 1f;
				limb.Person.AdrenalineLevel = 1f;
			}
		}

		private IEnumerator HealBurn(LimbBehaviour limb)
		{
			limb.PhysicalBehaviour.Temperature = limb.BodyTemperature;
			while (limb.PhysicalBehaviour.BurnProgress > 0.1f)
			{
				limb.PhysicalBehaviour.BurnProgress -= Time.deltaTime * 0.16f;
				yield return new WaitForEndOfFrame();
				if (limb.CirculationBehaviour.GetAmount(this) <= float.Epsilon)
				{
					yield break;
				}
			}
			limb.PhysicalBehaviour.BurnProgress = 0f;
		}

		private IEnumerator HealAcidFlesh(LimbBehaviour limb)
		{
			while (limb.SkinMaterialHandler.AcidProgress > 0.1f)
			{
				limb.SkinMaterialHandler.AcidProgress -= Time.deltaTime * 0.8f;
				yield return new WaitForEndOfFrame();
				if (limb.CirculationBehaviour.GetAmount(this) <= float.Epsilon)
				{
					yield break;
				}
			}
			limb.SkinMaterialHandler.AcidProgress = 0f;
		}

		private IEnumerator HealRottenFlesh(LimbBehaviour limb)
		{
			while (limb.SkinMaterialHandler.RottenProgress > 0.1f)
			{
				limb.SkinMaterialHandler.RottenProgress -= Time.deltaTime * 0.8f;
				yield return new WaitForEndOfFrame();
				if (limb.CirculationBehaviour.GetAmount(this) <= float.Epsilon)
				{
					yield break;
				}
			}
			limb.SkinMaterialHandler.RottenProgress = 0f;
		}

		private IEnumerator HealWounds(LimbBehaviour limb)
		{
			float factor = Mathf.Pow(0.5f, Time.deltaTime);
			while (((IEnumerable<Vector4>)limb.SkinMaterialHandler.damagePoints).Max((Func<Vector4, float>)intensity) > 0f)
			{
				for (int i = 0; i < limb.SkinMaterialHandler.damagePoints.Length; i++)
				{
					limb.SkinMaterialHandler.damagePoints[i].z *= factor;
				}
				limb.SkinMaterialHandler.Sync();
				yield return new WaitForEndOfFrame();
				if (limb.CirculationBehaviour.GetAmount(this) <= float.Epsilon)
				{
					break;
				}
			}
			float intensity(Vector4 c)
			{
				return c.z;
			}
		}
	}

	public override string GetLiquidID()
	{
		return "LIFE SERUM";
	}
}
