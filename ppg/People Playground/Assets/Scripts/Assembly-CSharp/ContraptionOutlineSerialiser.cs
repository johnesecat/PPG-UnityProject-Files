using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ContraptionOutlineSerialiser
{
	public const string OutlineExtension = ".outline";

	public static ContraptionOutline GenerateOutline(IEnumerable<PhysicalBehaviour> physicalBehaviours, Vector3 center)
	{
		RotatedRectangle[] array = new RotatedRectangle[physicalBehaviours.Count()];
		ContraptionOutline result = new ContraptionOutline(array);
		int num = 0;
		foreach (PhysicalBehaviour physicalBehaviour in physicalBehaviours)
		{
			if ((bool)physicalBehaviour)
			{
				Vector3 lossyScale = physicalBehaviour.transform.lossyScale;
				Vector3 size = physicalBehaviour.spriteRenderer.sprite.bounds.size;
				Vector2 size2 = new Vector2(size.x * lossyScale.x, size.y * lossyScale.y);
				Vector3 vector = physicalBehaviour.transform.position - center;
				array[num] = new RotatedRectangle(vector, size2, vector, physicalBehaviour.transform.eulerAngles.z);
				num++;
			}
		}
		return result;
	}

	public static ContraptionOutline LoadOutline(string path)
	{
		List<RotatedRectangle> list = new List<RotatedRectangle>();
		using (FileStream fileStream = new FileStream(path, FileMode.Open))
		{
			using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
			{
				try
				{
					int num = 0;
					while (!streamReader.EndOfStream)
					{
						num++;
						string text = streamReader.ReadLine();
						if (num != 1)
						{
							string[] array = text.Replace(',', '.').Split(' ');
							if (array.Length < 5)
							{
								throw new Exception($"Malformed at line {num}. Expected at least 5 segments, got {array.Length}: {text}");
							}
							float x = float.Parse(array[0], CultureInfo.InvariantCulture);
							float y = float.Parse(array[1], CultureInfo.InvariantCulture);
							float x2 = float.Parse(array[2], CultureInfo.InvariantCulture);
							float y2 = float.Parse(array[3], CultureInfo.InvariantCulture);
							float angleDegrees = float.Parse(array[4], CultureInfo.InvariantCulture);
							RotatedRectangle item = new RotatedRectangle(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x, y), angleDegrees);
							list.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Failed to deserialise outline: {0}", ex);
				}
				finally
				{
					streamReader.Close();
					streamReader.Dispose();
					fileStream.Dispose();
				}
				return new ContraptionOutline(list.ToArray());
			}
		}
	}

	public static void SaveOutline(string saveName, GameObject[] gameObjects, Vector3 center)
	{
		ContraptionOutline contraptionOutline = GenerateOutline(from a in gameObjects
			select a.GetComponent<PhysicalBehaviour>() into a
			where a
			select a, center);
		string path = "Contraptions/" + saveName + "/";
		if (!Directory.Exists("Contraptions/"))
		{
			Directory.CreateDirectory("Contraptions/");
		}
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		using (FileStream fileStream = new FileStream("Contraptions/" + saveName + "/" + saveName + ".outline", FileMode.Create))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
			{
				try
				{
					streamWriter.WriteLine("1.27.5");
					RotatedRectangle[] rectangles = contraptionOutline.Rectangles;
					for (int i = 0; i < rectangles.Length; i++)
					{
						RotatedRectangle rotatedRectangle = rectangles[i];
						streamWriter.Write(round(rotatedRectangle.Center.x).ToString(CultureInfo.InvariantCulture));
						streamWriter.Write(' ');
						streamWriter.Write(round(rotatedRectangle.Center.y).ToString(CultureInfo.InvariantCulture));
						streamWriter.Write(' ');
						streamWriter.Write(round(rotatedRectangle.Size.x).ToString(CultureInfo.InvariantCulture));
						streamWriter.Write(' ');
						streamWriter.Write(round(rotatedRectangle.Size.y).ToString(CultureInfo.InvariantCulture));
						streamWriter.Write(' ');
						streamWriter.Write(round(rotatedRectangle.AngleDegrees).ToString(CultureInfo.InvariantCulture));
						streamWriter.Write('\n');
					}
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Failed to serialise outline: {0}", ex);
				}
				finally
				{
					streamWriter.Close();
					streamWriter.Dispose();
					fileStream.Dispose();
				}
			}
		}
		float round(float x)
		{
			return Mathf.Round(x * 100f) / 100f;
		}
	}
}
