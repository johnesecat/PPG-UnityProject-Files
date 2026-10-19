using System;
using System.IO;
using ModModels;
using Newtonsoft.Json;
using UnityEngine;

internal static class CompilerClient
{
	private const string configPath = "ppgModCompiler\\config.json";

	private static readonly IModCompiler compiler = new ModCompilerSockets();

	public static void InitialiseConfig()
	{
		compiler.Config = ReadConfig();
	}

	public static void Dispose()
	{
		compiler.Dispose();
	}

	private static CompilerConfig ReadConfig()
	{
		try
		{
			return JsonConvert.DeserializeObject<CompilerConfig>(File.ReadAllText("ppgModCompiler\\config.json"));
		}
		catch (Exception ex)
		{
			Debug.LogWarningFormat("Failed to read compiler config: {0}", ex);
		}
		return new CompilerConfig();
	}

	public static void Start()
	{
		compiler.Start();
	}

	public static void Stop()
	{
		compiler.Stop();
	}

	public static CompilerReply RequestCompilationSynchronous(ModCompileInstructions instructions)
	{
		return compiler.RequestCompilationSynchronous(instructions);
	}
}
