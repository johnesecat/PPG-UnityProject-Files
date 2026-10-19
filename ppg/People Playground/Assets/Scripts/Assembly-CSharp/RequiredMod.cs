using System.Runtime.CompilerServices;

public struct RequiredMod
{
	public string Name;

	public string WorkshopId;

	public string UniqueIdentity;

	public bool HasWorkshopId
	{
		[IsReadOnly]
		get
		{
			return !string.IsNullOrWhiteSpace(WorkshopId);
		}
	}
}
