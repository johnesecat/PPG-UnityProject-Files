using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DoAtTimeOnce : MonoBehaviour
{
	[Serializable]
	public class Action
	{
		public string Name;

		public bool Enabled = true;

		public float Delay;

		public float Duration;

		[Space]
		public UnityEvent Event;
	}

	public bool StartSequenceOnStart;

	[ReorderableList]
	public Action[] Actions;

	private void Start()
	{
		if (StartSequenceOnStart)
		{
			ExecuteSequence();
		}
	}

	public void ExecuteSequence()
	{
		StartCoroutine(StartSequence());
	}

	private IEnumerator StartSequence()
	{
		Action[] actions = Actions;
		foreach (Action action in actions)
		{
			if (action.Enabled)
			{
				yield return new WaitForSeconds(action.Delay);
				action.Event?.Invoke();
				yield return new WaitForSeconds(action.Duration);
			}
		}
	}
}
