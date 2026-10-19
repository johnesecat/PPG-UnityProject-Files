using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Steamworks.Data;
using Steamworks.Ugc;

namespace Steamworks
{
	public class SteamUGC : SteamSharedClass<SteamUGC>
	{
		internal static ISteamUGC Internal => SteamSharedClass<SteamUGC>.Interface as ISteamUGC;

		public static event Action<Result> OnDownloadItemResult;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamUGC(server));
			InstallEvents(server);
		}

		internal static void InstallEvents(bool server)
		{
			Dispatch.Install(delegate(DownloadItemResult_t x)
			{
				SteamUGC.OnDownloadItemResult?.Invoke(x.Result);
			}, server);
		}

		public static async Task<bool> DeleteFileAsync(PublishedFileId fileId)
		{
			DeleteItemResult_t? r = await Internal.DeleteItem(fileId);
			return r.HasValue && r.GetValueOrDefault().Result == Result.OK;
		}

		public static bool Download(PublishedFileId fileId, bool highPriority = false)
		{
			return Internal.DownloadItem(fileId, highPriority);
		}

		public static async Task<bool> DownloadAsync(PublishedFileId fileId, Action<float> progress = null, int milisecondsUpdateDelay = 60, CancellationToken ct = default(CancellationToken))
		{
			Item item = new Item(fileId);
			if (ct == default(CancellationToken))
			{
				ct = new CancellationTokenSource(TimeSpan.FromSeconds(60.0)).Token;
			}
			progress?.Invoke(0f);
			if (!Download(fileId, highPriority: true))
			{
				return item.IsInstalled;
			}
			Action<Result> onDownloadStarted = null;
			try
			{
				bool downloadStarted = false;
				onDownloadStarted = delegate
				{
					downloadStarted = true;
				};
				OnDownloadItemResult += onDownloadStarted;
				while (!downloadStarted && !ct.IsCancellationRequested)
				{
					await Task.Delay(milisecondsUpdateDelay);
				}
			}
			finally
			{
				OnDownloadItemResult -= onDownloadStarted;
			}
			progress?.Invoke(0.2f);
			await Task.Delay(milisecondsUpdateDelay);
			while (!ct.IsCancellationRequested)
			{
				progress?.Invoke(0.2f + item.DownloadAmount * 0.8f);
				if (!item.IsDownloading && item.IsInstalled)
				{
					break;
				}
				await Task.Delay(milisecondsUpdateDelay);
			}
			progress?.Invoke(1f);
			return item.IsInstalled;
		}

		public static async Task<Item?> QueryFileAsync(PublishedFileId fileId)
		{
			ResultPage? result = await Query.All.WithFileId(fileId).GetPageAsync(1);
			if (!result.HasValue || result.Value.ResultCount != 1)
			{
				return null;
			}
			Item item = result.Value.Entries.First();
			result.Value.Dispose();
			return item;
		}

		public static async Task<bool> StartPlaytimeTracking(PublishedFileId fileId)
		{
			return (await Internal.StartPlaytimeTracking(new PublishedFileId[1] { fileId }, 1u)).Value.Result == Result.OK;
		}

		public static async Task<bool> StopPlaytimeTracking(PublishedFileId fileId)
		{
			return (await Internal.StopPlaytimeTracking(new PublishedFileId[1] { fileId }, 1u)).Value.Result == Result.OK;
		}

		public static async Task<bool> StopPlaytimeTrackingForAllItems()
		{
			return (await Internal.StopPlaytimeTrackingForAllItems()).Value.Result == Result.OK;
		}
	}
}
