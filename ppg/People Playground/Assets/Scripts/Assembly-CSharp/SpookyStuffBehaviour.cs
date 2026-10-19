using System;
using System.Collections;
using UnityEngine;

[NotDocumented]
public class SpookyStuffBehaviour : MonoBehaviour
{
	public AudioSource Source;

	[Space]
	public AudioClip EndOfDaySound;

	public AudioClip StartOfDaySound;

	[Space]
	public int StartOfDayTime;

	public int EndOfDayTime;

	private bool hadStart;

	private bool hadEnd;

	private int lastDay;

	private void Start()
	{
		lastDay = DateTime.Now.Day;
		StartCoroutine(Tick());
	}

	private IEnumerator Tick()
	{
		DateTime now = DateTime.Now;
		if (now.Hour == StartOfDayTime)
		{
			StartOfDay();
		}
		else if (now.Hour == EndOfDayTime)
		{
			EndOfDay();
		}
		if (now.Day != lastDay)
		{
			lastDay = now.Day;
			hadEnd = false;
			hadStart = false;
		}
		yield return new WaitForSecondsRealtime(60f);
		StartCoroutine(Tick());
	}

	private void StartOfDay()
	{
		if (!hadStart)
		{
			hadStart = true;
			Source.PlayOneShot(StartOfDaySound);
		}
	}

	private void EndOfDay()
	{
		if (!hadEnd)
		{
			hadEnd = true;
			Source.PlayOneShot(EndOfDaySound);
		}
	}
}
