using UnityEngine;

namespace Linefy
{
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://polyflow.xyz/content/linefy/documentation-1-1/linefy-documentation.html#LinefyRectTransform")]
	public class LinefyRectTransform : MonoBehaviour
	{
		private bool _centeredRectTransformMatrix;

		public bool centeredRectTransformMatrix;

		private RectTransform _rt;

		private Matrix4x4 _rtMatrix = Matrix4x4.identity;

		public Matrix4x4 rectTransformWorldMatrix
		{
			get
			{
				if (_rt == null)
				{
					_rt = GetComponent<RectTransform>();
				}
				if (_rt.hasChanged)
				{
					_rtMatrix = (centeredRectTransformMatrix ? _rt.GetCenteredWorldMatrix() : _rt.GetWorldMatrix());
					_rt.hasChanged = false;
				}
				if (centeredRectTransformMatrix != _centeredRectTransformMatrix)
				{
					_centeredRectTransformMatrix = centeredRectTransformMatrix;
					_rtMatrix = (centeredRectTransformMatrix ? _rt.GetCenteredWorldMatrix() : _rt.GetWorldMatrix());
				}
				return _rtMatrix;
			}
		}
	}
}
