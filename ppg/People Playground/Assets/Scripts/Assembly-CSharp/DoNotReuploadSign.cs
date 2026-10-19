using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct DoNotReuploadSign
{
	public static readonly byte[] Magic = Encoding.UTF8.GetBytes("\ud83d\udc0d\ud83c\udde9\ud83c\uddf4_\ud83c\uddf3\ud83c\uddf4\ud83c\uddf9_\ud83c\uddf7\ud83c\uddea\ud83c\uddfa\ud83c\uddf5\ud83c\uddf1\ud83c\uddf4\ud83c\udde6\ud83c\udde9\ud83d\udc80");

	public static void Mark(string path)
	{
		using (FileStream output = new FileStream(path, FileMode.Append))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(Magic);
			}
		}
	}

	public static bool HasMark(string path)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			fileStream.Seek(-Magic.Length, SeekOrigin.End);
			byte[] array = new byte[Magic.Length];
			fileStream.Read(array, 0, Magic.Length);
			if (array.SequenceEqual(Magic))
			{
				return true;
			}
		}
		return false;
	}
}
