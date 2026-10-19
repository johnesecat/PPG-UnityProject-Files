using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomSpriteBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public bool PickRandomOnStart = true;

	[SkipSerialisation]
	public bool RefreshOutlineOnChange = true;

	[SkipSerialisation]
	public Sprite[] sprites = new Sprite[0];

	[HideInInspector]
	public int chosenIndex = -10000;

	private PhysicalBehaviour physicalBehaviour;

	private SpriteRenderer spriteRenderer;

	public UnityEvent OnAfterChange;

	private void Awake()
	{
		if (base.enabled)
		{
			physicalBehaviour = GetComponent<PhysicalBehaviour>();
			spriteRenderer = GetComponent<SpriteRenderer>();
			if (sprites.Length != 0 && PickRandomOnStart)
			{
				chosenIndex = Random.Range(0, sprites.Length);
			}
			SetSpriteToIndex();
		}
	}

	public void SetSpriteToIndex()
	{
		if (sprites.Length != 0)
		{
			if (chosenIndex == -10000)
			{
				chosenIndex = (PickRandomOnStart ? Random.Range(0, sprites.Length) : 0);
			}
			if (chosenIndex >= sprites.Length)
			{
				chosenIndex = 0;
			}
			if (chosenIndex < 0)
			{
				chosenIndex = sprites.Length - 1;
			}
			spriteRenderer.sprite = sprites[chosenIndex];
			if (RefreshOutlineOnChange)
			{
				physicalBehaviour.RefreshOutline();
			}
			OnAfterChange?.Invoke();
		}
	}

	private void Start()
	{
		SetSpriteToIndex();
		List<ContextMenuButton> buttons = physicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("nextSkin", "Next texture", "Switches to the next texture", delegate
		{
			chosenIndex++;
			SetSpriteToIndex();
		})
		{
			LabelWhenMultipleAreSelected = "Next texture"
		};
		buttons.Add(item);
		List<ContextMenuButton> buttons2 = physicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton("previousSkin", "Previous texture", "Switches to the previous texture", delegate
		{
			chosenIndex--;
			SetSpriteToIndex();
		})
		{
			LabelWhenMultipleAreSelected = "Previous texture"
		};
		buttons2.Add(item);
	}
}
