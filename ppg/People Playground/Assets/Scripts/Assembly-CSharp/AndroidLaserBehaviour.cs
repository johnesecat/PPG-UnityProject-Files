using System;
using UnityEngine;

[Obsolete]
public class AndroidLaserBehaviour : MonoBehaviour, Messages.IUse
{
	public AudioSource audioSource;

	public PhysicalBehaviour trigger;

	public ParticleSystem particles;

	public int cooldownTime;

	public float recoil;

	public float hitKnockback;

	private float cooldown;

	public LayerMask HitLayer;

	private void Update()
	{
		if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		if (trigger.Charge > 10f)
		{
			Fire();
		}
	}

	public void Use(ActivationPropagation activation)
	{
		Fire();
	}

	private void Fire()
	{
		if (!(cooldown > 0f))
		{
			cooldown = cooldownTime;
			audioSource.PlayOneShot(audioSource.clip);
			trigger.rigidbody.AddForceAtPosition(base.transform.right * (0f - recoil), base.transform.position, ForceMode2D.Impulse);
			particles.Play();
			RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, base.transform.right, 1000f, HitLayer);
			if ((bool)raycastHit2D)
			{
				HandleHit(raycastHit2D);
			}
		}
	}

	private void HandleHit(RaycastHit2D hit)
	{
		ExplosionCreator.CreatePulseExplosion(hit.point, hitKnockback, 10f, soundAndEffects: true);
		PhysicalBehaviour component = hit.transform.GetComponent<PhysicalBehaviour>();
		if ((bool)component)
		{
			component.rigidbody.AddForceAtPosition(base.transform.right * hitKnockback * component.rigidbody.mass, hit.point, ForceMode2D.Impulse);
			component.BurnProgress += UnityEngine.Random.value;
			if ((double)UnityEngine.Random.value > 0.95)
			{
				component.Ignite();
			}
		}
	}
}
