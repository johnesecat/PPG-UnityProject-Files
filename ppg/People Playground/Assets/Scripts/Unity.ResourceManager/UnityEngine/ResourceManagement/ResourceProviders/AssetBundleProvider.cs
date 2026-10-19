using System;
using System.ComponentModel;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("AssetBundle Provider")]
	public class AssetBundleProvider : ResourceProviderBase
	{
		public override void Provide(ProvideHandle providerInterface)
		{
			new AssetBundleResource().Start(providerInterface);
		}

		public override Type GetDefaultType(IResourceLocation location)
		{
			return typeof(IAssetBundleResource);
		}

		public override void Release(IResourceLocation location, object asset)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			if (asset == null)
			{
				Debug.LogWarningFormat("Releasing null asset bundle from location {0}.  This is an indication that the bundle failed to load.", location);
			}
			else
			{
				(asset as AssetBundleResource)?.Unload();
			}
		}
	}
}
