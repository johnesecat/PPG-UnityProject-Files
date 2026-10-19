public struct WorkshopItem
{
	public string Name;

	public ulong FileId;

	public string Path;

	public WorkshopItem(string name, ulong fileId, string path)
	{
		Name = name;
		FileId = fileId;
		Path = path;
	}
}
