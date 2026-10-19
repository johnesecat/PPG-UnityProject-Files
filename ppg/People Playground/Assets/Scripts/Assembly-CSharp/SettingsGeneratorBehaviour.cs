using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGeneratorBehaviour : MonoBehaviour
{
	public Transform Container;

	public GameObject SettingTemplatePrefab;

	public SettingsTabsController SettingsTabsController;

	[Space]
	public GameObject ButtonSettingPrefab;

	public GameObject SliderSettingPrefab;

	private readonly Type boolType = typeof(bool);

	private readonly Type intType = typeof(int);

	private readonly Type floatType = typeof(float);

	private void Start()
	{
		Generate();
	}

	public void Generate()
	{
		FieldInfo[] fields = typeof(Preferences).GetFields(BindingFlags.Instance | BindingFlags.Public);
		Debug.Log(JsonConvert.SerializeObject(UserPreferenceManager.Current, Formatting.Indented));
		foreach (FieldInfo item in fields.OrderBy((FieldInfo o) => o.GetCustomAttribute<SortAttribute>()?.Order ?? int.MaxValue))
		{
			HideInSettingsMenuAttribute customAttribute = item.GetCustomAttribute<HideInSettingsMenuAttribute>();
			SettingAttribute customAttribute2 = item.GetCustomAttribute<SettingAttribute>();
			if (customAttribute != null || customAttribute2 == null)
			{
				continue;
			}
			if (item.FieldType.IsEnum)
			{
				GenerateEnumSetting(item, customAttribute2);
			}
			else if (item.FieldType == boolType)
			{
				GenerateBoolSetting(item, customAttribute2);
			}
			else
			{
				if (!(item.FieldType == intType) && !(item.FieldType == floatType))
				{
					continue;
				}
				OptionAttribute customAttribute3 = item.GetCustomAttribute<OptionAttribute>();
				if (customAttribute3 != null && item.FieldType == floatType)
				{
					SettingTemplateBehaviour setting = CreateNewSetting(customAttribute2);
					FormatAttribute customAttribute4 = item.GetCustomAttribute<FormatAttribute>();
					for (int i = 0; i < customAttribute3.Options.Length; i++)
					{
						float num = customAttribute3.Options[i];
						CreateRadioButton(item, setting, num, customAttribute4?.FormatString(num) ?? num.ToString());
					}
				}
				else
				{
					GenerateNumberSetting(item, customAttribute2);
				}
			}
		}
		SettingsTabsController.Settings = Container.GetComponentsInChildren<SettingTemplateBehaviour>();
		SettingsTabsController.SetCategory(SettingCategory.General);
	}

	private void GenerateNumberSetting(FieldInfo field, SettingAttribute settingAttr)
	{
		SettingTemplateBehaviour settingTemplateBehaviour = CreateNewSetting(settingAttr);
		bool isInt = field.FieldType == intType;
		FormatAttribute formatAttr = field.GetCustomAttribute<FormatAttribute>();
		RangeAttribute customAttribute = field.GetCustomAttribute<RangeAttribute>();
		StepAttribute stepAttr = field.GetCustomAttribute<StepAttribute>();
		GameObject gameObject = UnityEngine.Object.Instantiate(SliderSettingPrefab, settingTemplateBehaviour.ControlContainer);
		Slider slider = gameObject.GetComponentInChildren<Slider>();
		TextMeshProUGUI textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
		OnPointerUpEvent onPointerUpEvent = slider.gameObject.AddComponent<OnPointerUpEvent>();
		slider.minValue = customAttribute?.min ?? 0f;
		slider.maxValue = customAttribute?.max ?? 1f;
		slider.wholeNumbers = isInt;
		if (isInt)
		{
			slider.value = UserPreferenceManager.Current.GetByName<int>(field.Name);
			textMesh.text = formatAttr?.FormatString(Mathf.RoundToInt(slider.value)) ?? slider.value.ToString();
		}
		else
		{
			slider.value = UserPreferenceManager.Current.GetByName<float>(field.Name);
			textMesh.text = formatAttr?.FormatString(slider.value) ?? slider.value.ToString();
		}
		slider.onValueChanged.AddListener(delegate
		{
			float num = slider.value;
			if (stepAttr != null)
			{
				num = Utils.Snap(num, stepAttr.Step);
			}
			if (isInt)
			{
				num = Mathf.RoundToInt(num);
			}
			slider.value = num;
			if (isInt)
			{
				UserPreferenceManager.Current.SetByName(field.Name, Mathf.RoundToInt(num));
				textMesh.text = formatAttr?.FormatString(Mathf.RoundToInt(num)) ?? slider.value.ToString();
			}
			else
			{
				UserPreferenceManager.Current.SetByName(field.Name, num);
				textMesh.text = formatAttr?.FormatString(slider.value) ?? slider.value.ToString();
			}
			ApplySetting(updateVideo: false);
		});
		onPointerUpEvent.onPointerUp.AddListener(delegate
		{
			ApplySetting();
		});
	}

	private void GenerateEnumSetting(FieldInfo field, SettingAttribute settingAttr)
	{
		SettingTemplateBehaviour setting = CreateNewSetting(settingAttr);
		IList values = Enum.GetValues(field.FieldType);
		for (int i = 0; i < values.Count; i++)
		{
			object value = values[i];
			CreateRadioButton(field, setting, value);
		}
	}

	private void GenerateBoolSetting(FieldInfo field, SettingAttribute settingAttr)
	{
		SettingTemplateBehaviour setting = CreateNewSetting(settingAttr);
		CreateRadioButton(field, setting, true, "Enabled");
		CreateRadioButton(field, setting, false, "Disabled");
	}

	private SettingTemplateBehaviour CreateNewSetting(SettingAttribute settingAttr)
	{
		SettingTemplateBehaviour component = UnityEngine.Object.Instantiate(SettingTemplatePrefab, Container).GetComponent<SettingTemplateBehaviour>();
		component.name = settingAttr.Title;
		component.Category = settingAttr.Category;
		component.SetTitle(settingAttr.Title);
		component.SetDescription(settingAttr.Description);
		return component;
	}

	private void CreateRadioButton(FieldInfo field, SettingTemplateBehaviour setting, object value, string label = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(ButtonSettingPrefab, setting.ControlContainer);
		SettingButtonBehaviour i = gameObject.GetComponent<SettingButtonBehaviour>();
		i.Key = field.Name;
		i.Value = Convert.ChangeType(value, field.FieldType);
		i.Label.text = label ?? value.ToString();
		i.SetChecked();
		i.Button.onClick.AddListener(delegate
		{
			UserPreferenceManager.Current.SetByName(field.Name, i.Value);
			SettingButtonBehaviour[] componentsInChildren = setting.ControlContainer.GetComponentsInChildren<SettingButtonBehaviour>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SetChecked();
			}
			ApplySetting();
		});
	}

	private void ApplySetting(bool updateVideo = true)
	{
		GameSettingsInitBehaviour gameSettingsInitBehaviour = UnityEngine.Object.FindObjectOfType<GameSettingsInitBehaviour>();
		MixerControllerBehaviour mixerControllerBehaviour = UnityEngine.Object.FindObjectOfType<MixerControllerBehaviour>();
		VideoSettingsInitBehaviour videoSettingsInitBehaviour = UnityEngine.Object.FindObjectOfType<VideoSettingsInitBehaviour>();
		if ((bool)gameSettingsInitBehaviour)
		{
			gameSettingsInitBehaviour.Sync();
		}
		if ((bool)videoSettingsInitBehaviour && updateVideo)
		{
			videoSettingsInitBehaviour.Sync();
		}
		if ((bool)mixerControllerBehaviour)
		{
			mixerControllerBehaviour.Sync();
		}
		UserPreferenceManager.Save();
	}
}
