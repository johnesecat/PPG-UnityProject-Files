using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets
{
	internal class CheckCatalogsOperation : AsyncOperationBase<List<string>>
	{
		private AddressablesImpl m_Addressables;

		private List<string> m_LocalHashes;

		private List<AddressablesImpl.ResourceLocatorInfo> m_LocatorInfos;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;

		public CheckCatalogsOperation(AddressablesImpl aa)
		{
			m_Addressables = aa;
		}

		public AsyncOperationHandle<List<string>> Start(List<AddressablesImpl.ResourceLocatorInfo> locatorInfos)
		{
			m_LocatorInfos = new List<AddressablesImpl.ResourceLocatorInfo>(locatorInfos.Count);
			m_LocalHashes = new List<string>(locatorInfos.Count);
			List<IResourceLocation> list = new List<IResourceLocation>(locatorInfos.Count);
			foreach (AddressablesImpl.ResourceLocatorInfo locatorInfo in locatorInfos)
			{
				if (locatorInfo.CanUpdateContent)
				{
					list.Add(locatorInfo.HashLocation);
					m_LocalHashes.Add(locatorInfo.LocalHash);
					m_LocatorInfos.Add(locatorInfo);
				}
			}
			ContentCatalogProvider contentCatalogProvider = m_Addressables.ResourceManager.ResourceProviders.FirstOrDefault((IResourceProvider rp) => rp.GetType() == typeof(ContentCatalogProvider)) as ContentCatalogProvider;
			if (contentCatalogProvider != null)
			{
				contentCatalogProvider.DisableCatalogUpdateOnStart = false;
			}
			m_DepOp = m_Addressables.ResourceManager.CreateGroupOperation<string>(list);
			return m_Addressables.ResourceManager.StartOperation(this, m_DepOp);
		}

		protected override void Destroy()
		{
			m_Addressables.Release(m_DepOp);
		}

		protected override void GetDependencies(List<AsyncOperationHandle> dependencies)
		{
			dependencies.Add(m_DepOp);
		}

		protected override void Execute()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < m_DepOp.Result.Count; i++)
			{
				string text = m_DepOp.Result[i].Result as string;
				if (!string.IsNullOrEmpty(text) && text != m_LocalHashes[i])
				{
					list.Add(m_LocatorInfos[i].Locator.LocatorId);
					m_LocatorInfos[i].ContentUpdateAvailable = true;
				}
			}
			Complete(list, success: true, null);
		}
	}
}
