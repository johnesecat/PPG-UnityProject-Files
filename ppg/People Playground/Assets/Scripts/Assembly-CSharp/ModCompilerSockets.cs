using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using ModModels;
using Newtonsoft.Json;
using UnityEngine;
using WatsonTcp;

internal class ModCompilerSockets : IModCompiler, IDisposable
{
	private WatsonTcpClient client;

	private Process serverProcess;

	private readonly ConcurrentDictionary<int, CompilerReply> replies = new ConcurrentDictionary<int, CompilerReply>();

	public CompilerConfig Config { get; set; }

	public bool IsBusy { get; private set; }

	public void Start()
	{
		StartServer();
	}

	public CompilerReply RequestCompilationSynchronous(ModCompileInstructions instructions)
	{
		while (IsBusy)
		{
			Thread.Sleep(16);
		}
		IsBusy = true;
		try
		{
			return GetReply(instructions);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError(ex);
			CompilerReply result = default(CompilerReply);
			result.State = CompilationState.Error;
			result.Message = ex.Message;
			return result;
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void Stop()
	{
		if (client != null)
		{
			if (client.Connected)
			{
				client.Send(Encoding.UTF8.GetBytes("quit"));
			}
			client.Disconnect();
			client.Dispose();
		}
		replies.Clear();
		if (serverProcess != null)
		{
			serverProcess.Kill();
			serverProcess.Dispose();
		}
		KillAllServerProcesses();
		client = null;
		serverProcess = null;
		UnityEngine.Debug.Log("Compiler client stopped and server terminated.");
	}

	public void Dispose()
	{
		Stop();
	}

	private static void KillAllServerProcesses()
	{
		Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension("PPGModCompiler.exe"));
		if (processesByName != null && processesByName.Length != 0)
		{
			for (int i = 0; i < processesByName.Length; i++)
			{
				processesByName[i].Kill();
			}
		}
	}

	private static void OnServerError(object sender, DataReceivedEventArgs e)
	{
		UnityEngine.Debug.LogErrorFormat("Compiler server encountered an error: {0}", e.Data);
	}

	public void StartClient()
	{
		if (client != null)
		{
			client.Dispose();
		}
		client = new WatsonTcpClient(Config.Hostname, Config.Port);
		WatsonTcpClientSettings settings = client.Settings;
		settings.Logger = (Action<Severity, string>)Delegate.Combine(settings.Logger, (Action<Severity, string>)delegate(Severity s, string m)
		{
			Console.WriteLine("[{0}] {1}", s, m);
		});
		client.Events.MessageReceived += OnClientMessage;
		client.Events.ServerConnected += OnClientOpen;
		client.Events.ServerDisconnected += OnClientClose;
		client.Events.ExceptionEncountered += delegate(object o, ExceptionEventArgs e)
		{
			UnityEngine.Debug.LogErrorFormat("Compiler client encountered an exception: {0}", e.Exception);
		};
		client.Callbacks.SyncRequestReceived = (SyncRequest req) => new SyncResponse(req, "snapt iemand dit??");
		client.Connect();
		client.SendAndWait(1000, "hoe is het nou deel 1");
	}

	public void StartServer()
	{
		KillAllServerProcesses();
		ProcessStartInfo processStartInfo = new ProcessStartInfo("PPGModCompiler.exe");
		processStartInfo.WorkingDirectory = Path.GetFullPath("ppgModCompiler");
		processStartInfo.CreateNoWindow = true;
		processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		processStartInfo.Arguments = Process.GetCurrentProcess().Id.ToString();
		if (serverProcess != null)
		{
			serverProcess.Dispose();
		}
		serverProcess = Process.Start(processStartInfo);
		serverProcess.EnableRaisingEvents = true;
		serverProcess.ErrorDataReceived += OnServerError;
	}

	private static void OnClientOpen(object sender, ConnectionEventArgs args)
	{
		UnityEngine.Debug.Log("Compiler client connected to server! :)");
	}

	private static void OnClientClose(object sender, DisconnectionEventArgs args)
	{
		UnityEngine.Debug.LogWarningFormat("Compiler client disconnected from server");
	}

	private void OnClientMessage(object sender, MessageReceivedEventArgs args)
	{
		try
		{
			CompilerReply value = JsonConvert.DeserializeObject<CompilerReply>(Encoding.UTF8.GetString(args.Data));
			if (value.ID == -1)
			{
				throw new Exception("The compiler server failed critically.");
			}
			if (!replies.TryAdd(value.ID, value))
			{
				throw new Exception("Failed to add the compiler reply to the reply list");
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogErrorFormat("Received a weird as hell message from the mod compiler. Exception: {0}", ex.Message);
		}
	}

	private CompilerReply GetReply(ModCompileInstructions instructions)
	{
		if (client == null)
		{
			StartClient();
			while (client == null)
			{
				Thread.Sleep(16);
			}
		}
		if (!client.Connected && (serverProcess == null || serverProcess.HasExited))
		{
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Starting server");
			if (serverProcess == null)
			{
				StartServer();
			}
			else
			{
				serverProcess.Start();
			}
		}
		int key = (instructions.ID = instructions.Paths.GetHashCode());
		TimeSpan zero = TimeSpan.Zero;
		TimeSpan timeSpan = TimeSpan.FromSeconds(0.05000000074505806);
		while (true)
		{
			Thread.Sleep(timeSpan);
			zero += timeSpan;
			if (zero > TimeSpan.FromSeconds(15.0))
			{
				throw new Exception("Failed to start client in under 15 seconds");
			}
			if (client.Connected)
			{
				break;
			}
			client.Connect();
			client.SendAndWait(1000, "hoe is het nou");
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Connecting...");
		}
		zero = TimeSpan.Zero;
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(UserPreferenceManager.Current.MaxModCompilationTime);
		string s = JsonConvert.SerializeObject(instructions);
		BackgroundItemLoaderStatusBehaviour.SetDisplayState("Compiling");
		if (client.Send(Encoding.UTF8.GetBytes(s)))
		{
			UnityEngine.Debug.LogWarning("Failed to send message to compiler server...");
		}
		CompilerReply result;
		do
		{
			Thread.Sleep(timeSpan);
			zero += timeSpan;
			if (!client.Connected)
			{
				result = default(CompilerReply);
				result.State = CompilationState.Error;
				result.Message = "Client disconnected during compilation...";
				return result;
			}
			if (zero > timeSpan2)
			{
				result = default(CompilerReply);
				result.State = CompilationState.Error;
				result.Message = $"Compilation timeout! Mod took over {(int)timeSpan2.TotalSeconds} seconds to compile and will be ignored.";
				return result;
			}
		}
		while (!replies.ContainsKey(key));
		if (replies.TryRemove(key, out var value))
		{
			return value;
		}
		result = default(CompilerReply);
		result.State = CompilationState.Error;
		result.Message = "Failed to remove compiler reply somehow";
		return result;
	}
}
