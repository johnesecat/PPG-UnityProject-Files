using System;
using UnityEngine;

[NotDocumented]
[Obsolete]
public class NeedlerBehaviour : MonoBehaviour
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	public float FireInterval = 0.04f;

	public AudioClip[] FireSounds;

	public float FireVolume = 0.5f;

	public AudioSource Audio;

	public ParticleSystem flash;

	public GameObject needle;

	private void Use()
	{
		flash.transform.position = GetBarrelPosition();
		flash.transform.right = GetBarrelDirection();
		flash.Play();
		Audio.PlayOneShot(FireSounds.PickRandom(), FireVolume);
		UnityEngine.Object.Instantiate(needle, GetBarrelPosition(), base.transform.rotation).transform.right = base.transform.right * base.transform.localScale.x;
	}

	public Vector2 GetBarrelPosition()
	{
		return base.transform.TransformPoint(barrelPosition);
	}

	public Vector2 GetBarrelDirection()
	{
		return base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(GetBarrelPosition(), GetBarrelDirection());
	}
}
