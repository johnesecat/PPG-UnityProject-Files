using UnityEngine;

public class OnlyExistWith : MonoBehaviour
{
	[SkipSerialisation]
	public Component Other;

	[SkipSerialisation]
	public Component AlsoDelete;

	private void Update()
	{
		if (!Other)
		{
			Object.Destroy(AlsoDelete);
			Object.Destroy(this);
		}
	}
}
