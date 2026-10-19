public class BatteryAttachmentBehaviour : FirearmAttachmentBehaviour, Messages.IOnEMPHit
{
	[SkipSerialisation]
	public float BaseCharge = 5f;

	public override void OnConnect()
	{
		FirearmBehaviour.ApplyCharge = true;
	}

	public override void OnFire()
	{
	}

	private void FixedUpdate()
	{
		if ((bool)FirearmBehaviour)
		{
			FirearmBehaviour.Phys.Charge += BaseCharge;
		}
	}

	public override void OnHit(BallisticsEmitter.CallbackParams args)
	{
	}

	public override void OnDisconnect()
	{
		FirearmBehaviour.ApplyCharge = false;
	}

	public void OnEMPHit()
	{
		FirearmBehaviour.ApplyCharge = false;
	}
}
