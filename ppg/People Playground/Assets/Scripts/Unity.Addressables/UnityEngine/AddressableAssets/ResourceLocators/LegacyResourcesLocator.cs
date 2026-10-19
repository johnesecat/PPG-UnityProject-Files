using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets.ResourceLocators
{
	public class LegacyResourcesLocator : IResourceLocator
	{
		public IEnumerable<object> Keys => null;

		public string LocatorId => "LegacyResourcesLocator";

		public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
		{
			locations = null;
			string text = key as string;
			if (text == null)
			{
				return false;
			}
			locations = new List<IResourceLocation>();
			locations.Add(new ResourceLocationBase("LegacyResourceLocation", text, typeof(LegacyResourcesProvider).FullName, typeof(Object)));
			return true;
		}
	}
}
