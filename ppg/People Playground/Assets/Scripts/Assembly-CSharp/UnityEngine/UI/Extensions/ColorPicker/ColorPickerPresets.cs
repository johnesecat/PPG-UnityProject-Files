using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerPresets : MonoBehaviour
	{
		public enum SaveType
		{
			None,
			PlayerPrefs,
			JsonFile
		}

		protected class JsonColor
		{
			public Color32[] colors;

			public void SetColors(Color[] colorsIn)
			{
				colors = new Color32[colorsIn.Length];
				for (int i = 0; i < colorsIn.Length; i++)
				{
					colors[i] = colorsIn[i];
				}
			}

			public Color[] GetColors()
			{
				Color[] array = new Color[colors.Length];
				for (int i = 0; i < colors.Length; i++)
				{
					array[i] = colors[i];
				}
				return array;
			}
		}

		public ColorPickerControl picker;

		[SerializeField]
		protected GameObject presetPrefab;

		[SerializeField]
		protected int maxPresets = 16;

		[SerializeField]
		protected Color[] predefinedPresets;

		protected List<Color> presets = new List<Color>();

		public Image createPresetImage;

		public Transform createButton;

		[SerializeField]
		public SaveType saveMode;

		[SerializeField]
		protected string playerPrefsKey;

		public virtual string JsonFilePath => Application.persistentDataPath + "/" + playerPrefsKey + ".json";

		protected virtual void Reset()
		{
			playerPrefsKey = "colorpicker_" + GetInstanceID();
		}

		protected virtual void Awake()
		{
			picker.onHSVChanged.AddListener(HSVChanged);
			picker.onValueChanged.AddListener(ColorChanged);
			picker.CurrentColor = Color.white;
			presetPrefab.SetActive(value: false);
			presets.AddRange(predefinedPresets);
			LoadPresets(saveMode);
		}

		public virtual void CreatePresetButton()
		{
			CreatePreset(picker.CurrentColor);
		}

		public virtual void LoadPresets(SaveType saveType)
		{
			string text = "";
			switch (saveType)
			{
			case SaveType.PlayerPrefs:
				if (PlayerPrefs.HasKey(playerPrefsKey))
				{
					text = PlayerPrefs.GetString(playerPrefsKey);
				}
				break;
			case SaveType.JsonFile:
				if (File.Exists(JsonFilePath))
				{
					text = File.ReadAllText(JsonFilePath);
				}
				break;
			default:
				throw new NotImplementedException(saveType.ToString());
			case SaveType.None:
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					JsonColor jsonColor = JsonUtility.FromJson<JsonColor>(text);
					presets.AddRange(jsonColor.GetColors());
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			foreach (Color preset in presets)
			{
				CreatePreset(preset, loading: true);
			}
		}

		public virtual void SavePresets(SaveType saveType)
		{
			if (presets == null || presets.Count <= 0)
			{
				Debug.LogError("presets cannot be null or empty: " + ((presets == null) ? "NULL" : "EMPTY"));
				return;
			}
			JsonColor jsonColor = new JsonColor();
			jsonColor.SetColors(presets.ToArray());
			string text = JsonUtility.ToJson(jsonColor);
			switch (saveType)
			{
			case SaveType.None:
				Debug.LogWarning("Called SavePresets with SaveType = None...");
				break;
			case SaveType.PlayerPrefs:
				PlayerPrefs.SetString(playerPrefsKey, text);
				break;
			case SaveType.JsonFile:
				File.WriteAllText(JsonFilePath, text);
				break;
			default:
				throw new NotImplementedException(saveType.ToString());
			}
		}

		public virtual void CreatePreset(Color color, bool loading)
		{
			createButton.gameObject.SetActive(presets.Count < maxPresets);
			GameObject obj = Object.Instantiate(presetPrefab, presetPrefab.transform.parent);
			obj.transform.SetAsLastSibling();
			obj.SetActive(value: true);
			obj.GetComponent<Image>().color = color;
			createPresetImage.color = Color.white;
			if (!loading)
			{
				presets.Add(color);
				SavePresets(saveMode);
			}
		}

		public virtual void CreatePreset(Color color)
		{
			CreatePreset(color, loading: false);
		}

		public virtual void PresetSelect(Image sender)
		{
			picker.CurrentColor = sender.color;
		}

		protected virtual void HSVChanged(float h, float s, float v)
		{
			createPresetImage.color = HSVUtil.ConvertHsvToRgb(h * 360f, s, v, 1f);
		}

		protected virtual void ColorChanged(Color color)
		{
			createPresetImage.color = color;
		}
	}
}
