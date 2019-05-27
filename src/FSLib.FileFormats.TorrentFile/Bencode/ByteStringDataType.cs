using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	/// <summary>
	/// Byte-String
	/// </summary>
	public class ByteStringDataType : DataTypeBase<string>
	{
		/// <summary>
		/// 创建 <see cref="ByteStringDataType"/> 的新实例
		/// </summary>
		public ByteStringDataType(string value)
			: base(value)
		{
		}

		/// <summary>
		/// 创建 <see cref="ByteStringDataType"/> 的新实例
		/// </summary>
		public ByteStringDataType()
		{
		}

		#region Overrides of DataTypeBase<string>

		/// <summary>
		/// 将数据写入指定的流
		/// </summary>
		/// <param name="stream">要写入的流对象</param>
		protected override void WriteTo(Stream stream)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 获得最终写入时的数据长度
		/// </summary>
		public override int DataSize
		{
			get
			{
				var datasize = base.DataSize;
				return datasize + 1 + datasize.ToString().Length;
			}
		}

		/// <summary>
		/// 同步对象和数据
		/// </summary>
		/// <param name="fromDataToValue">是否将数据转换为值</param>
		protected override void SynchorizeData(bool fromDataToValue)
		{
			if (fromDataToValue)
			{
				if (_data == null || _data.Length == 0)
					_value = string.Empty;
				else
					_value = TextEncoding.GetString(_data);
			}
			else
			{
				if (string.IsNullOrEmpty(_value))
					_data = new byte[]
					{
					};
				else
					_data = TextEncoding.GetBytes(_value);
			}
		}

		/// <summary>
		/// 获得或设置文本的编码
		/// </summary>
		public override Encoding TextEncoding
		{
			get
			{
				return base.TextEncoding;
			}
			set
			{
				value = value ?? System.Text.Encoding.UTF8;
				if (value == base.TextEncoding)
					return;

				base.TextEncoding = value;
				_value = base.TextEncoding.GetString(Data);
			}
		}

		#endregion


		public override string ToString()
		{
			return base.Value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ByteStringDataTypeIgnoreCaseComparer : IComparer<ByteStringDataType>, IEqualityComparer<ByteStringDataType>
	{
		#region IComparer<ByteStringDataType> 成员

		public int Compare(ByteStringDataType x, ByteStringDataType y)
		{
			if (x == null ^ y == null)
				return x == null ? -1 : 1;
			if (x == null)
				return 0;

			return StringComparer.OrdinalIgnoreCase.Compare(x.Value, y.Value);
		}

		#endregion

		public bool Equals(ByteStringDataType x, ByteStringDataType y)
		{
			if (x == null ^ y == null)
				return false;

			return x == null || StringComparer.OrdinalIgnoreCase.Equals(x.Value, y.Value);
		}

		public int GetHashCode(ByteStringDataType obj)
		{
			return obj == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
		}
	}
}
