using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using nodesCatchNext.Base;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

internal class ConfigHandler
{
	private static string configRes = "nodeConfig.json";

	public static event ProcessDelegate ProcessEvent;

	public static int LoadConfig(ref Config config)
	{
		string text = Utils.LoadResource(Utils.GetPath(configRes));
		if (!Utils.IsNullOrEmpty(text))
		{
			config = Utils.FromJson<Config>(text);
			ShowMsg(b: false, "配置文件加载成功！");
		}
		if (config == null)
		{
			ShowMsg(b: false, "未找到配置文件，已生成默认配置文件");
			config = new Config
			{
				vmess = new List<VmessItem>()
			};
		}
		if (config.subItem == null)
		{
			config.subItem = new List<SubItem>();
		}
		foreach (SubItem item in config.subItem)
		{
			if (Utils.IsNullOrEmpty(item.id))
			{
				item.id = Utils.GetGUID();
			}
		}
		if (config.localPort == 0)
		{
			config.localPort = 40000;
		}
		if (config.externalControllerPort == 0)
		{
			config.externalControllerPort = 40001;
		}
		if (config.externalController == null)
		{
			config.externalController = "127.0.0.1:40001";
		}
		if (config.uiItem == null)
		{
			config.uiItem = new UIItem
			{
				mainSize = new Size(1640, 900)
			};
		}
		if (config.uiItem.mainLvColWidth == null)
		{
			config.uiItem.mainLvColWidth = new Dictionary<string, int>
			{
				{ "def", 40 },
				{ "configType", 80 },
				{ "remarks", 200 },
				{ "address", 120 },
				{ "port", 50 },
				{ "security", 90 },
				{ "network", 70 },
				{ "tls", 80 },
				{ "subRemarks", 70 },
				{ "tlsRtt", 80 },
				{ "httpsDelay", 80 },
				{ "testResult", 200 },
				{ "MaxSpeed", 80 }
			};
		}
		else
		{
			if (!config.uiItem.mainLvColWidth.ContainsKey("tlsRtt"))
			{
				config.uiItem.mainLvColWidth.Add("tlsRtt", 80);
			}
			if (!config.uiItem.mainLvColWidth.ContainsKey("httpsDelay"))
			{
				config.uiItem.mainLvColWidth.Add("httpsDelay", 80);
			}
		}
		if (Utils.IsNullOrEmpty(config.speedTestUrl))
		{
			config.speedTestUrl = "https://cdn.kernel.org/pub/linux/kernel/v6.x/linux-6.2.15.tar.xz";
		}
		if (Utils.IsNullOrEmpty(config.speedPingTestUrl))
		{
			config.speedPingTestUrl = "https://www.cloudflare.com/cdn-cgi/trace";
		}
		if (Utils.IsNullOrEmpty(config.Timeout))
		{
			config.Timeout = "5";
		}
		if (Utils.IsNullOrEmpty(config.PingNum))
		{
			config.PingNum = "100";
		}
		if (Utils.IsNullOrEmpty(config.LowSpeed))
		{
			config.LowSpeed = "0.5";
		}
		if (Utils.IsNullOrEmpty(config.ClashPort))
		{
			config.ClashPort = "9090";
		}
		if (Utils.IsNullOrEmpty(config.FMave))
		{
			config.FMave = "10";
		}
		if (Utils.IsNullOrEmpty(config.FMmax))
		{
			config.FMmax = "0.3";
		}
		if (Utils.IsNullOrEmpty(config.FMSecond))
		{
			config.FMSecond = "5";
		}
		if (Utils.IsNullOrEmpty(config.Thread))
		{
			config.Thread = "100";
		}
		if (Utils.IsNullOrEmpty(config.DownloadThread))
		{
			config.DownloadThread = "5";
		}
		config.defAllowInsecure = true;
		return 0;
	}

	public static void ShowMsg(bool b, string msg)
	{
		ConfigHandler.ProcessEvent?.Invoke(b, msg);
	}

	public static int RemoveServerViaSubid(ref Config config, string subid)
	{
		ToJsonFile(config);
		return 0;
	}

