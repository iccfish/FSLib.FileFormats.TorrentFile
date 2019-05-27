using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.UriProtocol
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ProtocolBase
	{
		string _url;

		public char UrlParameterSeperator { get; set; }

		/// <summary>
		/// 获得当前协议的名称
		/// </summary>
		public string ProtocolName { get; protected set; }

		/// <summary>
		/// 获得所有的链接参数
		/// </summary>
		public Dictionary<string, object> Properties { get; private set; }

		/// <summary>
		/// 设置属性
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="data">数据</param>
		public void SetProperty(string key, object data)
		{
			Properties[key] = data;
		}

		/// <summary>
		/// 获得指定的属性
		/// </summary>
		/// <typeparam name="T">属性值类型</typeparam>
		/// <param name="key">键名</param>
		/// <returns>指定的键值</returns>
		public T GetProperty<T>(string key)
		{
			return (T)Properties.GetValue(key);
		}

		/// <summary>
		/// 获得或设置完整的地址链接
		/// </summary>
		public string Url
		{
			get
			{
				if (_url.IsNullOrEmpty())
					_url = GenerateUrl();
				return _url;
			}
			set
			{
				if (value == _url)
					return;

				_url = null;
				Parse(value);
			}
		}

		protected virtual void Parse(string uri)
		{
			if (uri.IsNullOrEmpty()) return;

			var index = uri.IndexOf("://");
			if (index <= 0) return;

			var tagName = uri.Substring(0, index);
			if (!string.IsNullOrEmpty(tagName) && string.Compare(tagName, ProtocolName, true) != 0)
				throw new InvalidProgramException("URI Protocol Name Conflicted.");


			ProtocolName = tagName;
			var body = uri.Substring(index + 3);
			ParseUriBody(body);
		}

		/// <summary>
		/// 分析URL主体
		/// </summary>
		/// <param name="bodyUri">主体URI</param>
		protected virtual void ParseUriBody(string bodyUri)
		{
			var fragments = bodyUri.Split(UrlParameterSeperator);
			fragments.ForEach(ParseUriDataFragment);
		}

		/// <summary>
		/// 分解数据片段
		/// </summary>
		/// <param name="data"></param>
		protected virtual void ParseUriDataFragment(string data)
		{
			if (data.IsNullOrEmpty())
				return;

			var index = data.IndexOf('=');
			if (index == -1)
				return;

			var key = data.Substring(0, index);
			var value = data.Substring(index + 1);

			var obj = ParseUriDataArea(key, value);
			if (obj != null)
			{
				if (Properties.ContainsKey(key))
				{
					var container = Properties[key];
					if (container is ArrayList)
					{
						(container as ArrayList).Add(obj);
					}
					else
					{
						container = new ArrayList()
									{
										container,
										obj
									};
						Properties[key] = container;
					}
				}
				else
				{
					Properties.Add(key, obj);
				}
			}
		}

		/// <summary>
		/// 将URI中参数的数值转换为实际的对象
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected virtual object ParseUriDataArea(string key, string data)
		{
			return data;
		}

		/// <summary>
		/// 创建 <see cref="ProtocolBase" /> 的新实例
		/// </summary>
		public ProtocolBase(string procotolName)
			: this()
		{
			ProtocolName = procotolName;
		}

		/// <summary>
		/// 创建 <see cref="ProtocolBase" /> 的新实例
		/// </summary>
		public ProtocolBase()
		{
			Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			UrlParameterSeperator = '&';
		}

		/// <summary>
		/// 将当前的地址信息组合成URL地址
		/// </summary>
		/// <returns></returns>
		public string GenerateUrl()
		{
			var sb = new StringBuilder(0x400);
			GenerateUrl(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 将当前的地址信息组合成URL地址
		/// </summary>
		/// <returns></returns>
		protected virtual void GenerateUrl(System.Text.StringBuilder sb)
		{
			sb.Append(ProtocolName.ToLower());
			sb.Append("://");
		}

		/// <summary>
		/// 生成参数的主体
		/// </summary>
		/// <param name="sb"></param>
		protected virtual void GenerateUrlBody(StringBuilder sb)
		{
			var allProps = Properties.ToArray();
			for (int i = 0; i < allProps.Length; i++)
			{
				var property = allProps[i];

				GenerateUrlParameter(property.Key, property.Value, sb);
				if (i < allProps.Length - 1)
					sb.Append(UrlParameterSeperator);
			}
		}

		/// <summary>
		/// 针对每个不同的参数生成地址中的内容
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <param name="sb"></param>
		protected virtual void GenerateUrlParameter(string key, object data, StringBuilder sb)
		{
			if (data == null)
			{
				GenerateUrlParameterUnit(key, null, sb);
				return;
			}

			if (data is IDictionary)
			{
				var col = data as IDictionary;
				var count = col.Count;
				var index = 0;
				foreach (var c in col.Values)
				{
					GenerateUrlParameterUnit(key, c, sb);
					if (index++ < count - 1)
						sb.Append(UrlParameterSeperator);
				}

				return;
			}


			if (data is ICollection)
			{
				var col = data as ICollection;
				var count = col.Count;
				var index = 0;
				foreach (var c in col)
				{
					GenerateUrlParameterUnit(key, c, sb);
					if (index++ < count - 1)
						sb.Append(UrlParameterSeperator);
				}

				return;
			}
			GenerateUrlParameterUnit(key, data, sb);
		}

		/// <summary>
		/// 针对每个不同的参数中的单个值生成地址中的内容
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <param name="sb"></param>
		protected virtual void GenerateUrlParameterUnit(string key, object data, StringBuilder sb)
		{
			if (data == null)
				sb.Append(key + "=");
			else
			{
				sb.Append(key + "=" + data.ToString());
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return Url;
		}
	}
}
