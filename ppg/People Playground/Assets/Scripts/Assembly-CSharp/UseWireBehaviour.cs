using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseWireBehaviour : DistanceJointWireBehaviour, Messages.IIsolatedActivation, Messages.IIsolatedContinuousActivation, Messages.IUse, Messages.IUseContinuous
{
	protected float breakForceBuffer = 1f;

	private MaterialPropertyBlock materialPropertyBlock;

	public Color Color = new Color(1f, 11f, 0f);

	public ushort Channel;

	public bool CanBreak;

	private readonly Queue<ActivationPropagation> toSendOut = new Queue<ActivationPropagation>();

	private IUseEmitter target;

	private static float lastWarningTime = -1000f;

	private static bool GetIsNotPaused()
	{
		return Time.timeScale > float.Epsilon;
	}

	protected override void Start()
	{
		base.Start();
		materialPropertyBlock = new MaterialPropertyBlock();
		lineRenderer.GetPropertyBlock(materialPropertyBlock);
		lineRenderer.widthMultiplier = 0.05f;
		materialPropertyBlock.SetColor(ShaderProperties.Get("_GlowColour"), Color);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
		breakForceBuffer = (CanBreak ? currentBreakingForce : float.PositiveInfinity);
		typedJoint.breakForce = breakForceBuffer;
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
		if ((bool)typedJoint && (bool)typedJoint.connectedBody)
		{
			typedJoint.connectedBody.TryGetComponent<IUseEmitter>(out target);
		}
	}

	private void OnContinuousUpdate(float dt)
	{
		while (toSendOut.Count != 0)
		{
			ActivationPropagation activationPropagation = toSendOut.Dequeue();
			if (AssertProperState(activationPropagation))
			{
				target?.OnContinuousUse.Invoke(activationPropagation);
				activationPropagation.Target = typedJoint.connectedBody.gameObject;
				typedJoint.connectedBody.BroadcastMessage("UseContinuous", activationPropagation, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	protected override void Tick()
	{
		base.Tick();
		if (CanBreak)
		{
			float num = 0f;
			if ((bool)physicalBehaviour)
			{
				num = physicalBehaviour.BurnProgress;
			}
			if ((bool)otherPhysicalBehaviour)
			{
				num = Mathf.Max(num, otherPhysicalBehaviour.BurnProgress);
			}
			typedJoint.breakForce = breakForceBuffer * (1f - num);
		}
	}

	public void IsolatedActivation(ActivationPropagation activation)
	{
		if (activation.Path == null)
		{
			Use(new ActivationPropagation(direct: false, Channel));
		}
		else
		{
			Use(activation);
		}
	}

	public void IsolatedContinuousActivation(ActivationPropagation activation)
	{
		if (activation.Path == null)
		{
			UseContinuous(new ActivationPropagation(direct: false, Channel));
		}
		else
		{
			UseContinuous(activation);
		}
	}

	private void SendGlow()
	{
		materialPropertyBlock.SetFloat(ShaderProperties.Get("_Timestamp"), Time.timeSinceLevelLoad);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	public void Use(ActivationPropagation activation)
	{
		if (ActivationPropagation.CurrentlyActiveSignals >= 10000)
		{
			ShowTooManyPropagationsWarning();
		}
		else if (!(physicalBehaviour.ActivationPropagationDelay < 0f) && ValidateSignal(ref activation) && (physicalBehaviour.SendUserPropagation || !activation.Direct))
		{
			if (activation.TraversedCount > 10000)
			{
				ShowTooManyPropagationsWarning();
			}
			else
			{
				StartCoroutine(SendUseMessage(activation.Branch(base.transform.root)));
			}
		}
	}

	private bool ValidateSignal(ref ActivationPropagation activation)
	{
		if (activation.Direct)
		{
			activation.Channel = Channel;
			return true;
		}
		return activation.Channel == Channel;
	}

	private IEnumerator SendUseMessage(ActivationPropagation activation)
	{
		activation.ActivityIntensity++;
		yield return new WaitForSeconds(physicalBehaviour.ActivationPropagationDelay);
		if (AssertProperState(activation))
		{
			activation.Target = typedJoint.connectedBody.gameObject;
			typedJoint.connectedBody.BroadcastMessage("Use", activation, SendMessageOptions.DontRequireReceiver);
			target?.OnSingleUse.Invoke(activation);
			SendGlow();
			yield return new WaitForEndOfFrame();
		}
	}

	public void UseContinuous(ActivationPropagation activation)
	{
		ContinuousActivationBehaviour.AssertState();
		if (ActivationPropagation.CurrentlyActiveSignals >= 10000)
		{
			ShowTooManyPropagationsWarning();
		}
		else if (!(physicalBehaviour.ActivationPropagationDelay < 0f) && ValidateSignal(ref activation) && (physicalBehaviour.SendUserPropagation || !activation.Direct))
		{
			if (activation.TraversedCount > 10000)
			{
				ShowTooManyPropagationsWarning();
			}
			else
			{
				StartCoroutine(SendUseContinuousMessage(activation.Branch(base.transform.root)));
			}
		}
	}

	private IEnumerator SendUseContinuousMessage(ActivationPropagation activation)
	{
		activation.ActivityIntensity++;
		yield return new WaitForSeconds(physicalBehaviour.ActivationPropagationDelay);
		if (AssertProperState(activation))
		{
			toSendOut.Enqueue(activation);
			SendGlow();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}

	public override void Slice()
	{
		breakForceBuffer = 0f;
	}

	protected override void JointBroken()
	{
		if ((bool)physicalBehaviour)
		{
			physicalBehaviour.PlayClipOnce(Resources.LoadAll<AudioClip>("Audio/use wire break").PickRandom(), Random.value);
		}
	}

	private bool AssertProperState(ActivationPropagation activation)
	{
		if (typedJoint.connectedBody == null)
		{
			return false;
		}
		if (activation.Contains(typedJoint.connectedBody.transform.root))
		{
			return false;
		}
		return true;
	}

	private static void ShowTooManyPropagationsWarning()
	{
		if (Time.timeSinceLevelLoad - lastWarningTime > 1f)
		{
			if (Time.timeSinceLevelLoad - lastWarningTime > 10f)
			{
				UISoundBehaviour.Main.Warning();
			}
			NotificationControllerBehaviour.Show("<color=#ff7000>Too many activation signals!</color>");
			lastWarningTime = Time.timeSinceLevelLoad;
		}
	}
}
