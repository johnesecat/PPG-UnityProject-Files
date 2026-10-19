using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ConditionalTextBehaviour : MonoBehaviour
{
	public TextMeshProUGUI TextMesh;

	public ContextMenuBehaviour Component;

	public string Property = "enabled";

	public string TrueText = "Yes";

	public string FalseText = "No";

	private const float interval = 0.1f;

	private float t;

	private PropertyInfo foundProperty;

	private void Awake()
	{
		if (!TextMesh)
		{
			TextMesh = GetComponent<TextMeshProUGUI>();
		}
		foundProperty = Component.GetType().GetProperty(Property);
		if (foundProperty == null)
		{
			Debug.LogWarning("Property " + Property + " couldn't be found");
		}
	}

	private void Update()
	{
		t += Time.unscaledDeltaTime;
		if (t < 0.1f)
		{
			return;
		}
		t = 0f;
		if (foundProperty == null || Component == null || !Component)
		{
			return;
		}
		try
		{
			object obj = foundProperty?.GetValue(Component);
			if (obj != null)
			{
				bool flag = (bool)obj;
				TextMesh.text = (flag ? TrueText : FalseText);
			}
		}
		catch (Exception ex)
		{
			if (ex.InnerException != null)
			{
				Debug.LogError(ex.InnerException.Message);
			}
			else
			{
				Debug.LogError(ex.Message);
			}
		}
	}
}
