using NaughtyAttributes;
using UnityEngine;

public class BlasterboltBehaviour : MonoBehaviour
{
	public float Speed = 1f;

	public GameObject ImpactEffect;

	public GameObject HitDecal;

	public TrailRenderer Trail;

	public float ImpactStrength;

	public LayerMask layers;

	public float damage = 1f;

	public bool createHitDecal = true;

	public bool SpawnExplosionEffect;

	public bool CreateExplosion;

	public float DecalSizeMultiplier = 1f;

	public float TemperatureTarget = 900f;

	[ShowIf("CreateExplosion")]
	public float ExplosionDamage;

	public DecalDescriptor ImpactDecal;

	private bool closing;

	private void Update()
	{
		if (closing)
		{
			Trail.autodestruct = true;
			return;
		}
		float num = Speed * Time.deltaTime;
		Vector3 position = base.transform.position;
		Utils.LaserHit laserHit = Utils.MaterialAwareRaycast(dir: base.transform.right, origin: position, maxDistance: num, layers: layers);
		if (laserHit.hit.HasValue)
		{
			RaycastHit2D value = laserHit.hit.Value;
			PhysicalBehaviour physicalBehaviour = laserHit.physicalBehaviour;
			bool flag = Global.main.EnergyLayers.HasLayer(value.collider.gameObject.layer);
			if ((bool)physicalBehaviour && (physicalBehaviour.ReflectsLasers || flag))
			{
				physicalBehaviour.rigidbody.AddForceAtPosition(value.normal * (0f - ImpactStrength), value.point, ForceMode2D.Impulse);
				Vector2 vector = Vector2.Reflect(base.transform.right, value.normal);
				base.transform.position = value.point;
				base.transform.right = vector;
				if (physicalBehaviour.SimulateTemperature)
				{
					physicalBehaviour.Temperature += damage * Random.value * 0.02f;
				}
				if (!flag)
				{
					physicalBehaviour.BurnProgress += 0.002f * Random.value;
				}
			}
			else if ((bool)physicalBehaviour && physicalBehaviour.AbsorbsLasers)
			{
				base.transform.position = value.point;
				value.collider.attachedRigidbody.AddForceAtPosition(value.normal * (0f - ImpactStrength), value.point);
				physicalBehaviour.BurnProgress += 0.2f * Random.value;
				Impact(value);
			}
			else
			{
				Impact(value);
			}
		}
		else
		{
			base.transform.position += base.transform.right * num;
		}
	}

	private void Impact(RaycastHit2D hit)
	{
		Vector2 normal = hit.normal;
		float z = Mathf.Atan2(normal.y, normal.x);
		bool flag = WaterBehaviour.IsPointUnderWater(hit.point);
		if (flag)
		{
			ExplosionCreator.ExplosionParameters ex = default(ExplosionCreator.ExplosionParameters);
			ex.Position = hit.point;
			ExplosionCreator.CreateUnderwaterExplosionEffect(ex, doSplash: false);
		}
		else
		{
			Object.Instantiate(ImpactEffect, hit.point, Quaternion.Euler(0f, 0f, z)).transform.up = normal;
			if (createHitDecal)
			{
				Object.Instantiate(HitDecal, hit.point, Quaternion.identity, hit.transform);
			}
		}
		ExplosionCreator.CreatePulseExplosion(hit.point, ImpactStrength, 1f, !flag && SpawnExplosionEffect, breakObjects: false);
		if (CreateExplosion)
		{
			ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(hit.point), new ExplosionCreator.ExplosionParameters(8u, hit.point, 4f, ExplosionDamage, createFx: false, ExplosionCreator.EffectSize.Medium, 0.2f), doSplash: false);
		}
		if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(hit.transform, out var value) && value.SimulateTemperature)
		{
			value.Temperature = Mathf.Lerp(value.Temperature, TemperatureTarget, 0.015f / value.GetHeatCapacity());
		}
		if (hit.transform.CompareTag("Limb") && hit.transform.TryGetComponent<SkinMaterialHandler>(out var component))
		{
			component.AddDamagePoint(DamageType.Burn, hit.point, TemperatureTarget * 0.01f);
		}
		hit.transform.SendMessage("Shot", new Shot(normal, hit.point, damage), SendMessageOptions.DontRequireReceiver);
		if ((bool)ImpactDecal)
		{
			hit.transform.SendMessage("Decal", new DecalInstruction(ImpactDecal, hit.point, calculateDecalSize(damage)), SendMessageOptions.DontRequireReceiver);
		}
		closing = true;
		float calculateDecalSize(float dmg)
		{
			return Mathf.Max(Mathf.Pow(dmg, 3.35f) / Mathf.Pow(10f, 8f), 0.15f) * DecalSizeMultiplier;
		}
	}
}
