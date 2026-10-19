using System;
using System.Linq;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.Variable;
using UnityEngine;
using UnityEngine.Serialization;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public abstract class ComponentLookupBase<T> : MonoBehaviour where T : Component
	{
		[SerializeField]
		private LookupType _lookupType;

		[SerializeField]
		[ShowIf("IsUseValueVisible")]
		private T _useValue;

		[SerializeField]
		private StringReference _findName;

		[FormerlySerializedAs("_findNameMaxResolutionTimeBeforeGivingUp")]
		[SerializeField]
		[ShowIf("IsFindNameVisible")]
		private float _maxResolutionTimeBeforeGivingUp = 3f;

		[SerializeField]
		private bool _requireSpecificScene;

		[SerializeField]
		private StringReference _sceneName;

		[SerializeField]
		[ReferenceOptions(ForceVariableOnly = true)]
		private FeatureReference _findFeatureReference;

		private float _firstResolutionTime;

		private T _cachedFound;

		private bool _isResolutionByNameAbandoned;

		public bool IsUseValueVisible => _lookupType == LookupType.Direct;

		public bool IsFindNameVisible => _lookupType == LookupType.ByName;

		public bool IsByReferenceVisibleVisible => _lookupType == LookupType.ByFeatureReference;

		public T Resolve()
		{
			return Resolve(resetCache: false);
		}

		public T Resolve(bool resetCache)
		{
			switch (_lookupType)
			{
			case LookupType.Direct:
				return _useValue;
			case LookupType.ByName:
			case LookupType.ByFeatureReference:
				return GetCachedOrFind(resetCache);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private T GetCachedOrFind(bool resetCache)
		{
			if ((bool)(UnityEngine.Object)_cachedFound && !resetCache)
			{
				return _cachedFound;
			}
			if (_isResolutionByNameAbandoned && !resetCache)
			{
				return null;
			}
			if (_firstResolutionTime == 0f)
			{
				_firstResolutionTime = Time.realtimeSinceStartup;
			}
			if (resetCache || (!(UnityEngine.Object)_cachedFound && Time.realtimeSinceStartup - _firstResolutionTime < _maxResolutionTimeBeforeGivingUp))
			{
				if (_requireSpecificScene)
				{
					switch (_lookupType)
					{
					case LookupType.Direct:
						return _useValue;
					case LookupType.ByName:
					{
						GameObject obj = (from go in Resources.FindObjectsOfTypeAll<GameObject>()
							where go.name == _findName.Value
							select go).FirstOrDefault((GameObject go) => go.scene.name == _sceneName.Value);
						_cachedFound = (((object)obj != null) ? obj.GetComponent<T>() : null);
						break;
					}
					case LookupType.ByFeatureReference:
					{
						FeatureReferenceComponent featureReferenceComponent = (from fr in Resources.FindObjectsOfTypeAll<FeatureReferenceComponent>()
							where fr.FeatureReference.Variable == _findFeatureReference.Variable
							select fr).FirstOrDefault((FeatureReferenceComponent fe) => fe.gameObject.scene.name == _sceneName.Value);
						_cachedFound = (((object)featureReferenceComponent != null) ? featureReferenceComponent.GetComponent<T>() : null);
						break;
					}
					default:
						throw new ArgumentOutOfRangeException("_lookupType", _lookupType, null);
					}
				}
				else
				{
					switch (_lookupType)
					{
					case LookupType.Direct:
						return _useValue;
					case LookupType.ByName:
					{
						GameObject obj2 = GameObject.Find(_findName.Value);
						_cachedFound = (((object)obj2 != null) ? obj2.GetComponent<T>() : null);
						break;
					}
					case LookupType.ByFeatureReference:
					{
						FeatureReferenceComponent featureReferenceComponent2 = UnityEngine.Object.FindObjectsOfType<FeatureReferenceComponent>().FirstOrDefault((FeatureReferenceComponent fr) => fr.FeatureReference.Variable == _findFeatureReference.Variable);
						_cachedFound = (((object)featureReferenceComponent2 != null) ? featureReferenceComponent2.GetComponent<T>() : null);
						break;
					}
					default:
						throw new ArgumentOutOfRangeException("_lookupType", _lookupType, null);
					}
				}
			}
			else
			{
				_isResolutionByNameAbandoned = true;
				UnityEngine.Debug.LogWarning($"Unable to resolve transform by {_lookupType}: {_firstResolutionTime}, giving up..");
			}
			return _cachedFound;
		}

		public void RequireSpecificScene(string sceneName)
		{
			_requireSpecificScene = true;
			_sceneName.UseConstant = true;
			_sceneName.ConstantValue = sceneName;
		}
	}
}
