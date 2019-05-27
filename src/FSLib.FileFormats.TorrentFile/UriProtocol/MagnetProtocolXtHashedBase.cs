using System;
using System.ComponentModel;

namespace FSLib.FileFormats.UriProtocol
{
	public class MagnetProtocolXtHashedBase : MagnetProtocolXtBase, INotifyPropertyChanged
	{
		byte[] _hash;
		string _hashString;

		public byte[] Hash
		{
			get { return _hash; }
			set
			{
				if (Equals(value, _hash))
					return;
				_hash = value;
				OnPropertyChanged("Hash");
				_hashString = value.ToHexString(upperCase: false);
			}
		}

		public string HashString
		{
			get { return _hashString; }
			set
			{
				if (value == _hashString) return;
				_hashString = value;
				OnPropertyChanged("HashString");
				_hash = value.ConvertHexStringToBytes();
			}
		}

		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtHashedBase" />  的新实例(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolXtHashedBase(MagnetProtocolXtType type, byte[] hash)
			: base(type)
		{
			Hash = hash;
		}

		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtHashedBase" />  的新实例(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolXtHashedBase(MagnetProtocolXtType type, string hashString)
			: base(type)
		{
			HashString = hashString;
		}

		/// <summary>
		/// 获得对应的参数值
		/// </summary>
		/// <returns></returns>
		public override string GetParameter()
		{
			return base.GetParameter() + HashString;
		}

		/// <summary>
		/// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
		/// </summary>
		/// <returns>
		/// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
		/// </returns>
		public override string ToString()
		{
			return GetParameter();
		}
	}
}