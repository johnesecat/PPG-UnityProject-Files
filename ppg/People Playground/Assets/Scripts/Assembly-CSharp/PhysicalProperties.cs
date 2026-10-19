using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Physical Properties")]
public class PhysicalProperties : ScriptableObject
{
	public PhysicsMaterial2D PhysicMaterial;

	[Space]
	public bool Sharp;

	public float SharpForceThresholdMultiplier = 1f;

	public float LodgeStrengthMultiplier = 1f;

	public SharpAxis[] SharpAxes;

	[Space]
	[Range(0f, 1f)]
	public float Softness;

	public float BulletSpeedAbsorptionPower;

	[Range(0f, 1f)]
	public float Brittleness;

	public float Buoyancy;

	[Range(0f, 1f)]
	public float Flammability;

	public float MagneticAttractionIntensity;

	public bool Conducting;

	[Range(0f, 1f)]
	public float Burnrate;

	public float BurningTemperatureThreshold = 5000f;

	[Range(0f, 1f)]
	public float HeatTransferSpeedMultiplier = 1f;

	public AudioClip[] SizzleSounds;

	[Space]
	public AudioClip[] SoftImpact;

	public AudioClip[] HardImpact;

	public float ImpactIntensityMutliplier = 1f;

	public float HitVolumeMultiplier = 1f;

	[Space]
	public AudioClip SlidingLoop;

	[Space]
	public AudioClip[] StabSound;

	[Space]
	public GameObject ShotImpact;

	public bool IsPoolable;

	public bool SparksOnSlide;

	[Space]
	public bool IgnoreStabResistance;
}
