namespace HarmonyLib
{
	public class Traverse<T>
	{
		private readonly Traverse traverse;

		public T Value
		{
			get
			{
				return traverse.GetValue<T>();
			}
			set
			{
				traverse.SetValue(value);
			}
		}

		private Traverse()
		{
		}

		public Traverse(Traverse traverse)
		{
			this.traverse = traverse;
		}
	}
}
