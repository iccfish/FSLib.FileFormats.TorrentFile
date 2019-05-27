namespace FSLib.FileFormats.UriProtocol
{
	/// <summary>
	/// Magnet协议里的XT参数类型，详情参见 http://zh.wikipedia.org/wiki/Magnet#xt_.E5.8F.82.E6.95.B0
	/// </summary>
	public enum MagnetProtocolXtType
	{
		/// <summary>
		/// TTH（Tiger Tree 散列函数）
		/// </summary>
		Tth,
		/// <summary>
		/// SHA-1（安全散列算法 1）
		/// </summary>
		Sha1,
		/// <summary>
		/// BitPrint.这种散列函数包含一个 SHA-1 散列函数和一个 TTH 散列函数，用 "." 隔开。
		/// </summary>
		BitPrint,
		/// <summary>
		/// eD2k Hash（eDonkey2000）散列函数
		/// </summary>
		Ed2K,
		/// <summary>
		/// AICH (高级智能型损坏处理)不是正式的磁力链接的一部分。eDonkey2000 使用的散列函数算法，用于存储和控制下载完成、正在下载的文件的完整性。
		/// </summary>
		Aich,
		/// <summary>
		/// Kazaa 散列函数
		/// </summary>
		KzHash,
		/// <summary>
		/// BitTorrent 使用的散列函数算法。
		/// </summary>
		Btih,
		/// <summary>
		/// MD5（信息-摘要算法 5）
		/// </summary>
		Md5,
		/// <summary>
		/// CRC-32 (循环冗余校验) 不是正式的磁力链接的一部分。 没有任何已知的 P2P 网络使用。
		/// </summary>
		Crc32
	}
}