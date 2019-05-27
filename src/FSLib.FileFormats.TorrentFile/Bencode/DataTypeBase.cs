using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	public abstract class DataTypeBase
	{

		protected byte[] _data;

		/// <summary>
		/// 获得文件数据的起始位置（仅读取时有效）
		/// </summary>
		public long DataStartPosition { get; internal set; }

		/// <summary>
		/// 获得文件数据的结束位置（仅读取时有效）
		/// </summary>
		public long DataEndPosition { get; internal set; }

		/// <summary>
		/// 获得或设置字节数据
		/// </summary>
		public virtual byte[] Data
		{
			get { return _data; }
			set
			{
				_data = value;
				SynchorizeData(true);
			}
		}

		/// <summary>
		/// 创建 <see cref="DataTypeBase" /> 的新实例
		/// </summary>
		public DataTypeBase()
		{
		}

		/// <summary>
		/// 将数据写入指定的流
		/// </summary>
		/// <param name="stream">要写入的流对象</param>
		protected virtual void WriteTo(System.IO.Stream stream)
		{
			stream.Write(Data, 0, Data.Length);
		}
		/// <summary>
		/// 获得最终写入时的数据长度
		/// </summary>
		public virtual int DataSize { get { return Data.Length; } }

		/// <summary>
		/// 同步对象和数据
		/// </summary>
		/// <param name="fromDataToValue">是否将数据转换为值</param>
		protected abstract void SynchorizeData(bool fromDataToValue);

		/// <summary>
		/// 获得可能与系统相匹配的数据类型
		/// </summary>
		public virtual Type MathedClrType
		{
			get
			{
				return null;
			}
		}

		private Encoding _textEncoding;

		/// <summary>
		/// 获得或设置文本的编码
		/// </summary>
		public virtual System.Text.Encoding TextEncoding { get { return _textEncoding ?? Encoding.UTF8; } set { _textEncoding = value; } }
	}

	/// <summary>
	/// 数据的基类型对象
	/// </summary>
	/// <typeparam name="T">对象类型</typeparam>
	public abstract class DataTypeBase<T> : DataTypeBase
	{
		protected T _value;

		/// <summary>
		/// 获得或设置关联的数据
		/// </summary>
		public virtual T Value
		{
			get { return _value; }
			set
			{
				_value = value;
				SynchorizeData(false);
			}
		}

		/// <summary>
		/// 创建 <see cref="DataTypeBase" /> 的新实例
		/// </summary>
		public DataTypeBase(T value) { Value = value; }

		/// <summary>
		/// 创建 <see cref="DataTypeBase" /> 的新实例
		/// </summary>
		public DataTypeBase()
		{
		}
	}
}
