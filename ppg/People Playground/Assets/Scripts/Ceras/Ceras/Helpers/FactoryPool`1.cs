using System;
using System.Collections.Generic;

namespace Ceras.Helpers
{
	internal class FactoryPool<T> : IFactoryPool
	{
		private readonly Func<T> _factoryMethod;

		private readonly Stack<T> _objects = new Stack<T>();

		public int StartSize { get; }

		public int Capacity { get; set; }

		public int Available
		{
			get
			{
				lock (_objects)
				{
					return _objects.Count;
				}
			}
		}

		public Type ElementType => typeof(T);

		public FactoryPool(Func<T> factoryMethod, int startSize = 0)
		{
			_factoryMethod = factoryMethod;
			StartSize = startSize;
			for (int i = 0; i < startSize; i++)
			{
				ReturnObject(factoryMethod());
			}
		}

		public T RentObject()
		{
			lock (_objects)
			{
				if (_objects.Count > 0)
				{
					return _objects.Pop();
				}
				T result = _factoryMethod();
				Capacity++;
				return result;
			}
		}

		public void ReturnObject(T objectToReturn)
		{
			lock (_objects)
			{
				_objects.Push(objectToReturn);
			}
		}

		public void TrimPool()
		{
			lock (_objects)
			{
				while (_objects.Count > StartSize)
				{
					_objects.Pop();
					Capacity--;
				}
			}
		}
	}
}
