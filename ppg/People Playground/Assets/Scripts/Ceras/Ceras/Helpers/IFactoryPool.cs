using System;

namespace Ceras.Helpers
{
	internal interface IFactoryPool
	{
		int StartSize { get; }

		int Capacity { get; }

		int Available { get; }

		Type ElementType { get; }

		void TrimPool();
	}
}
