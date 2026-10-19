using System;
using System.IO;

[Serializable]
public class Contraption
{
	public const string FileExtension = ".jaap";

	public ObjectState[] ObjectStates;

	public Contraption()
	{
	}

	public Contraption(ObjectState[] objectStates)
	{
		ObjectStates = objectStates;
	}

	public static string GetPath(string name)
	{
		string text = "Contraptions/" + name + ".jaap";
		if (File.Exists(text))
		{
			return text;
		}
		return "Contraptions/" + name + "/" + name + ".jaap";
	}
}
