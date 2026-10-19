using System;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.SmoothTransformOperations;
using UnityEngine;

public class FollowObjectTargetSmooth : FollowObjectTarget
{
	[SerializeField]
	private float _speedPerSecond = 1f;

	[SerializeField]
	private bool _moveInstantlyWhenPositionDifferenceOverThreshold;

	[SerializeField]
	[ShowIf("_moveInstantlyWhenPositionDifferenceOverThreshold")]
	private float _positionDistanceThresholdToMoveInstantly = 20f;

	[SerializeField]
	private bool _isRotationSpeedSynchronizedWithPosition;

	[SerializeField]
	[ShowIf("ShowRotateAnglesPerSecond")]
	private float _rotateAnglesPerSecond = 1f;

	private float _lastPositionUpdateDuration;

	private bool ShowRotateAnglesPerSecond => !_isRotationSpeedSynchronizedWithPosition;

	private TrackableCoroutine _existingMoveRoutine { get; set; }

	private TrackableCoroutine _existingRotationRoutine { get; set; }

	public float SpeedPerSecond
	{
		get
		{
			return _speedPerSecond;
		}
		set
		{
			_speedPerSecond = value;
		}
	}

	protected override void UpdateTargetRotation(Quaternion targetRotation)
	{
		if (!(targetRotation != base.transform.rotation) || _existingRotationRoutine != null)
		{
			return;
		}
		if (_isRotationSpeedSynchronizedWithPosition)
		{
			if (_lastPositionUpdateDuration == 0f)
			{
				base.transform.rotation = targetRotation;
				return;
			}
			_existingRotationRoutine = TransformSmoothRotator.RotateOverSeconds(base.transform, GenerateRotationAdheringToLimitsSet(targetRotation), _lastPositionUpdateDuration);
			_existingRotationRoutine.Finished += ClearExistingRotationRoutine;
			_existingRotationRoutine.Start(base.StartCoroutine);
		}
		else
		{
			_existingRotationRoutine = TransformSmoothRotator.RotateConstantSpeed(base.transform, GenerateRotationAdheringToLimitsSet(targetRotation), _rotateAnglesPerSecond);
			_existingRotationRoutine.Finished += ClearExistingRotationRoutine;
			_existingRotationRoutine.Start(base.StartCoroutine);
		}
	}

	protected override void UpdateTargetPosition(Vector3 newPosition)
	{
		if (newPosition != base.transform.position && _existingMoveRoutine == null)
		{
			if (_moveInstantlyWhenPositionDifferenceOverThreshold && Vector3.Distance(newPosition, base.transform.position) > _positionDistanceThresholdToMoveInstantly)
			{
				base.transform.position = newPosition;
				_lastPositionUpdateDuration = 0f;
			}
			else
			{
				_existingMoveRoutine = TransformSmoothMover.MoveConstantSpeed(base.transform, newPosition, _speedPerSecond, out _lastPositionUpdateDuration);
				_existingMoveRoutine.Finished += ClearExistingMoveRoutine;
				_existingMoveRoutine.Start(base.StartCoroutine);
			}
		}
	}

	private void ClearExistingMoveRoutine(object sender, EventArgs e)
	{
		if (_existingMoveRoutine != null)
		{
			_existingMoveRoutine.Finished -= ClearExistingMoveRoutine;
			_existingMoveRoutine = null;
		}
	}

	private void ClearExistingRotationRoutine(object sender, EventArgs e)
	{
		if (_existingRotationRoutine != null)
		{
			_existingRotationRoutine.Finished -= ClearExistingRotationRoutine;
			_existingRotationRoutine = null;
		}
	}
}
