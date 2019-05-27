using System;

namespace FSLib.FileFormats.UriProtocol
{
	/// <summary>
	/// URI地址解析类
	/// </summary>
	public class ProtocolParser
	{
		public static ProtocolBase Parse(string url)
		{
			if (url.IsNullOrEmpty()) return null;

			var index = url.IndexOf("://");
			if (index <= 0) return null;

			var tagName = url.Substring(0, index);
			if (System.String.Compare(tagName, "ed2k", System.StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new Ed2kProtocol(url);
			}

			return null;
		}
	}
}
