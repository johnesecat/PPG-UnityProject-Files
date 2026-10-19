using UnityEngine;

public class ExistByChance : MonoBehaviour
{
	[Range(0f, 1f)]
	public float Chance;

	private void Start()
	{
		if (Random.value > Chance)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
