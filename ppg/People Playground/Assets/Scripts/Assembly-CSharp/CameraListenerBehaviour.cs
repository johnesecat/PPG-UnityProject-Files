using UnityEngine;

public class CameraListenerBehaviour : MonoBehaviour
{
	private AudioReverbFilter reverbFilter;

	public float orthosizeMin;

	public float orthosizeMax;

	public AnimationCurve masterVolumeCurve;

	public AnimationCurve dryVolumeCurve;

	private bool muted;

	public static CameraListenerBehaviour main;

	public static float NormalisedDistance { get; private set; }

	private void Awake()
	{
		reverbFilter = GetComponent<AudioReverbFilter>();
		main = this;
	}

	private void Update()
	{
		NormalisedDistance = (Mathf.Clamp(Global.main.camera.orthographicSize, orthosizeMin, orthosizeMax) - orthosizeMin) / (orthosizeMax - orthosizeMin);
		AudioListener.volume = (muted ? 0f : masterVolumeCurve.Evaluate(NormalisedDistance));
		reverbFilter.dryLevel = (1f - dryVolumeCurve.Evaluate(NormalisedDistance)) * -5000f;
		reverbFilter.room = dryVolumeCurve.Evaluate(NormalisedDistance) * -1000f - 1000f;
	}

	public void Mute()
	{
		muted = true;
	}

	public void Unmute()
	{
		muted = false;
	}
}
