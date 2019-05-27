using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FSLib.FileFormats.Bencode
{
	/// <summary>
	/// 异常的流结尾或数据结尾
	/// </summary>
	public class UnexpectEndException : ApplicationException
	{
		public UnexpectEndException()
			: base("Unexpect Data End.")
		{
		}

	}
}
