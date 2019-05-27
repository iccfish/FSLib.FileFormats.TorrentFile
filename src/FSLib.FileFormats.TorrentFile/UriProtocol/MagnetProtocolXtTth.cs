namespace FSLib.FileFormats.UriProtocol
{
	class MagnetProtocolXtTth : MagnetProtocolXtHashedBase
	{
		/// <summary>
		/// ���� <see cref="MagnetProtocolXtHashedBase" />  ����ʵ��(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolXtTth(byte[] hash) : base(MagnetProtocolXtType.Tth, hash)
		{
		}

		/// <summary>
		/// ���� <see cref="MagnetProtocolXtHashedBase" />  ����ʵ��(MagnetProtocolXtHashedBase)
		/// </summary>
		public MagnetProtocolXtTth(string hashString) : base(MagnetProtocolXtType.Tth, hashString)
		{
		}

		/// <summary>
		/// ��ö�Ӧ�Ĳ���ֵ
		/// </summary>
		/// <returns></returns>
		public override string GetParameter()
		{
			return "urn:tree:tiger:" + HashString;
		}
	}
}