using UnityEngine;

public interface IUndoableAction
{
	void Undo();

	void Redo();

	bool IsValid();

	bool IsRelatedTo(Object o);
}
