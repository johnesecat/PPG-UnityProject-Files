using System;
using System.ComponentModel;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Text Data Provider")]
	public class TextDataProvider : ResourceProviderBase
	{
		private class InternalOp
		{
			private TextDataProvider m_Provider;

			private UnityWebRequestAsyncOperation m_RequestOperation;

			private WebRequestQueueOperation m_RequestQueueOperation;

			private bool m_IgnoreFailures;

			private ProvideHandle m_PI;

			private float GetPercentComplete()
			{
				if (m_RequestOperation == null)
				{
					return 0f;
				}
				return m_RequestOperation.progress;
			}

			public void Start(ProvideHandle provideHandle, TextDataProvider rawProvider, bool ignoreFailures)
			{
				m_PI = provideHandle;
				provideHandle.SetProgressCallback(GetPercentComplete);
				m_Provider = rawProvider;
				m_IgnoreFailures = ignoreFailures;
				string text = m_PI.ResourceManager.TransformInternalId(m_PI.Location);
				if (File.Exists(text))
				{
					if (text.Length >= 260)
					{
						text = "\\\\?\\" + text;
					}
					string text2 = File.ReadAllText(text);
					object obj = m_Provider.Convert(m_PI.Type, text2);
					m_PI.Complete(obj, obj != null, (obj == null) ? new Exception($"Unable to load asset of type {m_PI.Type} from location {m_PI.Location}.") : null);
				}
				else if (ResourceManagerConfig.ShouldPathUseWebRequest(text))
				{
					UnityWebRequest request = new UnityWebRequest(text, "GET", new DownloadHandlerBuffer(), null);
					m_RequestQueueOperation = WebRequestQueue.QueueRequest(request);
					if (m_RequestQueueOperation.IsDone)
					{
						m_RequestOperation = m_RequestQueueOperation.Result;
						if (m_RequestOperation.isDone)
						{
							RequestOperation_completed(m_RequestOperation);
						}
						else
						{
							m_RequestOperation.completed += RequestOperation_completed;
						}
					}
					else
					{
						WebRequestQueueOperation requestQueueOperation = m_RequestQueueOperation;
						requestQueueOperation.OnComplete = (Action<UnityWebRequestAsyncOperation>)Delegate.Combine(requestQueueOperation.OnComplete, (Action<UnityWebRequestAsyncOperation>)delegate(UnityWebRequestAsyncOperation asyncOperation)
						{
							m_RequestOperation = asyncOperation;
							m_RequestOperation.completed += RequestOperation_completed;
						});
					}
				}
				else
				{
					Exception exception = null;
					if (!m_IgnoreFailures)
					{
						exception = new Exception($"Invalid path in TextDataProvider : '{text}'.");
					}
					m_PI.Complete<object>(null, m_IgnoreFailures, exception);
				}
			}

			private void RequestOperation_completed(AsyncOperation op)
			{
				UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = op as UnityWebRequestAsyncOperation;
				object obj = null;
				Exception exception = null;
				if (unityWebRequestAsyncOperation != null)
				{
					UnityWebRequest webRequest = unityWebRequestAsyncOperation.webRequest;
					if (string.IsNullOrEmpty(webRequest.error))
					{
						obj = m_Provider.Convert(m_PI.Type, webRequest.downloadHandler.text);
					}
					else
					{
						exception = new Exception($"TextDataProvider unable to load from url {webRequest.url}, result='{webRequest.error}'.");
					}
				}
				else
				{
					exception = new Exception("TextDataProvider unable to load from unknown url.");
				}
				m_PI.Complete(obj, obj != null || m_IgnoreFailures, exception);
			}
		}

		public bool IgnoreFailures { get; set; }

		public virtual object Convert(Type type, string text)
		{
			return text;
		}

		public override void Provide(ProvideHandle provideHandle)
		{
			new InternalOp().Start(provideHandle, this, IgnoreFailures);
		}
	}
}
