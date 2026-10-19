using UnityEngine;

public class IonBoltBehaviour : MonoBehaviour
{
	public float Speed;

	public DecalDescriptor ImpactDecal;

	public GameObject ImpactEffect;

	public GameObject EndOfLifeEffect;

	public GameObject Particles;

	public float ImpactStrength;

	public LayerMask Layers;

	public float Damage = 1f;

	public float DecalSize = 0.5f;

	public float AoERadius = 1f;

	public DeleteAfterTime DeleteAfterTime;

	private static readonly Collider2D[] buffer = new Collider2D[16];

	private RaycastHit2D hit;

	private PhysicalBehaviour hitPhys;

	private void Awake()
	{
		DeleteAfterTime.OnEndOfLife.AddListener(delegate
		{
			Object.Instantiate(EndOfLifeEffect, base.transform.position, Quaternion.identity);
			Particles.transform.SetParent(null);
			Particles.AddComponent<DeleteAfterTime>().Life = 1f;
		});
	}

	private void Start()
	{
		Object.Instantiate(ImpactEffect, base.transform.position, Quaternion.identity).transform.localScale = Vector3.one * 0.3f;
	}

	private void Update()
	{
		float num = Speed * Time.deltaTime;
		if (!DoHitCheck(num))
		{
			base.transform.position += base.transform.right * num;
		}
	}

	private bool DoHitCheck(float distance)
	{
		Vector3 right = base.transform.right;
		hit = Physics2D.Raycast(base.transform.position, right, distance, Layers);
		if (!hit.transform)
		{
			return false;
		}
		Vector2 normal = hit.normal;
		Vector2 vector = Vector2.Reflect(right, normal);
		float z = Mathf.Atan2(normal.y, normal.x);
		Object.Instantiate(ImpactEffect, hit.point, Quaternion.Euler(0f, 0f, z)).transform.up = normal;
		ExplosionCreator.CreatePulseExplosion(hit.point, ImpactStrength, 1f, soundAndEffects: false);
		int num = Physics2D.OverlapCircleNonAlloc(hit.point, AoERadius, buffer, Layers);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out hitPhys))
			{
				if ((double)Random.value > 0.5)
				{
					hitPhys.SendMessage("Break", -normal * ImpactStrength, SendMessageOptions.DontRequireReceiver);
				}
				hitPhys.rigidbody.AddForceAtPosition(-1f * ImpactStrength * hit.normal, hit.point, ForceMode2D.Impulse);
				hitPhys.BurnProgress += 0.5f * Random.value;
			}
		}
		if ((bool)ImpactDecal)
		{
			hit.transform.SendMessage("Decal", new DecalInstruction(ImpactDecal, hit.point, Color.green, DecalSize), SendMessageOptions.DontRequireReceiver);
		}
		base.transform.right = vector;
		DeleteAfterTime.Life *= 0.9f;
		base.transform.position = hit.point;
		return true;
	}
}
