using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class GameObjectReference : Reference<GameObject, GameObjectVariable>
	{
		public GameObjectReference(GameObject Value)
			: base(Value)
		{
		}

		public GameObjectReference()
		{
		}
	}
}
