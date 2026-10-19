using UnityEngine;

public class NewAtomBombExplosionBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public CircleCollider2D ShockwaveCollider;

	[SkipSerialisation]
	public SpriteRenderer ShockwaveSprite;

	[SkipSerialisation]
	public AnimationCurve ShockwaveGrowth;

	[SkipSerialisation]
	public PointEffector2D PointEffector;

	[Space]
	[SkipSerialisation]
	public float MaxForceMagnitude = 25000f;

	[SkipSerialisation]
	public float MinForceMagnitude;

	[Space]
	[SkipSerialisation]
	public float MinColliderSize = 10f;

	[SkipSerialisation]
	public float MaxColliderSize = 910.2273f;

	[Space]
	[SkipSerialisation]
	public float SpeedMultiplier = 1f;

	[SkipSerialisation]
	public float ShakeIntensity = 15f;

	[SkipSerialisation]
	public float VaporisationDistance;

	[SkipSerialisation]
	public float TemperatureMultiplier = 1f;

	[SkipSerialisation]
	public float EmpChance;

	private float time;

	private Camera mainCam;

	private static readonly RaycastHit2D[] buffer = new RaycastHit2D[8];

	private void Awake()
	{
		mainCam = Camera.main;
	}

	private void Start()
	{
		float num = VaporisationDistance * VaporisationDistance;
		Vector3 position = base.transform.position;
		for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[i];
			if (!physicalBehaviour)
			{
				continue;
			}
			float sqrMagnitude = (physicalBehaviour.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				physicalBehaviour.Temperature = 10000f;
				if (Random.value < physicalBehaviour.Properties.Softness && (bool)physicalBehaviour.transform.root && (bool)physicalBehaviour.transform.root.gameObject && physicalBehaviour.Deletable && physicalBehaviour.Disintegratable && !physicalBehaviour.isDisintegrated)
				{
					Object.Destroy(physicalBehaviour.transform.root.gameObject);
					continue;
				}
			}
			int num2 = Physics2D.LinecastNonAlloc(position, physicalBehaviour.transform.position, buffer);
			bool flag = true;
			for (int j = 0; j < num2; j++)
			{
				Transform transform = buffer[j].transform;
				if (transform != physicalBehaviour.transform && transform != base.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(transform, out var value) && value.AbsorbsLasers)
				{
					flag = false;
					break;
				}
			}
			if (Random.value < EmpChance)
			{
				physicalBehaviour.SendMessage("OnEMPHit", SendMessageOptions.DontRequireReceiver);
			}
			if (flag)
			{
				float num3 = Mathf.Max(float.Epsilon, sqrMagnitude) / 281250f;
				physicalBehaviour.Temperature = Mathf.Max(500f, Mathf.Max(physicalBehaviour.Temperature, Mathf.Min(10000f * TemperatureMultiplier, 100f / num3 * TemperatureMultiplier)));
			}
		}
	}

	private void Update()
	{
		float t = ShockwaveGrowth.Evaluate(SpeedMultiplier * time);
		ExpandShockwave(t);
		ShakeScreen(t);
		time += Time.deltaTime;
	}

	private void ExpandShockwave(float t)
	{
		ShockwaveCollider.radius = Mathf.Lerp(MinColliderSize, MaxColliderSize, t);
		PointEffector.forceMagnitude = Mathf.Lerp(MaxForceMagnitude, MinForceMagnitude, t);
		ShockwaveSprite.transform.localScale = 2f * ShockwaveCollider.radius * Vector3.one;
		ShockwaveSprite.color = new Color(ShockwaveSprite.color.r, ShockwaveSprite.color.g, ShockwaveSprite.color.b, 1f - t);
	}

	private void ShakeScreen(float t)
	{
		float num = ShockwaveCollider.radius * ShockwaveCollider.radius;
		float num2 = Vector2.SqrMagnitude(mainCam.transform.position - base.transform.position);
		float num3 = Mathf.Abs(num - num2);
		float num4 = ShakeIntensity * (1f - t);
		CameraShakeBehaviour.main.Shake(Mathf.Max(0f, 40000f - num3) * num4, base.transform.position, 0f);
		CameraShakeBehaviour.main.Shake(num4, base.transform.position);
	}
}
