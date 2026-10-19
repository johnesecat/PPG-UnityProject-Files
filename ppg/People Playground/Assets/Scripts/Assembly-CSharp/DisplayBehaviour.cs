using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DisplayBehaviour : MonoBehaviour, Messages.IUse
{
	[Serializable]
	[NotDocumented]
	public struct TotallyNotAnEasterEgg
	{
		public string Match;

		public UnityEvent Event;
	}

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer Light;

	[SkipSerialisation]
	public TextMeshPro TextMesh;

	[Space]
	public string Value;

	public bool Activated = true;

	[SkipSerialisation]
	[NotDocumented]
	public TotallyNotAnEasterEgg[] DontMindMe;

	[SerializeField]
	[SkipSerialisation]
	[ReorderableList]
	private string[] randomStartStrings = new string[0];

	private void Awake()
	{
		if ((double)UnityEngine.Random.value > 0.9)
		{
			Value = randomStartStrings.PickRandom();
		}
	}

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("changeDisplayText", "Edit text", "Change what the display displays on its display", delegate
		{
			Utils.OpenTextInputDialog(Value, this, setTextFromUserInput, "What should it say?", "Enter your text here");
		}));
		UpdateDisplay();
		UpdateActivation();
		void setTextFromUserInput(DisplayBehaviour t, string value)
		{
			t.Value = value;
			TotallyNotAnEasterEgg[] dontMindMe = t.DontMindMe;
			for (int i = 0; i < dontMindMe.Length; i++)
			{
				TotallyNotAnEasterEgg totallyNotAnEasterEgg = dontMindMe[i];
				if (totallyNotAnEasterEgg.Match == Value)
				{
					totallyNotAnEasterEgg.Event.Invoke();
				}
			}
			t.UpdateDisplay();
		}
	}

	public void UpdateDisplay()
	{
		TextMesh.text = Value;
	}

	public void UpdateActivation()
	{
		Light.enabled = Activated;
		TextMesh.gameObject.SetActive(Activated);
		if (Activated)
		{
			UpdateDisplay();
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivation();
	}
}
