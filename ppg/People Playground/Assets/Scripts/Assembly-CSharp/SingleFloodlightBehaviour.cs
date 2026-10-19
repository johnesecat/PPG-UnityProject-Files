using System;
using UnityEngine;

public class SingleFloodlightBehaviour : MonoBehaviour, Messages.IUse
{
	[Serializable]
	public class Highlight
	{
		public SpriteRenderer SpriteRenderer;

		public Color Colour;

		public float Intensity;

		[HideInInspector]
		public MaterialPropertyBlock PropertyBlock;

		public void Initialise()
		{
			PropertyBlock = new MaterialPropertyBlock();
			SpriteRenderer.GetPropertyBlock(PropertyBlock);
		}
	}

	public bool Activated;

	[SkipSerialisation]
	public GameObject ToToggle;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer Renderer;

	[SkipSerialisation]
	public Highlight[] Highlights;

	[SkipSerialisation]
	public AudioSource SwitchSoundSource;

	[SkipSerialisation]
	public AudioClip[] SwitchSounds;

	[SkipSerialisation]
	public AudioClip[] OnSwitchSound;

	private MaterialPropertyBlock propertyBlock;

	private void Awake()
	{
		if ((bool)Renderer)
		{
			propertyBlock = new MaterialPropertyBlock();
			Renderer.GetPropertyBlock(propertyBlock);
		}
		Highlight[] highlights = Highlights;
		for (int i = 0; i < highlights.Length; i++)
		{
			highlights[i].Initialise();
		}
	}

	private void Start()
	{
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
			SwitchSoundSource.PlayOneShot(SwitchSounds.PickRandom());
		}
	}

	private void Update()
	{
		float num = PhysicalBehaviour.Charge / 20f;
		if ((bool)Renderer)
		{
			propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), Activated ? (num * 0.01f + 0.09f) : 0f);
		}
		Highlight[] highlights = Highlights;
		foreach (Highlight highlight in highlights)
		{
			highlight.PropertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), Activated ? (num + highlight.Intensity) : 0f);
			highlight.SpriteRenderer.SetPropertyBlock(highlight.PropertyBlock);
			highlight.SpriteRenderer.color = Color.Lerp(highlight.Colour, Color.white, num);
		}
	}

	private void UpdateActivation()
	{
		if (Activated)
		{
			SwitchSoundSource.Play();
			if (OnSwitchSound != null && OnSwitchSound.Length != 0)
			{
				SwitchSoundSource.PlayOneShot(OnSwitchSound.PickRandom());
			}
			ToToggle.SetActive(value: true);
		}
		else
		{
			SwitchSoundSource.Stop();
			ToToggle.SetActive(value: false);
		}
	}

	private void OnWillRenderObject()
	{
		if (base.enabled && (bool)Renderer)
		{
			Renderer.SetPropertyBlock(propertyBlock);
		}
	}

	private void OnDisable()
	{
		Activated = false;
		SwitchSoundSource.Stop();
		ToToggle.SetActive(value: false);
		Highlight[] highlights = Highlights;
		foreach (Highlight highlight in highlights)
		{
			highlight.PropertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 0f);
			highlight.SpriteRenderer.SetPropertyBlock(highlight.PropertyBlock);
		}
	}
}
