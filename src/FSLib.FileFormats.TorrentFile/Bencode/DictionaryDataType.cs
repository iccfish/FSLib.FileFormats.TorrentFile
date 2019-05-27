using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	public class DictionaryDataType : DataTypeBase, IEnumerable<KeyValuePair<string, DataTypeBase>>
	{
		Dictionary<string, DataTypeBase> _keyMap;
		SortedDictionary<string, DataTypeBase> _valueMap;
		object _lockObject = new object();
		/// <summary>
		/// 创建 <see cref="DictionaryDataType" /> 的新实例
		/// </summary>
		public DictionaryDataType()
		{
			_valueMap = new SortedDictionary<string, DataTypeBase>(StringComparer.OrdinalIgnoreCase);
			_keyMap = new Dictionary<string, DataTypeBase>(StringComparer.OrdinalIgnoreCase);
		}

		public DataTypeBase this[string key]
		{
			get
			{
				DataTypeBase value;

				_valueMap.TryGetValue(key, out value);
				return value;
			}
		}

		public void Add(ByteStringDataType key, DataTypeBase value)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (value == null) throw new ArgumentNullException("value");

			lock (_lockObject)
			{
				if (_valueMap.ContainsKey(key.Value))
				{
					var data = _valueMap[key.Value];
					if (value is ListDataType && data is ListDataType)
					{
						var list = value as ListDataType;
						var olist = data as ListDataType;

						list.ForEach(olist.Add);
					}
				}
				else
				{
					_valueMap.Add(key.Value, value);
					_keyMap.Add(key.Value, key);
				}
			}
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
		protected override void SynchorizeData(bool fromDataToValue) { }

		public override Encoding TextEncoding
		{
			get
			{
				return base.TextEncoding;
			}
			set
			{
				if (base.TextEncoding == value)
					return;

				base.TextEncoding = value;
				//刷新列表
				lock (_lockObject)
				{
					var allkeys = _valueMap.Keys.ToArray();
					var map1 = _valueMap;
					var map2 = _keyMap;
					_valueMap = new SortedDictionary<string, DataTypeBase>();
					_keyMap = new Dictionary<string, DataTypeBase>();
					foreach (var key in allkeys)
					{
						var v_key = map2[key];
						var v_value = map1[key];

						v_key.TextEncoding = value;
						v_value.TextEncoding = value;

						//重新加入字典
						_valueMap.Add(v_key.ToString(), v_value);
						_keyMap.Add(v_key.ToString(), v_key);
					}
				}
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,DataTypeBase>> 成员

		public IEnumerator<KeyValuePair<string, DataTypeBase>> GetEnumerator()
		{
			return _valueMap.GetEnumerator();
		}

		#endregion

		#region IEnumerable 成员

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _valueMap.GetEnumerator();
		}

		#endregion
	}
}
