public class ChainBehaviour : DistanceJointWireBehaviour
{
	protected override void Start()
	{
		base.Start();
		lineRenderer.numCapVertices = 0;
	}

	public override int GetVertexCount()
	{
		return 12;
	}
}
