using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using nodesCatchNext.Base;
using nodesCatchNext.Handler;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Forms;

public class MainForm : Form
{
	private delegate void AppendTextDelegate(string text);

	private struct HDITEM
	{
		public int mask;

		public int cxy;

		public IntPtr pszText;

		public IntPtr hbm;

		public int cchTextMax;

		public int fmt;

		public IntPtr lParam;

		public int iImage;

		public int iOrder;

		public uint type;

		public IntPtr pvFilter;

		public uint state;
	}

	private const uint HDM_FIRST = 4608u;

	private const uint HDM_GETITEM = 4619u;

	private const uint HDM_SETITEM = 4620u;

	private const uint LVM_GETHEADER = 4127u;

	private const int HDF_SORTUP = 1024;

	private const int HDF_SORTDOWN = 512;

	private const int HDI_FORMAT = 4;

	private int currentSortColumn = -1;

	private bool currentSortAscending = true;

	public static Config config;

	public V2rayHandler v2rayHandler;

	public SubconverHandler subconverHandler;

	private List<int> lvSelecteds = new List<int>();

	private Dictionary<int, VmessItem> speedtestNodeMap = new Dictionary<int, VmessItem>();

	public CancellationTokenSource cts;

	private IContainer components;

	private SplitContainer splitContainer1;

	private ListViewFlickerFree lvServers;

	private Panel panel1;

	private ContextMenuStrip cmsMain;

	private ToolStripMenuItem menuExit;

	private ContextMenuStrip cmsLv;

	private ToolStripMenuItem menuRealPingServer;

	private ToolStripMenuItem menuDownLoadServer;

	private ToolStripMenuItem menuAutoTestSelected;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripMenuItem menuAddServers;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripMenuItem menuSelectAll;

	private ToolStripMenuItem menuRemoveServer;

	private ToolStripMenuItem menuEditServer;

	private ToolStripMenuItem menuRemoveDuplicateServer;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripMenuItem menuExport2ShareUrl;

	private ToolStripMenuItem menuExport2SubContent;

	private ToolStripSeparator toolStripSeparator6;

	private ToolStripMenuItem menuExport2Base64;

	private ToolStripMenuItem menuExport2Clash;

	private GroupBox groupBox3;

	private Button btnStartTest;

	private Button btnSaveConfig;

	private Button btStopTest;

	private Label label2;

	private TextBox tbLowSpeed;

	private Panel panel2;

	private GroupBox groupBox1;

	private TextBox txtMsgBox;

	private Button btnClearLog;

	private Button btnClearResult;

	private Label label3;

	private TextBox tbPingNum;

	private TextBox tbTimeout;

	private Label label4;

	private ToolStripMenuItem menuRemoveLoseServer;

	private ToolStripMenuItem menuRemoveLowServer;

	private ToolStripMenuItem menuRemoveNoResultServer;

	private CheckBox cbRealPing;

	private CheckBox cbSpeedTest;

	private ToolStripSeparator toolStripSeparator7;

	private ToolStripMenuItem menuStartClash;

	private TextBox tbClashPort;

	private Label label5;

	private Label label10;

	private TextBox tb_fm_ave;

	private Label label8;

	private TextBox tb_fm_max;

	private TextBox tb_fm_second;

	private Label label6;

	private CheckBox cbFastMode;

	private TextBox tbThread;

	private Button button2;

	private Button btnSettings;

	private Button button1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem 订阅列表ToolStripMenuItem;

	private ToolStripMenuItem 导入到节点列表ToolStripMenuItem;

	private Panel groupBox5;

	private Label label12;

	private CheckBox cbAutoSort;

	private Label label15;

	private ComboBox cmbSortColumn;

	private Label label16;

	private ComboBox cmbSortOrder;

	private CheckBox cbStrictMode;

	private TabControl tabSubscriptions;

	private ContextMenuStrip cmsTabSubscriptions;

	private string rightClickedTabSubId;

	private int draggedTabIndex = -1;

	private Rectangle dragBoxFromMouseDown;

	private string currentSubFilter;

	private CheckBox cbKeywordFilter;

	private TextBox tbKeywordFilter;

	private Button btnKeywordPreset;

	private ContextMenuStrip cmsKeywordPreset;

	private CheckBox cbAutoSave;

	private ToolTip toolTipKeywordFilter;

	private const int SubconverterPort = 25500;

	public static MainForm Instance { get; private set; }

	[DllImport("user32.dll")]
	private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", EntryPoint = "SendMessage")]
	private static extern IntPtr SendMessageHDITEM(IntPtr hWnd, uint Msg, IntPtr wParam, ref HDITEM lParam);

	public MainForm()
	{
		Instance = this;
		InitializeComponent();
		InitializeKeywordPresetMenu();
		InitializeSubscriptionTabControl();
		Global.processJob = new Job();
		Application.ApplicationExit += delegate
		{
			MyAppExit(blWindowsShutDown: false);
		};
		cmbSortColumn.SelectedIndex = 0;
		cmbSortOrder.SelectedIndex = 0;
		cbAutoSort.Checked = false;
		cmbSortColumn.Enabled = false;
		cmbSortOrder.Enabled = false;
		cbStrictMode.Checked = false;
	}

	private void InitializeKeywordPresetMenu()
	{
		cmsKeywordPreset.Items.AddRange(new ToolStripItem[14]
		{
			CreatePresetMenuItem("\ud83c\udded\ud83c\uddf0 香港", "\ud83c\udded\ud83c\uddf0,香港,HK"),
			CreatePresetMenuItem("\ud83c\uddf9\ud83c\uddfc 台湾", "\ud83c\uddf9\ud83c\uddfc,台湾,TW"),
			CreatePresetMenuItem("\ud83c\uddef\ud83c\uddf5 日本", "\ud83c\uddef\ud83c\uddf5,日本,JP"),
			CreatePresetMenuItem("\ud83c\uddfa\ud83c\uddf8 美国", "\ud83c\uddfa\ud83c\uddf8,美国,US,USA"),
			CreatePresetMenuItem("\ud83c\uddf8\ud83c\uddec 新加坡", "\ud83c\uddf8\ud83c\uddec,新加坡,SG,SGP"),
			CreatePresetMenuItem("\ud83c\uddf0\ud83c\uddf7 韩国", "\ud83c\uddf0\ud83c\uddf7,韩国,KR"),
			new ToolStripSeparator(),
			CreatePresetMenuItem("\ud83c\uddec\ud83c\udde7 英国", "\ud83c\uddec\ud83c\udde7,英国,UK,GB"),
			CreatePresetMenuItem("\ud83c\udde9\ud83c\uddea 德国", "\ud83c\udde9\ud83c\uddea,德国,DE"),
			CreatePresetMenuItem("\ud83c\udde8\ud83c\udde6 加拿大", "\ud83c\udde8\ud83c\udde6,加拿大,CA"),
			CreatePresetMenuItem("\ud83c\udde6\ud83c\uddfa 澳大利亚", "\ud83c\udde6\ud83c\uddfa,澳大利亚,AU"),
			CreatePresetMenuItem("\ud83c\uddeb\ud83c\uddf7 法国", "\ud83c\uddeb\ud83c\uddf7,法国,FR"),
			new ToolStripSeparator(),
			CreatePresetMenuItem("\ud83c\udf10 全球/清空", "", "CLEAR")
		});
	}

	// 列定义：key, 显示名称, 默认宽度
	private readonly List<(string key, string displayName, int defaultWidth)> columnDefinitions = new List<(string, string, int)>
	{
		("def", "No", 40),
		("configType", "类型", 80),
		("remarks", "别名", 200),
		("address", "服务器地址", 120),
		("port", "端口", 50),
		("security", "加密方式", 90),
		("network", "传输协议", 70),
		("tls", "TLS", 80),
		("subRemarks", "订阅", 70),
		("httpsDelay", "HTTPS延迟", 80),
		("testResult", "平均速度", 200),
		("MaxSpeed", "峰值速度", 80)
	};

	// 当前列顺序（key列表）
	private List<string> currentColumnOrder;

	private void InitServersView()
	{
		lvServers.BeginUpdate();
		lvServers.Items.Clear();
		lvServers.GridLines = true;
		lvServers.FullRowSelect = true;
		lvServers.View = View.Details;
		lvServers.Scrollable = true;
		lvServers.MultiSelect = true;
		lvServers.HeaderStyle = ColumnHeaderStyle.Clickable;
		
		// 获取列顺序
		currentColumnOrder = GetColumnOrder();
		
		// 按顺序添加列
		foreach (var key in currentColumnOrder)
		{
			var colDef = columnDefinitions.FirstOrDefault(c => c.key == key);
			if (colDef.key != null)
			{
				lvServers.Columns.Add(colDef.displayName, GetVisibleColumnWidth(key, colDef.defaultWidth), HorizontalAlignment.Center);
			}
		}
		
		if (config.recordTestTime)
		{
			lvServers.Columns.Add("最后测速", GetColumnWidth("lastTestTime", 130), HorizontalAlignment.Center);
		}
		EnsureColumnHeaderMinWidths();
		lvServers.EndUpdate();
	}

	private List<string> GetColumnOrder()
	{
		// 清理旧的 tlsRtt 配置（已移除的功能）
		if (config.uiItem?.mainLvColOrder != null)
		{
			config.uiItem.mainLvColOrder.RemoveAll(k => k == "tlsRtt");
		}
		if (config.uiItem?.mainLvColVisible != null && config.uiItem.mainLvColVisible.ContainsKey("tlsRtt"))
		{
			config.uiItem.mainLvColVisible.Remove("tlsRtt");
		}
		if (config.uiItem?.mainLvColWidth != null && config.uiItem.mainLvColWidth.ContainsKey("tlsRtt"))
		{
			config.uiItem.mainLvColWidth.Remove("tlsRtt");
		}
		
		if (config.uiItem?.mainLvColOrder != null && config.uiItem.mainLvColOrder.Count > 0)
		{
			// 使用保存的顺序，但确保包含所有列
			var order = new List<string>(config.uiItem.mainLvColOrder);
			foreach (var col in columnDefinitions)
			{
				if (!order.Contains(col.key))
				{
					order.Add(col.key);
				}
			}
			return order;
		}
		// 默认顺序
		return columnDefinitions.Select(c => c.key).ToList();
	}

	private bool IsColumnVisible(string columnName)
	{
		if (config.uiItem?.mainLvColVisible == null)
			return true;
		if (config.uiItem.mainLvColVisible.TryGetValue(columnName, out bool visible))
			return visible;
		return true;
	}

	private int GetVisibleColumnWidth(string key, int defaultValue)
	{
		if (!IsColumnVisible(key))
			return 0;
		return GetColumnWidth(key, defaultValue);
	}

	/// <summary>
	/// 根据列名获取实际的列索引（考虑隐藏列）
	/// </summary>
	private int GetColumnIndexByName(string columnText)
	{
		for (int i = 0; i < lvServers.Columns.Count; i++)
		{
			if (lvServers.Columns[i].Text == columnText)
				return i;
		}
		return -1;
	}

	/// <summary>
	/// 根据列索引获取对应的 EServerColName 枚举值
	/// </summary>
	private EServerColName GetServerColNameByIndex(int columnIndex)
	{
		if (columnIndex < 0 || columnIndex >= lvServers.Columns.Count)
			return EServerColName.def;
		
		string columnText = lvServers.Columns[columnIndex].Text;
		return columnText switch
		{
			"No" => EServerColName.def,
			"类型" => EServerColName.configType,
			"别名" => EServerColName.remarks,
			"服务器地址" => EServerColName.address,
			"端口" => EServerColName.port,
			"加密方式" => EServerColName.security,
			"传输协议" => EServerColName.network,
			"TLS" => EServerColName.tls,
			"订阅" => EServerColName.subRemarks,
			"HTTPS延迟" => EServerColName.httpsDelay,
			"平均速度" => EServerColName.testResult,
			"峰值速度" => EServerColName.MaxSpeed,
			"最后测速" => EServerColName.lastTestTime,
			_ => EServerColName.def
		};
	}

	private void menuExit_Click(object sender, EventArgs e)
	{
		base.Visible = false;
		Close();
		Application.Exit();
	}

