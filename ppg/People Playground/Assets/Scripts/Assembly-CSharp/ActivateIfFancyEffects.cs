using UnityEngine;

public class ActivateIfFancyEffects : MonoBehaviour
{
	[SkipSerialisation]
	public float Chance = 1f;

	private void Awake()
	{
		base.gameObject.SetActive(Chance > Random.value && UserPreferenceManager.Current.FancyEffects);
	}
}
