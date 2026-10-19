using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class DiscordLinkButtonBehaviour : MonoBehaviour
{
	public const string ServerId = "779898133064056862";

	public const string ValidationApiEndpoint = "https://discordapp.com/api/invite/{0}";

	public const string InviteLink = "https://discord.gg/{0}";

	private bool busy;

	private int validInviteIndex = -1;

	public readonly IReadOnlyList<string> InviteIds = new string[2] { "studiominus", "ezBxexVe4t" };

	private void OnEnable()
	{
		if (!busy)
		{
			StartCoroutine(ValidationRoutine());
		}
	}

	public void OpenDiscordLink()
	{
		if (validInviteIndex != -1)
		{
			Utils.OpenURL($"https://discord.gg/{InviteIds[validInviteIndex]}");
		}
	}

	public IEnumerator ValidationRoutine()
	{
		if (busy)
		{
			yield break;
		}
		busy = true;
		Task<int> task = Task.Run(async () => await Utils.TaskTimeout(Validate(), TimeSpan.FromSeconds(4.0)));
		yield return new WaitUntil(() => task.IsCompleted);
		if (task.IsFaulted)
		{
			Debug.LogError(task.Exception);
			foreach (Exception innerException in task.Exception.InnerExceptions)
			{
				Debug.LogError(innerException);
			}
			base.gameObject.SetActive(value: false);
		}
		else
		{
			int num = (validInviteIndex = task.Result);
			base.gameObject.SetActive(num != -1);
		}
		task.Dispose();
		busy = false;
	}

	private async Task<int> Validate()
	{
		for (int i = 0; i < InviteIds.Count; i++)
		{
			string inviteId = InviteIds[i];
			try
			{
				using (HttpClient http = new HttpClient())
				{
					string text = await http.GetStringAsync($"https://discordapp.com/api/invite/{inviteId}");
					Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
					if (dictionary != null)
					{
						if (dictionary.TryGetValue("message", out var value))
						{
							string text2 = value as string;
							if (text2 != null && text2 == "Unknown Invite")
							{
								goto IL_00f2;
							}
						}
						return i;
					}
					goto IL_00f2;
					IL_00f2:
					Debug.LogError("Discord link " + inviteId + " is invalid! Invite api returned `" + text + "`");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError($"Failed to validate discord invite link {inviteId}: {arg}");
			}
		}
		return -1;
	}
}
