using System;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Forms;

public  delegate void EveUserMsg(string sInfo,string sStatus,int iType); //声明事件

namespace GprsServer
{
	/// <summary>
	/// CdmaProcess 的摘要说明。
	/// </summary>
	public class CdmaProcess
	{
		private Socket client;   //与客户端通信的Socket
		private byte[] TcpBuff;// = new byte[2048];
		private static string thName="";
        public static event EveUserMsg Eveusermsg;

		public CdmaProcess(Socket clientSock)
		{
			string strFile="";
			//
			// TODO: 在此处添加构造函数逻辑
			client=clientSock;
			GprsServer.CdmaClients.Add(client);
			TcpBuff=new byte[1];
			//
			
		}
        
		public static void ShowUserMsg(string sInfo,string sStatus,int iType)													  
		{
			try
			{
				if(Eveusermsg!=null)
					Eveusermsg(sInfo,sStatus,iType);
			}
			catch(Exception)
			{}
		}

		//接收CDMA数据线程
		public void ProcessService()
		{
			byte[] buff = new byte[1];
			int recv = 0;
			byte[] Tbuff;
			byte[] AllBuff;
			int iLenght=0;
			int iIndex=0;
			int iLen=0;
			int iXorValue=0;
			string CarIpAddress="";
			string IporInstruction="";
			string strFile="";
			byte[] RecvAffirmBuff=new byte[]{0x29,0x29,0x21,0x00,0x05,0,0,0,0,0x0D};  //回应终端数组
			EndPoint remote = client.RemoteEndPoint; //设置远程客户终节点
			
			while(true)
			{
				try
				{
					if (client.Connected)
					{
						try
						{		
												
							buff = new byte[8192];
							recv=0;
							recv=client.Receive(buff);//读取数据内容				
						
							if (recv==0)
							{
								
								//GprsServer.WriteErrLog("TcpThread_"+System.Threading.Thread.CurrentThread.Name,"接收TCP终端数据错误!");
								System.Threading.Thread.Sleep(1);	
								continue;
								
							}					

						}
						catch(SocketException se)
						{
							if(se.ErrorCode==10054)
							{
								client.Shutdown(SocketShutdown.Both);
								client.Close();
								GprsServer.WriteErrLog("TcpThread_"+System.Threading.Thread.CurrentThread.Name,se.Message.ToString()+";"+se.StackTrace.ToString());
								break;
							}
							else
							{
								System.Threading.Thread.Sleep(1);
								continue;
							}
						}
						catch(Exception ce)
						{							
							client.Shutdown(SocketShutdown.Both);
							client.Close();
							GprsServer.WriteErrLog("TcpThread_"+System.Threading.Thread.CurrentThread.Name,ce.Message.ToString()+";"+ce.StackTrace.ToString());
							break;
						}

						AllBuff=new byte[recv + TcpBuff.Length];
						System.Array.Copy(TcpBuff,0,AllBuff,0,TcpBuff.Length);
						System.Array.Copy(buff,0,AllBuff,TcpBuff.Length,recv);
//						for (iIndex = 1; iIndex <=TcpBuff.Length; iIndex++)
//						{
//							AllBuff[iIndex-1]=TcpBuff[iIndex-1];
//						}
//
//						for (iIndex = 1; iIndex <=recv; iIndex++)
//						{
//							AllBuff[TcpBuff.Length+iIndex-1]=buff[iIndex-1];
//						}

						for (iIndex = 1; iIndex <=AllBuff.Length; iIndex++)
						{
							iLenght=AllBuff.Length-iIndex+1;
							if (iLenght<6)//检测数据包长度
							{
								//不完整，则将指令保存
								if (iLenght>0)
								{
									TcpBuff=new byte[iLenght];
									for (iLen = 1; iLen <=iLenght; iLen++)
									{
										TcpBuff[iLen-1]=AllBuff[iLen+iIndex-2];
									}
								}
								break;
							}
							else
							{
								//检测当前指令是否是完整的指令,查找数据包头  
								if (AllBuff[iIndex-1]==0x29 & AllBuff[iIndex]==0x29)
								{
									if ((AllBuff.Length -iIndex)>=(AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+4))
									{
										//检测当前指令是否是完整的指令   
										if ((AllBuff[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+iIndex+3])==0x0D)
										{
											//在接收的数据中获取单条完整的指令数据
											Tbuff=new byte[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+5];
											for (iLen = 1; iLen <=Tbuff.Length; iLen++)
											{
												Tbuff[iLen-1]=AllBuff[iLen+iIndex-2];
											}
											iXorValue=GprsServer.Get_CheckXor(ref Tbuff,Tbuff.Length-2);
											if(iXorValue!=Tbuff[Tbuff.Length-2])
											{
												//校验不合格，继续查找合法指令数据
												continue;
											}
											else
											{
												//获取车载终端手机号
												CarIpAddress=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8];

												if (GprsServer.bTsFlag)
												{
													if (GprsServer.strTerminal.Trim()!="")
													{
														if (GprsServer.strTerminal.Trim()==CarIpAddress)
														{
															string BuffToStr="";
															for(int i=0;i<Tbuff.Length;i++)
															{
																BuffToStr+=Tbuff[i].ToString("X2")+" ";
															}
															ShowUserMsg("上行TCP",BuffToStr,0);
														}
													}
													else
													{
														string BuffToStr="";
														for(int i=0;i<Tbuff.Length;i++)
														{
															BuffToStr+=Tbuff[i].ToString("X2")+" ";
														}
														ShowUserMsg("上行TCP",BuffToStr,0);
													}
												}
																														
												//-----------------检测系统哈希表是否包含此终端数据---------------------\\
												if(GprsServer.CarID_TcpIP_Hash.ContainsKey(CarIpAddress))
												{
													GprsServer.CarID_TcpIP_Hash[CarIpAddress]=remote;//有更新
												}
												else
												{
													GprsServer.CarID_TcpIP_Hash.Add(CarIpAddress,remote);//没有添加
												}

												//--------------------将数据转发到中心处理程序-----------------\\
												if (GprsServer.TcpSocket.Connected)
												{
													try
													{
														GprsServer.TcpSocket.Send(Tbuff,0,Tbuff.Length,SocketFlags.None);
														if (GprsServer.bTsFlag)
														{
															if (GprsServer.strTerminal.Trim()!="")
															{
																if (GprsServer.strTerminal.Trim()==CarIpAddress)
																{
																	ShowUserMsg("系统信息","TCP车载数据成功转发到网络中心处理程序!",1);
																}
															}
															else
															{
																ShowUserMsg("系统信息","TCP车载数据成功转发到网络中心处理程序!",1);
															}
														}
													}
													catch
													{
														if (GprsServer.bTsFlag)
														{
															if (GprsServer.strTerminal.Trim()!="")
															{
																if (GprsServer.strTerminal.Trim()==CarIpAddress)
																{
																	ShowUserMsg("系统信息","TCP车载数据转发到网络中心处理程序失败!",3);
																}
															}
															else
															{
																ShowUserMsg("系统信息","TCP车载数据转发到网络中心处理程序失败!",3); 
															}
														}
													}
												}
												iIndex=iIndex+Tbuff.Length-1;
												TcpBuff=new byte[1];
											}
										}
										else
										{
											continue;
										}
									}
									else
									{
										if ((AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+4)>1024)
										{
											continue;
										}
										else
										{
											//不完整，则将指令保存
											if (iLenght>0)
											{
												TcpBuff=new byte[iLenght];
												for (iLen = 1; iLen <=iLenght; iLen++)
												{
													TcpBuff[iLen-1]=AllBuff[iLen+iIndex-2];
												}
												break;
											}
											else
											{
												TcpBuff=new byte[1];
												break;
											}

										}
									}
								}
								else
								{
									continue;
								}
							}//检测数据包长度
						}
					}
					else
					{
						break;
					}
				}
				catch
				{
					//
				}
			}
		}
	}
}
