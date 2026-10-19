using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamMatchmaking : SteamClientClass<SteamMatchmaking>
	{
		internal static ISteamMatchmaking Internal => SteamClientClass<SteamMatchmaking>.Interface as ISteamMatchmaking;

		internal static int MaxLobbyKeyLength => 255;

		public static LobbyQuery LobbyList => default(LobbyQuery);

		public static event Action<Friend, Lobby> OnLobbyInvite;

		public static event Action<Lobby> OnLobbyEntered;

		public static event Action<Result, Lobby> OnLobbyCreated;

		public static event Action<Lobby, uint, ushort, SteamId> OnLobbyGameCreated;

		public static event Action<Lobby> OnLobbyDataChanged;

		public static event Action<Lobby, Friend> OnLobbyMemberDataChanged;

		public static event Action<Lobby, Friend> OnLobbyMemberJoined;

		public static event Action<Lobby, Friend> OnLobbyMemberLeave;

		public static event Action<Lobby, Friend> OnLobbyMemberDisconnected;

		public static event Action<Lobby, Friend, Friend> OnLobbyMemberKicked;

		public static event Action<Lobby, Friend, Friend> OnLobbyMemberBanned;

		public static event Action<Lobby, Friend, string> OnChatMessage;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamMatchmaking(server));
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install(delegate(LobbyInvite_t x)
			{
				SteamMatchmaking.OnLobbyInvite?.Invoke(new Friend(x.SteamIDUser), new Lobby(x.SteamIDLobby));
			});
			Dispatch.Install(delegate(LobbyEnter_t x)
			{
				SteamMatchmaking.OnLobbyEntered?.Invoke(new Lobby(x.SteamIDLobby));
			});
			Dispatch.Install(delegate(LobbyCreated_t x)
			{
				SteamMatchmaking.OnLobbyCreated?.Invoke(x.Result, new Lobby(x.SteamIDLobby));
			});
			Dispatch.Install(delegate(LobbyGameCreated_t x)
			{
				SteamMatchmaking.OnLobbyGameCreated?.Invoke(new Lobby(x.SteamIDLobby), x.IP, x.Port, x.SteamIDGameServer);
			});
			Dispatch.Install(delegate(LobbyDataUpdate_t x)
			{
				if (x.Success != 0)
				{
					if (x.SteamIDLobby == x.SteamIDMember)
					{
						SteamMatchmaking.OnLobbyDataChanged?.Invoke(new Lobby(x.SteamIDLobby));
					}
					else
					{
						SteamMatchmaking.OnLobbyMemberDataChanged?.Invoke(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDMember));
					}
				}
			});
			Dispatch.Install(delegate(LobbyChatUpdate_t x)
			{
				if ((x.GfChatMemberStateChange & (true ? 1u : 0u)) != 0)
				{
					SteamMatchmaking.OnLobbyMemberJoined?.Invoke(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged));
				}
				if ((x.GfChatMemberStateChange & 2u) != 0)
				{
					SteamMatchmaking.OnLobbyMemberLeave?.Invoke(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged));
				}
				if ((x.GfChatMemberStateChange & 4u) != 0)
				{
					SteamMatchmaking.OnLobbyMemberDisconnected?.Invoke(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged));
				}
				if ((x.GfChatMemberStateChange & 8u) != 0)
				{
					SteamMatchmaking.OnLobbyMemberKicked?.Invoke(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged), new Friend(x.SteamIDMakingChange));
				}
				if ((x.GfChatMemberStateChange & 0x10u) != 0)
				{
					SteamMatchmaking.OnLobbyMemberBanned?.Invoke(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged), new Friend(x.SteamIDMakingChange));
				}
			});
			Dispatch.Install<LobbyChatMsg_t>(OnLobbyChatMessageRecievedAPI);
		}

		private unsafe static void OnLobbyChatMessageRecievedAPI(LobbyChatMsg_t callback)
		{
			SteamId pSteamIDUser = default(SteamId);
			ChatEntryType peChatEntryType = ChatEntryType.Invalid;
			byte[] array = Helpers.TakeBuffer(4096);
			fixed (byte* ptr = array)
			{
				int lobbyChatEntry = Internal.GetLobbyChatEntry(callback.SteamIDLobby, (int)callback.ChatID, ref pSteamIDUser, (IntPtr)ptr, array.Length, ref peChatEntryType);
				if (lobbyChatEntry > 0)
				{
					SteamMatchmaking.OnChatMessage?.Invoke(new Lobby(callback.SteamIDLobby), new Friend(pSteamIDUser), Encoding.UTF8.GetString(array, 0, lobbyChatEntry));
				}
			}
		}

		public static async Task<Lobby?> CreateLobbyAsync(int maxMembers = 100)
		{
			LobbyCreated_t? lobby = await Internal.CreateLobby(LobbyType.Invisible, maxMembers);
			if (!lobby.HasValue || lobby.Value.Result != Result.OK)
			{
				return null;
			}
			Lobby value = default(Lobby);
			value.Id = lobby.Value.SteamIDLobby;
			return value;
		}

		public static async Task<Lobby?> JoinLobbyAsync(SteamId lobbyId)
		{
			LobbyEnter_t? lobby = await Internal.JoinLobby(lobbyId);
			if (!lobby.HasValue)
			{
				return null;
			}
			Lobby value = default(Lobby);
			value.Id = lobby.Value.SteamIDLobby;
			return value;
		}

		public static IEnumerable<ServerInfo> GetFavoriteServers()
		{
			int count = Internal.GetFavoriteGameCount();
			for (int i = 0; i < count; i++)
			{
				uint timeplayed = 0u;
				uint flags = 0u;
				ushort qport = 0;
				ushort cport = 0;
				uint ip = 0u;
				AppId appid = default(AppId);
				if (Internal.GetFavoriteGame(i, ref appid, ref ip, ref cport, ref qport, ref flags, ref timeplayed) && (flags & (true ? 1u : 0u)) != 0)
				{
					yield return new ServerInfo(ip, cport, qport, timeplayed);
				}
			}
		}

		public static IEnumerable<ServerInfo> GetHistoryServers()
		{
			int count = Internal.GetFavoriteGameCount();
			for (int i = 0; i < count; i++)
			{
				uint timeplayed = 0u;
				uint flags = 0u;
				ushort qport = 0;
				ushort cport = 0;
				uint ip = 0u;
				AppId appid = default(AppId);
				if (Internal.GetFavoriteGame(i, ref appid, ref ip, ref cport, ref qport, ref flags, ref timeplayed) && (flags & 2u) != 0)
				{
					yield return new ServerInfo(ip, cport, qport, timeplayed);
				}
			}
		}
	}
}
