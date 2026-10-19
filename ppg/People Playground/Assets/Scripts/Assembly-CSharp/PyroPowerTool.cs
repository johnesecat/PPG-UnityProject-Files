using UnityEngine;

public class PyroPowerTool : ToolBehaviour
{
	private PyroPowerToolEntity pyroEntity;

	private bool hasSelected;

	private void Awake()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/PyroPowerEntity"), Global.main.MousePosition, Quaternion.identity);
		pyroEntity = gameObject.GetComponent<PyroPowerToolEntity>();
	}

	private void Update()
	{
		if (!pyroEntity.IsCurrentTool)
		{
			return;
		}
		if (InputSystem.Up("drag"))
		{
			pyroEntity.StopHold();
		}
		if (!DialogBox.IsAnyDialogboxOpen && !Global.main.UILock && !Global.ActiveUiBlock && !TriggerEditorBehaviour.IsBeingEdited)
		{
			pyroEntity.transform.position = Global.main.MousePosition;
			if (InputSystem.Down("drag"))
			{
				pyroEntity.StartHold();
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
		pyroEntity.IsCurrentTool = true;
	}

	public override void OnToolUnchosen()
	{
		pyroEntity.StopHold();
		pyroEntity.IsCurrentTool = false;
	}
}
