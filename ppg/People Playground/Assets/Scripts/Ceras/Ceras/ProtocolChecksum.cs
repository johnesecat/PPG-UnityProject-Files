using System;
using System.Text;

namespace Ceras
{
	public class ProtocolChecksum
	{
		private xxHash _hash = new xxHash();

		private bool _isClosed;

		private bool _useDebugString = true;

		private string _debugString = "";

		private int _checksum;

		public int Checksum
		{
			get
			{
				if (!_isClosed)
				{
					throw new InvalidOperationException("not yet computed");
				}
				return _checksum;
			}
		}

		internal ProtocolChecksum()
		{
			_hash.Init();
		}

		internal void Add(string name)
		{
			if (_isClosed)
			{
				throw new InvalidOperationException("IsClosed");
			}
			if (_useDebugString)
			{
				_debugString = _debugString + "\r\n" + name;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(name);
			_hash.Update(bytes, bytes.Length);
		}

		internal void Finish()
		{
			_isClosed = true;
			_checksum = (int)_hash.Digest();
		}
	}
}
