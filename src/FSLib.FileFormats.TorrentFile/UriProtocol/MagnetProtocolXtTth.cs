namespace FSLib.FileFormats.UriProtocol
{
	class MagnetProtocolXtTth : MagnetProtocolXtHashedBase
	{
		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtHashedBase" />  的新实例(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolXtTth(byte[] hash) : base(MagnetProtocolXtType.Tth, hash)
		{
		}

		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtHashedBase" />  的新实例(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolXtTth(string hashString) : base(MagnetProtocolXtType.Tth, hashString)
		{
		}

		/// <summary>
		/// 获得对应的参数值
		/// </summary>
		/// <returns></returns>
		public override string GetParameter()
		{
			return "urn:tree:tiger:" + HashString;
		}
	}
}