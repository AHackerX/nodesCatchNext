using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

internal class MainFormHandler
{
	private static readonly Lazy<MainFormHandler> instance = new Lazy<MainFormHandler>(() => new MainFormHandler());

	private const int SubconverterPort = 25500;

	public static MainFormHandler Instance => instance.Value;

	public int AddBatchServers(Config config, string clipboardData, string subid = "")
	{
		// 检测是否为 Clash YAML 格式（包含 proxies:）
		if (clipboardData.IndexOf("proxies:") != -1)
		{
			// 优先使用原生解析器
			int nativeResult = TryNativeYamlParse(config, clipboardData, subid);
			if (nativeResult > 0)
			{
				return nativeResult;
			}
			// 原生解析失败，回退到 subconverter
		}

		// 其他需要 subconverter 转换的格式
		if ((clipboardData.IndexOf("method") != -1 && clipboardData.IndexOf("tag") != -1 && clipboardData.IndexOf("password") != -1) || clipboardData.IndexOf("[server_local]") != -1 || clipboardData.IndexOf("[Proxy]") != -1 || clipboardData.IndexOf("[RoutingRule]") != -1 || (clipboardData.IndexOf("server_port") != -1 && clipboardData.IndexOf("server") != -1) || (clipboardData.IndexOf("[SERVER]") != -1 && clipboardData.IndexOf("[POLICY]") != -1))
		{
			if (!ConfigHandler.TcpClientCheck("127.0.0.1", 25500))
			{
				MessageBox.Show($"无法链接到subconverter(端口:{25500})，请确认是否启动！建议重新打开测速软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return 0;
			}
			string text = Guid.NewGuid().ToString("N");
			string path = Utils.GetPath("subconverter\\temp_" + text + ".txt");
			try
			{
				File.WriteAllText(path, clipboardData, Encoding.UTF8);
				HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://127.0.0.1:{25500}/sub?target=mixed&url=temp_{text}.txt&insert=false");
				obj.Method = "GET";
				obj.Timeout = 10000;
				obj.ReadWriteTimeout = 10000;
				obj.ContinueTimeout = 10000;
				using WebResponse webResponse = obj.GetResponse();
				using StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
				string clipboardData2 = streamReader.ReadToEnd();
				return Instance.AddBatchServers(config, clipboardData2);
			}
			catch (Exception ex)
			{
				MessageBox.Show("无法转换节点，请确认信息正确！返回异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return 0;
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
		int num = _Add();
		if (num < 1)
		{
			clipboardData = Utils.Base64Decode(clipboardData);
			num = _Add();
		}
		return num;
		int _Add()
		{
			return ConfigHandler.AddBatchServers(ref config, clipboardData, subid);
		}
	}

	/// <summary>
	/// 尝试使用原生解析器解析 Clash YAML 配置
	/// </summary>
	private int TryNativeYamlParse(Config config, string yamlContent, string subid)
	{
		try
		{
			List<VmessItem> items = ClashYamlParser.ParseYaml(yamlContent);
			if (items == null || items.Count == 0)
				return 0;

			// 如果是订阅更新且模式为替换，先删除旧节点
			if (!string.IsNullOrEmpty(subid) && config.subUpdateMode == 0)
			{
				config.vmess?.RemoveAll(v => v.subid == subid);
			}

			int count = 0;
			foreach (var item in items)
			{
				item.subid = subid;
				
				// 根据类型添加节点
				int result = -1;
				switch (item.configType)
				{
					case 1: // VMess
						result = ConfigHandler.AddServer(ref config, item, -1);
						break;
					case 3: // Shadowsocks
						result = ConfigHandler.AddShadowsocksServer(ref config, item, -1);
						break;
					case 4: // Socks
						result = ConfigHandler.AddSocksServer(ref config, item, -1);
						break;
					case 5: // VLESS
						result = ConfigHandler.AddVlessServer(ref config, item, -1);
						break;
					case 6: // Trojan
						result = ConfigHandler.AddTrojanServer(ref config, item, -1);
						break;
					case 7: // HTTP
						result = ConfigHandler.AddHttpServer(ref config, item, -1);
						break;
					case 8: // HTTPS
						result = ConfigHandler.AddHttpsServer(ref config, item, -1);
						break;
					case 9: // SSR (ShadowsocksR)
						config.vmess.Add(item);
						result = 0;
						break;
					case 11: // Hysteria2
						result = ConfigHandler.AddHysteria2Server(ref config, item, -1);
						break;
				}
				
				if (result == 0)
					count++;
			}

			return count;
		}
		catch
		{
			return 0;
		}
	}
}
