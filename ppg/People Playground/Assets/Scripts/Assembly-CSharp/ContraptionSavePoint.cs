using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContraptionSavePoint : MonoBehaviour
{
	public static ContraptionSavePoint Main;

	[HideInInspector]
	public GameObject[] GameObjectsToSave;

	[HideInInspector]
	public string NameToSave;

	[HideInInspector]
	public Vector3 NewOrigin;

	private bool lockName;

	public TMP_InputField TMP_InputField;

	private void Start()
	{
		Main = this;
	}

	public void SetName(string name)
	{
		if (!lockName)
		{
			NameToSave = name;
		}
	}

	public void DoSave()
	{
		lockName = true;
		Serialise();
	}

	private void Serialise()
	{
		if (string.IsNullOrWhiteSpace(NameToSave.Normalize()) || NameToSave.Length == 0)
		{
			NameToSave = "Contraption " + UnityEngine.Random.Range(1000, 9999);
		}
		try
		{
			ObjectState[] states = ObjectStateConverter.Convert(GameObjectsToSave, NewOrigin);
			ContraptionSerialiser.SaveThumbnail(states, NameToSave);
			ContraptionSerialiser.SaveContraption(NameToSave, states);
			ContraptionOutlineSerialiser.SaveOutline(NameToSave, GameObjectsToSave, NewOrigin);
			NotificationControllerBehaviour.Show("<b>" + NameToSave + "</b> saved succesfully");
		}
		catch (Exception exception)
		{
			NotificationControllerBehaviour.Show("<b>" + NameToSave + "</b> saved with errors");
			Debug.LogException(exception);
		}
		finally
		{
			lockName = false;
		}
		try
		{
			CatalogBehaviour.Main.CreateItemButtons();
		}
		catch (Exception ex)
		{
			DialogBoxManager.Notification("Something happened. Please show this message to the developer:\n" + ex.Message);
			Debug.LogException(ex);
		}
	}

	public void Focus()
	{
		StartCoroutine(FocusOnTextbox());
	}

	private IEnumerator FocusOnTextbox()
	{
		yield return new WaitForEndOfFrame();
		EventSystem.current.SetSelectedGameObject(TMP_InputField.gameObject);
		TMP_InputField.ActivateInputField();
		TMP_InputField.Select();
	}
}
