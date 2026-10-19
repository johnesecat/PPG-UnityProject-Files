using System;
using UnityEngine;

public class NukeShockwaveBehaviour : MonoBehaviour
{
	public float Duration = 0.02f;

	public float SizeMultiplier = 5f;

	public AnimationCurve GrowingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public SpriteRenderer Renderer;

	public ParticleSystem ParticleSystem;

	public int RayCount = 256;

	public LayerMask ShockwaveEffectMask;

	public float PushAwayDistance;

	public float RangeFragment = 10f;

	public bool DoShockwaveSmoke;

	private float startSize;

	private Color startColor;

	private float currentRange;

	private float currentProgress;

	private float t;

	private void Start()
	{
		startSize = base.transform.localScale.x;
		startColor = Renderer.color;
		if (DoShockwaveSmoke)
		{
			int num = 8;
			for (int i = 0; i < num; i++)
			{
				float f = 360f * ((float)i / (float)num) * ((float)Math.PI / 180f);
				PushAway(new Vector2(Mathf.Cos(f), Mathf.Sin(f)));
			}
		}
	}

	private void PushAway(Vector2 dir)
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, dir, PushAwayDistance, ShockwaveEffectMask);
		if ((bool)raycastHit2D.transform)
		{
			base.transform.position -= (Vector3)dir * (PushAwayDistance - raycastHit2D.distance);
		}
	}

	private void Update()
	{
		t += Time.deltaTime;
		float time = t / Duration;
		float num = GrowingCurve.Evaluate(time) * SizeMultiplier + startSize;
		base.transform.localScale = new Vector3(num, num, 0f);
		Renderer.color = Color.Lerp(startColor, Color.black, time);
		currentRange = num;
		currentProgress = time;
		if (t > Duration)
		{
			base.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if (DoShockwaveSmoke)
		{
			SpawnParticles(Renderer.bounds.extents.x);
		}
	}

	private void SpawnParticles(float range)
	{
		ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
		int num = Mathf.Max(1, Mathf.CeilToInt((float)RayCount * currentProgress));
		for (int i = 0; i < num; i++)
		{
			float f = 360f * ((float)i / (float)num) * ((float)Math.PI / 180f);
			Vector3 vector = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position + vector * (range - RangeFragment), vector, RangeFragment, ShockwaveEffectMask);
			if ((bool)raycastHit2D.transform)
			{
				emitParams.position = raycastHit2D.point;
				emitParams.ResetStartSize();
				ParticleSystem.Emit(emitParams, 1);
			}
		}
	}
}
