using UnityEngine;

public class UseBubbler : MonoBehaviour, Messages.IUse, Messages.IUseContinuous
{
	public bool OnlyDirect;

	public void Use(ActivationPropagation activation)
	{
		if (!OnlyDirect || activation.Direct)
		{
			base.transform.parent.SendMessage("Use", activation, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void UseContinuous(ActivationPropagation activation)
	{
		ContinuousActivationBehaviour.AssertState();
		base.transform.parent.SendMessage("UseContinuous", activation, SendMessageOptions.DontRequireReceiver);
	}
}
