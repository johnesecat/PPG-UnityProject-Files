using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;

namespace UnityEngine.AddressableAssets
{
	internal class AddressablesImpl : IEqualityComparer<IResourceLocation>
	{
		public class ResourceLocatorInfo
		{
			public IResourceLocator Locator { get; private set; }

			public string LocalHash { get; private set; }

			public IResourceLocation CatalogLocation { get; private set; }

			public bool ContentUpdateAvailable { get; internal set; }

			public IResourceLocation HashLocation => CatalogLocation.Dependencies[0];

			public bool CanUpdateContent
			{
				get
				{
					if (!string.IsNullOrEmpty(LocalHash) && CatalogLocation != null && CatalogLocation.HasDependencies)
					{
						return CatalogLocation.Dependencies.Count == 2;
					}
					return false;
				}
			}

			public ResourceLocatorInfo(IResourceLocator loc, string localHash, IResourceLocation remoteCatalogLocation)
			{
				Locator = loc;
				LocalHash = localHash;
				CatalogLocation = remoteCatalogLocation;
			}

			internal void UpdateContent(IResourceLocator locator, string hash, IResourceLocation loc)
			{
				LocalHash = hash;
				CatalogLocation = loc;
				Locator = locator;
			}
		}

		private class LoadResourceLocationKeyOp : AsyncOperationBase<IList<IResourceLocation>>
		{
			private object m_Keys;

			private IList<IResourceLocation> m_locations;

			private AddressablesImpl m_Addressables;

			private Type m_ResourceType;

			protected override string DebugName => m_Keys.ToString();

			public void Init(AddressablesImpl aa, Type t, object keys)
			{
				m_Keys = keys;
				m_ResourceType = t;
				m_Addressables = aa;
			}

			protected override void Execute()
			{
				m_Addressables.GetResourceLocations(m_Keys, m_ResourceType, out m_locations);
				if (m_locations == null)
				{
					m_locations = new List<IResourceLocation>();
				}
				Complete(m_locations, success: true, string.Empty);
			}
		}

		private class LoadResourceLocationKeysOp : AsyncOperationBase<IList<IResourceLocation>>
		{
			private IEnumerable m_Key;

			private Addressables.MergeMode m_MergeMode;

			private IList<IResourceLocation> m_locations;

			private AddressablesImpl m_Addressables;

			private Type m_ResourceType;

			protected override string DebugName => "LoadResourceLocationKeysOp";

			public void Init(AddressablesImpl aa, Type t, IEnumerable key, Addressables.MergeMode mergeMode)
			{
				m_Key = key;
				m_ResourceType = t;
				m_MergeMode = mergeMode;
				m_Addressables = aa;
			}

			protected override void Execute()
			{
				m_Addressables.GetResourceLocations(m_Key, m_ResourceType, m_MergeMode, out m_locations);
				if (m_locations == null)
				{
					m_locations = new List<IResourceLocation>();
				}
				Complete(m_locations, success: true, string.Empty);
			}
		}

		private ResourceManager m_ResourceManager;

		private IInstanceProvider m_InstanceProvider;

		internal const string kCacheDataFolder = "{UnityEngine.Application.persistentDataPath}/com.unity.addressables/";

		public ISceneProvider SceneProvider;

		internal List<ResourceLocatorInfo> m_ResourceLocators = new List<ResourceLocatorInfo>();

		private AsyncOperationHandle<IResourceLocator> m_InitializationOperation;

		private AsyncOperationHandle<List<string>> m_ActiveCheckUpdateOperation;

		private AsyncOperationHandle<List<IResourceLocator>> m_ActiveUpdateOperation;

		private Action<AsyncOperationHandle> m_OnHandleCompleteAction;

		private Action<AsyncOperationHandle> m_OnSceneHandleCompleteAction;

		private Action<AsyncOperationHandle> m_OnHandleDestroyedAction;

		private Dictionary<object, AsyncOperationHandle> m_resultToHandle = new Dictionary<object, AsyncOperationHandle>();

		internal HashSet<AsyncOperationHandle> m_SceneInstances = new HashSet<AsyncOperationHandle>();

		internal bool hasStartedInitialization;

		public IInstanceProvider InstanceProvider
		{
			get
			{
				return m_InstanceProvider;
			}
			set
			{
				m_InstanceProvider = value;
				IUpdateReceiver updateReceiver = m_InstanceProvider as IUpdateReceiver;
				if (updateReceiver != null)
				{
					m_ResourceManager.AddUpdateReceiver(updateReceiver);
				}
			}
		}

		public ResourceManager ResourceManager
		{
			get
			{
				if (m_ResourceManager == null)
				{
					m_ResourceManager = new ResourceManager(new DefaultAllocationStrategy());
				}
				return m_ResourceManager;
			}
		}

		internal int SceneOperationCount => m_SceneInstances.Count;

		internal int TrackedHandleCount => m_resultToHandle.Count;

		public Func<IResourceLocation, string> InternalIdTransformFunc
		{
			get
			{
				return ResourceManager.InternalIdTransformFunc;
			}
			set
			{
				ResourceManager.InternalIdTransformFunc = value;
			}
		}

