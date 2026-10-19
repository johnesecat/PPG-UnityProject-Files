using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace HarmonyLib
{
	internal static class InlineSignatureParser
	{
		internal static InlineSignature ImportCallSite(Module moduleFrom, byte[] data)
		{
			InlineSignature inlineSignature = new InlineSignature();
			BinaryReader reader;
			using (MemoryStream input = new MemoryStream(data, writable: false))
			{
				reader = new BinaryReader(input);
				try
				{
					ReadMethodSignature(inlineSignature);
					return inlineSignature;
				}
				finally
				{
					if (reader != null)
					{
						((IDisposable)reader).Dispose();
					}
				}
			}
			Type GetTypeDefOrRef()
			{
				uint num3 = ReadCompressedUInt32();
				uint num4 = num3 >> 2;
				uint num5;
				switch (num3 & 3)
				{
				case 0u:
					num5 = 0x2000000u | num4;
					break;
				case 1u:
					num5 = 0x1000000u | num4;
					break;
				case 2u:
					num5 = 0x1B000000u | num4;
					break;
				default:
					num5 = 0u;
					break;
				}
				uint metadataToken = num5;
				return moduleFrom.ResolveType((int)metadataToken);
			}
			int ReadCompressedInt32()
			{
				byte b = reader.ReadByte();
				reader.BaseStream.Seek(-1L, SeekOrigin.Current);
				uint num = ReadCompressedUInt32();
				int num2 = (int)num >> 1;
				if ((num & 1) == 0)
				{
					return num2;
				}
				switch (b & 0xC0)
				{
				case 0:
				case 64:
					return num2 - 64;
				case 128:
					return num2 - 8192;
				default:
					return num2 - 268435456;
				}
			}
			uint ReadCompressedUInt32()
			{
				byte b2 = reader.ReadByte();
				if ((b2 & 0x80) == 0)
				{
					return b2;
				}
				if ((b2 & 0x40) == 0)
				{
					return (uint)(((b2 & -129) << 8) | reader.ReadByte());
				}
				return (uint)(((b2 & -193) << 24) | (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte());
			}
			void ReadMethodSignature(InlineSignature method)
			{
				byte b3 = reader.ReadByte();
				if ((b3 & 0x20u) != 0)
				{
					method.HasThis = true;
					b3 = (byte)(b3 & 0xFFFFFFDFu);
				}
				if ((b3 & 0x40u) != 0)
				{
					method.ExplicitThis = true;
					b3 = (byte)(b3 & 0xFFFFFFBFu);
				}
				method.CallingConvention = (CallingConvention)(b3 + 1);
				if ((b3 & 0x10u) != 0)
				{
					ReadCompressedUInt32();
				}
				uint num8 = ReadCompressedUInt32();
				method.ReturnType = ReadTypeSignature();
				for (int k = 0; k < num8; k++)
				{
					method.Parameters.Add(ReadTypeSignature());
				}
			}
			object ReadTypeSignature()
			{
				MetadataType metadataType = (MetadataType)reader.ReadByte();
				switch (metadataType)
				{
				case MetadataType.ValueType:
				case MetadataType.Class:
					return GetTypeDefOrRef();
				case MetadataType.Pointer:
					return ((Type)ReadTypeSignature()).MakePointerType();
				case MetadataType.FunctionPointer:
				{
					InlineSignature inlineSignature2 = new InlineSignature();
					ReadMethodSignature(inlineSignature2);
					return inlineSignature2;
				}
				case MetadataType.ByReference:
					return ((Type)ReadTypeSignature()).MakePointerType();
				case (MetadataType)29:
					return ((Type)ReadTypeSignature()).MakeArrayType();
				case MetadataType.Array:
				{
					Type type = (Type)ReadTypeSignature();
					uint rank = ReadCompressedUInt32();
					uint num6 = ReadCompressedUInt32();
					for (int i = 0; i < num6; i++)
					{
						ReadCompressedUInt32();
					}
					uint num7 = ReadCompressedUInt32();
					for (int j = 0; j < num7; j++)
					{
						ReadCompressedInt32();
					}
					return type.MakeArrayType((int)rank);
				}
				case MetadataType.OptionalModifier:
					return new InlineSignature.ModifierType
					{
						IsOptional = true,
						Modifier = GetTypeDefOrRef(),
						Type = ReadTypeSignature()
					};
				case MetadataType.RequiredModifier:
					return new InlineSignature.ModifierType
					{
						IsOptional = false,
						Modifier = GetTypeDefOrRef(),
						Type = ReadTypeSignature()
					};
				case MetadataType.Var:
				case MetadataType.GenericInstance:
				case MetadataType.MVar:
					throw new NotSupportedException($"Unsupported generic callsite element: {metadataType}");
				case MetadataType.Object:
					return typeof(object);
				case MetadataType.Void:
					return typeof(void);
				case MetadataType.TypedByReference:
					return typeof(TypedReference);
				case MetadataType.IntPtr:
					return typeof(IntPtr);
				case MetadataType.UIntPtr:
					return typeof(UIntPtr);
				case MetadataType.Boolean:
					return typeof(bool);
				case MetadataType.Char:
					return typeof(char);
				case MetadataType.SByte:
					return typeof(sbyte);
				case MetadataType.Byte:
					return typeof(byte);
				case MetadataType.Int16:
					return typeof(short);
				case MetadataType.UInt16:
					return typeof(ushort);
				case MetadataType.Int32:
					return typeof(int);
				case MetadataType.UInt32:
					return typeof(uint);
				case MetadataType.Int64:
					return typeof(long);
				case MetadataType.UInt64:
					return typeof(ulong);
				case MetadataType.Single:
					return typeof(float);
				case MetadataType.Double:
					return typeof(double);
				case MetadataType.String:
					return typeof(string);
				default:
					throw new NotSupportedException($"Unsupported callsite element: {metadataType}");
				}
			}
		}
	}
}
