using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FSLib.FileFormats.Bencode;

namespace FSLib.FileFormats.Torrent
{
	public class TorrentFile
	{
		public string Path { get; set; }

		public MetaInfo MetaInfo { get; private set; }

		public string Announce { get; set; }

		public List<string> AnnounceList { get; set; }

		public DateTime? CreationDate { get; set; }

		public string Comment { get; set; }

		public string CreatedBy { get; set; }

		public Encoding Encoding { get; set; }

		public List<NodeEntry> Nodes { get; set; }

		public List<string> HttpSeeds { get; set; }

		public bool IsPrivate { get; set; }

		/// <summary>
		/// 获得种子Hash
		/// </summary>
		public byte[] MetaInfoHash { get; private set; }

		/// <summary>
		/// 获得种子Hash
		/// </summary>
		public string MetaInfoHashString { get; private set; }

		/// <summary>
		/// 获得所有包含的文件
		/// </summary>
		public List<FileItem> Files
		{
			get
			{
				return MetaInfo.Files;
			}
		}

		/// <summary>
		/// 获得或设置文件名
		/// </summary>
		public string Name
		{
			get
			{
				return MetaInfo.Name;
			}
			set
			{
				MetaInfo.Name = value;
			}
		}


		/// <summary>
		/// 创建 <see cref="TorrentFile"/> 的新实例
		/// </summary>
		public TorrentFile()
		{
			AnnounceList = new List<string>();
			Encoding = Encoding.UTF8;
			MetaInfo = new MetaInfo();
		}

		/// <summary>
		/// 由指定的文件创建一个 <see cref="TorrentFile"/> 对象
		/// </summary>
		/// <param name="path"></param>
		public TorrentFile(string path, LoadFlag flag = LoadFlag.None)
			: this()
		{
			Load(path, flag);
		}

		/// <summary>
		/// 加载指定的文件
		/// </summary>
		/// <param name="path">文件路径</param>
		public void Load(string path, LoadFlag flag = LoadFlag.None)
		{
			Path = path;
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
			{
				Load(fs, flag);
			}
		}

		public void Load(Stream stream, LoadFlag flag = LoadFlag.None)
		{
			var tor = BencodeParser.Parse(Encoding.UTF8, stream);
			TorrentBencodeAdapter.FillInfoFromFile(tor[0] as DictionaryDataType, this);

			//加载Infobar区域的？
			if ((LoadFlag.LoadInfoSectionData & flag) == LoadFlag.LoadInfoSectionData)
			{
				if (!stream.CanSeek)
				{
					throw new InvalidOperationException("stream can not be seeked.");
				}

				var data = MetaInfo.OriginalDataFragment;
				MetaInfo.OriginalDataFragmentBuffer = new byte[(int)(data.DataEndPosition - data.DataStartPosition) + 1];

				stream.Seek(data.DataStartPosition, SeekOrigin.Begin);
				stream.Read(MetaInfo.OriginalDataFragmentBuffer, 0, MetaInfo.OriginalDataFragmentBuffer.Length);
			}

			if (flag.HasFlag(LoadFlag.ComputeMetaInfoHash))
			{
				var data = MetaInfo.OriginalDataFragment;
				var buffer = MetaInfo.OriginalDataFragmentBuffer;
				if (buffer == null)
				{
					if (!stream.CanSeek)
					{
						throw new InvalidOperationException("stream can not be seeked.");
					}

					buffer = new byte[(int)(data.DataEndPosition - data.DataStartPosition) + 1];
					stream.Seek(data.DataStartPosition, SeekOrigin.Begin);
					stream.Read(buffer, 0, buffer.Length);
				}

				var sha = SHA1.Create();
				MetaInfoHash = sha.ComputeHash(buffer);
				MetaInfoHashString = BitConverter.ToString(MetaInfoHash).Replace("-", "").ToUpper();
			}
		}
	}
}