		public AsyncOperationHandle ChainOperation
		{
			get
			{
				if (!hasStartedInitialization)
				{
					return InitializeAsync();
				}
				if (m_InitializationOperation.IsValid() && !m_InitializationOperation.IsDone)
				{
					return m_InitializationOperation;
				}
				if (m_ActiveUpdateOperation.IsValid() && !m_ActiveUpdateOperation.IsDone)
				{
					return m_ActiveUpdateOperation;
				}
				Debug.LogWarning("ChainOperation property should not be accessed unless ShouldChainRequest is true.");
				return default(AsyncOperationHandle);
			}
		}

		internal bool ShouldChainRequest
		{
			get
			{
				if (!hasStartedInitialization)
				{
					return true;
				}
				if (m_InitializationOperation.IsValid() && !m_InitializationOperation.IsDone)
				{
					return true;
				}
				if (m_ActiveUpdateOperation.IsValid())
				{
					return !m_ActiveUpdateOperation.IsDone;
				}
				return false;
			}
		}

		public string StreamingAssetsSubFolder => "aa";

		public string BuildPath => "Library/com.unity.addressables/" + StreamingAssetsSubFolder + "/" + PlatformMappingService.GetPlatform();

		public string PlayerBuildDataPath => Application.streamingAssetsPath + "/" + StreamingAssetsSubFolder;

		public string RuntimePath => PlayerBuildDataPath;

		public IEnumerable<IResourceLocator> ResourceLocators => m_ResourceLocators.Select((ResourceLocatorInfo l) => l.Locator);

		internal IEnumerable<string> CatalogsWithAvailableUpdates => from s in m_ResourceLocators
			where s.ContentUpdateAvailable
			select s.Locator.LocatorId;

