using TMPro;
using UnityEngine;

public class CardiopulmonaryBypassMachineBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public float PressureIntensity = 1f;

	[SkipSerialisation]
	public int ValidConnectionCount;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioSource LoopSource;

	[SkipSerialisation]
	public AudioClip ValidSound;

	[SkipSerialisation]
	public AudioClip InvalidSound;

	[SkipSerialisation]
	public GameObject Cosmetics;

	[SkipSerialisation]
	public TextMeshPro StatusText;

	[SkipSerialisation]
	public float TargetHeartrate = 70f;

	[SkipSerialisation]
	public bool IsConnected => ValidConnectionCount > 0;

	public float GetFinalIntensity()
	{
		if (!base.enabled || !Activated)
		{
			return 0f;
		}
		return PressureIntensity;
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
		}
		UpdateActivation();
	}

	public void UpdateActivation()
	{
		bool flag = Activated && base.enabled;
		Cosmetics.SetActive(flag);
		if ((bool)LoopSource)
		{
			if (flag)
			{
				LoopSource.Play();
			}
			else
			{
				LoopSource.Stop();
			}
		}
		if (flag)
		{
			UpdateStatusText();
		}
	}

	public void UpdateStatusText()
	{
		if (ValidConnectionCount == 0)
		{
			StatusText.text = "Inactive";
		}
		else
		{
			StatusText.text = "<color=green>Active</color>";
		}
	}

	private void OnDisable()
	{
		LoopSource.Stop();
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	public void PlayValidSound()
	{
		if (base.enabled)
		{
			PhysicalBehaviour.PlayClipOnce(ValidSound);
		}
	}

	public void PlayInvalidSound()
	{
		if (base.enabled)
		{
			PhysicalBehaviour.PlayClipOnce(InvalidSound);
		}
	}
}
