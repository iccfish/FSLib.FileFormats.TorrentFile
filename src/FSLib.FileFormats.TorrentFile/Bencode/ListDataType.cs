using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	public class ListDataType : DataTypeBase<List<DataTypeBase>>,IEnumerable<DataTypeBase>
	{
		/// <summary>
		/// 创建 <see cref="ListDataType" /> 的新实例
		/// </summary>
		public ListDataType()
		{
			Value = new List<DataTypeBase>();
		}

		public void Add(DataTypeBase data)
		{
			Value.Add(data);
		}

		#region Overrides of DataTypeBase

		/// <summary>
		/// 将数据写入指定的流
		/// </summary>
		/// <param name="stream">要写入的流对象</param>
		protected override void WriteTo(Stream stream) { throw new NotImplementedException(); }

		/// <summary>
		/// 同步对象和数据
		/// </summary>
		/// <param name="fromDataToValue">是否将数据转换为值</param>
		protected override void SynchorizeData(bool fromDataToValue) {  }

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
				base.TextEncoding = value;
				Value.ForEach(s => s.TextEncoding = value);
			}
		}

		#endregion

		#region IEnumerable<DataTypeBase> 成员

		public IEnumerator<DataTypeBase> GetEnumerator()
		{
			return Value.GetEnumerator();
		}

		#endregion

		#region IEnumerable 成员

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Value.GetEnumerator();
		}

		#endregion
	}
}
