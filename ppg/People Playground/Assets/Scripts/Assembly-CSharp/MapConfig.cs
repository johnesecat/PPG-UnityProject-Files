using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MapConfig : MonoBehaviour
{
	private class OverlayState
	{
		public Material Material;

		public GameObject GameObject;

		public bool Target;

		public float Visiblity;

		public PostProcessVolume OptionalPostProcessing;

		public AudioSource OptionalAudioSource;

		public OverlayState(GameObject gameObject)
		{
			GameObject = gameObject;
			Material = gameObject.GetComponentInChildren<Renderer>().sharedMaterial;
			OptionalPostProcessing = gameObject.GetComponentInChildren<PostProcessVolume>(includeInactive: true);
			OptionalAudioSource = gameObject.GetComponentInChildren<AudioSource>(includeInactive: true);
		}
	}

	[Serializable]
	public struct AmbientSoundConfig
	{
		public float Volume;

		public float Chance;

		public AudioClip[] Clips;
	}

	public bool OverrideAmbientSounds;

	[ShowIf("OverrideAmbientSounds")]
	public AmbientSoundConfig Ambience;

	public EnvironmentalSettings Settings = new EnvironmentalSettings();

	[Space]
	public GameObject SnowPrefab;

	public GameObject RainPrefab;

	public GameObject FogPrefab;

	public GameObject LightningCreatorPrefab;

	[NonSerialized]
	internal BoundingBoxBehaviour Bounds;

	private GameObject SnowInstance;

	private GameObject RainInstance;

	private GameObject FogInstance;

	private GameObject LightningCreatorInstance;

	public Color FoliageColour = new Color(0.58f, 0.48f, 0.29f);

	public float FoliageSwayIntensity = 0.01f;

	public float FoliageSwaySpeed = 1f;

	private PostProcessVolume heatEffect;

	private PostProcessVolume coldEffect;

	public AudioClip RainLoop;

	private WeatherLightningBehaviour lightning;

	public static MapConfig Instance;

	private readonly IDictionary<GameObject, OverlayState> overlayTarget = new Dictionary<GameObject, OverlayState>(3);

	private void Awake()
	{
		Bounds = GetComponent<BoundingBoxBehaviour>();
	}

	public void Reinitialise()
	{
		Bounds = GetComponent<BoundingBoxBehaviour>();
		StopAllCoroutines();
		if ((bool)SnowPrefab)
		{
			SnowInstance = PrepareWeatherInstance(SnowPrefab);
			overlayTarget.Add(SnowInstance, new OverlayState(SnowInstance));
		}
		if ((bool)RainPrefab)
		{
			RainInstance = PrepareWeatherInstance(RainPrefab);
			RainInstance.GetComponent<AudioSource>().clip = RainLoop;
			overlayTarget.Add(RainInstance, new OverlayState(RainInstance));
		}
		if ((bool)FogPrefab)
		{
			FogInstance = PrepareWeatherInstance(FogPrefab);
			overlayTarget.Add(FogInstance, new OverlayState(FogInstance));
		}
		if ((bool)LightningCreatorPrefab)
		{
			PrepareLightningPrefab();
		}
		if ((bool)base.transform && (bool)base.transform.parent)
		{
			PlayAudioAtRandom component = base.transform.parent.GetComponent<PlayAudioAtRandom>();
			if ((bool)component)
			{
				component.PositionCenter = Bounds.BoundingBox.center;
				component.Radius = Mathf.Max(Bounds.BoundingBox.extents.x, Bounds.BoundingBox.extents.y) * 1.41421f;
				if (OverrideAmbientSounds)
				{
					component.ChancePerSecond = Ambience.Chance;
					component.Clips = Ambience.Clips;
					component.AudioSource.volume = Ambience.Volume;
				}
			}
		}
		heatEffect = GameObject.FindGameObjectWithTag("HeatEffect").GetComponent<PostProcessVolume>();
		coldEffect = GameObject.FindGameObjectWithTag("ColdEffect").GetComponent<PostProcessVolume>();
		if (Settings != null)
		{
			ApplySettings(Settings);
		}
		Instance = this;
	}

	private void Start()
	{
		Reinitialise();
	}

	private void PrepareLightningPrefab()
	{
		Bounds boundingBox = Bounds.BoundingBox;
		LightningCreatorInstance = UnityEngine.Object.Instantiate(LightningCreatorPrefab, base.transform);
		LightningCreatorInstance.transform.position = new Vector3(boundingBox.min.x, boundingBox.max.y, 0f);
		lightning = LightningCreatorInstance.GetComponent<WeatherLightningBehaviour>();
		lightning.Width = boundingBox.size.x;
		lightning.Chance = Settings.Lightning_chance;
	}

	private GameObject PrepareWeatherInstance(GameObject prefab)
	{
		Bounds boundingBox = Bounds.BoundingBox;
		GameObject obj = UnityEngine.Object.Instantiate(prefab, base.transform);
		obj.transform.position = new Vector3(boundingBox.center.x, boundingBox.center.y, 20f);
		obj.transform.localScale = new Vector3(boundingBox.size.x, boundingBox.size.y, 1f);
		obj.SetActive(value: false);
		return obj;
	}

	public void ApplySettings(EnvironmentalSettings settings)
	{
		if (!base.gameObject.activeInHierarchy || settings == null)
		{
			return;
		}
		SetGravity(settings);
		SetFloodLights(settings);
		if ((bool)SnowInstance)
		{
			overlayTarget[SnowInstance].Target = settings.Snow;
		}
		if ((bool)RainInstance)
		{
			overlayTarget[RainInstance].Target = settings.Rain;
		}
		if ((bool)FogInstance)
		{
			overlayTarget[FogInstance].Target = settings.Fog;
		}
		if ((bool)lightning)
		{
			lightning.Chance = settings.Lightning_chance;
		}
		PhysicalBehaviour.AmbientTemperature = settings.Ambient_temperature;
		float value = Utils.MapRange(23f, 1000f, 0f, 1f, PhysicalBehaviour.AmbientTemperature);
		heatEffect.weight = Mathf.Sqrt(Mathf.Clamp01(value)).NaNFallback();
		coldEffect.weight = Utils.MapRange(-30f, 10f, 1f, 0f, PhysicalBehaviour.AmbientTemperature);
		Settings = settings;
		if (WaterBehaviour.waters != null)
		{
			foreach (WaterBehaviour item in WaterBehaviour.waters)
			{
				item.UpdateTemperatureState();
			}
		}
		if ((bool)Global.main && (bool)Global.main.GlobalFoliageMaterial)
		{
			Global.main.GlobalParticleFoliageMaterial.SetColor(ShaderProperties.Get("_LeafTint"), FoliageColour);
			Global.main.GlobalFoliageMaterial.SetColor(ShaderProperties.Get("_LeafTint"), FoliageColour);
			Global.main.GlobalFoliageMaterial.SetFloat(ShaderProperties.Get("_SwayIntensity"), FoliageSwayIntensity);
			Global.main.GlobalFoliageMaterial.SetFloat(ShaderProperties.Get("_SwaySpeed"), FoliageSwaySpeed);
		}
	}

	private static void SetGravity(EnvironmentalSettings settings)
	{
		Physics2D.gravity = settings.Gravity * Vector2.up;
		Physics.gravity = Physics2D.gravity;
	}

	private void SetFloodLights(EnvironmentalSettings settings)
	{
		if (Settings.Floodlights == settings.Floodlights)
		{
			return;
		}
		MapLightBehaviour[] array = UnityEngine.Object.FindObjectsOfType<MapLightBehaviour>();
		foreach (MapLightBehaviour mapLightBehaviour in array)
		{
			if (mapLightBehaviour.gameObject.activeInHierarchy)
			{
				if (settings.Floodlights)
				{
					mapLightBehaviour.Activate();
				}
				else
				{
					mapLightBehaviour.Deactivate();
				}
			}
		}
	}

	private void Update()
	{
		foreach (KeyValuePair<GameObject, OverlayState> item in overlayTarget)
		{
			OverlayFadeStep(item.Value);
		}
	}

	private void OverlayFadeStep(OverlayState item)
	{
		item.Visiblity = Mathf.Lerp(item.Visiblity, item.Target ? 1 : 0, Utils.GetLerpFactorDeltaTime(0.7f, Time.deltaTime));
		setActive(item.GameObject, item.Visiblity > 0.01f);
		if (Mathf.Abs(item.Visiblity - (float)(item.Target ? 1 : 0)) > 0.005f)
		{
			item.Material.SetFloat(ShaderProperties.Get("_Density"), item.Visiblity);
			if ((bool)item.OptionalPostProcessing)
			{
				item.OptionalPostProcessing.weight = item.Visiblity * item.Visiblity;
			}
			if ((bool)item.OptionalAudioSource)
			{
				item.OptionalAudioSource.volume = item.Visiblity;
			}
		}
		void setActive(GameObject o, bool a)
		{
			if (o.activeSelf != a)
			{
				o.SetActive(a);
			}
		}
	}
}
