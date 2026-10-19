using UnityEngine;

public class PulseDrumBehaviour : CanShoot, Messages.IUse
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public LayerMask Layers;

	public float BaseForce = 10f;

	public float ChargeInfluence = 10f;

	public int RayCount = 6;

	public float SpreadAngle = 30f;

	public float Range = 2f;

	public float EmitterSize = 0.5f;

	[SkipSerialisation]
	public AudioSource Source;

	[SkipSerialisation]
	public AudioClip[] Clips;

	private static readonly RaycastHit2D[] buffer = new RaycastHit2D[8];

	public override Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Shoot();
		}
	}

	public override void Shoot()
	{
		Source.PlayOneShot(Clips.PickRandom());
		Vector2 vector = BarrelPosition;
		Vector2 vector2 = BarrelDirection;
		float num = BaseForce + PhysicalBehaviour.Charge * ChargeInfluence;
		CameraShakeBehaviour.main.Shake(num * 0.01f, vector);
		PhysicalBehaviour.rigidbody.AddForceAtPosition((0f - num) * vector2, vector);
		float num2 = SpreadAngle / 2f;
		float num3 = EmitterSize / 2f;
		float num4 = (float)RayCount - 1f;
		for (int i = 0; i < RayCount; i++)
		{
			float num5 = (float)i / num4;
			float z = num5 * SpreadAngle - num2;
			float num6 = num5 * EmitterSize - num3;
			Vector3 vector3 = Quaternion.Euler(0f, 0f, z) * vector2;
			int num7 = Physics2D.RaycastNonAlloc(vector + Vector2.Perpendicular(vector2) * num6, vector3, buffer, Range, Layers);
			for (int j = 0; j < num7; j++)
			{
				RaycastHit2D raycastHit2D = buffer[j];
				if ((bool)raycastHit2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
				{
					Vector3 vector4 = num * value.rigidbody.mass * vector3;
					value.rigidbody.AddForceAtPosition(vector4, raycastHit2D.point);
				}
			}
		}
	}
}
