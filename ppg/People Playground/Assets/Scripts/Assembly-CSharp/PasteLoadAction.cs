using System;
using System.Collections.Generic;
using UnityEngine;

public class PasteLoadAction : IUndoableAction
{
	private string displayName;

	public List<UnityEngine.Object> RelevantObjects { get; private set; }

	public PasteLoadAction(IEnumerable<UnityEngine.Object> relevant, string displayName)
	{
		RelevantObjects = new List<UnityEngine.Object>(relevant);
		this.displayName = displayName;
	}

	public bool IsValid()
	{
		return RelevantObjects != null;
	}

	public void Redo()
	{
		throw new NotImplementedException();
	}

	public void Undo()
	{
		foreach (UnityEngine.Object relevantObject in RelevantObjects)
		{
			UnityEngine.Object.Destroy(relevantObject);
		}
		displayName = displayName.ToUpper();
		NotificationControllerBehaviour.Show("<b>" + displayName + "</b> removed");
	}

	public bool IsRelatedTo(UnityEngine.Object o)
	{
		return RelevantObjects.Contains(o);
	}
}
