using System;
using UnityEngine;

[Serializable]
public class EnvironmentalSettings
{
	[Tooltip("Should the floodlights be turned on?")]
	public bool Floodlights = true;

	[Tooltip("Toggle cosmetic rain")]
	public bool Rain;

	[Tooltip("Toggle cosmetic snow")]
	public bool Snow;

	[Tooltip("Toggle cosmetic fog")]
	public bool Fog;

	[Range(-200f, 200f)]
	[Tooltip("Change the gravity")]
	public float Gravity = -9.81f;

	[Range(0f, 100f)]
	[Tooltip("Change the chance the artificial sky generates a lightning bolt")]
	public float Lightning_chance;

	[Range(-100f, 9000f)]
	[Tooltip("Change the ambient temperature")]
	[TemperatureSetting]
	public float Ambient_temperature = 18f;

	public void CopyTo(EnvironmentalSettings other)
	{
		other.Floodlights = Floodlights;
		other.Rain = Rain;
		other.Snow = Snow;
		other.Fog = Fog;
		other.Gravity = Gravity;
		other.Lightning_chance = Lightning_chance;
		other.Ambient_temperature = Ambient_temperature;
	}
}
