using UnityEngine;
using UnityEngine.Events;

public class DeleteAfterTime : MonoBehaviour
{
	public float Life = 1f;

	[SkipSerialisation]
	public UnityEvent OnEndOfLife;

	private float time;

	[SkipSerialisation]
	public float Progress => time / Life;

	private void Update()
	{
		time += Time.deltaTime;
		if (time > Life)
		{
			OnEndOfLife?.Invoke();
			Object.Destroy(base.gameObject);
		}
	}
}
