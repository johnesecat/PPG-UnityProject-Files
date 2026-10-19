using UnityEngine;

public class ResolutionRadioButton : RadioButtonBehaviour
{
	public Vector2Int? Value;

	public override object GetValue()
	{
		return Value;
	}
}
