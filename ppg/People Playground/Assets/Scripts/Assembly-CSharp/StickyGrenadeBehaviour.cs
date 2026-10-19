using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyGrenadeBehaviour : MonoBehaviour, Messages.IOnEMPHit
{
	public float LightInterval = 1f;

	[SkipSerialisation]
	public SpriteRenderer light;

	[SkipSerialisation]
	public Color ArmedLightColour;

	[SkipSerialisation]
	public Color HotLightColor;

	[SkipSerialisation]
	public ExplosiveBehaviour Explosive;

	[SkipSerialisation]
	public PhysicalProperties[] StickyPropertiesToIgnore;

	private bool isStuck;

	private HashSet<PhysicalProperties> unstickyPropertySet;

	private FixedJoint2D joint;

	private int objectLayer;

	private int nocollisionLayer;

	private bool aboutToExplode;

	private bool isStuckToHuman;

	[SkipSerialisation]
	public AudioSource AudioSource;

	private void Awake()
	{
		unstickyPropertySet = new HashSet<PhysicalProperties>(StickyPropertiesToIgnore);
		StickyPropertiesToIgnore = null;
		objectLayer = LayerMask.NameToLayer("Objects");
		nocollisionLayer = LayerMask.NameToLayer("Debris");
	}

	public void StartBlinking()
	{
		StartCoroutine(Blink());
	}

	private void FixedUpdate()
	{
		bool flag = isStuck && (bool)joint && !joint.connectedBody;
		bool flag2 = !isStuck && (bool)joint;
		if ((isStuck && !joint) || flag2 || flag)
		{
			DestroyJoint();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!isStuck && Explosive.Armed && GetPhysicalBehaviour(collision, out var phys) && IsStickyWith(phys))
		{
			isStuck = true;
			CreateJoint(phys);
			if (phys.TryGetComponent<LimbBehaviour>(out var component) && component.SpeciesIdentity == "Human")
			{
				isStuckToHuman = true;
			}
		}
	}

	private void OnJointBreak2D(Joint2D broken)
	{
		if (!(broken != joint))
		{
			DestroyJoint();
		}
	}

	public void OnEMPHit()
	{
		Explosive.Disarm();
		Explosive.ArmOnUse = false;
		StopAllCoroutines();
		light.enabled = false;
		AudioSource.Stop();
		if (isStuckToHuman && aboutToExplode)
		{
			StatManager.UnlockAchievement("EMP_STICKY_GRENADE");
		}
	}

	private bool GetPhysicalBehaviour(Collision2D collision, out PhysicalBehaviour phys)
	{
		return Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out phys);
	}

	private bool IsStickyWith(PhysicalBehaviour phys)
	{
		return !unstickyPropertySet.Contains(phys.Properties);
	}

	private IEnumerator Blink()
	{
		light.color = ArmedLightColour;
		for (int j = 0; j < 12; j++)
		{
			light.enabled = !light.enabled;
			yield return new WaitForSeconds(0.474f);
		}
		light.color = HotLightColor;
		aboutToExplode = true;
		for (int j = 0; j < 14; j++)
		{
			light.enabled = !light.enabled;
			yield return new WaitForSeconds(0.061f);
		}
	}

	private void CreateJoint(PhysicalBehaviour phys)
	{
		if ((bool)joint)
		{
			Object.Destroy(joint);
		}
		joint = base.gameObject.AddComponent<FixedJoint2D>();
		joint.connectedBody = phys.rigidbody;
		base.gameObject.layer = nocollisionLayer;
	}

	private void DestroyJoint()
	{
		base.gameObject.layer = objectLayer;
		isStuck = false;
		if ((bool)joint)
		{
			Object.Destroy(joint);
		}
	}
}
