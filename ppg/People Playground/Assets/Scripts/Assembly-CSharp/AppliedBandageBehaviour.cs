using UnityEngine;

public class AppliedBandageBehaviour : SpringCableBehaviour
{
	public float SoakAmount;

	private float healingRatio = 0.001f;

	private LimbBehaviour limb;

	private Color bloodColour;

	public const float SoakRate = 1f;

	protected override void Start()
	{
		base.Start();
		lineRenderer.sortingLayerID = physicalBehaviour.spriteRenderer.sortingLayerID;
		lineRenderer.sortingOrder = physicalBehaviour.spriteRenderer.sortingOrder;
		typedJoint.frequency = 3f;
		typedJoint.dampingRatio = 0.2f;
		if (TryGetComponent<LimbBehaviour>(out var component))
		{
			if (component.IsAndroid)
			{
				return;
			}
			limb = component;
			component.Person.AdrenalineLevel += 0.25f;
			bloodColour = limb.GetOriginalBloodType().Color;
		}
		SetSoakColour();
	}

	private void FixedUpdate()
	{
		if ((bool)limb)
		{
			limb.CirculationBehaviour.BleedingRate *= 0.9f;
			limb.Health = Mathf.Lerp(limb.Health, limb.InitialHealth, healingRatio);
			SoakAmount = Mathf.Max(SoakAmount, Mathf.Clamp01(SoakAmount + Time.fixedDeltaTime * limb.CirculationBehaviour.BleedingRate * 0.5f));
			SetSoakColour();
		}
	}

	private void SetSoakColour()
	{
		if ((bool)limb)
		{
			Color color = Color.Lerp(Color.white, bloodColour, SoakAmount * 0.7f);
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
		}
	}
}
