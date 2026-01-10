using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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

	// 列显示设置控件
	private GroupBox groupBox3;
	private CheckedListBox clbColumns;
	private Button btnMoveUp;
	private Button btnMoveDown;

	private Config config;

	// 列信息：key, 显示名称
	private readonly List<(string key, string displayName)> allColumns = new List<(string, string)>
	{
		("def", "No"),
		("configType", "类型"),
		("remarks", "别名"),
		("address", "服务器地址"),
		("port", "端口"),
		("security", "加密方式"),
		("network", "传输协议"),
		("tls", "TLS"),
		("subRemarks", "订阅"),
		("httpsDelay", "HTTPS延迟"),
		("testResult", "平均速度"),
		("MaxSpeed", "峰值速度")
	};

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
		
		LoadColumnSettings();
	}

	private void LoadColumnSettings()
	{
		clbColumns.Items.Clear();
		
		var colVisible = config.uiItem?.mainLvColVisible;
		var colOrder = config.uiItem?.mainLvColOrder;
		
		// 清理旧的 tlsRtt 配置（已移除的功能）
		if (colVisible != null && colVisible.ContainsKey("tlsRtt"))
		{
			colVisible.Remove("tlsRtt");
		}
		if (colOrder != null)
		{
			colOrder.RemoveAll(k => k == "tlsRtt");
		}
		
		// 如果有保存的顺序，按顺序加载；否则使用默认顺序
		List<(string key, string displayName)> orderedColumns;
		if (colOrder != null && colOrder.Count > 0)
		{
			orderedColumns = new List<(string, string)>();
			foreach (var key in colOrder)
			{
				var col = allColumns.FirstOrDefault(c => c.key == key);
				if (col.key != null)
				{
					orderedColumns.Add(col);
				}
			}
			// 添加可能遗漏的列（新版本添加的列）
			foreach (var col in allColumns)
			{
				if (!orderedColumns.Any(c => c.key == col.key))
				{
					orderedColumns.Add(col);
				}
			}
		}
		else
		{
			orderedColumns = allColumns.ToList();
		}
		
		foreach (var col in orderedColumns)
		{
			bool isChecked = colVisible == null || !colVisible.ContainsKey(col.key) || colVisible[col.key];
			clbColumns.Items.Add(new ColumnItem(col.key, col.displayName), isChecked);
		}
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
		
		SaveColumnSettings();
	}

	private void SaveColumnSettings()
	{
		if (config.uiItem.mainLvColVisible == null)
		{
			config.uiItem.mainLvColVisible = new Dictionary<string, bool>();
		}
		if (config.uiItem.mainLvColOrder == null)
		{
			config.uiItem.mainLvColOrder = new List<string>();
		}
		
		config.uiItem.mainLvColVisible.Clear();
		config.uiItem.mainLvColOrder.Clear();
		
		for (int i = 0; i < clbColumns.Items.Count; i++)
		{
			var item = (ColumnItem)clbColumns.Items[i];
			config.uiItem.mainLvColVisible[item.Key] = clbColumns.GetItemChecked(i);
			config.uiItem.mainLvColOrder.Add(item.Key);
		}
	}

	private void btnMoveUp_Click(object sender, EventArgs e)
	{
		int index = clbColumns.SelectedIndex;
		if (index > 0)
		{
			bool isChecked = clbColumns.GetItemChecked(index);
			var item = clbColumns.Items[index];
			clbColumns.Items.RemoveAt(index);
			clbColumns.Items.Insert(index - 1, item);
			clbColumns.SetItemChecked(index - 1, isChecked);
			clbColumns.SelectedIndex = index - 1;
		}
	}

	private void btnMoveDown_Click(object sender, EventArgs e)
	{
		int index = clbColumns.SelectedIndex;
		if (index >= 0 && index < clbColumns.Items.Count - 1)
		{
			bool isChecked = clbColumns.GetItemChecked(index);
			var item = clbColumns.Items[index];
			clbColumns.Items.RemoveAt(index);
			clbColumns.Items.Insert(index + 1, item);
			clbColumns.SetItemChecked(index + 1, isChecked);
			clbColumns.SelectedIndex = index + 1;
		}
	}

	private void clbColumns_MouseDown(object sender, MouseEventArgs e)
	{
		// 获取点击位置对应的项索引
		int index = clbColumns.IndexFromPoint(e.Location);
		if (index < 0 || index >= clbColumns.Items.Count)
			return;
		
		// 计算复选框区域（大约前16-20像素是复选框区域）
		const int checkBoxWidth = 16;
		if (e.X <= checkBoxWidth)
		{
			// 点击了复选框区域，切换勾选状态
			clbColumns.SetItemChecked(index, !clbColumns.GetItemChecked(index));
		}
		// 点击文字区域只选中项目，不切换勾选状态（由默认行为处理选中）
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.clbColumns = new System.Windows.Forms.CheckedListBox();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.toolTipSettings = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
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
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(532, 350);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "常规设置";
            // 
            // cbTunWarning
            // 
            this.cbTunWarning.AutoSize = true;
            this.cbTunWarning.Location = new System.Drawing.Point(20, 35);
            this.cbTunWarning.Name = "cbTunWarning";
            this.cbTunWarning.Size = new System.Drawing.Size(239, 27);
            this.cbTunWarning.TabIndex = 0;
            this.cbTunWarning.Text = "测速前检测TUN模式并提示";
            this.toolTipSettings.SetToolTip(this.cbTunWarning, "开启VPN的TUN模式可能影响测速结果\r\n启用此选项会在测速前提示关闭TUN模式");
            this.cbTunWarning.UseVisualStyleBackColor = true;
            // 
            // cbAutoDedup
            // 
            this.cbAutoDedup.AutoSize = true;
            this.cbAutoDedup.Location = new System.Drawing.Point(20, 70);
            this.cbAutoDedup.Name = "cbAutoDedup";
            this.cbAutoDedup.Size = new System.Drawing.Size(219, 27);
            this.cbAutoDedup.TabIndex = 1;
            this.cbAutoDedup.Text = "一键测速前自动去重节点";
            this.toolTipSettings.SetToolTip(this.cbAutoDedup, "自动移除地址、端口相同的重复节点\r\n避免对同一节点进行重复测速");
            this.cbAutoDedup.UseVisualStyleBackColor = true;
            // 
            // cbAutoSaveAfterTest
            // 
            this.cbAutoSaveAfterTest.AutoSize = true;
            this.cbAutoSaveAfterTest.Location = new System.Drawing.Point(20, 105);
            this.cbAutoSaveAfterTest.Name = "cbAutoSaveAfterTest";
            this.cbAutoSaveAfterTest.Size = new System.Drawing.Size(219, 27);
            this.cbAutoSaveAfterTest.TabIndex = 2;
            this.cbAutoSaveAfterTest.Text = "测速结束后自动保存配置";
            this.toolTipSettings.SetToolTip(this.cbAutoSaveAfterTest, "测速完成后自动保存节点列表和测速结果到配置文件");
            this.cbAutoSaveAfterTest.UseVisualStyleBackColor = true;
            // 
            // cbExitConfirm
            // 
            this.cbExitConfirm.AutoSize = true;
            this.cbExitConfirm.Location = new System.Drawing.Point(20, 140);
            this.cbExitConfirm.Name = "cbExitConfirm";
            this.cbExitConfirm.Size = new System.Drawing.Size(168, 27);
            this.cbExitConfirm.TabIndex = 3;
            this.cbExitConfirm.Text = "允许直接关闭程序";
            this.toolTipSettings.SetToolTip(this.cbExitConfirm, "启用后点击关闭按钮直接退出程序\r\n禁用时会弹出确认对话框");
            this.cbExitConfirm.UseVisualStyleBackColor = true;
            // 
            // cbSaveConfigOnExit
            // 
            this.cbSaveConfigOnExit.AutoSize = true;
            this.cbSaveConfigOnExit.Location = new System.Drawing.Point(220, 140);
            this.cbSaveConfigOnExit.Name = "cbSaveConfigOnExit";
            this.cbSaveConfigOnExit.Size = new System.Drawing.Size(117, 27);
            this.cbSaveConfigOnExit.TabIndex = 10;
            this.cbSaveConfigOnExit.Text = "并保存配置";
            this.toolTipSettings.SetToolTip(this.cbSaveConfigOnExit, "关闭程序时自动保存完整配置\r\n包括节点列表、测速结果等所有设置");
            this.cbSaveConfigOnExit.UseVisualStyleBackColor = true;
            // 
            // cbSaveWindowPosition
            // 
            this.cbSaveWindowPosition.AutoSize = true;
            this.cbSaveWindowPosition.Location = new System.Drawing.Point(20, 175);
            this.cbSaveWindowPosition.Name = "cbSaveWindowPosition";
            this.cbSaveWindowPosition.Size = new System.Drawing.Size(134, 27);
            this.cbSaveWindowPosition.TabIndex = 4;
            this.cbSaveWindowPosition.Text = "记住窗口位置";
            this.toolTipSettings.SetToolTip(this.cbSaveWindowPosition, "下次打开程序时恢复上次的窗口位置和大小");
            this.cbSaveWindowPosition.UseVisualStyleBackColor = true;
            // 
            // cbAllowEditServer
            // 
            this.cbAllowEditServer.AutoSize = true;
            this.cbAllowEditServer.Location = new System.Drawing.Point(20, 210);
            this.cbAllowEditServer.Name = "cbAllowEditServer";
            this.cbAllowEditServer.Size = new System.Drawing.Size(168, 27);
            this.cbAllowEditServer.TabIndex = 5;
            this.cbAllowEditServer.Text = "允许编辑节点信息";
            this.toolTipSettings.SetToolTip(this.cbAllowEditServer, "启用后可以双击节点打开编辑窗口\r\n修改节点的地址、端口等信息");
            this.cbAllowEditServer.UseVisualStyleBackColor = true;
            // 
            // cbAutoRemoveNoResult
            // 
            this.cbAutoRemoveNoResult.AutoSize = true;
            this.cbAutoRemoveNoResult.Location = new System.Drawing.Point(20, 245);
            this.cbAutoRemoveNoResult.Name = "cbAutoRemoveNoResult";
            this.cbAutoRemoveNoResult.Size = new System.Drawing.Size(270, 27);
            this.cbAutoRemoveNoResult.TabIndex = 6;
            this.cbAutoRemoveNoResult.Text = "测速完成后自动移除无结果节点";
            this.toolTipSettings.SetToolTip(this.cbAutoRemoveNoResult, "自动删除测速失败或无响应的节点\r\n包括连接超时、无法建立连接的节点");
            this.cbAutoRemoveNoResult.UseVisualStyleBackColor = true;
            // 
            // cbRecordTestTime
            // 
            this.cbRecordTestTime.AutoSize = true;
            this.cbRecordTestTime.Location = new System.Drawing.Point(20, 280);
            this.cbRecordTestTime.Name = "cbRecordTestTime";
            this.cbRecordTestTime.Size = new System.Drawing.Size(168, 27);
            this.cbRecordTestTime.TabIndex = 7;
            this.cbRecordTestTime.Text = "记录节点测速时间";
            this.toolTipSettings.SetToolTip(this.cbRecordTestTime, "在节点列表中显示\"最后测速\"列\r\n记录每个节点最后一次测速完成的时间");
            this.cbRecordTestTime.UseVisualStyleBackColor = true;
            // 
            // labelSubUpdateMode
            // 
            this.labelSubUpdateMode.AutoSize = true;
            this.labelSubUpdateMode.Location = new System.Drawing.Point(14, 314);
            this.labelSubUpdateMode.Name = "labelSubUpdateMode";
            this.labelSubUpdateMode.Size = new System.Drawing.Size(129, 23);
            this.labelSubUpdateMode.TabIndex = 8;
            this.labelSubUpdateMode.Text = "订阅更新模式：";
            // 
            // cmbSubUpdateMode
            // 
            this.cmbSubUpdateMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubUpdateMode.FormattingEnabled = true;
            this.cmbSubUpdateMode.Items.AddRange(new object[] {
            "覆盖更新",
            "添加更新"});
            this.cmbSubUpdateMode.Location = new System.Drawing.Point(156, 311);
            this.cmbSubUpdateMode.Name = "cmbSubUpdateMode";
            this.cmbSubUpdateMode.Size = new System.Drawing.Size(110, 31);
            this.cmbSubUpdateMode.TabIndex = 9;
            this.toolTipSettings.SetToolTip(this.cmbSubUpdateMode, "覆盖更新：先删除该订阅的旧节点，再添加新节点\r\n添加更新：直接添加所有新节点，不删除旧节点");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelDelayTestUrl);
            this.groupBox2.Controls.Add(this.labelSpeedTestUrl);
            this.groupBox2.Controls.Add(this.tbDelayTestUrl);
            this.groupBox2.Controls.Add(this.tbSpeedTestUrl);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.groupBox2.Location = new System.Drawing.Point(12, 364);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(768, 100);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "测速URL设置";
            // 
            // labelDelayTestUrl
            // 
            this.labelDelayTestUrl.AutoSize = true;
            this.labelDelayTestUrl.Location = new System.Drawing.Point(15, 30);
            this.labelDelayTestUrl.Name = "labelDelayTestUrl";
            this.labelDelayTestUrl.Size = new System.Drawing.Size(128, 23);
            this.labelDelayTestUrl.TabIndex = 0;
            this.labelDelayTestUrl.Text = "延迟测速URL：";
            // 
            // labelSpeedTestUrl
            // 
            this.labelSpeedTestUrl.AutoSize = true;
            this.labelSpeedTestUrl.Location = new System.Drawing.Point(15, 65);
            this.labelSpeedTestUrl.Name = "labelSpeedTestUrl";
            this.labelSpeedTestUrl.Size = new System.Drawing.Size(128, 23);
            this.labelSpeedTestUrl.TabIndex = 1;
            this.labelSpeedTestUrl.Text = "下载测速URL：";
            // 
            // tbDelayTestUrl
            // 
            this.tbDelayTestUrl.Location = new System.Drawing.Point(156, 24);
            this.tbDelayTestUrl.Name = "tbDelayTestUrl";
            this.tbDelayTestUrl.Size = new System.Drawing.Size(600, 29);
            this.tbDelayTestUrl.TabIndex = 2;
            this.toolTipSettings.SetToolTip(this.tbDelayTestUrl, "用于测试 HTTPS 延迟的目标URL\r\n建议使用响应快、稳定的网站");
            // 
            // tbSpeedTestUrl
            // 
            this.tbSpeedTestUrl.Location = new System.Drawing.Point(156, 65);
            this.tbSpeedTestUrl.Name = "tbSpeedTestUrl";
            this.tbSpeedTestUrl.Size = new System.Drawing.Size(600, 29);
            this.tbSpeedTestUrl.TabIndex = 3;
            this.toolTipSettings.SetToolTip(this.tbSpeedTestUrl, "用于下载测速的文件URL\r\n建议使用大文件以获得准确的速度测试结果");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.clbColumns);
            this.groupBox3.Controls.Add(this.btnMoveUp);
            this.groupBox3.Controls.Add(this.btnMoveDown);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.groupBox3.Location = new System.Drawing.Point(550, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 350);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "列显示设置";
            // 
            // clbColumns
            // 
            this.clbColumns.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clbColumns.FormattingEnabled = true;
            this.clbColumns.Location = new System.Drawing.Point(10, 25);
            this.clbColumns.Name = "clbColumns";
            this.clbColumns.Size = new System.Drawing.Size(137, 312);
            this.clbColumns.TabIndex = 0;
            this.toolTipSettings.SetToolTip(this.clbColumns, "点击复选框切换显示状态\r\n点击文字选中后用右侧按钮调整顺序");
            this.clbColumns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.clbColumns_MouseDown);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(153, 135);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(71, 32);
            this.btnMoveUp.TabIndex = 1;
            this.btnMoveUp.Text = "上移";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(153, 175);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(71, 32);
            this.btnMoveDown.TabIndex = 2;
            this.btnMoveDown.Text = "下移";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(305, 470);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 32);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(405, 470);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(0, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(100, 23);
            this.lblInfo.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 515);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

	}

	// 列项辅助类
	private class ColumnItem
	{
		public string Key { get; }
		public string DisplayName { get; }

		public ColumnItem(string key, string displayName)
		{
			Key = key;
			DisplayName = displayName;
		}

		public override string ToString() => DisplayName;
	}
}
