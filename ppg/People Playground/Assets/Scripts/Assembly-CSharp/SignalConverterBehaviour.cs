using UnityEngine;

public class SignalConverterBehaviour : MonoBehaviour, Messages.IUse, Messages.IUseContinuous
{
	public ushort ChannelA;

	public ushort ChannelB;

	public void Use(ActivationPropagation propagation)
	{
		if (ConvertChannel(propagation, out var outgoing))
		{
			SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, outgoing), SendMessageOptions.DontRequireReceiver);
		}
	}

	public void UseContinuous(ActivationPropagation propagation)
	{
		ContinuousActivationBehaviour.AssertState();
		if (ConvertChannel(propagation, out var outgoing))
		{
			SendMessage("IsolatedContinuousActivation", new ActivationPropagation(base.transform.root, outgoing), SendMessageOptions.DontRequireReceiver);
		}
	}

	public bool ConvertChannel(ActivationPropagation incoming, out ushort outgoing)
	{
		outgoing = incoming.Channel;
		if (incoming.Direct)
		{
			return false;
		}
		if (incoming.Channel == ChannelA)
		{
			outgoing = ChannelB;
			return true;
		}
		if (incoming.Channel == ChannelB)
		{
			outgoing = ChannelA;
			return true;
		}
		return false;
	}
}
