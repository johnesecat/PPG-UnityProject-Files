using System.Collections;

namespace Novell.Directory.Ldap.Utilclass
{
	public class EnumeratedIterator : IEnumerator
	{
		private object tempAuxObj;

		private IEnumerator i;

		public virtual object Current => tempAuxObj;

		public virtual bool MoveNext()
		{
			bool num = hasMoreElements();
			if (num)
			{
				tempAuxObj = nextElement();
			}
			return num;
		}

		public virtual void Reset()
		{
			tempAuxObj = null;
		}

		public EnumeratedIterator(IEnumerator iterator)
		{
			i = iterator;
		}

		public bool hasMoreElements()
		{
			return i.MoveNext();
		}

		public object nextElement()
		{
			return i.Current;
		}
	}
}
