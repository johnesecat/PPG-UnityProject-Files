using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Ceras.Formatters
{
	internal class BitmapFormatter : IFormatter<Bitmap>, IFormatter
	{
		[ThreadStatic]
		private static MemoryStream _sharedMemoryStream;

		private CerasSerializer _ceras;

		private BitmapMode BitmapMode => _ceras.Config.Advanced.BitmapMode;

		public BitmapFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(Bitmap));
		}

		public void Serialize(ref byte[] buffer, ref int offset, Bitmap img)
		{
			if (_sharedMemoryStream == null)
			{
				_sharedMemoryStream = new MemoryStream(204800);
			}
			MemoryStream sharedMemoryStream = _sharedMemoryStream;
			sharedMemoryStream.Position = 0L;
			ImageFormat format = BitmapModeToImgFormat(BitmapMode);
			img.Save(sharedMemoryStream, format);
			long length = sharedMemoryStream.Length;
			if (length > int.MaxValue)
			{
				throw new InvalidOperationException("image too large");
			}
			int num = (int)length;
			sharedMemoryStream.Position = 0L;
			byte[] buffer2 = sharedMemoryStream.GetBuffer();
			SerializerBinary.WriteUInt32Fixed(ref buffer, ref offset, (uint)num);
			if (num > 0)
			{
				SerializerBinary.EnsureCapacity(ref buffer, offset, num);
				SerializerBinary.FastCopy(buffer2, 0, buffer, offset, num);
			}
			offset += num;
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Bitmap img)
		{
			int num = (int)SerializerBinary.ReadUInt32Fixed(buffer, ref offset);
			if (_sharedMemoryStream == null)
			{
				_sharedMemoryStream = new MemoryStream(num);
			}
			else if (_sharedMemoryStream.Capacity < num)
			{
				_sharedMemoryStream.Capacity = num;
			}
			MemoryStream sharedMemoryStream = _sharedMemoryStream;
			if (num < 0)
			{
				throw new InvalidOperationException($"Invalid bitmap size: {num} bytes");
			}
			if (num == 0)
			{
				img = null;
				return;
			}
			sharedMemoryStream.SetLength(num);
			sharedMemoryStream.Position = 0L;
			byte[] buffer2 = sharedMemoryStream.GetBuffer();
			SerializerBinary.FastCopy(buffer, offset, buffer2, 0, num);
			sharedMemoryStream.Position = 0L;
			img = new Bitmap(sharedMemoryStream);
			offset += num;
		}

		private static ImageFormat BitmapModeToImgFormat(BitmapMode mode)
		{
			switch (mode)
			{
			case BitmapMode.DontSerializeBitmaps:
				throw new InvalidOperationException("You need to set 'config.Advanced.BitmapMode' to any setting other than 'DontSerializeBitmaps'. Otherwise you need to skip data-members on your classes/structs that contain Image/Bitmap, or serialize them yourself using your own IFormatter<> implementation.");
			case BitmapMode.SaveAsBmp:
				return ImageFormat.Bmp;
			case BitmapMode.SaveAsJpg:
				return ImageFormat.Jpeg;
			case BitmapMode.SaveAsPng:
				return ImageFormat.Png;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
