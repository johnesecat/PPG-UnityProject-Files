using UnityEngine;

public class PanelsBehaviour : MonoBehaviour
{
	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		GetComponent<SpriteRenderer>().color = (UserPreferenceManager.Current.SimplifiedBackground ? Color.clear : Color.white);
	}
}
