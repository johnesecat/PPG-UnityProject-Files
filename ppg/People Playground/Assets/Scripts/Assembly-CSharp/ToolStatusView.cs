using UnityEngine;
using UnityEngine.UI;

public class ToolStatusView : MonoBehaviour
{
	public Image Image;

	public ToolControllerBehaviour ToolController;

	private void Start()
	{
		ToolController.ToolChanged += ToolChanged;
		if ((bool)ToolController.CurrentTool)
		{
			Image.sprite = GetByType(ToolController.CurrentTool);
		}
	}

	private void ToolChanged(object sender, ToolControllerBehaviour.ToolChangeEventArgs e)
	{
		base.gameObject.SetActive(ToolController.CurrentTool);
		if ((bool)ToolController.CurrentTool)
		{
			Image.sprite = GetByType(ToolController.CurrentTool);
		}
	}

	private Sprite GetByType(ToolBehaviour behaviour)
	{
		for (int i = 0; i < ToolLibrary.Instance.Tools.Count; i++)
		{
			ToolLibrary.Tool tool = ToolLibrary.Instance.Tools[i];
			if (tool.Type == behaviour.GetType().Name)
			{
				return tool.Icon;
			}
		}
		for (int j = 0; j < ToolLibrary.Instance.Powers.Count; j++)
		{
			ToolLibrary.Tool tool2 = ToolLibrary.Instance.Powers[j];
			if (tool2.Type == behaviour.GetType().Name)
			{
				return tool2.Icon;
			}
		}
		return null;
	}

	private void OnDestroy()
	{
		ToolController.ToolChanged -= ToolChanged;
	}
}
