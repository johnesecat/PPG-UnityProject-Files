using UnityEngine;

public class GameObjectComment : MonoBehaviour
{
	public bool IsEditable = true;

	public string TextInfo = "Add comment and lock when finished";

	public int MessageTypeAsInt;

	public void ToggleIsEditable()
	{
		IsEditable = !IsEditable;
	}

	private void Start()
	{
		base.enabled = false;
	}
}
