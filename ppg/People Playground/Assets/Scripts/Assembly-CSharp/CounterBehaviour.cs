using System.Collections.Generic;
using UnityEngine;

public class CounterBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public NixieTubeBehaviour NixieTubeBehaviour;

	[SkipSerialisation]
	public AudioClip SignalClip;

	public int CurrentCounter;

	public int MinValue;

	public int MaxValue = 99;

	public int ResetValue = 100;

	private PhysicalBehaviour phys;

	private void Start()
	{
		NixieTubeBehaviour.SetValue(CurrentCounter);
		phys = GetComponent<PhysicalBehaviour>();
		phys.ActivationPropagationDelay = -1f;
		List<ContextMenuButton> buttons = phys.ContextMenuOptions.Buttons;
		buttons.Add(new ContextMenuButton("resetCounter", "Clear counter", "Clear counter value", delegate
		{
			CurrentCounter = MinValue;
			NixieTubeBehaviour.SetValue(CurrentCounter);
		}));
		buttons.Add(new ContextMenuButton("setCounterWrap", "Set signal value", "Set counter signal value", delegate
		{
			Utils.OpenIntInputDialog(ResetValue, this, delegate(CounterBehaviour t, int v)
			{
				t.ResetValue = Mathf.Clamp(v, MinValue, MaxValue + 1);
			}, "Set value at which to reset the counter and emit a signal", $"Value from {MinValue} to {MaxValue + 1}");
		}));
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			switch (activation.Channel)
			{
			case 1:
				CurrentCounter--;
				break;
			case 2:
				CurrentCounter = MinValue;
				break;
			default:
				CurrentCounter++;
				break;
			}
			int currentCounter = CurrentCounter;
			CurrentCounter = Mathf.Clamp(CurrentCounter, MinValue, MaxValue);
			if (CurrentCounter >= ResetValue || currentCounter >= ResetValue)
			{
				CurrentCounter = MinValue;
			}
			else if (CurrentCounter < MinValue || currentCounter < MinValue)
			{
				CurrentCounter = MaxValue;
			}
			if (CurrentCounter == ResetValue || currentCounter == ResetValue)
			{
				SendSignal();
			}
			NixieTubeBehaviour.SetValue(CurrentCounter);
		}
	}

	private void SendSignal()
	{
		phys.PlayClipOnce(SignalClip);
		phys.ActivationPropagationDelay = 0.1f;
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			ushort channel = ActivationPropagation.AllChannels[i];
			BroadcastMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
		}
		phys.ActivationPropagationDelay = -1f;
	}
}
