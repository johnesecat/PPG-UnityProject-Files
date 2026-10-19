using UnityEngine;

public class ExistInThermalVision : MonoBehaviour
{
	private void Start()
	{
		ThermalVisionBehaviour.Instance.OnThermalVisionToggle.AddListener(ThermalVisionToggled);
		base.gameObject.SetActive(ThermalVisionBehaviour.Instance.ThermalVisionEnabled);
	}

	private void ThermalVisionToggled()
	{
		base.gameObject.SetActive(ThermalVisionBehaviour.Instance.ThermalVisionEnabled);
	}

	private void OnDestroy()
	{
		ThermalVisionBehaviour.Instance.OnThermalVisionToggle.RemoveListener(ThermalVisionToggled);
	}
}
