using System;
using UnityEngine;

public class PyroPowerToolEntity : MonoBehaviour
{
	public ParticleSystem ParticleSystem;

	public ParticleSystem LargePuff;

	public ParticleSystem DirectionalPuff;

	private bool isHeld;

	public float Radius = 1f;

	public int RayCount = 32;

	public float RayAngle = 25f;

	public float MinRange = 1f;

	public float Range = 20f;

	public float Force = 1f;

	public LayerMask Layers;

	public bool IsCurrentTool;

	public AudioSource AudioSource;

	public AudioClip StartSound;

	public AudioClip EndSound;

	public AudioSource AudioSourceBlast;

	protected Vector2 mouseMovement;

	protected Vector2 oldMousePos;

	private static readonly Collider2D[] buffer = new Collider2D[8];

	private bool blastSoundAvailable = true;

	public void StartHold()
	{
		isHeld = true;
		ParticleSystem.Play();
		AudioSource.Play();
		AudioSource.PlayOneShot(StartSound);
	}

	public void StopHold()
	{
		ParticleSystem.Stop();
		AudioSource.Stop();
		if (isHeld)
		{
			AudioSource.PlayOneShot(EndSound);
		}
		isHeld = false;
	}

	private void FixedUpdate()
	{
		if (IsCurrentTool)
		{
			mouseMovement = 1.5f * ((Vector2)Global.main.MousePosition - oldMousePos);
			if (isHeld)
			{
				SetFire();
			}
			oldMousePos = Global.main.MousePosition;
		}
	}

	private void SetFire()
	{
		Vector2 vector = base.transform.position;
		float num = Time.deltaTime * 0.1f;
		int num2 = Physics2D.OverlapCircleNonAlloc(Global.main.MousePosition, Radius, buffer, Layers);
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = buffer[i];
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				value.Ignite(ignoreFlammability: true);
				value.BurnProgress += num;
				if (value.SimulateTemperature && value.Temperature < 400f)
				{
					value.Temperature += 40f;
				}
			}
		}
		float num3 = Mathf.Min(mouseMovement.sqrMagnitude, Range);
		if (num3 < MinRange)
		{
			blastSoundAvailable = true;
			return;
		}
		Vector2 normalized = mouseMovement.normalized;
		if (blastSoundAvailable)
		{
			blastSoundAvailable = false;
			CameraShakeBehaviour.main.Shake(5f, base.transform.position, 0.8f);
			AudioSourceBlast.PlayOneShot(AudioSourceBlast.clip);
		}
		for (int j = 0; j < 50; j++)
		{
			Vector2 randomPosInCone = GetRandomPosInCone(normalized, num3);
			DirectionalPuff.Emit(new ParticleSystem.EmitParams
			{
				position = vector + randomPosInCone,
				rotation = Mathf.Atan2(0f - normalized.y, normalized.x) * 57.29578f + 90f,
				startColor = Color.Lerp(Color.gray, new Color(0f, 0f, 0f, 0f), randomPosInCone.sqrMagnitude / (num3 * num3))
			}, 1);
		}
		for (int k = 0; k < RayCount; k++)
		{
			Vector2 rayDir = GetRayDir(normalized, k);
			Debug.DrawLine(vector, num3 * rayDir + vector, Color.red);
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, rayDir, num3, Layers);
			if ((bool)raycastHit2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value2))
			{
				LargePuff.Emit(new ParticleSystem.EmitParams
				{
					position = raycastHit2D.point
				}, 1);
				value2.rigidbody.AddForce(Force * Mathf.Sqrt(value2.rigidbody.mass) * num3 * rayDir);
				value2.Ignite(ignoreFlammability: true);
				value2.BurnProgress += 0.2f;
				value2.burnIntensity = 1f;
			}
		}
	}

	private Vector2 GetRayDir(Vector2 initial, int index)
	{
		float f = Mathf.Atan2(initial.y, initial.x) + (Mathf.Lerp(0f, RayAngle, (float)index / ((float)RayCount - 1f)) - RayAngle / 2f) * ((float)Math.PI / 180f);
		return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
	}

	private Vector2 GetRandomPosInCone(Vector2 initial, float maxRange)
	{
		float f = Mathf.Atan2(initial.y, initial.x) + (Mathf.Lerp(0f, RayAngle, UnityEngine.Random.value) - RayAngle / 2f) * ((float)Math.PI / 180f);
		return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * UnityEngine.Random.value * maxRange;
	}
}
