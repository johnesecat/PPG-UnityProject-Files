using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TransmitterBehaviour : MonoBehaviour, IChannel, Messages.IUse, Messages.IUseContinuous
{
	private static HashSet<TransmitterBehaviour> transmitters = new HashSet<TransmitterBehaviour>();

	public string Channel = "0";

	[SkipSerialisation]
	public SpriteRenderer Light;

	[SkipSerialisation]
	public TextMeshPro TextMeshPro;

	private static readonly Dictionary<string, AudioClip> uniqueSounds = new Dictionary<string, AudioClip>();

	private AudioClip channelSound;

	private PhysicalBehaviour physicalBehaviour;

	public string GetChannel()
	{
		return Channel;
	}

	private void Awake()
	{
		transmitters.Add(this);
	}

	private void Start()
	{
		SetChannel(Channel);
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		physicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("changeRadioChannel", "Edit channel", "Change the radio channel", delegate
		{
			Utils.OpenTextInputDialog(Channel, this, delegate(TransmitterBehaviour instance, string value)
			{
				instance.SetChannel(value);
			}, "Channel name", "Enter channel name");
		}));
	}

	private void SetChannel(string channel)
	{
		Channel = channel;
		TextMeshPro.text = channel;
		channelSound = LoadOrCreateFor(channel);
	}

	private AudioClip LoadOrCreateFor(string channel)
	{
		if (channel.Length > 8)
		{
			channel = channel.Substring(0, 8);
		}
		if (uniqueSounds.TryGetValue(channel, out var value))
		{
			return value;
		}
		int num = 320 * channel.Length;
		AudioClip audioClip = AudioClip.Create(channel + " clip", num, 1, 8000, stream: false);
		float[] array = new float[num];
		int num2 = 0;
		string text = channel;
		for (int i = 0; i < text.Length; i++)
		{
			byte b = (byte)text[i];
			for (int j = 0; j < 320; j++)
			{
				int num3 = j + num2;
				float num4 = 200f + 8f * (float)(int)b;
				float num5 = (float)j / 320f;
				array[num3] = Mathf.Round(Mathf.Sin((float)Math.PI * 2f * (float)num3 * num4 / 8000f)) * num5 * 0.25f;
			}
			num2 += 320;
		}
		audioClip.SetData(array, 0);
		uniqueSounds.Add(channel, audioClip);
		return audioClip;
	}

	private bool IsLooping(ActivationPropagation p)
	{
		if (p.Path == null)
		{
			return false;
		}
		return p.Path.Intersect(from t in transmitters
			where t.Channel == Channel
			select t.transform.root).Any();
	}

	public virtual void Use(ActivationPropagation p)
	{
		if (base.enabled && !IsLooping(p))
		{
			StartCoroutine(Blink());
			Transmit("Use", new ActivationPropagation(base.transform.root, p.Channel, base.gameObject));
		}
	}

	public virtual void UseContinuous(ActivationPropagation p)
	{
		ContinuousActivationBehaviour.AssertState();
		if (base.enabled && !IsLooping(p))
		{
			Transmit("UseContinuous", new ActivationPropagation(base.transform.root, p.Channel));
		}
	}

	private void Transmit(string name, object content)
	{
		foreach (TransmitterBehaviour transmitter in transmitters)
		{
			if (transmitter != this && transmitter.enabled && transmitter.Channel == Channel)
			{
				transmitter.SendMessage(name, content, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		Light.enabled = false;
		TextMeshPro.enabled = false;
	}

	private void OnEnable()
	{
		TextMeshPro.enabled = true;
	}

	private void OnDestroy()
	{
		transmitters.Remove(this);
	}

	private IEnumerator Blink()
	{
		if ((bool)channelSound)
		{
			physicalBehaviour.PlayClipOnce(channelSound);
		}
		Light.enabled = true;
		yield return new WaitForSeconds(0.05f);
		Light.enabled = false;
	}
}
