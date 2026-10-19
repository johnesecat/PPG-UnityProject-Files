using UnityEngine;

public class ActivationTransformerBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	private bool shouldSend;

	private void Awake()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void Update()
	{
		bool flag = shouldSend;
		shouldSend = PhysicalBehaviour.Charge > 4.8f;
		if (shouldSend && !flag)
		{
			Utils.SendAllChannelIsolatedActivation(this);
		}
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}

	private void OnContinuousUpdate(float dt)
	{
		if (shouldSend)
		{
			Utils.SendAllChannelContinuousIsolatedActivation(this);
		}
	}
}
