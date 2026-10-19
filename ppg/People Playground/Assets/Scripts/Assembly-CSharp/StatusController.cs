using UnityEngine;

public class StatusController : MonoBehaviour
{
	public ToolControllerBehaviour ToolController;

	public ThermalVisionBehaviour ThermalVisionBehaviour;

	public GameObject TimeScaleView;

	public GameObject FramerateView;

	public GameObject ThermalVisionView;

	public GameObject LimbStatusView;

	private void Update()
	{
		bool flag = Mathf.Abs(Time.timeScale - 1f) > 0.02f;
		if (TimeScaleView.activeSelf != flag)
		{
			TimeScaleView.SetActive(flag);
		}
		if (LimbStatusView.activeSelf != Global.main.ShowLimbStatus)
		{
			LimbStatusView.SetActive(Global.main.ShowLimbStatus);
		}
		if (FramerateView.activeSelf != UserPreferenceManager.Current.ShowFramerate)
		{
			FramerateView.SetActive(UserPreferenceManager.Current.ShowFramerate);
		}
		if (ThermalVisionView.activeSelf != ThermalVisionBehaviour.ThermalVisionEnabled)
		{
			ThermalVisionView.SetActive(ThermalVisionBehaviour.ThermalVisionEnabled);
		}
	}
}
