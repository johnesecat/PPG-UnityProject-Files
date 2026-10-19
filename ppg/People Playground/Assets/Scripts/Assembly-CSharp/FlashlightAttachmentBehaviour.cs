using UnityEngine;

public class FlashlightAttachmentBehaviour : FirearmAttachmentBehaviour, Messages.IOnEMPHit
{
	[SkipSerialisation]
	public SpriteRenderer[] Lights;

	public override void OnConnect()
	{
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].enabled = true;
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
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].enabled = false;
		}
	}

	public void OnEMPHit()
	{
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].enabled = false;
		}
	}
}
