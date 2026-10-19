using UnityEngine;

public class TemperatureTargetRectangleBehaviour : MonoBehaviour
{
	public float TargetAmbientTemperature = 975f;

	public Vector3Int Size;

	public Vector3Int Offset;

	[Min(0f)]
	public float TransferRateMultiplier = 1f;

	private AmbientTemperatureGridBehaviour Grid;

	private void Start()
	{
		Grid = AmbientTemperatureGridBehaviour.Instance;
	}

	private void FixedUpdate()
	{
		if (!UserPreferenceManager.Current.AmbientTemperatureTransfer)
		{
			return;
		}
		float num = (float)Size.x / 2f;
		float num2 = (float)Size.y / 2f;
		Vector3 vector = base.transform.position + Offset;
		int gridSize = Grid.GridSize;
		for (int i = 0; i < Size.x; i++)
		{
			for (int j = 0; j < Size.y; j++)
			{
				float num3 = vector.x + (float)i - num;
				float num4 = vector.y + (float)j - num2;
				Vector2Int key = new Vector2Int(Mathf.RoundToInt(num3 / (float)gridSize) * gridSize, Mathf.RoundToInt(num4 / (float)gridSize) * gridSize);
				if (!Grid.World.TryGetValue(key, out var value))
				{
					value = new AmbientTemperatureGridBehaviour.Cell(PhysicalBehaviour.AmbientTemperature);
					Grid.World.TryAdd(key, value);
				}
				value.ForceSet(Mathf.Lerp(value.Get(), TargetAmbientTemperature, Grid.ObjectToAirTransferRate * TransferRateMultiplier));
			}
		}
	}
}
