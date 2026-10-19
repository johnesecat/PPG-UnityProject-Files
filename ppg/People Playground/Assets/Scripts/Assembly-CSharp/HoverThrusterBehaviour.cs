using NaughtyAttributes;
using UnityEngine;

public class HoverThrusterBehaviour : MonoBehaviour, Messages.IUse
{
	public float BaseHoverHeight = 1.2136364f;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer GlowSprite;

	[SkipSerialisation]
	public SpriteRenderer Light;

	[SkipSerialisation]
	public ParticleSystem Effects;

	[SkipSerialisation]
	public AudioSource IdleLoop;

	[SkipSerialisation]
	public Transform[] HoverPoints;

	[SkipSerialisation]
	public LayerMask Layers;

	public float ForceMultiplier = 1f;

	public float FactorPower = 2f;

	public float VelocityRetention = 0.99f;

	public bool Activated;

	[SkipSerialisation]
	[ReadOnly]
	public float[] calibratedHoverHeights;

	public const float MinHeight = 267f / 440f;

	public const float MaxHeight = 12136.364f;

	private void Awake()
	{
		calibratedHoverHeights = new float[HoverPoints.Length];
	}

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("setHoverThrusterRange", "Set hover height", "Set hover height", delegate
		{
			Utils.OpenFloatInputDialog(BaseHoverHeight * (220f / 267f), this, delegate(HoverThrusterBehaviour t, float v)
			{
				t.BaseHoverHeight = Mathf.Clamp(v * 1.2136364f, 267f / 440f, 12136.364f);
			}, "Set hover height in meters", $"Target height in meters from {0.5f} m to {Mathf.RoundToInt(10f)} km");
		}));
		UpdateActivation();
	}

	public void Use(ActivationPropagation a)
	{
		if (base.enabled)
		{
			switch (a.Channel)
			{
			case 1:
				BaseHoverHeight += 22f / 267f;
				break;
			case 2:
				BaseHoverHeight -= 22f / 267f;
				break;
			default:
				Activated = !Activated;
				UpdateActivation();
				break;
			}
			BaseHoverHeight = Mathf.Clamp(BaseHoverHeight, 267f / 440f, 12136.364f);
		}
	}

	private void Update()
	{
		if (PhysicalBehaviour.IsBeingUsedContinuously(1))
		{
			BaseHoverHeight += 4.11985f * Time.deltaTime;
		}
		if (PhysicalBehaviour.IsBeingUsedContinuously(2))
		{
			BaseHoverHeight -= 4.11985f * Time.deltaTime;
		}
		BaseHoverHeight = Mathf.Clamp(BaseHoverHeight, 267f / 440f, 12136.364f);
	}

	private void FixedUpdate()
	{
		if (PhysicalBehaviour.rigidbody.bodyType == RigidbodyType2D.Dynamic && Activated)
		{
			Hover();
		}
	}

	private void UpdateActivation()
	{
		ParticleSystem.EmissionModule emission = Effects.emission;
		emission.enabled = Activated;
		Light.enabled = Activated;
		GlowSprite.enabled = Activated;
		if (Activated)
		{
			IdleLoop.Play();
		}
		else
		{
			IdleLoop.Stop();
		}
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivation();
	}

	private void Hover()
	{
		float num = Mathf.Pow(BaseHoverHeight, 1.07f) * 1.7f;
		for (int i = 0; i < HoverPoints.Length; i++)
		{
			calibratedHoverHeights[i] = num + HoverPoints[i].localPosition.y;
		}
		Vector3 up = base.transform.up;
		Vector3 vector = up * -1f;
		float num2 = ForceMultiplier + PhysicalBehaviour.Charge * 4f;
		for (int j = 0; j < HoverPoints.Length; j++)
		{
			Transform transform = HoverPoints[j];
			float num3 = calibratedHoverHeights[j];
			RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, vector, num3 * 2f, Layers);
			if (!raycastHit2D.transform)
			{
				continue;
			}
			float num4 = Mathf.Clamp01((num3 - raycastHit2D.distance) / num3) * Vector2.Dot(raycastHit2D.normal, up);
			if (!(num4 <= float.Epsilon))
			{
				num4 = Mathf.Pow(num4, FactorPower);
				Vector2 point = raycastHit2D.point;
				Debug.DrawRay(point, raycastHit2D.normal * num4, Color.cyan);
				Vector3 vector2 = num2 * num4 * vector;
				PhysicalBehaviour.rigidbody.AddForceAtPosition(vector2 * -1f, transform.position, ForceMode2D.Force);
				PhysicalBehaviour.rigidbody.velocity *= VelocityRetention;
				PhysicalBehaviour.rigidbody.angularVelocity *= VelocityRetention;
				if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
				{
					value.rigidbody.AddForceAtPosition(vector2 / Mathf.Max(1f, raycastHit2D.distance * raycastHit2D.distance), point);
				}
			}
		}
	}
}
