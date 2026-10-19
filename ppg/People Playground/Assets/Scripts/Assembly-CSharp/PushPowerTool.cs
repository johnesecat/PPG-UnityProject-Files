using UnityEngine;

public class PushPowerTool : AOEPowerTool
{
	protected override void HandleObject(PhysicalBehaviour phys)
	{
		Vector2 vector = Global.main.MousePosition - phys.transform.position;
		float num = Mathf.Max(0.25f, vector.magnitude);
		if (num < DampeningRadius)
		{
			phys.rigidbody.velocity *= 0.95f;
			phys.rigidbody.angularVelocity *= 0.95f;
		}
		Vector2 vector2 = -vector / num;
		phys.rigidbody.AddForce((GetFalloff(Force, vector.sqrMagnitude, MaxForce) * vector2 + GetFalloff(1f, vector.sqrMagnitude, 1f) * mouseMovement) * phys.rigidbody.mass, ForceMode2D.Force);
	}

	protected override GameObject CreateEffectObject()
	{
		return Object.Instantiate(Resources.Load<GameObject>("Prefabs/Push tool effect"));
	}
}
