using UnityEngine;
using UnityEngine.UI;

public abstract class Toggle : MonoBehaviour
{
	public Image image;

	protected abstract bool Visible { get; }

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	protected virtual void Update()
	{
		image.enabled = Visible;
	}
}
