namespace FSLib.FileFormats.UriProtocol
{
	using System.ComponentModel;
	using System.FishExtension;

	using FSLib.Extension;


	public abstract class MagnetProtocolXtBase : INotifyPropertyChanged
	{
		MagnetProtocolXtType _type;

		/// <summary>
		/// 获得当前类型的类型
		/// </summary>
		public MagnetProtocolXtType Type
		{
			get { return _type; }
			set
			{
				if (value == _type)
					return;
				_type = value;
				OnPropertyChanged("Type");
			}
		}

		/// <summary>
		/// 创建 <see cref="MagnetProtocolXtBase" />  的新实例(MagnetProtocolXtBase)
		/// </summary>
		protected MagnetProtocolXtBase(MagnetProtocolXtType type)
		{
			Type = type;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// 获得对应的参数值
		/// </summary>
		/// <returns></returns>
		public virtual string GetParameter()
		{
			return "urn:" + Type.ToString().ToLower() + ":";
		}

		/// <summary>
		/// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
		/// </summary>
		/// <returns>
		/// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
		/// </returns>
		public override string ToString()
		{
			return GetParameter();
		}
	}
}