using UnityEngine;

public class ClearButtonBehaviour : MonoBehaviour
{
	private const string NoClearTag = "NoClear";

	private const string PoolableTag = "Poolable";

	public LayerMask ToRemove;

	public void ClearEverything()
	{
		EnvironmentalSettings settings = null;
		if ((bool)MapConfig.Instance)
		{
			settings = MapConfig.Instance.Settings.ShallowClone();
			MapLightBehaviour.StartEnabled = settings.Floodlights;
			PhysicalBehaviour.AmbientTemperature = settings.Ambient_temperature;
		}
		ObjectPoolBehaviour[] array = Object.FindObjectsOfType<ObjectPoolBehaviour>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		GameObject[] array2 = Object.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in array2)
		{
			if (ToRemove.HasLayer(gameObject.layer))
			{
				Object.Destroy(gameObject);
			}
		}
		DecalControllerBehaviour[] array3 = Object.FindObjectsOfType<DecalControllerBehaviour>();
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].Clear();
		}
		UndoControllerBehaviour.ClearHistory();
		if ((bool)AmbientTemperatureGridBehaviour.Instance)
		{
			AmbientTemperatureGridBehaviour.Instance.World.Clear();
		}
		Object.FindObjectOfType<MapLoaderBehaviour>().Load();
		StartCoroutine(Utils.NextFrameCoroutine(delegate
		{
			if ((bool)MapConfig.Instance)
			{
				settings.CopyTo(MapConfig.Instance.Settings);
				MapConfig.Instance.ApplySettings(MapConfig.Instance.Settings);
			}
			if ((bool)EnvironmentSettingsController.Main)
			{
				EnvironmentSettingsController.Main.Start();
			}
			if ((bool)MapConfig.Instance)
			{
				MapLightBehaviour[] array4 = Object.FindObjectsOfType<MapLightBehaviour>();
				foreach (MapLightBehaviour mapLightBehaviour in array4)
				{
					if (mapLightBehaviour.enabled)
					{
						if (MapConfig.Instance.Settings.Floodlights)
						{
							mapLightBehaviour.ActivateInstantly();
						}
						else
						{
							mapLightBehaviour.DeactivateInstantly();
						}
					}
				}
			}
		}));
		NotificationControllerBehaviour.Show("Cleared room");
	}
}
