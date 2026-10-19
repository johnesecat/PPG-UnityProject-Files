using UnityEngine;

[NotDocumented]
public class WhOpening : MonoBehaviour
{
	public AnimationCurve GrowingCurve;

	public GameObject Wh;

	public AudioSource AudioSource;

	private float duration;

	private float t;

	private void Awake()
	{
		duration = AudioSource.clip.length;
	}

	private void Start()
	{
	}

	private void Update()
	{
		t += Time.deltaTime / duration;
		base.transform.localScale = Vector3.one * GrowingCurve.Evaluate(t);
		if (t >= 1f)
		{
			End();
		}
	}

	private void FixedUpdate()
	{
		CameraShakeBehaviour.main.Shake(t * 0.5f, base.transform.position, 0.7f);
	}

	private void End()
	{
		ExplosionCreator.CreatePulseExplosion(base.transform.position, 650f, 35f, soundAndEffects: false);
		ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(base.transform.position), new ExplosionCreator.ExplosionParameters(32u, base.transform.position, 35f, 25f, createFx: true, big: true));
		base.gameObject.SetActive(value: false);
		Object.Instantiate(Wh, base.transform.position, Quaternion.identity);
	}
}
