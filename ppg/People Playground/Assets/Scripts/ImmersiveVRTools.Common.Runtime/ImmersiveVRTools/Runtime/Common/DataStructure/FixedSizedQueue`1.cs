using System.Collections.Generic;

namespace ImmersiveVRTools.Runtime.Common.DataStructure
{
	public class FixedSizedQueue<T> : Queue<T>
	{
		public int Size { get; private set; }

		public FixedSizedQueue(int size)
		{
			Size = size;
		}

		public new void Enqueue(T obj)
		{
			base.Enqueue(obj);
			while (base.Count > Size)
			{
				Dequeue();
			}
		}
	}
}
