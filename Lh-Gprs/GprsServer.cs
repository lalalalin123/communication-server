using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Messaging;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading ;
using System.Net;
using System.Timers ;

namespace GprsServer
{
	/// <summary>
	/// Form1 的摘要说明。
	/// </summary>
	public class GprsServer : System.Windows.Forms.Form
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		
		[DllImport("User32.dll",EntryPoint="SendMessage")]
		private static extern int 
			SendMessage(
			int hWnd,         // handle to destination window
			int Msg,          // message
			int wParam,       // first message parameter
			string lParam // second message parameter
			);
		[DllImport("User32.dll",EntryPoint="FindWindow")]
		private static extern int FindWindow(string lpClassName,string lpWindowName);

		[DllImport("kernel32.dll")]
		public static extern int GetPrivateProfileString ( string section ,string key , string def , StringBuilder retVal ,int size , string filePath ) ;
		[DllImport("kernel32")]		
		public static extern long WritePrivateProfileString ( string section,string key,string val,string filePath ) ;

		public class StateObject 
		{
			public Socket workSocket = null;               // Client socket.
			public const int BufferSize = 256;             // Size of receive buffer.
			public byte[] buffer = new byte[BufferSize];   // Receive buffer.
			public StringBuilder sb = new StringBuilder(); // Received data string.
		}

		private System.ComponentModel.IContainer components;
        
		public static bool ConnStatus=false; //数据库连接字
		public static string ConnStr;        //数据库连接字符串
		public static string UserId="";      //数据库用户名
		public static string PassWord="";    //数据库用户密码
		public static string DbName="";      //数据库名
		public static string DbServer="";    //数据库服务器名
		public static string strTerminal=""; //显示终端通讯号

		public static int LocaPort;//本地UDP端口1
		public static string LocaIP;//本地IP
		public static int LocaPort2;//本地UDP端口2
		public static string LocaIP2;//本地IP
        public static int CdmaPort; //本地CDMA的TCP端口
		public static int AgainCount; //重发指令次数
		public static string RemoteIp;
		public static int RemotePort;//远程端口
		public static string sCompanyName;//公司名称
		public static int iRecOrderCounts=0;
		private Icon m_Icon1; 
		private Icon m_Icon2;
		private Icon m_Icon3;

		private NotifyIcon notifyIcon;

		MenuItem menuItem1;
		MenuItem menuItem2;

		public static Hashtable CarID_RemoteIP_Hash; //UDP终端哈希表
        public static Hashtable CarID_TcpIP_Hash; //TCP终端哈希表
		public static Hashtable Instruction_Hash;   //重发指令哈希表
		public static Hashtable Instruction_Count;  //重发指令次数哈希表
		public static Hashtable csVehSock;	//判断车机属于UDP1还是UDP2
		public static Hashtable CarTypeByIp;	//根据车辆伪IP取UDP or TCP
		private Thread thGprs ;
		private Thread thGprs2 ;
		private Thread thCdma;
		private Thread thTcpMsg ;
		private Socket socket;
		private Socket socket2;

		public  static ArrayList CdmaClients; //保存Cdma连接套字节
		//private Socket CdmaSocket;
		private static Socket SocketTemp=null;
		public static Socket TcpSocket;

		private TcpListener Listen;　　　　  //侦听CDMA
		private bool CdmaListen=false;       //侦听状态

		private System.Windows.Forms.Timer TimerIcon;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Timer TimerConn;
		private byte[] TempBuff;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.ImageList imgList;// = new byte[1024];
		private byte[] TcpBuff;
		private System.Windows.Forms.ListView lvwMsg;
		private System.Windows.Forms.ColumnHeader sId;
		private System.Windows.Forms.ColumnHeader sNote;
		private System.Windows.Forms.Button cmdExit;
		private System.Windows.Forms.Button cmdSet;
		private System.Windows.Forms.Timer TimerCount;
		private System.Timers.Timer TimerCdma;// = new byte[2048];

		public static bool bTsFlag=false;//调试数据显示
		private System.Windows.Forms.Label labDebug;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label lblStatus;
		private DataBase ConnDB;
		public GprsServer()
		{

			InitializeComponent();
			//检测配置文件目录是否存在
			GetSysPra();
			ConnDB=new DataBase();   //连接数据库
			CdmaProcess.Eveusermsg += new EveUserMsg(ShowSysMsg);//定义事件
			CdmaClients=new ArrayList();
			if(ConnStatus)
			{
				DataBase.GetAllProtocolIP();
				ShowSysMsg("系统信息","已成功连接到数据库!",6);            
			}
			else
			{
				ShowSysMsg("系统信息","连接数据库失败!",6);
			}

			TempBuff= new byte[1];
			TcpBuff=new byte[1];

			m_Icon1 = new Icon("SysFile\\Icon1.ico");
			m_Icon2 = new Icon("SysFile\\Icon2.ico"); 	
			m_Icon3 = new Icon("SysFile\\Icon3.ico"); 

			notifyIcon = new NotifyIcon();  
			notifyIcon.Icon = m_Icon1;  
			notifyIcon.Text = sCompanyName;  
			notifyIcon.Visible = true;  

			menuItem1=new MenuItem("设置");   
			menuItem2=new MenuItem("退出"); 

			menuItem1.Click+=new EventHandler(this.menuItem1_Click);  
			menuItem2.Click+=new EventHandler(this.menuItem2_Click);  

			notifyIcon.ContextMenu=new ContextMenu(new MenuItem[]{menuItem1,menuItem2}); 
			notifyIcon.DoubleClick+=new System.EventHandler(this.notifyIcon_DBClick); 

			CarID_RemoteIP_Hash=new Hashtable();
			CarID_TcpIP_Hash=new Hashtable();
			Instruction_Hash=new Hashtable();     //重发指令哈希表
			Instruction_Count =new Hashtable();   //重发指令次数哈希表
			CarTypeByIp=new Hashtable();//根据车辆伪IP取UDP or TCP
			csVehSock=new Hashtable();
			thGprs = new Thread(new ThreadStart(ReadUdp));
			//启动线程
			thGprs.IsBackground = true;//将线程作为后台线程处理，用途，当主线程关闭，子线程随着关闭
			thGprs.Start();
            
			//*********************
			thGprs2 = new Thread(new ThreadStart(ReadUdp2)) ;
			//启动线程
			thGprs2.IsBackground =true;//将线程作为后台线程处理，用途，当主线程关闭，子线程随着关闭
			thGprs2.Start();
			//********************
			try
			{
				IPHostEntry IPHost = Dns.Resolve(RemoteIp);
				string []aliases = IPHost.Aliases; 
				IPAddress []addr = IPHost.AddressList;
				EndPoint ep = new IPEndPoint(addr[0],RemotePort); 
				TcpSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
				TcpSocket.Connect(ep);

				thTcpMsg = new Thread (new ThreadStart(ReadTcpMsg)) ;
				//启动线程
				thTcpMsg.IsBackground = true;
				thTcpMsg.Start();
				lblInfo.Text ="系统运行正常，正在中转GPRS数据...";
			}
			catch
			{}
		}

