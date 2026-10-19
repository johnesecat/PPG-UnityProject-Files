using UnityEngine;

public class LightningTool : ToolBehaviour
{
	private LightningToolEntityBehaviour lightningEntity;

	private bool hasSelected;

	private void Awake()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/LightningToolEntity"), Global.main.MousePosition, Quaternion.identity);
		lightningEntity = gameObject.GetComponent<LightningToolEntityBehaviour>();
		gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (!lightningEntity.gameObject.activeSelf)
		{
			return;
		}
		if (InputSystem.Up("drag"))
		{
			lightningEntity.StopHold();
		}
		if (!DialogBox.IsAnyDialogboxOpen && !Global.main.UILock && !Global.ActiveUiBlock && !TriggerEditorBehaviour.IsBeingEdited)
		{
			lightningEntity.transform.position = Global.main.MousePosition;
			if (InputSystem.Down("drag"))
			{
				lightningEntity.StartHold();
			}
		}
	}

	public override void OnDeselect()
	{
	}

	public override void OnFixedHold()
	{
	}

	public override void OnHold()
	{
	}

	public override void OnSelect()
	{
	}

	public override void OnToolChosen()
	{
		lightningEntity.gameObject.SetActive(value: true);
	}

	public override void OnToolUnchosen()
	{
		lightningEntity.gameObject.SetActive(value: false);
	}
}
