using System;
using System.ComponentModel;

namespace FSLib.FileFormats.UriProtocol
{
	public class MagnetProtocolBitPrint : MagnetProtocolXtHashedBase, INotifyPropertyChanged
	{
		byte[] _tthHash;
		string _tthHashString;

		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtHashedBase" />  的新实例(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolBitPrint(byte[] hash, byte[] tthHash)
			: base(MagnetProtocolXtType.BitPrint, hash)
		{
			TthHash = tthHash;
		}

		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtHashedBase" />  的新实例(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolBitPrint(string hashString, string tthHashString)
			: base(MagnetProtocolXtType.BitPrint, hashString)
		{
			TthHashString = tthHashString;
		}

		public byte[] TthHash
		{
			get { return _tthHash; }
			set
			{
				if (Equals(value, _tthHash)) return;
				_tthHash = value;
				OnPropertyChanged("TthHash");
				_tthHashString = value.ToHexString(upperCase: false);
			}
		}

		public string TthHashString
		{
			get { return _tthHashString; }
			set
			{
				if (value == _tthHashString) return;
				_tthHashString = value;
				OnPropertyChanged("TthHashString");
				_tthHash = value.ConvertHexStringToBytes();
			}
		}

		/// <summary>
		/// 获得对应的参数值
		/// </summary>
		/// <returns></returns>
		public override string GetParameter()
		{
			return base.GetParameter() + HashString + "." + TthHashString;
		}
	}
}