		public AddressablesImpl(IAllocationStrategy alloc)
		{
			m_ResourceManager = new ResourceManager(alloc);
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		internal void ReleaseSceneManagerOperation()
		{
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}

		internal void OnSceneUnloaded(Scene scene)
		{
			foreach (AsyncOperationHandle sceneInstance in m_SceneInstances)
			{
				if (!sceneInstance.IsValid())
				{
					m_SceneInstances.Remove(sceneInstance);
					break;
				}
				AsyncOperationHandle<SceneInstance> sceneLoadHandle = sceneInstance.Convert<SceneInstance>();
				if (sceneLoadHandle.Result.Scene == scene)
				{
					m_SceneInstances.Remove(sceneInstance);
					m_resultToHandle.Remove(sceneInstance.Result);
					AsyncOperationHandle<SceneInstance> op = m_ResourceManager.ReleaseScene(SceneProvider, sceneLoadHandle);
					op.Completed += delegate
					{
						Release(op);
					};
					break;
				}
			}
			m_ResourceManager.CleanupSceneInstances(scene);
		}

		public void Log(string msg)
		{
			Debug.Log(msg);
		}

		public void LogFormat(string format, params object[] args)
		{
			Debug.LogFormat(format, args);
		}

		public void LogWarning(string msg)
		{
			Debug.LogWarning(msg);
		}

		public void LogWarningFormat(string format, params object[] args)
		{
			Debug.LogWarningFormat(format, args);
		}

		public void LogError(string msg)
		{
			Debug.LogError(msg);
		}

		public void LogException(AsyncOperationHandle op, Exception ex)
		{
			if (op.Status == AsyncOperationStatus.Failed)
			{
				Debug.LogErrorFormat("{0} encountered in operation {1}, status={2}, result={3} : {4}", ex.GetType().Name, op.DebugName, op.Status, op.Result, ex.Message);
			}
		}

		public void LogErrorFormat(string format, params object[] args)
		{
			Debug.LogErrorFormat(format, args);
		}

		public string ResolveInternalId(string id)
		{
			string text = AddressablesRuntimeProperties.EvaluateString(id);
			if (text.Length >= 260 && text.StartsWith(Application.dataPath))
			{
				text = text.Substring(Application.dataPath.Length + 1);
			}
			return text;
		}

		public void AddResourceLocator(IResourceLocator loc, string localCatalogHash = null, IResourceLocation remoteCatalogLocation = null)
		{
			m_ResourceLocators.Add(new ResourceLocatorInfo(loc, localCatalogHash, remoteCatalogLocation));
		}

		public void RemoveResourceLocator(IResourceLocator loc)
		{
			m_ResourceLocators.RemoveAll((ResourceLocatorInfo l) => l.Locator == loc);
		}

		public void ClearResourceLocators()
		{
			m_ResourceLocators.Clear();
		}

		internal bool GetResourceLocations(object key, Type type, out IList<IResourceLocation> locations)
		{
			if (type == null && key is AssetReference)
			{
				type = (key as AssetReference).SubOjbectType;
			}
			key = EvaluateKey(key);
			locations = null;
			HashSet<IResourceLocation> hashSet = null;
			foreach (ResourceLocatorInfo resourceLocator in m_ResourceLocators)
			{
				if (!resourceLocator.Locator.Locate(key, type, out var locations2))
				{
					continue;
				}
				if (locations == null)
				{
					locations = locations2;
					continue;
				}
				if (hashSet == null)
				{
					hashSet = new HashSet<IResourceLocation>();
					foreach (IResourceLocation location in locations)
					{
						hashSet.Add(location);
					}
				}
				hashSet.UnionWith(locations2);
			}
			if (hashSet == null)
			{
				return locations != null;
			}
			locations = new List<IResourceLocation>(hashSet);
			return true;
		}

		internal bool GetResourceLocations(IEnumerable keys, Type type, Addressables.MergeMode merge, out IList<IResourceLocation> locations)
		{
			locations = null;
			HashSet<IResourceLocation> hashSet = null;
			foreach (object key in keys)
			{
				if (GetResourceLocations(key, type, out var locations2))
				{
					if (locations == null)
					{
						locations = locations2;
						if (merge == Addressables.MergeMode.None)
						{
							return true;
						}
						continue;
					}
					if (hashSet == null)
					{
						hashSet = new HashSet<IResourceLocation>(locations, this);
					}
					switch (merge)
					{
					case Addressables.MergeMode.Intersection:
						hashSet.IntersectWith(locations2);
						break;
					case Addressables.MergeMode.Union:
						hashSet.UnionWith(locations2);
						break;
					}
				}
				else if (merge == Addressables.MergeMode.Intersection)
				{
					locations = null;
					return false;
				}
			}
			if (hashSet == null)
			{
				return locations != null;
			}
			if (hashSet.Count == 0)
			{
				locations = null;
				return false;
			}
			locations = new List<IResourceLocation>(hashSet);
			return true;
		}

		public AsyncOperationHandle<IResourceLocator> InitializeAsync(string runtimeDataPath, string providerSuffix = null, bool autoReleaseHandle = true)
		{
			if (hasStartedInitialization)
			{
				if (m_InitializationOperation.IsValid())
				{
					return m_InitializationOperation;
				}
				return ResourceManager.CreateCompletedOperation(m_ResourceLocators[0].Locator, null);
			}
			if (ResourceManager.ExceptionHandler == null)
			{
				ResourceManager.ExceptionHandler = LogException;
			}
			hasStartedInitialization = true;
			if (m_InitializationOperation.IsValid())
			{
				return m_InitializationOperation;
			}
			if (string.IsNullOrEmpty(Application.streamingAssetsPath))
			{
				Addressables.LogWarning("Application.streamingAssetsPath has been stripped!");
			}
			if (string.IsNullOrEmpty(Application.persistentDataPath))
			{
				Addressables.LogWarning("Application.persistentDataPath has been stripped!");
			}
			if (string.IsNullOrEmpty(runtimeDataPath))
			{
				return ResourceManager.CreateCompletedOperation<IResourceLocator>(null, $"Invalid Key: {runtimeDataPath}");
			}
			m_OnHandleCompleteAction = OnHandleCompleted;
			m_OnSceneHandleCompleteAction = OnSceneHandleCompleted;
			m_OnHandleDestroyedAction = OnHandleDestroyed;
			m_InitializationOperation = InitializationOperation.CreateInitializationOperation(this, runtimeDataPath, providerSuffix);
			if (autoReleaseHandle)
			{
				m_InitializationOperation.Completed += delegate(AsyncOperationHandle<IResourceLocator> x)
				{
					ResourceManager.Release(x);
				};
			}
			return m_InitializationOperation;
		}

		public AsyncOperationHandle<IResourceLocator> InitializeAsync()
		{
			string id = RuntimePath + "/settings.json";
			return InitializeAsync(ResolveInternalId(id));
		}

		internal ResourceLocationBase CreateCatalogLocationWithHashDependencies(string catalogPath, string hashFilePath)
		{
			ResourceLocationBase resourceLocationBase = new ResourceLocationBase(catalogPath, catalogPath, typeof(ContentCatalogProvider).FullName, typeof(IResourceLocator));
			if (!string.IsNullOrEmpty(hashFilePath))
			{
				string text = ResolveInternalId("{UnityEngine.Application.persistentDataPath}/com.unity.addressables/" + Path.GetFileName(hashFilePath));
				resourceLocationBase.Dependencies.Add(new ResourceLocationBase(hashFilePath, hashFilePath, typeof(TextDataProvider).FullName, typeof(string)));
				resourceLocationBase.Dependencies.Add(new ResourceLocationBase(text, text, typeof(TextDataProvider).FullName, typeof(string)));
			}
			return resourceLocationBase;
		}

		public AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, bool autoReleaseHandle = true, string providerSuffix = null)
		{
			string hashFilePath = catalogPath.Replace(".json", ".hash");
			ResourceLocationBase loc = CreateCatalogLocationWithHashDependencies(catalogPath, hashFilePath);
			if (ShouldChainRequest)
			{
				return ResourceManager.CreateChainOperation(ChainOperation, (AsyncOperationHandle op) => LoadContentCatalogAsync(catalogPath, autoReleaseHandle, providerSuffix));
			}
			AsyncOperationHandle<IResourceLocator> handle = InitializationOperation.LoadContentCatalog(this, loc, providerSuffix);
			if (autoReleaseHandle)
			{
				handle.Completed += delegate
				{
					Release(handle);
				};
			}
			return handle;
		}

