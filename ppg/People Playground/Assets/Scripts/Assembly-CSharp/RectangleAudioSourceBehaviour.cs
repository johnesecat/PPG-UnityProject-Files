using UnityEngine;

public class RectangleAudioSourceBehaviour : MonoBehaviour
{
	[SerializeField]
	private AudioSource AudioSource;

	public float MaxVolume = 1f;

	public float MinVolume;

	public Rect Rectangle;

	public float MaxDistance = 10f;

	public bool AutoPlayAndStopBasedOnDistance = true;

	private void LateUpdate()
	{
		float normalisedDistanceFactor = GetNormalisedDistanceFactor();
		float num = Utils.MapRange(0f, 1f, MinVolume, MaxVolume, normalisedDistanceFactor);
		if (AutoPlayAndStopBasedOnDistance)
		{
			if (num <= float.Epsilon)
			{
				if (AudioSource.isPlaying)
				{
					AudioSource.Stop();
				}
				return;
			}
			if (!AudioSource.isPlaying)
			{
				AudioSource.Play();
			}
			AudioSource.volume = num;
		}
		else
		{
			AudioSource.volume = num;
		}
	}

	public float GetNormalisedDistanceFactor()
	{
		Vector2 vector = Global.CameraPosition;
		vector -= (Vector2)base.transform.position;
		if (Rectangle.Contains(vector))
		{
			return 1f;
		}
		if (MaxDistance <= float.Epsilon)
		{
			return 0f;
		}
		float magnitude = (new Vector2(Mathf.Clamp(vector.x, Rectangle.xMin, Rectangle.xMax), Mathf.Clamp(vector.y, Rectangle.yMin, Rectangle.yMax)) - vector).magnitude;
		return 1f - magnitude / MaxDistance;
	}
}
