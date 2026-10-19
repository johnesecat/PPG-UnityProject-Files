using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ScriptableObjectOnCreationExecutor
	{
		private const float MAX_TIME_TO_WAIT_FOR_OBJECT_TO_BE_CREATED_BEFORE_REMOVING_EVENT = 30f;

		private float _editorUpdateEventAddedAt;

		private bool _editorUpdateEventTimeSet;

		private Action _executeOnCreation;

		private UnityEngine.ScriptableObject _so;

		private float _abandonAfterNSecondsIfNotCreated;

		public ScriptableObjectOnCreationExecutor(Action executeOnCreation, UnityEngine.ScriptableObject so, float abandonAfterNSecondsIfNotCreated = 30f)
		{
			_executeOnCreation = executeOnCreation;
			_so = so;
			_abandonAfterNSecondsIfNotCreated = abandonAfterNSecondsIfNotCreated;
		}

		public void ExecuteOnCreationOnly()
		{
		}
	}
}
