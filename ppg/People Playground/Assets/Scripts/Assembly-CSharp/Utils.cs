using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using Newtonsoft.Json;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Utils
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Activations
	{
		public static void SendOnce(ActivationPropagation a, IUseEmitter target)
		{
			target.OnSingleUse.Invoke(a);
		}

		public static void SendContinuous(ActivationPropagation a, IUseEmitter target)
		{
			target.OnContinuousUse.Invoke(a);
		}
	}

	public struct LaserHit
	{
		public RaycastHit2D? hit;

		public PhysicalBehaviour physicalBehaviour;

		public Vector2 point;

		public int rayDepth;

		public float totalDistance;

		public LaserHit(RaycastHit2D? hit, Vector2 point, PhysicalBehaviour physicalBehaviour, int rayDepth)
		{
			this.hit = hit;
			this.point = point;
			this.physicalBehaviour = physicalBehaviour;
			this.rayDepth = rayDepth;
			totalDistance = 0f;
		}
	}

	public const float UnitsToMeters = 220f / 267f;

	public const float MetersToUnits = 1.2136364f;

	public const float E = (float)Math.E;

	private static Vector3[] getWorldSpaceBoundsBuffer = new Vector3[4];

	private static readonly Collider2D[] colliderBuffer = new Collider2D[16];

	public static bool HasLayer(this LayerMask mask, int layer)
	{
		return ((int)mask & (1 << layer)) != 0;
	}

	public static void Fill<T>(this T[] array, T val)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = val;
		}
	}

	public static Vector2 ClosestPointFix(this Rigidbody2D rb, Vector2 point)
	{
		Collider2D[] array = ArrayPool<Collider2D>.Shared.Rent(rb.attachedColliderCount);
		float num = float.MaxValue;
		Vector2 result = point;
		for (int i = 0; i < rb.GetAttachedColliders(array); i++)
		{
			Vector2 vector = array[i].ClosestPoint(point);
			float sqrMagnitude = (vector - point).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = vector;
			}
		}
		ArrayPool<Collider2D>.Shared.Return(array, clearArray: true);
		return result;
	}

	public static float SmoothApproach(float pastPosition, float targetPosition, float speed, float deltaTime)
	{
		float num = deltaTime * speed;
		return (targetPosition + (pastPosition - targetPosition) * Mathf.Exp(0f - num)).NaNFallback();
	}

	public static Vector2 SmoothApproach(Vector2 pastPosition, Vector2 targetPosition, float speed, float deltaTime)
	{
		float num = deltaTime * speed;
		return (targetPosition + (pastPosition - targetPosition) * Mathf.Exp(0f - num)).NaNFallback();
	}

	public static bool UniverseMatches(this ActionUniverse uni, ActionUniverse other)
	{
		if (uni == ActionUniverse.None || other == ActionUniverse.None)
		{
			return false;
		}
		if (uni == ActionUniverse.All || other == ActionUniverse.All)
		{
			return true;
		}
		return other == uni;
	}

	public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> range)
	{
		foreach (T item in range)
		{
			hashset.Add(item);
		}
	}

	public static float DistanceFromPointToLineSegment(Vector2 point, Vector2 lineEnd1, Vector2 lineEnd2)
	{
		return Mathf.Sqrt(SqrdDistanceFromPointToLineSegment(point, lineEnd1, lineEnd2));
	}

	public static float SqrdDistanceFromPointToLineSegment(Vector2 point, Vector2 lineEnd1, Vector2 lineEnd2)
	{
		float sqrMagnitude = (lineEnd1 - lineEnd2).sqrMagnitude;
		if ((double)sqrMagnitude == 0.0)
		{
			return (point - lineEnd1).magnitude;
		}
		float num = Mathf.Clamp01(Vector2.Dot(point - lineEnd1, lineEnd2 - lineEnd1) / sqrMagnitude);
		Vector2 vector = lineEnd1 + num * (lineEnd2 - lineEnd1);
		return (point - vector).sqrMagnitude;
	}

	public static T PickRandom<T>(this IList<T> collection)
	{
		if (collection == null)
		{
			return default(T);
		}
		if (collection.Count == 0)
		{
			return default(T);
		}
		return collection[UnityEngine.Random.Range(0, collection.Count)];
	}

	public static T PickRandom<T>(this IList<T> collection, int count)
	{
		if (collection == null)
		{
			return default(T);
		}
		if (collection.Count == 0)
		{
			return default(T);
		}
		return collection[UnityEngine.Random.Range(0, Math.Min(collection.Count, count))];
	}

	public static float Cosh(float x)
	{
		return (Mathf.Pow((float)Math.E, x) + Mathf.Pow((float)Math.E, 0f - x)) / 2f;
	}

	public static void LiquidMixProcess(BloodContainer container, Liquid a, Liquid b, Liquid result, float delta = 0.05f)
	{
		float amount = container.GetAmount(a);
		float amount2 = container.GetAmount(b);
		if (amount > delta && amount2 > delta)
		{
			container.RemoveLiquid(a, delta);
			container.RemoveLiquid(b, delta);
			container.AddLiquid(result, delta * 2f);
		}
	}

	public static void LiquidMixProcess(BloodContainer container, Liquid a, Liquid b, Liquid c, Liquid result, float delta = 0.05f)
	{
		float amount = container.GetAmount(a);
		float amount2 = container.GetAmount(b);
		float amount3 = container.GetAmount(c);
		if (amount > delta && amount2 > delta && amount3 > delta)
		{
			container.RemoveLiquid(a, delta);
			container.RemoveLiquid(b, delta);
			container.RemoveLiquid(c, delta);
			container.AddLiquid(result, delta * 3f);
		}
	}

	public static void LiquidMixProcess(BloodContainer container, Liquid a, Liquid b, Liquid c, Liquid d, Liquid result, float delta = 0.05f)
	{
		float amount = container.GetAmount(a);
		float amount2 = container.GetAmount(b);
		float amount3 = container.GetAmount(c);
		float amount4 = container.GetAmount(d);
		if (amount > delta && amount2 > delta && amount3 > delta && amount4 > delta)
		{
			container.RemoveLiquid(a, delta);
			container.RemoveLiquid(b, delta);
			container.RemoveLiquid(c, delta);
			container.RemoveLiquid(d, delta);
			container.AddLiquid(result, delta * 4f);
		}
	}

	public static void LiquidMixProcess(BloodContainer container, Liquid[] a, Liquid result, float delta = 0.05f)
	{
		for (int i = 0; i < a.Length; i++)
		{
			if (container.GetAmount(a[i]) < delta)
			{
				return;
			}
		}
		for (int j = 0; j < a.Length; j++)
		{
			container.RemoveLiquid(a[j], delta);
		}
		container.AddLiquid(result, delta * (float)a.Length);
	}

	public static void UpdateOutlineMaterial(ref MaterialPropertyBlock propertyBlock, SpriteRenderer outlineSpriteRenderer, Sprite targetSprite)
	{
		outlineSpriteRenderer.sprite = targetSprite;
		propertyBlock = new MaterialPropertyBlock();
		outlineSpriteRenderer.GetPropertyBlock(propertyBlock);
		Vector2 vector = new Vector2(outlineSpriteRenderer.sprite.texture.width, outlineSpriteRenderer.sprite.texture.height);
		Vector2 min = GetMin(outlineSpriteRenderer.sprite.uv, (Vector2 v) => v.sqrMagnitude);
		Vector2 vector2 = new Vector2(outlineSpriteRenderer.sprite.rect.width, outlineSpriteRenderer.sprite.rect.height);
		Vector4 value = new Vector4(min.x, min.y, vector2.x / vector.x, vector2.y / vector.y);
		propertyBlock.SetVector(ShaderProperties.Get("_AtlasTransform"), value);
		propertyBlock.SetTexture(ShaderProperties.Get("_MainTex"), outlineSpriteRenderer.sprite.texture);
		outlineSpriteRenderer.SetPropertyBlock(propertyBlock);
	}

	public static float NaNFallback(this float f, float fallback = 0f)
	{
		if (!float.IsNaN(f))
		{
			return f;
		}
		return fallback;
	}

	public static Vector2 NaNFallback(this Vector2 f, float fallback = 0f)
	{
		return new Vector2(f.x.NaNFallback(fallback), f.y.NaNFallback(fallback));
	}

	public static float RandomBetween(Vector2 range)
	{
		return UnityEngine.Random.Range(range.x, range.y);
	}

	public static Color ChangeRedToOrange(in Color color, float fuzzinessDegrees = 0.03f)
	{
		Color.RGBToHSV(color, out var H, out var _, out var _);
		if (Mathf.Abs(H - 0f) < fuzzinessDegrees || Mathf.Abs(H - 1f) < fuzzinessDegrees)
		{
			return new Color(0.87f, 0.6f, 0.18f, 1f);
		}
		return color;
	}

	public static void SetPointVelocity(this Rigidbody2D body, Vector2 vel, Vector2 position)
	{
		Vector2 vector = body.worldCenterOfMass - position;
		if (!(vel.sqrMagnitude < 1E-15f))
		{
			float num = 1f - Mathf.Abs(Vector2.Dot(vector, vel));
			float b = 0f - Vector2.Dot(Vector2.Perpendicular(vector), vel);
			body.velocity = Vector2.Lerp(body.velocity, vel, num);
			body.angularVelocity = Mathf.Lerp(body.angularVelocity, b, 1f - num);
		}
	}

	public static Bounds GetWorldSpaceBounds(this RectTransform rect)
	{
		rect.GetWorldCorners(getWorldSpaceBoundsBuffer);
		Vector3 vector = getWorldSpaceBoundsBuffer[0];
		Vector3 vector2 = getWorldSpaceBoundsBuffer[1];
		Vector3 vector3 = getWorldSpaceBoundsBuffer[3];
		return new Bounds(new Vector3((vector.x + vector3.x) / 2f, (vector.y + vector2.y) / 2f), new Vector3(Mathf.Abs(vector.x - vector3.x), Mathf.Abs(vector.y - vector2.y)));
	}

	public static string EscapeRichText(string v)
	{
		if (string.IsNullOrWhiteSpace(v))
		{
			return string.Empty;
		}
		string text = Regex.Replace(v, "\\bnoparse\\b", string.Empty);
		return "<noparse>" + text + "</noparse>";
	}

	public static void TransferEnergyFixedRate(PhysicalBehaviour a, PhysicalBehaviour b, float rate = 0.89f)
	{
		if (a.GetChargeWithWireResistance() > b.GetChargeWithWireResistance())
		{
			b.Charge = Mathf.Lerp(b.Charge, a.GetChargeWithWireResistance(), rate);
		}
		else
		{
			a.Charge = Mathf.Lerp(a.Charge, b.GetChargeWithWireResistance(), rate);
		}
	}

	public static void TransferEnergyDeltaTime(PhysicalBehaviour a, PhysicalBehaviour b, float rate, float dt)
	{
		TransferEnergyFixedRate(a, b, GetLerpFactorDeltaTime(rate, dt));
	}

	public static void AverageTemperature(PhysicalBehaviour a, PhysicalBehaviour b, float transferFactor = 0.01f)
	{
		if ((bool)a && (bool)b && a.SimulateTemperature && b.SimulateTemperature)
		{
			float num = GetMinHeatTransferSpeed(a, b) * transferFactor;
			float temperature = a.Temperature;
			float temperature2 = b.Temperature;
			a.Temperature = Mathf.Lerp(temperature, temperature2, num / a.GetHeatCapacity());
			b.Temperature = Mathf.Lerp(temperature2, temperature, num / b.GetHeatCapacity());
		}
	}

	public static float GetMinHeatTransferSpeed(PhysicalBehaviour a, PhysicalBehaviour b)
	{
		return Mathf.Min(a.Properties.HeatTransferSpeedMultiplier, b.Properties.HeatTransferSpeedMultiplier);
	}

	public static string GetHierachyPath(Transform transform)
	{
		if ((bool)transform.parent && transform.parent != transform.root)
		{
			return GetHierachyPath(transform.parent) + "/" + transform.name;
		}
		return transform.name;
	}

	public static IEnumerator DelayCoroutine(float delayInSeconds, Action action)
	{
		yield return new WaitForSeconds(delayInSeconds);
		action();
	}

	public static IEnumerator NextFrameCoroutine(Action action)
	{
		yield return new WaitForEndOfFrame();
		action();
	}

	public static float Triangle(float x)
	{
		return Math.Abs(Mod(x / (float)Math.PI - 0.5f, 2f) - 1f) * 2f - 1f;
	}

	public static async Task<T> HttpGet<T>(string uri)
	{
		using (HttpClient http = new HttpClient())
		{
			return JsonConvert.DeserializeObject<T>(await http.GetStringAsync(uri));
		}
	}

	public static async Task<HttpResponseMessage> HttpPost(string uri, string body)
	{
		using (HttpClient http = new HttpClient())
		{
			StringContent content = new StringContent(body, Encoding.UTF8);
			return await http.PostAsync(uri, content);
		}
	}

	public static async Task<byte[]> HttpDownload(string uri)
	{
		using (HttpClient http = new HttpClient())
		{
			return await http.GetByteArrayAsync(uri);
		}
	}

	public static float PreferenceToCelsius(float i)
	{
		switch (UserPreferenceManager.Current.TemperatureUnit)
		{
		case TemperatureUnit.Celsius:
			return i;
		case TemperatureUnit.Fahrenheit:
			return FahrenheitToCelsius(i);
		case TemperatureUnit.Kelvin:
			return KelvinToCelsius(i);
		default:
			return i;
		}
	}

	public static float CelsiusToPreference(float i)
	{
		switch (UserPreferenceManager.Current.TemperatureUnit)
		{
		case TemperatureUnit.Celsius:
			return i;
		case TemperatureUnit.Fahrenheit:
			return CelsiusToFahrenheit(i);
		case TemperatureUnit.Kelvin:
			return CelsiusToKelvin(i);
		default:
			return i;
		}
	}

	public static float CelsiusToFahrenheit(float i)
	{
		return i * 9f / 5f + 32f;
	}

	public static float CelsiusToKelvin(float i)
	{
		return i + 273.15f;
	}

	public static float FahrenheitToCelsius(float i)
	{
		return (i - 32f) * (5f / 9f);
	}

	public static float KelvinToCelsius(float i)
	{
		return i - 273.15f;
	}

	public static string GetTemperatureUnitSuffix(TemperatureUnit unit)
	{
		switch (unit)
		{
		case TemperatureUnit.Celsius:
			return "°C";
		case TemperatureUnit.Fahrenheit:
			return "°F";
		case TemperatureUnit.Kelvin:
			return "K";
		default:
			return string.Empty;
		}
	}

	public static void DrawCross(Vector3 center, float size, Color color, float duration)
	{
		Debug.DrawLine(center + Vector3.one * (0f - size), center + Vector3.one * size, color, duration);
		Debug.DrawLine(center + new Vector3(0f - size, size), center + new Vector3(size, 0f - size), color, duration);
	}

	internal static byte[] FBSF(string bsf)
	{
		return Convert.FromBase64String(bsf);
	}

	public static byte[] GetMD5(byte[] bytes)
	{
		MD5 mD = MD5.Create();
		byte[] result = mD.ComputeHash(bytes);
		mD.Dispose();
		return result;
	}

	public static byte[] GetMD5(string input)
	{
		MD5 mD = MD5.Create();
		byte[] result = mD.ComputeHash(Encoding.ASCII.GetBytes(input));
		mD.Dispose();
		return result;
	}

	public static string GetMD5AsString(string input)
	{
		return string.Join(null, from b in GetMD5(input)
			select b.ToString("X2"));
	}

	public static byte[] GetMD5ForFile(string filePath)
	{
		using (FileStream inputStream = File.Open(filePath, FileMode.Open))
		{
			MD5 mD = MD5.Create();
			byte[] result = mD.ComputeHash(inputStream);
			mD.Dispose();
			return result;
		}
	}

	public static Vector3 GetPerlin2Mapped(float x, float y)
	{
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.PerlinNoise(x, y) * 2f - 1f;
		zero.y = Mathf.PerlinNoise(y, x) * 2f - 1f;
		return zero;
	}

	public static float MapRange(float lower1, float upper1, float lower2, float upper2, float value)
	{
		return lower2 + (value - lower1) * (upper2 - lower2) / (upper1 - lower1);
	}

	public static T GetMin<T>(ICollection<T> collection, Func<T, float> singleFunc)
	{
		float num = float.MaxValue;
		T result = default(T);
		foreach (T item in collection)
		{
			float num2 = singleFunc(item);
			if (num2 < num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	[Obsolete]
	public static SpriteRenderer GetOutlineSprite(Transform transform)
	{
		Transform transform2 = transform.Find("Outline");
		if ((bool)transform2)
		{
			return transform2.GetComponent<SpriteRenderer>();
		}
		return null;
	}

	public static bool IsSerialisableType(Type type)
	{
		if (type.HasElementType)
		{
			return IsSerialisableType(type.GetElementType());
		}
		if (!type.IsValueType && !type.IsPrimitive && !(type == typeof(string)) && !(type == typeof(float)) && !(type == typeof(int)))
		{
			return type == typeof(bool);
		}
		return true;
	}

	public static string GetFormattedByteString(ulong b)
	{
		if (b < 1000)
		{
			return b + " B";
		}
		if (b < 1000000)
		{
			return b / 1000uL + " kB";
		}
		if (b < 1000000000)
		{
			return b / 1000000uL + " MB";
		}
		return b / 1000000000uL + " GB";
	}

	public static LaserHit MaterialAwareRaycast(Vector2 origin, Vector2 dir, float maxDistance, LayerMask layers, int maxSteps = 8, int depth = 0, float currentDistance = 0f)
	{
		if (maxSteps <= 0 || maxDistance <= 0f)
		{
			return new LaserHit(null, origin, null, depth);
		}
		RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, dir, maxDistance, layers);
		LaserHit result;
		if (!raycastHit2D || !raycastHit2D.transform)
		{
			Debug.DrawLine(origin, origin + dir * maxDistance, Color.red);
			currentDistance += maxDistance;
			result = new LaserHit(null, origin + dir * maxDistance, null, depth);
			result.totalDistance = currentDistance;
			return result;
		}
		Debug.DrawLine(origin, raycastHit2D.point, Color.green);
		if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
		{
			if (value.AbsorbsLasers)
			{
				result = new LaserHit(raycastHit2D, raycastHit2D.point, value, depth);
				result.totalDistance = currentDistance + raycastHit2D.distance;
				return result;
			}
			return MaterialAwareRaycast(raycastHit2D.point + dir * 0.01f, dir, maxDistance - raycastHit2D.distance, layers, maxSteps - 1, depth + 1, currentDistance + raycastHit2D.distance);
		}
		result = new LaserHit(raycastHit2D, raycastHit2D.point, null, depth);
		result.totalDistance = currentDistance + raycastHit2D.distance;
		return result;
	}

	public static float Snap(float v, float snap)
	{
		if (Mathf.Approximately(snap, 0f))
		{
			return v;
		}
		return Mathf.Round(v / snap) * snap;
	}

	public static Vector3 Snap(Vector3 v, float snap)
	{
		if (Mathf.Approximately(snap, 0f))
		{
			return v;
		}
		return new Vector3(Snap(v.x, snap), Snap(v.y, snap), Snap(v.z, snap));
	}

	public static Vector3 Rotate(Vector2 point, float degrees, Vector2 pivot = default(Vector2))
	{
		degrees *= (float)Math.PI / 180f;
		point -= pivot;
		point = new Vector2(Mathf.Cos(degrees) * point.x - Mathf.Sin(degrees) * point.y, Mathf.Sin(degrees) * point.x + Mathf.Cos(degrees) * point.y);
		point += pivot;
		return point;
	}

	public static float NormaliseAngle(float degrees)
	{
		return Mod(degrees, 360f);
	}

	public static float Mod(float a, float b)
	{
		return a - Mathf.Floor(a / b) * b;
	}

	public static void GetLaserEndPoint(Vector2 origin, Vector2 dir, ref List<LaserHit> list, LayerMask layers, float maximumDistance, uint maxIterations = 64u)
	{
		if (list.Count >= maxIterations)
		{
			return;
		}
		LaserHit laserHit = MaterialAwareRaycast(origin, dir, maximumDistance, layers);
		if (laserHit.hit.HasValue)
		{
			RaycastHit2D value = laserHit.hit.Value;
			if ((bool)laserHit.physicalBehaviour)
			{
				PhysicalBehaviour physicalBehaviour = laserHit.physicalBehaviour;
				if (!physicalBehaviour.AbsorbsLasers)
				{
					addToList(ref list);
					return;
				}
				list.Add(new LaserHit(value, value.point, physicalBehaviour, list.Count));
				if (physicalBehaviour.ReflectsLasers)
				{
					Vector2 point = value.point;
					Vector2 dir2 = Vector2.Reflect(dir, value.normal);
					GetLaserEndPoint(point, dir2, ref list, layers, maximumDistance - laserHit.totalDistance);
				}
			}
			else
			{
				list.Add(new LaserHit(value, value.point, null, list.Count));
			}
		}
		else
		{
			addToList(ref list);
		}
		void addToList(ref List<LaserHit> l)
		{
			l.Add(new LaserHit(null, origin + dir.normalized * maximumDistance, null, l.Count));
		}
	}

	public static bool IsFamilyShared()
	{
		if (SteamApps.AppOwner.IsValid && SteamClient.SteamId.IsValid)
		{
			return (ulong)SteamApps.AppOwner != (ulong)SteamClient.SteamId;
		}
		return true;
	}

	public static void OpenURL(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		try
		{
			if (SteamUtils.IsOverlayEnabled)
			{
				SteamFriends.OpenWebOverlay(url);
			}
			else
			{
				Application.OpenURL(url);
			}
		}
		catch (NullReferenceException)
		{
			Application.OpenURL(url);
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Could not open URL: {0}", ex2.Message);
		}
	}

	public static Sprite LoadSprite(string fullPath, FilterMode mode = FilterMode.Bilinear, float pixelsPerUnit = 35f, bool markNonReadable = true)
	{
		Texture2D texture2D = LoadTexture(fullPath, mode, markNonReadable);
		if (texture2D == null)
		{
			return null;
		}
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), 0.5f * Vector2.one, pixelsPerUnit);
	}

	public static Texture2D LoadTexture(string fullPath, FilterMode mode = FilterMode.Bilinear, bool markNonReadable = true)
	{
		byte[] data;
		try
		{
			data = File.ReadAllBytes(fullPath);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return null;
		}
		Texture2D texture2D = new Texture2D(0, 0);
		if (!texture2D.LoadImage(data, markNonReadable))
		{
			throw new InvalidDataException("Texture at " + fullPath + " cannot be loaded as a PNG");
		}
		texture2D.filterMode = mode;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		return texture2D;
	}

	public static AudioClip FileToAudioClip(string path)
	{
		switch (Path.GetExtension(path))
		{
		case ".mp3":
			return DecodeMP3File();
		case ".wav":
			return DecodeWAVFile();
		default:
			return DecodeUnknownFile();
		}
		AudioClip DecodeMP3File()
		{
			using (WaveStream stream2 = new Mp3FileReader(path))
			{
				return WaveStreamToAudioClip(stream2);
			}
		}
		AudioClip DecodeUnknownFile()
		{
			using (WaveStream stream4 = new AudioFileReader(path))
			{
				return WaveStreamToAudioClip(stream4);
			}
		}
		AudioClip DecodeWAVFile()
		{
			using (WaveStream stream3 = new WaveFileReader(path))
			{
				return WaveStreamToAudioClip(stream3);
			}
		}
		AudioClip WaveStreamToAudioClip(WaveStream stream)
		{
			bool num = stream.WaveFormat.Channels == 1;
			StereoToMonoProvider16 stereoToMonoProvider = (num ? null : new StereoToMonoProvider16(stream));
			byte[] array = new byte[stream.Length];
			int num2 = (num ? stream.Read(array, 0, array.Length) : stereoToMonoProvider.Read(array, 0, array.Length));
			short[] array2 = new short[num2 / 2];
			Buffer.BlockCopy(array, 0, array2, 0, num2);
			AudioClip audioClip = AudioClip.Create(path, array2.Length, 1, stream.WaveFormat.SampleRate, stream: false);
			audioClip.SetData(array2.Select((short b) => (float)b / 32767f).ToArray(), 0);
			return audioClip;
		}
	}

	public static void GetTopMostLayer(SpriteRenderer a, SpriteRenderer b, out int layerId, out int sortingOrder)
	{
		if (!b && !a)
		{
			layerId = SortingLayer.NameToID("Top");
			sortingOrder = 100;
			return;
		}
		int num = (a ? SortingLayer.GetLayerValueFromID(a.sortingLayerID) : int.MinValue);
		int num2 = (a ? a.sortingOrder : int.MinValue);
		int num3 = (b ? SortingLayer.GetLayerValueFromID(b.sortingLayerID) : int.MinValue);
		int num4 = (b ? b.sortingOrder : int.MinValue);
		if (num > num3)
		{
			layerId = a.sortingLayerID;
			sortingOrder = num2;
		}
		else
		{
			layerId = b.sortingLayerID;
			sortingOrder = num4;
		}
	}

	public static byte br(this Color color)
	{
		return (byte)(color.r * 255f);
	}

	public static byte bg(this Color color)
	{
		return (byte)(color.g * 255f);
	}

	public static byte bb(this Color color)
	{
		return (byte)(color.b * 255f);
	}

	public static void FixColliders(this GameObject instance)
	{
		Collider2D[] components = instance.GetComponents<Collider2D>();
		for (int i = 0; i < components.Length; i++)
		{
			UnityEngine.Object.Destroy(components[i]);
		}
		instance.AddComponent<PolygonCollider2D>();
		if (instance.TryGetComponent<PhysicalBehaviour>(out var component))
		{
			component.ResetColliderArray();
		}
	}

	public static T GetOrAddComponent<T>(this GameObject instance) where T : Component
	{
		if (instance.TryGetComponent<T>(out var component))
		{
			return component;
		}
		return instance.AddComponent<T>();
	}

	public static bool HasComponent<T>(this GameObject instance) where T : Component
	{
		T component;
		return instance.TryGetComponent<T>(out component);
	}

	public static void SendAllChannelIsolatedActivation(MonoBehaviour other)
	{
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			ushort channel = ActivationPropagation.AllChannels[i];
			other.SendMessage("IsolatedActivation", new ActivationPropagation(other.transform.root, channel), SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void SendAllChannelContinuousIsolatedActivation(MonoBehaviour other)
	{
		if (!Global.main.Paused && !Global.main.GetPausedMenu())
		{
			for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
			{
				ushort channel = ActivationPropagation.AllChannels[i];
				other.SendMessage("IsolatedContinuousActivation", new ActivationPropagation(other.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	[Obsolete]
	public static T LoadAddressableSync<T>(string key) where T : class
	{
		T received = null;
		AsyncOperationHandle<T> asyncOperationHandle = Addressables.LoadAssetAsync<T>("key");
		asyncOperationHandle.Completed += delegate(AsyncOperationHandle<T> o)
		{
			received = o.Result;
		};
		while (!asyncOperationHandle.IsDone)
		{
			Thread.Sleep(16);
		}
		return received;
	}

	public static void AssertValidThumbnail(string path)
	{
		FileInfo fileInfo = new FileInfo(path);
		if (!fileInfo.Exists)
		{
			throw new Exception("Thumbnail file cannot be found");
		}
		if (!fileInfo.Extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) && !fileInfo.Extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
		{
			throw new Exception("Thumbnail extension (\"" + fileInfo.Extension + "\") is invalid: only jpg and png are allowed");
		}
		if (fileInfo.Length > 1000000)
		{
			throw new Exception("Thumbnail exceeds 1 MB. Please compress or resize the image.");
		}
	}

	public static float GetLerpFactorDeltaTime(float lerpFactor, float deltaTime)
	{
		return 1f - Mathf.Pow(1f - lerpFactor, deltaTime);
	}

	public static float CalculateBreakForceForCable(AnchoredJoint2D joint, float baseStrength, float referenceMass = 5f, float massInfluence = 1f)
	{
		if (baseStrength == float.PositiveInfinity)
		{
			return baseStrength;
		}
		Rigidbody2D attachedRigidbody = joint.attachedRigidbody;
		Rigidbody2D connectedBody = joint.connectedBody;
		float num = (attachedRigidbody ? attachedRigidbody.mass : referenceMass);
		float b = (connectedBody ? connectedBody.mass : num);
		float num2 = Mathf.Max(num, b);
		return baseStrength * Mathf.Lerp(num2 / referenceMass, 1f, 1f - Mathf.Clamp01(massInfluence));
	}

	public static void SetLayer(this GameObject gm, int layer, bool includeChildren = true)
	{
		gm.layer = layer;
		if (!includeChildren)
		{
			return;
		}
		foreach (Transform item in gm.transform)
		{
			if (item.gameObject != gm && item.gameObject.CompareTag("RecursiveLayerChild"))
			{
				item.gameObject.SetLayer(layer);
			}
		}
	}

	public static float GetMaxImpulse(ContactPoint2D[] buffer, int count)
	{
		float num = float.MinValue;
		for (int i = 0; i < count; i++)
		{
			float normalImpulse = buffer[i].normalImpulse;
			if (!float.IsNaN(normalImpulse))
			{
				num = Mathf.Max(num, normalImpulse);
			}
		}
		return num;
	}

	public static float GetAverageImpulse(ContactPoint2D[] buffer, int count)
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			float normalImpulse = buffer[i].normalImpulse;
			if (!float.IsNaN(normalImpulse) && !float.IsInfinity(normalImpulse))
			{
				num += normalImpulse;
				num2++;
			}
		}
		return num / (float)num2;
	}

	public static float GetAverageImpulseRemoveOutliers(ContactPoint2D[] buffer, int count, float outlierThreshold = 1f)
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			float normalImpulse = buffer[i].normalImpulse;
			if (!float.IsNaN(normalImpulse) && !float.IsInfinity(normalImpulse))
			{
				num += normalImpulse;
				num2++;
			}
		}
		float num3 = num / (float)num2;
		if (count >= 2 && outlierThreshold > float.Epsilon)
		{
			for (int j = 0; j < count - 1; j++)
			{
				float normalImpulse2 = buffer[j].normalImpulse;
				if (Mathf.Abs(normalImpulse2 - num3) > outlierThreshold)
				{
					num -= normalImpulse2;
					num2--;
				}
			}
			return num / (float)num2;
		}
		return num3;
	}

	public static float GetMinImpulse(ContactPoint2D[] buffer, int count)
	{
		float num = float.MaxValue;
		for (int i = 0; i < count; i++)
		{
			float normalImpulse = buffer[i].normalImpulse;
			if (!float.IsNaN(normalImpulse))
			{
				num = Mathf.Min(num, normalImpulse);
			}
		}
		return num;
	}

	public static ContactPoint2D GetFirstValidContact(ContactPoint2D[] buffer, int count)
	{
		for (int i = 0; i < count; i++)
		{
			ContactPoint2D result = buffer[i];
			if (!float.IsNaN(result.normalImpulse) && !float.IsInfinity(result.normalImpulse))
			{
				return result;
			}
		}
		return buffer[0];
	}

	public static bool IsPointInsideBounds(this Bounds bounds, Vector2 point)
	{
		if (point.x > bounds.min.x && point.x < bounds.max.x)
		{
			if (point.y > bounds.min.y)
			{
				return point.y < bounds.max.y;
			}
			return false;
		}
		return false;
	}

	public static bool ContainsExpanded(this Bounds bounds, Vector2 point, float expansion = 0f)
	{
		if (point.x > bounds.min.x - expansion && point.x < bounds.max.x + expansion && point.y > bounds.min.y - expansion)
		{
			return point.y < bounds.max.y + expansion;
		}
		return false;
	}

	public static bool AreHdrRenderTexturesSupported()
	{
		return SystemInfo.IsFormatSupported(SystemInfo.GetGraphicsFormat(DefaultFormat.HDR), FormatUsage.Render);
	}

	private static bool IsFirstOfTypeInSelection<T>(T obj) where T : MonoBehaviour
	{
		Type typeFromHandle = typeof(T);
		Transform transform = obj.transform;
		for (int i = 0; i < SelectionController.Main.SelectedObjects.Count; i++)
		{
			if (SelectionController.Main.SelectedObjects[i].TryGetComponent<T>(out var component) && component.GetType() == typeFromHandle)
			{
				return transform == component.transform;
			}
		}
		return false;
	}

	public static void OpenFloatInputDialog<T>(float defaultValue, T callingBehaviour, Action<T, float> setValueFunction, string text, string placeholder) where T : MonoBehaviour
	{
		if (SelectionController.Main.SelectedObjects.Count > 1)
		{
			if (!IsFirstOfTypeInSelection(callingBehaviour))
			{
				return;
			}
			PhysicalBehaviour[] old = SelectionController.Main.SelectedObjects.ToArray();
			DialogBox dialog2 = null;
			dialog2 = DialogBoxManager.TextEntry(text, placeholder, new DialogButton("Apply", true, delegate
			{
				for (int i = 0; i < old.Length; i++)
				{
					if (old[i].TryGetComponent<T>(out var component) && component.GetType() == typeof(T))
					{
						setValue(dialog2, component);
					}
				}
			}), new DialogButton("Cancel", true));
			dialog2.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			dialog2.InputField.text = defaultValue.ToString();
			dialog2.InputField.Select();
		}
		else
		{
			DialogBox dialog = null;
			dialog = DialogBoxManager.TextEntry(text, placeholder, new DialogButton("Apply", true, delegate
			{
				setValue(dialog, callingBehaviour);
			}), new DialogButton("Cancel", true));
			dialog.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			dialog.InputField.text = defaultValue.ToString();
			dialog.InputField.Select();
		}
		void setValue(DialogBox d, T behaviour)
		{
			if (float.TryParse(d.InputField.text, out var result))
			{
				((<>c__DisplayClass95_0<T>)this).setValueFunction(behaviour, result);
			}
			else
			{
				UISoundBehaviour.Main.Warning();
				NotificationControllerBehaviour.Show("Invalid decimal input");
			}
		}
	}

	public static void OpenIntInputDialog<T>(int defaultValue, T callingBehaviour, Action<T, int> setValueFunction, string text, string placeholder) where T : MonoBehaviour
	{
		if (SelectionController.Main.SelectedObjects.Count > 1)
		{
			if (!IsFirstOfTypeInSelection(callingBehaviour))
			{
				return;
			}
			PhysicalBehaviour[] old = SelectionController.Main.SelectedObjects.ToArray();
			DialogBox dialog2 = null;
			dialog2 = DialogBoxManager.TextEntry(text, placeholder, new DialogButton("Apply", true, delegate
			{
				for (int i = 0; i < old.Length; i++)
				{
					if (old[i].TryGetComponent<T>(out var component) && component.GetType() == typeof(T))
					{
						setValue(dialog2, component);
					}
				}
			}), new DialogButton("Cancel", true));
			dialog2.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			dialog2.InputField.text = defaultValue.ToString();
			dialog2.InputField.Select();
		}
		else
		{
			DialogBox dialog = null;
			dialog = DialogBoxManager.TextEntry(text, placeholder, new DialogButton("Apply", true, delegate
			{
				setValue(dialog, callingBehaviour);
			}), new DialogButton("Cancel", true));
			dialog.InputField.contentType = TMP_InputField.ContentType.IntegerNumber;
			dialog.InputField.text = defaultValue.ToString();
			dialog.InputField.Select();
		}
		void setValue(DialogBox d, T behaviour)
		{
			if (int.TryParse(d.InputField.text, out var result))
			{
				((<>c__DisplayClass96_0<T>)this).setValueFunction(behaviour, result);
			}
			else
			{
				UISoundBehaviour.Main.Warning();
				NotificationControllerBehaviour.Show("Invalid decimal input");
			}
		}
	}

	public static void OpenTextInputDialog<T>(string defaultValue, T callingBehaviour, Action<T, string> setValueFunction, string message, string placeholder) where T : MonoBehaviour
	{
		if (SelectionController.Main.SelectedObjects.Count > 1)
		{
			if (!IsFirstOfTypeInSelection(callingBehaviour))
			{
				return;
			}
			PhysicalBehaviour[] old = SelectionController.Main.SelectedObjects.ToArray();
			DialogBox dialog2 = null;
			dialog2 = DialogBoxManager.TextEntry(message, placeholder, new DialogButton("Apply", true, delegate
			{
				for (int i = 0; i < old.Length; i++)
				{
					if (old[i].TryGetComponent<T>(out var component) && component.GetType() == typeof(T))
					{
						setValue(dialog2, component);
					}
				}
			}), new DialogButton("Cancel", true));
			dialog2.InputField.contentType = TMP_InputField.ContentType.Standard;
			dialog2.InputField.text = defaultValue.ToString();
			dialog2.InputField.Select();
		}
		else
		{
			DialogBox dialog = null;
			dialog = DialogBoxManager.TextEntry(message, placeholder, new DialogButton("Apply", true, delegate
			{
				setValue(dialog, callingBehaviour);
			}), new DialogButton("Cancel", true));
			dialog.InputField.contentType = TMP_InputField.ContentType.Standard;
			dialog.InputField.text = defaultValue.ToString();
			dialog.InputField.Select();
		}
		void setValue(DialogBox d, T behaviour)
		{
			((<>c__DisplayClass97_0<T>)this).setValueFunction(behaviour, d.InputField.text);
		}
	}

	public static void OpenColourInputDialog<T>(Color initialValue, string title, string description, Action<T, Color> setValueFunction) where T : MonoBehaviour
	{
		ColorpickerDialogBehaviour instance = ColorpickerDialogBehaviour.Instance;
		T component2;
		PhysicalBehaviour[] selected = SelectionController.Main.SelectedObjects.Where((PhysicalBehaviour p) => p.TryGetComponent<T>(out component2)).ToArray();
		instance.Title.text = title;
		instance.Description.text = description;
		instance.Open(initialValue);
		instance.OnApply.RemoveAllListeners();
		instance.OnCancel.RemoveAllListeners();
		instance.OnApply.AddListener(delegate(Color c)
		{
			PhysicalBehaviour[] array = selected;
			foreach (PhysicalBehaviour physicalBehaviour in array)
			{
				if ((bool)physicalBehaviour && physicalBehaviour.TryGetComponent<T>(out var component))
				{
					setValueFunction(component, c);
				}
			}
		});
	}

	public static Vector2 CastRayUntilOutside(GameObject targetBody, Vector2 minPoint, Vector2 direction, float distance, Collider2D[] buffer = null)
	{
		Vector2 b = minPoint + 1.1f * distance * direction;
		Collider2D[] array = buffer ?? colliderBuffer;
		int num = Mathf.Clamp(4, 32, Mathf.RoundToInt(distance * 5f));
		Vector2? vector = null;
		Vector2? vector2 = null;
		for (int i = 0; i < num; i++)
		{
			float t = (float)i / (float)(num - 1);
			Vector2 vector3 = Vector2.Lerp(minPoint, b, t);
			int num2 = Physics2D.OverlapPointNonAlloc(vector3, array);
			for (int j = 0; j < num2; j++)
			{
				if ((bool)array[j] && array[j].gameObject == targetBody)
				{
					if (!vector.HasValue)
					{
						vector = vector3;
					}
					else
					{
						vector2 = vector3;
					}
					break;
				}
			}
		}
		if (!vector.HasValue && !vector2.HasValue)
		{
			Debug.LogWarning("Attempt to CastRayUntilOutside but the ray never reaches the target body");
			return minPoint;
		}
		if (vector.HasValue && vector2.HasValue)
		{
			return vector2.Value;
		}
		if (vector.HasValue && !vector2.HasValue)
		{
			return vector.Value;
		}
		if (!vector.HasValue && vector2.HasValue)
		{
			Debug.LogError("Attempt to CastRayUntilOutside but only the last occupied position is valid somehow. This should NEVER HAPPEN.");
			return vector2.Value;
		}
		return minPoint;
	}

	public static async Task<T> TaskTimeout<T>(Task<T> task, TimeSpan timeout)
	{
		using (CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource())
		{
			if (await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)) == task)
			{
				timeoutCancellationTokenSource.Cancel();
				return await task;
			}
			throw new TimeoutException("The operation has timed out.");
		}
	}

	public static int ComputeConvexHull(Vector2[] points, List<Vector2> hull)
	{
		int num = points.Length;
		int num2 = 0;
		Array.Sort(points, (Vector2 a, Vector2 b) => (!(a.x > b.x)) ? 1 : (-1));
		hull.Clear();
		int i;
		for (i = 0; i < num; i++)
		{
			while (num2 >= 2 && ccw(hull[num2 - 2], hull[num2 - 1], points[i]) <= 0f)
			{
				num2--;
				hull.RemoveAt(num2);
			}
			hull.Add(points[i]);
			num2++;
		}
		i = num - 2;
		int num3 = num2 + 1;
		while (i >= 0)
		{
			while (num2 >= num3 && ccw(hull[num2 - 2], hull[num2 - 1], points[i]) <= 0f)
			{
				num2--;
				hull.RemoveAt(num2);
			}
			hull.Add(points[i]);
			num2++;
			i--;
		}
		return num2;
	}

	private static float ccw(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		return (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
	}
}
