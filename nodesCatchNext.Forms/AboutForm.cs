using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace nodesCatchNext.Forms;

public class AboutForm : Form
{
	private IContainer components;

	private Label label1;

	private Button button1;

	private Label label2;

	private LinkLabel linkLabel1;

	private LinkLabel linkLabel2;

	private GroupBox groupBox1;

	private LinkLabel linkLabel5;

	private Label label7;

	private Label label6;

	private GroupBox groupBox2;

	private Label label3;

	private Button button2;

	private LinkLabel linkLabel6;

	private Label label8;

	private Label label9;

	private Label label10;

	private LinkLabel linkLabel7;

	public AboutForm()
	{
		InitializeComponent();
		try
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
			if (File.Exists(text))
			{
				base.Icon = new Icon(text);
			}
		}
		catch
		{
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(linkLabel1.Text);
	}

	private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(linkLabel2.Text);
	}

	private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(linkLabel5.Text);
	}

	private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(linkLabel7.Text);
	}

	private void button2_Click(object sender, EventArgs e)
	{
		Process.Start("https://github.com/AHackerX/nodesCatchNext");
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.label1 = new System.Windows.Forms.Label();
		this.button1 = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.linkLabel1 = new System.Windows.Forms.LinkLabel();
		this.linkLabel2 = new System.Windows.Forms.LinkLabel();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.label6 = new System.Windows.Forms.Label();
		this.linkLabel5 = new System.Windows.Forms.LinkLabel();
		this.label7 = new System.Windows.Forms.Label();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.label3 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.linkLabel7 = new System.Windows.Forms.LinkLabel();
		this.button2 = new System.Windows.Forms.Button();
		this.linkLabel6 = new System.Windows.Forms.LinkLabel();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.label1.Location = new System.Drawing.Point(45, 35);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(120, 31);
		this.label1.TabIndex = 0;
		this.label1.Text = "v2rayN：";
		this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
		this.button1.Font = new System.Drawing.Font("宋体", 11f);
		this.button1.Location = new System.Drawing.Point(412, 675);
		this.button1.Margin = new System.Windows.Forms.Padding(4);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(145, 36);
		this.button1.TabIndex = 1;
		this.button1.Text = "关闭";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.label2.Location = new System.Drawing.Point(31, 94);
		this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(134, 31);
		this.label2.TabIndex = 2;
		this.label2.Text = "Mihomo：";
		this.label2.Click += new System.EventHandler(label2_Click);
		this.linkLabel1.AutoSize = true;
		this.linkLabel1.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.linkLabel1.Location = new System.Drawing.Point(119, 66);
		this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel1.Name = "linkLabel1";
		this.linkLabel1.Size = new System.Drawing.Size(415, 28);
		this.linkLabel1.TabIndex = 4;
		this.linkLabel1.TabStop = true;
		this.linkLabel1.Text = "https://github.com/2dust/v2rayN";
		this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
		this.linkLabel2.AutoSize = true;
		this.linkLabel2.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.linkLabel2.Location = new System.Drawing.Point(119, 125);
		this.linkLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel2.Name = "linkLabel2";
		this.linkLabel2.Size = new System.Drawing.Size(467, 28);
		this.linkLabel2.TabIndex = 5;
		this.linkLabel2.TabStop = true;
		this.linkLabel2.Text = "https://github.com/MetaCubeX/mihomo";
		this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
		this.groupBox1.Controls.Add(this.label6);
		this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.groupBox1.Location = new System.Drawing.Point(9, 364);
		this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox1.Size = new System.Drawing.Size(764, 136);
		this.groupBox1.TabIndex = 10;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "免责声明";
		this.label6.Font = new System.Drawing.Font("微软雅黑", 13.25f);
		this.label6.Location = new System.Drawing.Point(32, 31);
		this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(697, 96);
		this.label6.TabIndex = 19;
		this.label6.Text = "        本软件仅供非中国大陆地区用户学习交流使用，禁止在中国大陆传播使用，请务必遵守所在国法律法规，任何使用后果与软件作者无关！";
		this.linkLabel5.AutoSize = true;
		this.linkLabel5.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.linkLabel5.Location = new System.Drawing.Point(119, 184);
		this.linkLabel5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel5.Name = "linkLabel5";
		this.linkLabel5.Size = new System.Drawing.Size(545, 28);
		this.linkLabel5.TabIndex = 12;
		this.linkLabel5.TabStop = true;
		this.linkLabel5.Text = "https://github.com/asdlokj1qpi233/subconverter";
		this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel5_LinkClicked);
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.label7.Location = new System.Drawing.Point(4, 153);
		this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(192, 31);
		this.label7.TabIndex = 11;
		this.label7.Text = "Subconverter：";
		this.groupBox2.Controls.Add(this.linkLabel1);
		this.groupBox2.Controls.Add(this.linkLabel2);
		this.groupBox2.Controls.Add(this.label3);
		this.groupBox2.Controls.Add(this.label1);
		this.groupBox2.Controls.Add(this.linkLabel5);
		this.groupBox2.Controls.Add(this.label2);
		this.groupBox2.Controls.Add(this.label7);
		this.groupBox2.Controls.Add(this.label10);
		this.groupBox2.Controls.Add(this.linkLabel7);
		this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.groupBox2.Location = new System.Drawing.Point(9, 15);
		this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox2.Size = new System.Drawing.Size(764, 341);
		this.groupBox2.TabIndex = 11;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "基于";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.label3.Location = new System.Drawing.Point(15, 280);
		this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(687, 62);
		this.label3.TabIndex = 13;
		this.label3.Text = "支持测速协议：Shadowsocks、ShadowsocksR、Vmess\r\n                        Vless、Trojan、Socks5、HTTP(S)、Hysteria2";
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.label10.Location = new System.Drawing.Point(4, 212);
		this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(230, 31);
		this.label10.TabIndex = 14;
		this.label10.Text = "nodesCatch V2.0：";
		this.linkLabel7.AutoSize = true;
		this.linkLabel7.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.linkLabel7.Location = new System.Drawing.Point(119, 243);
		this.linkLabel7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel7.Name = "linkLabel7";
		this.linkLabel7.Size = new System.Drawing.Size(623, 28);
		this.linkLabel7.TabIndex = 15;
		this.linkLabel7.TabStop = true;
		this.linkLabel7.Text = "https://bulianglin.com/archives/nodescatch.html";
		this.linkLabel7.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel7_LinkClicked);
		this.button2.Font = new System.Drawing.Font("宋体", 11f);
		this.button2.Location = new System.Drawing.Point(211, 675);
		this.button2.Margin = new System.Windows.Forms.Padding(4);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(129, 36);
		this.button2.TabIndex = 24;
		this.button2.Text = "项目地址";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.linkLabel6.Location = new System.Drawing.Point(0, 0);
		this.linkLabel6.Name = "linkLabel6";
		this.linkLabel6.Size = new System.Drawing.Size(100, 23);
		this.linkLabel6.TabIndex = 0;
		this.label8.Location = new System.Drawing.Point(0, 0);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(100, 23);
		this.label8.TabIndex = 0;
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("微软雅黑", 10f);
		this.label9.Location = new System.Drawing.Point(131, 257);
		this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(564, 23);
		this.label9.TabIndex = 16;
		this.label9.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(776, 729);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.button1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(794, 776);
		this.MinimumSize = new System.Drawing.Size(794, 776);
		base.Name = "AboutForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "关于";
		this.groupBox1.ResumeLayout(false);
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		base.ResumeLayout(false);
	}

	private void label2_Click(object sender, EventArgs e)
	{
	}
}
