using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks.Ugc
{
	public struct Item
	{
		internal SteamUGCDetails_t details;

		internal PublishedFileId _id;

		public PublishedFileId Id => _id;

		public string Title
		{
			[IsReadOnly]
			get;
			internal set; }

		public string Description
		{
			[IsReadOnly]
			get;
			internal set; }

		public string[] Tags
		{
			[IsReadOnly]
			get;
			internal set; }

		public AppId CreatorApp => details.CreatorAppID;

		public AppId ConsumerApp => details.ConsumerAppID;

		public Friend Owner => new Friend(details.SteamIDOwner);

		public float Score => details.Score;

		public DateTime Created => Epoch.ToDateTime(details.TimeCreated);

		public DateTime Updated => Epoch.ToDateTime(details.TimeUpdated);

		public bool IsPublic => details.Visibility == RemoteStoragePublishedFileVisibility.Public;

		public bool IsFriendsOnly => details.Visibility == RemoteStoragePublishedFileVisibility.FriendsOnly;

		public bool IsPrivate => details.Visibility == RemoteStoragePublishedFileVisibility.Private;

		public bool IsBanned => details.Banned;

		public bool IsAcceptedForUse => details.AcceptedForUse;

		public uint VotesUp => details.VotesUp;

		public uint VotesDown => details.VotesDown;

		public bool IsInstalled => (State & ItemState.Installed) == ItemState.Installed;

		public bool IsDownloading => (State & ItemState.Downloading) == ItemState.Downloading;

		public bool IsDownloadPending => (State & ItemState.DownloadPending) == ItemState.DownloadPending;

		public bool IsSubscribed => (State & ItemState.Subscribed) == ItemState.Subscribed;

		public bool NeedsUpdate => (State & ItemState.NeedsUpdate) == ItemState.NeedsUpdate;

		public string Directory
		{
			get
			{
				ulong punSizeOnDisk = 0uL;
				uint punTimeStamp = 0u;
				if (!SteamUGC.Internal.GetItemInstallInfo(Id, ref punSizeOnDisk, out var pchFolder, ref punTimeStamp))
				{
					return null;
				}
				return pchFolder;
			}
		}

		public long DownloadBytesTotal
		{
			get
			{
				if (!NeedsUpdate)
				{
					return SizeBytes;
				}
				ulong punBytesDownloaded = 0uL;
				ulong punBytesTotal = 0uL;
				if (SteamUGC.Internal.GetItemDownloadInfo(Id, ref punBytesDownloaded, ref punBytesTotal))
				{
					return (long)punBytesTotal;
				}
				return -1L;
			}
		}

		public long DownloadBytesDownloaded
		{
			get
			{
				if (!NeedsUpdate)
				{
					return SizeBytes;
				}
				ulong punBytesDownloaded = 0uL;
				ulong punBytesTotal = 0uL;
				if (SteamUGC.Internal.GetItemDownloadInfo(Id, ref punBytesDownloaded, ref punBytesTotal))
				{
					return (long)punBytesDownloaded;
				}
				return -1L;
			}
		}

		public long SizeBytes
		{
			get
			{
				if (NeedsUpdate)
				{
					return DownloadBytesDownloaded;
				}
				ulong punSizeOnDisk = 0uL;
				uint punTimeStamp = 0u;
				if (!SteamUGC.Internal.GetItemInstallInfo(Id, ref punSizeOnDisk, out var _, ref punTimeStamp))
				{
					return 0L;
				}
				return (long)punSizeOnDisk;
			}
		}

		public float DownloadAmount
		{
			get
			{
				if (!IsDownloading)
				{
					return 1f;
				}
				ulong punBytesDownloaded = 0uL;
				ulong punBytesTotal = 0uL;
				if (SteamUGC.Internal.GetItemDownloadInfo(Id, ref punBytesDownloaded, ref punBytesTotal) && punBytesTotal != 0)
				{
					return (float)((double)punBytesDownloaded / (double)punBytesTotal);
				}
				if (NeedsUpdate || !IsInstalled || IsDownloading)
				{
					return 0f;
				}
				return 1f;
			}
		}

		private ItemState State => (ItemState)SteamUGC.Internal.GetItemState(Id);

		public string Url => $"http://steamcommunity.com/sharedfiles/filedetails/?source=Facepunch.Steamworks&id={Id}";

		public string ChangelogUrl => $"http://steamcommunity.com/sharedfiles/filedetails/changelog/{Id}";

		public string CommentsUrl => $"http://steamcommunity.com/sharedfiles/filedetails/comments/{Id}";

		public string DiscussUrl => $"http://steamcommunity.com/sharedfiles/filedetails/discussions/{Id}";

		public string StatsUrl => $"http://steamcommunity.com/sharedfiles/filedetails/stats/{Id}";

		public ulong NumSubscriptions
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumFavorites
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumFollowers
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumUniqueSubscriptions
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumUniqueFavorites
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumUniqueFollowers
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumUniqueWebsiteViews
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong ReportScore
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumSecondsPlayed
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumPlaytimeSessions
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumComments
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumSecondsPlayedDuringTimePeriod
		{
			[IsReadOnly]
			get;
			internal set; }

		public ulong NumPlaytimeSessionsDuringTimePeriod
		{
			[IsReadOnly]
			get;
			internal set; }

		public string PreviewImageUrl
		{
			[IsReadOnly]
			get;
			internal set; }

		public Result Result => details.Result;

		public Item(PublishedFileId id)
		{
			this = default(Item);
			_id = id;
		}

		public bool Download(bool highPriority = false)
		{
			return SteamUGC.Download(Id, highPriority);
		}

		public static async Task<Item?> GetAsync(PublishedFileId id, int maxageseconds = 1800)
		{
			SteamUGCRequestUGCDetailsResult_t? result = await SteamUGC.Internal.RequestUGCDetails(id, (uint)maxageseconds);
			if (!result.HasValue)
			{
				return null;
			}
			return From(result.Value.Details);
		}

		internal static Item From(SteamUGCDetails_t details)
		{
			Item result = default(Item);
			result._id = details.PublishedFileId;
			result.details = details;
			result.Title = details.TitleUTF8();
			result.Description = details.DescriptionUTF8();
			result.Tags = details.TagsUTF8().ToLower().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			return result;
		}

		public bool HasTag(string find)
		{
			if (Tags.Length == 0)
			{
				return false;
			}
			return Tags.Contains(find, StringComparer.OrdinalIgnoreCase);
		}

		public async Task<bool> Subscribe()
		{
			RemoteStorageSubscribePublishedFileResult_t? result = await SteamUGC.Internal.SubscribeItem(_id);
			return result.HasValue && result.GetValueOrDefault().Result == Result.OK;
		}

		public async Task<bool> DownloadAsync(Action<float> progress = null, int milisecondsUpdateDelay = 60, CancellationToken ct = default(CancellationToken))
		{
			return await SteamUGC.DownloadAsync(Id, progress, milisecondsUpdateDelay, ct);
		}

		public async Task<bool> Unsubscribe()
		{
			RemoteStorageUnsubscribePublishedFileResult_t? result = await SteamUGC.Internal.UnsubscribeItem(_id);
			return result.HasValue && result.GetValueOrDefault().Result == Result.OK;
		}

		public async Task<bool> AddFavorite()
		{
			UserFavoriteItemsListChanged_t? result = await SteamUGC.Internal.AddItemToFavorites(details.ConsumerAppID, _id);
			return result.HasValue && result.GetValueOrDefault().Result == Result.OK;
		}

		public async Task<bool> RemoveFavorite()
		{
			UserFavoriteItemsListChanged_t? result = await SteamUGC.Internal.RemoveItemFromFavorites(details.ConsumerAppID, _id);
			return result.HasValue && result.GetValueOrDefault().Result == Result.OK;
		}

		public async Task<Result?> Vote(bool up)
		{
			return (await SteamUGC.Internal.SetUserItemVote(Id, up))?.Result;
		}

		public async Task<UserItemVote?> GetUserVote()
		{
			GetUserItemVoteResult_t? result = await SteamUGC.Internal.GetUserItemVote(_id);
			if (!result.HasValue)
			{
				return null;
			}
			return UserItemVote.From(result.Value);
		}

		public Editor Edit()
		{
			return new Editor(Id);
		}
	}
}
