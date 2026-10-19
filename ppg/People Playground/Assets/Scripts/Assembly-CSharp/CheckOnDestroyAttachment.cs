using UnityEngine;

public class CheckOnDestroyAttachment : MonoBehaviour
{
	public FirearmAttachmentBehaviour Attachment;

	public void OnDestroy()
	{
		if (!(Attachment == null))
		{
			Attachment.OnDisconnect();
			if ((bool)Attachment.AttachmentPoint)
			{
				Attachment.AttachmentPoint.DisconnectOnAttachmentDestroyed();
			}
		}
	}
}
