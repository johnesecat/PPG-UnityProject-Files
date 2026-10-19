using UnityEngine;

[RequireComponent(typeof(BloodContainer))]
public class InfiniteLiquidSourceBehaviour : MonoBehaviour
{
	public string LiquidID;

	private BloodContainer c;

	private void Awake()
	{
		c = GetComponent<BloodContainer>();
	}

	private void Update()
	{
		if (string.IsNullOrWhiteSpace(LiquidID) || !Liquid.HasID(LiquidID))
		{
			Debug.LogWarningFormat("{0} is not a valid liquid", LiquidID);
		}
		else
		{
			c.AddLiquid(Liquid.GetLiquid(LiquidID), Time.deltaTime * 10f);
		}
	}
}
