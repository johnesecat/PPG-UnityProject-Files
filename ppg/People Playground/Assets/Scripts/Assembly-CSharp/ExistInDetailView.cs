using UnityEngine;

public class ExistInDetailView : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(Global.main.ShowLimbStatus);
		Global.main.LimbStatusToggled += Main_LimbStatusToggled;
	}

	private void Main_LimbStatusToggled(object sender, bool e)
	{
		base.gameObject.SetActive(e);
	}

	private void OnDestroy()
	{
		Global.main.LimbStatusToggled -= Main_LimbStatusToggled;
	}
}
