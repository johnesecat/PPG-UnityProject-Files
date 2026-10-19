using UnityEngine;

public class ElectricityTransformerBehaviour : MonoBehaviour, Messages.IUse, Messages.IUseContinuous
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public void UseContinuous(ActivationPropagation activation)
	{
		ContinuousActivationBehaviour.AssertState();
		if (base.enabled)
		{
			PhysicalBehaviour.charge += 5f;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			PhysicalBehaviour.charge += 50f;
		}
	}
}
