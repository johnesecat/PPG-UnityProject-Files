using UnityEngine;

public class EMPBehaviour : MonoBehaviour, Messages.IUse
{
	[Range(0f, 1f)]
	public float DamageChance = 0.25f;

	public float Range;

	[SkipSerialisation]
	public LayerMask ObjectLayers;

	[SkipSerialisation]
	public GameObject Effect;

	private GameObject Killzone;

	private void Awake()
	{
		Killzone = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Killzone"), base.transform.position, Quaternion.identity, base.transform);
		Killzone.GetComponent<IgnoreParentSize>().DesiredSize = 2f * Range * Vector2.one;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activate();
		}
	}

	public void Activate()
	{
		Object.Instantiate(Effect, base.transform.position, Quaternion.identity);
		Vector3 position = base.transform.position;
		float num = Range * Range;
		foreach (PhysicalBehaviour item in Global.main.PhysicalObjectsInWorld)
		{
			if (!(Random.value < DamageChance) && (position - item.transform.position).sqrMagnitude < num)
			{
				item.SendMessage("OnEMPHit", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void SetRange(float f)
	{
		Range = f;
		Killzone.GetComponent<IgnoreParentSize>().DesiredSize = 2f * Range * Vector2.one;
		Killzone.transform.localScale = Killzone.GetComponent<IgnoreParentSize>().DesiredSize;
	}
}
