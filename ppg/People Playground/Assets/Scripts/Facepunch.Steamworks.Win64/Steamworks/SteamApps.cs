using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamApps : SteamSharedClass<SteamApps>
	{
		internal static ISteamApps Internal => SteamSharedClass<SteamApps>.Interface as ISteamApps;

		public static bool IsSubscribed => Internal.BIsSubscribed();

		public static bool IsSubscribedFromFamilySharing => Internal.BIsSubscribedFromFamilySharing();

		public static bool IsLowVoilence => Internal.BIsLowViolence();

		public static bool IsCybercafe => Internal.BIsCybercafe();

		public static bool IsVACBanned => Internal.BIsVACBanned();

		public static string GameLanguage => Internal.GetCurrentGameLanguage();

		public static string[] AvailableLanguages => Internal.GetAvailableGameLanguages().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

		public static bool IsSubscribedFromFreeWeekend => Internal.BIsSubscribedFromFreeWeekend();

		public static string CurrentBetaName
		{
			get
			{
				if (!Internal.GetCurrentBetaName(out var pchName))
				{
					return null;
				}
				return pchName;
			}
		}

		public static SteamId AppOwner => Internal.GetAppOwner().Value;

		public static int BuildId => Internal.GetAppBuildId();

		public static string CommandLine
		{
			get
			{
				Internal.GetLaunchCommandLine(out var pszCommandLine);
				return pszCommandLine;
			}
		}

		public static event Action<AppId> OnDlcInstalled;

		public static event Action OnNewLaunchParameters;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamApps(server));
		}

		internal static void InstallEvents()
		{
			Dispatch.Install(delegate(DlcInstalled_t x)
			{
				SteamApps.OnDlcInstalled?.Invoke(x.AppID);
			});
			Dispatch.Install<NewUrlLaunchParameters_t>(delegate
			{
				SteamApps.OnNewLaunchParameters?.Invoke();
			});
		}

		public static bool IsSubscribedToApp(AppId appid)
		{
			return Internal.BIsSubscribedApp(appid.Value);
		}

		public static bool IsDlcInstalled(AppId appid)
		{
			return Internal.BIsDlcInstalled(appid.Value);
		}

		public static DateTime PurchaseTime(AppId appid = default(AppId))
		{
			if ((uint)appid == 0)
			{
				appid = SteamClient.AppId;
			}
			return Epoch.ToDateTime(Internal.GetEarliestPurchaseUnixTime(appid.Value));
		}

		public static IEnumerable<DlcInformation> DlcInformation()
		{
			AppId appid = default(AppId);
			bool available = false;
			for (int i = 0; i < Internal.GetDLCCount(); i++)
			{
				if (Internal.BGetDLCDataByIndex(i, ref appid, ref available, out var strVal))
				{
					yield return new DlcInformation
					{
						AppId = appid.Value,
						Name = strVal,
						Available = available
					};
					strVal = null;
				}
			}
		}

		public static void InstallDlc(AppId appid)
		{
			Internal.InstallDLC(appid.Value);
		}

		public static void UninstallDlc(AppId appid)
		{
			Internal.UninstallDLC(appid.Value);
		}

		public static void MarkContentCorrupt(bool missingFilesOnly)
		{
			Internal.MarkContentCorrupt(missingFilesOnly);
		}

		public static IEnumerable<DepotId> InstalledDepots(AppId appid = default(AppId))
		{
			if ((uint)appid == 0)
			{
				appid = SteamClient.AppId;
			}
			DepotId_t[] depots = new DepotId_t[32];
			uint count = Internal.GetInstalledDepots(appid.Value, depots, (uint)depots.Length);
			for (int i = 0; i < count; i++)
			{
				yield return new DepotId
				{
					Value = depots[i].Value
				};
			}
		}

		public static string AppInstallDir(AppId appid = default(AppId))
		{
			if ((uint)appid == 0)
			{
				appid = SteamClient.AppId;
			}
			if (Internal.GetAppInstallDir(appid.Value, out var pchFolder) == 0)
			{
				return null;
			}
			return pchFolder;
		}

		public static bool IsAppInstalled(AppId appid)
		{
			return Internal.BIsAppInstalled(appid.Value);
		}

		public static string GetLaunchParam(string param)
		{
			return Internal.GetLaunchQueryParam(param);
		}

		public static DownloadProgress DlcDownloadProgress(AppId appid)
		{
			ulong punBytesDownloaded = 0uL;
			ulong punBytesTotal = 0uL;
			DownloadProgress result;
			if (!Internal.GetDlcDownloadProgress(appid.Value, ref punBytesDownloaded, ref punBytesTotal))
			{
				result = default(DownloadProgress);
				return result;
			}
			result = default(DownloadProgress);
			result.BytesDownloaded = punBytesDownloaded;
			result.BytesTotal = punBytesTotal;
			result.Active = true;
			return result;
		}

		public static async Task<FileDetails?> GetFileDetailsAsync(string filename)
		{
			FileDetailsResult_t? r = await Internal.GetFileDetails(filename);
			if (!r.HasValue || r.Value.Result != Result.OK)
			{
				return null;
			}
			FileDetails value = default(FileDetails);
			value.SizeInBytes = r.Value.FileSize;
			value.Flags = r.Value.Flags;
			value.Sha1 = string.Join("", r.Value.FileSHA.Select((byte x) => x.ToString("x")));
			return value;
		}
	}
}
