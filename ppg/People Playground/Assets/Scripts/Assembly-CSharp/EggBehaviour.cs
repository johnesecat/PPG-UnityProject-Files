using System.Collections;
using TMPro;
using UnityEngine;

[NotDocumented]
public class EggBehaviour : MonoBehaviour
{
	private const int CodeLength = 14;

	public int Number;

	public TextMeshPro TextMesh;

	private AudioSource audioSource;

	private OffsetGridBehaviour controller;

	private SpriteRenderer spriteRenderer;

	private static bool canEnter = true;

	private static string enteredCode = string.Empty;

	private void Awake()
	{
		canEnter = true;
		enteredCode = string.Empty;
		controller = GetComponentInParent<OffsetGridBehaviour>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponentInParent<AudioSource>();
	}

	private void OnEnable()
	{
		base.gameObject.name = Number + " button";
	}

	private void Update()
	{
		if (Application.isPlaying && canEnter && (!TextMesh || TextMesh.text.Length != 14) && (bool)spriteRenderer && spriteRenderer.bounds.Contains(Global.main.MousePosition) && InputSystem.Down("drag"))
		{
			audioSource.PlayOneShot(audioSource.clip);
			TextMesh.text += Number;
			if (TextMesh.text.Length == 14)
			{
				enteredCode = TextMesh.text;
				canEnter = false;
				StartCoroutine(Validate());
			}
		}
	}

	private IEnumerator Validate()
	{
		yield return new WaitForSeconds(0.3f);
		TextMesh.color = Color.cyan;
		TextMesh.text = "Checking~";
		yield return new WaitForSeconds(0.3f);
		StartCoroutine(controller.CheckCode(enteredCode, delegate(OffsetGridBehaviour.Result result)
		{
			switch (result)
			{
			case OffsetGridBehaviour.Result.I:
				TextMesh.text = "Incorrect";
				TextMesh.color = Color.yellow;
				StartCoroutine(Flash(shouldBeEnterableAgain: true));
				break;
			case OffsetGridBehaviour.Result.V:
				TextMesh.text = "Receiving...";
				TextMesh.color = Color.white;
				canEnter = false;
				break;
			case OffsetGridBehaviour.Result.C:
				TextMesh.text = "modulo2.jpg";
				TextMesh.color = Color.green;
				StartCoroutine(Flash(shouldBeEnterableAgain: false, "modulo2.jpg"));
				break;
			case OffsetGridBehaviour.Result.E:
				TextMesh.text = "ERROR";
				TextMesh.color = Color.red;
				StartCoroutine(Flash(shouldBeEnterableAgain: true));
				break;
			}
		}));
	}

	private IEnumerator Flash(bool shouldBeEnterableAgain, string endText = "")
	{
		TextMesh.enabled = true;
		for (int i = 0; i < 8; i++)
		{
			TextMesh.enabled = !TextMesh.enabled;
			yield return new WaitForSeconds(0.1f);
		}
		TextMesh.text = endText;
		TextMesh.enabled = true;
		TextMesh.color = Color.white;
		canEnter = shouldBeEnterableAgain;
	}
}
