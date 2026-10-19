using UnityEngine;

public class DisableWhenControlTrigger : MonoBehaviour
{
	public string Control;

	public string IdentityOverride;

	public DisableWhenControlTrigger NextStep;

	public bool NeverSkip;

	private string Identity
	{
		get
		{
			if (string.IsNullOrWhiteSpace(IdentityOverride))
			{
				return base.name;
			}
			return IdentityOverride;
		}
	}

	private void Update()
	{
		if (InputSystem.Up(Control) || (PlayerPrefs.HasKey(Identity) && !NeverSkip))
		{
			PlayerPrefs.SetInt(Identity, 0);
			base.gameObject.SetActive(value: false);
			if ((bool)NextStep)
			{
				NextStep.gameObject.SetActive(value: true);
			}
		}
	}
}
