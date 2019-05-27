using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	public static class Utility
	{
		/// <summary>
		/// 创建一个 <see cref="ByteStringDataType"/> 类型的节点
		/// </summary>
		/// <param name="encoding"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ByteStringDataType Create(Encoding encoding, string value)
		{
			var node = new ByteStringDataType
			{
				TextEncoding = encoding
			};
			node.Value = value;

			return node;
		}

		/// <summary>
		/// 添加键和值到指定的字典
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddToDictionary(DictionaryDataType dict, string key, string value)
		{
			AddToDictionary(dict, dict.TextEncoding ?? Encoding.Default, key, value);
		}

		/// <summary>
		/// 添加键和值到指定的字典
		/// </summary>
		public static void AddToDictionary(DictionaryDataType dict, string key, int value)
		{
			dict.Add(Create(dict.TextEncoding, key), new IntegerDataType(value));
		}
		/// <summary>
		/// 添加键和值到指定的字典
		/// </summary>
		public static void AddToDictionary(DictionaryDataType dict, string key, long value)
		{
			dict.Add(Create(dict.TextEncoding, key), new IntegerDataType(value));
		}

		/// <summary>
		/// 添加键和值到指定的字典
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="encoding"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddToDictionary(DictionaryDataType dict, Encoding encoding, string key, string value)
		{
			dict.Add(Create(encoding, key), Create(encoding, value));
		}
	}
}