	private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			ShowForm();
		}
	}

	private void ShowForm()
	{
		Show();
		base.WindowState = FormWindowState.Normal;
		Activate();
		base.ShowInTaskbar = true;
		txtMsgBox.ScrollToCaret();
		SetVisibleCore(value: true);
	}

	private void RefreshServers()
	{
		RefreshServersView();
	}

	public void RefreshServersView()
	{
		lvServers.BeginUpdate();
		lvServers.Items.Clear();
		int num = 0;
		
		// 确保有列顺序
		if (currentColumnOrder == null)
		{
			currentColumnOrder = GetColumnOrder();
		}
		
		for (int i = 0; i < config.vmess.Count; i++)
		{
			VmessItem vmessItem = config.vmess[i];
			if ((currentSubFilter == null || (currentSubFilter == "__unassigned__" && string.IsNullOrEmpty(vmessItem.subid)) || (currentSubFilter != "__unassigned__" && vmessItem.subid == currentSubFilter)) && MatchesKeywordFilter(vmessItem.remarks))
			{
				num++;
				ListViewItem listViewItem = new ListViewItem(GetColumnValue(vmessItem, currentColumnOrder[0], num));
				
				// 按列顺序添加 SubItems（跳过第一列，因为已经是 ListViewItem 的 Text）
				for (int j = 1; j < currentColumnOrder.Count; j++)
				{
					string key = currentColumnOrder[j];
					Utils.AddSubItem(listViewItem, key, GetColumnValue(vmessItem, key, num));
				}
				
				if (config.recordTestTime)
				{
					Utils.AddSubItem(listViewItem, EServerColName.lastTestTime.ToString(), vmessItem.lastTestTime);
				}
				listViewItem.Tag = i;
				if (num % 2 == 1)
				{
					listViewItem.BackColor = Color.WhiteSmoke;
				}
				if (listViewItem != null)
				{
					lvServers.Items.Add(listViewItem);
				}
			}
		}
		lvServers.EndUpdate();
		UpdateUnassignedTabVisibility();
	}

	private string GetColumnValue(VmessItem vmessItem, string columnKey, int rowNum)
	{
		return columnKey switch
		{
			"def" => rowNum.ToString(),
			"configType" => ((EConfigType)vmessItem.configType).ToString(),
			"remarks" => vmessItem.remarks,
			"address" => vmessItem.address,
			"port" => vmessItem.port.ToString(),
			"security" => vmessItem.security,
			"network" => vmessItem.network,
			"tls" => Utils.IsNullOrEmpty(vmessItem.streamSecurity) ? "none" : vmessItem.streamSecurity,
			"subRemarks" => vmessItem.getSubRemarks(config),
			"httpsDelay" => Utils.IsNullOrEmpty(vmessItem.httpsDelay) ? "" : vmessItem.httpsDelay,
			"testResult" => vmessItem.testResult,
			"MaxSpeed" => vmessItem.MaxSpeed,
			"lastTestTime" => vmessItem.lastTestTime,
			_ => ""
		};
	}

	private int GetColumnWidth(string key, int defaultValue)
	{
		if (config.uiItem.mainLvColWidth == null)
		{
			return defaultValue;
		}
		if (config.uiItem.mainLvColWidth.ContainsKey(key))
		{
			int num = config.uiItem.mainLvColWidth[key];
			if (num != 0)
			{
				return num;
			}
			return defaultValue;
		}
		return defaultValue;
	}

	private int GetColumnHeaderMinWidth(int columnIndex)
	{
		if (columnIndex < 0 || columnIndex >= lvServers.Columns.Count)
		{
			return 0;
		}
		return TextRenderer.MeasureText(lvServers.Columns[columnIndex].Text ?? string.Empty, lvServers.Font).Width + 20;
	}

	private int CalculateAutoColumnWidth(int columnIndex)
	{
		int num = GetColumnHeaderMinWidth(columnIndex);
		if (num == 0)
		{
			return 0;
		}
		foreach (ListViewItem item in lvServers.Items)
		{
			int num2 = TextRenderer.MeasureText(((columnIndex == 0) ? item.Text : ((columnIndex < item.SubItems.Count) ? item.SubItems[columnIndex].Text : string.Empty)) ?? string.Empty, lvServers.Font).Width + 20;
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	private void HideForm()
	{
		Hide();
		base.ShowInTaskbar = false;
		SetVisibleCore(value: false);
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
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
		ConfigHandler.LoadConfig(ref config);
		base.Size = config.uiItem.mainSize;
		v2rayHandler = new V2rayHandler();
		v2rayHandler.ProcessEvent += v2rayHandler_ProcessEvent;
		v2rayHandler.ClashStart("-d ./config -ext-ctl 127.0.0.1:40001");
		subconverHandler = new SubconverHandler();
		subconverHandler.ProcessEvent += v2rayHandler_ProcessEvent;
		subconverHandler.SubconverStartNew();
		ConfigHandler.ProcessEvent += configHandler_ProcessEvent;
		InitServersView();
		InitSubscriptionTabs();
		RefreshServers();
		RestoreUI();
		TestParameter();
		Global.reloadV2ray = false;
		v2rayHandler.LoadV2rayCore(config);
		UpdateLastTestTimeColumnVisibility();
	}

	public void ShowMsg(string msg)
	{
		if (txtMsgBox.Lines.Length > 999)
		{
			ClearMsg();
		}
		txtMsgBox.AppendText(msg);
		if (!msg.EndsWith(Environment.NewLine))
		{
			txtMsgBox.AppendText(Environment.NewLine);
		}
	}

	private void ClearMsg()
	{
		txtMsgBox.Clear();
	}

	private void v2rayHandler_ProcessEvent(bool notify, string msg)
	{
		AppendText(notify, msg);
	}

	private void configHandler_ProcessEvent(bool notify, string msg)
	{
		AppendText(msg);
	}

	private void AppendText(bool notify, string msg)
	{
		try
		{
			AppendText(msg);
		}
		catch
		{
		}
	}

	private void AppendText(string text)
	{
		if (txtMsgBox.InvokeRequired)
		{
			Invoke(new AppendTextDelegate(AppendText), text);
		}
		else
		{
			ShowMsg(text);
		}
	}

	private void RestoreUI()
	{
		if (!config.uiItem.mainSize.IsEmpty && config.uiItem.mainSize.Width >= 400 && config.uiItem.mainSize.Height >= 300)
		{
			base.Width = config.uiItem.mainSize.Width;
			base.Height = config.uiItem.mainSize.Height;
		}
		if (config.saveWindowPosition && !config.uiItem.mainLocation.IsEmpty)
		{
			Point mainLocation = config.uiItem.mainLocation;
			if (mainLocation.X > -1000 && mainLocation.Y > -1000 && mainLocation.X < SystemInformation.VirtualScreen.Right && mainLocation.Y < SystemInformation.VirtualScreen.Bottom)
			{
				base.StartPosition = FormStartPosition.Manual;
				base.Location = mainLocation;
			}
		}
		for (int i = 0; i < lvServers.Columns.Count; i++)
		{
			EServerColName eServerColName = GetServerColNameByIndex(i);
			int val = ConfigHandler.GetformMainLvColWidth(ref config, eServerColName.ToString(), lvServers.Columns[i].Width);
			// 如果列被隐藏（宽度为0），保持隐藏状态
			if (IsColumnVisible(eServerColName.ToString()))
			{
				lvServers.Columns[i].Width = Math.Max(val, GetColumnHeaderMinWidth(i));
			}
		}
	}

	private void EnsureColumnHeaderMinWidths()
	{
		for (int i = 0; i < lvServers.Columns.Count; i++)
		{
			// 跳过隐藏的列（宽度为0）
			if (lvServers.Columns[i].Width == 0)
				continue;
			int columnHeaderMinWidth = GetColumnHeaderMinWidth(i);
			if (columnHeaderMinWidth > 0 && lvServers.Columns[i].Width < columnHeaderMinWidth)
			{
				lvServers.Columns[i].Width = columnHeaderMinWidth;
			}
		}
	}

	private void TestParameter()
	{
		tbTimeout.Text = config.Timeout;
		tbPingNum.Text = config.PingNum;
		tbLowSpeed.Text = config.LowSpeed;
		cbFastMode.Checked = config.fastMode;
		tb_fm_ave.Text = config.FMave;
		tb_fm_max.Text = config.FMmax;
		tb_fm_second.Text = config.FMSecond;
		tbThread.Text = config.Thread;
		tbClashPort.Text = config.ClashPort;
		cbSpeedTest.Checked = config.speedAble;
		cbRealPing.Checked = config.pingAble;
		cbAutoSort.Checked = config.autoSortEnabled;
		cmbSortColumn.SelectedIndex = config.autoSortColumn;
		cmbSortOrder.SelectedIndex = config.autoSortOrder;
		cbStrictMode.Checked = config.strictExclusionMode;
		tbKeywordFilter.Text = config.keywordFilter ?? string.Empty;
		cbKeywordFilter.Checked = config.keywordFilterEnabled;
		tbKeywordFilter.Enabled = config.keywordFilterEnabled;
		cbAutoSave.Checked = config.autoSaveAfterTest;
	}

	private bool DetectTunModeAndWarn()
	{
		if (!config.tunWarningEnabled)
		{
			return true;
		}
		if (Utils.TryDetectTunAdapter(out var adapterDisplayName))
		{
			string text = "检测到系统存在可能的代理 TUN/TAP 网卡：\n" + adapterDisplayName + "\n\n开启 TUN 模式会让测速结果包含代理链路导致结果失真，\n建议测速前关闭 TUN 模式或将本程序加入直连列表。\n\n是否仍要继续测速？";
			try
			{
				return MessageBox.Show(text, "检测到 TUN 模式", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
			}
			catch
			{
				return true;
			}
		}
		return true;
	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		switch (e.CloseReason)
		{
		case CloseReason.UserClosing:
			if (config.exitConfirmEnabled || MessageBox.Show("确认退出吗（请确认配置是否已保存）？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				StorageUI();
				if (config.saveConfigOnExit)
				{
					ConfigHandler.SaveConfig(ref config, reload: false);
				}
				else
				{
					ConfigHandler.SaveUIConfigOnly(config);
				}
				base.Visible = false;
				Application.Exit();
			}
			else
			{
				e.Cancel = true;
			}
			break;
		case CloseReason.TaskManagerClosing:
		case CloseReason.FormOwnerClosing:
		case CloseReason.ApplicationExitCall:
			StorageUI();
			if (config.saveConfigOnExit)
			{
				ConfigHandler.SaveConfig(ref config, reload: false);
			}
			else
			{
				ConfigHandler.SaveUIConfigOnly(config);
			}
			MyAppExit(blWindowsShutDown: false);
			break;
		case CloseReason.WindowsShutDown:
			StorageUI();
			if (config.saveConfigOnExit)
			{
				ConfigHandler.SaveConfig(ref config, reload: false);
			}
			else
			{
				ConfigHandler.SaveUIConfigOnly(config);
			}
			MyAppExit(blWindowsShutDown: true);
			break;
		case CloseReason.MdiFormClosing:
			break;
		}
	}

	private void StorageUI()
	{
		config.uiItem.mainSize = new Size(base.Width, base.Height);
		if (config.saveWindowPosition)
		{
			config.uiItem.mainLocation = base.Location;
		}
		for (int i = 0; i < lvServers.Columns.Count; i++)
		{
			EServerColName eServerColName = GetServerColNameByIndex(i);
			string key = eServerColName.ToString();
			// 跳过隐藏列，不保存宽度为0
			if (!IsColumnVisible(key))
				continue;
			ConfigHandler.AddformMainLvColWidth(ref config, key, lvServers.Columns[i].Width);
		}
	}

	private void MyAppExit(bool blWindowsShutDown)
	{
		try
		{
			v2rayHandler.V2rayStop();
		}
		catch
		{
		}
	}

	private void MainForm_Resize(object sender, EventArgs e)
	{
	}

	private void menuRealPingServer_Click(object sender, EventArgs e)
	{
		btStopTest.Enabled = true;
		Speedtest("httpsdelay");
	}

	private void menuDownLoadServer_Click(object sender, EventArgs e)
	{
		btStopTest.Enabled = true;
		Speedtest("speedtest");
	}



	private void menuAutoTestSelected_Click(object sender, EventArgs e)
	{
		if (GetLvSelectedIndex() < 0)
		{
			UI.Show("请先选择需要测速的服务器", this);
			return;
		}
		if (!DetectTunModeAndWarn())
		{
			AppendText(notify: false, "已取消测速（检测到TUN模式）");
			return;
		}
		btStopTest.Enabled = true;
		AppendText(notify: false, "选中节点一键自动测速已开启");
		Task.Run(delegate
		{
			AutoRunSelected();
		});
	}

	private void Speedtest(string actionType)
	{
		if (GetLvSelectedIndex() < 0)
		{
			return;
		}
		int pid = -1;
		bool flag = false;
		foreach (int lvSelected in lvSelecteds)
		{
			if (lvSelected >= 0 && lvSelected < config.vmess.Count && config.vmess[lvSelected].configType == 11)
			{
				flag = true;
				break;
			}
		}
		if ((actionType == "realping" && config.ThreadNum == 0) || (actionType == "speedtest" && config.DownloadThreadNum == 0) || actionType == "httpsdelay" || flag)
		{
			try
			{
				createYamlConfig();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "出现异常");
				return;
			}
		}
		else if ((actionType == "realping" && config.ThreadNum == 1) || (actionType == "speedtest" && config.DownloadThreadNum == 1))
		{
			AppendText(notify: false, "单线程模式仅支持多线程测速，请将线程数设置为0");
			return;
		}
		speedtestNodeMap.Clear();
		foreach (int lvSelected2 in lvSelecteds)
		{
			if (lvSelected2 >= 0 && lvSelected2 < config.vmess.Count)
			{
				speedtestNodeMap[lvSelected2] = config.vmess[lvSelected2];
			}
		}
		switch (actionType)
		{
		case "httpsdelay":
		case "realping":
			foreach (int lvSelected4 in lvSelecteds)
			{
				SetHttpsDelay(lvSelected4, "");
			}
			break;
		case "speedtest":
			foreach (int lvSelected7 in lvSelecteds)
			{
				SetTestResult(lvSelected7, "等待测速线程...");
			}
			break;
		default:
			ClearTestResult();
			break;
		}
		config.index = 0;
		cts = new CancellationTokenSource();
		switch (actionType)
		{
		case "realping":
		case "httpsdelay":
			new SpeedtestHandler(ref config, ref cts, ref v2rayHandler, lvSelecteds, actionType, UpdateSpeedtestHandler, UpdateMaxSpeedHandler, UpdateHttpsDelayHandler, btStopTestStat, pid);
			break;
		default:
			new SpeedtestHandler(ref config, ref cts, ref v2rayHandler, lvSelecteds, actionType, UpdateSpeedtestHandler, UpdateMaxSpeedHandler, btStopTestStat, pid);
			break;
		}
	}

	private void SetMaxSpeedResult(int k, string txt)
	{
		if (k < 0 || k >= config.vmess.Count)
		{
			return;
		}
		config.vmess[k].MaxSpeed = txt;
		foreach (ListViewItem item in lvServers.Items)
		{
			if (item.Tag != null && (int)item.Tag == k)
			{
				item.SubItems["MaxSpeed"].Text = txt;
				break;
			}
		}
	}

	private void SetTestResult(int k, string txt)
	{
		if (k < 0 || k >= config.vmess.Count)
		{
			return;
		}
		config.vmess[k].testResult = txt;
		foreach (ListViewItem item in lvServers.Items)
		{
			if (item.Tag != null && (int)item.Tag == k)
			{
				item.SubItems["testResult"].Text = txt;
				break;
			}
		}
		if (txt.Contains("MB/s") || txt.Contains("KB/s"))
		{
			SetLastTestTime(k, DateTime.Now.ToString("MM-dd HH:mm:ss"));
		}
	}

	private void SetLastTestTime(int k, string txt)
	{
		if (!config.recordTestTime || k < 0 || k >= config.vmess.Count)
		{
			return;
		}
		config.vmess[k].lastTestTime = txt;
		foreach (ListViewItem item in lvServers.Items)
		{
			if (item.Tag != null && (int)item.Tag == k)
			{
				if (item.SubItems.ContainsKey("lastTestTime"))
				{
					item.SubItems["lastTestTime"].Text = txt;
				}
				break;
			}
		}
	}

	private void SetHttpsDelay(int k, string txt)
	{
		if (k < 0 || k >= config.vmess.Count)
		{
			return;
		}
		config.vmess[k].httpsDelay = txt;
		foreach (ListViewItem item in lvServers.Items)
		{
			if (item.Tag != null && (int)item.Tag == k)
			{
				item.SubItems["httpsDelay"].Text = txt;
				break;
			}
		}
	}

	private void ClearTestResult()
	{
		foreach (int lvSelected in lvSelecteds)
		{
			SetTestResult(lvSelected, "等待测速线程...");
		}
	}

	public void createYamlConfig()
	{
		int externalControllerPort = config.externalControllerPort;
		string text = Guid.NewGuid().ToString("N");
		string path = Utils.GetPath("subconverter\\temp_" + text + ".txt");
		string text2 = "";
		string text3 = "";
		string path2 = Utils.GetPath("config\\temp.yaml");
		try
		{
			if (!ConfigHandler.TcpClientCheck("127.0.0.1", externalControllerPort))
			{
				throw new Exception("无法链接到Mihomo内核，请确认：\n1. mihomo-nodes.exe 文件是否存在于程序目录\n2. 端口 " + externalControllerPort + " 是否被占用\n3. 尝试重新打开测速软件");
			}
			if (!ConfigHandler.TcpClientCheck("127.0.0.1", 25500))
			{
				throw new Exception("无法链接到subconverter(端口:25500)，请确认：\n1. subconverter\\subconverter.exe 文件是否存在\n2. 端口 25500 是否被占用\n3. 尝试重新打开测速软件");
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<VmessItem> list = new List<VmessItem>();
			List<string> list2 = new List<string>();
			foreach (int lvSelected in lvSelecteds)
			{
				if (lvSelected >= 0 && lvSelected < config.vmess.Count)
				{
					VmessItem vmessItem = config.vmess[lvSelected];
					if (vmessItem.configType == 12)
					{
						string item = (config.GetLocalPort() + lvSelected).ToString();
						list.Add(vmessItem);
						list2.Add(item);
						continue;
					}
				}
				string shareUrl = ShareHandler.GetShareUrl(config, lvSelected, changRemark: true);
				if (!Utils.IsNullOrEmpty(shareUrl))
				{
					stringBuilder.Append(shareUrl);
					stringBuilder.AppendLine();
				}
			}
			string text4 = "";
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Insert(0, "ss://YWVzLTI1Ni1nY206ZmFCQW9ENTRrODdVSkc3QDEuMS4xLjE6NjY2#%e5%8d%a0%e4%bd%8d%e8%8a%82%e7%82%b9" + Environment.NewLine);
				File.WriteAllText(path, Utils.Base64Encode(stringBuilder.ToString()), Encoding.UTF8);
				HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://127.0.0.1:{25500}/sub?target=clash&url=temp_{text}.txt&insert=false&list=true");
				obj.Method = "GET";
				obj.Timeout = 10000;
				obj.ReadWriteTimeout = 10000;
				obj.ContinueTimeout = 10000;
				using StreamReader streamReader = new StreamReader(obj.GetResponse().GetResponseStream(), Encoding.UTF8);
				text4 = streamReader.ReadToEnd();
			}
			else
			{
				text4 = "mixed-port: 40000\r\nmode: Global\r\nlog-level: info\r\nexternal-controller: 127.0.0.1:" + externalControllerPort + "\r\nproxies:\r\nproxy-groups:\r\n  - name: PROXY\r\n    type: select\r\n    proxies:\r\nrules:\r\n  - MATCH,PROXY\r\n";
			}
			if (list.Count > 0)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				StringBuilder stringBuilder3 = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					VmessItem vmessItem2 = list[i];
					string text5 = list2[i];
					bool flag = vmessItem2.allowInsecure == "true" || vmessItem2.allowInsecure == "1";
					stringBuilder2.AppendLine("  - name: \"" + text5 + "\"");
					stringBuilder2.AppendLine("    type: anytls");
					stringBuilder2.AppendLine("    server: " + vmessItem2.address);
					stringBuilder2.AppendLine($"    port: {vmessItem2.port}");
					stringBuilder2.AppendLine("    password: \"" + vmessItem2.id + "\"");
					if (!string.IsNullOrEmpty(vmessItem2.sni))
					{
						stringBuilder2.AppendLine("    sni: " + vmessItem2.sni);
					}
					stringBuilder2.AppendLine("    skip-cert-verify: " + flag.ToString().ToLower());
					stringBuilder2.AppendLine("    udp: true");
					stringBuilder3.AppendLine("      - \"" + text5 + "\"");
				}
				int num = text4.IndexOf("proxies:");
				if (num >= 0)
				{
					int startIndex = text4.IndexOf("\n", num) + 1;
					text4 = text4.Insert(startIndex, stringBuilder2.ToString());
				}
				int num2 = text4.IndexOf("proxy-groups:");
				if (num2 >= 0)
				{
					int num3 = text4.IndexOf("proxies:", num2);
					if (num3 >= 0)
					{
						int startIndex2 = text4.IndexOf("\n", num3) + 1;
						text4 = text4.Insert(startIndex2, stringBuilder3.ToString());
					}
				}
			}
			if (string.IsNullOrEmpty(text4) || (!text4.Contains("proxies:") && list.Count == 0))
			{
				throw new Exception("没有可用测速节点");
			}
			File.WriteAllText(path2, text4, Encoding.UTF8);
			ShowMsg($"Mihomo配置文件生成成功（含 {list.Count} 个 AnyTLS 节点）");
			string args = JsonConvert.SerializeObject(new
			{
				path = Path.GetFullPath(path2).Replace("\\", "/")
			}, Formatting.None);
			text3 = "http://127.0.0.1:" + externalControllerPort + "/configs";
			text2 = ConfigHandler.sendReq(args, text3, "PUT");
			if (text2 != "204")
			{
				throw new Exception("切换测速配置文件失败！" + text2);
			}
			ShowMsg("切换测速配置文件成功！");
			text2 = ConfigHandler.sendReq(JsonConvert.SerializeObject(new
			{
				port = 40000
			}, Formatting.None), text3, "PATCH");
			if (text2 != "204")
			{
				throw new Exception("切换http代理端口失败！" + text2);
			}
			ShowMsg("切换http代理端口成功！");
			text2 = ConfigHandler.sendReq(JsonConvert.SerializeObject(new
			{
				mode = "Global"
			}, Formatting.None), text3, "PATCH");
			if (text2 != "204")
			{
				throw new Exception("切换全局代理失败！" + text2);
			}
			ShowMsg("切换全局代理成功！");
		}
		finally
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			catch
			{
			}
		}
	}

	private void UpdateSpeedtestHandler(int index, string msg)
	{
		lvServers.Invoke((MethodInvoker)delegate
		{
			SetTestResult(index, msg);
		});
	}

	private void UpdateHttpsDelayHandler(int index, string msg)
	{
		lvServers.Invoke((MethodInvoker)delegate
		{
			SetHttpsDelay(index, msg);
		});
	}

	private void UpdateMaxSpeedHandler(int index, string msg)
	{
		lvServers.Invoke((MethodInvoker)delegate
		{
			SetMaxSpeedResult(index, msg);
		});
	}

	private void btStopTestStat(bool enable)
	{
		btStopTest.Enabled = enable;
	}

	private int GetLvSelectedIndex()
	{
		int result = -1;
		lvSelecteds.Clear();
		try
		{
			if (lvServers.SelectedIndices.Count <= 0)
			{
				UI.Show("请先选择需要测速的服务器", this);
				return result;
			}
			result = ((lvServers.SelectedItems.Count <= 0 || lvServers.SelectedItems[0].Tag == null) ? lvServers.SelectedIndices[0] : ((int)lvServers.SelectedItems[0].Tag));
			foreach (ListViewItem selectedItem in lvServers.SelectedItems)
			{
				if (selectedItem.Tag != null)
				{
					lvSelecteds.Add((int)selectedItem.Tag);
				}
				else
				{
					lvSelecteds.Add(lvServers.Items.IndexOf(selectedItem));
				}
			}
			return result;
		}
		catch
		{
			return result;
		}
	}

	private bool MatchesKeywordFilter(string remarks)
	{
		if (config == null || !config.keywordFilterEnabled)
		{
			return true;
		}
		string text = tbKeywordFilter?.Text?.Trim();
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		if (string.IsNullOrEmpty(remarks))
		{
			return false;
		}
		string[] array = text.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		bool flag = false;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text2 = array2[i].Trim();
			if (text2.StartsWith("!"))
			{
				string value = text2.Substring(1);
				if (!string.IsNullOrEmpty(value) && remarks.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return false;
				}
			}
			else
			{
				flag = true;
				if (remarks.IndexOf(text2, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
		}
		return !flag;
	}

	private async void menuAddServers_Click(object sender, EventArgs e)
	{
		string clipboardData = Utils.GetClipboardData();
		if (string.IsNullOrWhiteSpace(clipboardData))
		{
			return;
		}
		string text = clipboardData.Trim();
		if ((text.StartsWith("http://") || text.StartsWith("https://")) && !text.Contains("vmess://") && !text.Contains("vless://") && !text.Contains("ss://") && !text.Contains("ssr://") && !text.Contains("trojan://") && !text.Contains("hysteria2://") && !text.Contains("hy2://") && !text.Contains("anytls://"))
		{
			ImportFromSubscriptionUrl(text);
			return;
		}
		AppendText(notify: false, "正在处理节点...");
		int num = await Task.Run(() => MainFormHandler.Instance.AddBatchServers(config, clipboardData));
		if (num > 0)
		{
			RefreshServers();
			UI.Show($"成功从剪贴板导入{num}个节点", this);
		}
		else
		{
			AppendText(notify: false, "没有导入任何节点");
		}
	}

	private void ImportFromSubscriptionUrl(string url)
	{
		int nodeCountBefore = config.vmess?.Count ?? 0;
		AppendText(notify: false, "检测到订阅链接，开始导入...");
		AppendText(notify: false, "--> URL: " + url);
		AppendText(notify: false, "--> 正在下载订阅数据...");
		DownloadHandle downloadHandle = new DownloadHandle();
		downloadHandle.Error += delegate(object obj, ErrorEventArgs args)
		{
			AppendText(notify: false, "--> 下载失败：" + args.GetException().Message);
			ShowMsg("下载订阅失败：" + args.GetException().Message);
		};
		downloadHandle.UpdateCompleted += delegate(object obj, DownloadHandle.ResultEventArgs args)
		{
			if (args.Success)
			{
				AppendText(notify: false, "--> 获取订阅数据成功，数据长度：" + args.Msg.Length);
				string text = Utils.Base64Decode(args.Msg);
				if (!Utils.IsNullOrEmpty(text))
				{
					AppendText(notify: false, "--> Base64解码成功");
					if (text.IndexOf("vmess://") != -1 || text.IndexOf("vless://") != -1 || text.IndexOf("ss://") != -1 || text.IndexOf("ssr://") != -1 || text.IndexOf("trojan://") != -1 || text.IndexOf("socks://") != -1 || text.IndexOf("hysteria2://") != -1 || text.IndexOf("hy2://") != -1 || text.IndexOf("anytls://") != -1)
					{
						if (MainFormHandler.Instance.AddBatchServers(config, text) <= 0)
						{
							AppendText(notify: false, "--> 导入节点信息失败");
							ShowMsg("导入节点信息失败");
						}
						else
						{
							int num = (config.vmess?.Count ?? 0) - nodeCountBefore;
							AppendText(notify: false, $"--> 成功导入 {num} 个节点");
							InitSubscriptionTabs();
							RefreshServersView();
							ShowMsg($"从订阅链接导入了 {num} 个节点");
						}
					}
					else
					{
						AppendText(notify: false, "--> 尝试作为非base64格式导入...");
						if (MainFormHandler.Instance.AddBatchServers(config, args.Msg) > 0)
						{
							int num2 = (config.vmess?.Count ?? 0) - nodeCountBefore;
							AppendText(notify: false, $"--> 成功导入 {num2} 个节点");
							InitSubscriptionTabs();
							RefreshServersView();
							ShowMsg($"从订阅链接导入了 {num2} 个节点");
						}
						else
						{
							AppendText(notify: false, "--> 订阅内容格式不正确，无法识别的节点格式");
							ShowMsg("订阅内容格式不正确");
						}
					}
				}
				else
				{
					AppendText(notify: false, "--> Base64解码失败，尝试直接导入原始内容...");
					if (MainFormHandler.Instance.AddBatchServers(config, args.Msg) > 0)
					{
						int num3 = (config.vmess?.Count ?? 0) - nodeCountBefore;
						AppendText(notify: false, $"--> 成功导入 {num3} 个节点");
						InitSubscriptionTabs();
						RefreshServersView();
						ShowMsg($"从订阅链接导入了 {num3} 个节点");
					}
					else
					{
						AppendText(notify: false, "--> 订阅内容为空或格式不正确");
						ShowMsg("订阅内容为空或格式不正确");
					}
				}
			}
			else
			{
				AppendText(notify: false, "--> 导入失败！" + args.Msg);
				ShowMsg("导入失败：" + args.Msg);
			}
		};
		downloadHandle.WebDownloadString(url);
	}

	private void menuSelectAll_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem item in lvServers.Items)
		{
			item.Selected = true;
		}
	}

	private void menuEditServer_Click(object sender, EventArgs e)
	{
		EditSelectedServer();
	}

	private void lvServers_DoubleClick(object sender, EventArgs e)
	{
		EditSelectedServer();
	}

	private void EditSelectedServer()
	{
		if (!config.allowEditServer)
		{
			return;
		}
		if (GetLvSelectedIndex() < 0)
		{
			UI.Show("请先选择需要编辑的节点", this);
			return;
		}
		if (lvSelecteds.Count > 1)
		{
			UI.Show("编辑功能仅支持单选，请只选择一个节点", this);
			return;
		}
		int num = lvSelecteds[0];
		if (num >= 0 && num < config.vmess.Count)
		{
			EditServerForm editServerForm = new EditServerForm(config.vmess[num], num);
			if (editServerForm.ShowDialog(this) == DialogResult.OK)
			{
				config.vmess[num] = editServerForm.ResultItem;
				RefreshServers();
				AppendText(notify: false, "节点 [" + editServerForm.ResultItem.remarks + "] 已更新");
			}
		}
	}

	private void menuRemoveServer_Click(object sender, EventArgs e)
	{
		if (GetLvSelectedIndex() >= 0 && UI.ShowYesNo("是否删除选中的节点？", this) != DialogResult.No)
		{
			for (int num = lvSelecteds.Count - 1; num >= 0; num--)
			{
				ConfigHandler.RemoveServer(ref config, lvSelecteds[num]);
			}
			RefreshServers();
		}
	}

	private void menuRemoveDuplicateServer_Click(object sender, EventArgs e)
	{
		Utils.DedupServerList(config.vmess, out var result, keepOlder: true);
		int count = config.vmess.Count;
		int count2 = result.Count;
		if (result != null)
		{
			config.vmess = result;
		}
		RefreshServers();
		UI.Show($"执行完成。已删除{count - count2}个重复节点", this);
	}

	private void menuExport2ShareUrl_Click(object sender, EventArgs e)
	{
		GetLvSelectedIndex();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int lvSelected in lvSelecteds)
		{
			string shareUrl = ShareHandler.GetShareUrl(config, lvSelected);
			if (!Utils.IsNullOrEmpty(shareUrl))
			{
				stringBuilder.Append(shareUrl);
				stringBuilder.AppendLine();
			}
		}
		if (stringBuilder.Length > 0)
		{
			Utils.SetClipboardData(stringBuilder.ToString());
			AppendText(notify: false, "节点URL已复制到剪贴板");
		}
	}

	private void menuExport2SubContent_Click(object sender, EventArgs e)
	{
		GetLvSelectedIndex();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int lvSelected in lvSelecteds)
		{
			string shareUrl = ShareHandler.GetShareUrl(config, lvSelected);
			if (!Utils.IsNullOrEmpty(shareUrl))
			{
				stringBuilder.Append(shareUrl);
				stringBuilder.AppendLine();
			}
		}
		if (stringBuilder.Length > 0)
		{
			Utils.SetClipboardData(Utils.Base64Encode(stringBuilder.ToString()));
			AppendText(notify: false, "节点订阅内容已复制到剪贴板");
		}
	}

	private void lvServers_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Control)
		{
			switch (e.KeyCode)
			{
			case Keys.A:
				menuSelectAll_Click(null, null);
				break;
			case Keys.C:
				menuExport2ShareUrl_Click(null, null);
				break;
			case Keys.V:
				menuAddServers_Click(null, null);
				break;
			case Keys.T:
				menuDownLoadServer_Click(null, null);
				break;
			case Keys.R:
				menuRealPingServer_Click(null, null);
				break;
			case Keys.E:
				menuAutoTestSelected_Click(null, null);
				break;
			}
		}
		else if (e.KeyCode == Keys.Delete)
		{
			menuRemoveServer_Click(null, null);
		}
	}

	private void lvServers_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		if (e.Column < 0)
		{
			return;
		}
		try
		{
			string value = lvServers.Columns[e.Column].Tag?.ToString();
			bool flag = Utils.IsNullOrEmpty(value) || !Convert.ToBoolean(value);
			// 根据列索引获取对应的枚举值（考虑隐藏列）
			EServerColName colName = GetServerColNameByIndex(e.Column);
			if (ConfigHandler.SortServers(ref config, colName, flag) != 0)
			{
				return;
			}
			lvServers.Columns[e.Column].Tag = Convert.ToString(flag);
			RefreshServers();
			SetSortIcon(e.Column, flag);
		}
		catch
		{
		}
		_ = e.Column;
	}

	private void SetSortIcon(int columnIndex, bool ascending)
	{
		try
		{
			IntPtr hWnd = SendMessage(lvServers.Handle, 4127u, IntPtr.Zero, IntPtr.Zero);
			for (int i = 0; i < lvServers.Columns.Count; i++)
			{
				HDITEM lParam = new HDITEM
				{
					mask = 4
				};
				SendMessageHDITEM(hWnd, 4619u, (IntPtr)i, ref lParam);
				lParam.fmt &= -1537;
				SendMessageHDITEM(hWnd, 4620u, (IntPtr)i, ref lParam);
			}
			if (columnIndex >= 0 && columnIndex < lvServers.Columns.Count)
			{
				HDITEM lParam2 = new HDITEM
				{
					mask = 4
				};
				SendMessageHDITEM(hWnd, 4619u, (IntPtr)columnIndex, ref lParam2);
				lParam2.fmt |= (ascending ? 1024 : 512);
				SendMessageHDITEM(hWnd, 4620u, (IntPtr)columnIndex, ref lParam2);
				currentSortColumn = columnIndex;
				currentSortAscending = ascending;
			}
		}
		catch
		{
		}
	}

	private void ClearSortIcon()
	{
		try
		{
			IntPtr hWnd = SendMessage(lvServers.Handle, 4127u, IntPtr.Zero, IntPtr.Zero);
			for (int i = 0; i < lvServers.Columns.Count; i++)
			{
				HDITEM lParam = new HDITEM
				{
					mask = 4
				};
				SendMessageHDITEM(hWnd, 4619u, (IntPtr)i, ref lParam);
				lParam.fmt &= -1537;
				SendMessageHDITEM(hWnd, 4620u, (IntPtr)i, ref lParam);
			}
			currentSortColumn = -1;
		}
		catch
		{
		}
	}

	private void tsbSubSetting_Click(object sender, EventArgs e)
	{
		new SubSettingForm().ShowDialog();
	}

	private void tsbSubUpdate_Click(object sender, EventArgs e)
	{
		UpdateSubscriptionProcess();
	}

	private void UpdateSubscriptionProcess()
	{
		new UpdateHandle().UpdateSubscriptionProcess(config, _updateUI);
		void _updateUI(bool success, string msg)
		{
			AppendText(notify: false, msg);
			if (success)
			{
				InitSubscriptionTabs();
				RefreshServers();
			}
		}
	}

	private void btnSaveConfig_Click(object sender, EventArgs e)
	{
		ConfigHandler.SaveConfig(ref config, reload: false);
		MessageBox.Show("配置文件保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	private void btnClearLog_Click(object sender, EventArgs e)
	{
		txtMsgBox.Clear();
	}

	private void btnClearResult_Click(object sender, EventArgs e)
	{
		int num = 0;
		foreach (ListViewItem item in lvServers.Items)
		{
			if (item.Tag != null)
			{
				int num2 = (int)item.Tag;
				if (num2 >= 0 && num2 < config.vmess.Count)
				{
					config.vmess[num2].httpsDelay = "";
					config.vmess[num2].testResult = "";
					config.vmess[num2].MaxSpeed = "";
					config.vmess[num2].lastTestTime = "";
					num++;
				}
			}
			item.SubItems["httpsDelay"].Text = "";
			item.SubItems["testResult"].Text = "";
			item.SubItems["MaxSpeed"].Text = "";
			if (config.recordTestTime && item.SubItems.ContainsKey("lastTestTime"))
			{
				item.SubItems["lastTestTime"].Text = "";
			}
		}
		AppendText(notify: false, $"已清空当前列表 {num} 个节点的测速结果");
	}

	private void button1_Click(object sender, EventArgs e)
	{
		cts.Cancel();
		btStopTest.Enabled = false;
		MessageBox.Show("已取消测速，请等待当前线程结束，否则软件将崩溃", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}

	private void btnStartTest_Click(object sender, EventArgs e)
	{
		if (!cbRealPing.Checked && !cbSpeedTest.Checked)
		{
			return;
		}
		if (btnStartTest.Text == "一键自动测速")
		{
			if (!DetectTunModeAndWarn())
			{
				AppendText(notify: false, "已取消测速（检测到TUN模式）");
				return;
			}
			AppendText(notify: false, "一键自动测速已开启");
			if (config.autoDedupEnabled)
			{
				Utils.DedupServerList(config.vmess, out var result, keepOlder: true);
				int count = config.vmess.Count;
				int count2 = result.Count;
				AppendText(notify: false, $"删除重复节点：{count - count2}个");
				if (result != null)
				{
					config.vmess = result;
				}
				RefreshServers();
			}
			btnStartTest.Text = "取消";
			Task.Run(delegate
			{
				AutoRun();
			});
		}
		else
		{
			cts.Cancel();
			MessageBox.Show("已取消测速，请等待当前线程结束，否则软件将崩溃", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			btnStartTest.Text = "一键自动测速";
		}
	}

	public void AutoRunSelected()
	{
		int pid = -1;
		cts = new CancellationTokenSource();
		CancellationToken token = cts.Token;
		List<int> list = new List<int>(lvSelecteds);
		if (list.Count == 0)
		{
			Invoke((Action)delegate
			{
				UI.Show("请先选择需要测速的服务器", this);
			});
			return;
		}
		Action<bool> action = delegate
		{
		};
		int num = 0;
		AppendText(notify: false, $"开始测试选中的 {list.Count} 个节点...");
		if (cbRealPing.Checked)
		{
			AppendText(notify: false, "开始 HTTPS 延迟测试...");
			config.index = 0;
			if (config.ThreadNum == 0)
			{
				try
				{
					createYamlConfig();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "出现异常");
					Invoke((Action)delegate
					{
						btStopTest.Enabled = false;
					});
					return;
				}
				speedtestNodeMap.Clear();
				foreach (int item in list)
				{
					if (item >= 0 && item < config.vmess.Count)
					{
						speedtestNodeMap[item] = config.vmess[item];
						// 使用 Invoke 确保在 UI 线程上更新
						lvServers.Invoke((MethodInvoker)delegate
						{
							SetHttpsDelay(item, "");
						});
					}
				}
				SpeedtestHandler httpsHandler = new SpeedtestHandler(ref config, ref cts, ref v2rayHandler, list, UpdateSpeedtestHandler, UpdateMaxSpeedHandler, UpdateHttpsDelayHandler, action, pid);
				if (config.ThreadNum == 0)
				{
					Thread thread = new Thread((ThreadStart)delegate
					{
						httpsHandler.RunRealPing(token);
					});
					thread.Start();
					thread.Join();
				}
				else
				{
					Thread thread2 = new Thread((ThreadStart)delegate
					{
						httpsHandler.RunRealPing2(token);
					});
					thread2.Start();
					thread2.Join();
				}
				RefreshServers();
				Thread.Sleep(200);
			}
			else
			{
				AppendText(notify: false, "单线程模式仅支持多线程测速，请将线程数设置为0");
			}
			Dictionary<string, int> dictionary = RemoveServer();
			num = dictionary.Values.Sum();
			if (num > 0)
			{
				AppendText(notify: false, $"移除无效服务器：共 {num} 个");
				foreach (KeyValuePair<string, int> item2 in dictionary)
				{
					AppendText(notify: false, $"  - {item2.Key}：{item2.Value} 个");
				}
				RefreshServers();
				Thread.Sleep(200);
			}
			else
			{
				AppendText(notify: false, "移除无效服务器：0 个");
			}
		}
		if (cbSpeedTest.Checked && !token.IsCancellationRequested)
		{
			List<int> list2 = new List<int>();
			if (cbRealPing.Checked && num > 0)
			{
				foreach (KeyValuePair<int, VmessItem> item3 in speedtestNodeMap)
				{
					VmessItem value = item3.Value;
					for (int num2 = 0; num2 < config.vmess.Count; num2++)
					{
						VmessItem vmessItem = config.vmess[num2];
						if (vmessItem.remarks == value.remarks && vmessItem.address == value.address && vmessItem.port == value.port)
						{
							list2.Add(num2);
							break;
						}
					}
				}
			}
			else
			{
				list2 = list;
			}
			if (list2.Count == 0)
			{
				AppendText(notify: false, "所有选中节点均已被删除，跳过速度测试");
				Invoke((Action)delegate
				{
					btStopTest.Enabled = false;
				});
				return;
			}
			AppendText(notify: false, $"开始下载速度测试...（共 {list2.Count} 个节点）");
			config.index = 0;
			if (config.DownloadThreadNum != 0)
			{
				AppendText(notify: false, "单线程模式仅支持多线程测速，请将线程数设置为0");
				Invoke((Action)delegate
				{
					btStopTest.Enabled = false;
				});
				return;
			}
			// 更新 lvSelecteds 为当前要测速的节点列表，确保配置文件中的代理名称与测速时使用的索引一致
			lvSelecteds = list2;
			try
			{
				createYamlConfig();
			}
			catch (Exception ex3)
			{
				MessageBox.Show(ex3.Message, "出现异常");
				Invoke((Action)delegate
				{
					btStopTest.Enabled = false;
				});
				return;
			}
			speedtestNodeMap.Clear();
			foreach (int item4 in list2)
			{
				if (item4 >= 0 && item4 < config.vmess.Count)
				{
					speedtestNodeMap[item4] = config.vmess[item4];
					// 使用 Invoke 确保在 UI 线程上更新
					lvServers.Invoke((MethodInvoker)delegate
					{
						SetTestResult(item4, "等待测速线程...");
					});
				}
			}
			SpeedtestHandler testSpeed = new SpeedtestHandler(ref config, ref cts, ref v2rayHandler, list2, UpdateSpeedtestHandler, UpdateMaxSpeedHandler, action, pid);
			if (config.DownloadThreadNum == 0)
			{
				Thread thread3 = new Thread((ThreadStart)delegate
				{
					testSpeed.RunSpeedTest(token);
				});
				thread3.Start();
				thread3.Join();
			}
			else
			{
				Thread thread4 = new Thread((ThreadStart)delegate
				{
					testSpeed.RunSpeedTest2(token);
				});
				thread4.Start();
				thread4.Join();
			}
			RefreshServers();
			if (!token.IsCancellationRequested)
			{
				Invoke((Action)delegate
				{
					SortListView();
				});
			}
		}
		if (!token.IsCancellationRequested)
		{
			Dictionary<string, int> dictionary2 = RemoveLowSpeedServers();
			int num3 = dictionary2.Values.Sum();
			if (num3 > 0)
			{
				AppendText(notify: false, $"移除低速服务器（阈值：{tbLowSpeed.Text.Trim()} MB/s）：共 {num3} 个");
				foreach (KeyValuePair<string, int> item5 in dictionary2)
				{
					AppendText(notify: false, $"  - {item5.Key}：{item5.Value} 个");
				}
			}
			else
			{
				AppendText(notify: false, "移除低速服务器（阈值：" + tbLowSpeed.Text.Trim() + " MB/s）：0 个");
			}
			RefreshServers();
		}
		AppendText(notify: false, "选中节点自动测速完成！");
		if (config.autoRemoveNoResultServer)
		{
			RemoveNoResultServers();
			RefreshServers();
		}
		Invoke((Action)delegate
		{
			btStopTest.Enabled = false;
		});
	}

	public void AutoRun()
	{
		int pid = -1;
		cts = new CancellationTokenSource();
		CancellationToken token = cts.Token;
		if (config.vmess == null || config.vmess.Count == 0)
		{
			Invoke((Action)delegate
			{
				UI.Show("请先选择需要测速的服务器", this);
				btnStartTest.Text = "一键自动测速";
			});
			return;
		}
		if (cbRealPing.Checked)
		{
			do
			{
				if (token.IsCancellationRequested)
				{
					return;
				}
				foreach (ListViewItem item in lvServers.Items)
				{
					item.Selected = true;
				}
				if (GetLvSelectedIndex() < 0)
				{
					Invoke((Action)delegate
					{
						btnStartTest.Text = "一键自动测速";
					});
					return;
				}
				AppendText(notify: false, "开始 HTTPS 延迟测试...");
				config.index = 0;
				if (config.ThreadNum == 0)
				{
					try
					{
						createYamlConfig();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "出现异常");
						btnStartTest.Text = "一键自动测速";
						return;
					}
					speedtestNodeMap.Clear();
					foreach (int lvSelected in lvSelecteds)
					{
						if (lvSelected >= 0 && lvSelected < config.vmess.Count)
						{
							speedtestNodeMap[lvSelected] = config.vmess[lvSelected];
						}
					}
					Task.Run(delegate
					{
						foreach (int lvSelected2 in lvSelecteds)
						{
							// 使用 Invoke 确保在 UI 线程上更新
							lvServers.Invoke((MethodInvoker)delegate
							{
								SetHttpsDelay(lvSelected2, "");
							});
						}
					}).Wait();
					SpeedtestHandler statistics = new SpeedtestHandler(ref config, ref cts, ref v2rayHandler, lvSelecteds, UpdateSpeedtestHandler, UpdateMaxSpeedHandler, UpdateHttpsDelayHandler, btStopTestStat, pid);
					if (config.ThreadNum == 0)
					{
						Thread thread = new Thread((ThreadStart)delegate
						{
							statistics.RunRealPing(token);
						});
						thread.Start();
						thread.Join();
					}
					else
					{
						Thread thread2 = new Thread((ThreadStart)delegate
						{
							statistics.RunRealPing2(token);
						});
						thread2.Start();
						thread2.Join();
					}
					Dictionary<string, int> dictionary = RemoveServer();
					int num = dictionary.Values.Sum();
					if (num > 0)
					{
						AppendText(notify: false, $"移除无效服务器：共 {num} 个");
						foreach (KeyValuePair<string, int> item2 in dictionary)
						{
							AppendText(notify: false, $"  - {item2.Key}：{item2.Value} 个");
						}
					}
					else
					{
						AppendText(notify: false, "移除无效服务器：0 个");
					}
					Thread.Sleep(200);
					RefreshServers();
					Thread.Sleep(200);
					pid = -1;
					continue;
				}
				AppendText(notify: false, "单线程模式仅支持多线程测速，请将线程数设置为0");
				btnStartTest.Text = "一键自动测速";
				return;
			}
			while (lvServers.Items.Count > Convert.ToInt32(config.PingNum));
		}
		if (cbSpeedTest.Checked && lvServers.Items.Count > 0)
		{
			foreach (ListViewItem item3 in lvServers.Items)
			{
				item3.Selected = true;
			}
			if (GetLvSelectedIndex() < 0)
			{
				Invoke((Action)delegate
				{
					btnStartTest.Text = "一键自动测速";
				});
				return;
			}
			config.index = 0;
			if (config.DownloadThreadNum != 0)
			{
				AppendText(notify: false, "单线程模式仅支持多线程测速，请将线程数设置为0");
				btnStartTest.Text = "一键自动测速";
				return;
			}
			try
			{
				createYamlConfig();
			}
			catch (Exception ex3)
			{
				MessageBox.Show(ex3.Message, "出现异常");
				return;
			}
			speedtestNodeMap.Clear();
			foreach (int lvSelected3 in lvSelecteds)
			{
				if (lvSelected3 >= 0 && lvSelected3 < config.vmess.Count)
				{
					speedtestNodeMap[lvSelected3] = config.vmess[lvSelected3];
				}
			}
			Task.Run(delegate
			{
				foreach (int lvSelected4 in lvSelecteds)
				{
					// 使用 Invoke 确保在 UI 线程上更新
					lvServers.Invoke((MethodInvoker)delegate
					{
						SetTestResult(lvSelected4, "等待测速线程...");
					});
				}
			}).Wait();
			SpeedtestHandler testSpeed = new SpeedtestHandler(ref config, ref cts, ref v2rayHandler, lvSelecteds, UpdateSpeedtestHandler, UpdateMaxSpeedHandler, btStopTestStat, pid);
			if (config.DownloadThreadNum == 0)
			{
				Thread thread3 = new Thread((ThreadStart)delegate
				{
					testSpeed.RunSpeedTest(token);
				});
				thread3.Start();
				thread3.Join();
			}
			else
			{
				Thread thread4 = new Thread((ThreadStart)delegate
				{
					testSpeed.RunSpeedTest2(token);
				});
				thread4.Start();
				thread4.Join();
			}
			RefreshServers();
			if (!token.IsCancellationRequested)
			{
				// 按 HTTPS延迟 列排序
				int httpsDelayColIndex = GetColumnIndexByName("HTTPS延迟");
				if (httpsDelayColIndex >= 0)
				{
					lvServers.Columns[httpsDelayColIndex].Tag = true;
					lvServers_ColumnClick(null, new ColumnClickEventArgs(httpsDelayColIndex));
				}
				Dictionary<string, int> dictionary2 = RemoveServer();
				int num2 = dictionary2.Values.Sum();
				if (num2 > 0)
				{
					AppendText(notify: false, $"移除无效服务器：共 {num2} 个");
					foreach (KeyValuePair<string, int> item4 in dictionary2)
					{
						AppendText(notify: false, $"  - {item4.Key}：{item4.Value} 个");
					}
				}
				else
				{
					AppendText(notify: false, "移除无效服务器：0 个");
				}
				RefreshServers();
				Thread.Sleep(200);
				Dictionary<string, int> dictionary3 = RemoveLowSpeedServers();
				int num3 = dictionary3.Values.Sum();
				if (num3 > 0)
				{
					AppendText(notify: false, $"移除低速服务器（阈值：{tbLowSpeed.Text.Trim()} MB/s）：共 {num3} 个");
					foreach (KeyValuePair<string, int> item5 in dictionary3)
					{
						AppendText(notify: false, $"  - {item5.Key}：{item5.Value} 个");
					}
				}
				else
				{
					AppendText(notify: false, "移除低速服务器（阈值：" + tbLowSpeed.Text.Trim() + " MB/s）：0 个");
				}
				RefreshServers();
			}
		}
		AppendText(notify: false, "自动测速完成！");
		if (config.autoRemoveNoResultServer)
		{
			RemoveNoResultServers();
			RefreshServers();
		}
		if (!token.IsCancellationRequested)
		{
			Invoke((Action)delegate
			{
				SortListView();
			});
		}
		Invoke((Action)delegate
		{
			btnStartTest.Text = "一键自动测速";
			if (!token.IsCancellationRequested)
			{
				if (config.autoSaveAfterTest)
				{
					ConfigHandler.SaveConfig(ref config, reload: false);
				}
				else if (MessageBox.Show(this, "是否保存当前测试结果？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					btnSaveConfig_Click(null, null);
				}
			}
		});
	}

	public Dictionary<string, int> RemoveLowSpeedServers()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		if (!double.TryParse(tbLowSpeed.Text.Trim(), out var result))
		{
			return dictionary;
		}
		List<int> list = new List<int>();
		for (int num = lvServers.Items.Count - 1; num >= 0; num--)
		{
			string text = lvServers.Items[num].SubItems["testResult"].Text;
			if (text.IndexOf("MB/s") != -1)
			{
				double num2 = Convert.ToDouble(text.Trim().Substring(0, text.Trim().Length - 5));
				if (result > num2)
				{
					if (lvServers.Items[num].Tag != null)
					{
						int item = (int)lvServers.Items[num].Tag;
						list.Add(item);
					}
					string key = ((num2 < 0.1) ? "速度 < 0.1 MB/s" : ((num2 < 0.5) ? "速度 0.1-0.5 MB/s" : ((!(num2 < 1.0)) ? $"速度 1.0-{result:F1} MB/s" : "速度 0.5-1.0 MB/s")));
					if (dictionary.ContainsKey(key))
					{
						dictionary[key]++;
					}
					else
					{
						dictionary[key] = 1;
					}
				}
			}
		}
		list.Sort((int a, int b) => b.CompareTo(a));
		foreach (int item2 in list)
		{
			config.vmess.RemoveAt(item2);
		}
		return dictionary;
	}

	public string GetRealPingTime(string url, WebProxy webProxy, out int responseTime)
	{
		string result = string.Empty;
		responseTime = -1;
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
			obj.Timeout = Convert.ToInt32(tbTimeout.Text.Trim()) * 1000;
			obj.Proxy = webProxy;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			HttpWebResponse httpWebResponse = (HttpWebResponse)obj.GetResponse();
			if (httpWebResponse.StatusCode != HttpStatusCode.OK && httpWebResponse.StatusCode != HttpStatusCode.NoContent)
			{
				result = httpWebResponse.StatusDescription;
			}
			stopwatch.Stop();
			responseTime = stopwatch.Elapsed.Milliseconds;
			httpWebResponse.Close();
		}
		catch (Exception ex)
		{
			result = ex.Message;
		}
		return result;
	}

	public string FormatOut(object time, string unit)
	{
		if (time.ToString().Equals("-1"))
		{
			return "Timeout";
		}
		return $"{time}{unit}";
	}

	public Dictionary<string, int> RemoveServer()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		List<int> list = new List<int>();
		for (int num = lvServers.Items.Count - 1; num >= 0; num--)
		{
			string text = null;
			string text3 = lvServers.Items[num].SubItems["httpsDelay"].Text;
			string text4 = lvServers.Items[num].SubItems["testResult"].Text;
			
			// 检查是否为等待/测试中状态
			bool httpsDelayPending = string.IsNullOrEmpty(text3) || text3 == "测速被取消" || text3 == "等待测速线程..." || text3 == "正在测速...";
			bool testResultPending = string.IsNullOrEmpty(text4) || text4 == "等待测速线程..." || text4 == "测速被取消" || text4 == "正在测试...";
			
			// 检查测试是否失败
			bool httpsDelayFailed = !httpsDelayPending && text3.IndexOf("ms") == -1;
			bool testResultFailed = !testResultPending && text4.IndexOf("MB/s") == -1 && text4.IndexOf("KB/s") == -1;
			
			// 检查是否为请求被中止/取消的错误
			bool testResultAborted = !testResultPending && (text4.Contains("请求被中止") || text4.Contains("请求已被取消") || text4.Contains("Aborted") || text4.Contains("Canceled"));
			
			if (config.strictExclusionMode)
			{
				// 严格模式：HTTPS延迟失败 或 下载测速失败，满足一个就删除
				if (httpsDelayFailed)
				{
					text = "HTTPS延迟 测试失败";
				}
				else if (testResultFailed)
				{
					text = ((!text4.Contains("超时") && !text4.Contains("Timeout")) ? ((!text4.Contains("连接失败") && !text4.Contains("无法连接")) ? "平均速度测试失败" : "平均速度测试连接失败") : "平均速度测试超时");
				}
			}
			else
			{
				// 非严格模式：HTTPS延迟失败 且 下载测速也失败才删除
				if (httpsDelayFailed && testResultFailed)
				{
					text = "HTTPS延迟和平均速度 均测试失败";
				}
				else if (httpsDelayFailed && testResultPending)
				{
					// HTTPS失败但下载测速未进行，视为失败
					text = "HTTPS延迟 测试失败";
				}
				else if (httpsDelayPending && testResultFailed)
				{
					// HTTPS未测试但下载测速失败，视为失败
					text = ((!text4.Contains("超时") && !text4.Contains("Timeout")) ? ((!text4.Contains("连接失败") && !text4.Contains("无法连接")) ? "平均速度测试失败" : "平均速度测试连接失败") : "平均速度测试超时");
				}
				else if (testResultAborted)
				{
					// 请求被中止/取消的情况，视为失败
					text = "平均速度测试请求被中止";
				}
			}
			
			if (text != null)
			{
				if (lvServers.Items[num].Tag != null)
				{
					int item = (int)lvServers.Items[num].Tag;
					list.Add(item);
				}
				if (dictionary.ContainsKey(text))
				{
					dictionary[text]++;
				}
				else
				{
					dictionary[text] = 1;
				}
			}
		}
		list.Sort((int a, int b) => b.CompareTo(a));
		foreach (int item2 in list)
		{
			config.vmess.RemoveAt(item2);
		}
		return dictionary;
	}

	private void menuRemoveLowServer_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("是否移除速度低于 " + tbLowSpeed.Text + "M/s 的节点?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
		{
			return;
		}
		Dictionary<string, int> dictionary = RemoveLowSpeedServers();
		int num = dictionary.Values.Sum();
		if (num > 0)
		{
			AppendText(notify: false, $"移除低速服务器（阈值：{tbLowSpeed.Text.Trim()} MB/s）：共 {num} 个");
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				AppendText(notify: false, $"  - {item.Key}：{item.Value} 个");
			}
			ConfigHandler.SaveConfig(ref config, reload: false);
		}
		else
		{
			AppendText(notify: false, "移除低速服务器（阈值：" + tbLowSpeed.Text.Trim() + " MB/s）：0 个");
		}
		RefreshServers();
	}

	private void menuRemoveLoseServer_Click(object sender, EventArgs e)
	{
		Dictionary<string, int> dictionary = RemoveServer();
		int num = dictionary.Values.Sum();
		if (num > 0)
		{
			AppendText(notify: false, $"移除无效服务器：共 {num} 个");
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				AppendText(notify: false, $"  - {item.Key}：{item.Value} 个");
			}
			ConfigHandler.SaveConfig(ref config, reload: false);
		}
		else
		{
			AppendText(notify: false, "移除无效服务器：0 个");
		}
		RefreshServers();
	}

	private void menuRemoveNoResultServer_Click(object sender, EventArgs e)
	{
		if (RemoveNoResultServers() > 0)
		{
			ConfigHandler.SaveConfig(ref config, reload: false);
		}
		RefreshServers();
	}

	public int RemoveNoResultServers()
	{
		int num = 0;
		for (int num2 = config.vmess.Count - 1; num2 >= 0; num2--)
		{
			if (string.IsNullOrWhiteSpace(config.vmess[num2].testResult))
			{
				config.vmess.RemoveAt(num2);
				num++;
			}
		}
		if (num > 0)
		{
			AppendText(notify: false, $"移除无结果节点：共 {num} 个");
		}
		else
		{
			AppendText(notify: false, "移除无结果节点：0 个");
		}
		return num;
	}

	private void tbTimeout_Leave(object sender, EventArgs e)
	{
		config.Timeout = tbTimeout.Text;
	}

	private void tbLowSpeed_TextChanged(object sender, EventArgs e)
	{
		config.LowSpeed = tbLowSpeed.Text.Trim();
	}

	private void tbPingNum_TextChanged(object sender, EventArgs e)
	{
		config.PingNum = tbPingNum.Text.Trim();
	}

	private void tbTimeout_TextChanged(object sender, EventArgs e)
	{
		config.Timeout = tbTimeout.Text.Trim();
	}

	private void tbKeywordFilter_TextChanged(object sender, EventArgs e)
	{
		config.keywordFilter = tbKeywordFilter.Text;
		if (config.keywordFilterEnabled)
		{
			RefreshServersView();
		}
	}

	private void cbKeywordFilter_CheckedChanged(object sender, EventArgs e)
	{
		bool flag = cbKeywordFilter.Checked;
		config.keywordFilterEnabled = flag;
		tbKeywordFilter.Enabled = flag;
		RefreshServersView();
	}

	private void btnKeywordPreset_Click(object sender, EventArgs e)
	{
		UpdateKeywordPresetCheckStates();
		cmsKeywordPreset.Show(btnKeywordPreset, new Point(0, btnKeywordPreset.Height));
	}

	private void KeywordPreset_Click(object sender, EventArgs e)
	{
		if (!(sender is ToolStripMenuItem toolStripMenuItem))
		{
			return;
		}
		if (toolStripMenuItem.Name == "CLEAR")
		{
			tbKeywordFilter.Text = string.Empty;
			UpdateKeywordPresetCheckStates();
		}
		else
		{
			if (!(toolStripMenuItem.Tag is string text) || string.IsNullOrEmpty(text))
			{
				return;
			}
			string text2 = tbKeywordFilter.Text?.Trim() ?? string.Empty;
			List<string> keywords = new List<string>();
			if (!string.IsNullOrEmpty(text2))
			{
				keywords.AddRange(text2.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
			}
			string[] array = text.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.All((string pk) => keywords.Contains(pk)))
			{
				string[] array2 = array;
				foreach (string item in array2)
				{
					keywords.Remove(item);
				}
			}
			else
			{
				string[] array2 = array;
				foreach (string item2 in array2)
				{
					if (!keywords.Contains(item2))
					{
						keywords.Add(item2);
					}
				}
			}
			tbKeywordFilter.Text = string.Join(",", keywords);
			UpdateKeywordPresetCheckStates();
			cmsKeywordPreset.Show(btnKeywordPreset, new Point(0, btnKeywordPreset.Height));
		}
	}

	private void UpdateKeywordPresetCheckStates()
	{
		string text = tbKeywordFilter.Text?.Trim() ?? string.Empty;
		List<string> keywords = new List<string>();
		if (!string.IsNullOrEmpty(text))
		{
			keywords.AddRange(text.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
		}
		foreach (ToolStripItem item in cmsKeywordPreset.Items)
		{
			if (item is ToolStripMenuItem toolStripMenuItem && item.Tag is string text2 && !string.IsNullOrEmpty(text2))
			{
				string[] source = text2.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				toolStripMenuItem.Checked = source.All((string pk) => keywords.Contains(pk));
			}
		}
	}

	private ToolStripMenuItem CreatePresetMenuItem(string displayText, string keyword, string name = null)
	{
		ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(displayText);
		toolStripMenuItem.Tag = keyword;
		toolStripMenuItem.Name = name ?? string.Empty;
		toolStripMenuItem.Click += KeywordPreset_Click;
		return toolStripMenuItem;
	}

	public void InitSubscriptionTabs()
	{
		InitSubscriptionTabs(restoreSelection: true);
	}

	public void InitSubscriptionTabs(bool restoreSelection)
	{
		if (tabSubscriptions == null)
		{
			return;
		}
		string text = null;
		if (restoreSelection && tabSubscriptions.SelectedTab != null)
		{
			text = tabSubscriptions.SelectedTab.Tag as string;
		}
		tabSubscriptions.TabPages.Clear();
		TabPage tabPage = new TabPage("所有节点");
		tabPage.Tag = null;
		tabSubscriptions.TabPages.Add(tabPage);
		if (config.vmess != null && config.vmess.Any((VmessItem v) => string.IsNullOrEmpty(v.subid)))
		{
			TabPage tabPage2 = new TabPage("无分组");
			tabPage2.Tag = "__unassigned__";
			tabSubscriptions.TabPages.Add(tabPage2);
		}
		if (config.subItem != null)
		{
			foreach (SubItem item in config.subItem)
			{
				if (item.enabled)
				{
					TabPage tabPage3 = new TabPage(item.remarks);
					tabPage3.Tag = item.id;
					tabSubscriptions.TabPages.Add(tabPage3);
				}
			}
		}
		if (tabSubscriptions.TabPages.Count <= 0)
		{
			return;
		}
		if (restoreSelection && text != null)
		{
			bool flag = false;
			for (int num = 0; num < tabSubscriptions.TabPages.Count; num++)
			{
				if (tabSubscriptions.TabPages[num].Tag as string == text)
				{
					tabSubscriptions.SelectedIndex = num;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				tabSubscriptions.SelectedIndex = 0;
			}
		}
		else if (restoreSelection && text == null)
		{
			tabSubscriptions.SelectedIndex = 0;
		}
		else
		{
			tabSubscriptions.SelectedIndex = 0;
		}
	}

	private void tabSubscriptions_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (tabSubscriptions.SelectedTab != null)
		{
			currentSubFilter = tabSubscriptions.SelectedTab.Tag as string;
			RefreshServersView();
		}
	}

	private void UpdateUnassignedTabVisibility()
	{
		if (tabSubscriptions == null)
		{
			return;
		}
		bool flag = config.vmess != null && config.vmess.Any((VmessItem v) => string.IsNullOrEmpty(v.subid));
		TabPage tabPage = null;
		for (int num = 0; num < tabSubscriptions.TabPages.Count; num++)
		{
			if (tabSubscriptions.TabPages[num].Tag as string == "__unassigned__")
			{
				tabPage = tabSubscriptions.TabPages[num];
				break;
			}
		}
		if (flag && tabPage == null)
		{
			TabPage tabPage2 = new TabPage("无分组");
			tabPage2.Tag = "__unassigned__";
			tabSubscriptions.TabPages.Insert(1, tabPage2);
		}
		else if (!flag && tabPage != null)
		{
			if (tabSubscriptions.SelectedTab == tabPage)
			{
				tabSubscriptions.SelectedIndex = 0;
			}
			tabSubscriptions.TabPages.Remove(tabPage);
		}
	}

	private void lvServers_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
	{
		try
		{
			EServerColName colName = GetServerColNameByIndex(e.ColumnIndex);
			string key = colName.ToString();
			
			// 如果列被隐藏，不保存宽度变化
			if (!IsColumnVisible(key))
				return;
			
			int columnHeaderMinWidth = GetColumnHeaderMinWidth(e.ColumnIndex);
			if (columnHeaderMinWidth > 0 && lvServers.Columns[e.ColumnIndex].Width < columnHeaderMinWidth)
			{
				lvServers.Columns[e.ColumnIndex].Width = columnHeaderMinWidth;
			}
			if (config.uiItem?.mainLvColWidth != null)
			{
				if (config.uiItem.mainLvColWidth.ContainsKey(key))
				{
					config.uiItem.mainLvColWidth[key] = lvServers.Columns[e.ColumnIndex].Width;
				}
			}
		}
		catch
		{
		}
	}

	private void lvServers_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
	{
		// 如果列当前是隐藏的（宽度为0），不允许用户调整
		EServerColName colName = GetServerColNameByIndex(e.ColumnIndex);
		if (!IsColumnVisible(colName.ToString()))
		{
			e.NewWidth = 0;
			e.Cancel = true;
			return;
		}
		
		if (e.NewWidth < 0)
		{
			int num = CalculateAutoColumnWidth(e.ColumnIndex);
			if (num > 0)
			{
				lvServers.Columns[e.ColumnIndex].Width = num;
				e.Cancel = true;
			}
		}
		else
		{
			int columnHeaderMinWidth = GetColumnHeaderMinWidth(e.ColumnIndex);
			if (columnHeaderMinWidth > 0 && e.NewWidth < columnHeaderMinWidth)
			{
				lvServers.Columns[e.ColumnIndex].Width = columnHeaderMinWidth;
				e.Cancel = true;
			}
		}
	}

	private void tssTool_Click(object sender, EventArgs e)
	{
		AboutForm aboutForm = new AboutForm();
		aboutForm.Owner = this;
		aboutForm.ShowDialog();
	}

	private void toolStripButton1_Click(object sender, EventArgs e)
	{
		Process.Start("https://bulianglin.com/archives/nodescatch.html");
	}

	private void menuExport2Base64_Click(object sender, EventArgs e)
	{
		GetLvSelectedIndex();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int lvSelected in lvSelecteds)
		{
			string shareUrl = ShareHandler.GetShareUrl(config, lvSelected);
			if (!Utils.IsNullOrEmpty(shareUrl))
			{
				stringBuilder.Append(shareUrl);
				stringBuilder.AppendLine();
			}
		}
		if (stringBuilder.Length > 0)
		{
			string path = Utils.ShowSaveFileDialog("文本文档（*.txt）|*.txt");
			if (!Utils.IsNullOrEmpty(path))
			{
				File.WriteAllText(path, Utils.Base64Encode(stringBuilder.ToString()), Encoding.UTF8);
				AppendText(notify: false, "保存成功");
			}
		}
	}

	private void menuExport2Clash_Click(object sender, EventArgs e)
	{
		GetLvSelectedIndex();
		if (!ConfigHandler.TcpClientCheck("127.0.0.1", 25500))
		{
			MessageBox.Show($"无法连接到Subconverter(端口:{25500})，请确认是否启动！建议重新打开测速软件");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int lvSelected in lvSelecteds)
		{
			string shareUrl = ShareHandler.GetShareUrl(config, lvSelected);
			if (!Utils.IsNullOrEmpty(shareUrl))
			{
				stringBuilder.Append(shareUrl);
				stringBuilder.AppendLine();
			}
		}
		if (stringBuilder.Length <= 0)
		{
			return;
		}
		string text = Guid.NewGuid().ToString("N");
		string path = Utils.GetPath("subconverter\\temp_" + text + ".txt");
		try
		{
			stringBuilder.Insert(0, "ss://YWVzLTI1Ni1nY206ZmFCQW9ENTRrODdVSkc3QDEuMS4xLjE6NjY2#%e5%8d%a0%e4%bd%8d%e8%8a%82%e7%82%b9" + Environment.NewLine);
			string path2 = Utils.ShowSaveFileDialog("Mihomo配置文件（*.yaml）|*.yaml");
			if (!Utils.IsNullOrEmpty(path2))
			{
				File.WriteAllText(path, Utils.Base64Encode(stringBuilder.ToString()), Encoding.UTF8);
				HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://127.0.0.1:{25500}/sub?target=clash&url=temp_{text}.txt&insert=false");
				obj.Method = "GET";
				obj.Timeout = 10000;
				obj.ReadWriteTimeout = 10000;
				obj.ContinueTimeout = 10000;
				using StreamReader streamReader = new StreamReader(obj.GetResponse().GetResponseStream(), Encoding.UTF8);
				string contents = streamReader.ReadToEnd();
				File.WriteAllText(path2, contents, Encoding.UTF8);
				AppendText(notify: false, "保存文件成功");
				return;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("导出配置文件失败！返回异常：" + ex.Message);
		}
		finally
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			catch
			{
			}
		}
	}

	private void menuStartClash_Click(object sender, EventArgs e)
	{
		GetLvSelectedIndex();
		if (!ConfigHandler.TcpClientCheck("127.0.0.1", config.externalControllerPort))
		{
			MessageBox.Show("Mihomo外部控制端口没有开启，请确认端口是否正确！建议重新打开测速软件");
			return;
		}
		if (!ConfigHandler.TcpClientCheck("127.0.0.1", 25500))
		{
			MessageBox.Show($"无法连接到Subconverter(端口:{25500})，请确认是否启动！建议重新打开测速软件");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int lvSelected in lvSelecteds)
		{
			string shareUrl = ShareHandler.GetShareUrl(config, lvSelected);
			if (!Utils.IsNullOrEmpty(shareUrl))
			{
				stringBuilder.Append(shareUrl);
				stringBuilder.AppendLine();
			}
		}
		if (stringBuilder.Length <= 0)
		{
			return;
		}
		string text = Guid.NewGuid().ToString("N");
		string path = Utils.GetPath("subconverter\\temp_" + text + ".txt");
		try
		{
			stringBuilder.Insert(0, "ss://YWVzLTI1Ni1nY206ZmFCQW9ENTRrODdVSkc3QDEuMS4xLjE6NjY2#%e5%8d%a0%e4%bd%8d%e8%8a%82%e7%82%b9" + Environment.NewLine);
			string path2 = Utils.GetPath("config\\temp.yaml");
			File.WriteAllText(path, Utils.Base64Encode(stringBuilder.ToString()), Encoding.UTF8);
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://127.0.0.1:{25500}/sub?target=clash&url=temp_{text}.txt&insert=false");
			obj.Method = "GET";
			obj.Timeout = 10000;
			obj.ReadWriteTimeout = 10000;
			obj.ContinueTimeout = 10000;
			using (StreamReader streamReader = new StreamReader(obj.GetResponse().GetResponseStream(), Encoding.UTF8))
			{
				string contents = streamReader.ReadToEnd();
				File.WriteAllText(path2, contents, Encoding.UTF8);
				AppendText(notify: false, "Mihomo配置文件生成成功，正在推送配置文件到Mihomo内核...");
			}
			string args = JsonConvert.SerializeObject(new
			{
				path = Path.GetFullPath(path2).Replace("\\", "/")
			}, Formatting.None);
			string uri = "http://127.0.0.1:" + config.externalControllerPort + "/configs";
			if (ConfigHandler.sendReq(args, uri, "PUT") != "204")
			{
				AppendText(notify: false, "切换YAML配置文件失败，请确认Mihomo是否启动");
			}
			else
			{
				AppendText(notify: false, "节点推送到Mihomo内核成功！");
			}
		}
		catch (Exception ex)
		{
			AppendText(notify: false, "切换YAML配置文件失败，返回异常：" + ex.Message);
		}
		finally
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			catch
			{
			}
		}
	}

	private void tbClashPort_TextChanged(object sender, EventArgs e)
	{
		config.ClashPort = tbClashPort.Text.Trim();
	}

	private void cbFastMode_CheckedChanged(object sender, EventArgs e)
	{
		config.fastMode = cbFastMode.Checked;
		tb_fm_second.Enabled = config.fastMode;
		tb_fm_max.Enabled = config.fastMode;
		tb_fm_ave.Enabled = config.fastMode;
		if (config.fastMode)
		{
			MessageBox.Show("快速模式在测下载速度时：如果" + tb_fm_second.Text + "秒内峰值速度没有达到" + tb_fm_max.Text + "MB/s，或者下载进度没有超过" + tb_fm_ave.Text + "%，则取消当前节点测速", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}

	private void tb_fm_ave_TextChanged(object sender, EventArgs e)
	{
		config.FMave = tb_fm_ave.Text.Trim();
	}

	private void tb_fm_max_TextChanged(object sender, EventArgs e)
	{
		config.FMmax = tb_fm_max.Text.Trim();
	}

	private void tb_fm_second_TextChanged(object sender, EventArgs e)
	{
		config.FMSecond = tb_fm_second.Text.Trim();
	}

	private void cbSpeedTest_CheckedChanged(object sender, EventArgs e)
	{
		config.speedAble = cbSpeedTest.Checked;
	}

	private void cbRealPing_CheckedChanged(object sender, EventArgs e)
	{
		TextBox textBox = tbPingNum;
		bool flag = (config.pingAble = cbRealPing.Checked);
		bool enabled = flag;
		textBox.Enabled = enabled;
	}

	private void cbAutoSort_CheckedChanged(object sender, EventArgs e)
	{
		bool flag = cbAutoSort.Checked;
		config.autoSortEnabled = flag;
		cmbSortColumn.Enabled = flag;
		cmbSortOrder.Enabled = flag;
	}

	private void cmbSortColumn_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (config != null)
		{
			config.autoSortColumn = cmbSortColumn.SelectedIndex;
		}
	}

	private void cmbSortOrder_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (config != null)
		{
			config.autoSortOrder = cmbSortOrder.SelectedIndex;
		}
	}

	private void cbStrictMode_CheckedChanged(object sender, EventArgs e)
	{
		config.strictExclusionMode = cbStrictMode.Checked;
	}

	private void cbAutoSave_CheckedChanged(object sender, EventArgs e)
	{
		config.autoSaveAfterTest = cbAutoSave.Checked;
	}

	private void SortListView()
	{
		if (!cbAutoSort.Checked || cmbSortColumn.SelectedIndex < 0 || cmbSortOrder.SelectedIndex < 0)
		{
			return;
		}
		EServerColName name = cmbSortColumn.SelectedIndex switch
		{
			0 => EServerColName.testResult, 
			1 => EServerColName.MaxSpeed, 
			2 => EServerColName.httpsDelay, 
			3 => EServerColName.lastTestTime, 
			_ => EServerColName.testResult, 
		};
		bool flag = cmbSortOrder.SelectedIndex == 1;
		try
		{
			ConfigHandler.SortServers(ref config, name, flag);
			RefreshServers();
			SetSortIcon(GetSortColumnIndex(), flag);
			AppendText(notify: false, "自动排序完成：按 " + cmbSortColumn.Text + " " + cmbSortOrder.Text);
		}
		catch (Exception ex)
		{
			AppendText(notify: false, "排序失败：" + ex.Message);
		}
	}

	private int GetSortColumnIndex()
	{
		// 根据下拉框选择获取对应的列名，然后查找实际的列索引
		string columnText = cmbSortColumn.SelectedIndex switch
		{
			0 => "平均速度",
			1 => "峰值速度",
			2 => "HTTPS延迟",
			3 => "最后测速",
			_ => "平均速度",
		};
		return GetColumnIndexByName(columnText);
	}

	private double GetSortValue(ListViewItem item, int columnIndex)
	{
		if (item.SubItems.Count <= columnIndex)
		{
			return double.MaxValue;
		}
		string text = item.SubItems[columnIndex].Text;
		if (string.IsNullOrEmpty(text) || text == "-" || text == "")
		{
			return double.MaxValue;
		}
		try
		{
			string text2 = Regex.Replace(text, "[^\\d.-]", "");
			if (string.IsNullOrEmpty(text2))
			{
				return double.MaxValue;
			}
			return double.Parse(text2);
		}
		catch
		{
			return double.MaxValue;
		}
	}

	private void MainForm_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.All;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void MainForm_DragDrop(object sender, DragEventArgs e)
	{
		string[] obj = (string[])e.Data.GetData(DataFormats.FileDrop);
		int num = 0;
		string[] array = obj;
		for (int i = 0; i < array.Length; i++)
		{
			string clipboardData = File.ReadAllText(array[i], Encoding.UTF8);
			num += MainFormHandler.Instance.AddBatchServers(config, clipboardData);
		}
		if (num > 0)
		{
			RefreshServers();
			AppendText(notify: false, $"成功从文件中导入{num}个节点");
		}
		else
		{
			UI.Show("未能成功解析节点，请检查文件内容！", this);
		}
	}

	private void tbThread_TextChanged(object sender, EventArgs e)
	{
		config.Thread = tbThread.Text.Trim();
	}

	private void button1_Click_1(object sender, EventArgs e)
	{
		contextMenuStrip1.Show(button1, 0, 0);
	}

	private void 导入到节点列表ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		UpdateSubscriptionProcess();
	}

	private void 订阅列表ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		new SubSettingForm().ShowDialog();
	}

	private void button2_Click(object sender, EventArgs e)
	{
		AboutForm aboutForm = new AboutForm();
		aboutForm.Owner = this;
		aboutForm.ShowDialog();
	}

	private void btnSettings_Click(object sender, EventArgs e)
	{
		if (new SettingsForm(config)
		{
			Owner = this
		}.ShowDialog() == DialogResult.OK)
		{
			// 重新初始化列表视图以应用列可见性设置
			ReinitializeServersView();
			AppendText(notify: false, "设置已生效（需手动保存配置）");
		}
	}

	private void ReinitializeServersView()
	{
		lvServers.BeginUpdate();
		lvServers.Columns.Clear();
		lvServers.Items.Clear();
		lvServers.EndUpdate();
		// 重新获取列顺序
		currentColumnOrder = null;
		InitServersView();
		RefreshServers();
	}

	private void UpdateLastTestTimeColumnVisibility()
	{
		lvServers.BeginUpdate();
		bool hasLastTestTimeColumn = false;
		for (int i = 0; i < lvServers.Columns.Count; i++)
		{
			if (lvServers.Columns[i].Text == "最后测速")
			{
				hasLastTestTimeColumn = true;
				break;
			}
		}
		
		if (config.recordTestTime)
		{
			if (!hasLastTestTimeColumn)
			{
				lvServers.Columns.Add("最后测速", GetColumnWidth("lastTestTime", 130), HorizontalAlignment.Center);
				RefreshServers();
			}
			if (!cmbSortColumn.Items.Contains("最后测速"))
			{
				cmbSortColumn.Items.Add("最后测速");
			}
		}
		else
		{
			if (hasLastTestTimeColumn)
			{
				for (int i = lvServers.Columns.Count - 1; i >= 0; i--)
				{
					if (lvServers.Columns[i].Text == "最后测速")
					{
						lvServers.Columns.RemoveAt(i);
						break;
					}
				}
				RefreshServers();
			}
			if (cmbSortColumn.Items.Contains("最后测速"))
			{
				if (cmbSortColumn.SelectedItem?.ToString() == "最后测速")
				{
					cmbSortColumn.SelectedIndex = 0;
				}
				cmbSortColumn.Items.Remove("最后测速");
			}
		}
		lvServers.EndUpdate();
	}

	private void MainForm_ResizeEnd(object sender, EventArgs e)
	{
		config.uiItem.mainSize = base.Size;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == (Keys.S | Keys.Control))
		{
			btnSaveConfig_Click(null, null);
			return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.lvServers = new nodesCatchNext.Base.ListViewFlickerFree();
		this.cmsLv = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuRealPingServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuDownLoadServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuAutoTestSelected = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.menuAddServers = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.menuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
		this.menuEditServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuRemoveServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuRemoveDuplicateServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuRemoveLoseServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuRemoveLowServer = new System.Windows.Forms.ToolStripMenuItem();
		this.menuRemoveNoResultServer = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.menuExport2ShareUrl = new System.Windows.Forms.ToolStripMenuItem();
		this.menuExport2SubContent = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		this.menuExport2Base64 = new System.Windows.Forms.ToolStripMenuItem();
		this.menuExport2Clash = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		this.menuStartClash = new System.Windows.Forms.ToolStripMenuItem();
		this.panel2 = new System.Windows.Forms.Panel();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.txtMsgBox = new System.Windows.Forms.TextBox();
		this.panel1 = new System.Windows.Forms.Panel();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.groupBox5 = new System.Windows.Forms.Panel();
		this.btnClearResult = new System.Windows.Forms.Button();
		this.btnClearLog = new System.Windows.Forms.Button();
		this.btnSaveConfig = new System.Windows.Forms.Button();
		this.label10 = new System.Windows.Forms.Label();
		this.btnStartTest = new System.Windows.Forms.Button();
		this.cbAutoSort = new System.Windows.Forms.CheckBox();
		this.label15 = new System.Windows.Forms.Label();
		this.button2 = new System.Windows.Forms.Button();
		this.tbLowSpeed = new System.Windows.Forms.TextBox();
		this.btnSettings = new System.Windows.Forms.Button();
		this.tb_fm_ave = new System.Windows.Forms.TextBox();
		this.tbTimeout = new System.Windows.Forms.TextBox();
		this.cmbSortColumn = new System.Windows.Forms.ComboBox();
		this.label2 = new System.Windows.Forms.Label();
		this.tbClashPort = new System.Windows.Forms.TextBox();
		this.btStopTest = new System.Windows.Forms.Button();
		this.label16 = new System.Windows.Forms.Label();
		this.cbKeywordFilter = new System.Windows.Forms.CheckBox();
		this.cmbSortOrder = new System.Windows.Forms.ComboBox();
		this.cbSpeedTest = new System.Windows.Forms.CheckBox();
		this.label5 = new System.Windows.Forms.Label();
		this.cbStrictMode = new System.Windows.Forms.CheckBox();
		this.label8 = new System.Windows.Forms.Label();
		this.tbKeywordFilter = new System.Windows.Forms.TextBox();
		this.tb_fm_max = new System.Windows.Forms.TextBox();
		this.btnKeywordPreset = new System.Windows.Forms.Button();
		this.tb_fm_second = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.订阅列表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.导入到节点列表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.label6 = new System.Windows.Forms.Label();
		this.cbAutoSave = new System.Windows.Forms.CheckBox();
		this.cbFastMode = new System.Windows.Forms.CheckBox();
		this.label12 = new System.Windows.Forms.Label();
		this.cbRealPing = new System.Windows.Forms.CheckBox();
		this.label4 = new System.Windows.Forms.Label();
		this.tbThread = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.tbPingNum = new System.Windows.Forms.TextBox();
		this.cmsKeywordPreset = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolTipKeywordFilter = new System.Windows.Forms.ToolTip(this.components);
		this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		this.cmsLv.SuspendLayout();
		this.panel2.SuspendLayout();
		this.groupBox1.SuspendLayout();
		this.panel1.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.groupBox5.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		this.cmsMain.SuspendLayout();
		base.SuspendLayout();
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.lvServers);
		this.splitContainer1.Panel1MinSize = 0;
		this.splitContainer1.Panel2.Controls.Add(this.panel2);
		this.splitContainer1.Panel2.Controls.Add(this.panel1);
		this.splitContainer1.Panel2MinSize = 0;
		this.splitContainer1.Size = new System.Drawing.Size(1841, 1283);
		this.splitContainer1.SplitterDistance = 774;
		this.splitContainer1.SplitterWidth = 5;
		this.splitContainer1.TabIndex = 2;
		this.splitContainer1.TabStop = false;
		this.lvServers.ContextMenuStrip = this.cmsLv;
		this.lvServers.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lvServers.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lvServers.FullRowSelect = true;
		this.lvServers.GridLines = true;
		this.lvServers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
		this.lvServers.HideSelection = false;
		this.lvServers.Location = new System.Drawing.Point(0, 0);
		this.lvServers.Margin = new System.Windows.Forms.Padding(4);
		this.lvServers.MultiSelect = false;
		this.lvServers.Name = "lvServers";
		this.lvServers.Size = new System.Drawing.Size(1841, 774);
		this.lvServers.TabIndex = 0;
		this.lvServers.UseCompatibleStateImageBehavior = false;
		this.lvServers.View = System.Windows.Forms.View.Details;
		this.lvServers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(lvServers_ColumnClick);
		this.lvServers.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(lvServers_ColumnWidthChanged);
		this.lvServers.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(lvServers_ColumnWidthChanging);
		this.lvServers.DoubleClick += new System.EventHandler(lvServers_DoubleClick);
		this.lvServers.KeyDown += new System.Windows.Forms.KeyEventHandler(lvServers_KeyDown);
		this.cmsLv.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.cmsLv.Items.AddRange(new System.Windows.Forms.ToolStripItem[21]
		{
			this.menuRealPingServer, this.menuDownLoadServer, this.menuAutoTestSelected, this.toolStripSeparator3, this.menuAddServers, this.toolStripSeparator4, this.menuSelectAll, this.menuEditServer,
			this.menuRemoveServer, this.menuRemoveDuplicateServer, this.menuRemoveLoseServer, this.menuRemoveLowServer, this.menuRemoveNoResultServer, this.toolStripSeparator5, this.menuExport2ShareUrl, this.menuExport2SubContent, this.toolStripSeparator6, this.menuExport2Base64,
			this.menuExport2Clash, this.toolStripSeparator7, this.menuStartClash
		});
		this.cmsLv.Name = "cmsLv";
		this.cmsLv.Size = new System.Drawing.Size(285, 466);
		this.menuRealPingServer.Name = "menuRealPingServer";
		this.menuRealPingServer.Size = new System.Drawing.Size(284, 24);
		this.menuRealPingServer.Text = "测试HTTPS延迟(Ctrl+R)";
		this.menuRealPingServer.Click += new System.EventHandler(menuRealPingServer_Click);
		this.menuDownLoadServer.Name = "menuDownLoadServer";
		this.menuDownLoadServer.Size = new System.Drawing.Size(284, 24);
		this.menuDownLoadServer.Text = "测试服务器下载速度(Ctrl+T)";
		this.menuDownLoadServer.Click += new System.EventHandler(menuDownLoadServer_Click);
		this.menuAutoTestSelected.Name = "menuAutoTestSelected";
		this.menuAutoTestSelected.Size = new System.Drawing.Size(284, 24);
		this.menuAutoTestSelected.Text = "选中节点一键测速(Ctrl+E)";
		this.menuAutoTestSelected.Click += new System.EventHandler(menuAutoTestSelected_Click);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new System.Drawing.Size(281, 6);
		this.menuAddServers.Name = "menuAddServers";
		this.menuAddServers.Size = new System.Drawing.Size(284, 24);
		this.menuAddServers.Text = "从剪贴板导入节点(Ctrl+V)";
		this.menuAddServers.Click += new System.EventHandler(menuAddServers_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(281, 6);
		this.menuSelectAll.Name = "menuSelectAll";
		this.menuSelectAll.Size = new System.Drawing.Size(284, 24);
		this.menuSelectAll.Text = "全选(Ctrl+A)";
		this.menuSelectAll.Click += new System.EventHandler(menuSelectAll_Click);
		this.menuEditServer.Name = "menuEditServer";
		this.menuEditServer.Size = new System.Drawing.Size(284, 24);
		this.menuEditServer.Text = "编辑节点(双击)";
		this.menuEditServer.Click += new System.EventHandler(menuEditServer_Click);
		this.menuRemoveServer.Name = "menuRemoveServer";
		this.menuRemoveServer.Size = new System.Drawing.Size(284, 24);
		this.menuRemoveServer.Text = "删除选中节点(多选)(Delete)";
		this.menuRemoveServer.Click += new System.EventHandler(menuRemoveServer_Click);
		this.menuRemoveDuplicateServer.Name = "menuRemoveDuplicateServer";
		this.menuRemoveDuplicateServer.Size = new System.Drawing.Size(284, 24);
		this.menuRemoveDuplicateServer.Text = "移除重复节点";
		this.menuRemoveDuplicateServer.Click += new System.EventHandler(menuRemoveDuplicateServer_Click);
		this.menuRemoveLoseServer.Name = "menuRemoveLoseServer";
		this.menuRemoveLoseServer.Size = new System.Drawing.Size(284, 24);
		this.menuRemoveLoseServer.Text = "移除无效节点";
		this.menuRemoveLoseServer.Click += new System.EventHandler(menuRemoveLoseServer_Click);
		this.menuRemoveLowServer.Name = "menuRemoveLowServer";
		this.menuRemoveLowServer.Size = new System.Drawing.Size(284, 24);
		this.menuRemoveLowServer.Text = "移除低速节点";
		this.menuRemoveLowServer.Click += new System.EventHandler(menuRemoveLowServer_Click);
		this.menuRemoveNoResultServer.Name = "menuRemoveNoResultServer";
		this.menuRemoveNoResultServer.Size = new System.Drawing.Size(284, 24);
		this.menuRemoveNoResultServer.Text = "移除无结果节点";
		this.menuRemoveNoResultServer.Click += new System.EventHandler(menuRemoveNoResultServer_Click);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		this.toolStripSeparator5.Size = new System.Drawing.Size(281, 6);
		this.menuExport2ShareUrl.Name = "menuExport2ShareUrl";
		this.menuExport2ShareUrl.Size = new System.Drawing.Size(284, 24);
		this.menuExport2ShareUrl.Text = "导出分享URL到剪贴板(Ctrl+C)";
		this.menuExport2ShareUrl.Click += new System.EventHandler(menuExport2ShareUrl_Click);
		this.menuExport2SubContent.Name = "menuExport2SubContent";
		this.menuExport2SubContent.Size = new System.Drawing.Size(284, 24);
		this.menuExport2SubContent.Text = "导出订阅内容到剪贴板";
		this.menuExport2SubContent.Click += new System.EventHandler(menuExport2SubContent_Click);
		this.toolStripSeparator6.Name = "toolStripSeparator6";
		this.toolStripSeparator6.Size = new System.Drawing.Size(281, 6);
		this.menuExport2Base64.Name = "menuExport2Base64";
		this.menuExport2Base64.Size = new System.Drawing.Size(284, 24);
		this.menuExport2Base64.Text = "导出Base64通用订阅文件";
		this.menuExport2Base64.Click += new System.EventHandler(menuExport2Base64_Click);
		this.menuExport2Clash.Name = "menuExport2Clash";
		this.menuExport2Clash.Size = new System.Drawing.Size(284, 24);
		this.menuExport2Clash.Text = "导出Mihomo订阅文件";
		this.menuExport2Clash.Click += new System.EventHandler(menuExport2Clash_Click);
		this.toolStripSeparator7.Name = "toolStripSeparator7";
		this.toolStripSeparator7.Size = new System.Drawing.Size(281, 6);
		this.menuStartClash.Name = "menuStartClash";
		this.menuStartClash.Size = new System.Drawing.Size(284, 24);
		this.menuStartClash.Text = "选中节点推送到Mihomo内核";
		this.menuStartClash.Click += new System.EventHandler(menuStartClash_Click);
		this.panel2.Controls.Add(this.groupBox1);
		this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel2.Location = new System.Drawing.Point(0, 194);
		this.panel2.Margin = new System.Windows.Forms.Padding(4);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(1841, 310);
		this.panel2.TabIndex = 1;
		this.groupBox1.Controls.Add(this.txtMsgBox);
		this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBox1.Location = new System.Drawing.Point(0, 0);
		this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupBox1.Size = new System.Drawing.Size(1841, 310);
		this.groupBox1.TabIndex = 12;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "反馈";
		this.txtMsgBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtMsgBox.BackColor = System.Drawing.SystemColors.ButtonShadow;
		this.txtMsgBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.txtMsgBox.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.txtMsgBox.ForeColor = System.Drawing.SystemColors.Info;
		this.txtMsgBox.Location = new System.Drawing.Point(3, 20);
		this.txtMsgBox.Margin = new System.Windows.Forms.Padding(4);
		this.txtMsgBox.Multiline = true;
		this.txtMsgBox.Name = "txtMsgBox";
		this.txtMsgBox.ReadOnly = true;
		this.txtMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.txtMsgBox.Size = new System.Drawing.Size(1834, 288);
		this.txtMsgBox.TabIndex = 5;
		this.panel1.Controls.Add(this.groupBox3);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Margin = new System.Windows.Forms.Padding(4);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(1841, 194);
		this.panel1.TabIndex = 0;
		this.groupBox3.Controls.Add(this.groupBox5);
		this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBox3.Location = new System.Drawing.Point(0, 0);
		this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupBox3.Size = new System.Drawing.Size(1841, 194);
		this.groupBox3.TabIndex = 0;
		this.groupBox3.TabStop = false;
		this.groupBox5.Anchor = System.Windows.Forms.AnchorStyles.Top;
		this.groupBox5.Controls.Add(this.btnClearResult);
		this.groupBox5.Controls.Add(this.btnClearLog);
		this.groupBox5.Controls.Add(this.btnSaveConfig);
		this.groupBox5.Controls.Add(this.label10);
		this.groupBox5.Controls.Add(this.btnStartTest);
		this.groupBox5.Controls.Add(this.cbAutoSort);
		this.groupBox5.Controls.Add(this.label15);
		this.groupBox5.Controls.Add(this.button2);
		this.groupBox5.Controls.Add(this.tbLowSpeed);
		this.groupBox5.Controls.Add(this.btnSettings);
		this.groupBox5.Controls.Add(this.tb_fm_ave);
		this.groupBox5.Controls.Add(this.tbTimeout);
		this.groupBox5.Controls.Add(this.cmbSortColumn);
		this.groupBox5.Controls.Add(this.label2);
		this.groupBox5.Controls.Add(this.tbClashPort);
		this.groupBox5.Controls.Add(this.btStopTest);
		this.groupBox5.Controls.Add(this.label16);
		this.groupBox5.Controls.Add(this.cbKeywordFilter);
		this.groupBox5.Controls.Add(this.cmbSortOrder);
		this.groupBox5.Controls.Add(this.cbSpeedTest);
		this.groupBox5.Controls.Add(this.label5);
		this.groupBox5.Controls.Add(this.cbStrictMode);
		this.groupBox5.Controls.Add(this.label8);
		this.groupBox5.Controls.Add(this.tbKeywordFilter);
		this.groupBox5.Controls.Add(this.tb_fm_max);
		this.groupBox5.Controls.Add(this.btnKeywordPreset);
		this.groupBox5.Controls.Add(this.tb_fm_second);
		this.groupBox5.Controls.Add(this.button1);
		this.groupBox5.Controls.Add(this.label6);
		this.groupBox5.Controls.Add(this.cbAutoSave);
		this.groupBox5.Controls.Add(this.cbFastMode);
		this.groupBox5.Controls.Add(this.label12);
		this.groupBox5.Controls.Add(this.cbRealPing);
		this.groupBox5.Controls.Add(this.label4);
		this.groupBox5.Controls.Add(this.tbThread);
		this.groupBox5.Controls.Add(this.label3);
		this.groupBox5.Controls.Add(this.tbPingNum);
		this.groupBox5.Location = new System.Drawing.Point(0, -8);
		this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox5.Name = "groupBox5";
		this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox5.Size = new System.Drawing.Size(1841, 202);
		this.groupBox5.TabIndex = 29;
		this.btnClearResult.Location = new System.Drawing.Point(1384, 156);
		this.btnClearResult.Name = "btnClearResult";
		this.btnClearResult.Size = new System.Drawing.Size(100, 35);
		this.btnClearResult.TabIndex = 35;
		this.btnClearResult.Text = "清空结果";
		this.btnClearResult.UseVisualStyleBackColor = true;
		this.btnClearResult.Click += new System.EventHandler(btnClearResult_Click);
		this.btnClearLog.Location = new System.Drawing.Point(124, 157);
		this.btnClearLog.Name = "btnClearLog";
		this.btnClearLog.Size = new System.Drawing.Size(100, 35);
		this.btnClearLog.TabIndex = 6;
		this.btnClearLog.Text = "清空反馈";
		this.btnClearLog.UseVisualStyleBackColor = true;
		this.btnClearLog.Click += new System.EventHandler(btnClearLog_Click);
		this.btnSaveConfig.Location = new System.Drawing.Point(886, 79);
		this.btnSaveConfig.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btnSaveConfig.Name = "btnSaveConfig";
		this.btnSaveConfig.Size = new System.Drawing.Size(119, 48);
		this.btnSaveConfig.TabIndex = 5;
		this.btnSaveConfig.Text = "保存配置";
		this.btnSaveConfig.UseVisualStyleBackColor = true;
		this.btnSaveConfig.Click += new System.EventHandler(btnSaveConfig_Click);
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(892, 167);
		this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(113, 15);
		this.label10.TabIndex = 31;
		this.label10.Text = "限定时间(秒)：";
		this.btnStartTest.Location = new System.Drawing.Point(1561, 90);
		this.btnStartTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btnStartTest.Name = "btnStartTest";
		this.btnStartTest.Size = new System.Drawing.Size(155, 55);
		this.btnStartTest.TabIndex = 4;
		this.btnStartTest.Text = "一键自动测速";
		this.btnStartTest.UseVisualStyleBackColor = true;
		this.btnStartTest.Click += new System.EventHandler(btnStartTest_Click);
		this.cbAutoSort.AutoSize = true;
		this.cbAutoSort.Location = new System.Drawing.Point(1079, 36);
		this.cbAutoSort.Margin = new System.Windows.Forms.Padding(4);
		this.cbAutoSort.Name = "cbAutoSort";
		this.cbAutoSort.Size = new System.Drawing.Size(89, 19);
		this.cbAutoSort.TabIndex = 11;
		this.cbAutoSort.Text = "自动排序";
		this.cbAutoSort.UseVisualStyleBackColor = true;
		this.cbAutoSort.CheckedChanged += new System.EventHandler(cbAutoSort_CheckedChanged);
		this.label15.AutoSize = true;
		this.label15.Location = new System.Drawing.Point(1176, 37);
		this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(82, 15);
		this.label15.TabIndex = 12;
		this.label15.Text = "排序依据：";
		this.button2.Location = new System.Drawing.Point(1626, 156);
		this.button2.Margin = new System.Windows.Forms.Padding(4);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(90, 35);
		this.button2.TabIndex = 28;
		this.button2.Text = "关于软件";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.tbLowSpeed.Location = new System.Drawing.Point(492, 117);
		this.tbLowSpeed.Margin = new System.Windows.Forms.Padding(4);
		this.tbLowSpeed.Name = "tbLowSpeed";
		this.tbLowSpeed.Size = new System.Drawing.Size(49, 25);
		this.tbLowSpeed.TabIndex = 3;
		this.tbLowSpeed.Text = "0.5";
		this.tbLowSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tbLowSpeed.TextChanged += new System.EventHandler(tbLowSpeed_TextChanged);
		this.btnSettings.Location = new System.Drawing.Point(1499, 156);
		this.btnSettings.Margin = new System.Windows.Forms.Padding(4);
		this.btnSettings.Name = "btnSettings";
		this.btnSettings.Size = new System.Drawing.Size(119, 35);
		this.btnSettings.TabIndex = 29;
		this.btnSettings.Text = "设置";
		this.btnSettings.UseVisualStyleBackColor = true;
		this.btnSettings.Click += new System.EventHandler(btnSettings_Click);
		this.tb_fm_ave.Location = new System.Drawing.Point(1186, 161);
		this.tb_fm_ave.Margin = new System.Windows.Forms.Padding(4);
		this.tb_fm_ave.Name = "tb_fm_ave";
		this.tb_fm_ave.Size = new System.Drawing.Size(49, 25);
		this.tb_fm_ave.TabIndex = 29;
		this.tb_fm_ave.Text = "10";
		this.tb_fm_ave.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tb_fm_ave.TextChanged += new System.EventHandler(tb_fm_ave_TextChanged);
		this.tbTimeout.Location = new System.Drawing.Point(492, 77);
		this.tbTimeout.Margin = new System.Windows.Forms.Padding(4);
		this.tbTimeout.Name = "tbTimeout";
		this.tbTimeout.Size = new System.Drawing.Size(49, 25);
		this.tbTimeout.TabIndex = 1;
		this.tbTimeout.Text = "5";
		this.tbTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tbTimeout.TextChanged += new System.EventHandler(tbTimeout_TextChanged);
		this.cmbSortColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbSortColumn.FormattingEnabled = true;
		this.cmbSortColumn.Items.AddRange(new object[4] { "平均速度", "峰值速度", "HTTPS延迟", "最后测速" });
		this.cmbSortColumn.Location = new System.Drawing.Point(1266, 32);
		this.cmbSortColumn.Margin = new System.Windows.Forms.Padding(4);
		this.cmbSortColumn.Name = "cmbSortColumn";
		this.cmbSortColumn.Size = new System.Drawing.Size(100, 23);
		this.cmbSortColumn.TabIndex = 13;
		this.cmbSortColumn.SelectedIndexChanged += new System.EventHandler(cmbSortColumn_SelectedIndexChanged);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(324, 120);
		this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(160, 15);
		this.label2.TabIndex = 5;
		this.label2.Text = "清空低速节点(MB/s)：";
		this.tbClashPort.Location = new System.Drawing.Point(1497, 123);
		this.tbClashPort.Margin = new System.Windows.Forms.Padding(4);
		this.tbClashPort.Name = "tbClashPort";
		this.tbClashPort.Size = new System.Drawing.Size(53, 25);
		this.tbClashPort.TabIndex = 12;
		this.tbClashPort.Text = "9090";
		this.tbClashPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tbClashPort.TextChanged += new System.EventHandler(tbClashPort_TextChanged);
		this.btStopTest.Enabled = false;
		this.btStopTest.Location = new System.Drawing.Point(1561, 15);
		this.btStopTest.Margin = new System.Windows.Forms.Padding(4);
		this.btStopTest.Name = "btStopTest";
		this.btStopTest.Size = new System.Drawing.Size(155, 55);
		this.btStopTest.TabIndex = 2;
		this.btStopTest.Text = "停止手动测速线程";
		this.btStopTest.UseVisualStyleBackColor = true;
		this.btStopTest.Click += new System.EventHandler(button1_Click);
		this.label16.AutoSize = true;
		this.label16.Location = new System.Drawing.Point(1377, 38);
		this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label16.Name = "label16";
		this.label16.Size = new System.Drawing.Size(82, 15);
		this.label16.TabIndex = 14;
		this.label16.Text = "排序顺序：";
		this.cbKeywordFilter.AutoSize = true;
		this.cbKeywordFilter.Checked = true;
		this.cbKeywordFilter.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbKeywordFilter.Location = new System.Drawing.Point(1266, 81);
		this.cbKeywordFilter.Name = "cbKeywordFilter";
		this.cbKeywordFilter.Size = new System.Drawing.Size(112, 19);
		this.cbKeywordFilter.TabIndex = 33;
		this.cbKeywordFilter.Text = "关键词筛选:";
		this.toolTipKeywordFilter.SetToolTip(this.cbKeywordFilter, "勾选后启用关键词筛选规则");
		this.cbKeywordFilter.UseVisualStyleBackColor = true;
		this.cbKeywordFilter.CheckedChanged += new System.EventHandler(cbKeywordFilter_CheckedChanged);
		this.cmbSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbSortOrder.FormattingEnabled = true;
		this.cmbSortOrder.Items.AddRange(new object[2] { "降序", "升序" });
		this.cmbSortOrder.Location = new System.Drawing.Point(1463, 32);
		this.cmbSortOrder.Margin = new System.Windows.Forms.Padding(4);
		this.cmbSortOrder.Name = "cmbSortOrder";
		this.cmbSortOrder.Size = new System.Drawing.Size(80, 23);
		this.cmbSortOrder.TabIndex = 15;
		this.cmbSortOrder.SelectedIndexChanged += new System.EventHandler(cmbSortOrder_SelectedIndexChanged);
		this.cbSpeedTest.AutoSize = true;
		this.cbSpeedTest.Checked = true;
		this.cbSpeedTest.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbSpeedTest.Location = new System.Drawing.Point(124, 119);
		this.cbSpeedTest.Margin = new System.Windows.Forms.Padding(4);
		this.cbSpeedTest.Name = "cbSpeedTest";
		this.cbSpeedTest.Size = new System.Drawing.Size(104, 19);
		this.cbSpeedTest.TabIndex = 9;
		this.cbSpeedTest.Text = "测下载速度";
		this.cbSpeedTest.UseVisualStyleBackColor = true;
		this.cbSpeedTest.CheckedChanged += new System.EventHandler(cbSpeedTest_CheckedChanged);
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(1332, 127);
		this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(160, 15);
		this.label5.TabIndex = 11;
		this.label5.Text = "Mihomo外部控制端口：";
		this.cbStrictMode.AutoSize = true;
		this.cbStrictMode.Location = new System.Drawing.Point(1079, 126);
		this.cbStrictMode.Margin = new System.Windows.Forms.Padding(4);
		this.cbStrictMode.Name = "cbStrictMode";
		this.cbStrictMode.Size = new System.Drawing.Size(119, 19);
		this.cbStrictMode.TabIndex = 16;
		this.cbStrictMode.Text = "严格排除模式";
		this.toolTipKeywordFilter.SetToolTip(this.cbStrictMode, "启用后，HTTPS延迟测速失败即删除节点");
		this.cbStrictMode.UseVisualStyleBackColor = true;
		this.cbStrictMode.CheckedChanged += new System.EventHandler(cbStrictMode_CheckedChanged);
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(1071, 167);
		this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(106, 15);
		this.label8.TabIndex = 28;
		this.label8.Text = "下载进度(%)：";
		this.tbKeywordFilter.Location = new System.Drawing.Point(1384, 79);
		this.tbKeywordFilter.Name = "tbKeywordFilter";
		this.tbKeywordFilter.Size = new System.Drawing.Size(135, 25);
		this.tbKeywordFilter.TabIndex = 32;
		this.toolTipKeywordFilter.SetToolTip(this.tbKeywordFilter, "支持多个关键词，用逗号或空格分隔\r\n例如：香港,日本,美国 或 \ud83c\udded\ud83c\uddf0 \ud83c\uddef\ud83c\uddf5 \ud83c\uddfa\ud83c\uddf8\r\n使用 ! 前缀排除关键词\r\n例如：!过期 !失效");
		this.tbKeywordFilter.TextChanged += new System.EventHandler(tbKeywordFilter_TextChanged);
		this.tb_fm_max.Location = new System.Drawing.Point(824, 160);
		this.tb_fm_max.Margin = new System.Windows.Forms.Padding(4);
		this.tb_fm_max.Name = "tb_fm_max";
		this.tb_fm_max.Size = new System.Drawing.Size(49, 25);
		this.tb_fm_max.TabIndex = 26;
		this.tb_fm_max.Text = "300";
		this.tb_fm_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tb_fm_max.TextChanged += new System.EventHandler(tb_fm_max_TextChanged);
		this.btnKeywordPreset.Location = new System.Drawing.Point(1521, 79);
		this.btnKeywordPreset.Name = "btnKeywordPreset";
		this.btnKeywordPreset.Size = new System.Drawing.Size(33, 25);
		this.btnKeywordPreset.TabIndex = 34;
		this.btnKeywordPreset.Text = "▼";
		this.toolTipKeywordFilter.SetToolTip(this.btnKeywordPreset, "选择常用地区预设");
		this.btnKeywordPreset.UseVisualStyleBackColor = true;
		this.btnKeywordPreset.Click += new System.EventHandler(btnKeywordPreset_Click);
		this.tb_fm_second.Location = new System.Drawing.Point(1013, 161);
		this.tb_fm_second.Margin = new System.Windows.Forms.Padding(4);
		this.tb_fm_second.Name = "tb_fm_second";
		this.tb_fm_second.Size = new System.Drawing.Size(49, 25);
		this.tb_fm_second.TabIndex = 25;
		this.tb_fm_second.Text = "5";
		this.tb_fm_second.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tb_fm_second.TextChanged += new System.EventHandler(tb_fm_second_TextChanged);
		this.button1.ContextMenuStrip = this.contextMenuStrip1;
		this.button1.Location = new System.Drawing.Point(687, 79);
		this.button1.Margin = new System.Windows.Forms.Padding(4);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(118, 48);
		this.button1.TabIndex = 27;
		this.button1.Text = "订阅管理";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click_1);
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.订阅列表ToolStripMenuItem, this.导入到节点列表ToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(184, 52);
		this.订阅列表ToolStripMenuItem.Name = "订阅列表ToolStripMenuItem";
		this.订阅列表ToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
		this.订阅列表ToolStripMenuItem.Text = "订阅列表";
		this.订阅列表ToolStripMenuItem.Click += new System.EventHandler(订阅列表ToolStripMenuItem_Click);
		this.导入到节点列表ToolStripMenuItem.Name = "导入到节点列表ToolStripMenuItem";
		this.导入到节点列表ToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
		this.导入到节点列表ToolStripMenuItem.Text = "导入到节点列表";
		this.导入到节点列表ToolStripMenuItem.Click += new System.EventHandler(导入到节点列表ToolStripMenuItem_Click);
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(684, 167);
		this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(130, 15);
		this.label6.TabIndex = 24;
		this.label6.Text = "峰值速度(MB/s)：";
		this.cbAutoSave.AutoSize = true;
		this.cbAutoSave.Location = new System.Drawing.Point(1079, 81);
		this.cbAutoSave.Margin = new System.Windows.Forms.Padding(4);
		this.cbAutoSave.Name = "cbAutoSave";
		this.cbAutoSave.Size = new System.Drawing.Size(164, 19);
		this.cbAutoSave.TabIndex = 17;
		this.cbAutoSave.Text = "测速后自动保存配置";
		this.toolTipKeywordFilter.SetToolTip(this.cbAutoSave, "启用后，自动测速结束时将自动保存配置，无需弹窗确认\r\n禁用时，测速结束后会弹窗询问是否保存");
		this.cbAutoSave.UseVisualStyleBackColor = true;
		this.cbAutoSave.CheckedChanged += new System.EventHandler(cbAutoSave_CheckedChanged);
		this.cbFastMode.AutoSize = true;
		this.cbFastMode.Checked = true;
		this.cbFastMode.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbFastMode.Location = new System.Drawing.Point(548, 166);
		this.cbFastMode.Margin = new System.Windows.Forms.Padding(4);
		this.cbFastMode.Name = "cbFastMode";
		this.cbFastMode.Size = new System.Drawing.Size(119, 19);
		this.cbFastMode.TabIndex = 23;
		this.cbFastMode.Text = "启用快速模式";
		this.cbFastMode.UseVisualStyleBackColor = true;
		this.cbFastMode.CheckedChanged += new System.EventHandler(cbFastMode_CheckedChanged);
		this.label12.AutoSize = true;
		this.label12.Location = new System.Drawing.Point(121, 34);
		this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(127, 15);
		this.label12.TabIndex = 27;
		this.label12.Text = "延迟测速线程数：";
		this.cbRealPing.AutoSize = true;
		this.cbRealPing.Checked = true;
		this.cbRealPing.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbRealPing.Location = new System.Drawing.Point(124, 79);
		this.cbRealPing.Margin = new System.Windows.Forms.Padding(4);
		this.cbRealPing.Name = "cbRealPing";
		this.cbRealPing.Size = new System.Drawing.Size(104, 19);
		this.cbRealPing.TabIndex = 10;
		this.cbRealPing.Text = "测延迟速度";
		this.cbRealPing.UseVisualStyleBackColor = true;
		this.cbRealPing.CheckedChanged += new System.EventHandler(cbRealPing_CheckedChanged);
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(371, 80);
		this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(113, 15);
		this.label4.TabIndex = 8;
		this.label4.Text = "延迟超时(秒)：";
		this.tbThread.Location = new System.Drawing.Point(256, 31);
		this.tbThread.Margin = new System.Windows.Forms.Padding(4);
		this.tbThread.Name = "tbThread";
		this.tbThread.Size = new System.Drawing.Size(53, 25);
		this.tbThread.TabIndex = 13;
		this.tbThread.Text = "100";
		this.tbThread.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tbThread.TextChanged += new System.EventHandler(tbThread_TextChanged);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(326, 37);
		this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(158, 15);
		this.label3.TabIndex = 7;
		this.label3.Text = "延迟结果应小于(个)：";
		this.tbPingNum.Location = new System.Drawing.Point(492, 33);
		this.tbPingNum.Margin = new System.Windows.Forms.Padding(4);
		this.tbPingNum.Name = "tbPingNum";
		this.tbPingNum.Size = new System.Drawing.Size(49, 25);
		this.tbPingNum.TabIndex = 2;
		this.tbPingNum.Text = "100";
		this.tbPingNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.tbPingNum.TextChanged += new System.EventHandler(tbPingNum_TextChanged);
		this.cmsKeywordPreset.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.cmsKeywordPreset.Name = "cmsKeywordPreset";
		this.cmsKeywordPreset.Size = new System.Drawing.Size(61, 4);
		this.cmsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.menuExit });
		this.cmsMain.Name = "cmsMain";
		this.cmsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.cmsMain.ShowCheckMargin = true;
		this.cmsMain.ShowImageMargin = false;
		this.cmsMain.Size = new System.Drawing.Size(109, 28);
		this.menuExit.Name = "menuExit";
		this.menuExit.Size = new System.Drawing.Size(108, 24);
		this.menuExit.Text = "退出";
		this.menuExit.Click += new System.EventHandler(menuExit_Click);
		this.AllowDrop = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1841, 1283);
		base.Controls.Add(this.splitContainer1);
		base.Margin = new System.Windows.Forms.Padding(5);
		this.MinimumSize = new System.Drawing.Size(1760, 1330);
		base.Name = "MainForm";
		this.Text = "nodesCatchNext - V3.8";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MainForm_FormClosing);
		base.Load += new System.EventHandler(MainForm_Load);
		base.ResizeEnd += new System.EventHandler(MainForm_ResizeEnd);
		base.DragDrop += new System.Windows.Forms.DragEventHandler(MainForm_DragDrop);
		base.DragEnter += new System.Windows.Forms.DragEventHandler(MainForm_DragEnter);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		this.cmsLv.ResumeLayout(false);
		this.panel2.ResumeLayout(false);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.panel1.ResumeLayout(false);
		this.groupBox3.ResumeLayout(false);
		this.groupBox5.ResumeLayout(false);
		this.groupBox5.PerformLayout();
		this.contextMenuStrip1.ResumeLayout(false);
		this.cmsMain.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	private void InitializeSubscriptionTabControl()
	{
		tabSubscriptions = new TabControl();
		tabSubscriptions.Dock = DockStyle.None;
		tabSubscriptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		tabSubscriptions.Location = new Point(0, 0);
		tabSubscriptions.Name = "tabSubscriptions";
		tabSubscriptions.SelectedIndex = 0;
		tabSubscriptions.Size = new Size(splitContainer1.Panel1.Width, 26);
		tabSubscriptions.TabIndex = 0;
		tabSubscriptions.AllowDrop = true;
		tabSubscriptions.SelectedIndexChanged += tabSubscriptions_SelectedIndexChanged;
		tabSubscriptions.MouseDown += TabSubscriptions_MouseDown;
		tabSubscriptions.MouseMove += TabSubscriptions_MouseMove;
		tabSubscriptions.DragOver += TabSubscriptions_DragOver;
		tabSubscriptions.DragDrop += TabSubscriptions_DragDrop;
		cmsTabSubscriptions = new ContextMenuStrip();
		lvServers.Location = new Point(0, 26);
		lvServers.Height = splitContainer1.Panel1.Height - 26;
		lvServers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		lvServers.Dock = DockStyle.None;
		splitContainer1.Panel1.Controls.Add(tabSubscriptions);
		tabSubscriptions.BringToFront();
	}

	private void UpdateTabContextMenu()
	{
		cmsTabSubscriptions.Items.Clear();
		if (rightClickedTabSubId == null)
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("更新所有订阅");
			toolStripMenuItem.Click += MenuUpdateSub_Click;
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("删除所有节点");
			toolStripMenuItem2.Click += MenuDeleteAllNodes_Click;
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("删除所有无分组节点");
			toolStripMenuItem3.Click += MenuDeleteUnassignedNodes_Click;
			cmsTabSubscriptions.Items.Add(toolStripMenuItem);
			cmsTabSubscriptions.Items.Add(toolStripMenuItem2);
			cmsTabSubscriptions.Items.Add(toolStripMenuItem3);
		}
		else if (rightClickedTabSubId == "__unassigned__")
		{
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem("删除所有无分组节点");
			toolStripMenuItem4.Click += MenuDeleteUnassignedNodes_Click;
			cmsTabSubscriptions.Items.Add(toolStripMenuItem4);
		}
		else
		{
			ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem("更新此订阅");
			toolStripMenuItem5.Click += MenuUpdateSub_Click;
			ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem("删除此订阅的所有节点");
			toolStripMenuItem6.Click += MenuDeleteSubNodes_Click;
			ToolStripMenuItem toolStripMenuItem7 = new ToolStripMenuItem("复制订阅地址");
			toolStripMenuItem7.Click += MenuCopySubUrl_Click;
			cmsTabSubscriptions.Items.Add(toolStripMenuItem5);
			cmsTabSubscriptions.Items.Add(toolStripMenuItem6);
			cmsTabSubscriptions.Items.Add(toolStripMenuItem7);
			cmsTabSubscriptions.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem8 = new ToolStripMenuItem("上移订阅");
			toolStripMenuItem8.Click += MenuMoveSubUp_Click;
			ToolStripMenuItem toolStripMenuItem9 = new ToolStripMenuItem("下移订阅");
			toolStripMenuItem9.Click += MenuMoveSubDown_Click;
			cmsTabSubscriptions.Items.Add(toolStripMenuItem8);
			cmsTabSubscriptions.Items.Add(toolStripMenuItem9);
		}
	}

	private void TabSubscriptions_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			for (int i = 0; i < tabSubscriptions.TabPages.Count; i++)
			{
				if (tabSubscriptions.GetTabRect(i).Contains(e.Location))
				{
					draggedTabIndex = i;
					Size dragSize = SystemInformation.DragSize;
					dragBoxFromMouseDown = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
					break;
				}
			}
		}
		else
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			for (int j = 0; j < tabSubscriptions.TabPages.Count; j++)
			{
				if (tabSubscriptions.GetTabRect(j).Contains(e.Location))
				{
					TabPage tabPage = tabSubscriptions.TabPages[j];
					rightClickedTabSubId = tabPage.Tag as string;
					UpdateTabContextMenu();
					cmsTabSubscriptions.Show(tabSubscriptions, e.Location);
					break;
				}
			}
		}
	}

	private void TabSubscriptions_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left && draggedTabIndex >= 0 && dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
		{
			if (draggedTabIndex > 0)
			{
				TabPage data = tabSubscriptions.TabPages[draggedTabIndex];
				tabSubscriptions.DoDragDrop(data, DragDropEffects.Move);
			}
			dragBoxFromMouseDown = Rectangle.Empty;
		}
	}

	private void TabSubscriptions_DragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(typeof(TabPage)))
		{
			e.Effect = DragDropEffects.Move;
			Point pt = tabSubscriptions.PointToClient(new Point(e.X, e.Y));
			for (int i = 0; i < tabSubscriptions.TabPages.Count; i++)
			{
				if (tabSubscriptions.GetTabRect(i).Contains(pt))
				{
					if (i != draggedTabIndex && i > 0)
					{
						tabSubscriptions.Cursor = Cursors.Hand;
					}
					else
					{
						tabSubscriptions.Cursor = Cursors.No;
					}
					break;
				}
			}
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void TabSubscriptions_DragDrop(object sender, DragEventArgs e)
	{
		tabSubscriptions.Cursor = Cursors.Default;
		if (!e.Data.GetDataPresent(typeof(TabPage)))
		{
			return;
		}
		Point pt = tabSubscriptions.PointToClient(new Point(e.X, e.Y));
		int num = -1;
		for (int i = 0; i < tabSubscriptions.TabPages.Count; i++)
		{
			if (tabSubscriptions.GetTabRect(i).Contains(pt))
			{
				num = i;
				break;
			}
		}
		if (num < 0 || num == draggedTabIndex || draggedTabIndex < 1 || num < 1)
		{
			draggedTabIndex = -1;
			return;
		}
		string text = tabSubscriptions.TabPages[draggedTabIndex].Tag as string;
		if (string.IsNullOrEmpty(text) || config.subItem == null)
		{
			draggedTabIndex = -1;
			return;
		}
		int num2 = -1;
		int num3 = -1;
		for (int j = 0; j < config.subItem.Count; j++)
		{
			if (config.subItem[j].id == text)
			{
				num2 = j;
			}
			string text2 = tabSubscriptions.TabPages[num].Tag as string;
			if (!string.IsNullOrEmpty(text2) && config.subItem[j].id == text2)
			{
				num3 = j;
			}
		}
		if (num2 < 0 || num3 < 0)
		{
			draggedTabIndex = -1;
			return;
		}
		SubItem item = config.subItem[num2];
		config.subItem.RemoveAt(num2);
		if (num2 < num3)
		{
			config.subItem.Insert(num3 - 1, item);
		}
		else
		{
			config.subItem.Insert(num3, item);
		}
		ConfigHandler.SaveConfig(ref config, reload: false);
		InitSubscriptionTabs(restoreSelection: true);
		draggedTabIndex = -1;
		ShowMsg("订阅顺序已更新");
	}

	private void MenuUpdateSub_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rightClickedTabSubId))
		{
			UpdateSubscriptionProcess();
			return;
		}
		SubItem subItem = null;
		if (config.subItem != null)
		{
			foreach (SubItem item in config.subItem)
			{
				if (item.id == rightClickedTabSubId)
				{
					subItem = item;
					break;
				}
			}
		}
		if (subItem == null || string.IsNullOrWhiteSpace(subItem.url))
		{
			ShowMsg("订阅信息无效或URL为空");
			return;
		}
		int nodeCountBefore = config.vmess?.Count ?? 0;
		string subRemarks = subItem.remarks;
		string url = subItem.url.Trim();
		string subId = subItem.id.Trim();
		AppendText(notify: false, "开始更新订阅：" + subRemarks);
		AppendText(notify: false, "--> 正在下载订阅数据...");
		DownloadHandle downloadHandle = new DownloadHandle();
		downloadHandle.UpdateCompleted += delegate(object obj, DownloadHandle.ResultEventArgs args)
		{
			if (args.Success)
			{
				AppendText(notify: false, "--> 获取网页数据成功");
				string text = Utils.Base64Decode(args.Msg);
				if (!Utils.IsNullOrEmpty(text))
				{
					if (text.IndexOf("vmess://") != -1 || text.IndexOf("vless://") != -1 || text.IndexOf("ss://") != -1 || text.IndexOf("ssr://") != -1 || text.IndexOf("trojan://") != -1 || text.IndexOf("socks://") != -1 || text.IndexOf("http://") != -1 || text.IndexOf("https://") != -1 || text.IndexOf("hysteria2://") != -1 || text.IndexOf("hy2://") != -1 || text.IndexOf("anytls://") != -1)
					{
						if (MainFormHandler.Instance.AddBatchServers(config, text, subId) <= 0)
						{
							AppendText(notify: false, "--> 导入节点信息失败");
							ShowMsg("导入节点信息失败");
						}
						else
						{
							AppendText(notify: false, "--> 节点信息更新完成");
							int num = (config.vmess?.Count ?? 0) - nodeCountBefore;
							InitSubscriptionTabs();
							RefreshServersView();
							ShowMsg($"订阅 \"{subRemarks}\" 更新完成，导入了 {num} 个节点");
						}
					}
					else
					{
						AppendText(notify: false, "--> 订阅内容格式不正确");
						ShowMsg("订阅内容格式不正确");
					}
				}
				else
				{
					AppendText(notify: false, "--> Base64解码失败，订阅内容可能为空");
					ShowMsg("Base64解码失败，订阅内容可能为空");
				}
			}
			else
			{
				AppendText(notify: false, "--> 导入失败！" + args.Msg);
				ShowMsg("导入失败：" + args.Msg);
			}
		};
		downloadHandle.WebDownloadString(url);
	}

	private void MenuDeleteSubNodes_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rightClickedTabSubId))
		{
			return;
		}
		string text = "未知订阅";
		if (config.subItem != null)
		{
			foreach (SubItem item in config.subItem)
			{
				if (item.id == rightClickedTabSubId)
				{
					text = item.remarks;
					break;
				}
			}
		}
		if (MessageBox.Show("确定要删除订阅 \"" + text + "\" 的所有节点吗？\n\n此操作不可撤销！", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		int num = 0;
		for (int num2 = config.vmess.Count - 1; num2 >= 0; num2--)
		{
			if (config.vmess[num2].subid == rightClickedTabSubId)
			{
				num++;
			}
		}
		for (int num3 = config.vmess.Count - 1; num3 >= 0; num3--)
		{
			if (config.vmess[num3].subid == rightClickedTabSubId)
			{
				config.vmess.RemoveAt(num3);
			}
		}
		ConfigHandler.SaveConfig(ref config, reload: false);
		RefreshServersView();
		ShowMsg($"已删除订阅 \"{text}\" 的 {num} 个节点");
	}

	private void MenuCopySubUrl_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rightClickedTabSubId))
		{
			return;
		}
		if (config.subItem != null)
		{
			foreach (SubItem item in config.subItem)
			{
				if (item.id == rightClickedTabSubId)
				{
					if (!string.IsNullOrWhiteSpace(item.url))
					{
						Utils.SetClipboardData(item.url);
						ShowMsg("已复制订阅 \"" + item.remarks + "\" 的地址");
					}
					else
					{
						ShowMsg("该订阅地址为空");
					}
					return;
				}
			}
		}
		ShowMsg("未找到订阅信息");
	}

	private void MenuDeleteAllNodes_Click(object sender, EventArgs e)
	{
		if (config.vmess == null || config.vmess.Count == 0)
		{
			ShowMsg("当前没有可删除的节点");
		}
		else if (MessageBox.Show("确定要删除所有节点吗？\n\n此操作不可撤销！", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
		{
			int count = config.vmess.Count;
			config.vmess.Clear();
			ConfigHandler.SaveConfig(ref config, reload: false);
			RefreshServersView();
			ShowMsg($"已删除所有节点，共 {count} 个");
		}
	}

	private void MenuDeleteUnassignedNodes_Click(object sender, EventArgs e)
	{
		if (config.vmess == null || config.vmess.Count == 0)
		{
			ShowMsg("当前没有可删除的节点");
			return;
		}
		int num = config.vmess.RemoveAll((VmessItem v) => string.IsNullOrEmpty(v.subid));
		if (num == 0)
		{
			ShowMsg("没有找到未分组的节点");
			return;
		}
		ConfigHandler.SaveConfig(ref config, reload: false);
		InitSubscriptionTabs();
		RefreshServersView();
		ShowMsg($"已删除所有未分组的节点，共 {num} 个");
	}

	private void MenuMoveSubUp_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rightClickedTabSubId) || config.subItem == null || config.subItem.Count <= 1)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < config.subItem.Count; i++)
		{
			if (config.subItem[i].id == rightClickedTabSubId)
			{
				num = i;
				break;
			}
		}
		if (num <= 0)
		{
			ShowMsg("此订阅已经在最上面，无法上移");
			return;
		}
		SubItem value = config.subItem[num];
		config.subItem[num] = config.subItem[num - 1];
		config.subItem[num - 1] = value;
		ConfigHandler.SaveConfig(ref config, reload: false);
		InitSubscriptionTabs(restoreSelection: true);
		ShowMsg("订阅已上移");
	}

	private void MenuMoveSubDown_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rightClickedTabSubId) || config.subItem == null || config.subItem.Count <= 1)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < config.subItem.Count; i++)
		{
			if (config.subItem[i].id == rightClickedTabSubId)
			{
				num = i;
				break;
			}
		}
		if (num < 0 || num >= config.subItem.Count - 1)
		{
			ShowMsg("此订阅已经在最下面，无法下移");
			return;
		}
		SubItem value = config.subItem[num];
		config.subItem[num] = config.subItem[num + 1];
		config.subItem[num + 1] = value;
		ConfigHandler.SaveConfig(ref config, reload: false);
		InitSubscriptionTabs(restoreSelection: true);
		ShowMsg("订阅已下移");
	}
}