	public static int AddformMainLvColWidth(ref Config config, string name, int width)
	{
		if (config.uiItem.mainLvColWidth == null)
		{
			config.uiItem.mainLvColWidth = new Dictionary<string, int>();
		}
		if (config.uiItem.mainLvColWidth.ContainsKey(name))
		{
			config.uiItem.mainLvColWidth[name] = width;
		}
		else
		{
			config.uiItem.mainLvColWidth.Add(name, width);
		}
		return 0;
	}

	public static int GetformMainLvColWidth(ref Config config, string name, int width)
	{
		if (config.uiItem.mainLvColWidth == null)
		{
			config.uiItem.mainLvColWidth = new Dictionary<string, int>();
		}
		if (config.uiItem.mainLvColWidth.ContainsKey(name))
		{
			return config.uiItem.mainLvColWidth[name];
		}
		return width;
	}

	public static int SaveConfig(ref Config config, bool reload)
	{
		Global.reloadV2ray = reload;
		ToJsonFile(config);
		ShowMsg(b: false, "配置文件保存成功！");
		return 0;
	}

	public static int SaveUIConfigOnly(Config currentConfig)
	{
		try
		{
			string text = Utils.LoadResource(Utils.GetPath(configRes));
			if (Utils.IsNullOrEmpty(text))
			{
				return -1;
			}
			Config config = Utils.FromJson<Config>(text);
			if (config == null)
			{
				return -1;
			}
			if (config.uiItem == null)
			{
				config.uiItem = new UIItem();
			}
			config.uiItem.mainSize = currentConfig.uiItem.mainSize;
			config.uiItem.mainLocation = currentConfig.uiItem.mainLocation;
			config.uiItem.mainLvColWidth = currentConfig.uiItem.mainLvColWidth;
			ToJsonFile(config);
			return 0;
		}
		catch
		{
			return -1;
		}
	}

	public static void ToJsonFile(Config config)
	{
		config.index = 0;
		Utils.ToJsonFile(config, Utils.GetPath(configRes));
	}

