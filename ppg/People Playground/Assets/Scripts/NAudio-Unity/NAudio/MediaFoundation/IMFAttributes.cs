using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NAudio.MediaFoundation
{
	[ComImport]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	[Guid("2CD2D921-C447-44A7-A13C-4ADABFC247E3")]
	public interface IMFAttributes
	{
		void GetItem([In][MarshalAs(43)] Guid guidKey, [In][Out] global::System.IntPtr pValue);

		void GetItemType([In][MarshalAs(43)] Guid guidKey, out int pType);

		void CompareItem([In][MarshalAs(43)] Guid guidKey, global::System.IntPtr value, [MarshalAs(2)] out bool pbResult);

		void Compare([MarshalAs(28)] IMFAttributes pTheirs, int matchType, [MarshalAs(2)] out bool pbResult);

		void GetUINT32([In][MarshalAs(43)] Guid guidKey, out int punValue);

		void GetUINT64([In][MarshalAs(43)] Guid guidKey, out long punValue);

		void GetDouble([In][MarshalAs(43)] Guid guidKey, out double pfValue);

		void GetGUID([In][MarshalAs(43)] Guid guidKey, out Guid pguidValue);

		void GetStringLength([In][MarshalAs(43)] Guid guidKey, out int pcchLength);

		void GetString([In][MarshalAs(43)] Guid guidKey, [Out][MarshalAs(21)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		void GetAllocatedString([In][MarshalAs(43)] Guid guidKey, [MarshalAs(21)] out string ppwszValue, out int pcchLength);

		void GetBlobSize([In][MarshalAs(43)] Guid guidKey, out int pcbBlobSize);

		void GetBlob([In][MarshalAs(43)] Guid guidKey, [Out][MarshalAs(42)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		void GetAllocatedBlob([In][MarshalAs(43)] Guid guidKey, out global::System.IntPtr ip, out int pcbSize);

		void GetUnknown([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(43)] Guid riid, [MarshalAs(25)] out object ppv);

		void SetItem([In][MarshalAs(43)] Guid guidKey, global::System.IntPtr Value);

		void DeleteItem([In][MarshalAs(43)] Guid guidKey);

		void DeleteAllItems();

		void SetUINT32([In][MarshalAs(43)] Guid guidKey, int unValue);

		void SetUINT64([In][MarshalAs(43)] Guid guidKey, long unValue);

		void SetDouble([In][MarshalAs(43)] Guid guidKey, double fValue);

		void SetGUID([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(43)] Guid guidValue);

		void SetString([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(21)] string wszValue);

		void SetBlob([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(42, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		void SetUnknown([MarshalAs(43)] Guid guidKey, [In][MarshalAs(25)] object pUnknown);

		void LockStore();

		void UnlockStore();

		void GetCount(out int pcItems);

		void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] global::System.IntPtr pValue);

		void CopyAllItems([In][MarshalAs(28)] IMFAttributes pDest);
	}
}
