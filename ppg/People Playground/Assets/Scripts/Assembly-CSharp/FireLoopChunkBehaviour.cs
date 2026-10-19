using UnityEngine;

public class FireLoopChunkBehaviour : MonoBehaviour
{
	private AudioSource AudioSource;

	public float Range;

	public float QuietDownSpeedMultiplier = 1f;

	private float RandomStart;

	public bool Dirty { get; private set; }

	private void Awake()
	{
		RandomStart = Random.value;
	}

	public void FuzzySetVolume(float volume, float weight = 0.5f)
	{
		if (!Dirty)
		{
			if (!AudioSource)
			{
				AudioSource = base.gameObject.AddComponent<AudioSource>();
				AudioSource.clip = Resources.Load<AudioClip>("Audio/fire_loop");
				AudioSource.minDistance = 1f;
				AudioSource.maxDistance = Range;
				AudioSource.playOnAwake = false;
				AudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
				AudioSource.loop = true;
				AudioSource.spatialBlend = 1f;
				AudioSource.dopplerLevel = 0f;
				base.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
			}
			Dirty = true;
			base.gameObject.SetActive(value: true);
		}
		if (AudioSource.volume <= volume)
		{
			AudioSource.volume = Mathf.Lerp(AudioSource.volume, volume, weight);
		}
		if (!AudioSource.isPlaying)
		{
			AudioSource.time = RandomStart * AudioSource.clip.length;
			AudioSource.Play();
		}
	}

	private void Update()
	{
		if (Dirty)
		{
			if (AudioSource.volume > 0.05f)
			{
				AudioSource.volume -= Time.smoothDeltaTime * QuietDownSpeedMultiplier;
				return;
			}
			AudioSource.Stop();
			Dirty = false;
			base.gameObject.SetActive(value: false);
		}
	}
}
