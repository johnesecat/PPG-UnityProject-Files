using System;

[Serializable]
public class MigrationEvent
{
	public enum Condition
	{
		OlderThan,
		OlderThanOrEqualTo,
		EqualTo,
		NewerThan,
		NewerThanOrEqualTo
	}

	public string Version = "1.27.5";

	public Condition VersionCondition;

	public SpawnableAsset ToSpawnInstead;

	public bool IsApplicable(string originVersion)
	{
		if (ToSpawnInstead == null)
		{
			return false;
		}
		int difference = GameVersion.GetDifference(originVersion, Version);
		switch (VersionCondition)
		{
		case Condition.OlderThan:
			if (difference >= 0)
			{
				return false;
			}
			break;
		case Condition.OlderThanOrEqualTo:
			if (difference > 0)
			{
				return false;
			}
			break;
		case Condition.EqualTo:
			if (difference != 0)
			{
				return false;
			}
			break;
		case Condition.NewerThan:
			if (difference <= 0)
			{
				return false;
			}
			break;
		case Condition.NewerThanOrEqualTo:
			if (difference < 0)
			{
				return false;
			}
			break;
		}
		return true;
	}
}
