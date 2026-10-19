using UnityEngine;

public class FireSoundEmitter : MonoBehaviour
{
	public float Intensity = 1f;

	private void FixedUpdate()
	{
		if (base.gameObject.activeInHierarchy)
		{
			Global.main.FireLoopSoundControllerBehaviour.FuzzySetVolumeAt(base.transform.position, Mathf.Clamp01(Intensity));
		}
	}
}
