using UnityEngine;

public class EditableTintBehaviour : MonoBehaviour
{
	public Color Tint;

	private PhysicalBehaviour phys;

	private MaterialPropertyBlock propertyBlock;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	public void SetTint(Color color)
	{
		Tint = color;
		phys.spriteRenderer.GetPropertyBlock(propertyBlock);
		propertyBlock.SetColor(ShaderProperties.Get("_Tint"), color);
		phys.spriteRenderer.SetPropertyBlock(propertyBlock);
	}

	private void Start()
	{
		propertyBlock = new MaterialPropertyBlock();
		phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => !ColorpickerDialogBehaviour.IsOpen, "setEditableTint", "Change colour", "Change the colour of this object", delegate
		{
			Utils.OpenColourInputDialog(Tint, "Pick a colour", "Set tint for selected objects", delegate(EditableTintBehaviour obj, Color c)
			{
				obj.SetTint(c);
			});
		}));
		SetTint(Tint);
	}
}
