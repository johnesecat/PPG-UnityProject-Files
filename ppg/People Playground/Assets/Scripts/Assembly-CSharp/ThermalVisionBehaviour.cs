using UnityEngine;
using UnityEngine.Events;

public class ThermalVisionBehaviour : MonoBehaviour
{
	public bool ThermalVisionEnabled;

	public SpriteRenderer SpriteRenderer;

	public LayerMask ToRead;

	public float MinTemp = -273f;

	public float MaxTemp = 1000f;

	public const int ThermalVisionResolution = 64;

	public const int ComponentsPerEntry = 2;

	public UnityEvent OnThermalVisionToggle = new UnityEvent();

	private Texture2D tex;

	private Material materialInstance;

	private float t;

	private readonly byte[] data = new byte[8192];

	public static ThermalVisionBehaviour Instance { get; private set; }

	private void Awake()
	{
		if (!SystemInfo.SupportsTextureFormat(TextureFormat.RG16))
		{
			base.enabled = false;
			StopThermalVision();
			Debug.LogWarning("Thermal vision is not supported on this GPU");
			return;
		}
		materialInstance = Object.Instantiate(SpriteRenderer.sharedMaterial);
		tex = new Texture2D(64, 64, TextureFormat.RG16, mipChain: false);
		SetupTexture();
		tex.SetPixelData(data, 0);
		tex.Apply(updateMipmaps: false);
		SpriteRenderer.material = materialInstance;
		Instance = this;
		OnThermalVisionToggle.Invoke();
	}

	private void SetupTexture()
	{
		tex = new Texture2D(64, 64, TextureFormat.RG16, mipChain: false);
		tex.wrapMode = TextureWrapMode.Clamp;
		materialInstance.SetTexture(ShaderProperties.Get("_HeatData"), tex);
	}

	private void Update()
	{
		if (!DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock && InputSystem.Up("toggleThermalVision"))
		{
			ToggleThermalVision();
		}
		if (!ThermalVisionEnabled)
		{
			return;
		}
		if (!tex)
		{
			SetupTexture();
		}
		t += Time.unscaledDeltaTime;
		if (!(t > 1f / (float)UserPreferenceManager.Current.ThermalVisionUpdateRate))
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				float num2 = (float)i / 63f;
				float num3 = (float)j / 63f;
				Vector2 vector = base.transform.TransformPoint(num3 - 0.5f, num2 - 0.5f, 0f);
				if (LavaBehaviour.TryGetLavaAtPoint(vector, out var lava))
				{
					SetHeatAt(num, CelsiusToData(lava.LavaTemperature));
					SetAlphaAt(num, byte.MaxValue);
				}
				else
				{
					Collider2D collider2D = Physics2D.OverlapPoint(vector, ToRead);
					if ((bool)collider2D && (bool)collider2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
					{
						float celsius = (value.SimulateTemperature ? value.Temperature : PhysicalBehaviour.AmbientTemperature);
						SetHeatAt(num, CelsiusToData(celsius));
						SetAlphaAt(num, GetAlphaFor(celsius));
					}
					else if (UserPreferenceManager.Current.AmbientTemperatureTransfer)
					{
						float temperatureAtPoint = AmbientTemperatureGridBehaviour.Instance.GetTemperatureAtPoint(vector);
						SetHeatAt(num, CelsiusToData(temperatureAtPoint));
						SetAlphaAt(num, GetAlphaFor(temperatureAtPoint, 0.5f));
					}
					else
					{
						SetHeatAt(num, CelsiusToData(PhysicalBehaviour.AmbientTemperature));
						SetAlphaAt(num, 0);
					}
				}
				num++;
			}
		}
		tex.SetPixelData(data, 0);
		tex.Apply(updateMipmaps: false);
		t = 0f;
	}

	private static byte GetAlphaFor(float Celsius, float multiplier = 1f)
	{
		return (byte)(255f * multiplier * Mathf.Clamp01(Mathf.Abs(PhysicalBehaviour.AmbientTemperature - Celsius) / 5f));
	}

	private void SetHeatAt(int i, byte heat)
	{
		data[i * 2] = heat;
	}

	private void SetAlphaAt(int i, byte alpha)
	{
		data[i * 2 + 1] = alpha;
	}

	private byte CelsiusToData(float celsius)
	{
		return (byte)Mathf.RoundToInt(Utils.MapRange(MinTemp, MaxTemp, 0f, 255f, Mathf.Clamp(celsius, MinTemp, MaxTemp)));
	}

	public void ToggleThermalVision()
	{
		ThermalVisionEnabled = !ThermalVisionEnabled;
		if (ThermalVisionEnabled)
		{
			StartThermalVision();
		}
		else
		{
			StopThermalVision();
		}
	}

	public void StartThermalVision()
	{
		ThermalVisionEnabled = true;
		SpriteRenderer.enabled = true;
		OnThermalVisionToggle.Invoke();
	}

	public void StopThermalVision()
	{
		ThermalVisionEnabled = false;
		SpriteRenderer.enabled = false;
		OnThermalVisionToggle.Invoke();
	}

	private void OnDestroy()
	{
		Object.Destroy(tex);
		Object.Destroy(materialInstance);
	}
}
