using System;

internal class SortAttribute : Attribute
{
	public int Order;

	public SortAttribute(int order)
	{
		Order = order;
	}
}
