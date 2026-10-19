using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InfraredThermometer : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	[SkipSerialisation]
	public TextMeshPro ValueText;

	[SkipSerialisation]
	public TextMeshPro UnitText;

	[SkipSerialisation]
	public Transform RayPoint;

	[SkipSerialisation]
	public LayerMask LayersToHit;

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	public const float RangeInMeters = 7f;

	public const float RangeInUnits = 8.495455f;

	public const int MinTemp = -273;

	public const int MaxTemp = 9999;

	private Utils.LaserHit finalHit;

	private Vector3[] vertices;

	private static List<Utils.LaserHit> hits = new List<Utils.LaserHit>();

	private void Update()
	{
		if (Activated)
		{
			float num = ReadTemperatureFromRay(RayPoint.position, RayPoint.right * ((!(base.transform.localScale.x < 0f)) ? 1 : (-1)));
			ValueText.text = (float.IsNaN(num) ? string.Empty : Mathf.RoundToInt(Utils.CelsiusToPreference(Mathf.Clamp(num, -273f, 9999f))).ToString());
			UnitText.text = Utils.GetTemperatureUnitSuffix(UserPreferenceManager.Current.TemperatureUnit);
		}
	}

	private float ReadTemperatureFromRay(Vector2 origin, Vector2 direction)
	{
		hits.Clear();
		Utils.GetLaserEndPoint(origin, direction, ref hits, LayersToHit, 8.495455f, 8u);
		if (hits.Count != 0)
		{
			vertices = new Vector3[hits.Count + 1];
			vertices[0] = origin;
			LineRenderer.positionCount = hits.Count + 1;
			for (int i = 0; i < hits.Count; i++)
			{
				vertices[i + 1] = hits[i].point;
			}
			finalHit = hits.Last();
			LineRenderer.SetPositions(vertices);
			if ((bool)finalHit.physicalBehaviour)
			{
				if (!finalHit.physicalBehaviour.SimulateTemperature)
				{
					return PhysicalBehaviour.AmbientTemperature;
				}
				return finalHit.physicalBehaviour.Temperature;
			}
			return float.NaN;
		}
		LineRenderer.positionCount = 2;
		vertices = new Vector3[2];
		vertices[0] = origin;
		vertices[1] = origin + direction * 8.495455f;
		LineRenderer.SetPositions(vertices);
		return float.NaN;
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivation();
	}

	public void UpdateActivation()
	{
		ValueText.enabled = Activated;
		LineRenderer.enabled = Activated;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}
}
