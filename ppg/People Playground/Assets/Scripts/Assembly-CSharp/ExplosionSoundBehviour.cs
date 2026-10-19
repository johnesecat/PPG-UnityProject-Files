using UnityEngine;

public class ExplosionSoundBehviour : MonoBehaviour, Messages.IOnPoolableReinitialised
{
	public AudioClip[] Near;

	public AudioSource audioSource;

	public bool StartWhenAwake = true;

	public float Delay;

	public DistantSoundBehaviour.SoundType DistantSound;

	private void Awake()
	{
		if (!audioSource)
		{
			audioSource = GetComponent<AudioSource>();
		}
		Global.main.AddAudioSource(audioSource);
	}

	private void Start()
	{
		if (StartWhenAwake)
		{
			Play(Delay);
		}
	}

	public void Play(float delay = 0f)
	{
		if (!base.enabled)
		{
			return;
		}
		audioSource.clip = Near.PickRandom();
		audioSource.PlayDelayed(delay);
		if (UserPreferenceManager.Current.DistantSoundEffects && DistantSound != 0)
		{
			StartCoroutine(Utils.DelayCoroutine(delay, delegate
			{
				DistantSoundBehaviour.Instance.Play(DistantSound, base.transform.position, 0.5f);
			}));
		}
	}

	private void OnDestroy()
	{
		Global.main.RemoveAudioSource(audioSource);
	}

	public void OnPoolableReinitialised(ObjectPoolBehaviour pool)
	{
		Start();
	}
}
