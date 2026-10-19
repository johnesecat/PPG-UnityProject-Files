using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class GameObjectListReference : Reference<List<GameObject>, GameObjectListVariable>
	{
		public GameObjectListReference(List<GameObject> Value)
			: base(Value)
		{
		}

		public GameObjectListReference()
		{
		}
	}
}
