using UnityEngine;

public class LiftPowerTool : AOEPowerTool
{
	protected override void HandleObject(PhysicalBehaviour phys)
	{
		Vector2 vector = Global.main.MousePosition - phys.transform.position;
		phys.rigidbody.AddForce((GetFalloff(Force, vector.sqrMagnitude, MaxForce) * Vector2.up + GetFalloff(1f, vector.sqrMagnitude, 1f) * mouseMovement) * phys.rigidbody.mass, ForceMode2D.Force);
	}

	protected override GameObject CreateEffectObject()
	{
		return Object.Instantiate(Resources.Load<GameObject>("Prefabs/Lift tool effect"));
	}
}
