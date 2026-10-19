using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class HovercarBehaviour : MonoBehaviour, Messages.IUse
{
	public float HoverHeight = 1.189f;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public CosmeticRotationBehaviour Fan;

	[SkipSerialisation]
	public AudioSource IdleLoop;

	[SkipSerialisation]
	public AudioSource AccLoop;

	[SkipSerialisation]
	public Transform[] HoverPoints;

	[SkipSerialisation]
	public LayerMask Layers;

	public float ForceMultiplier = 1f;

	public float FactorPower = 2f;

	public float VelocityRetention = 0.99f;

	public float ParkForceMultiplier = 1f;

	public float BaseSpeed = 1f;

	public bool Activated;

	[SkipSerialisation]
	[ReadOnly]
	public float[] calibratedHoverHeights;

	private bool isOnGround;

	private void Awake()
	{
		calibratedHoverHeights = new float[HoverPoints.Length];
		for (int i = 0; i < HoverPoints.Length; i++)
		{
			calibratedHoverHeights[i] = HoverHeight + HoverPoints[i].localPosition.y;
		}
	}

	private void Start()
	{
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("reverseCar", "Reverse gear", "Reverse vehicle", delegate
		{
			BaseSpeed = 0f - BaseSpeed;
		})
		{
			Condition = () => base.enabled
		};
		buttons.Add(item);
		IdleLoop.Play();
		AccLoop.Play();
	}

	public void Use(ActivationPropagation a)
	{
		if (base.enabled)
		{
			if (a.Channel == 1)
			{
				BaseSpeed = 0f - BaseSpeed;
			}
			else
			{
				Activated = !Activated;
			}
		}
	}

	private void Update()
	{
		Fan.RotationSpeedTarget = -1500f - PhysicalBehaviour.Charge * 150f;
		AccLoop.volume = Mathf.Lerp(AccLoop.volume, (isOnGround && Activated) ? 1 : 0, Utils.GetLerpFactorDeltaTime(0.9f, Time.deltaTime));
	}

	private void FixedUpdate()
	{
		if (PhysicalBehaviour.rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			Hover();
			if (Activated && isOnGround)
			{
				PhysicalBehaviour.rigidbody.AddForce(GetTargetForceVector(), ForceMode2D.Force);
			}
		}
	}

	public Vector2 GetTargetForceVector()
	{
		return (float)((!(base.transform.lossyScale.x < 0f)) ? 1 : (-1)) * (BaseSpeed + PhysicalBehaviour.Charge) * base.transform.right;
	}

	private void OnEnable()
	{
		Fan.ShouldRotate = true;
		IdleLoop.Play();
		AccLoop.Play();
	}

	private void OnDisable()
	{
		Fan.ShouldRotate = false;
		IdleLoop.Stop();
		AccLoop.Stop();
	}

	private void Hover()
	{
		Vector3 up = base.transform.up;
		Vector3 vector = up * -1f;
		isOnGround = false;
		for (int i = 0; i < HoverPoints.Length; i++)
		{
			Transform transform = HoverPoints[i];
			float num = calibratedHoverHeights[i];
			RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, vector, num * 2f, Layers);
			if (!raycastHit2D.transform)
			{
				continue;
			}
			float num2 = Mathf.Clamp01((num - raycastHit2D.distance) / num) * Vector2.Dot(raycastHit2D.normal, up);
			if (num2 <= float.Epsilon)
			{
				continue;
			}
			isOnGround = true;
			num2 = Mathf.Pow(num2, FactorPower);
			Vector2 point = raycastHit2D.point;
			Debug.DrawRay(point, raycastHit2D.normal * num2, Color.cyan);
			Vector3 vector2 = ForceMultiplier * num2 * vector;
			PhysicalBehaviour.rigidbody.AddForceAtPosition(vector2 * -1f, transform.position, ForceMode2D.Force);
			PhysicalBehaviour.rigidbody.velocity *= VelocityRetention;
			PhysicalBehaviour.rigidbody.angularVelocity *= VelocityRetention;
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
			{
				value.rigidbody.AddForceAtPosition(vector2, point);
				if (Activated)
				{
					value.rigidbody.AddForceAtPosition(GetTargetForceVector() * -0.25f, point);
					continue;
				}
				Vector2 vector3 = ((value.rigidbody.bodyType == RigidbodyType2D.Dynamic) ? value.rigidbody.GetPointVelocity(raycastHit2D.point) : default(Vector2)) - PhysicalBehaviour.rigidbody.GetPointVelocity(raycastHit2D.point);
				PhysicalBehaviour.rigidbody.AddForceAtPosition(vector3 * ParkForceMultiplier, transform.position, ForceMode2D.Force);
			}
		}
	}
}
