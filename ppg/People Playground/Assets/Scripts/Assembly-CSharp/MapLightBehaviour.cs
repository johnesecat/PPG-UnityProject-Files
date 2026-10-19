using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MapLightBehaviour : MonoBehaviour
{
	public static bool StartEnabled;

	public float TimeIntervalInSeconds = 0.7f;

	public float Order;

	public AudioClip[] Switches;

	public GameObject OnGroup;

	public GameObject OffGroup;

	public AudioSource AudioSource;

	private SpriteRenderer spriteRenderer;

	public bool SetPropertyBlock = true;

	public bool Activated;

	private MaterialPropertyBlock propertyBlock;

	public UnityEvent OnActivate = new UnityEvent();

	public UnityEvent OnDeactivate = new UnityEvent();

	public float FlickerIntensity;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		if (SetPropertyBlock)
		{
			spriteRenderer.GetPropertyBlock(propertyBlock);
		}
	}

	private void Start()
	{
		Activated = StartEnabled;
		UpdateActivated(!StartEnabled);
		StartCoroutine(DelayedActivation());
	}

	private IEnumerator DelayedActivation(int factor = 4)
	{
		yield return new WaitForSeconds(TimeIntervalInSeconds * Random.Range(1f, 1.1f) * (Order + (float)factor));
		Activated = true;
		UpdateActivated();
	}

	public void Deactivate(int delayFactor = 4)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			StartCoroutine(DelayedDeactivation(delayFactor));
		}
	}

	public void Activate(int delayFactor = 4)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			StartCoroutine(DelayedActivation(delayFactor));
		}
	}

	private IEnumerator DelayedDeactivation(int factor = 4)
	{
		yield return new WaitForSeconds(TimeIntervalInSeconds * (Order + (float)factor));
		Activated = false;
		if ((bool)AudioSource)
		{
			AudioSource.PlayOneShot(Switches.PickRandom());
		}
		UpdateActivated();
	}

	public void ActivateInstantly()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			Activated = true;
			UpdateActivated(playSound: false);
		}
	}

	public void DeactivateInstantly()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			Activated = false;
			UpdateActivated(playSound: false);
		}
	}

	private void UpdateActivated(bool playSound = true)
	{
		if (Activated)
		{
			OnActivate?.Invoke();
			if (playSound && (bool)AudioSource)
			{
				AudioSource.PlayOneShot(Switches.PickRandom());
			}
			if ((bool)OnGroup)
			{
				OnGroup.SetActive(value: true);
			}
			if (SetPropertyBlock)
			{
				propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 1f);
			}
			if ((bool)OffGroup)
			{
				OffGroup.SetActive(value: false);
			}
		}
		else
		{
			OnDeactivate?.Invoke();
			if ((bool)OnGroup)
			{
				OnGroup.SetActive(value: false);
			}
			if (SetPropertyBlock)
			{
				propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 0f);
			}
			if ((bool)OffGroup)
			{
				OffGroup.SetActive(value: true);
			}
		}
		if (SetPropertyBlock)
		{
			spriteRenderer.SetPropertyBlock(propertyBlock);
		}
	}
}
