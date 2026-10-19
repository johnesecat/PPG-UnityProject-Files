using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	public static class Utility
	{
		private static readonly byte[] readBuffer = new byte[8192];

		internal static T ToType<T>(this IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return default(T);
			}
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}

		internal static object ToType(this IntPtr ptr, Type t)
		{
			if (ptr == IntPtr.Zero)
			{
				return null;
			}
			return Marshal.PtrToStructure(ptr, t);
		}

		internal static uint Swap(uint x)
		{
			return ((x & 0xFF) << 24) + ((x & 0xFF00) << 8) + ((x & 0xFF0000) >> 8) + ((x & 0xFF000000u) >> 24);
		}

		public static uint IpToInt32(this IPAddress ipAddress)
		{
			return Swap((uint)ipAddress.Address);
		}

		public static IPAddress Int32ToIp(uint ipAddress)
		{
			return new IPAddress(Swap(ipAddress));
		}

		public static string FormatPrice(string currency, double price)
		{
			string text = price.ToString("0.00");
			switch (currency)
			{
			case "AED":
				return text + "د.إ";
			case "ARS":
				return "$" + text + " ARS";
			case "AUD":
				return "A$" + text;
			case "BRL":
				return "R$" + text;
			case "CAD":
				return "C$" + text;
			case "CHF":
				return "Fr. " + text;
			case "CLP":
				return "$" + text + " CLP";
			case "CNY":
				return text + "元";
			case "COP":
				return "COL$ " + text;
			case "CRC":
				return "₡" + text;
			case "EUR":
				return "€" + text;
			case "SEK":
				return text + "kr";
			case "GBP":
				return "£" + text;
			case "HKD":
				return "HK$" + text;
			case "ILS":
				return "₪" + text;
			case "IDR":
				return "Rp" + text;
			case "INR":
				return "₹" + text;
			case "JPY":
				return "¥" + text;
			case "KRW":
				return "₩" + text;
			case "KWD":
				return "KD " + text;
			case "KZT":
				return text + "₸";
			case "MXN":
				return "Mex$" + text;
			case "MYR":
				return "RM " + text;
			case "NOK":
				return text + " kr";
			case "NZD":
				return "$" + text + " NZD";
			case "PEN":
				return "S/. " + text;
			case "PHP":
				return "₱" + text;
			case "PLN":
				return text + "zł";
			case "QAR":
				return "QR " + text;
			case "RUB":
				return text + "₽";
			case "SAR":
				return "SR " + text;
			case "SGD":
				return "S$" + text;
			case "THB":
				return "฿" + text;
			case "TRY":
				return "₺" + text;
			case "TWD":
				return "NT$ " + text;
			case "UAH":
				return "₴" + text;
			case "USD":
				return "$" + text;
			case "UYU":
				return "$U " + text;
			case "VND":
				return "₫" + text;
			case "ZAR":
				return "R " + text;
			default:
				return text + " " + currency;
			}
		}

		public static string ReadNullTerminatedUTF8String(this BinaryReader br)
		{
			lock (readBuffer)
			{
				int num = 0;
				byte b;
				while ((b = br.ReadByte()) != 0 && num < readBuffer.Length)
				{
					readBuffer[num] = b;
					num++;
				}
				return Encoding.UTF8.GetString(readBuffer, 0, num);
			}
		}
	}
}
