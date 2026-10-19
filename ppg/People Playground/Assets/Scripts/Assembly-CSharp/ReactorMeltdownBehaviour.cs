using UnityEngine;

public class ReactorMeltdownBehaviour : MonoBehaviour
{
	public float SetTemperature = 5000f;

	public float SetCharge = 12f;

	public LayerMask SparkHitLayers;

	public bool MeltdownActive;

	public float AmbientHeatupMax = 400f;

	public float AmbientHeatupSpeed = 0.5f;

	public void Explode()
	{
		MeltdownActive = true;
		CameraShakeBehaviour.main.Shake(3000f, base.transform.position, 0.1f);
		MapLightBehaviour[] componentsInChildren = base.transform.root.GetComponentsInChildren<MapLightBehaviour>();
		foreach (MapLightBehaviour obj in componentsInChildren)
		{
			obj.DeactivateInstantly();
			obj.gameObject.SetActive(value: false);
		}
		foreach (PhysicalBehaviour item in Global.main.PhysicalObjectsInWorld)
		{
			if (!(item == null) && !item.gameObject.isStatic && SparkHitLayers.HasLayer(item.gameObject.layer))
			{
				if (item.SimulateTemperature)
				{
					item.Temperature = Mathf.Max(SetTemperature, item.Temperature);
				}
				item.Charge = Mathf.Max(SetCharge, item.Charge);
				item.rigidbody.AddForceAtPosition(Random.insideUnitCircle * 10f, item.rigidbody.GetRelativePoint(item.GetRandomGridPoint()), ForceMode2D.Impulse);
				item.SendMessage("Break", Random.insideUnitCircle * 4f, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void FixedUpdate()
	{
		if (MeltdownActive && PhysicalBehaviour.AmbientTemperature < AmbientHeatupMax)
		{
			PhysicalBehaviour.AmbientTemperature += Time.fixedDeltaTime * AmbientHeatupSpeed;
		}
	}
}
