using UnityEngine;

public abstract class ConditionalSettingBehaviour : MonoBehaviour
{
	public void Sync()
	{
		base.gameObject.SetActive(GetShouldBeActive());
	}

	public abstract bool GetShouldBeActive();
}
