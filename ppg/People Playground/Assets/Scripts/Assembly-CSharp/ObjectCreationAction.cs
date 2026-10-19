using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCreationAction : IUndoableAction
{
	private string displayName;

	public HashSet<UnityEngine.Object> RelevantObjects { get; set; }

	public ObjectCreationAction(UnityEngine.Object relevant, string displayName)
	{
		RelevantObjects = new HashSet<UnityEngine.Object>();
		RelevantObjects.Add(relevant);
		this.displayName = displayName;
	}

	public ObjectCreationAction(UnityEngine.Object relevant)
	{
		RelevantObjects = new HashSet<UnityEngine.Object>();
		RelevantObjects.Add(relevant);
		displayName = relevant.name;
	}

	public bool IsValid()
	{
		if (RelevantObjects != null)
		{
			return RelevantObjects.Any();
		}
		return false;
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