        /// <summary>
        /// 读取系统参数
        /// </summary>
		private void GetSysPra()
		{
			StringBuilder temp = new StringBuilder(255);
			if (Directory.Exists("SysIni"))
			{
				if (File.Exists("SysIni\\SysIni.ini"))
				{
					int i;
					i= GetPrivateProfileString("DbConn","DbServer","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						DbServer="127.0.0.1";
						WritePrivateProfileString("DbConn","DbServer","127.0.0.1","SysIni\\SysIni.ini");
					}
					else
					{
						DbServer=temp.ToString( );
					}
					i = GetPrivateProfileString("DbConn","DbName","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						DbName="CenterDb";
						WritePrivateProfileString("DbConn","DbName","lh_gpsdb","SysIni\\SysIni.ini");
					}
					else
					{
						DbName=temp.ToString( );
					}
					i = GetPrivateProfileString("DbConn","UserId","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						UserId="sa";
						WritePrivateProfileString("DbConn","UserId","sa","SysIni\\SysIni.ini");
					}
					else
					{
						UserId=temp.ToString( );
					}
					i = GetPrivateProfileString("DbConn","PassWord","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						PassWord="";
						WritePrivateProfileString("DbConn","PassWord","","SysIni\\SysIni.ini");
					}
					else
					{
						PassWord=temp.ToString( );
					}
					i=GetPrivateProfileString("PortIni","CdmaPort","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						CdmaPort=8890;
						WritePrivateProfileString("PortIni","CdmaPort","8890","SysIni\\SysIni.ini");
					}
					else
					{
						CdmaPort=int.Parse(temp.ToString());
					}
					//
					i = GetPrivateProfileString("Company","Name","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						sCompanyName="";
						WritePrivateProfileString("Company","Name","","SysIni\\SysIni.ini");
					}
					else
					{
						sCompanyName=temp.ToString( );
					}
					//
					i = GetPrivateProfileString("PortIni","TcpAddress","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						RemoteIp="127.0.0.1";
						WritePrivateProfileString("PortIni","TcpAddress","127.0.0.1","SysIni\\SysIni.ini");
					}
					else
					{
						RemoteIp=temp.ToString( );
					}
					//
					i = GetPrivateProfileString("PortIni","TcpPort","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						RemotePort=6666;
						WritePrivateProfileString("PortIni","TcpPort","6666","SysIni\\SysIni.ini");
					}
					else
					{
						RemotePort=int.Parse(temp.ToString( ));
					}
					//
					i= GetPrivateProfileString("PortIni","UdpPort","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						LocaPort=8888;
						WritePrivateProfileString("PortIni","UdpPort","8888","SysIni\\SysIni.ini");
					}
					else
					{
						LocaPort=int.Parse(temp.ToString( ));
					}
					//
					i = GetPrivateProfileString("PortIni","BindIp1","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						LocaIP="127.0.0.3";
						WritePrivateProfileString("PortIni","BindIp1","127.0.0.3","SysIni\\SysIni.ini");
					}
					else
					{
						LocaIP=temp.ToString( );
					}
					//
					//
					i = GetPrivateProfileString("PortIni","BindIp2","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						LocaIP2="127.0.0.2";
						WritePrivateProfileString("PortIni","BindIp2","127.0.0.2","SysIni\\SysIni.ini");
					}
					else
					{
						LocaIP2=temp.ToString( );
					}
					//
					i= GetPrivateProfileString("PortIni","UdpPort2","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						LocaPort2=8889;
						WritePrivateProfileString("PortIni","UdpPort2","8889","SysIni\\SysIni.ini");
					}
					else
					{
						LocaPort2=int.Parse(temp.ToString( ));
					}
					//
					i=GetPrivateProfileString("Instruction","AgainCount","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						AgainCount=3;
						WritePrivateProfileString("Instruction","AgainCount","3","SysIni\\SysIni.ini");
					}
					else
					{
                        AgainCount=int.Parse(temp.ToString( ));
					}
					//
					i = GetPrivateProfileString("Show","Mobile","",temp,255,"SysIni\\SysIni.ini");
					if (i==0)
					{
						strTerminal="";
						WritePrivateProfileString("Show","Mobile","","SysIni\\SysIni.ini");
					}
					else
					{
						strTerminal=temp.ToString( );
					}
				}
				else
				{
					//File.Create("SysIni\\SysIni.ini",255);
					LocaPort=8888;
					WritePrivateProfileString("PortIni","UdpPort","8888","SysIni\\SysIni.ini");
					sCompanyName="XX科技";
					WritePrivateProfileString("Company","Name","","SysIni\\SysIni.ini");
					RemoteIp="127.0.0.1";
					WritePrivateProfileString("PortIni","TcpAddress","127.0.0.1","SysIni\\SysIni.ini");
					RemotePort=10001;
					WritePrivateProfileString("PortIni","TcpPort","10001","SysIni\\SysIni.ini");
					LocaIP="127.0.0.1";
					WritePrivateProfileString("PortIni","BindIp1","127.0.0.1","SysIni\\SysIni.ini");
					LocaPort2=8889;
					WritePrivateProfileString("PortIni","UdpPort2","8889","SysIni\\SysIni.ini");
					LocaIP2="127.0.0.1";
					WritePrivateProfileString("PortIni","BindIp2","127.0.0.1","SysIni\\SysIni.ini");
					CdmaPort=8890;
					WritePrivateProfileString("PortIni","CdmaPort","8890","SysIni\\SysIni.ini");
					AgainCount=3;
					WritePrivateProfileString("Instruction","AgainCount","3","SysIni\\SysIni.ini");

					DbServer="127.0.0.1";
					WritePrivateProfileString("DbConn","DbServer","127.0.0.1","SysIni\\SysIni.ini");
					DbName="CenterDb";
					WritePrivateProfileString("DbConn","DbName","CenterDb","SysIni\\SysIni.ini");
					UserId="sa";
					WritePrivateProfileString("DbConn","UserId","sa","SysIni\\SysIni.ini");
					PassWord="";
					WritePrivateProfileString("DbConn","PassWord","","SysIni\\SysIni.ini");
					strTerminal="";
					WritePrivateProfileString("Show","Mobile","","SysIni\\SysIni.ini");
				}
			}
			else
			{
				Directory.CreateDirectory("SysIni");
				//File.Create("SysIni\\SysIni.ini",255);
				
				sCompanyName="XX科技";
				WritePrivateProfileString("Company","Name","","SysIni\\SysIni.ini");
				RemoteIp="127.0.0.1";
				WritePrivateProfileString("PortIni","TcpAddress","127.0.0.1","SysIni\\SysIni.ini");
				RemotePort=10001;
				WritePrivateProfileString("PortIni","TcpPort","10001","SysIni\\SysIni.ini");
				LocaPort=8888;
				WritePrivateProfileString("PortIni","UdpPort","8888","SysIni\\SysIni.ini");
				LocaIP="127.0.0.1";
				WritePrivateProfileString("PortIni","BindIp1","127.0.0.1","SysIni\\SysIni.ini");
				LocaPort2=8889;
				WritePrivateProfileString("PortIni","UdpPort2","8889","SysIni\\SysIni.ini");
				LocaIP2="127.0.0.1";
				WritePrivateProfileString("PortIni","BindIp2","127.0.0.1","SysIni\\SysIni.ini");
				CdmaPort=8890;
				WritePrivateProfileString("PortIni","CdmaPort","8890","SysIni\\SysIni.ini");
				AgainCount=3;
				WritePrivateProfileString("Instruction","AgainCount","3","SysIni\\SysIni.ini");

				DbServer="127.0.0.1";
				WritePrivateProfileString("DbConn","DbServer","127.0.0.1","SysIni\\SysIni.ini");
				DbName="CenterDb";
				WritePrivateProfileString("DbConn","DbName","CenterDb","SysIni\\SysIni.ini");
				UserId="sa";
				WritePrivateProfileString("DbConn","UserId","sa","SysIni\\SysIni.ini");
				PassWord="";
				WritePrivateProfileString("DbConn","PassWord","","SysIni\\SysIni.ini");
				strTerminal="";
				WritePrivateProfileString("Show","Mobile","","SysIni\\SysIni.ini");
			}
			ConnStr="Provider=SQLOLEDB;Driver={SQL Server}; Server="+DbServer+";  UID="+UserId +";PWD="+PassWord+"; Database="+DbName+";";
		}
		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		
		private void menuItem1_Click(object sender,System.EventArgs e)
		{  
			//
		}  
		private void menuItem2_Click(object sender,System.EventArgs e)
		{  
			this.Close();  
			Application.Exit();  
		}  

