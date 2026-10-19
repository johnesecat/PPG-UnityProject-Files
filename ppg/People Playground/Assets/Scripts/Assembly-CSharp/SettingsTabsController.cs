using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTabsController : MonoBehaviour
{
	public SettingCategory StartingCategory;

	public ScrollRect ScrollRect;

	public SettingTemplateBehaviour[] Settings;

	private void Start()
	{
		SetCategory(StartingCategory);
	}

	private void OnPageSelected()
	{
		StartCoroutine(OnNextFrame(delegate
		{
			SetCategory(StartingCategory);
		}));
	}

	public void SetCategoryGeneral()
	{
		SetCategory(SettingCategory.General);
	}

	public void SetCategoryGore()
	{
		SetCategory(SettingCategory.Gore);
	}

	public void SetCategoryVisualEffects()
	{
		SetCategory(SettingCategory.VisualEffects);
	}

	public void SetCategoryUserInterface()
	{
		SetCategory(SettingCategory.UserInterface);
	}

	public void SetCategoryVideo()
	{
		SetCategory(SettingCategory.Video);
	}

	public void SetCategoryAudio()
	{
		SetCategory(SettingCategory.Audio);
	}

	public void SetCategory(SettingCategory cat)
	{
		for (int i = 0; i < Settings.Length; i++)
		{
			SettingTemplateBehaviour settingTemplateBehaviour = Settings[i];
			if ((bool)settingTemplateBehaviour)
			{
				settingTemplateBehaviour.gameObject.SetActive(cat == settingTemplateBehaviour.Category);
			}
		}
		StartCoroutine(OnNextFrame(delegate
		{
			ScrollRect.verticalNormalizedPosition = 1f;
			base.transform.localPosition = default(Vector3);
		}));
	}

	private IEnumerator OnNextFrame(Action a)
	{
		yield return new WaitForEndOfFrame();
		a();
	}
}
