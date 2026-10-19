using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JavelinBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Transform RayStart;

	[SkipSerialisation]
	public ProjectileLauncherBehaviour ProjectileLauncher;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public int LockOnLayer;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[SkipSerialisation]
	public AudioClip LockedOnClip;

	[SkipSerialisation]
	public LineRenderer Laser;

	[SkipSerialisation]
	[ColorUsage(false, true)]
	public Color LockingColor;

	[SkipSerialisation]
	[ColorUsage(false, true)]
	public Color LockedColor;

	[SkipSerialisation]
	public MuteBehaviour MuteBehaviour;

	private List<Utils.LaserHit> hits = new List<Utils.LaserHit>();

	private Vector3[] positions;

	private Vector2 endPoint;

	private PhysicalBehaviour target;

	private bool IsFlipped => base.transform.localScale.x < 0f;

	private void Start()
	{
		Laser.useWorldSpace = true;
		Laser.positionCount = 2;
		Laser.startColor = LockingColor;
		Laser.endColor = LockingColor;
		ProjectileLauncher.OnLaunch += TransferTarget;
	}

	private void OnEnable()
	{
		CastLaser();
		Laser.enabled = true;
	}

	private void OnDisable()
	{
		Laser.enabled = false;
	}

	private void TransferTarget(object source, GameObject projectile)
	{
		if (!projectile.TryGetComponent<JavelinMissileBehaviour>(out var component))
		{
			Debug.LogWarning("This missile launcher is not launching missiles...");
			return;
		}
		CastLaser();
		component.StaticTargetPoint = endPoint;
		component.Target = target;
	}

	private void FixedUpdate()
	{
		Color color = (target ? LockedColor : LockingColor);
		Laser.startColor = color;
		Laser.endColor = color;
		CastLaser();
	}

	private void CastLaser()
	{
		if (!PhysicalBehaviour.rigidbody)
		{
			return;
		}
		hits.Clear();
		Vector3 localPosition = RayStart.localPosition;
		localPosition.x *= ((!IsFlipped) ? 1 : (-1));
		Utils.GetLaserEndPoint(PhysicalBehaviour.rigidbody.GetRelativePoint(localPosition), RayStart.TransformVector(Vector3.right), ref hits, LayerMask, 15000f);
		Laser.positionCount = hits.Count + 1;
		positions = new Vector3[hits.Count + 1];
		positions[0] = RayStart.position;
		for (int i = 0; i < hits.Count; i++)
		{
			positions[i + 1] = hits[i].point;
		}
		Laser.SetPositions(positions);
		Utils.LaserHit laserHit = hits.LastOrDefault();
		endPoint = laserHit.point;
		if ((bool)laserHit.physicalBehaviour && laserHit.physicalBehaviour.gameObject.layer == LockOnLayer)
		{
			if (target != laserHit.physicalBehaviour && !MuteBehaviour.Muted)
			{
				PhysicalBehaviour.PlayClipOnce(LockedOnClip);
			}
			target = laserHit.physicalBehaviour;
		}
		else
		{
			target = null;
		}
	}

	private void OnDestroy()
	{
		ProjectileLauncher.OnLaunch -= TransferTarget;
	}
}
