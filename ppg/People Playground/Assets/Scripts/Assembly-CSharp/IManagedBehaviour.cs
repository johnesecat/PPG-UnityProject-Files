public interface IManagedBehaviour
{
	void ManagedUpdate();

	void ManagedFixedUpdate();

	void ManagedLateUpdate();

	bool ShouldUpdate();
}