		private AsyncOperationHandle<SceneInstance> TrackHandle(AsyncOperationHandle<SceneInstance> handle)
		{
			handle.Completed += delegate(AsyncOperationHandle<SceneInstance> sceneHandle)
			{
				m_OnSceneHandleCompleteAction(sceneHandle);
			};
			return handle;
		}

		private AsyncOperationHandle<TObject> TrackHandle<TObject>(AsyncOperationHandle<TObject> handle)
		{
			handle.CompletedTypeless += m_OnHandleCompleteAction;
			return handle;
		}

		private AsyncOperationHandle TrackHandle(AsyncOperationHandle handle)
		{
			handle.Completed += m_OnHandleCompleteAction;
			return handle;
		}

		internal void ClearTrackHandles()
		{
			m_resultToHandle.Clear();
		}

		public AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(IResourceLocation location)
		{
			return TrackHandle(ResourceManager.ProvideResource<TObject>(location));
		}

		private AsyncOperationHandle<TObject> LoadAssetWithChain<TObject>(AsyncOperationHandle dep, object key)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => LoadAssetAsync<TObject>(key));
		}

		public AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key)
		{
			if (ShouldChainRequest)
			{
				return TrackHandle(LoadAssetWithChain<TObject>(ChainOperation, key));
			}
			key = EvaluateKey(key);
			Type type = typeof(TObject);
			if (type.IsArray)
			{
				type = type.GetElementType();
			}
			else if (type.IsGenericType && typeof(IList<>) == type.GetGenericTypeDefinition())
			{
				type = type.GetGenericArguments()[0];
			}
			foreach (ResourceLocatorInfo resourceLocator in m_ResourceLocators)
			{
				if (!resourceLocator.Locator.Locate(key, type, out var locations))
				{
					continue;
				}
				foreach (IResourceLocation item in locations)
				{
					if (ResourceManager.GetResourceProvider(typeof(TObject), item) != null)
					{
						return TrackHandle(ResourceManager.ProvideResource<TObject>(item));
					}
				}
			}
			return ResourceManager.CreateCompletedOperation(default(TObject), new InvalidKeyException(key, type).Message);
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsWithChain(AsyncOperationHandle dep, IEnumerable keys, Addressables.MergeMode mode, Type type)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => LoadResourceLocationsAsync(keys, mode, type));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(IEnumerable keys, Addressables.MergeMode mode, Type type = null)
		{
			if (ShouldChainRequest)
			{
				return TrackHandle(LoadResourceLocationsWithChain(ChainOperation, keys, mode, type));
			}
			LoadResourceLocationKeysOp loadResourceLocationKeysOp = new LoadResourceLocationKeysOp();
			loadResourceLocationKeysOp.Init(this, type, keys, mode);
			return TrackHandle(ResourceManager.StartOperation(loadResourceLocationKeysOp, default(AsyncOperationHandle)));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsWithChain(AsyncOperationHandle dep, object key, Type type)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => LoadResourceLocationsAsync(key, type));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(object key, Type type = null)
		{
			if (ShouldChainRequest)
			{
				return TrackHandle(LoadResourceLocationsWithChain(ChainOperation, key, type));
			}
			LoadResourceLocationKeyOp loadResourceLocationKeyOp = new LoadResourceLocationKeyOp();
			loadResourceLocationKeyOp.Init(this, type, key);
			return TrackHandle(ResourceManager.StartOperation(loadResourceLocationKeyOp, default(AsyncOperationHandle)));
		}

		public AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			return TrackHandle(ResourceManager.ProvideResources(locations, releaseDependenciesOnFailure, callback));
		}

		private AsyncOperationHandle<IList<TObject>> LoadAssetsWithChain<TObject>(AsyncOperationHandle dep, IEnumerable keys, Action<TObject> callback, Addressables.MergeMode mode, bool releaseDependenciesOnFailure)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => LoadAssetsAsync(keys, callback, mode, releaseDependenciesOnFailure));
		}

		public AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IEnumerable keys, Action<TObject> callback, Addressables.MergeMode mode, bool releaseDependenciesOnFailure)
		{
			if (ShouldChainRequest)
			{
				return TrackHandle(LoadAssetsWithChain(ChainOperation, keys, callback, mode, releaseDependenciesOnFailure));
			}
			if (!GetResourceLocations(keys, typeof(TObject), mode, out var locations))
			{
				return ResourceManager.CreateCompletedOperation<IList<TObject>>(null, new InvalidKeyException(keys, typeof(TObject)).Message);
			}
			return LoadAssetsAsync(locations, callback, releaseDependenciesOnFailure);
		}

		private AsyncOperationHandle<IList<TObject>> LoadAssetsWithChain<TObject>(AsyncOperationHandle dep, object key, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op2) => LoadAssetsAsync(key, callback, releaseDependenciesOnFailure));
		}

		public AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			if (ShouldChainRequest)
			{
				return TrackHandle(LoadAssetsWithChain(ChainOperation, key, callback, releaseDependenciesOnFailure));
			}
			if (!GetResourceLocations(key, typeof(TObject), out var locations))
			{
				return ResourceManager.CreateCompletedOperation<IList<TObject>>(null, new InvalidKeyException(key, typeof(TObject)).Message);
			}
			return LoadAssetsAsync(locations, callback, releaseDependenciesOnFailure);
		}

		private void OnHandleDestroyed(AsyncOperationHandle handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				m_resultToHandle.Remove(handle.Result);
			}
		}

		private void OnSceneHandleCompleted(AsyncOperationHandle handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				m_SceneInstances.Add(handle);
				if (!m_resultToHandle.ContainsKey(handle.Result))
				{
					handle.Destroyed += m_OnHandleDestroyedAction;
					m_resultToHandle.Add(handle.Result, handle);
				}
			}
		}

		private void OnHandleCompleted(AsyncOperationHandle handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded && !m_resultToHandle.ContainsKey(handle.Result))
			{
				handle.Destroyed += m_OnHandleDestroyedAction;
				m_resultToHandle.Add(handle.Result, handle);
			}
		}

		public void Release<TObject>(TObject obj)
		{
			AsyncOperationHandle value;
			if (obj == null)
			{
				LogWarning("Addressables.Release() - trying to release null object.");
			}
			else if (m_resultToHandle.TryGetValue(obj, out value))
			{
				Release(value);
			}
			else
			{
				LogError("Addressables.Release was called on an object that Addressables was not previously aware of.  Thus nothing is being released");
			}
		}

		public void Release<TObject>(AsyncOperationHandle<TObject> handle)
		{
			m_ResourceManager.Release(handle);
		}

		public void Release(AsyncOperationHandle handle)
		{
			m_ResourceManager.Release(handle);
		}

		private AsyncOperationHandle<long> GetDownloadSizeWithChain(AsyncOperationHandle dep, object key)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => GetDownloadSizeAsync(key));
		}

		private AsyncOperationHandle<long> GetDownloadSizeWithChain(AsyncOperationHandle dep, IEnumerable keys)
		{
			return ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => GetDownloadSizeAsync(keys));
		}

		public AsyncOperationHandle<long> GetDownloadSizeAsync(object key)
		{
			return GetDownloadSizeAsync(new object[1] { key });
		}

		public AsyncOperationHandle<long> GetDownloadSizeAsync(IEnumerable keys)
		{
			if (ShouldChainRequest)
			{
				return TrackHandle(GetDownloadSizeWithChain(ChainOperation, keys));
			}
			List<IResourceLocation> list = new List<IResourceLocation>();
			foreach (object key in keys)
			{
				IList<IResourceLocation> locations;
				if (key is IList<IResourceLocation>)
				{
					locations = key as IList<IResourceLocation>;
				}
				else if (key is IResourceLocation)
				{
					locations = new List<IResourceLocation>(1) { key as IResourceLocation };
				}
				else if (!GetResourceLocations(key, typeof(object), out locations))
				{
					return ResourceManager.CreateCompletedOperation(0L, new InvalidKeyException(key, typeof(object)).Message);
				}
				foreach (IResourceLocation item in locations)
				{
					if (item.HasDependencies)
					{
						list.AddRange(item.Dependencies);
					}
				}
			}
			long num = 0L;
			foreach (IResourceLocation item2 in list.Distinct())
			{
				ILocationSizeData locationSizeData = item2.Data as ILocationSizeData;
				if (locationSizeData != null)
				{
					num += locationSizeData.ComputeSize(item2, ResourceManager);
				}
			}
			return ResourceManager.CreateCompletedOperation(num, string.Empty);
		}

		private AsyncOperationHandle DownloadDependenciesAsyncWithChain(AsyncOperationHandle dep, object key, bool autoReleaseHandle)
		{
			AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle = ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => DownloadDependenciesAsync(key).Convert<IList<IAssetBundleResource>>());
			if (autoReleaseHandle)
			{
				asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
				{
					Release(op);
				};
			}
			return asyncOperationHandle;
		}

		private static List<IResourceLocation> GatherDependenciesFromLocations(IList<IResourceLocation> locations)
		{
			HashSet<IResourceLocation> hashSet = new HashSet<IResourceLocation>();
			foreach (IResourceLocation location in locations)
			{
				if (location.ResourceType == typeof(IAssetBundleResource))
				{
					hashSet.Add(location);
				}
				if (!location.HasDependencies)
				{
					continue;
				}
				foreach (IResourceLocation dependency in location.Dependencies)
				{
					if (dependency.ResourceType == typeof(IAssetBundleResource))
					{
						hashSet.Add(dependency);
					}
				}
			}
			return new List<IResourceLocation>(hashSet);
		}

		public AsyncOperationHandle DownloadDependenciesAsync(object key, bool autoReleaseHandle = false)
		{
			if (ShouldChainRequest)
			{
				return DownloadDependenciesAsyncWithChain(ChainOperation, key, autoReleaseHandle);
			}
			if (!GetResourceLocations(key, typeof(object), out var locations))
			{
				AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle = ResourceManager.CreateCompletedOperation<IList<IAssetBundleResource>>(null, new InvalidKeyException(key, typeof(object)).Message);
				if (autoReleaseHandle)
				{
					asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
					{
						Release(op);
					};
				}
				return asyncOperationHandle;
			}
			AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle2 = LoadAssetsAsync<IAssetBundleResource>(GatherDependenciesFromLocations(locations), null, releaseDependenciesOnFailure: true);
			if (autoReleaseHandle)
			{
				asyncOperationHandle2.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
				{
					Release(op);
				};
			}
			return asyncOperationHandle2;
		}

		private AsyncOperationHandle DownloadDependenciesAsyncWithChain(AsyncOperationHandle dep, IList<IResourceLocation> locations, bool autoReleaseHandle)
		{
			AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle = ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => DownloadDependenciesAsync(locations).Convert<IList<IAssetBundleResource>>());
			if (autoReleaseHandle)
			{
				asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
				{
					Release(op);
				};
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle DownloadDependenciesAsync(IList<IResourceLocation> locations, bool autoReleaseHandle = false)
		{
			if (ShouldChainRequest)
			{
				return DownloadDependenciesAsyncWithChain(ChainOperation, locations, autoReleaseHandle);
			}
			AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle = LoadAssetsAsync<IAssetBundleResource>(GatherDependenciesFromLocations(locations), null, releaseDependenciesOnFailure: true);
			if (autoReleaseHandle)
			{
				asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
				{
					Release(op);
				};
			}
			return asyncOperationHandle;
		}

		private AsyncOperationHandle DownloadDependenciesAsyncWithChain(AsyncOperationHandle dep, IEnumerable keys, Addressables.MergeMode mode, bool autoReleaseHandle)
		{
			AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle = ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => DownloadDependenciesAsync(keys, mode).Convert<IList<IAssetBundleResource>>());
			if (autoReleaseHandle)
			{
				asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
				{
					Release(op);
				};
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle DownloadDependenciesAsync(IEnumerable keys, Addressables.MergeMode mode, bool autoReleaseHandle = false)
		{
			if (ShouldChainRequest)
			{
				return DownloadDependenciesAsyncWithChain(ChainOperation, keys, mode, autoReleaseHandle);
			}
			if (!GetResourceLocations(keys, typeof(object), mode, out var locations))
			{
				AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle = ResourceManager.CreateCompletedOperation<IList<IAssetBundleResource>>(null, new InvalidKeyException(keys, typeof(object)).Message);
				if (autoReleaseHandle)
				{
					asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
					{
						Release(op);
					};
				}
				return asyncOperationHandle;
			}
			AsyncOperationHandle<IList<IAssetBundleResource>> asyncOperationHandle2 = LoadAssetsAsync<IAssetBundleResource>(GatherDependenciesFromLocations(locations), null, releaseDependenciesOnFailure: true);
			if (autoReleaseHandle)
			{
				asyncOperationHandle2.Completed += delegate(AsyncOperationHandle<IList<IAssetBundleResource>> op)
				{
					Release(op);
				};
			}
			return asyncOperationHandle2;
		}

		internal bool ClearDependencyCacheForKey(object key)
		{
			bool flag = true;
			if (key is IResourceLocation && (key as IResourceLocation).HasDependencies)
			{
				foreach (IResourceLocation item in GatherDependenciesFromLocations((key as IResourceLocation).Dependencies))
				{
					if (item.Data is AssetBundleRequestOptions)
					{
						flag = flag && Caching.ClearAllCachedVersions((item.Data as AssetBundleRequestOptions).BundleName);
					}
				}
				return flag;
			}
			if (GetResourceLocations(key, typeof(object), out var locations))
			{
				foreach (IResourceLocation item2 in GatherDependenciesFromLocations(locations))
				{
					if (item2.Data is AssetBundleRequestOptions)
					{
						flag = flag && Caching.ClearAllCachedVersions((item2.Data as AssetBundleRequestOptions).BundleName);
					}
				}
				return flag;
			}
			return flag;
		}

		public AsyncOperationHandle<bool> ClearDependencyCacheAsync(object key, bool autoReleaseHandle)
		{
			if (ShouldChainRequest)
			{
				return ResourceManager.CreateChainOperation(ChainOperation, (AsyncOperationHandle op) => ClearDependencyCacheAsync(key, autoReleaseHandle));
			}
			bool flag = ClearDependencyCacheForKey(key);
			AsyncOperationHandle<bool> asyncOperationHandle = ResourceManager.CreateCompletedOperation(flag, flag ? string.Empty : "Unable to clear the cache.  AssetBundle's may still be loaded for the given key.");
			if (autoReleaseHandle)
			{
				Release(asyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle<bool> ClearDependencyCacheAsync(IList<IResourceLocation> locations, bool autoReleaseHandle)
		{
			if (ShouldChainRequest)
			{
				return ResourceManager.CreateChainOperation(ChainOperation, (AsyncOperationHandle op) => ClearDependencyCacheAsync(locations, autoReleaseHandle));
			}
			bool flag = true;
			foreach (IResourceLocation location in locations)
			{
				flag = flag && ClearDependencyCacheForKey(location);
			}
			AsyncOperationHandle<bool> asyncOperationHandle = ResourceManager.CreateCompletedOperation(flag, flag ? string.Empty : "Unable to clear the cache.  AssetBundle's may still be loaded for the given key(s).");
			if (autoReleaseHandle)
			{
				Release(asyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle<bool> ClearDependencyCacheAsync(IEnumerable keys, bool autoReleaseHandle)
		{
			if (ShouldChainRequest)
			{
				return ResourceManager.CreateChainOperation(ChainOperation, (AsyncOperationHandle op) => ClearDependencyCacheAsync(keys, autoReleaseHandle));
			}
			bool flag = true;
			foreach (object key in keys)
			{
				flag = flag && ClearDependencyCacheForKey(key);
			}
			AsyncOperationHandle<bool> asyncOperationHandle = ResourceManager.CreateCompletedOperation(flag, flag ? string.Empty : "Unable to clear the cache.  AssetBundle's may still be loaded for the given key(s).");
			if (autoReleaseHandle)
			{
				Release(asyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			return InstantiateAsync(location, new InstantiationParameters(parent, instantiateInWorldSpace), trackHandle);
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			return InstantiateAsync(location, new InstantiationParameters(position, rotation, parent), trackHandle);
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			return InstantiateAsync(key, new InstantiationParameters(parent, instantiateInWorldSpace), trackHandle);
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			return InstantiateAsync(key, new InstantiationParameters(position, rotation, parent), trackHandle);
		}

		private AsyncOperationHandle<GameObject> InstantiateWithChain(AsyncOperationHandle dep, object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			AsyncOperationHandle<GameObject> result = ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => InstantiateAsync(key, instantiateParameters, trackHandle: false));
			if (trackHandle)
			{
				result.CompletedTypeless += m_OnHandleCompleteAction;
			}
			return result;
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			if (ShouldChainRequest)
			{
				return InstantiateWithChain(ChainOperation, key, instantiateParameters, trackHandle);
			}
			key = EvaluateKey(key);
			foreach (ResourceLocatorInfo resourceLocator in m_ResourceLocators)
			{
				if (resourceLocator.Locator.Locate(key, typeof(GameObject), out var locations))
				{
					return InstantiateAsync(locations[0], instantiateParameters, trackHandle);
				}
			}
			return ResourceManager.CreateCompletedOperation<GameObject>(null, new InvalidKeyException(key, typeof(GameObject)).Message);
		}

		private AsyncOperationHandle<GameObject> InstantiateWithChain(AsyncOperationHandle dep, IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			AsyncOperationHandle<GameObject> result = ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => InstantiateAsync(location, instantiateParameters, trackHandle: false));
			if (trackHandle)
			{
				result.CompletedTypeless += m_OnHandleCompleteAction;
			}
			return result;
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			if (ShouldChainRequest)
			{
				return InstantiateWithChain(ChainOperation, location, instantiateParameters, trackHandle);
			}
			AsyncOperationHandle<GameObject> result = ResourceManager.ProvideInstance(InstanceProvider, location, instantiateParameters);
			if (!trackHandle)
			{
				return result;
			}
			result.CompletedTypeless += m_OnHandleCompleteAction;
			return result;
		}

		public bool ReleaseInstance(GameObject instance)
		{
			if (instance == null)
			{
				LogWarning("Addressables.ReleaseInstance() - trying to release null object.");
				return false;
			}
			if (m_resultToHandle.TryGetValue(instance, out var value))
			{
				Release(value);
				return true;
			}
			return false;
		}

		internal AsyncOperationHandle<SceneInstance> LoadSceneWithChain(AsyncOperationHandle dep, object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
		{
			return TrackHandle(ResourceManager.CreateChainOperation(dep, (AsyncOperationHandle op) => LoadSceneAsync(key, loadMode, activateOnLoad, priority, trackHandle: false)));
		}

		public AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, bool trackHandle = true)
		{
			if (ShouldChainRequest)
			{
				return LoadSceneWithChain(ChainOperation, key, loadMode, activateOnLoad, priority);
			}
			if (!GetResourceLocations(key, typeof(SceneInstance), out var locations))
			{
				return ResourceManager.CreateCompletedOperation(default(SceneInstance), new InvalidKeyException(key, typeof(SceneInstance)).Message);
			}
			return LoadSceneAsync(locations[0], loadMode, activateOnLoad, priority, trackHandle);
		}

		public AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, bool trackHandle = true)
		{
			AsyncOperationHandle<SceneInstance> asyncOperationHandle = ResourceManager.ProvideScene(SceneProvider, location, loadMode, activateOnLoad, priority);
			if (trackHandle)
			{
				return TrackHandle(asyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance scene, bool autoReleaseHandle = true)
		{
			if (!m_resultToHandle.TryGetValue(scene, out var value))
			{
				string text = $"Addressables.UnloadSceneAsync() - Cannot find handle for scene {scene}";
				LogWarning(text);
				return ResourceManager.CreateCompletedOperation(scene, text);
			}
			if (value.m_InternalOp.IsRunning)
			{
				return CreateUnloadSceneWithChain(value, autoReleaseHandle);
			}
			return UnloadSceneAsync(value, autoReleaseHandle);
		}

		public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle handle, bool autoReleaseHandle = true)
		{
			if (handle.m_InternalOp.IsRunning)
			{
				return CreateUnloadSceneWithChain(handle, autoReleaseHandle);
			}
			return UnloadSceneAsync(handle.Convert<SceneInstance>(), autoReleaseHandle);
		}

		public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle, bool autoReleaseHandle = true)
		{
			if (handle.m_InternalOp.IsRunning)
			{
				return CreateUnloadSceneWithChain(handle, autoReleaseHandle);
			}
			return InternalUnloadScene(handle, autoReleaseHandle);
		}

		internal AsyncOperationHandle<SceneInstance> CreateUnloadSceneWithChain(AsyncOperationHandle handle, bool autoReleaseHandle)
		{
			return m_ResourceManager.CreateChainOperation(handle, (AsyncOperationHandle completedHandle) => InternalUnloadScene(completedHandle.Convert<SceneInstance>(), autoReleaseHandle));
		}

		internal AsyncOperationHandle<SceneInstance> CreateUnloadSceneWithChain(AsyncOperationHandle<SceneInstance> handle, bool autoReleaseHandle)
		{
			return m_ResourceManager.CreateChainOperation(handle, (AsyncOperationHandle<SceneInstance> completedHandle) => InternalUnloadScene(completedHandle, autoReleaseHandle));
		}

		internal AsyncOperationHandle<SceneInstance> InternalUnloadScene(AsyncOperationHandle<SceneInstance> handle, bool autoReleaseHandle)
		{
			AsyncOperationHandle<SceneInstance> result = ResourceManager.ReleaseScene(SceneProvider, handle);
			if (autoReleaseHandle)
			{
				result.Completed += delegate(AsyncOperationHandle<SceneInstance> op)
				{
					Release(op);
				};
			}
			return result;
		}

		private object EvaluateKey(object obj)
		{
			if (obj is IKeyEvaluator)
			{
				return (obj as IKeyEvaluator).RuntimeKey;
			}
			return obj;
		}

		internal AsyncOperationHandle<List<string>> CheckForCatalogUpdates(bool autoReleaseHandle = true)
		{
			if (m_ActiveCheckUpdateOperation.IsValid())
			{
				Release(m_ActiveCheckUpdateOperation);
			}
			m_ActiveCheckUpdateOperation = new CheckCatalogsOperation(this).Start(m_ResourceLocators);
			if (autoReleaseHandle)
			{
				m_ActiveCheckUpdateOperation.CompletedTypeless += delegate(AsyncOperationHandle o)
				{
					ResourceManager.Release(o);
				};
			}
			return m_ActiveCheckUpdateOperation;
		}

		internal ResourceLocatorInfo GetLocatorInfo(string c)
		{
			foreach (ResourceLocatorInfo resourceLocator in m_ResourceLocators)
			{
				if (resourceLocator.Locator.LocatorId == c)
				{
					return resourceLocator;
				}
			}
			return null;
		}

		internal AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(IEnumerable<string> catalogIds = null, bool autoReleaseHandle = true)
		{
			if (m_ActiveUpdateOperation.IsValid())
			{
				return m_ActiveUpdateOperation;
			}
			if (catalogIds == null && !CatalogsWithAvailableUpdates.Any())
			{
				return m_ResourceManager.CreateChainOperation(CheckForCatalogUpdates(), (AsyncOperationHandle<List<string>> depOp) => UpdateCatalogs(CatalogsWithAvailableUpdates, autoReleaseHandle));
			}
			AsyncOperationHandle<List<IResourceLocator>> result = new UpdateCatalogsOperation(this).Start((catalogIds == null) ? CatalogsWithAvailableUpdates : catalogIds);
			if (autoReleaseHandle)
			{
				result.CompletedTypeless += delegate(AsyncOperationHandle o)
				{
					ResourceManager.Release(o);
				};
			}
			return result;
		}

		public bool Equals(IResourceLocation x, IResourceLocation y)
		{
			if (x.PrimaryKey.Equals(y.PrimaryKey) && x.ResourceType.Equals(y.ResourceType))
			{
				return x.InternalId.Equals(y.InternalId);
			}
			return false;
		}

		public int GetHashCode(IResourceLocation loc)
		{
			return loc.PrimaryKey.GetHashCode() * 31 + loc.ResourceType.GetHashCode();
		}
	}
}
