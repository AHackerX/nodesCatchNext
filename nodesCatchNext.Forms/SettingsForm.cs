using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Forms;

public class SettingsForm : Form
{
	private IContainer components;

	private GroupBox groupBox1;

	private CheckBox cbTunWarning;

	private CheckBox cbAutoDedup;

	private CheckBox cbAutoSaveAfterTest;

	private CheckBox cbExitConfirm;

	private CheckBox cbSaveConfigOnExit;

	private CheckBox cbSaveWindowPosition;

	private CheckBox cbAllowEditServer;

	private CheckBox cbAutoRemoveNoResult;

	private CheckBox cbRecordTestTime;

	private Label labelSubUpdateMode;

	private ComboBox cmbSubUpdateMode;

	private GroupBox groupBox2;

	private Label labelDelayTestUrl;

	private Label labelSpeedTestUrl;

	private TextBox tbDelayTestUrl;

	private TextBox tbSpeedTestUrl;

	private Button btnOK;

	private Button btnCancel;

	private Label lblInfo;

	private ToolTip toolTipSettings;

	private Config config;

	public SettingsForm(Config cfg)
	{
		config = cfg;
		InitializeComponent();
		LoadSettings();
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

	private void LoadSettings()
	{
		cbTunWarning.Checked = config.tunWarningEnabled;
		cbAutoDedup.Checked = config.autoDedupEnabled;
		cbAutoSaveAfterTest.Checked = config.autoSaveAfterTest;
		cbExitConfirm.Checked = config.exitConfirmEnabled;
		cbSaveConfigOnExit.Checked = config.saveConfigOnExit;
		cbSaveWindowPosition.Checked = config.saveWindowPosition;
		cbAllowEditServer.Checked = config.allowEditServer;
		cbAutoRemoveNoResult.Checked = config.autoRemoveNoResultServer;
		cbRecordTestTime.Checked = config.recordTestTime;
		tbDelayTestUrl.Text = config.speedPingTestUrl ?? "https://www.cloudflare.com/cdn-cgi/trace";
		tbSpeedTestUrl.Text = config.speedTestUrl ?? "https://cdn.kernel.org/pub/linux/kernel/v6.x/linux-6.2.15.tar.xz";
		cmbSubUpdateMode.SelectedIndex = config.subUpdateMode;
	}

	private void SaveSettings()
	{
		config.tunWarningEnabled = cbTunWarning.Checked;
		config.autoDedupEnabled = cbAutoDedup.Checked;
		config.autoSaveAfterTest = cbAutoSaveAfterTest.Checked;
		config.exitConfirmEnabled = cbExitConfirm.Checked;
		config.saveConfigOnExit = cbSaveConfigOnExit.Checked;
		config.saveWindowPosition = cbSaveWindowPosition.Checked;
		config.allowEditServer = cbAllowEditServer.Checked;
		config.autoRemoveNoResultServer = cbAutoRemoveNoResult.Checked;
		config.recordTestTime = cbRecordTestTime.Checked;
		config.speedPingTestUrl = tbDelayTestUrl.Text.Trim();
		config.speedTestUrl = tbSpeedTestUrl.Text.Trim();
		config.subUpdateMode = cmbSubUpdateMode.SelectedIndex;
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		SaveSettings();
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
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
		this.components = new System.ComponentModel.Container();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.cbTunWarning = new System.Windows.Forms.CheckBox();
		this.cbAutoDedup = new System.Windows.Forms.CheckBox();
		this.cbAutoSaveAfterTest = new System.Windows.Forms.CheckBox();
		this.cbExitConfirm = new System.Windows.Forms.CheckBox();
		this.cbSaveConfigOnExit = new System.Windows.Forms.CheckBox();
		this.cbSaveWindowPosition = new System.Windows.Forms.CheckBox();
		this.cbAllowEditServer = new System.Windows.Forms.CheckBox();
		this.cbAutoRemoveNoResult = new System.Windows.Forms.CheckBox();
		this.cbRecordTestTime = new System.Windows.Forms.CheckBox();
		this.labelSubUpdateMode = new System.Windows.Forms.Label();
		this.cmbSubUpdateMode = new System.Windows.Forms.ComboBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.labelDelayTestUrl = new System.Windows.Forms.Label();
		this.labelSpeedTestUrl = new System.Windows.Forms.Label();
		this.tbDelayTestUrl = new System.Windows.Forms.TextBox();
		this.tbSpeedTestUrl = new System.Windows.Forms.TextBox();
		this.btnOK = new System.Windows.Forms.Button();
		this.btnCancel = new System.Windows.Forms.Button();
		this.lblInfo = new System.Windows.Forms.Label();
		this.toolTipSettings = new System.Windows.Forms.ToolTip(this.components);
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.cbTunWarning);
		this.groupBox1.Controls.Add(this.cbAutoDedup);
		this.groupBox1.Controls.Add(this.cbAutoSaveAfterTest);
		this.groupBox1.Controls.Add(this.cbExitConfirm);
		this.groupBox1.Controls.Add(this.cbSaveConfigOnExit);
		this.groupBox1.Controls.Add(this.cbSaveWindowPosition);
		this.groupBox1.Controls.Add(this.cbAllowEditServer);
		this.groupBox1.Controls.Add(this.cbAutoRemoveNoResult);
		this.groupBox1.Controls.Add(this.cbRecordTestTime);
		this.groupBox1.Controls.Add(this.labelSubUpdateMode);
		this.groupBox1.Controls.Add(this.cmbSubUpdateMode);
		this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10f);
		this.groupBox1.Location = new System.Drawing.Point(12, 12);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(752, 330);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "常规设置";
		this.cbTunWarning.AutoSize = true;
		this.cbTunWarning.Location = new System.Drawing.Point(20, 35);
		this.cbTunWarning.Name = "cbTunWarning";
		this.cbTunWarning.Size = new System.Drawing.Size(239, 27);
		this.cbTunWarning.TabIndex = 0;
		this.cbTunWarning.Text = "测速前检测TUN模式并提示";
		this.toolTipSettings.SetToolTip(this.cbTunWarning, "开启VPN的TUN模式可能影响测速结果\r\n启用此选项会在测速前提示关闭TUN模式");
		this.cbTunWarning.UseVisualStyleBackColor = true;
		this.cbAutoDedup.AutoSize = true;
		this.cbAutoDedup.Location = new System.Drawing.Point(20, 70);
		this.cbAutoDedup.Name = "cbAutoDedup";
		this.cbAutoDedup.Size = new System.Drawing.Size(219, 27);
		this.cbAutoDedup.TabIndex = 1;
		this.cbAutoDedup.Text = "一键测速前自动去重节点";
		this.toolTipSettings.SetToolTip(this.cbAutoDedup, "自动移除地址、端口相同的重复节点\r\n避免对同一节点进行重复测速");
		this.cbAutoDedup.UseVisualStyleBackColor = true;
		this.cbAutoSaveAfterTest.AutoSize = true;
		this.cbAutoSaveAfterTest.Location = new System.Drawing.Point(20, 105);
		this.cbAutoSaveAfterTest.Name = "cbAutoSaveAfterTest";
		this.cbAutoSaveAfterTest.Size = new System.Drawing.Size(219, 27);
		this.cbAutoSaveAfterTest.TabIndex = 2;
		this.cbAutoSaveAfterTest.Text = "测速结束后自动保存配置";
		this.toolTipSettings.SetToolTip(this.cbAutoSaveAfterTest, "测速完成后自动保存节点列表和测速结果到配置文件");
		this.cbAutoSaveAfterTest.UseVisualStyleBackColor = true;
		this.cbExitConfirm.AutoSize = true;
		this.cbExitConfirm.Location = new System.Drawing.Point(20, 140);
		this.cbExitConfirm.Name = "cbExitConfirm";
		this.cbExitConfirm.Size = new System.Drawing.Size(168, 27);
		this.cbExitConfirm.TabIndex = 3;
		this.cbExitConfirm.Text = "允许直接关闭程序";
		this.toolTipSettings.SetToolTip(this.cbExitConfirm, "启用后点击关闭按钮直接退出程序\r\n禁用时会弹出确认对话框");
		this.cbExitConfirm.UseVisualStyleBackColor = true;
		this.cbSaveConfigOnExit.AutoSize = true;
		this.cbSaveConfigOnExit.Location = new System.Drawing.Point(220, 140);
		this.cbSaveConfigOnExit.Name = "cbSaveConfigOnExit";
		this.cbSaveConfigOnExit.Size = new System.Drawing.Size(110, 27);
		this.cbSaveConfigOnExit.TabIndex = 10;
		this.cbSaveConfigOnExit.Text = "并保存配置";
		this.toolTipSettings.SetToolTip(this.cbSaveConfigOnExit, "关闭程序时自动保存完整配置\r\n包括节点列表、测速结果等所有设置");
		this.cbSaveConfigOnExit.UseVisualStyleBackColor = true;
		this.cbSaveWindowPosition.AutoSize = true;
		this.cbSaveWindowPosition.Location = new System.Drawing.Point(20, 175);
		this.cbSaveWindowPosition.Name = "cbSaveWindowPosition";
		this.cbSaveWindowPosition.Size = new System.Drawing.Size(134, 27);
		this.cbSaveWindowPosition.TabIndex = 4;
		this.cbSaveWindowPosition.Text = "记住窗口位置";
		this.toolTipSettings.SetToolTip(this.cbSaveWindowPosition, "下次打开程序时恢复上次的窗口位置和大小");
		this.cbSaveWindowPosition.UseVisualStyleBackColor = true;
		this.cbAllowEditServer.AutoSize = true;
		this.cbAllowEditServer.Location = new System.Drawing.Point(20, 210);
		this.cbAllowEditServer.Name = "cbAllowEditServer";
		this.cbAllowEditServer.Size = new System.Drawing.Size(168, 27);
		this.cbAllowEditServer.TabIndex = 5;
		this.cbAllowEditServer.Text = "允许编辑节点信息";
		this.toolTipSettings.SetToolTip(this.cbAllowEditServer, "启用后可以双击节点打开编辑窗口\r\n修改节点的地址、端口等信息");
		this.cbAllowEditServer.UseVisualStyleBackColor = true;
		this.cbAutoRemoveNoResult.AutoSize = true;
		this.cbAutoRemoveNoResult.Location = new System.Drawing.Point(20, 245);
		this.cbAutoRemoveNoResult.Name = "cbAutoRemoveNoResult";
		this.cbAutoRemoveNoResult.Size = new System.Drawing.Size(270, 27);
		this.cbAutoRemoveNoResult.TabIndex = 6;
		this.cbAutoRemoveNoResult.Text = "测速完成后自动移除无结果节点";
		this.toolTipSettings.SetToolTip(this.cbAutoRemoveNoResult, "自动删除测速失败或无响应的节点\r\n包括连接超时、无法建立连接的节点");
		this.cbAutoRemoveNoResult.UseVisualStyleBackColor = true;
		this.cbRecordTestTime.AutoSize = true;
		this.cbRecordTestTime.Location = new System.Drawing.Point(20, 280);
		this.cbRecordTestTime.Name = "cbRecordTestTime";
		this.cbRecordTestTime.Size = new System.Drawing.Size(168, 27);
		this.cbRecordTestTime.TabIndex = 7;
		this.cbRecordTestTime.Text = "记录节点测速时间";
		this.toolTipSettings.SetToolTip(this.cbRecordTestTime, "在节点列表中显示\"最后测速\"列\r\n记录每个节点最后一次测速完成的时间");
		this.cbRecordTestTime.UseVisualStyleBackColor = true;
		this.labelSubUpdateMode.AutoSize = true;
		this.labelSubUpdateMode.Location = new System.Drawing.Point(300, 35);
		this.labelSubUpdateMode.Name = "labelSubUpdateMode";
		this.labelSubUpdateMode.Size = new System.Drawing.Size(129, 23);
		this.labelSubUpdateMode.TabIndex = 8;
		this.labelSubUpdateMode.Text = "订阅更新模式：";
		this.cmbSubUpdateMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbSubUpdateMode.FormattingEnabled = true;
		this.cmbSubUpdateMode.Items.AddRange(new object[2] { "覆盖更新", "添加更新" });
		this.cmbSubUpdateMode.Location = new System.Drawing.Point(444, 32);
		this.cmbSubUpdateMode.Name = "cmbSubUpdateMode";
		this.cmbSubUpdateMode.Size = new System.Drawing.Size(120, 31);
		this.cmbSubUpdateMode.TabIndex = 9;
		this.toolTipSettings.SetToolTip(this.cmbSubUpdateMode, "覆盖更新：先删除该订阅的旧节点，再添加新节点\r\n添加更新：直接添加所有新节点，不删除旧节点");
		this.groupBox2.Controls.Add(this.labelDelayTestUrl);
		this.groupBox2.Controls.Add(this.labelSpeedTestUrl);
		this.groupBox2.Controls.Add(this.tbDelayTestUrl);
		this.groupBox2.Controls.Add(this.tbSpeedTestUrl);
		this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10f);
		this.groupBox2.Location = new System.Drawing.Point(12, 355);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(752, 100);
		this.groupBox2.TabIndex = 4;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "测速URL设置";
		this.labelDelayTestUrl.AutoSize = true;
		this.labelDelayTestUrl.Location = new System.Drawing.Point(15, 30);
		this.labelDelayTestUrl.Name = "labelDelayTestUrl";
		this.labelDelayTestUrl.Size = new System.Drawing.Size(128, 23);
		this.labelDelayTestUrl.TabIndex = 0;
		this.labelDelayTestUrl.Text = "延迟测速URL：";
		this.labelSpeedTestUrl.AutoSize = true;
		this.labelSpeedTestUrl.Location = new System.Drawing.Point(15, 65);
		this.labelSpeedTestUrl.Name = "labelSpeedTestUrl";
		this.labelSpeedTestUrl.Size = new System.Drawing.Size(128, 23);
		this.labelSpeedTestUrl.TabIndex = 1;
		this.labelSpeedTestUrl.Text = "下载测速URL：";
		this.tbDelayTestUrl.Location = new System.Drawing.Point(156, 24);
		this.tbDelayTestUrl.Name = "tbDelayTestUrl";
		this.tbDelayTestUrl.Size = new System.Drawing.Size(590, 29);
		this.tbDelayTestUrl.TabIndex = 2;
		this.toolTipSettings.SetToolTip(this.tbDelayTestUrl, "用于测试 TLS RTT 和 HTTPS 延迟的目标URL\r\n建议使用响应快、稳定的网站");
		this.tbSpeedTestUrl.Location = new System.Drawing.Point(156, 65);
		this.tbSpeedTestUrl.Name = "tbSpeedTestUrl";
		this.tbSpeedTestUrl.Size = new System.Drawing.Size(590, 29);
		this.tbSpeedTestUrl.TabIndex = 3;
		this.toolTipSettings.SetToolTip(this.tbSpeedTestUrl, "用于下载测速的文件URL\r\n建议使用大文件以获得准确的速度测试结果");
		this.btnOK.Location = new System.Drawing.Point(298, 470);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(80, 32);
		this.btnOK.TabIndex = 1;
		this.btnOK.Text = "确定";
		this.btnOK.UseVisualStyleBackColor = true;
		this.btnOK.Click += new System.EventHandler(btnOK_Click);
		this.btnCancel.Location = new System.Drawing.Point(398, 470);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(80, 32);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "取消";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		this.lblInfo.Location = new System.Drawing.Point(0, 0);
		this.lblInfo.Name = "lblInfo";
		this.lblInfo.Size = new System.Drawing.Size(100, 23);
		this.lblInfo.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(776, 515);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.btnCancel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SettingsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "设置";
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		base.ResumeLayout(false);
	}
}
