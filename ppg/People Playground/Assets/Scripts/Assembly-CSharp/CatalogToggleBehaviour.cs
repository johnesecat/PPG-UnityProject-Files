using UnityEngine;

public class CatalogToggleBehaviour : MonoBehaviour
{
	public Canvas Canvas;

	public RectTransform ToToggle;

	public bool ToggledOn = true;

	public float speed = 0.7f;

	private void Update()
	{
		if (Global.main.GetPausedMenu() || DialogBox.IsAnyDialogboxOpen || Global.ActiveUiBlock)
		{
			return;
		}
		if (InputSystem.Up("toggleUi"))
		{
			Global.main.UiEnabled = !Global.main.UiEnabled;
			Global.main.OnUiToggle.Invoke();
			UpdateUiVisibility();
		}
		if ((bool)Canvas && Canvas.enabled)
		{
			if (InputSystem.Up("toybox") && !EnvironmentSettingsController.Main.gameObject.activeInHierarchy)
			{
				ToggledOn = !ToggledOn;
				CatalogBehaviour.Main.ResizableUIElementHandle.gameObject.SetActive(ToggledOn);
			}
			ToToggle.localScale = Vector3.Lerp(ToToggle.localScale, ToggledOn ? Vector3.one : new Vector3(0f, 1f, 1f), speed);
		}
	}

	public void UpdateUiVisibility()
	{
		Canvas.enabled = Global.main.UiEnabled;
	}
}
