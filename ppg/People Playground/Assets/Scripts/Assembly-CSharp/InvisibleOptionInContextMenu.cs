using System.Collections.Generic;
using UnityEngine;

public class InvisibleOptionInContextMenu : MonoBehaviour
{
	public bool Invisible;

	public bool VisibleInDetailView = true;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		Invisible = !GetComponent<SpriteRenderer>().enabled;
		Global.main.LimbStatusToggled += DetailViewToggled;
	}

	private void OnDestroy()
	{
		Global.main.LimbStatusToggled -= DetailViewToggled;
	}

	private void DetailViewToggled(object sender, bool e)
	{
		UpdateVisibility();
	}

	private void Start()
	{
		List<ContextMenuButton> buttons = phys.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("toggleInvisibility", () => (!phys.spriteRenderer.enabled) ? "Make visible" : "Make invisible", "Toggle invisibility", delegate
		{
			Invisible = !Invisible;
			UpdateVisibility();
		})
		{
			Condition = () => phys.spriteRenderer
		};
		buttons.Add(item);
		UpdateVisibility();
	}

	public void UpdateVisibility()
	{
		phys.spriteRenderer.enabled = !Invisible || (VisibleInDetailView && Global.main.ShowLimbStatus);
	}
}
