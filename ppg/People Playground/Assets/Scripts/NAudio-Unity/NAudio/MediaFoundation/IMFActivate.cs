using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NAudio.MediaFoundation
{
	[ComImport]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	[Guid("7FEE9E9A-4A89-47a6-899C-B6A53A70FB67")]
	public interface IMFActivate : IMFAttributes
	{
		new void GetItem([In][MarshalAs(43)] Guid guidKey, [In][Out] global::System.IntPtr pValue);

		new void GetItemType([In][MarshalAs(43)] Guid guidKey, out int pType);

		new void CompareItem([In][MarshalAs(43)] Guid guidKey, global::System.IntPtr value, [MarshalAs(2)] out bool pbResult);

		new void Compare([MarshalAs(28)] IMFAttributes pTheirs, int matchType, [MarshalAs(2)] out bool pbResult);

		new void GetUINT32([In][MarshalAs(43)] Guid guidKey, out int punValue);

		new void GetUINT64([In][MarshalAs(43)] Guid guidKey, out long punValue);

		new void GetDouble([In][MarshalAs(43)] Guid guidKey, out double pfValue);

		new void GetGUID([In][MarshalAs(43)] Guid guidKey, out Guid pguidValue);

		new void GetStringLength([In][MarshalAs(43)] Guid guidKey, out int pcchLength);

		new void GetString([In][MarshalAs(43)] Guid guidKey, [Out][MarshalAs(21)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		new void GetAllocatedString([In][MarshalAs(43)] Guid guidKey, [MarshalAs(21)] out string ppwszValue, out int pcchLength);

		new void GetBlobSize([In][MarshalAs(43)] Guid guidKey, out int pcbBlobSize);

		new void GetBlob([In][MarshalAs(43)] Guid guidKey, [Out][MarshalAs(42)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		new void GetAllocatedBlob([In][MarshalAs(43)] Guid guidKey, out global::System.IntPtr ip, out int pcbSize);

		new void GetUnknown([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(43)] Guid riid, [MarshalAs(25)] out object ppv);

		new void SetItem([In][MarshalAs(43)] Guid guidKey, global::System.IntPtr value);

		new void DeleteItem([In][MarshalAs(43)] Guid guidKey);

		new void DeleteAllItems();

		new void SetUINT32([In][MarshalAs(43)] Guid guidKey, int unValue);

		new void SetUINT64([In][MarshalAs(43)] Guid guidKey, long unValue);

		new void SetDouble([In][MarshalAs(43)] Guid guidKey, double fValue);

		new void SetGUID([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(43)] Guid guidValue);

		new void SetString([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(21)] string wszValue);

		new void SetBlob([In][MarshalAs(43)] Guid guidKey, [In][MarshalAs(42, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		new void SetUnknown([MarshalAs(43)] Guid guidKey, [In][MarshalAs(25)] object pUnknown);

		new void LockStore();

		new void UnlockStore();

		new void GetCount(out int pcItems);

		new void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] global::System.IntPtr pValue);

		new void CopyAllItems([In][MarshalAs(28)] IMFAttributes pDest);

		void ActivateObject([In][MarshalAs(43)] Guid riid, [MarshalAs(28)] out object ppv);

		void ShutdownObject();

		void DetachObject();
	}
}
