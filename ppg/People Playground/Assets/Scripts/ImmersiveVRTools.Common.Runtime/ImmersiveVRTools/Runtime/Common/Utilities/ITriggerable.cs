namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public interface ITriggerable
	{
		string TriggerName { get; }

		void Trigger();
	}
}
