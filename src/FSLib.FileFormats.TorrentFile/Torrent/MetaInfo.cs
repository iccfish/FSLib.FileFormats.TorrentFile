using System.Collections.Generic;
using System;
using System.Linq;
using FSLib.FileFormats.Bencode;

namespace FSLib.FileFormats.Torrent
{
	public class MetaInfo
	{
		private long _length;

		/// <summary>
		/// �ܳ���
		/// </summary>
		public long Length
		{
			get
			{
				if (Files != null && Files.Count > 0)
				{
					return Files.Sum(s => s.Length);
				}
				return _length;
			}
			set
			{
				_length = value;
			}
		}

		public string Name { get; set; }

		public List<FileItem> Files { get; set; }

		public int PieceLength { get; set; }

		public byte[] Pieces { get; set; }

		public byte[] Roothash { get; set; }

		public string Publisher { get; set; }

		public string PublisherUrl { get; set; }


		public string Md5SumString
		{
			get
			{
				if (Md5Sum == null)
					return string.Empty;

				//correct invalid hash
				if (Md5Sum.All(s => s == 0)) return string.Empty;

				return BitConverter.ToString(Md5Sum).Replace("-", "").ToUpper();
			}
		}

		public byte[] Md5Sum { get; set; }

		public byte[] Filehash { get; set; }

		public byte[] Ed2k { get; set; }

		public string Ed2khash
		{
			get
			{
				if (Ed2k == null)
					return string.Empty;

				//correct invalid hash
				if (Ed2k.All(s => s == 0)) return string.Empty;

				return BitConverter.ToString(Ed2k).Replace("-", "").ToUpper();
			}
		}

		public string FilehashString
		{
			get
			{
				if (Filehash == null)
					return string.Empty;

				//correct invalid hash
				if (Filehash.All(s => s == 0)) return string.Empty;

				return BitConverter.ToString(Filehash).Replace("-", "").ToUpper();
			}
		}

		/// <summary>
		/// ���� <see cref="MetaInfo"/> ����ʵ��
		/// </summary>
		public MetaInfo()
		{
			Files = new List<FileItem>();
			//Pieces = new List<byte[]>();
		}

		/// <summary>
		/// ԭʼ������Ƭ�Σ�����ȡʱ��Ч
		/// </summary>
		public DictionaryDataType OriginalDataFragment { get; internal set; }
		/// <summary>
		/// ԭʼ������Ƭ�Σ�����ȡʱ��Ч
		/// </summary>
		public byte[] OriginalDataFragmentBuffer { get; internal set; }
	}
}