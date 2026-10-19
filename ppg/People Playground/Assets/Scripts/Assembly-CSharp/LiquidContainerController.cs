using UnityEngine;

[ExecuteAlways]
public class LiquidContainerController : MonoBehaviour
{
	private class Particle
	{
		public Vector2 Position;

		public Vector2 Velocity;
	}

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public AudioSource SloshingSource;

	public float SloshingIntensity;

	private MaterialPropertyBlock properties;

	private Renderer renderComponent;

	[SkipSerialisation]
	public Material LowFidelityLiquid;

	private bool isFancy;

	public Bounds Container;

	public float FillPercentage;

	public float ParticleRadius = 1f;

	private Particle[] particles;

	private Vector4[] particlePositions;

	private const int particleCount = 16;

	private const float repulsionDistance = 0.01f;

	private const float repulsionForce = 0.07f;

	private Vector2 acceleration;

	private Vector2 preVelocity;

	private Color color;

	private float OffsetRepulsionDistance => 0.01f + FillPercentage * 0.2f;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			if (properties != null)
			{
				properties.SetColor(ShaderProperties.Get("_LiquidColor"), value);
			}
		}
	}

	private bool NoNeedToSimulate
	{
		get
		{
			if (isFancy && !Mathf.Approximately(FillPercentage, 0f))
			{
				return Mathf.Approximately(FillPercentage, 1f);
			}
			return true;
		}
	}

	private void Awake()
	{
		isFancy = UserPreferenceManager.Current.FancyEffects;
		if (!isFancy)
		{
			GetComponent<Renderer>().sharedMaterial = LowFidelityLiquid;
			return;
		}
		particles = new Particle[16];
		particlePositions = new Vector4[16];
		for (int i = 0; i < 16; i++)
		{
			particles[i] = new Particle
			{
				Position = new Vector2(Random.Range(Container.min.x, Container.max.x), Random.Range(Container.min.y, Container.max.y))
			};
			particlePositions[i] = particles[i].Position;
		}
	}

	private void Start()
	{
		SloshingSource.enabled = isFancy;
		if (isFancy)
		{
			SimulateLiquid();
			for (int i = 0; i < 16; i++)
			{
				particlePositions[i] = particles[i].Position;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!NoNeedToSimulate && (!renderComponent || renderComponent.isVisible))
		{
			SimulateLiquid();
		}
	}

	private void SimulateLiquid()
	{
		Vector2 vector = base.transform.InverseTransformVector(Physics2D.gravity);
		Vector2 velocity = Rigidbody.velocity;
		acceleration = preVelocity - velocity;
		preVelocity = velocity;
		Vector2 vector2 = base.transform.InverseTransformVector(acceleration);
		for (int i = 0; i < 16; i++)
		{
			Particle particle = particles[i];
			for (int j = 0; j < 16; j++)
			{
				Particle particle2 = particles[j];
				if (particle2 == particle)
				{
					continue;
				}
				Vector2 vector3 = particle2.Position - particle.Position;
				float sqrMagnitude = vector3.sqrMagnitude;
				if (sqrMagnitude == 0f)
				{
					particle.Position += Random.insideUnitCircle * 0.01f;
					continue;
				}
				float num = Mathf.Sqrt(sqrMagnitude);
				if ((double)num < (double)OffsetRepulsionDistance * 1.5)
				{
					particle.Velocity += -(vector3 / num / (sqrMagnitude + 1f)) * 0.07f;
				}
			}
		}
		float num2 = 0f;
		for (int k = 0; k < particles.Length; k++)
		{
			Particle particle3 = particles[k];
			if (!Container.Contains(particle3.Position))
			{
				if (particle3.Position.x < Container.min.x || particle3.Position.x > Container.max.x)
				{
					particle3.Velocity.x = 0f;
				}
				if (particle3.Position.y < Container.min.y || particle3.Position.y > Container.max.y)
				{
					particle3.Velocity.y = 0f;
				}
				particle3.Position = Container.ClosestPoint(particle3.Position);
			}
			particle3.Position += particle3.Velocity * 0.1f;
			particle3.Velocity *= 0.95f;
			particle3.Velocity += vector2 * 0.25f;
			particle3.Velocity += 0.002f * Rigidbody.gravityScale * vector;
			num2 += particle3.Velocity.sqrMagnitude;
			Vector2 vector4 = particle3.Position - (Vector2)Container.min;
			Vector2 vector5 = (Vector2)Container.max - particle3.Position;
			if (vector4.x < OffsetRepulsionDistance)
			{
				particle3.Velocity.x += edgeRepulse(vector4.x);
			}
			if (vector4.y < OffsetRepulsionDistance)
			{
				particle3.Velocity.y += edgeRepulse(vector4.y);
			}
			if (vector5.x < OffsetRepulsionDistance)
			{
				particle3.Velocity.x -= edgeRepulse(vector5.x);
			}
			if (vector5.y < OffsetRepulsionDistance)
			{
				particle3.Velocity.y -= edgeRepulse(vector5.y);
			}
		}
		SloshingSource.volume = Mathf.Lerp(SloshingSource.volume, TransformSoundVolume(SloshingIntensity * (num2 / 16f)), 0.6f);
		float edgeRepulse(float edgeDelta)
		{
			float num3 = 1f + Mathf.Abs(edgeDelta);
			return 0.5f / (num3 * num3) * 0.07f;
		}
	}

	private float TransformSoundVolume(float input)
	{
		float fillPercentage = FillPercentage;
		float num = -4f * fillPercentage * fillPercentage + 4f * fillPercentage;
		return num * num * num * input;
	}

	public void AddForce(Vector2 globalForce, float randomisation = 0f)
	{
		if (particles == null || !isFancy)
		{
			return;
		}
		Vector2 vector = base.transform.InverseTransformVector(globalForce);
		for (int i = 0; i < 16; i++)
		{
			if (particles[i] != null)
			{
				particles[i].Velocity += vector * (1f - Random.value * randomisation);
			}
		}
	}

	private void Update()
	{
		if (NoNeedToSimulate)
		{
			SloshingSource.volume = 0f;
			return;
		}
		float t = 1f - Mathf.Pow(0.00025f, Time.deltaTime);
		for (int i = 0; i < 16; i++)
		{
			particlePositions[i] = Vector2.Lerp(particlePositions[i], particles[i].Position, t);
		}
	}

	private void OnWillRenderObject()
	{
		if (!renderComponent || properties == null)
		{
			properties = new MaterialPropertyBlock();
			renderComponent = GetComponent<Renderer>();
			properties.SetColor(ShaderProperties.Get("_LiquidColor"), Color);
			if (isFancy)
			{
				properties.SetVectorArray(ShaderProperties.Get("_Particles"), particlePositions);
				properties.SetInt(ShaderProperties.Get("_ParticleCount"), 16);
				properties.SetFloat(ShaderProperties.Get("_ClipThreshold"), GetClipThreshold());
				renderComponent.SetPropertyBlock(properties);
			}
		}
		if (!isFancy)
		{
			properties.SetColor(ShaderProperties.Get("_LiquidColor"), Color);
			properties.SetFloat(ShaderProperties.Get("_Fill"), FillPercentage);
			properties.SetVector(ShaderProperties.Get("_UvBounds"), new Vector4(Container.min.x + 0.5f, Container.min.y + 0.5f, Container.max.x + 0.5f, Container.max.y + 0.5f));
			renderComponent.SetPropertyBlock(properties);
		}
		else
		{
			SetShaderProperties();
		}
	}

	private void SetShaderProperties()
	{
		if (properties != null)
		{
			if (particlePositions != null)
			{
				properties.SetVectorArray(ShaderProperties.Get("_Particles"), particlePositions);
			}
			properties.SetInt(ShaderProperties.Get("_ParticleCount"), 16);
			properties.SetFloat(ShaderProperties.Get("_ClipThreshold"), GetClipThreshold());
			properties.SetVector(ShaderProperties.Get("_UvBounds"), new Vector4(Container.min.x + 0.5f, Container.min.y + 0.5f, Container.max.x + 0.5f, Container.max.y + 0.5f));
			renderComponent.SetPropertyBlock(properties);
		}
	}

	private float GetClipThreshold()
	{
		return (1f - FillPercentage) / Mathf.Max(0.01f, ParticleRadius);
	}

	private void OnBecameVisible()
	{
		SetShaderProperties();
	}
}
