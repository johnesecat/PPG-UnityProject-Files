using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using ModModels;
using Newtonsoft.Json;
using UnityEngine;

[Obsolete]
internal class ModCompilerPipes : IModCompiler, IDisposable
{
	private NamedPipeServerStream pipeServer;

	private StreamWriter writer;

	private Process pipeClient;

	private Task receiveTask;

	private bool shouldStop;

	private readonly ConcurrentDictionary<int, CompilerReply> replies = new ConcurrentDictionary<int, CompilerReply>();

	public CompilerConfig Config { get; set; }

	public bool IsBusy { get; private set; }

	private string GetPipeName()
	{
		return Process.GetCurrentProcess().Id.ToString();
	}

	public void Start()
	{
		string pipeName = GetPipeName();
		shouldStop = false;
		KillAllServerProcesses();
		ProcessStartInfo processStartInfo = new ProcessStartInfo(Path.GetFullPath("ppgModCompiler/PPGModCompiler.exe"));
		pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
		processStartInfo.WorkingDirectory = Path.GetFullPath("ppgModCompiler");
		processStartInfo.CreateNoWindow = true;
		processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		processStartInfo.Arguments = pipeName;
		processStartInfo.UseShellExecute = false;
		if (pipeClient != null)
		{
			pipeClient.Dispose();
		}
		pipeClient = Process.Start(processStartInfo);
		pipeClient.EnableRaisingEvents = true;
		pipeClient.ErrorDataReceived += OnServerError;
		UnityEngine.Debug.Log("Waiting for pipe connection...");
		pipeServer.WaitForConnection();
		UnityEngine.Debug.Log("Pipe connected!");
		writer = new StreamWriter(pipeServer);
		writer.AutoFlush = true;
		receiveTask = Task.Run((Action)ReceiveMessages);
	}

	private void ReceiveMessages()
	{
		using (StreamReader streamReader = new StreamReader(pipeServer))
		{
			while (!shouldStop)
			{
				string value = streamReader.ReadLine();
				try
				{
					CompilerReply value2 = JsonConvert.DeserializeObject<CompilerReply>(value);
					if (value2.ID == -1)
					{
						throw new Exception("The compiler server failed critically.");
					}
					if (!replies.TryAdd(value2.ID, value2))
					{
						throw new Exception("Failed to add the compiler reply to the reply list");
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogErrorFormat("Received a weird as hell message from the mod compiler. Exception: {0}", ex.Message);
				}
			}
		}
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
		catch (Exception message)
		{
			UnityEngine.Debug.LogError(message);
			CompilerReply result = default(CompilerReply);
			result.State = CompilationState.Error;
			result.Message = "Unknown error occurred...";
			return result;
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void Stop()
	{
		shouldStop = true;
		receiveTask.Wait(2500);
		receiveTask.Dispose();
		if (pipeClient != null)
		{
			if (pipeServer.IsConnected && writer != null)
			{
				writer.WriteLine("quit");
			}
			pipeClient.WaitForExit(2500);
			pipeClient.Close();
			pipeClient.Kill();
			pipeClient.Dispose();
		}
		writer.Close();
		pipeServer.Disconnect();
		pipeServer.Close();
		KillAllServerProcesses();
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

	public void Dispose()
	{
		writer.Dispose();
		pipeServer.Dispose();
	}

	private CompilerReply GetReply(ModCompileInstructions instructions)
	{
		if (pipeServer == null)
		{
			throw new Exception("Compiler client isn't running.");
		}
		if (!pipeServer.IsConnected && (pipeClient == null || pipeClient.HasExited))
		{
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Starting server");
			if (pipeClient == null)
			{
				Start();
			}
			else
			{
				pipeClient.Start();
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
			if (pipeServer.IsConnected)
			{
				break;
			}
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Connecting...");
		}
		zero = TimeSpan.Zero;
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(UserPreferenceManager.Current.MaxModCompilationTime);
		string value = JsonConvert.SerializeObject(instructions, Formatting.None);
		BackgroundItemLoaderStatusBehaviour.SetDisplayState("Compiling");
		writer.WriteLine(value);
		CompilerReply result;
		do
		{
			Thread.Sleep(timeSpan);
			zero += timeSpan;
			if (!pipeServer.IsConnected)
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
		if (replies.TryRemove(key, out var value2))
		{
			return value2;
		}
		result = default(CompilerReply);
		result.State = CompilationState.Error;
		result.Message = "Failed to remove compiler reply somehow";
		return result;
	}
}
