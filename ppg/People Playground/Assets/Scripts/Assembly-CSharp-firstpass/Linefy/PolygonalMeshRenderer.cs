using System;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	[ExecuteInEditMode]
	[DefaultExecutionOrder(1000)]
	[HelpURL("https://polyflow.xyz/content/linefy/documentation-1-1/linefy-documentation.html#PolygonalMeshRenderer")]
	public class PolygonalMeshRenderer : MonoBehaviour
	{
		[Tooltip("Source Polygonal Mesh Asset.")]
		public PolygonalMeshAsset polygonalMeshAsset;

		[Tooltip("Polygonal Mesh properties")]
		public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();

		[Tooltip("When enabled, the mesh will be drawn even without an attached MeshRenderer but with a default PolygonalMesh material. Default material settings are available in Polygonal Mesh Properties foldout")]
		public bool drawDefault;

		[Tooltip("Display mesh wireframe.")]
		public bool wireframeEnabled = true;

		[Tooltip("Enables automatical wireframe.viewOffset recalculation.")]
		public bool autoWireframeViewOffset = true;

		[Tooltip("Wireframe properties")]
		public SerializationData_Lines wireframeProperties = new SerializationData_Lines(2f, Color.black, 1f);

		private MeshFilter _mf;

		private PolygonalMesh polygonalMesh;

		private Lines wireframe;

		[Obsolete("wireframeWidth  is Obsolete , use wireframeProperties.widthMultiplier")]
		public float wireframeWidth;

		private MeshFilter mf
		{
			get
			{
				if (_mf == null)
				{
					_mf = GetComponent<MeshFilter>();
				}
				return _mf;
			}
		}

		public void LateUpdate()
		{
			if (polygonalMeshAsset != null && (polygonalMesh == null || polygonalMesh.modificationInfo.hash != polygonalMeshAsset.serializedPolygonalMesh.modificationInfo.hash))
			{
				if (polygonalMeshAsset.serializedPolygonalMesh == null)
				{
					Debug.LogWarning("Something wrong with PolygonalMeshAsset serialization. serializedPolygonalMesh == null");
				}
				else if (polygonalMesh == null)
				{
					polygonalMesh = new PolygonalMesh(polygonalMeshAsset.serializedPolygonalMesh);
				}
				else
				{
					polygonalMesh.BuildFromSPM(polygonalMeshAsset.serializedPolygonalMesh);
				}
			}
			if (polygonalMesh == null)
			{
				return;
			}
			polygonalMesh.LoadSerializationData(polygonalMeshProperties);
			if (drawDefault)
			{
				polygonalMesh.Draw(base.transform.localToWorldMatrix, base.gameObject.layer);
			}
			if (mf != null)
			{
				polygonalMesh.Apply();
				mf.sharedMesh = polygonalMesh.generatedMesh;
			}
			if (wireframeEnabled)
			{
				Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
				if (wireframe == null)
				{
					wireframe = new Lines(polygonalMesh.positionEdgesCount);
				}
				wireframe.LoadSerializationData(wireframeProperties);
				polygonalMesh.positionEdgesWireframe = wireframe;
				polygonalMesh.Apply();
				if (autoWireframeViewOffset)
				{
					Vector3 lossyScale = base.transform.lossyScale;
					float num = Mathf.Min(lossyScale.x, Mathf.Min(lossyScale.y, lossyScale.z));
					wireframeProperties.viewOffset = polygonalMesh.bounds.size.x * 0.0025f * num;
				}
				wireframe.Draw(localToWorldMatrix, base.gameObject.layer);
			}
			else
			{
				polygonalMesh.positionEdgesWireframe = null;
			}
		}

		private void OnDestroy()
		{
			if (wireframe != null)
			{
				wireframe.Dispose();
			}
			if (polygonalMesh != null)
			{
				polygonalMesh.Dispose();
			}
		}
	}
}
