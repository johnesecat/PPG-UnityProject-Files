using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	internal class GroupOperation : AsyncOperationBase<IList<AsyncOperationHandle>>, ICachable
	{
		[Flags]
		public enum GroupOperationSettings
		{
			None = 0x0,
			ReleaseDependenciesOnFailure = 0x1,
			AllowFailedDependencies = 0x2
		}

		private Action<AsyncOperationHandle> m_InternalOnComplete;

		private int m_LoadedCount;

		private GroupOperationSettings m_Settings;

		private string debugName;

		private const int k_MaxDisplayedLocationLength = 45;

		private HashSet<string> m_CachedDependencyLocations = new HashSet<string>();

		int ICachable.Hash { get; set; }

		protected override string DebugName
		{
			get
			{
				List<AsyncOperationHandle> list = new List<AsyncOperationHandle>();
				GetDependencies(list);
				if (list.Count == 0)
				{
					return "Dependencies";
				}
				if (debugName != null && DependenciesAreUnchanged(list))
				{
					return debugName;
				}
				m_CachedDependencyLocations.Clear();
				string text = "Dependencies [";
				for (int i = 0; i < list.Count; i++)
				{
					string text2 = list[i].LocationName;
					m_CachedDependencyLocations.Add(text2);
					if (text2 != null)
					{
						if (text2.Length > 45)
						{
							text2 = AsyncOperationBase<object>.ShortenPath(text2, keepExtension: true);
							text2 = text2.Substring(0, Math.Min(45, text2.Length)) + "...";
						}
						text = ((i != list.Count - 1) ? (text + text2 + ", ") : (text + text2));
					}
				}
				text = (debugName = text + "]");
				return debugName;
			}
		}

		protected override float Progress
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < base.Result.Count; i++)
				{
					AsyncOperationHandle asyncOperationHandle = base.Result[i];
					num = (asyncOperationHandle.IsDone ? (num + 1f) : (num + asyncOperationHandle.PercentComplete));
				}
				return num / (float)base.Result.Count;
			}
		}

		public GroupOperation()
		{
			m_InternalOnComplete = OnOperationCompleted;
			base.Result = new List<AsyncOperationHandle>();
		}

		internal IList<AsyncOperationHandle> GetDependentOps()
		{
			return base.Result;
		}

		protected override void GetDependencies(List<AsyncOperationHandle> deps)
		{
			deps.AddRange(base.Result);
		}

		internal override void ReleaseDependencies()
		{
			for (int i = 0; i < base.Result.Count; i++)
			{
				if (base.Result[i].IsValid())
				{
					base.Result[i].Release();
				}
			}
			base.Result.Clear();
		}

		internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
		{
			DownloadStatus downloadStatus = default(DownloadStatus);
			downloadStatus.IsDone = base.IsDone;
			DownloadStatus result = downloadStatus;
			for (int i = 0; i < base.Result.Count; i++)
			{
				if (base.Result[i].IsValid())
				{
					DownloadStatus downloadStatus2 = base.Result[i].InternalGetDownloadStatus(visited);
					result.DownloadedBytes += downloadStatus2.DownloadedBytes;
					result.TotalBytes += downloadStatus2.TotalBytes;
				}
			}
			return result;
		}

		private bool DependenciesAreUnchanged(List<AsyncOperationHandle> deps)
		{
			if (m_CachedDependencyLocations.Count != deps.Count)
			{
				return false;
			}
			foreach (AsyncOperationHandle dep in deps)
			{
				if (!m_CachedDependencyLocations.Contains(dep.LocationName))
				{
					return false;
				}
			}
			return true;
		}

		protected override void Execute()
		{
			m_LoadedCount = 0;
			for (int i = 0; i < base.Result.Count; i++)
			{
				if (base.Result[i].IsDone)
				{
					m_LoadedCount++;
					continue;
				}
				AsyncOperationHandle asyncOperationHandle = base.Result[i];
				asyncOperationHandle.Completed += m_InternalOnComplete;
			}
			CompleteIfDependenciesComplete();
		}

		private void CompleteIfDependenciesComplete()
		{
			if (m_LoadedCount != base.Result.Count)
			{
				return;
			}
			bool success = true;
			string errorMsg = string.Empty;
			if (!m_Settings.HasFlag(GroupOperationSettings.AllowFailedDependencies))
			{
				for (int i = 0; i < base.Result.Count; i++)
				{
					if (base.Result[i].Status != AsyncOperationStatus.Succeeded)
					{
						success = false;
						errorMsg = ((base.Result[i].OperationException != null) ? base.Result[i].OperationException.Message : string.Empty);
						break;
					}
				}
			}
			Complete(base.Result, success, errorMsg, m_Settings.HasFlag(GroupOperationSettings.ReleaseDependenciesOnFailure));
		}

		protected override void Destroy()
		{
			ReleaseDependencies();
		}

		public void Init(List<AsyncOperationHandle> operations, bool releaseDependenciesOnFailure = true, bool allowFailedDependencies = false)
		{
			base.Result = new List<AsyncOperationHandle>(operations);
			m_Settings = (releaseDependenciesOnFailure ? GroupOperationSettings.ReleaseDependenciesOnFailure : GroupOperationSettings.None);
			if (allowFailedDependencies)
			{
				m_Settings |= GroupOperationSettings.AllowFailedDependencies;
			}
		}

		public void Init(List<AsyncOperationHandle> operations, GroupOperationSettings settings)
		{
			base.Result = new List<AsyncOperationHandle>(operations);
			m_Settings = settings;
		}

		private void OnOperationCompleted(AsyncOperationHandle op)
		{
			m_LoadedCount++;
			CompleteIfDependenciesComplete();
		}
	}
}
