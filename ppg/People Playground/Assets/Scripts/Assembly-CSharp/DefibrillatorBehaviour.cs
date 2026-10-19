using System.Collections;
using TMPro;
using UnityEngine;

public class DefibrillatorBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioClip StandClearClip;

	[SkipSerialisation]
	public AudioClip AudioIndicator;

	[SkipSerialisation]
	public TextMeshPro TextScreen;

	[SkipSerialisation]
	public PhysicalBehaviour Pad1;

	[SkipSerialisation]
	public PhysicalBehaviour Pad2;

	private bool busy;

	private Collider2D[] buffer = new Collider2D[16];

	private int bufferSize;

	[SkipSerialisation]
	public LayerMask PadLayer;

	private void Start()
	{
		TextScreen.text = string.Empty;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled && !busy)
		{
			StartCoroutine(DefibRoutine());
		}
	}

	private IEnumerator DefibRoutine()
	{
		busy = true;
		TextScreen.text = "STAND BY";
		PhysicalBehaviour.PlayClipOnce(StandClearClip);
		yield return new WaitForSeconds(2f);
		TextScreen.text = "CLEAR";
		PhysicalBehaviour.PlayClipOnce(AudioIndicator);
		Defibrillate(Pad1);
		Defibrillate(Pad2);
		yield return new WaitForSeconds(AudioIndicator.length);
		TextScreen.text = "";
		busy = false;
	}

	private void Defibrillate(PhysicalBehaviour pad)
	{
		pad.Charge += 0.2f;
		Transform transform = pad.transform;
		bufferSize = Physics2D.OverlapCircleNonAlloc(transform.TransformPoint(-0.1f, 0f, 0f), 0.1f, buffer, PadLayer);
		for (int i = 0; i < bufferSize; i++)
		{
			Collider2D collider2D = buffer[i];
			if ((bool)collider2D && collider2D.transform.TryGetComponent<CirculationBehaviour>(out var component))
			{
				component.IsPump = component.WasInitiallyPumping;
				component.BloodFlow = 1f;
				component.Limb.Person.AdrenalineLevel += 20f;
				component.Limb.Person.Consciousness = 1f;
			}
		}
	}

	private void OnEnable()
	{
		TextScreen.enabled = true;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		TextScreen.enabled = false;
		TextScreen.text = string.Empty;
		busy = false;
	}
}
