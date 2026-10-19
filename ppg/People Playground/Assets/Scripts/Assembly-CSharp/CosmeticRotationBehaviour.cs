using UnityEngine;

public class CosmeticRotationBehaviour : MonoBehaviour
{
	public enum RotAxis
	{
		X,
		Y,
		Z
	}

	public RotAxis Axis = RotAxis.Z;

	public float RotationSpeedTarget = 10f;

	public float Dampening = 0.9f;

	public float Acceleration = 0.1f;

	public bool ShouldRotate = true;

	private float rot;

	private float vel;

	private void Update()
	{
		if (ShouldRotate)
		{
			vel = Mathf.Lerp(vel, RotationSpeedTarget, Utils.GetLerpFactorDeltaTime(Acceleration, Time.deltaTime));
		}
		vel = Mathf.Lerp(vel, 0f, Utils.GetLerpFactorDeltaTime(1f - Dampening, Time.deltaTime));
		rot += vel * Time.deltaTime;
		switch (Axis)
		{
		case RotAxis.X:
			base.transform.localRotation = Quaternion.Euler(rot, 0f, 0f);
			break;
		case RotAxis.Y:
			base.transform.localRotation = Quaternion.Euler(0f, rot, 0f);
			break;
		default:
			base.transform.localRotation = Quaternion.Euler(0f, 0f, rot);
			break;
		}
	}
}
