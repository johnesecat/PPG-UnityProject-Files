using System;
using UnityEngine;

[NotDocumented]
public class AntennaReceiverBehaviour : MonoBehaviour
{
	private struct AudioInformation
	{
		public float OldFactor;

		public float CurrentFactor;
	}

	[NonSerialized]
	public int ConnectedAntennae;

	[NonSerialized]
	public float SignalStrength;

	private GameObject componentChild;

	private AudioSource audioSource;

	private AudioClip audioProvider;

	private const int sampleRate = 44100;

	private int currentAudioPosition;

	private static int globalStartAudioPos;

	private readonly System.Random rand = new System.Random();

	[SkipSerialisation]
	public Vector2 TargetPosition;

	[SkipSerialisation]
	public Map TargetMap;

	private AudioInformation audioInformation;

	private float timeSpentNearTarget;

	private bool correctMap;

	private void Awake()
	{
		correctMap = MapLoaderBehaviour.CurrentMap == TargetMap;
		componentChild = new GameObject("antenna");
		componentChild.transform.SetParent(base.transform);
		componentChild.transform.localPosition = default(Vector3);
		componentChild.AddComponent<Optout>();
		audioSource = componentChild.AddComponent<AudioSource>();
		audioSource.spatialBlend = 1f;
		audioSource.playOnAwake = false;
		audioSource.loop = true;
		audioSource.minDistance = 4f;
		audioSource.maxDistance = 50f;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		Global.main.AddAudioSource(audioSource);
		audioProvider = AudioClip.Create("radioClip", 44100, 1, 44100, stream: true, ProvideAudioData);
		audioSource.clip = audioProvider;
	}

	private void ProvideAudioData(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = GetNextFloat(i, data.Length);
			currentAudioPosition++;
			globalStartAudioPos = currentAudioPosition;
		}
	}

	private float GetNextFloat(int index, int count)
	{
		float t = (float)index / ((float)count - 1f);
		if (rand.NextDouble() > (double)SignalStrength)
		{
			return 0f;
		}
		float a = (float)rand.NextDouble();
		float num = (float)currentAudioPosition / 44100f;
		float num2 = Mathf.Lerp(100f, 3000f, Mathf.PerlinNoise(num % 60f * 0.754578f, 3498.346f));
		float b = Mathf.Sin(num * 2f * (float)Math.PI * num2);
		float num3 = Mathf.Lerp(audioInformation.OldFactor, audioInformation.CurrentFactor, t);
		return Mathf.Lerp(a, b, num3 * num3);
	}

	private void Update()
	{
		if (ConnectedAntennae != 0)
		{
			audioInformation.OldFactor = audioInformation.CurrentFactor;
			audioInformation.CurrentFactor = (correctMap ? Mathf.Pow(Mathf.Clamp01(10f / Mathf.Max(0.1f, Vector2.SqrMagnitude((Vector2)base.transform.position - TargetPosition))), 2f) : 0f);
			bool flag = audioInformation.CurrentFactor > 0.9f;
			if (flag)
			{
				timeSpentNearTarget += Time.deltaTime;
			}
			if (timeSpentNearTarget > 3f && flag && SteamworksInitialiser.IsInitialised)
			{
				StatManager.IncrementInteger(StatManager.Stat.SGNR);
			}
		}
	}

	private void FixedUpdate()
	{
		if (ConnectedAntennae > 0)
		{
			SignalStrength = Mathf.Lerp(SignalStrength, 0f, Time.fixedDeltaTime * 5f);
		}
		else if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
	}

	private void OnDisable()
	{
		if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
	}

	private void OnEnable()
	{
		if (ConnectedAntennae > 0)
		{
			audioSource.Play();
		}
	}

	public void Connect()
	{
		if (base.enabled)
		{
			if (TryGetComponent<JukeboxBehaviour>(out var component))
			{
				component.StopMusic();
			}
			if (!audioSource.isPlaying)
			{
				currentAudioPosition = globalStartAudioPos;
				audioSource.Play();
			}
		}
	}

	private void OnDestroy()
	{
		Global.main.RemoveAudioSource(audioSource);
		UnityEngine.Object.Destroy(componentChild);
		UnityEngine.Object.Destroy(audioProvider);
	}
}