	public static int AddBatchServers(ref Config config, string clipboardData, string subid = "")
	{
		if (Utils.IsNullOrEmpty(clipboardData))
		{
			return -1;
		}
		if (!string.IsNullOrEmpty(subid) && config.subUpdateMode == 0)
		{
			config.vmess?.RemoveAll((VmessItem v) => v.subid == subid);
		}
		int num = 0;
		string[] array = clipboardData.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			string text = array[num2].Trim();
			if (Utils.IsNullOrEmpty(text))
			{
				continue;
			}
			string msg;
			VmessItem vmessItem = ShareHandler.ImportFromClipboardConfig(text, out msg);
			if (vmessItem == null)
			{
				continue;
			}
			vmessItem.subid = subid;
			if (vmessItem.configType == 1)
			{
				if (AddServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 3)
			{
				if (AddShadowsocksServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 4)
			{
				if (AddSocksServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 7)
			{
				if (AddHttpServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 8)
			{
				if (AddHttpsServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 6)
			{
				if (AddTrojanServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 5)
			{
				if (AddVlessServer(ref config, vmessItem, -1) == 0)
				{
					num++;
				}
			}
			else if (vmessItem.configType == 9)
			{
				config.vmess.Add(vmessItem);
				num++;
			}
			else if (vmessItem.configType == 11 && AddHysteria2Server(ref config, vmessItem, -1) == 0)
			{
				num++;
			}
			else if (vmessItem.configType == 12 && AddAnyTLSServer(ref config, vmessItem, -1) == 0)
			{
				num++;
			}
		}
		return num;
	}

	public static int AddVlessServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 5;
		vmessItem.address = vmessItem.address.TrimEx();
		vmessItem.id = vmessItem.id.TrimEx();
		vmessItem.security = vmessItem.security.TrimEx();
		vmessItem.network = vmessItem.network.TrimEx();
		vmessItem.headerType = vmessItem.headerType.TrimEx();
		vmessItem.requestHost = vmessItem.requestHost.TrimEx();
		vmessItem.path = vmessItem.path.TrimEx();
		vmessItem.streamSecurity = vmessItem.streamSecurity.TrimEx();
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			if (Utils.IsNullOrEmpty(vmessItem.allowInsecure))
			{
				vmessItem.allowInsecure = config.defAllowInsecure.ToString();
			}
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddShadowsocksServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 3;
		vmessItem.address = vmessItem.address.TrimEx();
		vmessItem.id = vmessItem.id.TrimEx();
		vmessItem.security = vmessItem.security.TrimEx();
		vmessItem.network = vmessItem.network.TrimEx();
		if (!Global.ssSecuritys.Contains(vmessItem.security))
		{
			return -1;
		}
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddSocksServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 4;
		vmessItem.address = vmessItem.address.TrimEx();
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddHttpServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 7;
		vmessItem.address = vmessItem.address.TrimEx();
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddHttpsServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 8;
		vmessItem.address = vmessItem.address.TrimEx();
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddTrojanServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 6;
		vmessItem.address = vmessItem.address.TrimEx();
		vmessItem.id = vmessItem.id.TrimEx();
		vmessItem.streamSecurity = "tls";
		if (Utils.IsNullOrEmpty(vmessItem.allowInsecure))
		{
			vmessItem.allowInsecure = config.defAllowInsecure.ToString();
		}
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddHysteria2Server(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 11;
		vmessItem.address = vmessItem.address.TrimEx();
		vmessItem.id = vmessItem.id.TrimEx();
		vmessItem.network = "hysteria2";
		vmessItem.streamSecurity = "tls";
		vmessItem.sni = vmessItem.sni.TrimEx();
		vmessItem.path = vmessItem.path.TrimEx();
		vmessItem.requestHost = vmessItem.requestHost.TrimEx();
		vmessItem.publicKey = vmessItem.publicKey.TrimEx();
		if (Utils.IsNullOrEmpty(vmessItem.allowInsecure))
		{
			vmessItem.allowInsecure = config.defAllowInsecure.ToString();
		}
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddAnyTLSServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 12;
		vmessItem.address = vmessItem.address.TrimEx();
		vmessItem.id = vmessItem.id.TrimEx();
		vmessItem.network = "anytls";
		vmessItem.streamSecurity = "tls";
		vmessItem.sni = vmessItem.sni.TrimEx();
		if (Utils.IsNullOrEmpty(vmessItem.allowInsecure))
		{
			vmessItem.allowInsecure = config.defAllowInsecure.ToString();
		}
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			config.vmess.Add(vmessItem);
			if (config.vmess.Count == 1)
			{
				config.index = 0;
				Global.reloadV2ray = true;
			}
		}
		return 0;
	}

	public static int AddServer(ref Config config, VmessItem vmessItem, int index)
	{
		vmessItem.configVersion = 2;
		vmessItem.configType = 1;
		vmessItem.address = vmessItem.address.TrimEx();
		vmessItem.id = vmessItem.id.TrimEx();
		vmessItem.security = vmessItem.security.TrimEx();
		vmessItem.network = vmessItem.network.TrimEx();
		vmessItem.headerType = vmessItem.headerType.TrimEx();
		vmessItem.requestHost = vmessItem.requestHost.TrimEx();
		vmessItem.path = vmessItem.path.TrimEx();
		vmessItem.streamSecurity = vmessItem.streamSecurity.TrimEx();
		if (index >= 0)
		{
			config.vmess[index] = vmessItem;
			if (config.index.Equals(index))
			{
				Global.reloadV2ray = true;
			}
		}
		else
		{
			if (Utils.IsNullOrEmpty(vmessItem.allowInsecure))
			{
				vmessItem.allowInsecure = config.defAllowInsecure.ToString();
			}
			config.vmess.Add(vmessItem);
		}
		return 0;
	}

	public static int RemoveServer(ref Config config, int index)
	{
		if (index < 0 || index > config.vmess.Count - 1)
		{
			return -1;
		}
		config.vmess.RemoveAt(index);
		return 0;
	}

	public static int AddSubItem(ref Config config, string url)
	{
		foreach (SubItem item2 in config.subItem)
		{
			if (url == item2.url)
			{
				return 0;
			}
		}
		SubItem item = new SubItem
		{
			id = Utils.GetGUID(),
			remarks = "剪贴板导入",
			url = url
		};
		config.subItem.Add(item);
		return SaveSubItem(ref config);
	}

	public static int SaveSubItem(ref Config config)
	{
		if (config.subItem == null || config.subItem.Count <= 0)
		{
			return -1;
		}
		foreach (SubItem item in config.subItem)
		{
			if (Utils.IsNullOrEmpty(item.id))
			{
				item.id = Utils.GetGUID();
			}
		}
		return 0;
	}

	public static int SortServers(ref Config config, EServerColName name, bool asc)
	{
		if (config.vmess.Count <= 0)
		{
			return -1;
		}
		if ((uint)(name - 1) > 5u && (uint)(name - 8) > 1u && name != EServerColName.MaxSpeed && name != EServerColName.tls && name != EServerColName.subRemarks && name != EServerColName.tlsRtt && name != EServerColName.httpsDelay && name != EServerColName.testResult && name != EServerColName.lastTestTime)
		{
			return -1;
		}
		switch (name)
		{
		case EServerColName.MaxSpeed:
			if (asc)
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.MaxSpeed) descending, ParseMaxSpeed(item.MaxSpeed)
					select item).ToList();
			}
			else
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.MaxSpeed) descending, ParseMaxSpeed(item.MaxSpeed) descending
					select item).ToList();
			}
			break;
		case EServerColName.tls:
			if (asc)
			{
				config.vmess = config.vmess.OrderBy((VmessItem item) => Utils.IsNullOrEmpty(item.streamSecurity) ? "none" : item.streamSecurity).ToList();
			}
			else
			{
				config.vmess = config.vmess.OrderByDescending((VmessItem item) => Utils.IsNullOrEmpty(item.streamSecurity) ? "none" : item.streamSecurity).ToList();
			}
			break;
		case EServerColName.subRemarks:
		{
			Config configCopy = config;
			if (asc)
			{
				config.vmess = config.vmess.OrderBy((VmessItem item) => item.getSubRemarks(configCopy)).ToList();
			}
			else
			{
				config.vmess = config.vmess.OrderByDescending((VmessItem item) => item.getSubRemarks(configCopy)).ToList();
			}
			break;
		}
		case EServerColName.tlsRtt:
			if (asc)
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.tlsRtt) descending, ParseTlsRtt(item.tlsRtt)
					select item).ToList();
			}
			else
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.tlsRtt) descending, ParseTlsRtt(item.tlsRtt) descending
					select item).ToList();
			}
			break;
		case EServerColName.httpsDelay:
			if (asc)
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.httpsDelay) descending, ParseTlsRtt(item.httpsDelay)
					select item).ToList();
			}
			else
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.httpsDelay) descending, ParseTlsRtt(item.httpsDelay) descending
					select item).ToList();
			}
			break;
		case EServerColName.testResult:
			if (asc)
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.testResult) descending, ParseTestResult(item.testResult)
					select item).ToList();
			}
			else
			{
				config.vmess = (from item in config.vmess
					orderby HasTestResult(item.testResult) descending, ParseTestResult(item.testResult) descending
					select item).ToList();
			}
			break;
		case EServerColName.lastTestTime:
			if (asc)
			{
				config.vmess = (from item in config.vmess
					orderby !string.IsNullOrEmpty(item.lastTestTime) descending, item.lastTestTime
					select item).ToList();
			}
			else
			{
				config.vmess = (from item in config.vmess
					orderby !string.IsNullOrEmpty(item.lastTestTime) descending, item.lastTestTime descending
					select item).ToList();
			}
			break;
		default:
		{
			IQueryable<VmessItem> query = config.vmess.AsQueryable();
			if (asc)
			{
				config.vmess = query.OrderBy(name.ToString()).ToList();
			}
			else
			{
				config.vmess = query.OrderByDescending(name.ToString()).ToList();
			}
			break;
		}
		}
		return 0;
	}

	private static double ParseTlsRtt(string tlsRtt)
	{
		if (string.IsNullOrEmpty(tlsRtt))
		{
			return -1.0;
		}
		if (tlsRtt.Contains("超时") || tlsRtt.Contains("Timeout") || tlsRtt.Contains("无法连接"))
		{
			return double.MaxValue;
		}
		if (tlsRtt.Contains("ms") && double.TryParse(tlsRtt.Replace("ms", "").Trim(), out var result))
		{
			return result;
		}
		return -1.0;
	}

	private static double ParseTestResult(string testResult)
	{
		if (string.IsNullOrEmpty(testResult))
		{
			return -1.0;
		}
		if (testResult.Contains("超时") || testResult.Contains("Timeout") || testResult.Contains("无法连接") || testResult.Contains("测速被取消") || testResult.Contains("等待测速线程..."))
		{
			return -1.0;
		}
		if (testResult.Contains("ms") && double.TryParse(testResult.Replace("ms", "").Trim(), out var result))
		{
			return result;
		}
		testResult = testResult.Trim();
		double result4;
		if (testResult.IndexOf("MB/s") != -1)
		{
			if (double.TryParse(testResult.Substring(0, testResult.IndexOf("MB/s")).Trim(), out var result2))
			{
				return result2;
			}
		}
		else if (testResult.IndexOf("KB/s") != -1)
		{
			if (double.TryParse(testResult.Substring(0, testResult.IndexOf("KB/s")).Trim(), out var result3))
			{
				return result3 / 1024.0;
			}
		}
		else if (testResult.IndexOf("B/s") != -1 && double.TryParse(testResult.Substring(0, testResult.IndexOf("B/s")).Trim(), out result4))
		{
			return result4 / 1048576.0;
		}
		return -1.0;
	}

	private static double ParseMaxSpeed(string maxSpeed)
	{
		if (string.IsNullOrEmpty(maxSpeed))
		{
			return -1.0;
		}
		maxSpeed = maxSpeed.Trim();
		double result4;
		if (maxSpeed.IndexOf("MB/s") != -1)
		{
			if (double.TryParse(maxSpeed.Substring(0, maxSpeed.IndexOf("MB/s")).Trim(), out var result))
			{
				return result;
			}
		}
		else if (maxSpeed.IndexOf("KB/s") != -1)
		{
			if (double.TryParse(maxSpeed.Substring(0, maxSpeed.IndexOf("KB/s")).Trim(), out var result2))
			{
				return result2 / 1024.0;
			}
		}
		else if (maxSpeed.IndexOf("B/s") != -1)
		{
			if (double.TryParse(maxSpeed.Substring(0, maxSpeed.IndexOf("B/s")).Trim(), out var result3))
			{
				return result3 / 1048576.0;
			}
		}
		else if (double.TryParse(maxSpeed, out result4))
		{
			return result4;
		}
		return -1.0;
	}

	private static bool HasTestResult(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		if (value.Contains("超时") || value.Contains("Timeout") || value.Contains("无法连接") || value.Contains("测速被取消") || value.Contains("等待测速线程...") || value.Contains("正在测试..."))
		{
			return false;
		}
		return true;
	}

	public static string sendReq(string args, string uri, string method)
	{
		string result = "";
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = method;
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Timeout = 300;
			httpWebRequest.ReadWriteTimeout = 300;
			httpWebRequest.ContinueTimeout = 300;
			byte[] bytes = Encoding.UTF8.GetBytes(args);
			httpWebRequest.ContentLength = bytes.Length;
			using (Stream stream = httpWebRequest.GetRequestStream())
			{
				stream.Write(bytes, 0, bytes.Length);
			}
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				result = Convert.ToInt32(httpWebResponse.StatusCode).ToString();
			}
			return result;
		}
		catch (WebException ex)
		{
			if (ex.Response is HttpWebResponse httpWebResponse2)
			{
				result = Convert.ToInt32(httpWebResponse2.StatusCode).ToString();
				try
				{
					using StreamReader streamReader = new StreamReader(httpWebResponse2.GetResponseStream());
					string text = streamReader.ReadToEnd();
					if (!string.IsNullOrEmpty(text))
					{
						result = result + ": " + text;
					}
				}
				catch
				{
				}
			}
			else
			{
				result = ex.Message;
			}
			return result;
		}
		catch (Exception ex2)
		{
			return ex2.Message;
		}
	}

	public static bool TcpClientCheck(string ip, int port)
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
		TcpClient tcpClient = null;
		try
		{
			tcpClient = new TcpClient();
			tcpClient.Connect(remoteEP);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			tcpClient?.Close();
		}
	}
}
