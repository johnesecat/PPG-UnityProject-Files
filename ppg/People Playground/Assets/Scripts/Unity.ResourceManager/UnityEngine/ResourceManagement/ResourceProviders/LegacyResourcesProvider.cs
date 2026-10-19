using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Assets from Legacy Resources")]
	public class LegacyResourcesProvider : ResourceProviderBase
	{
		internal class InternalOp
		{
			private AsyncOperation m_RequestOperation;

			private ProvideHandle m_ProvideHandle;

			public void Start(ProvideHandle provideHandle)
			{
				m_ProvideHandle = provideHandle;
				provideHandle.SetProgressCallback(PercentComplete);
				m_RequestOperation = Resources.LoadAsync(m_ProvideHandle.ResourceManager.TransformInternalId(m_ProvideHandle.Location), m_ProvideHandle.Type);
				m_RequestOperation.completed += AsyncOperationCompleted;
			}

			private void AsyncOperationCompleted(AsyncOperation op)
			{
				object obj = (op as ResourceRequest)?.asset;
				obj = ((obj != null && m_ProvideHandle.Type.IsAssignableFrom(obj.GetType())) ? obj : null);
				m_ProvideHandle.Complete(obj, obj != null, (obj == null) ? new Exception($"Unable to load asset of type {m_ProvideHandle.Type} from location {m_ProvideHandle.Location}.") : null);
			}

			public float PercentComplete()
			{
				if (m_RequestOperation == null)
				{
					return 0f;
				}
				return m_RequestOperation.progress;
			}
		}

		public override void Provide(ProvideHandle pi)
		{
			Type type = pi.Type;
			bool flag = type.IsGenericType && typeof(IList<>) == type.GetGenericTypeDefinition();
			string text = pi.ResourceManager.TransformInternalId(pi.Location);
			string mainKey;
			string subKey;
			if (type.IsArray || flag)
			{
				object obj = null;
				obj = ((!type.IsArray) ? ResourceManagerConfig.CreateListResult(type, Resources.LoadAll(text, type.GetGenericArguments()[0])) : ResourceManagerConfig.CreateArrayResult(type, Resources.LoadAll(text, type.GetElementType())));
				pi.Complete(obj, obj != null, (obj == null) ? new Exception($"Unable to load asset of type {pi.Type} from location {pi.Location}.") : null);
			}
			else if (ResourceManagerConfig.ExtractKeyAndSubKey(text, out mainKey, out subKey))
			{
				Object[] array = Resources.LoadAll(mainKey, pi.Type);
				object obj2 = null;
				Object[] array2 = array;
				foreach (Object @object in array2)
				{
					if (@object.name == subKey && pi.Type.IsAssignableFrom(@object.GetType()))
					{
						obj2 = @object;
						break;
					}
				}
				pi.Complete(obj2, obj2 != null, (obj2 == null) ? new Exception($"Unable to load asset of type {pi.Type} from location {pi.Location}.") : null);
			}
			else
			{
				new InternalOp().Start(pi);
			}
		}

		public override void Release(IResourceLocation location, object asset)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			Object @object = asset as Object;
			if (@object != null && !(@object is GameObject))
			{
				Resources.UnloadAsset(@object);
			}
		}
	}
}
