using UnityEngine;

public class TemperatureRayGunBehaviour : MonoBehaviour, Messages.IUseContinuous
{
	public float TemperatureDelta = 5f;

	public LayerMask Layers;

	private float useHeat;

	[SkipSerialisation]
	public AudioClip InitialiseClip;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public ParticleSystem Muzzle;

	[SkipSerialisation]
	public ParticleSystem Impact;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	public void UseContinuous(ActivationPropagation activation)
	{
		ContinuousActivationBehaviour.AssertState();
		if (base.enabled)
		{
			if (useHeat < 0f || !AudioSource.isPlaying)
			{
				PhysicalBehaviour.PlayClipOnce(InitialiseClip, 1.5f);
				AudioSource.Play();
			}
			useHeat = 0.1f;
		}
	}

	private void Update()
	{
		if (LineRenderer.enabled)
		{
			LineRenderer.SetPosition(0, LineRenderer.transform.position);
		}
	}

	private void OnDisable()
	{
		AudioSource.Stop();
		LineRenderer.enabled = false;
		if ((bool)Impact)
		{
			Impact.Stop();
		}
		Muzzle.Stop();
		useHeat = 0f;
	}

	private void FixedUpdate()
	{
		if (useHeat > 0f)
		{
			CameraShakeBehaviour.main.Shake(0.05f, base.transform.position);
			Vector3 position = LineRenderer.transform.position;
			Vector3 normalized = (LineRenderer.transform.right * base.transform.localScale.x).normalized;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(position, normalized, float.MaxValue, Layers);
			Vector3 vector;
			if ((bool)raycastHit2D.transform)
			{
				vector = raycastHit2D.point;
				CameraShakeBehaviour.main.Shake(0.05f, vector);
			}
			else
			{
				vector = position + normalized * 100000f;
			}
			LineRenderer.SetPosition(1, vector);
			Impact.transform.position = vector;
			if ((bool)raycastHit2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value) && value.SimulateTemperature)
			{
				value.Temperature += TemperatureDelta * Mathf.Max(1f, PhysicalBehaviour.Charge);
			}
			if (!Impact.isEmitting)
			{
				Impact.Play();
			}
			if (!Muzzle.isEmitting)
			{
				Muzzle.Play();
			}
		}
		else
		{
			if (AudioSource.isPlaying)
			{
				AudioSource.Stop();
			}
			if (Impact.isEmitting)
			{
				Impact.Stop();
			}
			if (Muzzle.isEmitting)
			{
				Muzzle.Stop();
			}
		}
		LineRenderer.enabled = useHeat > 0f;
		useHeat -= Time.deltaTime;
	}
}
