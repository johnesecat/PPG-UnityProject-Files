using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MonoMod.Utils;

namespace HarmonyLib
{
	public class Harmony
	{
		public static bool DEBUG;

		public string Id { get; private set; }

		public Harmony(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id cannot be null or empty");
			}
			try
			{
				string environmentVariable = Environment.GetEnvironmentVariable("HARMONY_DEBUG");
				if (environmentVariable != null && environmentVariable.Length > 0)
				{
					environmentVariable = environmentVariable.Trim();
					DEBUG = environmentVariable == "1" || bool.Parse(environmentVariable);
				}
			}
			catch
			{
			}
			if (DEBUG)
			{
				Assembly assembly = typeof(Harmony).Assembly;
				Version version = assembly.GetName().Version;
				string text = assembly.Location;
				string text2 = Environment.Version.ToString();
				string text3 = Environment.OSVersion.Platform.ToString();
				if (string.IsNullOrEmpty(text))
				{
					text = new Uri(assembly.CodeBase).LocalPath;
				}
				int size = IntPtr.Size;
				Platform current = PlatformHelper.Current;
				FileLog.Log($"### Harmony id={id}, version={version}, location={text}, env/clr={text2}, platform={text3}, ptrsize:runtime/env={size}/{current}");
				MethodBase outsideCaller = AccessTools.GetOutsideCaller();
				if ((object)outsideCaller.DeclaringType != null)
				{
					Assembly assembly2 = outsideCaller.DeclaringType.Assembly;
					text = assembly2.Location;
					if (string.IsNullOrEmpty(text))
					{
						text = new Uri(assembly2.CodeBase).LocalPath;
					}
					FileLog.Log("### Started from " + outsideCaller.FullDescription() + ", location " + text);
					FileLog.Log($"### At {DateTime.Now:yyyy-MM-dd hh.mm.ss}");
				}
			}
			Id = id;
		}

		public void PatchAll()
		{
			Assembly assembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
			PatchAll(assembly);
		}

		public PatchProcessor CreateProcessor(MethodBase original)
		{
			return new PatchProcessor(this, original);
		}

		public PatchClassProcessor CreateClassProcessor(Type type)
		{
			return new PatchClassProcessor(this, type);
		}

		public ReversePatcher CreateReversePatcher(MethodBase original, HarmonyMethod standin)
		{
			return new ReversePatcher(this, original, standin);
		}

		public void PatchAll(Assembly assembly)
		{
			AccessTools.GetTypesFromAssembly(assembly).Do(delegate(Type type)
			{
				CreateClassProcessor(type).Patch();
			});
		}

		public MethodInfo Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
		{
			PatchProcessor patchProcessor = CreateProcessor(original);
			patchProcessor.AddPrefix(prefix);
			patchProcessor.AddPostfix(postfix);
			patchProcessor.AddTranspiler(transpiler);
			patchProcessor.AddFinalizer(finalizer);
			return patchProcessor.Patch();
		}

		public static MethodInfo ReversePatch(MethodBase original, HarmonyMethod standin, MethodInfo transpiler = null)
		{
			return PatchFunctions.ReversePatch(standin, original, transpiler);
		}

		public void UnpatchAll(string harmonyID = null)
		{
			foreach (MethodBase original in GetAllPatchedMethods().ToList())
			{
				bool num = original.HasMethodBody();
				Patches patchInfo2 = GetPatchInfo(original);
				if (num)
				{
					patchInfo2.Postfixes.DoIf(IDCheck, delegate(Patch patchInfo)
					{
						Unpatch(original, patchInfo.PatchMethod);
					});
					patchInfo2.Prefixes.DoIf(IDCheck, delegate(Patch patchInfo)
					{
						Unpatch(original, patchInfo.PatchMethod);
					});
				}
				patchInfo2.Transpilers.DoIf(IDCheck, delegate(Patch patchInfo)
				{
					Unpatch(original, patchInfo.PatchMethod);
				});
				if (num)
				{
					patchInfo2.Finalizers.DoIf(IDCheck, delegate(Patch patchInfo)
					{
						Unpatch(original, patchInfo.PatchMethod);
					});
				}
			}
			bool IDCheck(Patch patchInfo)
			{
				if (harmonyID != null)
				{
					return patchInfo.owner == harmonyID;
				}
				return true;
			}
		}

		public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyID = "*")
		{
			CreateProcessor(original).Unpatch(type, harmonyID);
		}

		public void Unpatch(MethodBase original, MethodInfo patch)
		{
			CreateProcessor(original).Unpatch(patch);
		}

		public static bool HasAnyPatches(string harmonyID)
		{
			return (from original in GetAllPatchedMethods()
				select GetPatchInfo(original)).Any((Patches info) => info.Owners.Contains(harmonyID));
		}

		public static Patches GetPatchInfo(MethodBase method)
		{
			return PatchProcessor.GetPatchInfo(method);
		}

		public IEnumerable<MethodBase> GetPatchedMethods()
		{
			return from original in GetAllPatchedMethods()
				where GetPatchInfo(original).Owners.Contains(Id)
				select original;
		}

		public static IEnumerable<MethodBase> GetAllPatchedMethods()
		{
			return PatchProcessor.GetAllPatchedMethods();
		}

		public static MethodBase GetOriginalMethod(MethodInfo replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			return HarmonySharedState.GetOriginal(replacement);
		}

		public static MethodBase GetMethodFromStackframe(StackFrame frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame");
			}
			return HarmonySharedState.FindReplacement(frame) ?? frame.GetMethod();
		}

		public static MethodBase GetOriginalMethodFromStackframe(StackFrame frame)
		{
			MethodBase methodBase = GetMethodFromStackframe(frame);
			MethodInfo methodInfo = methodBase as MethodInfo;
			if ((object)methodInfo != null)
			{
				methodBase = GetOriginalMethod(methodInfo) ?? methodBase;
			}
			return methodBase;
		}

		public static Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			return PatchProcessor.VersionInfo(out currentVersion);
		}
	}
}
