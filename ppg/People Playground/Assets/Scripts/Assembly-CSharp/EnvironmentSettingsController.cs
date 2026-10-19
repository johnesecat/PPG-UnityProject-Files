using System.Reflection;
using TMPro;
using UnityEngine;

public class EnvironmentSettingsController : MonoBehaviour
{
	public static EnvironmentSettingsController Main;

	public GameObject CheckboxControl;

	public GameObject NumberControl;

	[Space]
	public TextMeshProUGUI TooltipTarget;

	public Transform Container;

	private MapConfig currentMapConfig;

	private void Awake()
	{
		Main = this;
	}

	public void Start()
	{
		base.gameObject.SetActive(value: false);
		currentMapConfig = Object.FindObjectOfType<MapConfig>();
		RegenerateControls();
	}

	private void Update()
	{
		if (!Global.main.GetPausedMenu() && !DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock && InputSystem.Up("toybox"))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void RegenerateControls()
	{
		foreach (Transform item in Container)
		{
			Object.Destroy(item.gameObject);
		}
		FieldInfo[] fields = typeof(EnvironmentalSettings).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(bool))
			{
				CreateToggle(fieldInfo);
			}
			else if (fieldInfo.FieldType == typeof(float))
			{
				CreateNumberInput(fieldInfo);
			}
		}
		UISoundBehaviour.Refresh();
	}

	private void CreateNumberInput(FieldInfo member)
	{
		GameObject ctrlInstance = Object.Instantiate(NumberControl, Container);
		EnvironmentControlBehaviour control = PrepareControl(member, ctrlInstance);
		RangeAttribute range = member.GetCustomAttribute<RangeAttribute>();
		TemperatureSettingAttribute temperatureAttr = member.GetCustomAttribute<TemperatureSettingAttribute>();
		control.NumberInput.contentType = TMP_InputField.ContentType.DecimalNumber;
		float realValue = (float)member.GetValue(currentMapConfig.Settings);
		if (temperatureAttr != null)
		{
			control.NumberInput.text = temperatureAttr.ToPreference(realValue).ToString();
		}
		else
		{
			control.NumberInput.text = realValue.ToString();
		}
		control.NumberInput.onSelect.AddListener(delegate
		{
			Global.main.AddUiBlocker();
		});
		control.NumberInput.onDeselect.AddListener(delegate
		{
			Global.main.RemoveUiBlocker();
		});
		control.NumberInput.onEndEdit.AddListener(delegate(string value)
		{
			if (float.TryParse(value, out var result))
			{
				if (temperatureAttr != null)
				{
					result = temperatureAttr.ToCelsius(result);
					if (range != null)
					{
						result = Mathf.Clamp(result, range.min, range.max);
					}
					SetValue(member, result);
					result = temperatureAttr.ToPreference(result);
				}
				else
				{
					if (range != null)
					{
						result = Mathf.Clamp(result, range.min, range.max);
					}
					SetValue(member, result);
				}
				control.NumberInput.text = result.ToString();
			}
			else
			{
				Debug.LogError("Environment value input cannot be parsed");
			}
		});
	}

	private void CreateToggle(FieldInfo member)
	{
		GameObject ctrlInstance = Object.Instantiate(CheckboxControl, Container);
		EnvironmentControlBehaviour environmentControlBehaviour = PrepareControl(member, ctrlInstance);
		environmentControlBehaviour.Toggle.isOn = (bool)member.GetValue(currentMapConfig.Settings);
		environmentControlBehaviour.Toggle.onValueChanged.AddListener(delegate(bool value)
		{
			SetValue(member, value);
		});
	}

	private EnvironmentControlBehaviour PrepareControl(FieldInfo member, GameObject ctrlInstance)
	{
		EnvironmentControlBehaviour component = ctrlInstance.GetComponent<EnvironmentControlBehaviour>();
		TooltipAttribute customAttribute = member.GetCustomAttribute<TooltipAttribute>();
		component.Label.text = member.Name.Replace("_", " ");
		component.Tooltip.Text = customAttribute?.tooltip ?? "";
		component.Tooltip.TooltipText = TooltipTarget;
		return component;
	}

	private void SetValue(FieldInfo member, object value)
	{
		EnvironmentalSettings environmentalSettings = currentMapConfig.Settings.ShallowClone();
		member.SetValue(environmentalSettings, value);
		currentMapConfig.ApplySettings(environmentalSettings);
	}
}
