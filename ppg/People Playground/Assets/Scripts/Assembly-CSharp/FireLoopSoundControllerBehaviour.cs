using UnityEngine;

public class FireLoopSoundControllerBehaviour : MonoBehaviour
{
	public FireLoopChunkBehaviour[,] FireChunks;

	public float ChunkSize = 5f;

	private Vector3 Origin;

	public Vector2Int ChunkGridSize { get; private set; }

	private void Start()
	{
		Vector2 vector = (Vector2)Global.main.CameraControlBehaviour.BoundingBox.size / ChunkSize;
		ChunkGridSize = new Vector2Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
		Origin = Global.main.CameraControlBehaviour.BoundingBox.center - Global.main.CameraControlBehaviour.BoundingBox.extents;
		GenerateGrid();
	}

	private void GenerateGrid()
	{
		FireChunks = new FireLoopChunkBehaviour[ChunkGridSize.x, ChunkGridSize.y];
		for (int i = 0; i < ChunkGridSize.y; i++)
		{
			for (int j = 0; j < ChunkGridSize.x; j++)
			{
				FireChunks[j, i] = null;
			}
		}
	}

	private void CreateChunkAt(int x, int y)
	{
		GameObject obj = new GameObject("FIRE CHUNK [" + x + "," + y + "]");
		obj.transform.SetParent(base.transform);
		obj.transform.position = Origin + ChunkSize * new Vector3(x, y, 0f);
		FireLoopChunkBehaviour fireLoopChunkBehaviour = obj.AddComponent<FireLoopChunkBehaviour>();
		fireLoopChunkBehaviour.Range = ChunkSize;
		FireChunks[x, y] = fireLoopChunkBehaviour;
		obj.SetActive(value: false);
	}

	public void FuzzySetVolumeAt(Vector3 position, float volume, float weight = 0.5f)
	{
		Vector2Int snappedPos = WorldToGrid(position);
		GetOrCreateChunkAt(snappedPos).FuzzySetVolume(volume, weight);
	}

	private FireLoopChunkBehaviour GetOrCreateChunkAt(Vector2Int snappedPos)
	{
		FireLoopChunkBehaviour fireLoopChunkBehaviour = FireChunks[snappedPos.x, snappedPos.y];
		if ((bool)fireLoopChunkBehaviour)
		{
			return fireLoopChunkBehaviour;
		}
		CreateChunkAt(snappedPos.x, snappedPos.y);
		return FireChunks[snappedPos.x, snappedPos.y];
	}

	private Vector2Int WorldToGrid(Vector3 position)
	{
		Vector2Int result = new Vector2Int(Mathf.RoundToInt((position.x - Origin.x) / ChunkSize), Mathf.RoundToInt((position.y - Origin.y) / ChunkSize));
		result.x = Mathf.Clamp(result.x, 0, ChunkGridSize.x - 1);
		result.y = Mathf.Clamp(result.y, 0, ChunkGridSize.y - 1);
		return result;
	}
}
