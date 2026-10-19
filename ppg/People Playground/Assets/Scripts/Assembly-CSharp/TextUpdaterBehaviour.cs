using System;
using TMPro;
using UnityEngine;

public class TextUpdaterBehaviour : MonoBehaviour
{
	public Func<string> StringFunction;

	private TextMeshProUGUI textMesh;

	private void Awake()
	{
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	private void Update()
	{
		textMesh.text = StringFunction();
	}
}
