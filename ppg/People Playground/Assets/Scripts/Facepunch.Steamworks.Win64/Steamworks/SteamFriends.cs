using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamFriends : SteamClientClass<SteamFriends>
	{
		private static Dictionary<string, string> richPresence;

		private static bool _listenForFriendsMessages;

		internal static ISteamFriends Internal => SteamClientClass<SteamFriends>.Interface as ISteamFriends;

		public static bool ListenForFriendsMessages
		{
			get
			{
				return _listenForFriendsMessages;
			}
			set
			{
				_listenForFriendsMessages = value;
				Internal.SetListenForFriendsMessages(value);
			}
		}

		public static event Action<Friend, string, string> OnChatMessage;

		public static event Action<Friend> OnPersonaStateChange;

		public static event Action<Friend, string> OnGameRichPresenceJoinRequested;

		public static event Action<bool> OnGameOverlayActivated;

		public static event Action<string, string> OnGameServerChangeRequested;

		public static event Action<Lobby, SteamId> OnGameLobbyJoinRequested;

		public static event Action<Friend> OnFriendRichPresenceUpdate;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamFriends(server));
			richPresence = new Dictionary<string, string>();
			InstallEvents();
		}

		internal void InstallEvents()
		{
			Dispatch.Install(delegate(PersonaStateChange_t x)
			{
				SteamFriends.OnPersonaStateChange?.Invoke(new Friend(x.SteamID));
			});
			Dispatch.Install(delegate(GameRichPresenceJoinRequested_t x)
			{
				SteamFriends.OnGameRichPresenceJoinRequested?.Invoke(new Friend(x.SteamIDFriend), x.ConnectUTF8());
			});
			Dispatch.Install<GameConnectedFriendChatMsg_t>(OnFriendChatMessage);
			Dispatch.Install(delegate(GameOverlayActivated_t x)
			{
				SteamFriends.OnGameOverlayActivated?.Invoke(x.Active != 0);
			});
			Dispatch.Install(delegate(GameServerChangeRequested_t x)
			{
				SteamFriends.OnGameServerChangeRequested?.Invoke(x.ServerUTF8(), x.PasswordUTF8());
			});
			Dispatch.Install(delegate(GameLobbyJoinRequested_t x)
			{
				SteamFriends.OnGameLobbyJoinRequested?.Invoke(new Lobby(x.SteamIDLobby), x.SteamIDFriend);
			});
			Dispatch.Install(delegate(FriendRichPresenceUpdate_t x)
			{
				SteamFriends.OnFriendRichPresenceUpdate?.Invoke(new Friend(x.SteamIDFriend));
			});
		}

		private unsafe static void OnFriendChatMessage(GameConnectedFriendChatMsg_t data)
		{
			if (SteamFriends.OnChatMessage == null)
			{
				return;
			}
			Friend arg = new Friend(data.SteamIDUser);
			byte[] array = Helpers.TakeBuffer(32768);
			ChatEntryType peChatEntryType = ChatEntryType.ChatMsg;
			fixed (byte* ptr = array)
			{
				int friendMessage = Internal.GetFriendMessage(data.SteamIDUser, data.MessageID, (IntPtr)ptr, array.Length, ref peChatEntryType);
				if (friendMessage != 0 || peChatEntryType != 0)
				{
					string arg2 = peChatEntryType.ToString();
					string @string = Encoding.UTF8.GetString(array, 0, friendMessage);
					SteamFriends.OnChatMessage(arg, arg2, @string);
				}
			}
		}

		private static IEnumerable<Friend> GetFriendsWithFlag(FriendFlags flag)
		{
			for (int i = 0; i < Internal.GetFriendCount((int)flag); i++)
			{
				yield return new Friend(Internal.GetFriendByIndex(i, (int)flag));
			}
		}

		public static IEnumerable<Friend> GetFriends()
		{
			return GetFriendsWithFlag(FriendFlags.Immediate);
		}

		public static IEnumerable<Friend> GetBlocked()
		{
			return GetFriendsWithFlag(FriendFlags.Blocked);
		}

		public static IEnumerable<Friend> GetFriendsRequested()
		{
			return GetFriendsWithFlag(FriendFlags.FriendshipRequested);
		}

		public static IEnumerable<Friend> GetFriendsClanMembers()
		{
			return GetFriendsWithFlag(FriendFlags.ClanMember);
		}

		public static IEnumerable<Friend> GetFriendsOnGameServer()
		{
			return GetFriendsWithFlag(FriendFlags.OnGameServer);
		}

		public static IEnumerable<Friend> GetFriendsRequestingFriendship()
		{
			return GetFriendsWithFlag(FriendFlags.RequestingFriendship);
		}

		public static IEnumerable<Friend> GetPlayedWith()
		{
			for (int i = 0; i < Internal.GetCoplayFriendCount(); i++)
			{
				yield return new Friend(Internal.GetCoplayFriend(i));
			}
		}

		public static IEnumerable<Friend> GetFromSource(SteamId steamid)
		{
			for (int i = 0; i < Internal.GetFriendCountFromSource(steamid); i++)
			{
				yield return new Friend(Internal.GetFriendFromSourceByIndex(steamid, i));
			}
		}

		public static void OpenOverlay(string type)
		{
			Internal.ActivateGameOverlay(type);
		}

		public static void OpenUserOverlay(SteamId id, string type)
		{
			Internal.ActivateGameOverlayToUser(type, id);
		}

		public static void OpenStoreOverlay(AppId id)
		{
			Internal.ActivateGameOverlayToStore(id.Value, OverlayToStoreFlag.None);
		}

		public static void OpenWebOverlay(string url, bool modal = false)
		{
			Internal.ActivateGameOverlayToWebPage(url, modal ? ActivateGameOverlayToWebPageMode.Modal : ActivateGameOverlayToWebPageMode.Default);
		}

		public static void OpenGameInviteOverlay(SteamId lobby)
		{
			Internal.ActivateGameOverlayInviteDialog(lobby);
		}

		public static void SetPlayedWith(SteamId steamid)
		{
			Internal.SetPlayedWith(steamid);
		}

		public static bool RequestUserInformation(SteamId steamid, bool nameonly = true)
		{
			return Internal.RequestUserInformation(steamid, nameonly);
		}

		internal static async Task CacheUserInformationAsync(SteamId steamid, bool nameonly)
		{
			if (RequestUserInformation(steamid, nameonly))
			{
				await Task.Delay(100);
				while (RequestUserInformation(steamid, nameonly))
				{
					await Task.Delay(50);
				}
				await Task.Delay(500);
			}
		}

		public static async Task<Image?> GetSmallAvatarAsync(SteamId steamid)
		{
			await CacheUserInformationAsync(steamid, nameonly: false);
			return SteamUtils.GetImage(Internal.GetSmallFriendAvatar(steamid));
		}

		public static async Task<Image?> GetMediumAvatarAsync(SteamId steamid)
		{
			await CacheUserInformationAsync(steamid, nameonly: false);
			return SteamUtils.GetImage(Internal.GetMediumFriendAvatar(steamid));
		}

		public static async Task<Image?> GetLargeAvatarAsync(SteamId steamid)
		{
			await CacheUserInformationAsync(steamid, nameonly: false);
			int imageid;
			for (imageid = Internal.GetLargeFriendAvatar(steamid); imageid == -1; imageid = Internal.GetLargeFriendAvatar(steamid))
			{
				await Task.Delay(50);
			}
			return SteamUtils.GetImage(imageid);
		}

		public static string GetRichPresence(string key)
		{
			if (richPresence.TryGetValue(key, out var value))
			{
				return value;
			}
			return null;
		}

		public static bool SetRichPresence(string key, string value)
		{
			bool flag = Internal.SetRichPresence(key, value);
			if (flag)
			{
				richPresence[key] = value;
			}
			return flag;
		}

		public static void ClearRichPresence()
		{
			richPresence.Clear();
			Internal.ClearRichPresence();
		}

		public static async Task<bool> IsFollowing(SteamId steamID)
		{
			return (await Internal.IsFollowing(steamID)).Value.IsFollowing;
		}

		public static async Task<int> GetFollowerCount(SteamId steamID)
		{
			return (await Internal.GetFollowerCount(steamID)).Value.Count;
		}

		public static async Task<SteamId[]> GetFollowingList()
		{
			int resultCount = 0;
			List<SteamId> steamIds = new List<SteamId>();
			FriendsEnumerateFollowingList_t? result;
			do
			{
				FriendsEnumerateFollowingList_t? friendsEnumerateFollowingList_t;
				result = (friendsEnumerateFollowingList_t = await Internal.EnumerateFollowingList((uint)resultCount));
				friendsEnumerateFollowingList_t = friendsEnumerateFollowingList_t;
				if (!friendsEnumerateFollowingList_t.HasValue)
				{
					continue;
				}
				resultCount += result.Value.ResultsReturned;
				Array.ForEach(result.Value.GSteamID, delegate(ulong id)
				{
					if (id != 0)
					{
						steamIds.Add(id);
					}
				});
			}
			while (result.HasValue && resultCount < result.Value.TotalResultCount);
			return steamIds.ToArray();
		}
	}
}
