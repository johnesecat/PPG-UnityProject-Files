using System;
using ImmersiveVRTools.Runtime.Common.Extensions;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common
{
	public class FollowObjectTarget : MonoBehaviour
	{
		[SerializeField]
		private Transform Source;

		[SerializeField]
		private string SourceName;

		public Func<Vector3> GetPositionOffset;

		[SerializeField]
		private bool FollowPosition = true;

		[SerializeField]
		private Boolean3 _followRotation = new Boolean3(all: true);

		[SerializeField]
		private bool RelativeToInitialSourceRotation;

		[SerializeField]
		private float _sourceUpdateEveryNSeconds;

		private Quaternion _initialSourceRotation;

		private Quaternion _initialTargetRotation;

		private float _secondsSinceLastSourceUpdate;

		public Boolean3 FollowRotation
		{
			get
			{
				return _followRotation;
			}
			set
			{
				_followRotation = value;
			}
		}

		public void SetSource(Transform sourceTransform, Func<Vector3> getPostionOffset = null, bool relativeToInitialSourceRotation = false)
		{
			Source = sourceTransform;
			GetPositionOffset = getPostionOffset;
			if (sourceTransform != null)
			{
				_initialSourceRotation = sourceTransform.rotation;
				_initialTargetRotation = base.transform.rotation;
			}
			RelativeToInitialSourceRotation = relativeToInitialSourceRotation;
		}

		[ContextMenu("Update")]
		private void Update()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!Source && !string.IsNullOrEmpty(SourceName))
			{
				GameObject gameObject = GameObject.Find(SourceName);
				if ((bool)gameObject)
				{
					Source = gameObject.transform;
				}
			}
			if (Source != null && (_sourceUpdateEveryNSeconds == 0f || _secondsSinceLastSourceUpdate > _sourceUpdateEveryNSeconds))
			{
				if (FollowPosition)
				{
					UpdateTargetPosition(Source.transform.position - (GetPositionOffset?.Invoke() ?? Vector3.zero));
				}
				if (_followRotation.AnyTrue())
				{
					Quaternion targetRotation = ((!RelativeToInitialSourceRotation) ? Source.rotation : (Quaternion.Inverse(_initialSourceRotation * Quaternion.Inverse(Source.rotation)) * _initialTargetRotation));
					UpdateTargetRotation(targetRotation);
				}
				_secondsSinceLastSourceUpdate = 0f;
			}
			_secondsSinceLastSourceUpdate += Time.deltaTime;
		}

		protected virtual void UpdateTargetPosition(Vector3 newPosition)
		{
			base.transform.position = newPosition;
		}

		protected virtual void UpdateTargetRotation(Quaternion targetRotation)
		{
			base.transform.SetRotation(GenerateRotationAdheringToLimitsSet(targetRotation));
		}

		protected Quaternion GenerateRotationAdheringToLimitsSet(Quaternion targetRotation)
		{
			return Quaternion.Euler(_followRotation.x ? targetRotation.eulerAngles.x : 0f, _followRotation.y ? targetRotation.eulerAngles.y : 0f, _followRotation.z ? targetRotation.eulerAngles.z : 0f);
		}
	}
}
