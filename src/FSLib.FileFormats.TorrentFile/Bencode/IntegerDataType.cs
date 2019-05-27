using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	/// <summary>
	/// 
	/// </summary>
	public class IntegerDataType : DataTypeBase<long>
	{
		/// <summary>
		/// 创建 <see cref="IntegerDataType"/> 的新实例
		/// </summary>
		public IntegerDataType(long value)
			: base(value)
		{

		}
		/// <summary>
		/// 创建 <see cref="IntegerDataType"/> 的新实例
		/// </summary>
		public IntegerDataType()
		{
		}
		#region Overrides of DataTypeBase<long>

		/// <summary>
		/// 将数据写入指定的流
		/// </summary>
		/// <param name="stream">要写入的流对象</param>
		protected override void WriteTo(Stream stream)
		{
			stream.WriteByte((byte)'i');
			base.WriteTo(stream);
			stream.WriteByte((byte)'e');
		}

		/// <summary>
		/// 同步对象和数据
		/// </summary>
		/// <param name="fromDataToValue">是否将数据转换为值</param>
		protected override void SynchorizeData(bool fromDataToValue)
		{
			if (fromDataToValue) _value = long.Parse(new string(Array.ConvertAll(_data, s => (char)s)));
			else _data = Array.ConvertAll(_value.ToString().ToCharArray(), s => (byte)s);
		}

		public override int DataSize
		{
			get
			{
				return base.DataSize + 2;
			}
		}
		#endregion

		public override Type MathedClrType
		{
			get
			{
				return Value > int.MaxValue ? typeof(long) : typeof(int);
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
