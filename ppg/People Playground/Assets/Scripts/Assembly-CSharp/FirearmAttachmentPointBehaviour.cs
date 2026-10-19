using System.Collections;
using UnityEngine;

public class FirearmAttachmentPointBehaviour : MonoBehaviour, Messages.IOnBeforeSerialise
{
	public FirearmAttachmentType.AttachmentType AttachmentType;

	public FirearmAttachmentBehaviour FirearmAttachment;

	private FirearmBehaviour firearmBehaviour;

	private HingeJoint2D joint;

	private bool ignoreConnect;

	private SpriteRenderer detailViewBlob;

	public void Start()
	{
		CreateDetailViewBlob();
		firearmBehaviour = base.transform.parent.GetComponent<FirearmBehaviour>();
		if ((bool)FirearmAttachment && !joint)
		{
			joint = firearmBehaviour.gameObject.AddComponent<HingeJoint2D>();
			joint.connectedBody = FirearmAttachment.gameObject.GetComponent<Rigidbody2D>();
			joint.autoConfigureConnectedAnchor = false;
			joint.connectedAnchor = Vector2.zero;
			joint.anchor = GetLocalJointAnchor();
			joint.useLimits = true;
			joint.limits = new JointAngleLimits2D
			{
				max = 0f,
				min = 0f
			};
			FirearmAttachment.FirearmBehaviour = firearmBehaviour;
			FirearmAttachment.AttachmentPoint = this;
			firearmBehaviour.OnFire.AddListener(FirearmAttachment.OnFire);
			firearmBehaviour.BallisticsEmitter.BulletEntryEvent.AddListener(FirearmAttachment.OnHit);
			FirearmAttachment.OnConnect();
			FirearmAttachment.transform.SetParent(firearmBehaviour.transform);
			detailViewBlob.enabled = false;
		}
	}

	private void CreateDetailViewBlob()
	{
		GameObject gameObject = new GameObject("attachment point indicator", typeof(ExistInDetailView), typeof(Optout));
		gameObject.transform.SetParent(base.transform);
		gameObject.layer = 16;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(0.1f, 0.1f);
		SpriteRenderer spriteRenderer = (detailViewBlob = gameObject.AddComponent<SpriteRenderer>());
		spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Circle");
		spriteRenderer.color = Color.cyan;
	}

	public void ConnectAttachment()
	{
		if (!FirearmAttachment.Connected)
		{
			FirearmAttachment.transform.localScale = firearmBehaviour.transform.localScale;
			FirearmAttachment.transform.rotation = firearmBehaviour.transform.rotation;
			joint = firearmBehaviour.gameObject.AddComponent<HingeJoint2D>();
			joint.connectedBody = FirearmAttachment.gameObject.GetComponent<Rigidbody2D>();
			joint.autoConfigureConnectedAnchor = false;
			joint.connectedAnchor = Vector2.zero;
			joint.anchor = GetLocalJointAnchor();
			joint.useLimits = true;
			joint.limits = new JointAngleLimits2D
			{
				max = 0f,
				min = 0f
			};
			FirearmAttachment.transform.SetParent(firearmBehaviour.transform);
			firearmBehaviour.OnFire.AddListener(FirearmAttachment.OnFire);
			firearmBehaviour.BallisticsEmitter.BulletEntryEvent.AddListener(FirearmAttachment.OnHit);
			FirearmAttachment.Connect();
			detailViewBlob.enabled = false;
		}
	}

	private Vector2 GetLocalJointAnchor()
	{
		float num = FirearmAttachment.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height * 0.5f * 0.028571429f - 0.014285714f;
		return firearmBehaviour.transform.InverseTransformPoint(base.transform.TransformPoint(FirearmAttachment.AttachmentOffset.x, FirearmAttachment.AttachmentOffset.y + num, 0f));
	}

	public void DisconnectAttachment()
	{
		ignoreConnect = true;
		Object.Destroy(joint);
		if ((bool)FirearmAttachment)
		{
			FirearmAttachment.OnDisconnect();
			FirearmAttachment.AttachmentPoint = null;
			FirearmAttachment.FirearmBehaviour = null;
			FirearmAttachment.Connected = false;
			firearmBehaviour.OnFire.RemoveListener(FirearmAttachment.OnFire);
			firearmBehaviour.BallisticsEmitter.BulletEntryEvent.RemoveListener(FirearmAttachment.OnHit);
			FirearmAttachment.transform.SetParent(null);
			FirearmAttachment = null;
		}
		detailViewBlob.enabled = true;
		StartCoroutine(ConnectDelay());
	}

	public void DisconnectOnAttachmentDestroyed()
	{
		ignoreConnect = false;
		Object.Destroy(joint);
		detailViewBlob.enabled = true;
	}

	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (!ignoreConnect && !FirearmAttachment && collider.gameObject.TryGetComponent<FirearmAttachmentBehaviour>(out FirearmAttachment))
		{
			if (FirearmAttachment.AttachmentType != AttachmentType)
			{
				FirearmAttachment = null;
				return;
			}
			FirearmAttachment.FirearmBehaviour = firearmBehaviour;
			FirearmAttachment.AttachmentPoint = this;
			ConnectAttachment();
		}
	}

	private void OnDestroy()
	{
		if ((bool)FirearmAttachment)
		{
			Object.Destroy(FirearmAttachment);
		}
	}

	private IEnumerator ConnectDelay()
	{
		yield return new WaitForSeconds(0.75f);
		ignoreConnect = false;
	}

	public void OnBeforeSerialise()
	{
		if ((bool)FirearmAttachment)
		{
			FirearmAttachment.transform.SetParent(null);
			StartCoroutine(Utils.NextFrameCoroutine(delegate
			{
				FirearmAttachment.transform.SetParent(firearmBehaviour.transform);
			}));
		}
	}
}
