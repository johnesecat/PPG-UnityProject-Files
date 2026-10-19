using UnityEngine;

public class Nitroglycerine : Liquid
{
	public const string ID = "NITRO";

	public const string ExplosiveBehaviourTag = "nitro";

	public override string GetDisplayName()
	{
		return "Nitroglycerine";
	}

	public Nitroglycerine()
	{
		Color = new Color(245f, 152f, 66f) / 255f;
		Color.a = 1f;
	}

	public override void OnEnterContainer(BloodContainer container)
	{
		if (!container.gameObject.TryGetComponent<ExplosiveBehaviour>(out var component) || !component.CompareTag("nitro"))
		{
			ExplosiveBehaviour explosiveBehaviour = container.gameObject.AddComponent<ExplosiveBehaviour>();
			explosiveBehaviour.Tag = "nitro";
			explosiveBehaviour.Range = 5f;
			explosiveBehaviour.Delay = 0f;
			explosiveBehaviour.FragmentationRayCount = 8u;
			explosiveBehaviour.DestroyOnExplode = container.GetComponent<LimbBehaviour>();
			explosiveBehaviour.ShouldCreateKillzone = false;
			explosiveBehaviour.ArmOnUse = false;
			explosiveBehaviour.MinChargeToExplode = 4f;
			explosiveBehaviour.ExplodesOnFragmentHit = true;
			explosiveBehaviour.ImpactForceThreshold = 2f;
			explosiveBehaviour.DismemberChance = 0.2f;
			explosiveBehaviour.ExplodeBurnProgressThreshold = 0.5f;
		}
	}

	public override void OnExitContainer(BloodContainer container)
	{
		if (container.gameObject.TryGetComponent<ExplosiveBehaviour>(out var component) && component.Tag == "nitro")
		{
			Object.Destroy(component);
		}
	}

	public override void OnEnterLimb(LimbBehaviour limb)
	{
		limb.Person.Wince(12f);
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
