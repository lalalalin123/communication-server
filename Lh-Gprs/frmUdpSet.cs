using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GprsServer
{
	/// <summary>
	/// frmUdpSet 的摘要说明。
	/// </summary>
	public class frmUdpSet : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtCenterPort;
		private System.Windows.Forms.TextBox txtCenterIP;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtCount;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtPassWord;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TextBox txtDbName;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button cmdSet;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TextBox txtTcpPort;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtMobile;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmUdpSet()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTcpPort = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCount = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtCenterPort = new System.Windows.Forms.TextBox();
            this.txtCenterIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.txtPassWord = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdSet = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTcpPort);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtCount);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.txtCenterPort);
            this.groupBox1.Controls.Add(this.txtCenterIP);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 303);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtTcpPort
            // 
            this.txtTcpPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTcpPort.Location = new System.Drawing.Point(139, 136);
            this.txtTcpPort.Name = "txtTcpPort";
            this.txtTcpPort.Size = new System.Drawing.Size(149, 25);
            this.txtTcpPort.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 139);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 15);
            this.label13.TabIndex = 24;
            this.label13.Text = "本地TCP端口:";
            // 
            // txtCount
            // 
            this.txtCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCount.Location = new System.Drawing.Point(139, 175);
            this.txtCount.Name = "txtCount";
            this.txtCount.Size = new System.Drawing.Size(149, 25);
            this.txtCount.TabIndex = 23;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(43, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 15);
            this.label11.TabIndex = 22;
            this.label11.Text = "重发次数:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "本地UDP端口2:";
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(139, 77);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(149, 25);
            this.textBox3.TabIndex = 20;
            this.textBox3.Text = "8889";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 15);
            this.label6.TabIndex = 19;
            this.label6.Text = "网络IP2:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "网络IP1:";
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(139, 107);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(149, 25);
            this.textBox2.TabIndex = 17;
            this.textBox2.Text = "127.0.0.2";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(139, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(149, 25);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = "192.168.1.10";
            // 
            // txtName
            // 
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Location = new System.Drawing.Point(139, 265);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(149, 25);
            this.txtName.TabIndex = 15;
            // 
            // txtCenterPort
            // 
            this.txtCenterPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCenterPort.Location = new System.Drawing.Point(139, 234);
            this.txtCenterPort.Name = "txtCenterPort";
            this.txtCenterPort.Size = new System.Drawing.Size(149, 25);
            this.txtCenterPort.TabIndex = 14;
            // 
            // txtCenterIP
            // 
            this.txtCenterIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCenterIP.Location = new System.Drawing.Point(139, 206);
            this.txtCenterIP.Name = "txtCenterIP";
            this.txtCenterIP.Size = new System.Drawing.Size(149, 25);
            this.txtCenterIP.TabIndex = 13;
            // 
            // txtPort
            // 
            this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPort.Location = new System.Drawing.Point(139, 18);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(149, 25);
            this.txtPort.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 267);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "托盘名称:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "中心端口:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "中心IP:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "本地UDP端口1:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.txtMobile);
            this.groupBox2.Controls.Add(this.txtPassWord);
            this.groupBox2.Controls.Add(this.txtUserName);
            this.groupBox2.Controls.Add(this.txtDbName);
            this.groupBox2.Controls.Add(this.txtServer);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Location = new System.Drawing.Point(325, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(254, 211);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据库设置";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 149);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(151, 15);
            this.label14.TabIndex = 9;
            this.label14.Text = "只显示伪IP终端数据:";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // txtMobile
            // 
            this.txtMobile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMobile.Location = new System.Drawing.Point(21, 175);
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.Size = new System.Drawing.Size(214, 25);
            this.txtMobile.TabIndex = 8;
            // 
            // txtPassWord
            // 
            this.txtPassWord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassWord.Location = new System.Drawing.Point(93, 113);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(142, 25);
            this.txtPassWord.TabIndex = 3;
            // 
            // txtUserName
            // 
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserName.Location = new System.Drawing.Point(93, 82);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(142, 25);
            this.txtUserName.TabIndex = 2;
            // 
            // txtDbName
            // 
            this.txtDbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDbName.Location = new System.Drawing.Point(93, 51);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(142, 25);
            this.txtDbName.TabIndex = 1;
            // 
            // txtServer
            // 
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServer.Location = new System.Drawing.Point(93, 21);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(142, 25);
            this.txtServer.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 15);
            this.label8.TabIndex = 7;
            this.label8.Text = "口令:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 93);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 15);
            this.label9.TabIndex = 6;
            this.label9.Text = "用户名:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 15);
            this.label10.TabIndex = 5;
            this.label10.Text = "数据库:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 15);
            this.label12.TabIndex = 4;
            this.label12.Text = "服务器:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmdCancel);
            this.groupBox3.Controls.Add(this.cmdSet);
            this.groupBox3.Location = new System.Drawing.Point(323, 229);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(256, 82);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            // 
            // cmdCancel
            // 
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdCancel.Location = new System.Drawing.Point(155, 31);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(90, 33);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "取消(&C)";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdSet
            // 
            this.cmdSet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdSet.Location = new System.Drawing.Point(53, 31);
            this.cmdSet.Name = "cmdSet";
            this.cmdSet.Size = new System.Drawing.Size(91, 33);
            this.cmdSet.TabIndex = 1;
            this.cmdSet.Text = "确定(&O)";
            this.cmdSet.Click += new System.EventHandler(this.cmdSet_Click);
            // 
            // frmUdpSet
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 18);
            this.ClientSize = new System.Drawing.Size(593, 328);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "frmUdpSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.frmUdpSet_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmUdpSet_Load(object sender, System.EventArgs e)
		{
			txtPort.Text=GprsServer.LocaPort.ToString();
			txtTcpPort.Text =GprsServer.CdmaPort.ToString();
			txtCenterIP.Text =GprsServer.RemoteIp;
			txtCenterPort.Text =GprsServer.RemotePort.ToString();
			txtName.Text =GprsServer.sCompanyName;
			textBox1.Text=GprsServer.LocaIP.ToString();
			textBox2.Text=GprsServer.LocaIP2.ToString();
            textBox3.Text=GprsServer.LocaPort2.ToString();
			txtCount.Text =GprsServer.AgainCount.ToString();

			txtServer.Text=GprsServer.DbServer;
			txtDbName.Text=GprsServer.DbName;
			txtUserName.Text=GprsServer.UserId;
			txtPassWord.Text=GprsServer.PassWord;
			txtMobile.Text =GprsServer.strTerminal ;
		}

		private void cmdSet_Click(object sender, System.EventArgs e)
		{
			try
			{
				GprsServer.DbServer=txtServer.Text.Trim();
				GprsServer.WritePrivateProfileString("DbConn","DbServer",txtServer.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.DbName=txtDbName.Text.Trim();
				GprsServer.WritePrivateProfileString("DbConn","DbName",txtDbName.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.UserId=txtUserName.Text.Trim();
				GprsServer.WritePrivateProfileString("DbConn","UserId",txtUserName.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.PassWord=txtPassWord.Text.Trim();
				GprsServer.WritePrivateProfileString("DbConn","PassWord",txtPassWord.Text.Trim(),"SysIni\\SysIni.ini");
				
				GprsServer.CdmaPort=Convert.ToInt16(txtTcpPort.Text.Trim());
				GprsServer.WritePrivateProfileString("PortIni","CdmaPort",txtTcpPort.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.sCompanyName=txtName.Text.Trim();
				GprsServer.WritePrivateProfileString("Company","Name",txtName.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.RemoteIp=txtCenterIP.Text.Trim();
				GprsServer.WritePrivateProfileString("PortIni","TcpAddress",txtCenterIP.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.RemotePort=Convert.ToInt16(txtCenterPort.Text.Trim());
				GprsServer.WritePrivateProfileString("PortIni","TcpPort",txtCenterPort.Text.Trim(),"SysIni\\SysIni.ini");
				
				GprsServer.LocaPort=Convert.ToInt16(txtPort.Text.Trim());
				GprsServer.WritePrivateProfileString("PortIni","UdpPort",txtPort.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.LocaIP=textBox1.Text.Trim();
				GprsServer.WritePrivateProfileString("PortIni","BindIp1",textBox1.Text.Trim(),"SysIni\\SysIni.ini");
				
				GprsServer.LocaIP2=textBox2.Text.Trim();
                GprsServer.WritePrivateProfileString("PortIni","BindIp2",textBox2.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.LocaPort2=Convert.ToInt16(textBox3.Text.Trim());
				GprsServer.WritePrivateProfileString("PortIni","UdpPort2",textBox3.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.AgainCount=Convert.ToInt16(txtCount.Text.Trim());
                GprsServer.WritePrivateProfileString("Instruction","AgainCount",txtCount.Text.Trim(),"SysIni\\SysIni.ini");
				GprsServer.strTerminal =txtMobile.Text.Trim();
				GprsServer.WritePrivateProfileString("Show","Mobile",txtMobile.Text.Trim(),"SysIni\\SysIni.ini");
				this.Close();
			}
			catch
			{
				MessageBox.Show("设置错误，请填写正确的参数值。","提示",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
			}
		}

		private void label14_Click(object sender, System.EventArgs e)
		{
		
		}

		}
}
