using System;
using System.Collections;

namespace Novell.Directory.Ldap.Utilclass
{
	public class ArrayEnumeration : IEnumerator
	{
		private object tempAuxObj;

		private object[] eArray;

		private int index;

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

		public ArrayEnumeration(object[] eArray)
		{
			this.eArray = eArray;
		}

		public bool hasMoreElements()
		{
			if (eArray == null)
			{
				return false;
			}
			return index < eArray.Length;
		}

		public object nextElement()
		{
			if (eArray == null || index >= eArray.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			return eArray[index++];
		}
	}
}
