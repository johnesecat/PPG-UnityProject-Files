using UnityEngine;

public class IncreaseStatOnStart : MonoBehaviour
{
	public string StatName;

	public bool IsFloat;

	private void Start()
	{
		if (IsFloat)
		{
			StatManager.IncrementFloat(new StatManager.Stat(StatName));
		}
		else
		{
			StatManager.IncrementInteger(new StatManager.Stat(StatName));
		}
	}
}
