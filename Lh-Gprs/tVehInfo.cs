using System;

namespace GprsServer
{
	/// <summary>
	/// tVehInfo ��ժҪ˵����
    /// ������Ϣ��װ
	/// </summary>
	public class tVehInfo
	{
		private string sIpAddress;
		private int iProtocol;
		private int iSendType;
		public tVehInfo()
		{
			//
		}
		public string ipaddress
		{
			get{return sIpAddress;}
			set{sIpAddress=value;}
		}
		public int iprotocol
		{
			get{return iProtocol;}
			set{iProtocol=value;}
		}
		public int isendtype
		{
			get{return iSendType;}
			set{iSendType=value;}
		}
	}
}
