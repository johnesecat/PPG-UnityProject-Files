using UnityEngine.Events;

public interface IUseEmitter
{
	UnityEvent<ActivationPropagation> OnSingleUse { get; }

	UnityEvent<ActivationPropagation> OnContinuousUse { get; }
}
