using System;
using UnityEngine;

[Obsolete]
public class FreezeObjectAction : IUndoableAction
{
	private string displayName;

	public GameObject RelevantObject { get; private set; }

	public FreezeObjectAction(GameObject relevant, string displayName)
	{
		RelevantObject = relevant;
		this.displayName = displayName;
	}

	public bool IsRelatedTo(UnityEngine.Object o)
	{
		return o == RelevantObject;
	}

	public bool IsValid()
	{
		return RelevantObject != null;
	}

	public void Redo()
	{
		throw new NotImplementedException();
	}

	public void Undo()
	{
		if ((bool)RelevantObject.GetComponent<PhysicalBehaviour>())
		{
			FreezeBehaviour component = RelevantObject.GetComponent<FreezeBehaviour>();
			if ((bool)component)
			{
				NotificationControllerBehaviour.Show("<b>" + displayName + "</b> unfrozen again");
				UnityEngine.Object.Destroy(component);
			}
			else
			{
				NotificationControllerBehaviour.Show("<b>" + displayName + "</b> frozen again");
				RelevantObject.AddComponent<FreezeBehaviour>();
			}
		}
	}
}
