using NaughtyAttributes;
using UnityEngine;

public class PlayAudioAtRandom : MonoBehaviour
{
	public AudioSource AudioSource;

	public float ChancePerSecond = 0.5f;

	public bool ChooseRandomClip;

	[ShowIf("ChooseRandomClip")]
	public AudioClip[] Clips;

	public bool PlaceInRandomPositionInCircle;

	[ShowIf("PlaceInRandomPositionInCircle")]
	public bool OnlyOnEdge;

	[ShowIf("PlaceInRandomPositionInCircle")]
	public Vector2 PositionCenter;

	[ShowIf("PlaceInRandomPositionInCircle")]
	public float Radius = 5000f;

	private float t;

	private void Update()
	{
		if (ChooseRandomClip && (Clips == null || Clips.Length == 0))
		{
			return;
		}
		t += Time.deltaTime;
		if (!(t > 1f))
		{
			return;
		}
		t = 0f;
		if (Random.value <= ChancePerSecond && !AudioSource.isPlaying)
		{
			if (PlaceInRandomPositionInCircle)
			{
				AudioSource.transform.localPosition = PositionCenter + (OnlyOnEdge ? Random.insideUnitCircle.normalized : Random.insideUnitCircle) * Radius;
			}
			if (ChooseRandomClip)
			{
				AudioSource.PlayOneShot(Clips.PickRandom());
			}
			else
			{
				AudioSource.PlayOneShot(AudioSource.clip);
			}
		}
	}
}
