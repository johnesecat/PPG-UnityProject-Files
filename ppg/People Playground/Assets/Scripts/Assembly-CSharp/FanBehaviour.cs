using System;
using NaughtyAttributes;
using UnityEngine;

public class FanBehaviour : MonoBehaviour, Messages.IUse
{
	public enum FanMode
	{
		Fan,
		Propeller
	}

	public float Strength;

	[ShowIf("IsFan")]
	public float Range = 5f;

	[ShowIf("IsFan")]
	public float AreaWidth;

	public Vector2 LocalStartPos;

	public Vector2 RelativeDirection = Vector2.down;

	public Vector2 DeathSize;

	public Vector2 DeathDirection;

	public Vector2 DeathLocalStartPos;

	private static readonly Collider2D[] buffer = new Collider2D[128];

	[SkipSerialisation]
	public SpriteRenderer FirstBladeRenderer;

	[SkipSerialisation]
	public SpriteRenderer SecondBladeRenderer;

	private PhysicalBehaviour physicalBehaviour;

	public bool Activated;

	[SkipSerialisation]
	public AudioSource Audio;

	[SkipSerialisation]
	public AudioClip Switch;

	[SkipSerialisation]
	public AudioClip StartupSound;

	[SkipSerialisation]
	public AudioClip ShutoffSound;

	[SkipSerialisation]
	[ShowIf("IsFan")]
	public LayerMask Mask;

	private float flipClock;

	private float clock;

	private bool flip;

	private float spinSpeed;

	public FanMode Mode;

	public const float HalfPi = (float)Math.PI / 2f;

	private bool IsFan()
	{
		return Mode == FanMode.Fan;
	}

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		UpdateActivated();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			Audio.PlayOneShot(Switch);
			UpdateActivated();
		}
	}

	private void Update()
	{
		SpinBlades();
	}

	private void SpinBlades()
	{
		float deltaTime = Time.deltaTime;
		if (Activated)
		{
			spinSpeed = Mathf.Lerp(spinSpeed, 40f * (physicalBehaviour.Charge + 1f), Utils.GetLerpFactorDeltaTime(0.9f, deltaTime));
		}
		else
		{
			spinSpeed = Mathf.Lerp(spinSpeed, 0f, Utils.GetLerpFactorDeltaTime(0.9f, deltaTime));
		}
		if (spinSpeed < 0f)
		{
			spinSpeed = 0f;
		}
		deltaTime *= spinSpeed;
		flipClock += deltaTime;
		clock += deltaTime;
		if (flipClock >= (float)Math.PI / 2f)
		{
			FirstBladeRenderer.sortingOrder = ((!flip) ? 1 : (-1));
			SecondBladeRenderer.sortingOrder = (flip ? 1 : (-1));
			flipClock %= (float)Math.PI / 2f;
			flip = !flip;
		}
		Vector3 localScale = new Vector3(Mathf.Abs(Mathf.Sin(clock + (float)Math.PI / 2f)), 1f, 1f);
		FirstBladeRenderer.transform.localScale = localScale;
		SecondBladeRenderer.transform.localScale = localScale;
	}

	private void FixedUpdate()
	{
		if (Activated)
		{
			Vector3 worldDir = base.transform.TransformVector(RelativeDirection);
			switch (Mode)
			{
			case FanMode.Fan:
				HoverAndPush(worldDir);
				Kill();
				break;
			case FanMode.Propeller:
				Fly(worldDir);
				break;
			}
		}
	}

	private void Fly(Vector3 worldDir)
	{
		physicalBehaviour.rigidbody.AddForce((physicalBehaviour.Charge + 1f) * 50f * Strength * worldDir);
	}

	private void HoverAndPush(Vector3 worldDir)
	{
		float length = Range * 2f + physicalBehaviour.Charge * 0.5f;
		Push(worldDir, length);
		Hover(worldDir, length);
	}

	private void Kill()
	{
		int num = Physics2D.OverlapBoxNonAlloc(base.transform.TransformPoint(DeathLocalStartPos), new Vector2(DeathSize.x, DeathSize.y), base.transform.eulerAngles.z, buffer, Mask);
		if (num == 0)
		{
			return;
		}
		Vector3 vector = base.transform.TransformVector(DeathDirection) * Mathf.Sin(Time.time * spinSpeed) * spinSpeed * 25f * (Mathf.Min(physicalBehaviour.Charge, 2f) + 1f);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (!(collider2D.transform == base.transform) && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				value.rigidbody.AddForce(vector);
				physicalBehaviour.rigidbody.AddForce(-vector);
			}
		}
	}

	private void UpdateActivated()
	{
		if (Activated)
		{
			Audio.Play();
			Audio.PlayOneShot(StartupSound);
			return;
		}
		Audio.Stop();
		if (spinSpeed > 30f)
		{
			Audio.PlayOneShot(ShutoffSound, 0.5f);
		}
	}

	private void Hover(Vector3 worldDir, float length)
	{
		if (physicalBehaviour.Wetness > 0.2f)
		{
			physicalBehaviour.rigidbody.AddForce((physicalBehaviour.Charge + 1f) * 50f * worldDir);
		}
		Vector3 vector = base.transform.TransformPoint(LocalStartPos);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, -worldDir, length, Mask);
		if (!raycastHit2D)
		{
			physicalBehaviour.rigidbody.AddForce((physicalBehaviour.Charge + 1f) * 20f * worldDir);
			return;
		}
		Debug.DrawRay(raycastHit2D.point, worldDir);
		Vector2 vector2 = vector - worldDir * length / 2f;
		Debug.DrawRay(vector2, worldDir);
		Vector2 vector3 = -20f * physicalBehaviour.rigidbody.mass * (vector2 - raycastHit2D.point);
		if (!(Vector2.Dot(worldDir.normalized, vector3.normalized) < 0f))
		{
			Debug.DrawLine(base.transform.position, (Vector2)base.transform.position + vector3);
			physicalBehaviour.rigidbody.AddForce(vector3);
			physicalBehaviour.rigidbody.velocity *= 0.98f;
		}
	}

	private void Push(Vector3 worldDir, float length)
	{
		float num = Strength * (1f + physicalBehaviour.Charge);
		int num2 = Physics2D.OverlapBoxNonAlloc(base.transform.TransformPoint(LocalStartPos), new Vector2(AreaWidth, length), Mathf.Atan2(worldDir.y, worldDir.x) * 57.29578f - 90f, buffer, Mask);
		Vector3 vector = 2f * (0f - num) * worldDir;
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = buffer[i];
			if (!(collider2D.transform == base.transform) && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				value.rigidbody.AddForce(vector * Mathf.Max(value.rigidbody.mass, 1f));
				value.Temperature = Mathf.Lerp(value.Temperature, PhysicalBehaviour.AmbientTemperature, 0.005f);
			}
		}
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivated();
	}
}
