using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class JukeboxBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public AudioClip[] tracks;

	[SkipSerialisation]
	public AudioSource audioSource;

	[SkipSerialisation]
	public PhysicalBehaviour physicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer renderer;

	[SkipSerialisation]
	public Gradient glowGradient;

	[SkipSerialisation]
	public float glowGradientSpeed = 0.1f;

	[SkipSerialisation]
	public SpriteRenderer glow;

	[SkipSerialisation]
	[FormerlySerializedAs("Distorion")]
	public AudioDistortionFilter Distortion;

	private MaterialPropertyBlock propertyBlock;

	public const bool ShouldLoadCustomMusic = true;

	private int trackIndex;

	private float perceivedVolume;

	private float previousPerceivedVolume;

	private readonly Collider2D[] buffer = new Collider2D[64];

	public float DamageRadius = 15f;

	public LayerMask DamageLayers;

	private float chaos;

	private void Start()
	{
		Distortion.enabled = false;
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
		List<AudioClip> list = tracks.ToList();
		if (JukeboxSongLoader.CustomSongs != null)
		{
			if (JukeboxSongLoader.CustomSongs.Any())
			{
				list.Clear();
			}
			list.AddRange(JukeboxSongLoader.CustomSongs);
			tracks = list.ToArray();
		}
		physicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("jukeboxNext", "Next track", "Next track", NextSong));
		physicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("jukeboxPrevious", "Previous track", "Previous track", PreviousSong));
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			if (!audioSource.isPlaying)
			{
				NextSong();
			}
			else
			{
				audioSource.Stop();
			}
		}
	}

	private void OnDisable()
	{
		audioSource.Stop();
		perceivedVolume = 0f;
		glow.enabled = false;
	}

	private void OnEnable()
	{
		glow.enabled = true;
	}

	public void NextSong()
	{
		if (base.enabled)
		{
			audioSource.clip = SetTrackFromOffset(1);
			audioSource.Play();
			ShowPlayingNotification();
		}
	}

	public void PreviousSong()
	{
		if (base.enabled)
		{
			audioSource.clip = SetTrackFromOffset(-1);
			audioSource.Play();
			ShowPlayingNotification();
		}
	}

	public void PlayIndex(int index)
	{
		audioSource.clip = SetTrackFromIndex(index);
		audioSource.Play();
		ShowPlayingNotification();
	}

	private void ShowPlayingNotification()
	{
		try
		{
			NotificationControllerBehaviour.Show("Now playing <color=#ff54f9>" + Path.GetFileNameWithoutExtension(audioSource.clip.name) + "</color>");
		}
		catch (Exception)
		{
			NotificationControllerBehaviour.Show("Now playing <color=#ff54f9>" + audioSource.clip.name + "</color>");
		}
	}

	private AudioClip SetTrackFromOffset(int offset)
	{
		trackIndex += offset;
		if (trackIndex >= tracks.Length)
		{
			trackIndex = 0;
		}
		if (trackIndex < 0)
		{
			trackIndex = tracks.Length - 1;
		}
		return tracks[trackIndex];
	}

	private AudioClip SetTrackFromIndex(int index)
	{
		trackIndex = index;
		if (trackIndex >= tracks.Length)
		{
			trackIndex = 0;
		}
		if (trackIndex < 0)
		{
			trackIndex = tracks.Length - 1;
		}
		return tracks[trackIndex];
	}

	private void FixedUpdate()
	{
		audioSource.minDistance = physicalBehaviour.Charge * 3f + 15f;
		audioSource.maxDistance = physicalBehaviour.Charge * 10f + 1500f;
		audioSource.spread = physicalBehaviour.Charge * 10f;
		audioSource.spatialBlend = 1f - Mathf.Clamp01(physicalBehaviour.Charge / 10f);
		if ((bool)audioSource.clip && audioSource.isPlaying)
		{
			CalculateVisualiser();
			chaos = Mathf.Clamp01(physicalBehaviour.Charge / 1000f);
			Distortion.distortionLevel = Mathf.Sqrt(chaos);
			if ((double)chaos > 0.1)
			{
				CameraShakeBehaviour.main.Shake(perceivedVolume * chaos * 4f, base.transform.position, 0f);
				if ((perceivedVolume - previousPerceivedVolume) * chaos > 0.025f)
				{
					DestroyYourEars();
				}
			}
			Distortion.enabled = true;
		}
		else
		{
			perceivedVolume = 0f;
			Distortion.enabled = false;
		}
		previousPerceivedVolume = perceivedVolume;
		SetRenderParams();
	}

	public void StopMusic()
	{
		audioSource.Stop();
	}

	private void SetRenderParams()
	{
		float num = perceivedVolume * Mathf.Max(1f, 10f * chaos);
		propertyBlock.SetFloat(ShaderProperties.Get("_MusicVolume"), num);
		propertyBlock.SetTexture(ShaderProperties.Get("_MainTex"), renderer.sprite.texture);
		renderer.SetPropertyBlock(propertyBlock);
		Color color = glowGradient.Evaluate(Time.time * glowGradientSpeed % 1f);
		color *= num * 4f;
		color.a = 1f;
		glow.color = color;
	}

	private void DestroyYourEars()
	{
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, DamageRadius, buffer, DamageLayers);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (!AliveBehaviour.AliveByTransform.TryGetValue(collider2D.transform.root, out var value))
			{
				continue;
			}
			PersonBehaviour personBehaviour = value as PersonBehaviour;
			if ((object)personBehaviour == null)
			{
				continue;
			}
			LimbBehaviour[] limbs = personBehaviour.Limbs;
			foreach (LimbBehaviour limbBehaviour in limbs)
			{
				if (limbBehaviour.HasBrain)
				{
					limbBehaviour.Crush();
					personBehaviour.Wince();
					StatManager.UnlockAchievement("JUKEBOX_HEAD_EXPLOSION");
				}
			}
		}
	}

	private void CalculateVisualiser()
	{
		float[] array = new float[256];
		audioSource.GetSpectrumData(array, 0, FFTWindow.Rectangular);
		float f = array.Average() * 27f;
		f = Mathf.Pow(f, 1.5f);
		f = (perceivedVolume = Mathf.Clamp01(f));
	}
}
