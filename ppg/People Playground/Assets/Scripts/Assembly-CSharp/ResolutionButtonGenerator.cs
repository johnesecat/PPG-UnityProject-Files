using System.Linq;
using TMPro;
using UnityEngine;

public class ResolutionButtonGenerator : MonoBehaviour
{
	public GameObject ButtonPrefab;

	private void Awake()
	{
		foreach (Resolution item in from r in Screen.resolutions.Distinct(new ResolutionComparer())
			orderby r.width * r.height descending
			select r)
		{
			GameObject obj = Object.Instantiate(ButtonPrefab, base.transform);
			obj.GetComponent<ResolutionRadioButton>().Value = new Vector2Int(item.width, item.height);
			obj.GetComponentInChildren<TextMeshProUGUI>().text = item.width + "x" + item.height;
		}
	}
}
