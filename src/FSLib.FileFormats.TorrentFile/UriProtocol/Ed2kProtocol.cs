using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FSLib.FileFormats.UriProtocol
{
	public class Ed2kProtocol : ProtocolBase
	{
		public Ed2kProtocol()
			: base("ed2k")
		{
		}

		public Ed2kProtocol(string url)
			: base("ed2k")
		{
			Url = url;
		}

		/// <summary>
		/// 将当前的地址信息组合成URL地址
		/// </summary>
		/// <returns></returns>
		protected override void GenerateUrl(StringBuilder sb)
		{
			base.GenerateUrl(sb);

			var list = new List<string>(10) {"", TypeString};

			if (UrlType == LinkType.File)
			{
				list.Add(FileName);
				list.Add(Size.ToString());
				list.Add(FileHash);

				if (!RootHash.IsNullOrEmpty()) list.Add("h=" + RootHash);
				if (HashSet.Count > 0) list.Add("p=" + string.Join(":", HashSet.ToArray()));
				if (WebSources.Count > 0) WebSources.ForEach(s => list.Add("s=" + s));
				if (Sources.Count > 0)
				{
					list.Add("/");
					list.Add("sources," + string.Join(",", Sources.Select(s => s.Host + ":" + s.Port).ToArray()));
				}
			}
			else if (UrlType == LinkType.NodesList || UrlType == LinkType.ServerList)
			{
				list.Add(SourceUrl);
			}
			else if (UrlType == LinkType.Server)
			{
				var host = ServerAddress;
				list.Add(host.Host);
				list.Add(host.Port.ToString());
			}

			sb.Append(string.Join("|", list.ToArray()));
			sb.Append("/");
		}

		protected override void ParseUriBody(string bodyUri)
		{
			base.ParseUriBody(bodyUri);

			var segments = bodyUri.Trim('/').Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
			var parseIndex = 1;

			TypeString = segments[0];
			if (segments[0] == "file")
			{
				FileName = segments[parseIndex++];
				Size = segments[parseIndex++].ToInt64();
				FileHash = segments[parseIndex++];
			}
			else if (segments[0] == "serverlist")
			{
				SourceUrl = segments[parseIndex++];
			}
			else if (segments[0] == "nodeslist")
			{
				SourceUrl = segments[parseIndex++];
			}
			else if (segments[0] == "server")
			{
				ServerAddress = new DnsEndPoint(segments[parseIndex++], segments[parseIndex++].ToInt32());
			}
			for (int i = parseIndex; i < segments.Length; i++)
			{
				var segment = segments[i];

				if (segment.StartsWith("s="))
				{
					//WEB链接
					WebSources.Add(System.Web.HttpUtility.UrlDecode(segment.Remove(0, 2), Encoding.UTF8));
				}
				else if (segment.StartsWith("sources,"))
				{
					Sources.AddRange(segment.Remove(0, 8).Split(',').Select(s =>
					                                                        	{
					                                                        		var arg = s.Split(':');
					                                                        		return new DnsEndPoint(arg[0], arg[1].ToInt32());
					                                                        	}).ToArray());
				}
				else if (segment.StartsWith("h="))
				{
					RootHash = segment.Remove(0, 2);
				}
				else if (segment.StartsWith("p="))
				{
					HashSet.AddRange(segment.Remove(0, 2).Split(':'));
				}
			}

		}

		public bool IsValid
		{
			get
			{
				if (UrlType == LinkType.File)
				{
					return Properties.ContainsKey("name")
					       && Properties.ContainsKey("size")
					       && Properties.ContainsKey("hash");
				}

				return false;
			}
		}

		#region 属性

		/// <summary>
		/// 获得或设置类型字符串
		/// </summary>
		public string TypeString
		{
			get { return GetProperty<string>("type"); }
			set { SetProperty("type", value); }
		}

		/// <summary>
		/// 获得或设置文件名
		/// </summary>
		public string FileName
		{
			get { return GetProperty<string>("name"); }
			set { SetProperty("name", System.Web.HttpUtility.UrlDecode(value, Encoding.UTF8)); }
		}

		/// <summary>
		/// 获得或设置文件大小
		/// </summary>
		public long Size
		{
			get { return GetProperty<long>("size"); }
			set { SetProperty("size", value); }
		}

		/// <summary>
		/// 文件Hash
		/// </summary>
		public string FileHash
		{
			get { return GetProperty<string>("hash"); }
			set { SetProperty("hash", value); }
		}

		/// <summary>
		/// 根Hash
		/// </summary>
		public string RootHash
		{
			get { return GetProperty<string>("roothash"); }
			set { SetProperty("roothash", value); }
		}

		/// <summary>
		/// HashSet
		/// </summary>
		public List<string> HashSet
		{
			get
			{
				var list = GetProperty<List<string>>("hashset");
				if (list == null)
				{
					list = new List<string>();
					HashSet = list;
				}
				return list;
			}
			private set { SetProperty("hashset", value); }
		}

		/// <summary>
		/// IP来源
		/// </summary>
		public List<DnsEndPoint> Sources
		{
			get
			{
				var list = GetProperty<List<DnsEndPoint>>("sources");
				if (list == null)
				{
					list = new List<DnsEndPoint>();
					Sources = list;
				}
				return list;
			}
			private set { SetProperty("sources", value); }
		}

		/// <summary>
		/// 来源URL
		/// </summary>
		public string SourceUrl
		{
			get { return GetProperty<string>("sourceurl"); }
			set { SetProperty("sourceurl", value); }
		}

		/// <summary>
		/// HTTP来源URL
		/// </summary>
		public List<string> WebSources
		{
			get
			{
				var list = GetProperty<List<string>>("websources");
				if (list == null)
				{
					list = new List<string>();
					WebSources = list;
				}
				return list;
			}
			set { SetProperty("websources", value); }
		}

		/// <summary>
		/// 获得或设置服务器地址
		/// </summary>
		public DnsEndPoint ServerAddress
		{
			get { return GetProperty<DnsEndPoint>("address"); }
			set { SetProperty("address", value); }
		}

		#endregion

		/// <summary>
		/// 获得或设置文件类型
		/// </summary>
		public LinkType UrlType
		{
			get
			{
				switch (TypeString)
				{
					case "file":
						return LinkType.File;
					case "server":
						return LinkType.Server;
					case "serverlist":
						return LinkType.ServerList;
					case "nodeslist":
						return LinkType.NodesList;
				}

				return LinkType.Unknown;
			}
		}

		/// <summary>
		/// 电驴链接类型
		/// </summary>
		public enum LinkType
		{
			/// <summary>
			/// 未知类型
			/// </summary>
			Unknown,

			/// <summary>
			/// 文件
			/// </summary>
			File,

			/// <summary>
			/// 服务器
			/// </summary>
			Server,

			/// <summary>
			/// 服务器地址列表
			/// </summary>
			ServerList,

			/// <summary>
			/// 客户端节点列表
			/// </summary>
			NodesList
		}
	}

#if !NET4

	/// <summary>
	/// .NET4以下版本使用的替换类
	/// </summary>
	public class DnsEndPoint
	{
		public string Host { get; set; }

		public int Port { get; set; }

		/// <summary>
		/// 创建 <see cref="DnsEndPoint" /> 的新实例
		/// </summary>
		public DnsEndPoint(string hostAddress, int port)
		{
			Host = hostAddress;
			Port = port;
		}

		public DnsEndPoint()
		{
		}
	}

#endif

}
