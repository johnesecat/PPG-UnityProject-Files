using System;
using UnityEngine;
using UnityEngine.Video;

public class TelevisionBehaviour : MonoBehaviour, Messages.IUse, Messages.IBreak, Messages.IOnEMPHit, Messages.IShot
{
	[SkipSerialisation]
	public SpriteRenderer MyRenderer;

	[SkipSerialisation]
	public SpriteRenderer ScreenRenderer;

	[SkipSerialisation]
	public AudioSource Audio;

	[SkipSerialisation]
	public VideoPlayer Video;

	[SkipSerialisation]
	public Sprite BrokenTelly;

	[SkipSerialisation]
	public Sprite AltTelly;

	[SkipSerialisation]
	public Sprite AltBrokenTelly;

	[SkipSerialisation]
	public float TemperatureThreshold = 300f;

	[SkipSerialisation]
	public float NoiseInterval;

	public bool Broken;

	public bool Activated;

	public float BreakingTreshold = 2f;

	[Space]
	[SkipSerialisation]
	public VideoClip MessageVideo;

	[SkipSerialisation]
	public AudioClip MessageAudio;

	[SkipSerialisation]
	public Vector2 B2Pos;

	[SkipSerialisation]
	public Map B2Map;

	[SkipSerialisation]
	public VideoClip B2Video;

	[SkipSerialisation]
	public AudioClip B2Audio;

	[SkipSerialisation]
	public VideoClip B3Video;

	[SkipSerialisation]
	public AudioClip B3Audio;

	private bool alt;

	[HideInInspector]
	public double videoTime;

	private VideoClip prevVid;

	private AudioClip prevAud;

	private PhysicalBehaviour physicalBehaviour;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		UpdateActivation();
		Video.time = Math.Min(Video.length, videoTime);
		DateTime now = DateTime.Now;
		if (now.Day == 5 && now.Month == 12)
		{
			Video.clip = B3Video;
			Audio.clip = B3Audio;
			return;
		}
		TimeSpan timeOfDay = now.TimeOfDay;
		TimeSpan timeSpan = new TimeSpan(0, 0, 0);
		TimeSpan timeSpan2 = new TimeSpan(3, 0, 0);
		alt = timeOfDay > timeSpan && timeOfDay < timeSpan2;
		if (alt)
		{
			MyRenderer.sprite = AltTelly;
			Video.clip = MessageVideo;
			Audio.clip = MessageAudio;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		Activated = !Activated;
		UpdateActivation();
	}

	private void Update()
	{
		videoTime = Video.time;
		Video.playbackSpeed = Time.timeScale;
		if (!Broken && physicalBehaviour.Temperature > TemperatureThreshold)
		{
			Break(default(Vector2));
		}
		if (IsInSpecialPos())
		{
			if (!prevVid || !prevAud)
			{
				prevVid = Video.clip;
				prevAud = Audio.clip;
				Video.clip = B2Video;
				Audio.clip = B2Audio;
				if (Activated)
				{
					Stop();
					Audio.time = 0f;
					Video.time = 0.0;
					Activated = true;
					UpdateActivation();
				}
			}
		}
		else if ((bool)prevVid && (bool)prevAud)
		{
			Video.clip = prevVid;
			Audio.clip = prevAud;
			prevVid = null;
			prevAud = null;
			if (Activated)
			{
				Stop();
				Audio.time = 0f;
				Video.time = 0.0;
				Activated = true;
				UpdateActivation();
			}
		}
	}

	private bool IsInSpecialPos()
	{
		if (MapLoaderBehaviour.CurrentMap == B2Map)
		{
			return Vector2.Distance(base.transform.position, B2Pos) < 8f;
		}
		return false;
	}

	private void UpdateActivation()
	{
		ScreenRenderer.enabled = Activated && !Broken;
		if (Activated && !Broken)
		{
			Play();
		}
		else
		{
			Stop();
		}
		if (Broken)
		{
			ScreenRenderer.enabled = false;
			MyRenderer.sprite = (alt ? AltBrokenTelly : BrokenTelly);
		}
		GetComponent<PhysicalBehaviour>().RefreshOutline();
	}

	private void Play()
	{
		Video.Play();
		Audio.Play();
		Audio.time = (float)videoTime;
	}

	private void Stop()
	{
		Video.Stop();
		Audio.Stop();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.GetContact(0).normalImpulse > BreakingTreshold)
		{
			Break(default(Vector2));
		}
	}

	public void Break(Vector2 velocity)
	{
		if (!Broken)
		{
			Broken = true;
			UpdateActivation();
		}
	}

	public void OnEMPHit()
	{
		Break(default(Vector2));
	}

	public void Shot(Shot shot)
	{
		Break(default(Vector2));
	}
}
