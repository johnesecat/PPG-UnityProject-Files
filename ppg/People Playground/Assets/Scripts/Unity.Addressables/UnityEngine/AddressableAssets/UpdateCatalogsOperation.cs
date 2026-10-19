using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets
{
	internal class UpdateCatalogsOperation : AsyncOperationBase<List<IResourceLocator>>
	{
		private AddressablesImpl m_Addressables;

		private List<AddressablesImpl.ResourceLocatorInfo> m_LocatorInfos;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;

		public UpdateCatalogsOperation(AddressablesImpl aa)
		{
			m_Addressables = aa;
		}

		public AsyncOperationHandle<List<IResourceLocator>> Start(IEnumerable<string> catalogIds)
		{
			m_LocatorInfos = new List<AddressablesImpl.ResourceLocatorInfo>();
			List<IResourceLocation> list = new List<IResourceLocation>();
			foreach (string catalogId in catalogIds)
			{
				AddressablesImpl.ResourceLocatorInfo locatorInfo = m_Addressables.GetLocatorInfo(catalogId);
				list.Add(locatorInfo.CatalogLocation);
				m_LocatorInfos.Add(locatorInfo);
			}
			if (list.Count == 0)
			{
				return m_Addressables.ResourceManager.CreateCompletedOperation<List<IResourceLocator>>(null, "Content update not available.");
			}
			ContentCatalogProvider contentCatalogProvider = m_Addressables.ResourceManager.ResourceProviders.FirstOrDefault((IResourceProvider rp) => rp.GetType() == typeof(ContentCatalogProvider)) as ContentCatalogProvider;
			if (contentCatalogProvider != null)
			{
				contentCatalogProvider.DisableCatalogUpdateOnStart = false;
			}
			m_DepOp = m_Addressables.ResourceManager.CreateGroupOperation<object>(list);
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
			List<IResourceLocator> list = new List<IResourceLocator>(m_DepOp.Result.Count);
			for (int i = 0; i < m_DepOp.Result.Count; i++)
			{
				IResourceLocator resourceLocator = m_DepOp.Result[i].Result as IResourceLocator;
				string hash = null;
				IResourceLocation loc = null;
				if (resourceLocator == null)
				{
					ContentCatalogData obj = m_DepOp.Result[i].Result as ContentCatalogData;
					resourceLocator = obj.CreateCustomLocator(obj.location.PrimaryKey);
					hash = obj.localHash;
					loc = obj.location;
				}
				m_LocatorInfos[i].UpdateContent(resourceLocator, hash, loc);
				list.Add(m_LocatorInfos[i].Locator);
			}
			Complete(list, success: true, null);
		}
	}
}
