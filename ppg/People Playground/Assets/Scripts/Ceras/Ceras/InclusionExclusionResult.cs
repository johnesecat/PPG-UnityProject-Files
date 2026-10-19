namespace Ceras
{
	public struct InclusionExclusionResult
	{
		public readonly bool IsIncluded;

		public readonly string Reason;

		public InclusionExclusionResult(bool isIncluded, string reason)
		{
			IsIncluded = isIncluded;
			Reason = reason;
		}
	}
}
