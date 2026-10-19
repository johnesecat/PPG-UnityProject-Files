using UnityEngine;

public abstract class BaseBoltBehaviour : MonoBehaviour
{
	public float Speed;

	public LayerMask Layers;

	private RaycastHit2D hit;

	public abstract bool ShouldReflect(RaycastHit2D hit);

	protected abstract void OnHit(RaycastHit2D hit);

	public virtual float GetSpeedMultiplier()
	{
		return 1f;
	}

	protected virtual void Update()
	{
		float num = Speed * Time.deltaTime * GetSpeedMultiplier();
		if (DoHitCheck(num))
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.transform.position += base.transform.right * num;
		}
	}

	protected bool DoHitCheck(float distance)
	{
		Vector3 right = base.transform.right;
		hit = Physics2D.Raycast(base.transform.position, right, distance, (int)Layers | (int)Global.main.EnergyLayers);
		if (!hit.transform)
		{
			return false;
		}
		if (float.IsNaN(hit.point.x) || float.IsNaN(hit.point.x))
		{
			return false;
		}
		Vector2 normal = hit.normal;
		if (ShouldReflect(hit))
		{
			base.transform.right = Vector3.Reflect(right, normal);
			base.transform.position = hit.point;
			return false;
		}
		OnHit(hit);
		base.transform.position = hit.point;
		return true;
	}
}
