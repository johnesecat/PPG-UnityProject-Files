using UnityEngine;

public class DistantSoundBehaviour : MonoBehaviour
{
	public enum SoundType
	{
		None,
		SmallExplosion,
		LargeExplosion,
		LargerExplosion,
		SmallFirearm,
		MediumFirearm,
		LargeFirearm
	}

	public AudioClip[] SmallDistantExplosions;

	public AudioClip[] LargeDistantExplosions;

	public AudioClip[] LargerDistantExplosions;

	[Space]
	public AudioClip[] SmallDistantFirearms;

	public AudioClip[] MediumDistantFirearms;

	public AudioClip[] LargeDistantFirearms;

	[Space]
	public AudioSource AudioSource;

	public static DistantSoundBehaviour Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void Play(SoundType soundType, Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			switch (soundType)
			{
			case SoundType.SmallExplosion:
				PlaySmallExplosion(point, volume);
				break;
			case SoundType.LargeExplosion:
				PlayLargeExplosion(point, volume);
				break;
			case SoundType.LargerExplosion:
				PlayLargerExplosion(point, volume);
				break;
			case SoundType.SmallFirearm:
				PlaySmallFirearm(point, volume);
				break;
			case SoundType.MediumFirearm:
				PlayMediumFirearm(point, volume);
				break;
			case SoundType.LargeFirearm:
				PlayLargeFirearm(point, volume);
				break;
			}
		}
	}

	public void PlaySmallExplosion(Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			AudioSource.PlayOneShot(SmallDistantExplosions.PickRandom(), Mathf.Clamp01(volume));
		}
	}

	public void PlayLargeExplosion(Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			AudioSource.PlayOneShot(LargeDistantExplosions.PickRandom(), Mathf.Clamp01(volume));
		}
	}

	public void PlayLargerExplosion(Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			AudioSource.PlayOneShot(LargerDistantExplosions.PickRandom(), Mathf.Clamp01(volume));
		}
	}

	public void PlaySmallFirearm(Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			AudioSource.PlayOneShot(SmallDistantFirearms.PickRandom(), Mathf.Clamp01(volume));
		}
	}

	public void PlayMediumFirearm(Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			AudioSource.PlayOneShot(MediumDistantFirearms.PickRandom(), Mathf.Clamp01(volume));
		}
	}

	public void PlayLargeFirearm(Vector2 point, float volume = 1f)
	{
		if (UserPreferenceManager.Current.DistantSoundEffects)
		{
			AudioSource.PlayOneShot(LargeDistantFirearms.PickRandom(), Mathf.Clamp01(volume));
		}
	}
}
