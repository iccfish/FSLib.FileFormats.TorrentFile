using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Torrent
{
	using FSLib.Extension;
	using System.IO;

	using FSLib.Extension.FishLib;
	using FSLib.FileFormats.Bencode;

	/// <summary>
	/// Torrent文件到Bencode的适配器
	/// </summary>
	class TorrentBencodeAdapter
	{
		public static void FillInfoFromFile(Bencode.DictionaryDataType rootNode, TorrentFile file)
		{
			var meta = file.MetaInfo;

			//识别编码
			var encoding = GetEncoding(rootNode);
			if (encoding != null)
			{
				file.Encoding = encoding;
				rootNode.TextEncoding = encoding;
			}
			//基本信息
			file.Announce = GetValue(rootNode, "announce");
			var alist = rootNode["announce-list"] as ListDataType;
			file.AnnounceList = alist == null ? new List<string>() : alist.SelectMany(s => (s is ListDataType) ? (s as ListDataType).Select(x => x.ToString()).ToArray() : new[] { s.ToString() }).ToList();
			file.CreatedBy = GetValue(rootNode, "created by");
			file.Comment = GetValue(rootNode, "comment");
			file.CreationDate = null;
			var dateNode = rootNode["creation date"];
			if (dateNode != null && dateNode is Bencode.IntegerDataType)
			{
				file.CreationDate = new DateTime(1970, 1, 1).AddSeconds((dateNode as Bencode.IntegerDataType).Value);
			}
			//
			file.Nodes = GetNodeList(rootNode, "nodes");
			//roothash
			file.IsPrivate = GetInt32(rootNode, "private") > 0;
			meta.Roothash = GetByteValue(rootNode, "root hash");
			meta.Filehash = GetByteValue(rootNode, "filehash");
			meta.Ed2k = GetByteValue(rootNode, "ed2k");
			meta.Md5Sum = GetByteValue(rootNode, "md5sum");
			//名称
			var metaNode = meta.OriginalDataFragment = rootNode["info"] as Bencode.DictionaryDataType;
			meta.Name = GetValue(metaNode, "name");
			meta.PieceLength = GetInt32(metaNode, "piece length");
			meta.Length = GetInt64(metaNode, "length");

			//files?
			var filesNode = metaNode["files"] as Bencode.ListDataType;
			if (filesNode != null)
				meta.Files = GetFileList(filesNode);

			//hash
			meta.Pieces = GetByteValue(metaNode, "pieces");//.SplitPage(20);
			meta.Publisher = GetValue(metaNode, "publisher");
			meta.PublisherUrl = GetValue(metaNode, "publisher-url");
		}

		static string GetValue(Bencode.DataTypeBase node)
		{
			if (node is Bencode.ByteStringDataType)
				return node.ToString();
			if (node is Bencode.ListDataType)
			{
				var list = node as Bencode.ListDataType;
				return list.Select(s => s.ToString()).Join(@"\");
			}

			return string.Empty;
		}

		static string GetValue(Bencode.DictionaryDataType dic, string key)
		{
			var node = dic[key + ".utf-8"];
			if (node != null)
			{
				node.TextEncoding = Encoding.UTF8;
				return GetValue(node);
			}

			node = dic[key];
			if (node == null)
				return string.Empty;

			return GetValue(node);
		}

		static int GetInt32(Bencode.DictionaryDataType dic, string key)
		{
			return (int)GetInt64(dic, key);
		}

		static long GetInt64(Bencode.DictionaryDataType dic, string key)
		{
			var node = dic[key];
			if (node == null || !(node is Bencode.IntegerDataType))
				return 0;

			return (node as Bencode.IntegerDataType).Value;
		}

		static byte[] GetByteValue(Bencode.DictionaryDataType dic, string key)
		{
			var node = dic[key];
			if (node == null || !(node is Bencode.ByteStringDataType))
				return null;

			return node.Data;
		}

		static List<FileItem> GetFileList(Bencode.ListDataType list)
		{
			return list.Cast<Bencode.DictionaryDataType>()
			.Select(s => new FileItem(GetValue(s, "name"), GetInt64(s, "length"))
			{
				Md5Sum = GetByteValue(s, "md5sum"),
				Path = IOUtility.RemoveInvalidPathChars(GetValue(s, "path")),
				TextEncoding = list.TextEncoding,
				Filehash = GetByteValue(s, "filehash"),
				Ed2k = GetByteValue(s, "ed2k")
			}).ToList();
		}

		static List<string> GetValueList(Bencode.DictionaryDataType dic, string key)
		{
			var node = dic[key];
			if (node == null || !(node is Bencode.ListDataType))
				return new List<string>();

			return (node as Bencode.ListDataType).Select(s => s.ToString()).ToList();
		}

		static Encoding GetEncoding(Bencode.DictionaryDataType rootNode)
		{
			var node = rootNode["encoding"];
			if (node != null)
			{
				var codeName = node.ToString();
				if (string.Compare(codeName, "UTF8", StringComparison.OrdinalIgnoreCase) == 0) return System.Text.Encoding.UTF8;

				try
				{
					return Encoding.GetEncoding(node.ToString());
				}
				catch (Exception)
				{
					return Encoding.UTF8;
				}
			}

			node = rootNode["codepage"];
			if (node != null)
			{
				return Encoding.GetEncoding((int)(node as Bencode.IntegerDataType).Value);
			}

			return Encoding.UTF8;
		}

		static List<NodeEntry> GetNodeList(Bencode.DictionaryDataType dic, string key)
		{
			var node = dic[key];
			if (node == null || !(node is Bencode.ListDataType))
				return new List<NodeEntry>();

			return (node as Bencode.ListDataType).Cast<Bencode.ListDataType>().Select(s => new NodeEntry
			{
				Host = s.Value[0].ToString(),
				Port = (int)(s.Value[1] as Bencode.IntegerDataType).Value
			}).ToList();
		}
	}
}
