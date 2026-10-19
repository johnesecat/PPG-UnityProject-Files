using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public class SceneProvider : ISceneProvider
	{
		private class SceneOp : AsyncOperationBase<SceneInstance>, IUpdateReceiver
		{
			private bool m_ActivateOnLoad;

			private SceneInstance m_Inst;

			private IResourceLocation m_Location;

			private LoadSceneMode m_LoadMode;

			private int m_Priority;

			private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;

			private ResourceManager m_ResourceManager;

			protected override string DebugName => string.Format("Scene({0})", (m_Location == null) ? "Invalid" : AsyncOperationBase<SceneInstance>.ShortenPath(m_ResourceManager.TransformInternalId(m_Location), keepExtension: false));

			protected override float Progress
			{
				get
				{
					float num = 0.9f;
					float num2 = 0.1f;
					float num3 = 0f;
					if (m_Inst.m_Operation != null)
					{
						num3 += m_Inst.m_Operation.progress * num2;
					}
					if (!m_DepOp.IsDone)
					{
						return num3 + m_DepOp.PercentComplete * num;
					}
					return num3 + num;
				}
			}

			public SceneOp(ResourceManager rm)
			{
				m_ResourceManager = rm;
			}

			internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
			{
				if (!m_DepOp.IsValid())
				{
					DownloadStatus result = default(DownloadStatus);
					result.IsDone = base.IsDone;
					return result;
				}
				return m_DepOp.InternalGetDownloadStatus(visited);
			}

			public void Init(IResourceLocation location, LoadSceneMode loadMode, bool activateOnLoad, int priority, AsyncOperationHandle<IList<AsyncOperationHandle>> depOp)
			{
				m_DepOp = depOp;
				if (m_DepOp.IsValid())
				{
					m_DepOp.Acquire();
				}
				m_Location = location;
				m_LoadMode = loadMode;
				m_ActivateOnLoad = activateOnLoad;
				m_Priority = priority;
			}

			protected override void GetDependencies(List<AsyncOperationHandle> deps)
			{
				if (m_DepOp.IsValid())
				{
					deps.Add(m_DepOp);
				}
			}

			protected override void Execute()
			{
				bool loadingFromBundle = false;
				if (m_DepOp.IsValid())
				{
					foreach (AsyncOperationHandle item in m_DepOp.Result)
					{
						IAssetBundleResource assetBundleResource = item.Result as IAssetBundleResource;
						if (assetBundleResource != null && assetBundleResource.GetAssetBundle() != null)
						{
							loadingFromBundle = true;
						}
					}
				}
				m_Inst = InternalLoadScene(m_Location, loadingFromBundle, m_LoadMode, m_ActivateOnLoad, m_Priority);
				((IUpdateReceiver)this).Update(0f);
				if (!base.IsDone)
				{
					m_ResourceManager.AddUpdateReceiver(this);
				}
			}

			internal SceneInstance InternalLoadScene(IResourceLocation location, bool loadingFromBundle, LoadSceneMode loadMode, bool activateOnLoad, int priority)
			{
				string path = m_ResourceManager.TransformInternalId(location);
				AsyncOperation asyncOperation = InternalLoad(path, loadingFromBundle, loadMode);
				asyncOperation.allowSceneActivation = activateOnLoad;
				asyncOperation.priority = priority;
				SceneInstance result = default(SceneInstance);
				result.m_Operation = asyncOperation;
				result.Scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
				return result;
			}

			private AsyncOperation InternalLoad(string path, bool loadingFromBundle, LoadSceneMode mode)
			{
				return SceneManager.LoadSceneAsync(path, new LoadSceneParameters
				{
					loadSceneMode = mode
				});
			}

			protected override void Destroy()
			{
				if (m_DepOp.IsValid())
				{
					m_DepOp.Release();
				}
				base.Destroy();
			}

			void IUpdateReceiver.Update(float unscaledDeltaTime)
			{
				if (m_Inst.m_Operation.isDone || (!m_ActivateOnLoad && m_Inst.m_Operation.progress == 0.9f))
				{
					m_ResourceManager.RemoveUpdateReciever(this);
					Complete(m_Inst, success: true, null);
				}
			}
		}

		private class UnloadSceneOp : AsyncOperationBase<SceneInstance>
		{
			private SceneInstance m_Instance;

			private AsyncOperationHandle<SceneInstance> m_sceneLoadHandle;

			protected override float Progress => m_sceneLoadHandle.PercentComplete;

			public void Init(AsyncOperationHandle<SceneInstance> sceneLoadHandle)
			{
				if (sceneLoadHandle.ReferenceCount > 0)
				{
					m_sceneLoadHandle = sceneLoadHandle;
					m_Instance = m_sceneLoadHandle.Result;
				}
			}

			protected override void Execute()
			{
				if (m_sceneLoadHandle.IsValid() && m_Instance.Scene.isLoaded)
				{
					AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(m_Instance.Scene);
					if (asyncOperation == null)
					{
						UnloadSceneCompleted(null);
					}
					else
					{
						asyncOperation.completed += UnloadSceneCompleted;
					}
				}
				else
				{
					UnloadSceneCompleted(null);
				}
			}

			private void UnloadSceneCompleted(AsyncOperation obj)
			{
				Complete(m_Instance, success: true, "");
				if (m_sceneLoadHandle.IsValid())
				{
					m_sceneLoadHandle.Release();
				}
			}
		}

		public AsyncOperationHandle<SceneInstance> ProvideScene(ResourceManager resourceManager, IResourceLocation location, LoadSceneMode loadMode, bool activateOnLoad, int priority)
		{
			AsyncOperationHandle<IList<AsyncOperationHandle>> asyncOperationHandle = default(AsyncOperationHandle<IList<AsyncOperationHandle>>);
			if (location.HasDependencies)
			{
				asyncOperationHandle = resourceManager.ProvideResourceGroupCached(location.Dependencies, location.DependencyHashCode, typeof(IAssetBundleResource), null);
			}
			SceneOp sceneOp = new SceneOp(resourceManager);
			sceneOp.Init(location, loadMode, activateOnLoad, priority, asyncOperationHandle);
			AsyncOperationHandle<SceneInstance> result = resourceManager.StartOperation(sceneOp, asyncOperationHandle);
			if (asyncOperationHandle.IsValid())
			{
				asyncOperationHandle.Release();
			}
			return result;
		}

		public AsyncOperationHandle<SceneInstance> ReleaseScene(ResourceManager resourceManager, AsyncOperationHandle<SceneInstance> sceneLoadHandle)
		{
			UnloadSceneOp unloadSceneOp = new UnloadSceneOp();
			unloadSceneOp.Init(sceneLoadHandle);
			return resourceManager.StartOperation(unloadSceneOp, sceneLoadHandle);
		}
	}
}
