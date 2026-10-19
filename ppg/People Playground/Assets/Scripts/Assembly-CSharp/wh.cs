using UnityEngine;
using UnityEngine.Events;

[NotDocumented]
public class wh : MonoBehaviour
{
	public UnityEvent OnLiftoff;

	public GameObject Uncharted;

	public LayerMask ToRemove;

	public string NoClearTag;

	private float t;

	private bool liftoff;

	private void OnBecameInvisible()
	{
		t = 0f;
	}

	private void OnWillRenderObject()
	{
		t += Time.deltaTime;
	}

	private void Update()
	{
		if (t > 10f && !liftoff)
		{
			liftoff = true;
			OnLiftoff.Invoke();
			StatManager.IncrementInteger(StatManager.Stat.WDJZM);
			if ((bool)AmbientTemperatureGridBehaviour.Instance)
			{
				AmbientTemperatureGridBehaviour.Instance.World.Clear();
			}
			UndoControllerBehaviour.ClearHistory();
			MapLoaderBehaviour.CurrentMap = new Map
			{
				InstantiateOverride = delegate(Transform parent)
				{
					Object.Instantiate(Uncharted, parent);
				}
			};
			Object.FindObjectOfType<ClearButtonBehaviour>().ClearEverything();
		}
	}
}
