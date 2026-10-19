using UnityEngine;
using UnityEngine.Audio;

public class MixerControllerBehaviour : MonoBehaviour
{
	public AudioMixer Mixer;

	public string MasterKey;

	[Space]
	public string SoundEffectsKey;

	[Space]
	public string UserInterfaceKey;

	[Space]
	public string AmbienceKey;

	[Space]
	public string AmbienceCutoffKey;

	private float ScaleVolume(float p)
	{
		return Mathf.Lerp(-80f, 0f, Mathf.Pow(p, 0.3f));
	}

	public void Sync()
	{
		Mixer.SetFloat(MasterKey, ScaleVolume(UserPreferenceManager.Current.MasterVolume));
		Mixer.SetFloat(SoundEffectsKey, ScaleVolume(UserPreferenceManager.Current.SfxVolume));
		Mixer.SetFloat(UserInterfaceKey, ScaleVolume(UserPreferenceManager.Current.UserInterfaceVolume));
		Mixer.SetFloat(AmbienceKey, ScaleVolume(UserPreferenceManager.Current.AmbienceVolume));
		Mixer.SetFloat(AmbienceCutoffKey, UserPreferenceManager.Current.AmbienceHighpassCutoff);
	}

	public void Start()
	{
		Sync();
	}
}
