using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSLib.FileFormats.Bencode
{
	public class BufferTooSmallException:ApplicationException
	{
		public BufferTooSmallException()
			: base("Buffer not long enough.")
		{
		}
	}
}
