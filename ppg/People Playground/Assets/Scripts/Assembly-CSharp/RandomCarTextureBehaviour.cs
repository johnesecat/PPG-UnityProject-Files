using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomCarTextureBehaviour : MonoBehaviour
{
	[Serializable]
	public struct CarSprites
	{
		public Sprite Body;

		public Sprite Background;

		public Sprite Bonnet;

		public Sprite Boot;

		public Sprite FrontDoor;

		public Sprite BackDoor;
	}

	[SkipSerialisation]
	public SpriteRenderer Body;

	[SkipSerialisation]
	public SpriteRenderer Background;

	[SkipSerialisation]
	public SpriteRenderer Bonnet;

	[SkipSerialisation]
	public SpriteRenderer Boot;

	[SkipSerialisation]
	public SpriteRenderer FrontDoor;

	[SkipSerialisation]
	public SpriteRenderer BackDoor;

	[SkipSerialisation]
	public CarSprites[] Textures;

	[HideInInspector]
	public int chosenIndex = -1;

	private PhysicalBehaviour physicalBehaviour;

	public UnityEvent OnAfterChange;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		if (base.enabled)
		{
			if (Textures.Length != 0)
			{
				chosenIndex = UnityEngine.Random.Range(0, Textures.Length);
			}
			SetSpritesToIndex();
		}
	}

	private void SetSpritesToIndex()
	{
		if (Textures.Length != 0)
		{
			if (chosenIndex == -1)
			{
				chosenIndex = UnityEngine.Random.Range(0, Textures.Length);
			}
			if (chosenIndex >= Textures.Length)
			{
				chosenIndex = 0;
			}
			if (chosenIndex < 0)
			{
				chosenIndex = Textures.Length - 1;
			}
			CarSprites carSprites = Textures[chosenIndex];
			Body.sprite = carSprites.Body;
			Background.sprite = carSprites.Background;
			Bonnet.sprite = carSprites.Bonnet;
			Boot.sprite = carSprites.Boot;
			FrontDoor.sprite = carSprites.FrontDoor;
			BackDoor.sprite = carSprites.BackDoor;
			physicalBehaviour.RefreshOutline();
			OnAfterChange?.Invoke();
		}
	}

	private void Start()
	{
		SetSpritesToIndex();
		List<ContextMenuButton> buttons = physicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("nextSkin", "Next texture", "Switches to the next texture", delegate
		{
			chosenIndex++;
			SetSpritesToIndex();
		})
		{
			LabelWhenMultipleAreSelected = "Next texture"
		};
		buttons.Add(item);
		List<ContextMenuButton> buttons2 = physicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton("previousSkin", "Previous texture", "Switches to the previous texture", delegate
		{
			chosenIndex--;
			SetSpritesToIndex();
		})
		{
			LabelWhenMultipleAreSelected = "Previous texture"
		};
		buttons2.Add(item);
	}
}
