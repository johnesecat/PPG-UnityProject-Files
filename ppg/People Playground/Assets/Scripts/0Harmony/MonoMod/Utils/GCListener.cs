using System;

namespace MonoMod.Utils
{
	internal static class GCListener
	{
		private sealed class CollectionDummy
		{
			~CollectionDummy()
			{
				Unloading |= AppDomain.CurrentDomain.IsFinalizingForUnload() || Environment.HasShutdownStarted;
				if (!Unloading)
				{
					GC.ReRegisterForFinalize(this);
				}
				GCListener.OnCollect?.Invoke();
			}
		}

		private static bool Unloading;

		public static event Action OnCollect;

		static GCListener()
		{
			new CollectionDummy();
		}
	}
}
