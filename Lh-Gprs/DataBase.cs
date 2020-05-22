using System;
using System.Data;
using System.Data.OleDb ;
using System.Collections;
using System.Windows.Forms ;

namespace GprsServer
{
	/// <summary>
	/// DataBase 的摘要说明。
	/// </summary>
	public class DataBase
	{
		private static OleDbConnection  myConn; 
		private static OleDbCommand myCommand;
		private static OleDbDataReader myDbRead;
		public static Hashtable CarInfo_Hash;
		public DataBase()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			Conndb();

		}

		private static void Conndb()
		{
			try
			{
				myConn=new OleDbConnection(GprsServer.ConnStr);
				myConn.Open();
				GprsServer.ConnStatus=true;
							
			}
			catch (Exception)
			{
				//MessageBox.Show("数据库连接不正常!","提示");
				GprsServer.ConnStatus=false;
			}
		}
        
		public static int GetEquiptype(string strIp)
		{
            string strSql;
			try
			{
				strSql="Select Type from Vehicle where ipAddress='"+strIp+"'";
				myCommand=myConn.CreateCommand();
				myCommand.CommandText =strSql; 
				myDbRead=myCommand.ExecuteReader();
				while (myDbRead.Read())
				{
					if ((myDbRead["Type"].ToString().ToUpper() =="CDMA")|(myDbRead["Type"].ToString().ToUpper() =="TCP"))
					{
						myDbRead.Close();
						myDbRead=null;
						myCommand.Dispose();
						return 1;
					}
					else
					{
						myDbRead.Close();
						myDbRead=null;
						myCommand.Dispose();
						return 0;
					}
				}
			}
			catch(Exception)
			{
				if (myConn.State.ToString()=="Closed")
				{
					Conndb();
				}
				return 0;

			}
        return 0;
		}
		public static void GetAllProtocolIP()
		{
			string strSql;
			try
			{
				tVehInfo nVehInfo=null;
				System.Data.OleDb.OleDbCommand GetProTypeCommand=null;
				System.Data.OleDb.OleDbDataReader GetProTypeReader=null;
				strSql="select distinct IpAddress,type,TaxiNo from vehicle";
				GetProTypeCommand=myConn.CreateCommand();
				GetProTypeCommand.CommandText=strSql;
				GetProTypeReader=GetProTypeCommand.ExecuteReader();
				CarInfo_Hash=new Hashtable();
				while(GetProTypeReader.Read())
				{
					nVehInfo=new tVehInfo();
					nVehInfo.ipaddress=GetProTypeReader["IpAddress"].ToString();
					if(GetProTypeReader["type"].ToString().ToUpper()=="TCP")
					{
						nVehInfo.isendtype=1;
					}
					else
					{
						nVehInfo.isendtype=0;
					}					
					if(CarInfo_Hash.ContainsKey((string)GetProTypeReader["IpAddress"]))
					{
						CarInfo_Hash[(string)GetProTypeReader["IpAddress"]]=nVehInfo;
					}
					else
					{
						CarInfo_Hash.Add((string)GetProTypeReader["IpAddress"],nVehInfo);
					}
				}
				GetProTypeReader.Close();
				GetProTypeReader=null;
				GetProTypeCommand.Dispose();
			}
			catch(Exception ce)
			{
				GprsServer.WriteErrLog("GetAllProtocolIP",ce.Message.ToString()+ce.StackTrace.ToString());
				if (myConn.State.ToString()=="Closed")
				{
					Conndb();
				}
				return ;
			}

		}

	}
}
