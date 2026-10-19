using System.Collections.Generic;
using UnityEngine;

public class ImmobilityFieldBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public GameObject CaptureObject;

	[SkipSerialisation]
	public Collider2D Trigger;

	public float Strength;

	[SkipSerialisation]
	public AudioClip[] CaptureClips;

	[SkipSerialisation]
	public AudioClip EnableClip;

	public Dictionary<GameObject, (FrictionJoint2D joint, Collider2D coll)> joints = new Dictionary<GameObject, (FrictionJoint2D, Collider2D)>();

	[SkipSerialisation]
	public LayerMask Layers;

	private float FinalStrength => 2f * (Strength + 8f * PhysicalBehaviour.Charge);

	private void Start()
	{
		UpdateActivation();
	}

	private void UpdateActivation()
	{
		CaptureObject.SetActive(Activated);
		Trigger.enabled = Activated;
		DestroyAllJoints();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			if (Activated && (bool)EnableClip)
			{
				PhysicalBehaviour.PlayClipOnce(EnableClip);
			}
			UpdateActivation();
		}
	}

	private void OnDisable()
	{
		DestroyAllJoints();
		CaptureObject.SetActive(value: false);
		Trigger.enabled = false;
	}

	private void DestroyAllJoints()
	{
		foreach (KeyValuePair<GameObject, (FrictionJoint2D, Collider2D)> joint in joints)
		{
			if ((bool)joint.Key)
			{
				joint.Key.SendMessage("OnImmobilityRelease", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		FrictionJoint2D[] components = GetComponents<FrictionJoint2D>();
		for (int i = 0; i < components.Length; i++)
		{
			Object.Destroy(components[i]);
		}
		joints.Clear();
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	private void FixedUpdate()
	{
		float finalStrength = FinalStrength;
		foreach (KeyValuePair<GameObject, (FrictionJoint2D, Collider2D)> joint in joints)
		{
			if (!joint.Value.Item1 || !joint.Value.Item2 || !joint.Value.Item2.enabled)
			{
				if ((bool)joint.Key)
				{
					joint.Key.SendMessage("OnImmobilityRelease", this, SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)joint.Value.Item1)
				{
					Object.Destroy(joint.Value.Item1);
				}
			}
			else
			{
				joint.Value.Item1.anchor = base.transform.InverseTransformPoint(joint.Value.Item2.transform.position);
				joint.Value.Item1.maxForce = finalStrength;
				joint.Value.Item1.maxTorque = finalStrength;
				joint.Value.Item1.connectedBody.WakeUp();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Activated && !joints.ContainsKey(collision.gameObject) && Layers.HasLayer(collision.gameObject.layer) && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value))
		{
			FrictionJoint2D frictionJoint2D = base.gameObject.AddComponent<FrictionJoint2D>();
			frictionJoint2D.autoConfigureConnectedAnchor = false;
			frictionJoint2D.enableCollision = true;
			frictionJoint2D.maxForce = FinalStrength;
			frictionJoint2D.connectedBody = value.rigidbody;
			frictionJoint2D.maxTorque = FinalStrength;
			frictionJoint2D.anchor = base.transform.InverseTransformPoint(value.transform.position);
			frictionJoint2D.connectedAnchor = Vector2.zero;
			joints.Add(collision.gameObject, (frictionJoint2D, collision));
			PhysicalBehaviour.PlayClipOnce(CaptureClips.PickRandom());
			collision.gameObject.SendMessage("OnImmobilityCapture", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (joints.TryGetValue(collision.gameObject, out var value))
		{
			collision.gameObject.SendMessage("OnImmobilityRelease", this, SendMessageOptions.DontRequireReceiver);
			Object.Destroy(value.Item1);
			joints.Remove(collision.gameObject);
		}
	}

	private void OnDestroy()
	{
		DestroyAllJoints();
	}
}
