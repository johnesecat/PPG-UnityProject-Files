using UnityEngine;

public class HeatPipeBehaviour : DistanceJointWireBehaviour
{
	private MaterialPropertyBlock propertyBlock;

	private float fakeCableTemperature;

	protected override void Start()
	{
		base.Start();
		propertyBlock = new MaterialPropertyBlock();
		lineRenderer.GetPropertyBlock(propertyBlock);
	}

	public virtual float GetHeatTransferFactor()
	{
		return 0.2f;
	}

	protected override void Tick()
	{
		if ((bool)otherPhysicalBehaviour && (bool)physicalBehaviour && otherPhysicalBehaviour.SimulateTemperature && physicalBehaviour.SimulateTemperature)
		{
			Utils.AverageTemperature(physicalBehaviour, otherPhysicalBehaviour, GetHeatTransferFactor());
			fakeCableTemperature = 0.5f * (physicalBehaviour.Temperature + otherPhysicalBehaviour.Temperature);
		}
		fakeCableTemperature = Mathf.Lerp(fakeCableTemperature, PhysicalBehaviour.AmbientTemperature, GetHeatTransferFactor());
	}

	private void OnWillRenderObject()
	{
		float num = Mathf.Clamp01(Utils.MapRange(0f, 1000f, 0f, 1f, fakeCableTemperature));
		propertyBlock.SetFloat(ShaderProperties.Get("_Progress"), num * num);
		lineRenderer.SetPropertyBlock(propertyBlock);
	}

	public override int GetVertexCount()
	{
		return 1;
	}
}
