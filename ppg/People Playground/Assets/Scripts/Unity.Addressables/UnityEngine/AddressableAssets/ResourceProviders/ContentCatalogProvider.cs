using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets.ResourceProviders
{
	[DisplayName("Content Catalog Provider")]
	public class ContentCatalogProvider : ResourceProviderBase
	{
		public enum DependencyHashIndex
		{
			Remote,
			Cache,
			Count
		}

		internal class InternalOp
		{
			internal class BundledCatalog
			{
				private readonly string m_BundlePath;

				private bool m_OpInProgress;

				private AssetBundleCreateRequest m_LoadBundleRequest;

				internal AssetBundle m_CatalogAssetBundle;

				private AssetBundleRequest m_LoadTextAssetRequest;

				private ContentCatalogData m_CatalogData;

				public bool OpInProgress => m_OpInProgress;

				public bool OpIsSuccess
				{
					get
					{
						if (!m_OpInProgress)
						{
							return m_CatalogData != null;
						}
						return false;
					}
				}

				public event Action<ContentCatalogData> OnLoaded;

				public BundledCatalog(string bundlePath)
				{
					if (string.IsNullOrEmpty(bundlePath))
					{
						throw new ArgumentNullException("bundlePath", "Catalog bundle path is null.");
					}
					if (!bundlePath.EndsWith(".bundle"))
					{
						throw new ArgumentException("You must supply a valid bundle file path.");
					}
					m_BundlePath = bundlePath;
				}

				~BundledCatalog()
				{
					Unload();
				}

				private void Unload()
				{
					m_CatalogAssetBundle?.Unload(unloadAllLoadedObjects: true);
					m_CatalogAssetBundle = null;
				}

				public void LoadCatalogFromBundleAsync()
				{
					if (m_OpInProgress)
					{
						Addressables.LogError("Operation in progress : A catalog is already being loaded. Please wait for the operation to complete.");
						return;
					}
					m_OpInProgress = true;
					m_LoadBundleRequest = AssetBundle.LoadFromFileAsync(m_BundlePath);
					m_LoadBundleRequest.completed += delegate(AsyncOperation loadOp)
					{
						AssetBundleCreateRequest assetBundleCreateRequest = loadOp as AssetBundleCreateRequest;
						if (assetBundleCreateRequest != null && assetBundleCreateRequest.assetBundle != null)
						{
							m_CatalogAssetBundle = assetBundleCreateRequest.assetBundle;
							m_LoadTextAssetRequest = m_CatalogAssetBundle.LoadAllAssetsAsync<TextAsset>();
							m_LoadTextAssetRequest.completed += delegate(AsyncOperation op)
							{
								AssetBundleRequest assetBundleRequest = op as AssetBundleRequest;
								if (assetBundleRequest != null)
								{
									TextAsset textAsset = assetBundleRequest.asset as TextAsset;
									if ((object)textAsset != null && textAsset.text != null)
									{
										m_CatalogData = JsonUtility.FromJson<ContentCatalogData>(textAsset.text);
										this.OnLoaded?.Invoke(m_CatalogData);
										goto IL_0060;
									}
								}
								Addressables.LogError("No catalog text assets where found in bundle " + m_BundlePath);
								goto IL_0060;
								IL_0060:
								Unload();
								m_OpInProgress = false;
							};
						}
						else
						{
							Addressables.LogError("Unable to load dependent bundle from location : " + m_BundlePath);
							m_OpInProgress = false;
						}
					};
				}
			}

			private string m_LocalDataPath;

			private string m_RemoteHashValue;

			private string m_LocalHashValue;

			private ProvideHandle m_ProviderInterface;

			internal ContentCatalogData m_ContentCatalogData;

			public void Start(ProvideHandle providerInterface, bool disableCatalogUpdateOnStart, bool isLocalCatalogInBundle)
			{
				m_ProviderInterface = providerInterface;
				m_LocalDataPath = null;
				m_RemoteHashValue = null;
				List<object> list = new List<object>();
				m_ProviderInterface.GetDependencies(list);
				string text = DetermineIdToLoad(m_ProviderInterface.Location, list, disableCatalogUpdateOnStart);
				bool isLocalCatalog = text.Equals(GetTransformedInternalId(m_ProviderInterface.Location));
				LoadCatalog(text, isLocalCatalogInBundle, isLocalCatalog);
			}

			public void Release()
			{
				m_ContentCatalogData?.CleanData();
			}

			internal void LoadCatalog(string idToLoad, bool isLocalCatalogInBundle, bool isLocalCatalog)
			{
				try
				{
					if (isLocalCatalogInBundle && isLocalCatalog)
					{
						BundledCatalog bundledCatalog = new BundledCatalog(idToLoad);
						bundledCatalog.OnLoaded += delegate(ContentCatalogData ccd)
						{
							m_ContentCatalogData = ccd;
							OnCatalogLoaded(ccd);
						};
						bundledCatalog.LoadCatalogFromBundleAsync();
						return;
					}
					IResourceLocation location = new ResourceLocationBase(idToLoad, idToLoad, typeof(JsonAssetProvider).FullName, typeof(ContentCatalogData));
					AsyncOperationHandle<ContentCatalogData> asyncOperationHandle = m_ProviderInterface.ResourceManager.ProvideResource<ContentCatalogData>(location);
					asyncOperationHandle.Completed += delegate(AsyncOperationHandle<ContentCatalogData> op)
					{
						m_ContentCatalogData = op.Result;
						m_ProviderInterface.ResourceManager.Release(op);
						OnCatalogLoaded(m_ContentCatalogData);
					};
				}
				catch (Exception exception)
				{
					m_ProviderInterface.Complete<ContentCatalogData>(null, status: false, exception);
				}
			}

			private string GetTransformedInternalId(IResourceLocation loc)
			{
				if (m_ProviderInterface.ResourceManager == null)
				{
					return loc.InternalId;
				}
				return m_ProviderInterface.ResourceManager.TransformInternalId(loc);
			}

			internal string DetermineIdToLoad(IResourceLocation location, IList<object> dependencyObjects, bool disableCatalogUpdateOnStart = false)
			{
				string text = GetTransformedInternalId(location);
				if (dependencyObjects != null && location.Dependencies != null && dependencyObjects.Count == 2 && location.Dependencies.Count == 2)
				{
					string text2 = dependencyObjects[0] as string;
					m_LocalHashValue = dependencyObjects[1] as string;
					if (string.IsNullOrEmpty(text2))
					{
						if (!string.IsNullOrEmpty(m_LocalHashValue))
						{
							text = GetTransformedInternalId(location.Dependencies[1]).Replace(".hash", ".json");
						}
					}
					else if (text2 == m_LocalHashValue)
					{
						text = GetTransformedInternalId(location.Dependencies[1]).Replace(".hash", ".json");
					}
					else
					{
						if (disableCatalogUpdateOnStart)
						{
							m_LocalHashValue = Hash128.Compute(text).ToString();
						}
						else
						{
							text = GetTransformedInternalId(location.Dependencies[0]).Replace(".hash", ".json");
							m_LocalDataPath = GetTransformedInternalId(location.Dependencies[1]).Replace(".hash", ".json");
						}
						m_RemoteHashValue = text2;
					}
				}
				return text;
			}

			private void OnCatalogLoaded(ContentCatalogData ccd)
			{
				if (ccd != null)
				{
					ccd.location = m_ProviderInterface.Location;
					ccd.localHash = m_LocalHashValue;
					if (!string.IsNullOrEmpty(m_RemoteHashValue) && !string.IsNullOrEmpty(m_LocalDataPath))
					{
						string directoryName = Path.GetDirectoryName(m_LocalDataPath);
						if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
						string localDataPath = m_LocalDataPath;
						File.WriteAllText(localDataPath, JsonUtility.ToJson(ccd));
						File.WriteAllText(localDataPath.Replace(".json", ".hash"), m_RemoteHashValue);
						ccd.localHash = m_RemoteHashValue;
					}
				}
				m_ProviderInterface.Complete(ccd, ccd != null, (ccd == null) ? new Exception($"Unable to load ContentCatalogData  from location {m_ProviderInterface.Location}.") : null);
			}
		}

		public bool DisableCatalogUpdateOnStart;

		public bool IsLocalCatalogInBundle;

		internal Dictionary<IResourceLocation, InternalOp> m_LocationToCatalogLoadOpMap = new Dictionary<IResourceLocation, InternalOp>();

		private ResourceManager m_RM;

		public ContentCatalogProvider(ResourceManager resourceManagerInstance)
		{
			m_RM = resourceManagerInstance;
			m_BehaviourFlags = ProviderBehaviourFlags.CanProvideWithFailedDependencies;
		}

		public override void Release(IResourceLocation location, object obj)
		{
			if (m_LocationToCatalogLoadOpMap.ContainsKey(location))
			{
				m_LocationToCatalogLoadOpMap[location].Release();
				m_LocationToCatalogLoadOpMap.Remove(location);
			}
			base.Release(location, obj);
		}

		public override void Provide(ProvideHandle providerInterface)
		{
			if (!m_LocationToCatalogLoadOpMap.ContainsKey(providerInterface.Location))
			{
				m_LocationToCatalogLoadOpMap.Add(providerInterface.Location, new InternalOp());
			}
			m_LocationToCatalogLoadOpMap[providerInterface.Location].Start(providerInterface, DisableCatalogUpdateOnStart, IsLocalCatalogInBundle);
		}
	}
}
