using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Cartridge")]
public class Cartridge : ScriptableObject
{
	public float Damage = 1f;

	public float Recoil = 1f;

	public float ImpactForce = 1f;

	public float StartSpeed = 100f;

	public float PenetrationRandomAngleMultiplier = 0.1f;

	public Material Casing;
}
