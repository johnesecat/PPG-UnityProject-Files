using UnityEngine;

public class FreezeSerialisationHelper : MonoBehaviour, Messages.IOnUserFreeze, Messages.IOnUserUnfreeze
{
	public bool IsFrozen;

	private void Start()
	{
		FreezeBehaviour component;
		if (IsFrozen)
		{
			base.gameObject.GetOrAddComponent<FreezeBehaviour>();
		}
		else if (base.gameObject.TryGetComponent<FreezeBehaviour>(out component))
		{
			Object.Destroy(component);
		}
	}

	public void OnUserFreeze()
	{
		IsFrozen = true;
	}

	public void OnUserUnfreeze()
	{
		IsFrozen = false;
	}
}
