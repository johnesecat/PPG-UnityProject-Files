using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[Obsolete]
public abstract class RadioButtonBehaviour : MonoBehaviour
{
	public Image toggleImage;

	public Button button;

	public event EventHandler OnClick;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(Click);
	}

	private void OnEnable()
	{
		toggleImage = base.transform.GetChild(1).GetComponent<Image>();
	}

	public void Click()
	{
		if (this.OnClick != null)
		{
			this.OnClick(this, EventArgs.Empty);
		}
	}

	public void Check()
	{
		toggleImage.enabled = true;
	}

	public void Uncheck()
	{
		toggleImage.enabled = false;
	}

	public abstract object GetValue();
}
