using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	/// <summary>
	/// BenCode文件结构的分析引擎
	/// </summary>
	public class BencodeParser
	{
		public static List<DataTypeBase> Parse(Encoding textEncoding, System.IO.Stream stream)
		{
			if (textEncoding == null)
				throw new ArgumentNullException("textEncoding");
			if (stream == null)
				throw new ArgumentNullException("stream");
			var result = new List<DataTypeBase>();

			AddListContentToList(textEncoding, stream, result);

			return result;
		}

		static void AddListContentToList(Encoding textEncoding, System.IO.Stream stream, List<DataTypeBase> list)
		{
			if (textEncoding == null)
				throw new ArgumentNullException("textEncoding");
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (list == null)
				throw new ArgumentNullException("list");


			int ch;
			while ((ch = stream.ReadByte()) != -1 && ch != 'e')
			{
				stream.Seek(-1, System.IO.SeekOrigin.Current);

				if (ch >= '0' && ch <= '9')
					list.Add(ParseAsString(textEncoding, stream));
				else if (ch == 'i')
					list.Add(ParseAsNumberic(stream));
				else if (ch == 'l')
					list.Add(ParseAsList(textEncoding, stream));
				else if (ch == 'd')
					list.Add(ParseAsDicionary(textEncoding, stream));
				else
					break;	//未知的字符，中止。
			}
		}

		static IntegerDataType ParseAsNumberic(System.IO.Stream stream)
		{
			var start = stream.Position;

			if (stream.ReadByte() != 'i')
				throw new Exception("Expect for 'i'.");

			var buffer = new List<char>(10);
			int ch;
			while ((ch = stream.ReadByte()) != -1 && ch != 'e')
			{
				buffer.Add((char)ch);
			}
			if (ch != 'e')
				throw new UnexpectEndException();

			return new IntegerDataType(long.Parse(new string(buffer.ToArray())))
			{
				DataStartPosition = start,
				DataEndPosition = stream.Position - 1
			};
		}

		static ByteStringDataType ParseAsString(Encoding textEncoding, System.IO.Stream stream)
		{
			var start = stream.Position;

			var buffer = new List<char>(10);
			int ch;
			while ((ch = stream.ReadByte()) != -1 && ch != ':')
			{
				buffer.Add((char)ch);
			}
			if (ch != ':')
				throw new UnexpectEndException();

			var length = int.Parse(new string(buffer.ToArray()));
			if (length > 0x400000)
				throw new ArgumentOutOfRangeException();

			var buf = new byte[length];
			if (stream.Read(buf, 0, buf.Length) != buf.Length)
				throw new UnexpectEndException();

			return new ByteStringDataType()
			{
				TextEncoding = textEncoding,
				Data = buf,
				DataStartPosition = start,
				DataEndPosition = stream.Position - 1
			};
		}

		static ListDataType ParseAsList(Encoding textEncoding, System.IO.Stream stream)
		{
			var start = stream.Position;

			var list = new ListDataType();

			if (stream.ReadByte() != 'l')
				throw new InvalidOperationException();
			AddListContentToList(textEncoding, stream, list.Value);

			list.DataStartPosition = start;
			list.DataEndPosition = stream.Position - 1;

			return list;
		}

		static DictionaryDataType ParseAsDicionary(Encoding textEncoding, System.IO.Stream stream)
		{
			//键值一定是字符串
			var result = new DictionaryDataType()
			{
				TextEncoding = textEncoding,
				DataStartPosition = stream.Position
			};

			if (stream.ReadByte() != 'd')
				throw new InvalidOperationException();

			int ch;
			while ((ch = stream.ReadByte()) != -1 && ch != 'e')
			{
				stream.Seek(-1, System.IO.SeekOrigin.Current);
				//读取key
				var key = ParseAsString(textEncoding, stream);
				DataTypeBase value;
				//读取值
				ch = stream.ReadByte();
				if (ch == -1)
					throw new UnexpectEndException();

				stream.Seek(-1, System.IO.SeekOrigin.Current);
				if (ch >= '0' && ch <= '9')
					value = ParseAsString(textEncoding, stream);
				else if (ch == 'i')
					value = ParseAsNumberic(stream);
				else if (ch == 'l')
					value = ParseAsList(textEncoding, stream);
				else if (ch == 'd')
					value = ParseAsDicionary(textEncoding, stream);
				else
					break;	//未知的字符，中止。

				result.Add(key, value);
			}
			if (ch != 'e')
				throw new UnexpectEndException();

			result.DataEndPosition = stream.Position - 1;

			return result;
		}
	}
}
