using System;
using System.Text;
using UnityEngine;

public class FuseBoxBehaviour : MonoBehaviour
{
	private static string d = Encoding.ASCII.GetString(Utils.FBSF("MDAxMDExMDExMTEwMTAwMDExMDAxMTAxMDAwMDExMDExMDExMDAwMDEwMTEwMTEwMTEwMDAwMTAxMDAxMTAxMTExMDExMDEwMDExMTEwMDAwMTAwMDEwMTAxMDAwMTEwMDExMDAwMDEwMDAxMDEwMTEwMTAwMTExMTAxMTExMDAxMDExMDAxMTExMDEwMTEwMTEwMDAxMTAwMTAxMDAwMTEwMDExMDExMDExMTAwMTEwMDExMTAwMDEwMTAw"));

	private const int sampleRate = 44100;

	private AudioClip audioProvider;

	private long currentAudioPosition;

	public AudioSource Source;

	private System.Random rand = new System.Random();

	private void Awake()
	{
		audioProvider = AudioClip.Create("radioClip", 44100, 1, 44100, stream: true, ProvideAudioData);
		Source.clip = audioProvider;
	}

	private void ProvideAudioData(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			float num = (float)currentAudioPosition / 44100f;
			float num2 = num * 0.05f;
			char c = d[(int)(num2 % 1f * (float)d.Length)];
			float num3 = Mathf.Sin(num * 2f * (float)Math.PI * 300f) * (float)rand.NextDouble();
			float num4 = Mathf.Sin(num * 2f * (float)Math.PI * 300f);
			data[i] = ((!(num2 % 2f < 1f)) ? 0f : ((c == '0') ? num3 : num4));
			currentAudioPosition++;
			if (currentAudioPosition > 4410000)
			{
				currentAudioPosition = 0L;
			}
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(audioProvider);
	}
}
