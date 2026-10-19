using UnityEngine;

public class BoundingBoxBehaviour : MonoBehaviour
{
	public Bounds BoundingBox;

	public float Extend = 50f;

	private void Start()
	{
		Global.main.CameraControlBehaviour.BoundingBox = BoundingBox;
		Global.main.CameraControlBehaviour.Extend = Extend;
	}
}
