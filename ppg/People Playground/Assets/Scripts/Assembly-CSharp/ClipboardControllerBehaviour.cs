using UnityEngine;

public class ClipboardControllerBehaviour : MonoBehaviour
{
	public ObjectState[] ObjectStateInClipboard { get; set; }

	public static ClipboardControllerBehaviour Main { get; private set; }

	private void Awake()
	{
		Main = this;
	}
}
