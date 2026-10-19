using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImmersiveVRTools.Runtime.Common.Extensions;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common
{
	public class AlignVRPlayscapeViewWithPoint : MonoBehaviour
	{
		[SerializeField]
		private Transform _playscapeToAlign;

		[SerializeField]
		private Transform _offset;

		[SerializeField]
		private Vector3 _customAdjustment;

		[SerializeField]
		private Transform _alightToReference;

		[SerializeField]
		private bool _autoStartAfterPlayEnabled;

		[SerializeField]
		[ShowIf("_autoStartAfterPlayEnabled")]
		private float _autoStartNSecondsAfterPlay = 1f;

		[SerializeField]
		private bool _updateEveryFrame;

		private Vector3 _playscapeCalculatedPosition;

		private void Start()
		{
			List<AlignVRPlayscapeViewWithPoint> list = (from p in Object.FindObjectsOfType<AlignVRPlayscapeViewWithPoint>()
				where p.enabled
				select p).ToList();
			if (list.Count > 1)
			{
				foreach (AlignVRPlayscapeViewWithPoint item in list)
				{
					UnityEngine.Debug.LogError("There are multiple active AlignVRPlayscapeViewWithPoint - please make sure only 1 is enabled at scene start, current: " + item.name, item);
				}
			}
			if (_autoStartAfterPlayEnabled)
			{
				StartCoroutine(Align(_autoStartNSecondsAfterPlay));
			}
		}

		private void Update()
		{
			if (_updateEveryFrame)
			{
				_playscapeToAlign.SetPosition(_playscapeCalculatedPosition + _customAdjustment);
			}
		}

		private IEnumerator Align(float secondsDelay)
		{
			yield return new WaitForSeconds(secondsDelay);
			Align();
		}

		[ContextMenu("Align")]
		private void Align()
		{
			_playscapeToAlign.SetPosition(Vector3.zero);
			_playscapeToAlign.SetPosition(_alightToReference.position + _offset.position * -1f);
			_playscapeCalculatedPosition = _playscapeToAlign.position;
			UnityEngine.Debug.Log("Playscape postion realigned, rotation still todo");
		}

		[ContextMenu("Create Playscape reference from current SceneView")]
		private void CapturePlayscapeReference()
		{
		}
	}
}
