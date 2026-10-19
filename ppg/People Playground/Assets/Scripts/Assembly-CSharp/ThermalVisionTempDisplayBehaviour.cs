using System;
using TMPro;
using UnityEngine;

public class ThermalVisionTempDisplayBehaviour : MonoBehaviour
{
	public LayerMask ToRead;

	public TextMeshPro Text;

	private void Start()
	{
		SetText(PhysicalBehaviour.AmbientTemperature);
	}

	private void Update()
	{
		if (!(Global.main == null) && !Global.main.GetPausedMenu())
		{
			base.transform.position = Global.main.MousePosition;
			Collider2D collider2D = Physics2D.OverlapPoint(Global.main.MousePosition, ToRead);
			LavaBehaviour lava;
			if ((bool)collider2D && (bool)collider2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				float text = (value.SimulateTemperature ? value.Temperature : PhysicalBehaviour.AmbientTemperature);
				SetText(text);
			}
			else if (LavaBehaviour.TryGetLavaAtPoint(Global.main.MousePosition, out lava))
			{
				SetText(lava.LavaTemperature);
			}
			else if (UserPreferenceManager.Current.AmbientTemperatureTransfer)
			{
				float temperatureAtPoint = AmbientTemperatureGridBehaviour.Instance.GetTemperatureAtPoint(Global.main.MousePosition);
				SetText(temperatureAtPoint);
			}
			else
			{
				SetText(PhysicalBehaviour.AmbientTemperature);
			}
		}
	}

	private void SetText(float celsius)
	{
		float num = Utils.CelsiusToPreference(Mathf.Clamp(celsius, -273.15f, 99999f));
		string arg = ((celsius >= 99999f) ? "+" : string.Empty);
		string temperatureUnitSuffix = Utils.GetTemperatureUnitSuffix(UserPreferenceManager.Current.TemperatureUnit);
		Text.text = $"{Math.Round(num, 1)} {temperatureUnitSuffix} {arg}".Trim();
	}
}
