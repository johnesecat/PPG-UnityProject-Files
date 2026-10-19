using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NotificationControllerBehaviour : MonoBehaviour
{
	public GameObject NotificationPrefab;

	public float NotificationLifetime = 3f;

	public uint MaximumAllowedNotifications = 16u;

	private static NotificationControllerBehaviour main;

	private static List<string> backlog = new List<string>();

	public List<string> startingHints = new List<string>();

	private int notificationCount;

	private List<GameObject> notificationObjects = new List<GameObject>();

	private void Start()
	{
		main = this;
		for (int i = 0; i < startingHints.Count; i++)
		{
			Show(startingHints[i]);
		}
		for (int j = 0; j < backlog.Count; j++)
		{
			Show(backlog[j]);
		}
		backlog.Clear();
		Application.logMessageReceived += LogMessageReceived;
	}

	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (UserPreferenceManager.Current.LogDebugMessages)
		{
			string text = "white";
			switch (type)
			{
			case LogType.Error:
				text = "orange";
				break;
			case LogType.Assert:
				text = "purple";
				break;
			case LogType.Warning:
				text = "yellow";
				break;
			case LogType.Log:
				text = "blue";
				break;
			case LogType.Exception:
				text = "red";
				break;
			}
			Show("<color=\"" + text + "\">" + condition + "</color>");
		}
	}

	public static void Show(string text)
	{
		if ((bool)main)
		{
			main.StartCoroutine(main.CreateNotification(text));
		}
		else
		{
			backlog.Add(text);
		}
	}

	public static float GetNotificationLifetime()
	{
		return main.NotificationLifetime;
	}

	private IEnumerator CreateNotification(string text)
	{
		if (notificationCount > MaximumAllowedNotifications)
		{
			DestroyNotification(notificationObjects.First());
		}
		notificationCount++;
		GameObject i = Object.Instantiate(NotificationPrefab, Vector3.zero, Quaternion.identity, base.transform);
		i.GetComponentInChildren<TextMeshProUGUI>().text = text;
		notificationObjects.Add(i);
		yield return new WaitForSecondsRealtime(NotificationLifetime);
		if ((bool)i)
		{
			DestroyNotification(i);
		}
	}

	private void DestroyNotification(GameObject n)
	{
		notificationObjects.Remove(n);
		Object.Destroy(n);
		notificationCount--;
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= LogMessageReceived;
	}
}
