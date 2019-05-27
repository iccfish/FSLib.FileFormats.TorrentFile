using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Torrent
{
	[Flags]
	public enum LoadFlag
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,
		/// <summary>
		/// 同时加载Info区域的字节
		/// </summary>
		LoadInfoSectionData = 1,
		/// <summary>
		/// 计算MetaInfo的hash
		/// </summary>
		ComputeMetaInfoHash = 2
	}
}
