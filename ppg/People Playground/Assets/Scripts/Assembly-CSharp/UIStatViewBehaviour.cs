using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatViewBehaviour : MonoBehaviour
{
	public string StatID = "";

	public string StatName = "";

	public bool RoundToInt;

	public Sprite Icon;

	public bool GetFromSteam;

	[Space]
	public TextMeshProUGUI NameDisplay;

	public TextMeshProUGUI ValueDisplay;

	public Image IconImage;

	private void Start()
	{
		IconImage.sprite = Icon;
		NameDisplay.text = StatName;
		if (GetFromSteam && SteamworksInitialiser.IsInitialised)
		{
			NonSteamStatManager.Stats.SetStat(StatID, StatManager.GetInt(new StatManager.Stat(StatID)));
		}
		UpdateValue();
	}

	private void OnEnable()
	{
		UpdateValue();
	}

	private void UpdateValue()
	{
		float f = NonSteamStatManager.Stats.GetStat(StatID);
		if (RoundToInt)
		{
			f = Mathf.RoundToInt(f);
		}
		ValueDisplay.text = f.ToString();
	}
}
