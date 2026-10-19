using System.Linq;
using UnityEngine;

public class NeedleBehaviour : MonoBehaviour
{
	public float Speed = 10f;

	public Transform target;

	public GameObject staticNeedle;

	private bool hit;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position += base.transform.right * Time.smoothDeltaTime * Speed;
	}

	private void FixedUpdate()
	{
		foreach (Collider2D item in from c in Physics2D.OverlapCircleAll(base.transform.position + base.transform.right * 7.5f, 15f)
			orderby (c.transform.position - base.transform.position).sqrMagnitude descending
			select c)
		{
			AliveBehaviour component = item.transform.root.GetComponent<AliveBehaviour>();
			if ((bool)component && component.IsAlive() && Physics2D.Raycast(base.transform.position, item.transform.position - base.transform.position, 15f).collider == item)
			{
				target = item.transform;
			}
		}
		if ((bool)target)
		{
			base.transform.right = Vector2.Lerp(base.transform.right, (target.position - base.transform.position).normalized, 0.2f);
			if (Vector2.Distance(target.position, base.transform.position) < 15f)
			{
				target = null;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!hit && !collision.isTrigger)
		{
			Rigidbody2D attachedRigidbody = collision.attachedRigidbody;
			LimbBehaviour component = collision.GetComponent<LimbBehaviour>();
			Vector3 position = base.transform.position;
			if ((bool)attachedRigidbody)
			{
				attachedRigidbody.AddForceAtPosition(base.transform.right * 0.05f, position, ForceMode2D.Impulse);
			}
			float z = Mathf.Atan2(base.transform.right.y, base.transform.right.x) * 57.29578f;
			GameObject obj = Object.Instantiate(staticNeedle, collision.ClosestPoint(base.transform.position), Quaternion.Euler(0f, 0f, z));
			obj.transform.SetParent(collision.transform, worldPositionStays: true);
			obj.GetComponent<StaticNeedleBehaviour>().limb = component;
			Object.Destroy(base.gameObject);
			hit = true;
		}
	}
}
