using UnityEngine;

public class CameraSyncBehaviour : MonoBehaviour
{
	public Camera Master;

	public Camera Slave;

	public bool OrthographicSizeIsZPosition;

	private void OnPreRender()
	{
		Transform transform = Master.transform;
		Slave.transform.SetPositionAndRotation(transform.position, transform.rotation);
		if (OrthographicSizeIsZPosition)
		{
			Vector3 position = Slave.transform.position;
			position.z = 0f - Master.orthographicSize;
			Slave.transform.position = position;
		}
		else
		{
			Slave.orthographicSize = Master.orthographicSize;
		}
	}
}
