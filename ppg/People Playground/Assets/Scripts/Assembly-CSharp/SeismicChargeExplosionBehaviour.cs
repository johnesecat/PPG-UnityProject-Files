using UnityEngine;

public class SeismicChargeExplosionBehaviour : MonoBehaviour
{
	public AnimationCurve XScaleCurve;

	public AnimationCurve YScaleCurve;

	public float SizeMultiplier = 1f;

	public float Duration = 1f;

	public AudioClip SeismicChargeSound;

	public AudioSource ExplosionAudioSource;

	public PointEffector2D Magnet;

	public float Strength = 55f;

	public GameObject bolt;

	public uint BoltCount = 32u;

	public float MuteStartSecond;

	public float MuteEndSecond;

	private float time;

	private bool muted;

	private GameObject[] bolts;

	private void Awake()
	{
		bolts = new GameObject[BoltCount];
		for (int i = 0; i < BoltCount; i++)
		{
			GameObject gameObject = Object.Instantiate(bolt, base.transform.position, Quaternion.identity, base.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			bolts[i] = gameObject;
		}
	}

	private void Start()
	{
		base.transform.localScale = new Vector3(XScaleCurve.Evaluate(time), YScaleCurve.Evaluate(time), 1f) * SizeMultiplier;
		ExplosionAudioSource.clip = SeismicChargeSound;
		ExplosionAudioSource.Play();
	}

	private void Update()
	{
		time += Time.deltaTime;
		base.transform.localScale = new Vector3(XScaleCurve.Evaluate(time / Duration), YScaleCurve.Evaluate(time / Duration), 1f) * SizeMultiplier;
		if (time > MuteStartSecond && time <= MuteEndSecond && !muted)
		{
			muted = true;
			CameraListenerBehaviour.main.Mute();
		}
		if (time > MuteEndSecond && muted)
		{
			muted = false;
			CameraListenerBehaviour.main.Unmute();
		}
		if (time < MuteStartSecond)
		{
			CameraShakeBehaviour.main.Shake(20f, base.transform.position, 0.5f);
		}
	}

	private void FixedUpdate()
	{
		if (!(time > MuteEndSecond) || !Magnet.enabled)
		{
			return;
		}
		GameObject[] array = bolts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		CameraShakeBehaviour.main.Shake(25f, base.transform.position);
		Magnet.enabled = false;
		Collider2D[] array2 = Physics2D.OverlapCircleAll(base.transform.position, 350f, Magnet.colliderMask);
		foreach (Collider2D collider2D in array2)
		{
			collider2D.SendMessage("OnFragmentHit", 25, SendMessageOptions.DontRequireReceiver);
			if ((bool)collider2D.attachedRigidbody)
			{
				collider2D.attachedRigidbody.AddForce(collider2D.attachedRigidbody.mass * (0f - Strength) * (base.transform.position - collider2D.transform.position).normalized, ForceMode2D.Impulse);
			}
			PhysicalBehaviour component = collider2D.GetComponent<PhysicalBehaviour>();
			if ((bool)component)
			{
				component.Charge = 150f * Random.value;
			}
		}
	}
}
