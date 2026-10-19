using UnityEngine;

public class SpawnableSelection
{
	public SpawnableAsset SpawnableAsset;

	public Contraption Contraption;

	public ContraptionMetaData ContraptionMetaData;

	public string Name
	{
		get
		{
			if ((bool)SpawnableAsset)
			{
				return SpawnableAsset.name;
			}
			if (Contraption != null && ContraptionMetaData != null)
			{
				return ContraptionMetaData.Name;
			}
			Debug.LogWarning("Attempt to retrieve name of unset SpawnableSelection");
			return "Unknown";
		}
	}
}
