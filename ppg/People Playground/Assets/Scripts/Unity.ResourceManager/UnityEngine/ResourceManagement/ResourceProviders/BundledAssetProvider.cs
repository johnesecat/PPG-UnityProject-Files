using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Assets from Bundles Provider")]
	public class BundledAssetProvider : ResourceProviderBase
	{
		internal class InternalOp
		{
			private AssetBundleRequest m_RequestOperation;

			private ProvideHandle m_ProvideHandle;

			private string subObjectName;

			internal static AssetBundle LoadBundleFromDependecies(IList<object> results)
			{
				if (results == null || results.Count == 0)
				{
					return null;
				}
				AssetBundle result = null;
				bool flag = true;
				for (int i = 0; i < results.Count; i++)
				{
					IAssetBundleResource assetBundleResource = results[i] as IAssetBundleResource;
					if (assetBundleResource != null)
					{
						AssetBundle assetBundle = assetBundleResource.GetAssetBundle();
						if (flag)
						{
							result = assetBundle;
						}
						flag = false;
					}
				}
				return result;
			}

			public void Start(ProvideHandle provideHandle)
			{
				provideHandle.SetProgressCallback(ProgressCallback);
				subObjectName = null;
				m_ProvideHandle = provideHandle;
				m_RequestOperation = null;
				List<object> list = new List<object>();
				m_ProvideHandle.GetDependencies(list);
				AssetBundle assetBundle = LoadBundleFromDependecies(list);
				if (assetBundle == null)
				{
					m_ProvideHandle.Complete<AssetBundle>(null, status: false, new Exception("Unable to load dependent bundle from location " + m_ProvideHandle.Location));
					return;
				}
				string text = m_ProvideHandle.ResourceManager.TransformInternalId(m_ProvideHandle.Location);
				string mainKey;
				string subKey;
				if (m_ProvideHandle.Type.IsArray)
				{
					m_RequestOperation = assetBundle.LoadAssetWithSubAssetsAsync(text, m_ProvideHandle.Type.GetElementType());
				}
				else if (m_ProvideHandle.Type.IsGenericType && typeof(IList<>) == m_ProvideHandle.Type.GetGenericTypeDefinition())
				{
					m_RequestOperation = assetBundle.LoadAssetWithSubAssetsAsync(text, m_ProvideHandle.Type.GetGenericArguments()[0]);
				}
				else if (ResourceManagerConfig.ExtractKeyAndSubKey(text, out mainKey, out subKey))
				{
					subObjectName = subKey;
					m_RequestOperation = assetBundle.LoadAssetWithSubAssetsAsync(mainKey, m_ProvideHandle.Type);
				}
				else
				{
					m_RequestOperation = assetBundle.LoadAssetAsync(text, m_ProvideHandle.Type);
				}
				m_RequestOperation.completed += ActionComplete;
			}

			private void ActionComplete(AsyncOperation obj)
			{
				object obj2 = null;
				if (m_RequestOperation != null)
				{
					if (m_ProvideHandle.Type.IsArray)
					{
						obj2 = ResourceManagerConfig.CreateArrayResult(m_ProvideHandle.Type, m_RequestOperation.allAssets);
					}
					else if (m_ProvideHandle.Type.IsGenericType && typeof(IList<>) == m_ProvideHandle.Type.GetGenericTypeDefinition())
					{
						obj2 = ResourceManagerConfig.CreateListResult(m_ProvideHandle.Type, m_RequestOperation.allAssets);
					}
					else if (string.IsNullOrEmpty(subObjectName))
					{
						obj2 = ((m_RequestOperation.asset != null && m_ProvideHandle.Type.IsAssignableFrom(m_RequestOperation.asset.GetType())) ? m_RequestOperation.asset : null);
					}
					else if (m_RequestOperation.allAssets != null)
					{
						Object[] allAssets = m_RequestOperation.allAssets;
						foreach (Object @object in allAssets)
						{
							if (@object.name == subObjectName && m_ProvideHandle.Type.IsAssignableFrom(@object.GetType()))
							{
								obj2 = @object;
								break;
							}
						}
					}
				}
				m_ProvideHandle.Complete(obj2, obj2 != null, (obj2 == null) ? new Exception($"Unable to load asset of type {m_ProvideHandle.Type} from location {m_ProvideHandle.Location}.") : null);
			}

			public float ProgressCallback()
			{
				if (m_RequestOperation == null)
				{
					return 0f;
				}
				return m_RequestOperation.progress;
			}
		}

		public override void Provide(ProvideHandle provideHandle)
		{
			new InternalOp().Start(provideHandle);
		}
	}
}
