using System;
using UnityEngine;

public class ToolWheelBehaviour : MonoBehaviour
{
	private bool isOpen = true;

	public ToolTab CurrentTab;

	public TabEntry[] Tabs;

	public static ToolWheelBehaviour Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		base.transform.localScale = Vector3.one * 1.2f;
		SetTab(ToolTab.Tools);
	}

	private void Update()
	{
		if (!Global.main.GetPausedMenu() && !DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock && Global.main.UiEnabled)
		{
			if (InputSystem.Up("toybox") && !EnvironmentSettingsController.Main.gameObject.activeInHierarchy)
			{
				Toggle();
			}
			if (InputSystem.Up("toolPowerToggle"))
			{
				SetTab((int)(CurrentTab + 1) % Enum.GetValues(typeof(ToolTab)).Length);
				NotificationControllerBehaviour.Show($"Changed to {CurrentTab} tab");
			}
		}
	}

	public void Toggle()
	{
		isOpen = !isOpen;
		base.transform.localScale = (isOpen ? (Vector3.one * 1.2f) : Vector3.zero);
	}

	public void SetTab(ToolTab tab)
	{
		CurrentTab = tab;
		for (int i = 0; i < Tabs.Length; i++)
		{
			TabEntry tabEntry = Tabs[i];
			tabEntry.Object.SetActive(tabEntry.Tab == CurrentTab);
		}
	}

	public void SetTab(int i)
	{
		SetTab((ToolTab)i);
	}
}
