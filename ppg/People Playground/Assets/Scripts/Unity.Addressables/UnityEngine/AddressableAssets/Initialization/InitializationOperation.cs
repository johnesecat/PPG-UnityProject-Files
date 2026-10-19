using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.AddressableAssets.Utility;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.AddressableAssets.Initialization
{
	internal class InitializationOperation : AsyncOperationBase<IResourceLocator>
	{
		private AsyncOperationHandle<ResourceManagerRuntimeData> m_rtdOp;

		private string m_ProviderSuffix;

		private AddressablesImpl m_Addressables;

		private ResourceManagerDiagnostics m_Diagnostics;

		private InitalizationObjectsOperation m_InitGroupOps;

		protected override float Progress
		{
			get
			{
				if (m_rtdOp.IsValid())
				{
					return m_rtdOp.PercentComplete;
				}
				return 0f;
			}
		}

		protected override string DebugName => "InitializationOperation";

		public InitializationOperation(AddressablesImpl aa)
		{
			m_Addressables = aa;
			m_Diagnostics = new ResourceManagerDiagnostics(aa.ResourceManager);
		}

		internal static AsyncOperationHandle<IResourceLocator> CreateInitializationOperation(AddressablesImpl aa, string playerSettingsLocation, string providerSuffix)
		{
			JsonAssetProvider jsonAssetProvider = new JsonAssetProvider();
			jsonAssetProvider.IgnoreFailures = true;
			aa.ResourceManager.ResourceProviders.Add(jsonAssetProvider);
			TextDataProvider textDataProvider = new TextDataProvider();
			textDataProvider.IgnoreFailures = true;
			aa.ResourceManager.ResourceProviders.Add(textDataProvider);
			aa.ResourceManager.ResourceProviders.Add(new ContentCatalogProvider(aa.ResourceManager));
			ResourceLocationBase location = new ResourceLocationBase("RuntimeData", playerSettingsLocation, typeof(JsonAssetProvider).FullName, typeof(ResourceManagerRuntimeData));
			InitializationOperation initializationOperation = new InitializationOperation(aa);
			initializationOperation.m_rtdOp = aa.ResourceManager.ProvideResource<ResourceManagerRuntimeData>(location);
			initializationOperation.m_ProviderSuffix = providerSuffix;
			initializationOperation.m_InitGroupOps = new InitalizationObjectsOperation();
			initializationOperation.m_InitGroupOps.Init(initializationOperation.m_rtdOp, aa);
			AsyncOperationHandle<bool> asyncOperationHandle = aa.ResourceManager.StartOperation(initializationOperation.m_InitGroupOps, initializationOperation.m_rtdOp);
			return aa.ResourceManager.StartOperation(initializationOperation, asyncOperationHandle);
		}

		protected override void Execute()
		{
			if (m_rtdOp.Result == null)
			{
				Addressables.LogWarningFormat("Addressables - Unable to load runtime data at location {0}.", m_rtdOp);
				Complete(base.Result, success: false, $"Addressables - Unable to load runtime data at location {m_rtdOp}.");
				return;
			}
			ResourceManagerRuntimeData result = m_rtdOp.Result;
			m_Addressables.ResourceManager.postProfilerEvents = result.ProfileEvents;
			WebRequestQueue.SetMaxConcurrentRequests(result.MaxConcurrentWebRequests);
			m_Addressables.Release(m_rtdOp);
			if (result.CertificateHandlerType != null)
			{
				m_Addressables.ResourceManager.CertificateHandlerInstance = Activator.CreateInstance(result.CertificateHandlerType) as CertificateHandler;
			}
			if (!result.LogResourceManagerExceptions)
			{
				ResourceManager.ExceptionHandler = null;
			}
			if (!result.ProfileEvents)
			{
				m_Diagnostics.Dispose();
				m_Diagnostics = null;
				m_Addressables.ResourceManager.ClearDiagnosticCallbacks();
			}
			ContentCatalogProvider contentCatalogProvider = m_Addressables.ResourceManager.ResourceProviders.FirstOrDefault((IResourceProvider rp) => rp.GetType() == typeof(ContentCatalogProvider)) as ContentCatalogProvider;
			if (contentCatalogProvider != null)
			{
				contentCatalogProvider.DisableCatalogUpdateOnStart = result.DisableCatalogUpdateOnStartup;
				contentCatalogProvider.IsLocalCatalogInBundle = result.IsLocalCatalogInBundle;
			}
			ResourceLocationMap resourceLocationMap = new ResourceLocationMap("CatalogLocator", result.CatalogLocations);
			m_Addressables.AddResourceLocator(resourceLocationMap);
			if (!resourceLocationMap.Locate("AddressablesMainContentCatalog", typeof(ContentCatalogData), out var locations))
			{
				Addressables.LogWarningFormat("Addressables - Unable to find any catalog locations in the runtime data.");
				m_Addressables.RemoveResourceLocator(resourceLocationMap);
				Complete(base.Result, success: false, "Addressables - Unable to find any catalog locations in the runtime data.");
			}
			else
			{
				LoadContentCatalogInternal(locations, 0, resourceLocationMap);
			}
		}

		private static void LoadProvider(AddressablesImpl addressables, ObjectInitializationData providerData, string providerSuffix)
		{
			int num = -1;
			string text = (string.IsNullOrEmpty(providerSuffix) ? providerData.Id : (providerData.Id + providerSuffix));
			for (int i = 0; i < addressables.ResourceManager.ResourceProviders.Count; i++)
			{
				if (addressables.ResourceManager.ResourceProviders[i].ProviderId == text)
				{
					num = i;
					break;
				}
			}
			if (num >= 0 && string.IsNullOrEmpty(providerSuffix))
			{
				return;
			}
			IResourceProvider resourceProvider = providerData.CreateInstance<IResourceProvider>(text);
			if (resourceProvider != null)
			{
				if (num < 0 || !string.IsNullOrEmpty(providerSuffix))
				{
					addressables.ResourceManager.ResourceProviders.Add(resourceProvider);
				}
				else
				{
					addressables.ResourceManager.ResourceProviders[num] = resourceProvider;
				}
			}
			else
			{
				Addressables.LogWarningFormat("Addressables - Unable to load resource provider from {0}.", providerData);
			}
		}

		private static AsyncOperationHandle<IResourceLocator> OnCatalogDataLoaded(AddressablesImpl addressables, AsyncOperationHandle<ContentCatalogData> op, string providerSuffix)
		{
			ContentCatalogData result = op.Result;
			addressables.Release(op);
			if (result == null)
			{
				return addressables.ResourceManager.CreateCompletedOperation<IResourceLocator>(null, new Exception("Failed to load content catalog.").Message);
			}
			if (result.ResourceProviderData != null)
			{
				foreach (ObjectInitializationData resourceProviderDatum in result.ResourceProviderData)
				{
					LoadProvider(addressables, resourceProviderDatum, providerSuffix);
				}
			}
			if (addressables.InstanceProvider == null)
			{
				IInstanceProvider instanceProvider = result.InstanceProviderData.CreateInstance<IInstanceProvider>();
				if (instanceProvider != null)
				{
					addressables.InstanceProvider = instanceProvider;
				}
			}
			if (addressables.SceneProvider == null)
			{
				ISceneProvider sceneProvider = result.SceneProviderData.CreateInstance<ISceneProvider>();
				if (sceneProvider != null)
				{
					addressables.SceneProvider = sceneProvider;
				}
			}
			ResourceLocationMap resourceLocationMap = result.CreateCustomLocator(result.location.PrimaryKey, providerSuffix);
			addressables.AddResourceLocator(resourceLocationMap, result.localHash, result.location);
			addressables.AddResourceLocator(new DynamicResourceLocator(addressables));
			return addressables.ResourceManager.CreateCompletedOperation((IResourceLocator)resourceLocationMap, string.Empty);
		}

		public static AsyncOperationHandle<IResourceLocator> LoadContentCatalog(AddressablesImpl addressables, IResourceLocation loc, string providerSuffix)
		{
			Type typeFromHandle = typeof(ProviderOperation<ContentCatalogData>);
			ProviderOperation<ContentCatalogData> providerOperation = addressables.ResourceManager.CreateOperation<ProviderOperation<ContentCatalogData>>(typeFromHandle, typeFromHandle.GetHashCode(), 0, null);
			IResourceProvider provider = null;
			foreach (IResourceProvider resourceProvider in addressables.ResourceManager.ResourceProviders)
			{
				if (resourceProvider is ContentCatalogProvider)
				{
					provider = resourceProvider;
					break;
				}
			}
			AsyncOperationHandle<IList<AsyncOperationHandle>> asyncOperationHandle = addressables.ResourceManager.CreateGroupOperation<string>(loc.Dependencies, allowFailedDependencies: true);
			providerOperation.Init(addressables.ResourceManager, provider, loc, asyncOperationHandle, releaseDependenciesOnFailure: true);
			AsyncOperationHandle<ContentCatalogData> dependentOp = addressables.ResourceManager.StartOperation(providerOperation, asyncOperationHandle);
			asyncOperationHandle.Release();
			return addressables.ResourceManager.CreateChainOperation(dependentOp, (AsyncOperationHandle<ContentCatalogData> res) => OnCatalogDataLoaded(addressables, res, providerSuffix));
		}

		public AsyncOperationHandle<IResourceLocator> LoadContentCatalog(IResourceLocation loc, string providerSuffix)
		{
			return LoadContentCatalog(m_Addressables, loc, providerSuffix);
		}

		private void LoadContentCatalogInternal(IList<IResourceLocation> catalogs, int index, ResourceLocationMap locMap)
		{
			AsyncOperationHandle<IResourceLocator> asyncOperationHandle = LoadContentCatalog(catalogs[index], m_ProviderSuffix);
			asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IResourceLocator> op)
			{
				if (op.Result != null)
				{
					m_Addressables.RemoveResourceLocator(locMap);
					base.Result = op.Result;
					Complete(base.Result, success: true, string.Empty);
					m_Addressables.Release(op);
				}
				else if (index + 1 >= catalogs.Count)
				{
					Addressables.LogWarningFormat("Addressables - initialization failed.", op);
					m_Addressables.RemoveResourceLocator(locMap);
					Complete(base.Result, success: false, (op.OperationException != null) ? op.OperationException.Message : "LoadContentCatalogInternal");
					m_Addressables.Release(op);
				}
				else
				{
					LoadContentCatalogInternal(catalogs, index + 1, locMap);
					m_Addressables.Release(op);
				}
			};
		}
	}
}
