using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ColliderSetter : MonoBehaviour
	{
		private static readonly List<string> ExcludeNames = new List<string>();

		[ContextMenu("SetBoxColliderToEncapsulateModels")]
		public void SetBoxColliderToEncapsulateModels()
		{
			SetBoxColliderToEncapsulateModels<BoxCollider>(1, new GameObject[1] { base.gameObject });
		}

		public void SetBoxColliderToEncapsulateModels<CollType>(int colliderPadding, params GameObject[] from) where CollType : Collider
		{
			List<Bounds> list = new List<Bounds>();
			Bounds bounds = default(Bounds);
			int num = 0;
			GameObject[] array = from;
			foreach (GameObject gameObject in array)
			{
				if (!ExcludeNames.Any(gameObject.name.Contains))
				{
					Bounds childRendererBounds = GetChildRendererBounds(gameObject);
					list.Add(childRendererBounds);
					bounds.Encapsulate(childRendererBounds);
				}
			}
			array = from;
			foreach (GameObject gameObject2 in array)
			{
				if (typeof(CollType) == typeof(BoxCollider))
				{
					BoxCollider boxCollider = gameObject2.AddComponent<BoxCollider>();
					boxCollider.enabled = true;
					boxCollider.size = list[num].size * 2f * colliderPadding;
					boxCollider.center = new Vector3(0f, boxCollider.size.y / 2f, 0f);
					num++;
				}
			}
		}

		private Bounds GetChildRendererBounds(GameObject go)
		{
			MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren.Length != 0)
			{
				Bounds bounds = componentsInChildren[0].sharedMesh.bounds;
				MeshFilter[] array = componentsInChildren;
				foreach (MeshFilter meshFilter in array)
				{
					if (!ExcludeNames.Any(meshFilter.gameObject.name.Contains))
					{
						bounds.Encapsulate(meshFilter.sharedMesh.bounds);
					}
				}
				return bounds;
			}
			return default(Bounds);
		}
	}
}
