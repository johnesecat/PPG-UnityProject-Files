using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ParentChildMovementWithoutHierarchy
	{
		private readonly Transform _parent;

		private readonly Transform _child;

		private Vector3 _localParentRealativeChildPosition;

		private Vector3 _parentRelativeForwardDirection;

		private Vector3 _parentRelativeUpDirection;

		public ParentChildMovementWithoutHierarchy(Transform parent, Transform child)
		{
			_parent = parent;
			_child = child;
		}

		public void Initialize()
		{
			_localParentRealativeChildPosition = _parent.InverseTransformPoint(_child.position);
			_parentRelativeForwardDirection = _parent.InverseTransformDirection(_child.forward);
			_parentRelativeUpDirection = _parent.InverseTransformDirection(_child.up);
		}

		public PositionRotationPair GenerateParentAdjustedChildPositionRotationPair()
		{
			Vector3 position = _parent.TransformPoint(_localParentRealativeChildPosition);
			Vector3 forward = _parent.TransformDirection(_parentRelativeForwardDirection);
			Vector3 upwards = _parent.TransformDirection(_parentRelativeUpDirection);
			Quaternion rotation = Quaternion.LookRotation(forward, upwards);
			return new PositionRotationPair(position, rotation);
		}
	}
}
