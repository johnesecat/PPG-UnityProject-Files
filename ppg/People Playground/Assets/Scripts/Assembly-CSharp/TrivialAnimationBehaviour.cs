using System.Collections;
using UnityEngine;

public abstract class TrivialAnimationBehaviour : MonoBehaviour
{
	public float Duration = 1f;

	public bool Loop;

	public bool PlayOnStart;

	public bool IsPlaying { get; private set; }

	private void Start()
	{
		if (PlayOnStart)
		{
			Play();
		}
	}

	public void Play()
	{
		StopAllCoroutines();
		StartCoroutine(Routine());
	}

	public void Stop()
	{
		IsPlaying = false;
	}

	private IEnumerator Routine()
	{
		float t = 0f;
		IsPlaying = true;
		AnimationTick(0f);
		while (IsPlaying)
		{
			AnimationTick(t / Duration);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime;
			if (t >= Duration)
			{
				if (!Loop)
				{
					break;
				}
				t = 0f;
			}
		}
		IsPlaying = false;
		AnimationTick(1f);
	}

	protected abstract void AnimationTick(float progress);

	public void ForceSetProgress(float progress)
	{
		AnimationTick(progress);
	}
}
