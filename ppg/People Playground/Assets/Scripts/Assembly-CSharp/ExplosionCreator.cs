using System;
using UnityEngine;

public static class ExplosionCreator
{
	public enum EffectSize
	{
		Medium,
		Small,
		Large
	}

	[Serializable]
	public struct ExplosionParameters
	{
		public uint FragmentationRayCount;

		public Vector3 Position;

		public float Range;

		public float FragmentForce;

		public bool CreateParticlesAndSound;

		public EffectSize EffectSize;

		public float DismemberChance;

		public int BallisticShrapnelCount;

		[Obsolete]
		public bool LargeExplosionParticles
		{
			get
			{
				return EffectSize == EffectSize.Large;
			}
			set
			{
				EffectSize = (value ? EffectSize.Large : EffectSize.Medium);
			}
		}

		[Obsolete]
		public ExplosionParameters(uint rayCount, Vector3 pos, float range, float force, bool createFx, bool big, float dismember = 0f, int shrapnelCount = 0)
		{
			FragmentationRayCount = rayCount;
			Position = pos;
			Range = range;
			FragmentForce = force;
			CreateParticlesAndSound = createFx;
			EffectSize = (big ? EffectSize.Large : EffectSize.Medium);
			DismemberChance = dismember;
			BallisticShrapnelCount = shrapnelCount;
		}

		public ExplosionParameters(uint rayCount, Vector3 pos, float range, float force, bool createFx, EffectSize effectSize = EffectSize.Medium, float dismember = 0f, int shrapnelCount = 0)
		{
			FragmentationRayCount = rayCount;
			Position = pos;
			Range = range;
			FragmentForce = force;
			CreateParticlesAndSound = createFx;
			EffectSize = effectSize;
			DismemberChance = dismember;
			BallisticShrapnelCount = shrapnelCount;
		}
	}

	private static readonly Collider2D[] hitBuffer = new Collider2D[64];

