using System.Collections;
using UnityEngine;

public class UnderwaterExplosionBehaviour : MonoBehaviour
{
	public float GivenTop;

	public float MaxDistanceToSurface;

	public bool DoSplash = true;

	public ParticleSystem Splash;

	public ExplosionSoundBehviour SplashAudio;

	private void Start()
	{
		float num = Mathf.Abs(base.transform.position.y - GivenTop);
		if (num <= MaxDistanceToSurface && DoSplash)
		{
			float d = Mathf.Min(0.25f, num * 0.01f);
			Splash.transform.position = new Vector3(Splash.transform.position.x, GivenTop, 0f);
			StartCoroutine(DelayedSplash(d));
		}
	}

	private IEnumerator DelayedSplash(float d)
	{
		yield return new WaitForSeconds(d);
		Splash.Play(withChildren: true);
		SplashAudio.Play();
	}
}
