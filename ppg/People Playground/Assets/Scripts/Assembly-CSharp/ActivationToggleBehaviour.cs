using UnityEngine;

public class ActivationToggleBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject LightObject;

	public bool Activated;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		phys.OnSingleUse.AddListener(OnUse);
	}

	private void Start()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
		UpdateActivation();
	}

	private void OnUse(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		LightObject.SetActive(Activated);
	}

	private void OnDestroy()
	{
		phys.OnSingleUse.RemoveListener(OnUse);
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}

	private void OnContinuousUpdate(float arg0)
	{
		if (Activated)
		{
			Utils.SendAllChannelContinuousIsolatedActivation(this);
		}
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivation();
	}
}
