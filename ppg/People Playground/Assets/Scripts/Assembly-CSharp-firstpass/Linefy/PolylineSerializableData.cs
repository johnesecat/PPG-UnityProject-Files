using System;
using UnityEngine;

namespace Linefy
{
	[Obsolete("PolylineSerializableData is Obsolete , use Linefy.Serialization.SerializationData_Polyline and Linefy.Serialization.SerializationDataFull_Polyline instead")]
	public class PolylineSerializableData
	{
		public string name;

		public Vector3[] vertices;
	}
}
