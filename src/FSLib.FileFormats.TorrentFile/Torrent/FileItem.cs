using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.FileFormats.Bencode;

namespace FSLib.FileFormats.Torrent
{
	/// <summary>
	/// 表示一个种子文件中的各个文件项
	/// </summary>
	public class FileItem
	{
		public string Md5SumString
		{
			get
			{
				if (Md5Sum == null)
					return string.Empty;
				return Encoding.ASCII.GetString(Md5Sum).ToUpper();
			}
		}

		public string Ed2khash
		{
			get
			{
				if (Ed2k == null)
					return string.Empty;
				return Encoding.ASCII.GetString(Ed2k).ToUpper();
			}
		}

		public string FilehashString
		{
			get
			{
				if (Filehash == null)
					return string.Empty;
				return Encoding.ASCII.GetString(Filehash).ToUpper();
			}
		}

		public string Name
		{
			get
			{
				return System.IO.Path.GetFileName(Path);
			}
		}

		/// <summary>
		/// 获得当前的文件项是否是一个PaddingFile
		/// </summary>
		public bool IsPaddingFile
		{
			get { return Name.IndexOf("_____padding_file_") != -1; }
		}

		public string Path { get; set; }

		public long Length { get; set; }

		public byte[] Md5Sum { get; set; }

		public byte[] Filehash { get; set; }

		public byte[] Ed2k { get; set; }


		/// <summary>
		/// 获得或设置文本编码
		/// </summary>
		public Encoding TextEncoding { get; set; }

		/// <summary>
		/// 创建 <see cref="FileItem"/> 的新实例
		/// </summary>
		public FileItem()
		{
		}

		/// <summary>
		/// 创建 <see cref="FileItem"/> 的新实例
		/// </summary>
		public FileItem(string path, long length)
		{
			Path = path;
			Length = length;
		}

		#region 隐式转换

		public static implicit operator FileItem (DictionaryDataType data)
		{
			var path = data["path"];
			var length = data["length"];

			if (path == null || length == null)
				return null;

			return new FileItem
			{
				Path = path.ToString(),
				Length = (length as IntegerDataType).Value
			};
			return null;
		}

		public static implicit operator DictionaryDataType (FileItem item)
		{
			var dic = new DictionaryDataType()
			{
				TextEncoding = item.TextEncoding
			};
			var pathList = item.Path.Split(new char[]
			{
				'/',
				'\\'
			}, StringSplitOptions.RemoveEmptyEntries);

			var plist = new ListDataType
			{
				TextEncoding = item.TextEncoding
			};

			Bencode.Utility.AddToDictionary(dic, "path", item.Path);
			Bencode.Utility.AddToDictionary(dic, "length", item.Length);

			return dic;
		}

		#endregion

	}
}