		private void notifyIcon_DBClick(object sender, System.EventArgs e)
		{
			//
			this.Show();
			if (this.WindowState ==FormWindowState.Minimized)
				this.WindowState =FormWindowState.Normal;
			this.Activate();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
				notifyIcon.Visible =false;; 
				notifyIcon.Icon=null;
				notifyIcon.Dispose();
				m_Icon1.Dispose();
				m_Icon2.Dispose();
				m_Icon3.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GprsServer));
            this.TimerIcon = new System.Windows.Forms.Timer(this.components);
            this.lblInfo = new System.Windows.Forms.Label();
            this.TimerConn = new System.Windows.Forms.Timer(this.components);
            this.lvwMsg = new System.Windows.Forms.ListView();
            this.sId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sNote = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cmdExit = new System.Windows.Forms.Button();
            this.cmdSet = new System.Windows.Forms.Button();
            this.TimerCount = new System.Windows.Forms.Timer(this.components);
            this.TimerCdma = new System.Timers.Timer();
            this.labDebug = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TimerCdma)).BeginInit();
            this.SuspendLayout();
            // 
            // TimerIcon
            // 
            this.TimerIcon.Enabled = true;
            this.TimerIcon.Interval = 1000;
            this.TimerIcon.Tick += new System.EventHandler(this.TimerIcon_Tick);
            // 
            // lblInfo
            // 
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInfo.Location = new System.Drawing.Point(5, 383);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(342, 30);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimerConn
            // 
            this.TimerConn.Enabled = true;
            this.TimerConn.Interval = 6000;
            this.TimerConn.Tick += new System.EventHandler(this.TimerConn_Tick);
            // 
            // lvwMsg
            // 
            this.lvwMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sId,
            this.sNote});
            this.lvwMsg.FullRowSelect = true;
            this.lvwMsg.Location = new System.Drawing.Point(640, 291);
            this.lvwMsg.Name = "lvwMsg";
            this.lvwMsg.Size = new System.Drawing.Size(99, 48);
            this.lvwMsg.SmallImageList = this.imgList;
            this.lvwMsg.TabIndex = 1;
            this.lvwMsg.UseCompatibleStateImageBehavior = false;
            this.lvwMsg.View = System.Windows.Forms.View.Details;
            // 
            // sId
            // 
            this.sId.Text = "类型";
            this.sId.Width = 70;
            // 
            // sNote
            // 
            this.sNote.Text = "消息内容";
            this.sNote.Width = 522;
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "");
            this.imgList.Images.SetKeyName(1, "");
            this.imgList.Images.SetKeyName(2, "");
            this.imgList.Images.SetKeyName(3, "");
            this.imgList.Images.SetKeyName(4, "");
            this.imgList.Images.SetKeyName(5, "");
            this.imgList.Images.SetKeyName(6, "");
            this.imgList.Images.SetKeyName(7, "");
            this.imgList.Images.SetKeyName(8, "");
            this.imgList.Images.SetKeyName(9, "");
            // 
            // checkBox1
            // 
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox1.Location = new System.Drawing.Point(539, 388);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(138, 25);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "显示调试数据";
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cmdExit
            // 
            this.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdExit.Location = new System.Drawing.Point(869, 386);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(80, 28);
            this.cmdExit.TabIndex = 3;
            this.cmdExit.Text = "退出(&E)";
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // cmdSet
            // 
            this.cmdSet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdSet.Location = new System.Drawing.Point(773, 386);
            this.cmdSet.Name = "cmdSet";
            this.cmdSet.Size = new System.Drawing.Size(80, 28);
            this.cmdSet.TabIndex = 4;
            this.cmdSet.Text = "设置(&S)";
            this.cmdSet.Click += new System.EventHandler(this.cmdSet_Click);
            // 
            // TimerCount
            // 
            this.TimerCount.Enabled = true;
            this.TimerCount.Interval = 6000;
            this.TimerCount.Tick += new System.EventHandler(this.TimerCount_Tick);
            // 
            // TimerCdma
            // 
            this.TimerCdma.Enabled = true;
            this.TimerCdma.Interval = 6000D;
            this.TimerCdma.SynchronizingObject = this;
            this.TimerCdma.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimedEvent);
            // 
            // labDebug
            // 
            this.labDebug.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labDebug.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labDebug.Location = new System.Drawing.Point(5, 3);
            this.labDebug.Name = "labDebug";
            this.labDebug.Size = new System.Drawing.Size(950, 372);
            this.labDebug.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(669, 386);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 28);
            this.button1.TabIndex = 9;
            this.button1.Text = "刷新";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatus.ForeColor = System.Drawing.Color.Blue;
            this.lblStatus.Location = new System.Drawing.Point(355, 383);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(178, 30);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GprsServer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 18);
            this.ClientSize = new System.Drawing.Size(969, 428);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labDebug);
            this.Controls.Add(this.cmdSet);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lvwMsg);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GprsServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "中继服务器GPRS终端接收程序(SW-GPS 2.05)";
            this.Load += new System.EventHandler(this.GprsServer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TimerCdma)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new GprsServer());
		}

        /// <summary>
        /// 定时检测CDMA监听状态
        /// </summary>
		public void OnTimedEvent(object source, ElapsedEventArgs e)								  
		{
			try
			{
				if(!CdmaListen )
				{
					BeginListenCdma();
				}
			}
			catch(Exception)
			{}
			
		}

		public static byte Get_CheckXor(ref byte[] temp,int len)
		{
			byte A=0;
			for(int i=0;i<len;i++)
			{
				A^=temp[i];
			}
			return A;
		}

		private void ReadTcpMsg()  //读取用户发送的指令数据
		{
			byte[] buff= new byte[2048];
			string CartIpAddress="";      //车辆的IP地址
			string IporInstruction="";    //车载地址及指令信令(重发指令)
			EndPoint TempRemote = null;
			string BuffToStr="";

			int recv = 0;
			byte[] Tbuff;
			byte[] AllBuff;
			int iLenght=0;
			int iIndex=0;
			int iLen=0;
			int iXorValue=0;

			while(true)
			{
				try
				{
					if(!TcpSocket.Connected)
					{
						System.Threading.Thread.Sleep(1);
						break;
					}
					buff=new byte[8192];
					recv=0;
					recv=TcpSocket.Receive(buff);//读取数据内容
					if (recv==0)
					{
						System.Threading.Thread.Sleep(1);
						continue;
					}

				}
				catch
				{
					TcpSocket.Shutdown(SocketShutdown.Both);
					TcpSocket.Close();
					break;
				}
				try
				{
					AllBuff = new byte[recv + TcpBuff.Length];
					System.Array.Copy(TcpBuff,0,AllBuff,0,TcpBuff.Length);
					System.Array.Copy(buff,0,AllBuff,TcpBuff.Length,recv);
//					for (iIndex = 1; iIndex <=TcpBuff.Length; iIndex++)
//					{
//						AllBuff[iIndex-1]=TcpBuff[iIndex-1];
//					}
//
//					for (iIndex = 1; iIndex <=recv; iIndex++)
//					{
//						AllBuff[TcpBuff.Length+iIndex-1]=buff[iIndex-1];
//					}

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
							//检测当前指令是否是完整的指令,查找数据包头  兼容协议
							if ((AllBuff[iIndex-1]==0x29 & AllBuff[iIndex]==0x29)|(AllBuff[iIndex-1]==0x24&AllBuff[iIndex]==0x24))
							{
								if ((AllBuff.Length -iIndex)>=(AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+4))
								{
									//检测当前指令是否是完整的指令  兼容协议
									if (((AllBuff[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+iIndex+3])==0x0D)|((AllBuff[AllBuff[iIndex+2]*256+AllBuff[iIndex+3]+iIndex+3])==0x0A))
									{
										//在接收的数据中获取单条完整的指令数据
										Tbuff=new byte[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+5];
										for (iLen = 1; iLen <=Tbuff.Length; iLen++)
										{
											Tbuff[iLen-1]=AllBuff[iLen+iIndex-2];
										}
										iXorValue=Get_CheckXor(ref Tbuff,Tbuff.Length-2);
										if(iXorValue!=Tbuff[Tbuff.Length-2])
										{
											//校验不合格，继续查找合法指令数据
											continue;
										}
										else
										{
											if(Tbuff[2]==0x96)
											{//刷新车辆信息
												DataBase.GetAllProtocolIP();
												iIndex=iIndex+Tbuff.Length-1;
												TcpBuff=new byte[1];
												continue;
											}
											//获取车载终端手机号
											CartIpAddress=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8];
                                            #region
                                            if (bTsFlag)
											{
												if (strTerminal.Trim()!="")
												{
													if (strTerminal.Trim()==CartIpAddress)
													{	
														BuffToStr="";     //string BuffToStr="";
														for(int i=0;i<Tbuff.Length;i++)
														{
															if (i==Tbuff.Length-1)
															{
																BuffToStr+=Tbuff[i].ToString("X2");
															}
															else
															{
																BuffToStr+=Tbuff[i].ToString("X2")+" ";
															}
														}
														ShowSysMsg("下行指令",BuffToStr,2);
													}
												}
												else
												{
													BuffToStr="";     //string BuffToStr="";
													for(int i=0;i<Tbuff.Length;i++)
													{
														if (i==Tbuff.Length-1)
														{
															BuffToStr+=Tbuff[i].ToString("X2");
														}
														else
														{
															BuffToStr+=Tbuff[i].ToString("X2")+" ";
														}
													}
													ShowSysMsg("下行指令",BuffToStr,2);
												}
											}
                                            #endregion
                                            tVehInfo pVehInfo = null;
											pVehInfo=(tVehInfo)DataBase.CarInfo_Hash[CartIpAddress];//IP所对应车载终端信息
											int iEquiptype=pVehInfo.isendtype;
											//int iEquiptype=DataBase.GetEquiptype(CartIpAddress);    // CDMA用户指令下发
											//*******************指令重发*************************************************
											if (iEquiptype==0)  	// UDP再加入到重发指令哈希表
											{
												switch (Tbuff[2])
												{
													case 0x30:case 0x31:case 0x34:case 0x38: case 0x39:case 0x63:case 0x3D:case 0x3E:case 0x3F:case 0x40:case 0x41:case 0x46:case 0x47:case 0x3A:case 0x27:case 0x29:case 0x26:case 0x65:case 0x28:case 0x25:case 0x74:case 0x75:case 0x7C:case 0x3C:case 0x86:case 0x7A:
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+Tbuff[2];
														break;
													default:
														IporInstruction="";
														break;
												}
												if (IporInstruction!="")
												{
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{  
														Instruction_Count[IporInstruction]=0;    //刷新重发次数
														//break;
													}
													else
													{
														//Instruction_Hash.Add(IporInstruction,System.Text.Encoding.ASCII.GetString(Tbuff));
														BuffToStr="";     //string BuffToStr="";
														for(int i=0;i<Tbuff.Length;i++)
														{
															if (i==Tbuff.Length-1)
															{
																BuffToStr+=Tbuff[i].ToString("X2");
															}
															else
															{
																BuffToStr+=Tbuff[i].ToString("X2")+" ";
															}
														}
														Instruction_Hash.Add(IporInstruction,BuffToStr);
														Instruction_Count.Add(IporInstruction,0);  //重发次数
													}
												}
											}
											//*******************指令重发*************************************************
											if (iEquiptype==0)
											{
												TempRemote=(EndPoint)CarID_RemoteIP_Hash[CartIpAddress];
											}
											else
											{
												TempRemote=(EndPoint)CarID_TcpIP_Hash[CartIpAddress]; 
											}
											if(TempRemote == null)
											{
												if (bTsFlag)
												{
													if (strTerminal.Trim()!="")
													{
														if (strTerminal.Trim()==CartIpAddress)
														{
															ShowSysMsg("系统信息","无法查找到接收数据的远程终节点!",3);
														}
													}
													else
													{
														ShowSysMsg("系统信息","无法查找到接收数据的远程终节点!",3);
													}
												}
											}
											else
											{
												try
												{
													if (iEquiptype==0)  // UDP GPRS用户指令下发
													{
														if(csVehSock.ContainsKey((string)CartIpAddress))
														{
															int selSock=Int32.Parse(csVehSock[(string)CartIpAddress].ToString());
															if(selSock==1)
															{
																socket2.SendTo(Tbuff,TempRemote);
															}
															else
															{
																socket.SendTo(Tbuff,TempRemote);
															}
															if (bTsFlag)
															{
																if (strTerminal.Trim()!="")
																{
																	if (strTerminal.Trim()==CartIpAddress)
																	{
																		ShowSysMsg("下行数据!","数据成功转发到车载终端!",1);
																	}
																}
																else
																{
																	//ShowSysMsg("数据成功转发到车载终端!",1);
																}
															}
														
															if (bTsFlag)
															{
															}
														
														}
													}
													else    // TCP车机指令下发
													{
														//*********************************
														//CdmaSocket.SendTo(Tbuff,TempRemote); 
														//向车辆所属用户转发车辆GPRS数据
														for(int i=0;i<CdmaClients.Count;i++)
														{ 
															//根据车辆用户列表中存放的客户SOCKET实例化一个临时的Socket
															SocketTemp=(Socket)CdmaClients[i];
															if (SocketTemp.Connected)
															{
																try	
																{
																	if (SocketTemp.RemoteEndPoint.ToString() == TempRemote.ToString())   //060909 qingweiXie
																	{
																		// SocketTemp.SendTo(Tbuff,TempRemote);   // CDMA用户指令下发
																		//if (SocketTemp.Connected)
																		SocketTemp.BeginSend(Tbuff,0,Tbuff.Length ,SocketFlags.None ,null,null);  //061124 qingweiXie
																		if (bTsFlag)
																		{
																			if (strTerminal.Trim()!="")
																			{
																				if (strTerminal.Trim()==CartIpAddress)
																				{
																					ShowSysMsg("系统信息","数据成功转发到车载终端!",1);
																				}
																			}
																			else
																			{
																				ShowSysMsg("系统信息","数据成功转发到车载终端!",1);
																			}
																		}
																	}
																}
																catch
																{
																	//ShowSysMsg("系统信息","Socket连接不成功!",1);
																	SocketTemp.Shutdown(SocketShutdown.Both);
																	SocketTemp.Close();
																	CdmaClients.RemoveAt(i);//出错后，将车辆类别中存放的该用户socket对象删除
																	i=i-1;
																}
															}
															else
															{
																//ShowSysMsg("系统信息","Socket连接不成功!",1);			
																CdmaClients.RemoveAt(i);//出错后，将车辆类别中存放的该用户socket对象删除
																i=i-1;
															}
														}
														//*********************************
													}
												
												}
												catch
												{
													if (bTsFlag)
													{
														if (strTerminal.Trim()!="")
														{
															if (strTerminal.Trim()==CartIpAddress)
															{
																ShowSysMsg("系统信息","数据转发到车载终端失败!",3);
															}
														}
														else
														{
															ShowSysMsg("系统信息","数据转发到车载终端失败!",3);
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
				catch(Exception ce)
				{
					WriteErrLog("ReadTcpMsg",ce.Message.ToString()+";"+ce.StackTrace.ToString());
				}
			}
		}

		public void ShowSysMsg(string sInfo,string sNote,int iIcon)
		{
			//显示系统消息
			try
			{
				if(iRecOrderCounts>10)
				{
					iRecOrderCounts=0;
					labDebug.Text="";
				}
				iRecOrderCounts++;
				labDebug.Text+=sNote+"\r\n";
			}
			catch
			{
				//lvwMsg.EndUpdate();
			}
		}
        
        /// <summary>
        /// 开启CMDA TCP监听线程
        /// </summary>
		private void BeginListenCdma()
		{
			try
			{
				if (thCdma!=null&& thCdma.IsAlive)
				{ 
					thCdma.Abort();
				}
				IPAddress myCdma = IPAddress.Parse(LocaIP2); 
				Listen=new TcpListener(myCdma ,CdmaPort);
				//Listen=new TcpListener(IPAddress.Any ,CdmaPort);
				Listen.Start(); //侦听

				thCdma=new Thread(new ThreadStart(ServiceListen));//实例化接收用户连接线程
				thCdma.Name ="ListenCdma";  //定义线程名称
				thCdma.IsBackground =true;//指定线程在后台执行
				thCdma.Start(); //线程开始
				CdmaListen=true;		
               
				ShowSysMsg("系统信息","系统成功在TCP:"+CdmaPort + "端口侦听!",6);
			}
			catch (Exception)
			{
				CdmaListen=false;
			}
		}

        /// <summary>
        /// CDMA TCP连接监听线程
        /// </summary>
		private void  ServiceListen()     
		{
			string strFile="";
			strFile=Application.StartupPath+"\\Tcp.txt";
			while(true)
			{
				try
				{
					//System.Threading.Thread.Sleep(1);   //Thread.Sleep(100),这样ＣＰＵ就不会是１００％了．
					//if (Listen.Pending())  //确定队列中是否有可用的连接请求 07/10/15 Xie
					//{2008-11-06 lzx
						Socket CdmaSocket=Listen.AcceptSocket();  //接收CDMA连接请求
						//20071019  TCP有一个连接检测机制，就是如果在指定的时间内（一般为2个小时）没有数据传送，会给对端发送一个Keep-Alive
						uint dummy = 0;
						byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
						BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
						BitConverter.GetBytes((uint)600000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));       //10分钟后开始检测
						BitConverter.GetBytes((uint)20000).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);    //每20秒发送一次

						const uint SIO_KEEPALIVE_VALS = 0x80000000 | 0x18000000 | 4;
						unchecked
						{
							int sio = (int)SIO_KEEPALIVE_VALS;
							CdmaSocket.IOControl(sio, inOptionValues, null);     //2003写的，没有枚举值IOControlCode.KeepAliveValues
						}
						//20071019 第一次探测是在最后一次数据发送的两个小时，然后每隔1秒探测一次，一共探测5次，如果5次都没有收到回应的话，就会断开这个连接。但两个小时对于我们的项目来说显然太长了。我们必须缩短这个时间。那么我们该如何做呢？我要利用Socket类的IOControl()函数。
						CdmaSocket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendTimeout ,1000);  //设置客户Socket的属性
						//CdmaSocket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress,true);  //实现端口复用 07/10/15 Xie 
						//接收CDMA数据并处理
						CdmaProcess tp=new CdmaProcess(CdmaSocket);
						
						Thread CdmaRecv=new Thread(new ThreadStart(tp.ProcessService)); //实例客户(Socket)套接字接收线程
						CdmaRecv.IsBackground=true; //指定线程在后台执行                                                              //
						CdmaRecv.Start();    //线程开始  
					//}
				}
				catch(Exception e)
				{
					if (System.IO.File.Exists(strFile))
					{
						StreamWriter swWriter=new  StreamWriter(strFile,true);
						swWriter.WriteLine("AcceptSocket Error,时间:"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+";错误代码:"+e.Source.ToString()+";错误提示:"+e.Message);
						swWriter.Flush();
						swWriter.Close();
					}
					//ShowSysMsg("错误代码:" +e.Source.ToString()+";错误提示:"+e.Message ,3);
				}
			} 
		}

		private void ReadUdp()      //从UDP数据端口读取GPRS数据
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
			byte[] RecvAffirmBuff=new byte[]{0x29,0x29,0x21,0x00,0x05,0,0,0,0,0x0D};//回应终端数组
			byte[] RecvAffirmBuffC=new byte[]{0x24,0x24,0x21,0x00,0x05,0,0,0,0,0x0A};  //回应终端数组  兼容协议
            // CHENT
			IPAddress myIP = IPAddress.Parse(LocaIP); 

            IPEndPoint ipep = new IPEndPoint(myIP ,LocaPort);
			//CHENT
			//*IPEndPoint ipep = new IPEndPoint(IPAddress.Any ,LocaPort);	
			//socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			try
			{
				socket.Bind(ipep);
				ShowSysMsg("系统信息","系统成功在UDP:"+LocaPort + "端口侦听!",6);
			}
			catch
			{
				ShowSysMsg("系统信息","端口"+LocaPort+"已被占用，系统侦听失败!",5);
				return;
			}

			IPEndPoint sender = new IPEndPoint(IPAddress.Any , 0);//指远程终端（终节点）Ip地址对象 IPAddress.Any表示任何地址 0 表示任何端口
			EndPoint remote = (EndPoint)(sender);	//指远程终端（终节点）

			while(true)
			{
				try
				{
					buff=new byte[8192];
					recv = socket.ReceiveFrom(buff , ref remote);//引用参数输入之前必须初始化，侧重于修改
				}
				catch(SocketException se)
				{
					WriteErrLog("GprsServer","ReadUdp:"+se.Message.ToString()+";"+se.ErrorCode);
				}
				catch(Exception ep)
				{
					WriteErrLog("GprsServer","ReadUdp:"+ep.Message.ToString()+";"+ep.StackTrace.ToString());
				}
					try
					{
						//---------------------2008-11-06 lzx------------------------------//
						AllBuff=new byte[recv +TempBuff.Length];
						for (iIndex = 1; iIndex <=TempBuff.Length; iIndex++)
						{
							AllBuff[iIndex-1]=TempBuff[iIndex-1];
						}

						for (iIndex = 1; iIndex <=recv; iIndex++)
						{
							AllBuff[TempBuff.Length+iIndex-1]=buff[iIndex-1];
						}

						for (iIndex = 1; iIndex <=AllBuff.Length; iIndex++)
						{
							iLenght=AllBuff.Length-iIndex+1;
							if (iLenght<6)//检测数据包长度
							{
								//不完整，则将指令保存
								if (iLenght>0)
								{
									TempBuff=new byte[iLenght];
									for (iLen = 1; iLen <=iLenght; iLen++)
									{
										TempBuff[iLen-1]=AllBuff[iLen+iIndex-2];
									}
								}
								break;
							}
							else
							{
								//检测当前指令是否是完整的指令,查找数据包头   兼容协议
								if ((AllBuff[iIndex-1]==0x29 & AllBuff[iIndex]==0x29)|(AllBuff[iIndex-1]==0x24&AllBuff[iIndex]==0x24))
								{
									if ((AllBuff.Length -iIndex)>=(AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+4))
									{
										//检测当前指令是否是完整的指令  兼容协议
										if (((AllBuff[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+iIndex+3])==0x0D)|((AllBuff[AllBuff[iIndex+2]*256+AllBuff[iIndex+3]+iIndex+3])==0x0A))
										{
											//在接收的数据中获取单条完整的指令数据
											Tbuff=new byte[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+5];
											for (iLen = 1; iLen <=Tbuff.Length; iLen++)
											{
												Tbuff[iLen-1]=AllBuff[iLen+iIndex-2];
											}
											iXorValue=Get_CheckXor(ref Tbuff,Tbuff.Length-2);
											if(iXorValue!=Tbuff[Tbuff.Length-2])
											{
												//校验不合格，继续查找合法指令数据
												continue;
											}
											else
											{
												//获取车载终端手机号
												CarIpAddress=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8];

												if (bTsFlag)
												{
													if (strTerminal.Trim()!="")
													{
														if (strTerminal.Trim()==CarIpAddress)
														{
															string BuffToStr="";
															for(int i=0;i<Tbuff.Length;i++)
															{
																BuffToStr+=Tbuff[i].ToString("X2")+" ";
															}
															ShowSysMsg("上行UDP",BuffToStr,0);
														}
													}
													else
													{
														string BuffToStr="";
														for(int i=0;i<Tbuff.Length;i++)
														{
															BuffToStr+=Tbuff[i].ToString("X2")+" ";
														}
														ShowSysMsg("上行UDP",BuffToStr,0);
													}
												}
												
												//*******************指令重发*************************************************
												switch (Tbuff[2])
												{
													case 0x81:
														byte[] bTemp=new byte[] {0x30};
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp[0];
														if (Instruction_Hash.ContainsKey(IporInstruction))
														{
															Instruction_Hash.Remove(IporInstruction);
															Instruction_Count.Remove(IporInstruction);
														}
														break;
													case 0x83:
														byte[] bTemp1=new byte[] {0x31};
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp1[0];
														if (Instruction_Hash.ContainsKey(IporInstruction))
														{
															Instruction_Hash.Remove(IporInstruction);
															Instruction_Count.Remove(IporInstruction);
														}
														break;
													case 0xB3:
														byte[] bTemp4=new byte[] {0x31};
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp4[0];
														if (Instruction_Hash.ContainsKey(IporInstruction))
														{
															Instruction_Hash.Remove(IporInstruction);
															Instruction_Count.Remove(IporInstruction);
														}
														break;
													case 0xB2:
														byte[] bTemp5=new byte[] {0x3D};
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp5[0];
														if (Instruction_Hash.ContainsKey(IporInstruction))
														{
															Instruction_Hash.Remove(IporInstruction);
															Instruction_Count.Remove(IporInstruction);
														}
														break;
													case 0x87:
														byte[] bTemp2=new byte[] {0x63};
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp2[0];
														if (Instruction_Hash.ContainsKey(IporInstruction))
														{
															Instruction_Hash.Remove(IporInstruction);
															Instruction_Count.Remove(IporInstruction);
														}
														break;
													case 0x7C:
														byte[] bTemp3=new byte[] {0x7C};
														IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp3[0];
														if (Instruction_Hash.ContainsKey(IporInstruction))
														{
															Instruction_Hash.Remove(IporInstruction);
															Instruction_Count.Remove(IporInstruction);
														}
														break;
													case 0x85:
														if (Tbuff.Length>=40)
														{
															switch (Tbuff[42])
															{
																case 0x34:case 0x38:case 0x39:case 0x3D:case 0x3E:case 0x3F:case 0x40:case 0x41:case 0x46:case 0x47:case 0x3A:case 0x27:case 0x29:case 0x26:case 0x65:case 0x28:case 0x25:case 0x74:case 0x75:case 0x7C:case 0x3C:case 0x86:case 0x7A:
																	IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+Tbuff[42];
																	if (Instruction_Hash.ContainsKey(IporInstruction))
																	{
																		Instruction_Hash.Remove(IporInstruction);
																		Instruction_Count.Remove(IporInstruction);
																	}
																	break;
																default:
																	break;
															}
														}
														else
														{
															switch (Tbuff[9])
															{
																case 0x34:case 0x38:case 0x39:case 0x3D:case 0x3E:case 0x3F:case 0x40:case 0x41:case 0x46:case 0x47:case 0x3A:case 0x27:case 0x29:case 0x26:case 0x65:case 0x28:case 0x25:case 0x74:case 0x75:case 0x7C:case 0x3C:case 0x86:case 0x7A:
																	IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+Tbuff[9];
																	if (Instruction_Hash.ContainsKey(IporInstruction))
																	{
																		Instruction_Hash.Remove(IporInstruction);
																		Instruction_Count.Remove(IporInstruction);
																	}
																	break;
																default:
																	break;
															}
														}
														break;
													default:
														break;
												}
                                        
												//*******************指令重发*************************************************

												//-----------------检测系统哈希表是否包含此终端数据---------------------\\
												if(CarID_RemoteIP_Hash.ContainsKey(CarIpAddress))
												{
													CarID_RemoteIP_Hash[CarIpAddress]=remote;//有更新
												}
												else
												{
													CarID_RemoteIP_Hash.Add(CarIpAddress,remote);//没有添加
												}
												if(csVehSock.ContainsKey(CarIpAddress))
												{
													csVehSock[CarIpAddress]=0;
												}
												else
												{
													csVehSock.Add(CarIpAddress,0);
												}
												//-------------------向终端发出0x21的接收确认-------------------\\
												if (Tbuff[0]==0x29 & Tbuff[1]==0x29)   
												{
													//----------修改过后的应答,新的80指令如果第三十个字节为1就应答,否则不应答
													/*
													if (Tbuff.Length>=40 & Tbuff[2]==0x80)
													{
														byte bySwitch=0x40;
														if ((Tbuff[33]&bySwitch)!=0)
														{
															RecvAffirmBuff[5]=Tbuff[Tbuff.Length-2];
															RecvAffirmBuff[6]=Tbuff[2];
															RecvAffirmBuff[7]=Tbuff[9];
															RecvAffirmBuff[8]=Get_CheckXor(ref RecvAffirmBuff,8);
															socket.SendTo(RecvAffirmBuff,remote);
														}
													}
													else
													{
													*/
													//----------修改过后的应答,新的80指令如果第三十个字节为1就应答,否则不应答
													RecvAffirmBuff[5]=Tbuff[Tbuff.Length-2];
													RecvAffirmBuff[6]=Tbuff[2];
													RecvAffirmBuff[7]=Tbuff[9];
													RecvAffirmBuff[8]=Get_CheckXor(ref RecvAffirmBuff,8);
													socket.SendTo(RecvAffirmBuff,remote);
													//}
												}
												else   //兼容协议
												{
													RecvAffirmBuffC[5]=Tbuff[Tbuff.Length-2];
													RecvAffirmBuffC[6]=Tbuff[2];
													RecvAffirmBuffC[8]=Get_CheckXor(ref RecvAffirmBuffC,8);
													socket.SendTo(RecvAffirmBuffC,remote);
												}
												iIndex=iIndex+Tbuff.Length-1;
												TempBuff=new byte[1];

												//--------------------将数据转发到中心处理程序-----------------\\
												if (TcpSocket.Connected)
												{
													try
													{
														TcpSocket.Send(Tbuff,0,Tbuff.Length,SocketFlags.None);
														if (bTsFlag)
														{
															if (strTerminal.Trim()!="")
															{
																if (strTerminal.Trim()==CarIpAddress)
																{
																	ShowSysMsg("系统信息","数据成功转发到网络中心处理程序!",1);
																}
															}
															else
															{
																ShowSysMsg("系统信息","数据成功转发到网络中心处理程序!",1);
															}
														}
													}
													catch
													{
														if (bTsFlag)
														{
															if (strTerminal.Trim()!="")
															{
																if (strTerminal.Trim()==CarIpAddress)
																{
																	ShowSysMsg("系统信息","数据转发到网络中心处理程序失败!",3);
																}
															}
															else
															{
																ShowSysMsg("系统信息","数据转发到网络中心处理程序失败!",3);
															}
														}
													}
												}
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
												TempBuff=new byte[iLenght];
												for (iLen = 1; iLen <=iLenght; iLen++)
												{
													TempBuff[iLen-1]=AllBuff[iLen+iIndex-2];
												}
												break;
											}
											else
											{
												TempBuff=new byte[1];
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
						System.Threading.Thread.Sleep(10);
					}					
					catch(Exception ce)
					{
						System.Threading.Thread.Sleep(10);
						WriteErrLog("ReadUdp1",ce.Message.ToString()+ce.StackTrace.ToString());
					}
							
				
			}
    	}

        /// <summary>
        /// 异步发送完成回调函数
        /// </summary>
		protected virtual void SendTo_Callback(IAsyncResult ar) 
		{ 
			StateObject so  = (StateObject)ar.AsyncState; 
			Socket s  =  so.workSocket; 
			int send  =  s.EndSendTo(ar); 
			s.Close(); 
			Console.WriteLine( "Send  Complete ");     //   这里才是真正发送结束的地方 
		}

		private void ReadUdp2()      //从UDP数据端口读取GPRS数据
		{
			byte[] buff = new byte[1024];
			int recv = 0;
			byte[] Tbuff;
			byte[] AllBuff;
			int iLenght=0;
			int iIndex=0;
			int iLen=0;
			int iXorValue=0;
			string CarIpAddress="";
			string IporInstruction="";
			byte[] RecvAffirmBuff=new byte[]{0x29,0x29,0x21,0x00,0x05,0,0,0,0,0x0D};//回应终端数组
			byte[] RecvAffirmBuffC=new byte[]{0x24,0x24,0x21,0x00,0x05,0,0,0,0,0x0A};  //回应终端数组  兼容协议
			// CHENT
			IPAddress myIP2 = IPAddress.Parse(LocaIP2); 

			IPEndPoint ipep2 = new IPEndPoint(myIP2 ,LocaPort2);
			socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			try
			{
				socket2.Bind(ipep2);
				ShowSysMsg("系统信息","系统成功在UDP:"+LocaPort2 + "端口侦听!",6);
			}
			catch
			{
				ShowSysMsg("系统信息","端口"+LocaPort2+"已被占用，系统侦听失败!",5);
				return;
			}
			IPEndPoint sender2 = new IPEndPoint(IPAddress.Any , 0);//指远程终端（终节点）Ip地址对象 IPAddress.Any表示任何地址 0 表示任何端口
			EndPoint remote2 = (EndPoint)(sender2);	//指远程终端（终节点）

			while(true)
			{
				try
				{
					buff=new byte[8192];
					recv = socket2.ReceiveFrom(buff , ref remote2);
				}
				catch
				{
					if (bTsFlag)
					{
						//ShowSysMsg("接收车载终端数据错误!",3);
					}
				}
				try
				{
					//---------------------you2004-12-31 begin------------------------------//
					AllBuff=new byte[recv +TempBuff.Length];
					for (iIndex = 1; iIndex <=TempBuff.Length; iIndex++)
					{
						AllBuff[iIndex-1]=TempBuff[iIndex-1];
					}

					for (iIndex = 1; iIndex <=recv; iIndex++)
					{
						AllBuff[TempBuff.Length+iIndex-1]=buff[iIndex-1];
					}

					for (iIndex = 1; iIndex <=AllBuff.Length; iIndex++)
					{
						iLenght=AllBuff.Length-iIndex+1;
						if (iLenght<6)//检测数据包长度
						{
							//不完整，则将指令保存
							if (iLenght>0)
							{
								TempBuff=new byte[iLenght];
								for (iLen = 1; iLen <=iLenght; iLen++)
								{
									TempBuff[iLen-1]=AllBuff[iLen+iIndex-2];
								}
							}
							break;
						}
						else
						{
							//检测当前指令是否是完整的指令,查找数据包头   兼容协议
							if ((AllBuff[iIndex-1]==0x29 & AllBuff[iIndex]==0x29)|(AllBuff[iIndex-1]==0x24&AllBuff[iIndex]==0x24))
							{
								if ((AllBuff.Length -iIndex)>=(AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+4))
								{
									//检测当前指令是否是完整的指令  兼容协议
									if (((AllBuff[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+iIndex+3])==0x0D)|((AllBuff[AllBuff[iIndex+2]*256+AllBuff[iIndex+3]+iIndex+3])==0x0A))
									{
										//在接收的数据中获取单条完整的指令数据
										Tbuff=new byte[AllBuff[iIndex+2]*256+ AllBuff[iIndex+3]+5];
										for (iLen = 1; iLen <=Tbuff.Length; iLen++)
										{
											Tbuff[iLen-1]=AllBuff[iLen+iIndex-2];
										}
										iXorValue=Get_CheckXor(ref Tbuff,Tbuff.Length-2);
										if(iXorValue!=Tbuff[Tbuff.Length-2])
										{
											//校验不合格，继续查找合法指令数据
											continue;
										}
										else
										{
											//获取车载终端手机号
											CarIpAddress=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8];

											if (bTsFlag)
											{
												if (strTerminal.Trim()!="")
												{
													if (strTerminal.Trim()==CarIpAddress)
													{
														string BuffToStr="";
														for(int i=0;i<Tbuff.Length;i++)
														{
															BuffToStr+=Tbuff[i].ToString("X2")+" ";
														}
														ShowSysMsg("上行UDP",BuffToStr,0);
													}
												}
												else
												{
													string BuffToStr="";
													for(int i=0;i<Tbuff.Length;i++)
													{
														BuffToStr+=Tbuff[i].ToString("X2")+" ";
													}
													ShowSysMsg("上行UDP",BuffToStr,0);
												}
											}
										
											//*******************指令重发*************************************************
											switch (Tbuff[2])
											{
												case 0x81:
													byte[] bTemp=new byte[] {0x30};
													IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp[0];
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{
														Instruction_Hash.Remove(IporInstruction);
														Instruction_Count.Remove(IporInstruction);
													}
													break;
												case 0x83:
													byte[] bTemp1=new byte[] {0x31};
													IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp1[0];
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{
														Instruction_Hash.Remove(IporInstruction);
														Instruction_Count.Remove(IporInstruction);
													}
													break;
												case 0xB3:
													byte[] bTemp4=new byte[] {0x31};
													IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp4[0];
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{
														Instruction_Hash.Remove(IporInstruction);
														Instruction_Count.Remove(IporInstruction);
													}
													break;
												case 0xB2:
													byte[] bTemp5=new byte[] {0x3D};
													IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp5[0];
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{
														Instruction_Hash.Remove(IporInstruction);
														Instruction_Count.Remove(IporInstruction);
													}
													break;
												case 0x87:
													byte[] bTemp2=new byte[] {0x63};
													IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp2[0];
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{
														Instruction_Hash.Remove(IporInstruction);
														Instruction_Count.Remove(IporInstruction);
													}
													break;
												case 0x7C:
													byte[] bTemp3=new byte[] {0x7C};
													IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+bTemp3[0];
													if (Instruction_Hash.ContainsKey(IporInstruction))
													{
														Instruction_Hash.Remove(IporInstruction);
														Instruction_Count.Remove(IporInstruction);
													}
													break;
												case 0x85:
													if (Tbuff.Length>=40)
													{
														switch (Tbuff[42])
														{
															case 0x34:case 0x38:case 0x39:case 0x3D:case 0x3E:case 0x3F:case 0x40:case 0x41:case 0x46:case 0x47:case 0x3A:case 0x27:case 0x29:case 0x26:case 0x65:case 0x28:case 0x25:case 0x74:case 0x75:case 0x7C:case 0x3C:case 0x86:case 0x7A:
																IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+Tbuff[42];
																if (Instruction_Hash.ContainsKey(IporInstruction))
																{
																	Instruction_Hash.Remove(IporInstruction);
																	Instruction_Count.Remove(IporInstruction);
																}
																break;
															default:
																break;
														}
													}
													else
													{
														switch (Tbuff[9])
														{
															case 0x34:case 0x38:case 0x39:case 0x3D:case 0x3E:case 0x3F:case 0x40:case 0x41:case 0x46:case 0x47:case 0x3A:case 0x27:case 0x29:case 0x26:case 0x65:case 0x28:case 0x25:case 0x74:case 0x75:case 0x7C:case 0x3C:case 0x86:case 0x7A:
																IporInstruction=Tbuff[5]+"." +Tbuff[6] +"."+Tbuff[7] +"."+Tbuff[8]+":"+Tbuff[9];
																if (Instruction_Hash.ContainsKey(IporInstruction))
																{
																	Instruction_Hash.Remove(IporInstruction);
																	Instruction_Count.Remove(IporInstruction);
																}
																break;
															default:
																break;
														}
													}
													break;
												default:
													break;
											}
                                        
											//*******************指令重发*************************************************
 
										
											//-----------------检测系统哈希表是否包含此终端数据---------------------\\
											if(CarID_RemoteIP_Hash.ContainsKey(CarIpAddress))
											{
												CarID_RemoteIP_Hash[CarIpAddress]=remote2;//有更新
											}
											else
											{
												CarID_RemoteIP_Hash.Add(CarIpAddress,remote2);//没有添加
											}
											if(csVehSock.ContainsKey(CarIpAddress))
											{
												csVehSock[CarIpAddress]=1;
											}
											else
											{
												csVehSock.Add(CarIpAddress,1);
											}
											//-------------------向终端发出0x21的接收确认-------------------\\
											if (Tbuff[0]==0x29 & Tbuff[1]==0x29)   
											{
												//----------修改过后的应答,新的80指令如果第三十个字节为1就应答,否则不应答
											
												/*if (Tbuff.Length>=40 & Tbuff[2]==0x80)
												{
													byte bySwitch=0x40;
													if ((Tbuff[33]&bySwitch)!=0)
													{
														RecvAffirmBuff[5]=Tbuff[Tbuff.Length-2];
														RecvAffirmBuff[6]=Tbuff[2];
														RecvAffirmBuff[7]=Tbuff[9];
														RecvAffirmBuff[8]=Get_CheckXor(ref RecvAffirmBuff,8);
														socket2.SendTo(RecvAffirmBuff,remote2);
													}
												}
												else
												{
												*/

												//----------修改过后的应答,新的80指令如果第三十个字节为1就应答,否则不应答
												RecvAffirmBuff[5]=Tbuff[Tbuff.Length-2];
												RecvAffirmBuff[6]=Tbuff[2];
												RecvAffirmBuff[7]=Tbuff[9];
												RecvAffirmBuff[8]=Get_CheckXor(ref RecvAffirmBuff,8);
												socket2.SendTo(RecvAffirmBuff,remote2);
												//}
											}
											else
											{
												RecvAffirmBuffC[5]=Tbuff[Tbuff.Length-2];
												RecvAffirmBuffC[6]=Tbuff[2];
												RecvAffirmBuffC[8]=Get_CheckXor(ref RecvAffirmBuffC,8);
												socket2.SendTo(RecvAffirmBuffC,remote2);
											}
											iIndex=iIndex+Tbuff.Length-1;
											TempBuff=new byte[1];

											//--------------------将数据转发到中心处理程序-----------------\\
											if (TcpSocket.Connected)
											{
												try
												{
													TcpSocket.Send(Tbuff,0,Tbuff.Length,SocketFlags.None);
													if (bTsFlag)
													{
														if (strTerminal.Trim()!="")
														{
															if (strTerminal.Trim()==CarIpAddress)
															{
																ShowSysMsg("系统信息","数据成功转发到网络中心处理程序!",1);
															}
														}
														else
														{
															ShowSysMsg("系统信息","数据成功转发到网络中心处理程序!",1);
														}
													}
												}
												catch
												{
													if (bTsFlag)
													{
														if (strTerminal.Trim()!="")
														{
															if (strTerminal.Trim()==CarIpAddress)
															{
																ShowSysMsg("系统信息","数据转发到网络中心处理程序失败!",3);
															}
														}
														else
														{
															ShowSysMsg("系统信息","数据转发到网络中心处理程序失败!",3);
														}
													}
												}
											}
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
											TempBuff=new byte[iLenght];
											for (iLen = 1; iLen <=iLenght; iLen++)
											{
												TempBuff[iLen-1]=AllBuff[iLen+iIndex-2];
											}
											break;
										}
										else
										{
											TempBuff=new byte[1];
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
				catch(Exception ce)
				{
					WriteErrLog("ReadUdp2",ce.Message.ToString()+ce.StackTrace.ToString());
				}
				//---------------------you2004-12-31 end------------------------------//
			}

		}

		private void GprsServer_Load(object sender, System.EventArgs e)
		{
			//
		}

		int I=0;
		private void TimerIcon_Tick(object sender, System.EventArgs e)
		{
			if(thGprs!=null)
			{ 
				if(thGprs.IsAlive)
					notifyIcon.Icon = notifyIcon.Icon == m_Icon1 ? m_Icon2 : m_Icon1;
				else
					notifyIcon.Icon = notifyIcon.Icon == m_Icon1 ? m_Icon3 : m_Icon1;
			}
			I++;
			if(I>5)
			{
				I=FindWindow(null,@sCompanyName);
				if( I!= 0)
				{
					SendMessage(I,0x501,1002,"");
				}
				I=0;
			}		
		}

		//定时重发指令
		private void TimerCount_Tick(object sender,System.EventArgs e)
		{
			EndPoint TempRemote = null;
			string strKey="";
			string strAddress="";
			byte[] bVeh;
			if (Instruction_Count.Count == 0) 
			{
                //this.TimerCount.Enabled =false;
				return;
			}
			else
			{
				//IDictionaryEnumerator myEnumerator = Instruction_Count.GetEnumerator();
				//while (myEnumerator.MoveNext())
				
				foreach (string key in Instruction_Hash.Keys)      //foreach (string key in Instruction_Count.Keys)
				{
					if (int.Parse(Instruction_Count[key].ToString())<AgainCount)
					{
						Instruction_Count[key]=int.Parse(Instruction_Count[key].ToString())+1;
						//***********************
						strKey=key.ToString().Trim();
						strAddress=strKey.Substring(0,strKey.IndexOf(":")).ToString().Trim();
						TempRemote=(EndPoint)CarID_RemoteIP_Hash[strAddress];
						//*************2006/11/24 qingweiXie********
						int iEquiptype=DataBase.GetEquiptype(strAddress);    // CDMA用户指令下发

						if( TempRemote==null)
						{
							if (bTsFlag)
							{
								ShowSysMsg("系统信息","无法查找到接收数据的远程终节点!",3);
								break;
							}
						}
						else
						{
							try
							{
								// 显示重发指令内容
								//byte[] bVeh=System.Text.Encoding.Default.GetBytes(Instruction_Hash[key].ToString());
								char[] splitChar=new char[] {' '};
                                string[] strVehInfo;
                                strVehInfo=Instruction_Hash[key].ToString().Split(splitChar);
								/*string BuffToStr="";
								for(int i=0;i<bVeh.Length;i++)
								{
									BuffToStr+=bVeh[i].ToString("X2")+" ";
								}
                                ShowSysMsg(BuffToStr,0);*/
								bVeh=new byte[strVehInfo.Length];
								for (int i=0 ;i<strVehInfo.Length;i++)
								{
                                    bVeh[i]=(byte)Convert.ToInt32(strVehInfo[i], 16);   //Convert.ToByte(strVehInfo[i]);
								}
								ShowSysMsg("指令重发",Instruction_Hash[key].ToString(),0);
                                // 显示重发指令内容
								if (iEquiptype==0)  // GPRS用户指令下发
								{
									socket.SendTo(bVeh,TempRemote);
									if (bTsFlag)
									{
										ShowSysMsg("系统信息","数据成功转发到车载终端!",1);
									}
									socket2.SendTo(bVeh,TempRemote);
									/*if (bTsFlag)
									{
										ShowSysMsg("数据成功转发到车载终端!",1);
									}*/
								}
								break;      //指令间隔重发  2006/11/27  qingweiXie
							}
							catch
							{
								if (bTsFlag)
								{
									ShowSysMsg("系统信息","数据转发到车载终端失败!",3);
									break;
								}
							}
						}
						//***********************
					}
					else
					{
						Instruction_Hash.Remove(key);
						Instruction_Count.Remove(key);
						return;
					}
				}
			}
		}

        /// <summary>
        /// 中心数据库连接状态检测
        /// </summary>
		private void TimerConn_Tick(object sender, System.EventArgs e)
		{
			if (!TcpSocket.Connected)
			{
				try
				{
					lblInfo.Text ="与中心数据处理程序断开，正在进行二次连接...";
					IPHostEntry IPHost = Dns.Resolve(RemoteIp);
					string []aliases = IPHost.Aliases; 
					IPAddress []addr = IPHost.AddressList;
					EndPoint ep = new IPEndPoint(addr[0],RemotePort); 
					TcpSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
					TcpSocket.Connect(ep);
                    
					thTcpMsg = new Thread ( new ThreadStart(ReadTcpMsg)) ;
					//启动线程 
					thTcpMsg.IsBackground =true;
					thTcpMsg.Start() ;
					lblInfo.Text ="系统运行正常，正在中转GPRS数据...";
				}
				catch
				{
					return;
				}
			}
		}

        /// <summary>
        /// 调试数据显示
        /// </summary>
		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			bTsFlag=checkBox1.Checked;
		}

		private void cmdExit_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("确定关闭中继服务器？", "提示",MessageBoxButtons.YesNo,MessageBoxIcon.Information) ==  DialogResult.Yes)
			{
				Application.Exit ();
			}
		}

        /// <summary>
        /// 参数设置
        /// </summary>  
		private void cmdSet_Click(object sender, System.EventArgs e)
		{
			frmUdpSet frmudpset=new frmUdpSet();
			frmudpset.Show();
		}
	
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel =true;
			this.Hide();
			this.ShowInTaskbar = false;
			this.notifyIcon.Visible = true;
			//this.WindowState =FormWindowState.Minimized;
		}

        /// <summary>
        /// 日志记录
        /// </summary>
		public static void WriteErrLog(string errSubName,string errNote)
		{
			try
			{
				string strFiles="Err-"+ errSubName + ".log";
						
				StreamWriter swWriter=new StreamWriter(strFiles,true);
				swWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+";"+errNote);
				swWriter.Flush();
				swWriter.Close();
			}
			catch(Exception ce)
			{
				string tpFile="NotWriteErrlog.log";
				StreamWriter swWriter=new  StreamWriter(tpFile,true);
				swWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+";"+"写日志文件不成功;过程:"+""+ce.Message.ToString()+";"+ce.StackTrace.ToString());
				swWriter.Flush();
				swWriter.Close();
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			labDebug.Text="";
		}
	}
}
