public static class CloneUtil
{
	public static T ShallowClone<T>(this T obj)
	{
		return CloneUtil<T>.ShallowClone(obj);
	}
}
