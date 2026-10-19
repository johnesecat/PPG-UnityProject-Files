using UnityEngine;

public class ScopeAttachmentBehaviour : FirearmAttachmentBehaviour, Messages.IOnEMPHit
{
	[Range(0f, 100f)]
	public int AccuracyPercent = 25;

	public override void OnConnect()
	{
		FirearmBehaviour.InaccuracyMultipliers.Add(1f - (float)AccuracyPercent / 100f);
		if (TryGetComponent<LaserBehaviour>(out var component))
		{
			component.Activated = true;
			component.UpdateActivation();
			component.barrelDirection = FirearmBehaviour.BarrelDirection * Vector2.right;
		}
	}

	public override void OnFire()
	{
	}

	public override void OnHit(BallisticsEmitter.CallbackParams args)
	{
	}

	public override void OnDisconnect()
	{
		FirearmBehaviour.InaccuracyMultipliers.Remove(1f - (float)(AccuracyPercent / 100));
		if (TryGetComponent<LaserBehaviour>(out var component))
		{
			component.Activated = false;
			component.UpdateActivation();
		}
	}

	public void OnEMPHit()
	{
		if (TryGetComponent<LaserBehaviour>(out var component))
		{
			component.Activated = false;
			component.UpdateActivation();
		}
	}
}
