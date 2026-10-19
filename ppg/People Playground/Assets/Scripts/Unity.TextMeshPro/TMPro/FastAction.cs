using System;
using System.Collections.Generic;

namespace TMPro
{
	public class FastAction
	{
		private LinkedList<Action> delegates = new LinkedList<Action>();

		private Dictionary<Action, LinkedListNode<Action>> lookup = new Dictionary<Action, LinkedListNode<Action>>();

		public void Add(Action rhs)
		{
			if (!lookup.ContainsKey(rhs))
			{
				lookup[rhs] = delegates.AddLast(rhs);
			}
		}

		public void Remove(Action rhs)
		{
			if (lookup.TryGetValue(rhs, out var value))
			{
				lookup.Remove(rhs);
				delegates.Remove(value);
			}
		}

		public void Call()
		{
			for (LinkedListNode<Action> linkedListNode = delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value();
			}
		}
	}
}
