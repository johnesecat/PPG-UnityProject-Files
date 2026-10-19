using System;

namespace Linefy
{
	[Serializable]
	public struct ModificationInfo
	{
		public string name;

		public string date;

		public int hash;

		public ModificationInfo(string name)
		{
			this.name = name;
			date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
			hash = (date + name).GetHashCode();
		}
	}
}
