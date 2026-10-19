using UnityEngine;

public class QualityButtonBehaviour : MonoBehaviour
{
	public void SetLevel(int level)
	{
		QualitySettings.SetQualityLevel(level);
	}
}
