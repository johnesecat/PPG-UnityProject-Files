using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.ScriptableObject
{
	public class ScriptableObjectStateRoot
	{
		private readonly Dictionary<object, Dictionary<Type, ScriptableObjectState>> _ownerObjectToStateMap = new Dictionary<object, Dictionary<Type, ScriptableObjectState>>();

		public T Get<T>(object owner) where T : ScriptableObjectState, new()
		{
			AssertOwnerExists<T>(owner);
			if (!_ownerObjectToStateMap.ContainsKey(owner))
			{
				_ownerObjectToStateMap.Add(owner, new Dictionary<Type, ScriptableObjectState>());
			}
			Dictionary<Type, ScriptableObjectState> dictionary = _ownerObjectToStateMap[owner];
			if (!dictionary.ContainsKey(typeof(T)))
			{
				dictionary.Add(typeof(T), new T());
			}
			return (T)dictionary[typeof(T)];
		}

		public T GetGlobal<T>() where T : ScriptableObjectState, new()
		{
			return Get<T>(this);
		}

		public void Remove<T>(object owner) where T : ScriptableObjectState
		{
			if (!_ownerObjectToStateMap.ContainsKey(owner))
			{
				UnityEngine.Debug.LogWarning($"Unable to remove state object: {typeof(T).Name} for owner: {owner} as there's no states for that owner at all");
				return;
			}
			Dictionary<Type, ScriptableObjectState> dictionary = _ownerObjectToStateMap[owner];
			if (!dictionary.ContainsKey(typeof(T)))
			{
				UnityEngine.Debug.LogWarning($"Unable to remove state object: {typeof(T).Name} for owner: {owner} as there's no state with specified type for that owner");
				return;
			}
			dictionary.Remove(typeof(T));
			if (!dictionary.Any())
			{
				_ownerObjectToStateMap.Remove(owner);
			}
		}

		public void RemoveGlobal<T>() where T : ScriptableObjectState
		{
			Remove<T>(this);
		}

		private static void AssertOwnerExists<T>(object owner) where T : ScriptableObjectState, new()
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner needs to be specified to retrieve state object from ScriptableObjectStateRoot");
			}
		}
	}
}
