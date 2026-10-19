using UnityEngine;

public class GlowtubeBehaviour : MonoBehaviour, Messages.IUse, Messages.IShot, Messages.ISlice, Messages.IOnFragmentHit, Messages.IBreak, Messages.IOnEMPHit
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer LightSprite;

	[SkipSerialisation]
	public SpriteRenderer ToToggle;

	[SkipSerialisation]
	public Color GasColour;

	[SkipSerialisation]
	public GameObject Gas;

	[SkipSerialisation]
	public float PowerUpperbound;

	public Gradient PowerGradient;

	private Color initialColour;

	[HideInInspector]
	public bool Broken;

	[HideInInspector]
	public bool Activated;

	public float GasContents = 1f;

	private Collider2D[] buffer = new Collider2D[64];

	[SkipSerialisation]
	public LayerMask LayerMask;

	[SkipSerialisation]
	public float TargetTemperature = 35f;

	[SkipSerialisation]
	public float GasEffectRadius = 3f;

	private Color Colour => initialColour * PowerGradient.Evaluate(PhysicalBehaviour.Charge / PowerUpperbound * Mathf.Clamp01(GasContents)) * Mathf.Clamp01(GasContents);

	private void Start()
	{
		initialColour = ToToggle.color;
		UpdateActivation();
	}

	private void Update()
	{
		if (Activated)
		{
			ToToggle.color = Colour;
			LightSprite.color = Colour;
		}
		if (PhysicalBehaviour.Charge > 800f && !Broken)
		{
			GasContents += Time.deltaTime;
			ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(base.transform.position), new ExplosionCreator.ExplosionParameters(16u, base.transform.position, 4f, 5f, createFx: true));
			Break(default(Vector2));
		}
		if (Broken)
		{
			GasContents -= Time.deltaTime * 0.25f;
		}
	}

	private void FixedUpdate()
	{
		if (Activated && !Broken && PhysicalBehaviour.SimulateTemperature)
		{
			PhysicalBehaviour.Temperature = Mathf.Lerp(PhysicalBehaviour.Temperature, TargetTemperature, 0.01f);
		}
	}

	public void Use(ActivationPropagation _)
	{
		Activated = !Activated;
		UpdateActivation();
	}

	public void Shot(Shot shot)
	{
		if (shot.damage > 50f)
		{
			Break(default(Vector2));
		}
	}

	public void Slice()
	{
		Break(default(Vector2));
	}

	public void OnEMPHit()
	{
		Break(default(Vector2));
	}

	public void OnFragmentHit(float force)
	{
		Break(default(Vector2));
	}

	public void Break(Vector2 velocity)
	{
		if (!Broken)
		{
			Broken = true;
			ParticleSystem component = Object.Instantiate(Gas, base.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = component.main;
			main.startColor = GasColour;
			component.Play(withChildren: true);
		}
	}

	private void UpdateActivation()
	{
		ToToggle.color = Colour;
		ToToggle.enabled = Activated;
		LightSprite.color = Colour;
		LightSprite.enabled = Activated;
	}
}