	public static void Explode(ExplosionParameters ex)
	{
		CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(ex.Position), ex);
	}

	public static void Explode(Vector3 center, float force)
	{
		ExplosionParameters ex = default(ExplosionParameters);
		ex.Position = center;
		ex.DismemberChance = 0.025f;
		ex.Range = force * 2.5f;
		ex.LargeExplosionParticles = force > 30f;
		ex.CreateParticlesAndSound = true;
		ex.FragmentationRayCount = 24u;
		ex.FragmentForce = force;
		Explode(ex);
	}

	public static GameObject GetEffectPrefabForSize(EffectSize s)
	{
		switch (s)
		{
		case EffectSize.Small:
			return Resources.Load<GameObject>("Prefabs/SmallExplosion");
		case EffectSize.Large:
			return Resources.Load<GameObject>("Prefabs/BigExplosion");
		case EffectSize.Medium:
			return Resources.Load<GameObject>("Prefabs/Explosion");
		default:
			return Resources.Load<GameObject>("Prefabs/Explosion");
		}
	}

	public static void CreateFragmentationExplosion(ExplosionParameters ex)
	{
		CameraShakeBehaviour.main.Shake(ex.FragmentForce, ex.Position);
		if (ex.CreateParticlesAndSound)
		{
			UnityEngine.Object.Instantiate(GetEffectPrefabForSize(ex.EffectSize), ex.Position, Quaternion.identity);
		}
		for (int i = 0; i < ex.FragmentationRayCount; i++)
		{
			FragmentationRay(i, ex.FragmentationRayCount, ex.Position, ex.Range, ex.FragmentForce, ex.DismemberChance);
		}
		for (int j = 0; j < Physics2D.OverlapCircleNonAlloc(ex.Position, Mathf.Max(ex.Range / 10f, 1f), hitBuffer); j++)
		{
			hitBuffer[j].SendMessage("Decal", new DecalInstruction(Global.main.BlastMarkDecal, hitBuffer[j].ClosestPoint(ex.Position), 2f), SendMessageOptions.DontRequireReceiver);
		}
		Vector3 position = ex.Position;
		int ballisticShrapnelCount = ex.BallisticShrapnelCount;
		if (ballisticShrapnelCount > 0 && (bool)GlobalShrapnelEmitter.Instance)
		{
			GlobalShrapnelEmitter.Instance.EmitShrapnel(position, ballisticShrapnelCount);
		}
	}

	[Obsolete]
	public static void CreateFragmentationExplosion(uint fragmentationRayCount, Vector3 position, float range, float fragmentForce, bool particleAndSound, bool big = false, float dismemberChance = 0f)
	{
		ExplosionParameters ex = default(ExplosionParameters);
		ex.FragmentationRayCount = fragmentationRayCount;
		ex.Position = position;
		ex.DismemberChance = dismemberChance;
		ex.FragmentForce = fragmentForce;
		ex.LargeExplosionParticles = big;
		ex.CreateParticlesAndSound = particleAndSound;
		ex.Range = range;
		CreateFragmentationExplosion(ex);
	}

	public static void CreatePulseExplosion(Vector3 position, float force, float range, bool soundAndEffects, bool breakObjects = true)
	{
		CameraShakeBehaviour.main.Shake(force * range * 2f, position);
		if (soundAndEffects)
		{
			UnityEngine.Object.Instantiate(GetEffectPrefabForSize(EffectSize.Medium), position, Quaternion.identity);
		}
		int num = Physics2D.OverlapCircleNonAlloc(position, range, hitBuffer, LayerMask.GetMask("Objects", "CollidingDebris", "Debris"));
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = hitBuffer[i];
			if (!collider2D || !collider2D.attachedRigidbody)
			{
				continue;
			}
			Rigidbody2D attachedRigidbody = collider2D.attachedRigidbody;
			Vector3 vector = collider2D.transform.position - position;
			float sqrMagnitude = vector.sqrMagnitude;
			if (!(sqrMagnitude < float.Epsilon))
			{
				float num2 = Mathf.Sqrt(sqrMagnitude);
				Vector3 vector2 = vector / num2;
				float num3 = force / Mathf.Max(1f, sqrMagnitude / (range * range)) * 3f;
				float num4 = Mathf.Min(attachedRigidbody.mass, 1f);
				attachedRigidbody.AddForce(num3 * vector2 * num4, ForceMode2D.Impulse);
				if (breakObjects && (float)UnityEngine.Random.Range(0, 10) > force)
				{
					collider2D.BroadcastMessage("Break", (Vector2)(-1f * num3 * vector2), SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static void CreateUnderwaterExplosionEffect(ExplosionParameters ex, bool doSplash = true)
	{
		WaterBehaviour waterAtPoint = WaterBehaviour.GetWaterAtPoint(ex.Position);
		UnderwaterExplosionBehaviour component = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/UnderwaterExplosion"), ex.Position, Quaternion.identity).GetComponent<UnderwaterExplosionBehaviour>();
		component.GivenTop = (waterAtPoint ? waterAtPoint.GetGlobalSurfaceLevel() : ex.Position.y);
		component.DoSplash = doSplash;
		component.MaxDistanceToSurface = Mathf.Clamp(ex.Range, 7f, 25f);
		component.transform.localScale = Vector3.one * Mathf.Lerp(ex.Range / 10f, 1f, 0.8f);
	}

	[Obsolete]
	public static void CreateUnderwaterExplosionEffect(float waterSurfaceLevel, ExplosionParameters ex, bool doSplash = true)
	{
		CreateUnderwaterExplosionEffect(ex, doSplash);
	}

	[Obsolete]
	public static void CreateExplosionWithWater(bool isUnderWater, float waterSurfaceLevel, ExplosionParameters ex, bool doSplash = true)
	{
		CreateExplosionWithWater(isUnderWater, ex, doSplash);
	}

	public static void CreateExplosionWithWater(bool isUnderWater, ExplosionParameters ex, bool doSplash = true)
	{
		if (isUnderWater)
		{
			CreateUnderwaterExplosionEffect(ex, doSplash);
			ex.CreateParticlesAndSound = false;
		}
		CreateFragmentationExplosion(ex);
	}

	private static void FragmentationRay(int index, uint rayCount, Vector3 position, float range, float fragmentForce, float dismemberChance)
	{
		float f = 360f * ((float)index / (float)rayCount) * ((float)Math.PI / 180f);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(position, new Vector2(Mathf.Cos(f), Mathf.Sin(f)), range);
		if (!raycastHit2D)
		{
			return;
		}
		raycastHit2D.transform.BroadcastMessage("OnFragmentHit", fragmentForce, SendMessageOptions.DontRequireReceiver);
		if ((bool)raycastHit2D.rigidbody)
		{
			raycastHit2D.rigidbody.AddForceAtPosition((raycastHit2D.transform.position - position).normalized * fragmentForce, raycastHit2D.point, ForceMode2D.Impulse);
		}
		if (!raycastHit2D.transform.CompareTag("Limb"))
		{
			return;
		}
		LimbBehaviour limb = raycastHit2D.transform.GetComponent<LimbBehaviour>();
		limb.SkinMaterialHandler.AddDamagePoint((UnityEngine.Random.value > 0.5f) ? DamageType.Bullet : DamageType.Blunt, raycastHit2D.point, fragmentForce * (float)UnityEngine.Random.Range(3, 8));
		limb.Damage(fragmentForce * 1.5f);
		if (!(limb.SpeciesIdentity != "Android") || !(UnityEngine.Random.value < dismemberChance))
		{
			return;
		}
		if (UserPreferenceManager.Current.ProceduralFragments && raycastHit2D.distance < 3f && (double)UnityEngine.Random.value < 0.25)
		{
			limb.StartCoroutine(Utils.DelayCoroutine(0.01f, delegate
			{
				limb.Crush();
			}));
		}
		else
		{
			limb.Slice();
		}
	}
}
