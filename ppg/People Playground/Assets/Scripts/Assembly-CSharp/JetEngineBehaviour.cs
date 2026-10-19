using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEngineBehaviour : MonoBehaviour, Messages.IUse, Messages.IOnAfterDeserialise
{
	public enum State
	{
		Idle,
		Starting,
		Active,
		Shutdown
	}

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioSource Source;

	[SkipSerialisation]
	public AudioClip StartupSound;

	[SkipSerialisation]
	public AudioClip ShutoffSound;

	[SkipSerialisation]
	public PointEffector2D IntakeEffector;

	[SkipSerialisation]
	public float IntakeForceMultiplier = 1f;

	[Space]
	[SkipSerialisation]
	public PointEffector2D ExhaustEffector;

	[SkipSerialisation]
	public float ExhaustForceMultiplier = 1f;

	[SkipSerialisation]
	public float ExhaustTemperature = 800f;

	[SkipSerialisation]
	public float StartupDurationSeconds = 11f;

	[SkipSerialisation]
	public float ShutoffDurationSeconds = 1f;

	[SkipSerialisation]
	public SpriteRenderer PlasmaJet;

	[SkipSerialisation]
	public AnimationCurve StartupIntensityCurve;

	[SkipSerialisation]
	public float ScreenShakeIntensity = 0.25f;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachinery;

	[SkipSerialisation]
	public bool WillDrown = true;

	[SkipSerialisation]
	public SpriteRenderer Lighting;

	public bool Activated;

	public State CurrentState;

	[SkipSerialisation]
	public float Force;

	private static readonly Collider2D[] buffer = new Collider2D[32];

	private GuideTemperatureBehaviour guideTemperatureBehaviour;

	private Collider2D exhaustEffectorCollider;

	private Collider2D intakeEffectorCollider;

	private void Awake()
	{
		guideTemperatureBehaviour = GetComponent<GuideTemperatureBehaviour>();
		PlasmaJet.color = Color.clear;
		exhaustEffectorCollider = ExhaustEffector.GetComponent<Collider2D>();
		intakeEffectorCollider = IntakeEffector.GetComponent<Collider2D>();
	}

	private void FixedUpdate()
	{
		switch (CurrentState)
		{
		case State.Idle:
			ExhaustEffector.forceMagnitude = 0f;
			IntakeEffector.forceMagnitude = 0f;
			break;
		case State.Starting:
			if (WillDrown && PhysicalBehaviour.IsUnderWater)
			{
				DamagableMachinery.ForceBreak();
			}
			break;
		case State.Active:
		{
			float num = Mathf.Abs(base.transform.localScale.x);
			ExhaustEffector.forceMagnitude = ExhaustForceMultiplier * num;
			IntakeEffector.forceMagnitude = IntakeForceMultiplier * num;
			ExhaustAffectObjects(1f);
			PhysicalBehaviour.rigidbody.AddRelativeForce(new Vector2(Force * base.transform.localScale.x, 0f), ForceMode2D.Force);
			CameraShakeBehaviour.main.Shake(ScreenShakeIntensity, base.transform.position);
			if (WillDrown && PhysicalBehaviour.IsUnderWater)
			{
				DamagableMachinery.ForceBreak();
			}
			break;
		}
		case State.Shutdown:
			ExhaustEffector.forceMagnitude = 0f;
			IntakeEffector.forceMagnitude = 0f;
			break;
		}
	}

	private void ExhaustAffectObjects(float intensity)
	{
		float t = 0.05f * intensity;
		int num = Physics2D.OverlapCollider(exhaustEffectorCollider, new ContactFilter2D
		{
			layerMask = ExhaustEffector.colliderMask
		}, buffer);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (collider2D.gameObject != base.gameObject && (bool)collider2D && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && value.SimulateTemperature && value.Temperature < ExhaustTemperature)
			{
				value.Temperature = Mathf.Lerp(value.Temperature, ExhaustTemperature, t);
			}
		}
	}

	public void OnAfterDeserialise(List<GameObject> gameObjects)
	{
		switch (CurrentState)
		{
		case State.Starting:
			CurrentState = State.Idle;
			Source.Stop();
			SetVisualJetIntensity(0f);
			StartCoroutine(StartupRoutine());
			break;
		case State.Active:
			guideTemperatureBehaviour.enabled = true;
			SetVisualJetIntensity(1f);
			Source.Play();
			break;
		case State.Shutdown:
			CurrentState = State.Active;
			SetVisualJetIntensity(1f);
			StartCoroutine(ShutoffRoutine());
			break;
		case State.Idle:
			break;
		}
	}

	private IEnumerator StartupRoutine()
	{
		if (CurrentState == State.Idle)
		{
			CurrentState = State.Starting;
			float t = 0f;
			Source.PlayOneShot(StartupSound);
			SetVisualJetIntensity(0f);
			guideTemperatureBehaviour.enabled = true;
			while (t < StartupDurationSeconds)
			{
				float num = t / StartupDurationSeconds;
				float num2 = num * num * 0.25f;
				float num3 = num2 * Mathf.Abs(base.transform.localScale.x);
				ExhaustEffector.forceMagnitude = ExhaustForceMultiplier * num3;
				IntakeEffector.forceMagnitude = IntakeForceMultiplier * num3;
				ExhaustAffectObjects(num2);
				SetVisualJetIntensity(StartupIntensityCurve.Evaluate(num));
				CameraShakeBehaviour.main.Shake(num * ScreenShakeIntensity * 0.5f, base.transform.position);
				PhysicalBehaviour.rigidbody.AddRelativeForce(new Vector2(Force * base.transform.localScale.x * num * 0.4f, 0f), ForceMode2D.Force);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			SetVisualJetIntensity(1f);
			CameraShakeBehaviour.main.Shake(40f * ScreenShakeIntensity, base.transform.position);
			Source.Play();
			CurrentState = State.Active;
		}
	}

	private IEnumerator ShutoffRoutine()
	{
		if (CurrentState == State.Active)
		{
			CurrentState = State.Shutdown;
			float t = 0f;
			Source.Stop();
			Source.PlayOneShot(ShutoffSound);
			guideTemperatureBehaviour.enabled = false;
			SetVisualJetIntensity(1f);
			while (t < ShutoffDurationSeconds)
			{
				SetVisualJetIntensity(1f - t / ShutoffDurationSeconds);
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			SetVisualJetIntensity(0f);
			ExhaustEffector.forceMagnitude = 0f;
			IntakeEffector.forceMagnitude = 0f;
			yield return new WaitForSeconds(4f);
			CurrentState = State.Idle;
		}
	}

	public void SetVisualJetIntensity(float i)
	{
		PlasmaJet.color = new Color(0f, 0f, 0f, i);
		Color color = Lighting.color;
		color.a = i * i;
		Lighting.color = color;
	}

	public void Use(ActivationPropagation activation)
	{
		if (!DamagableMachinery.Destroyed)
		{
			Activated = !Activated;
			if (Activated)
			{
				TryStartup();
			}
			else
			{
				TryShutoff();
			}
		}
	}

	public void TryStartup()
	{
		if (!DamagableMachinery.Destroyed)
		{
			StartCoroutine(StartupRoutine());
		}
	}

	public void TryShutoff()
	{
		StartCoroutine(ShutoffRoutine());
	}

	public void ForceShutoff()
	{
		StopAllCoroutines();
		Activated = false;
		ExhaustEffector.forceMagnitude = 0f;
		ExhaustEffector.forceMagnitude = 0f;
		IntakeEffector.forceMagnitude = 0f;
		CurrentState = State.Idle;
		Source.Stop();
		guideTemperatureBehaviour.enabled = false;
		SetVisualJetIntensity(0f);
	}
}
