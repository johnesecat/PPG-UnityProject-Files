using UnityEngine;

public class DisableWhenWet : MonoBehaviour
{
	public PhysicalBehaviour PhysicalBehaviour;

	public GameObject ToDisable;

	private void Update()
	{
		if (PhysicalBehaviour.Wetness > 0f && ToDisable.activeSelf)
		{
			ToDisable.SetActive(value: false);
		}
		else if (!ToDisable.activeSelf)
		{
			ToDisable.SetActive(value: true);
		}
	}
}
