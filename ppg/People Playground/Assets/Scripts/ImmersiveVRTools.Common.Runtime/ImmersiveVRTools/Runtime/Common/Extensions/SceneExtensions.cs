using System.Reflection;
using UnityEngine.SceneManagement;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class SceneExtensions
	{
		private static PropertyInfo _loadingStatePropertyInfo = typeof(Scene).GetProperty("loadingState", BindingFlags.Instance | BindingFlags.NonPublic);

		public static SceneLoadingState GetSceneLoadingState(this Scene scene)
		{
			return (SceneLoadingState)_loadingStatePropertyInfo.GetValue(scene);
		}

		public static bool IsSceneLoadedOrLoading(this Scene scene)
		{
			return scene.GetSceneLoadingState() != SceneLoadingState.NotLoaded;
		}
	}
}
