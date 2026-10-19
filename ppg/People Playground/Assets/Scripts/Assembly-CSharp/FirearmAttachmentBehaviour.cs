using UnityEngine;

public abstract class FirearmAttachmentBehaviour : MonoBehaviour, Messages.IUse
{
	public FirearmAttachmentType.AttachmentType AttachmentType;

	public FirearmBehaviour FirearmBehaviour;

	public FirearmAttachmentPointBehaviour AttachmentPoint;

	public Vector2 AttachmentOffset;

	[SkipSerialisation]
	public AudioClip ConnectClip;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public bool Connected;

	private void Awake()
	{
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
		base.gameObject.GetOrAddComponent<CheckOnDestroyAttachment>().Attachment = this;
	}

	public void Use(ActivationPropagation propagation)
	{
		if ((bool)AttachmentPoint)
		{
			Debug.Log(propagation.Target);
			if (propagation.Target == base.transform.gameObject)
			{
				AttachmentPoint.DisconnectAttachment();
			}
		}
	}

	public void Connect()
	{
		if (!Connected)
		{
			Connected = true;
			if ((bool)ConnectClip)
			{
				PhysicalBehaviour.PlayClipOnce(ConnectClip);
			}
			OnConnect();
		}
	}

	public abstract void OnFire();

	public abstract void OnHit(BallisticsEmitter.CallbackParams args);

	public abstract void OnConnect();

	public abstract void OnDisconnect();
}
