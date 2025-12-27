using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

internal class UpdateHandle
{
	public class ResultEventArgs : EventArgs
	{
		public bool Success;

		public string Msg;

		public ResultEventArgs(bool success, string msg)
		{
			Success = success;
			Msg = msg;
		}
	}

	private Action<bool, string> _updateFunc;

	private Config _config;

	private int _totalSubscriptions;

	private int _processedSubscriptions;

	private int _successfulSubscriptions;

	private int _totalImportedNodes;

	private List<(string name, string reason)> _failedSubscriptions = new List<(string, string)>();

	private object _lockObject = new object();

	private const int SubconverterPort = 25500;

	private void CheckAndReportCompletion()
	{
		lock (_lockObject)
		{
			if (_processedSubscriptions >= _totalSubscriptions)
			{
				string arg = BuildCompletionSummary();
				_updateFunc(arg1: false, arg);
			}
		}
	}

	private string BuildCompletionSummary()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("========================================");
		if (_successfulSubscriptions == _totalSubscriptions)
		{
			stringBuilder.AppendLine("       所有订阅更新完成!");
		}
		else if (_successfulSubscriptions == 0)
		{
			stringBuilder.AppendLine("       所有订阅更新失败!");
		}
		else
		{
			stringBuilder.AppendLine("     订阅更新完成 (部分失败)");
		}
		stringBuilder.AppendLine("----------------------------------------");
		stringBuilder.AppendLine($"  成功订阅: {_successfulSubscriptions} 个");
		if (_failedSubscriptions.Count > 0)
		{
			stringBuilder.AppendLine($"  失败订阅: {_failedSubscriptions.Count} 个");
			foreach (var (text, text2) in _failedSubscriptions)
			{
				stringBuilder.AppendLine("    - " + text + ": " + text2);
			}
		}
		stringBuilder.AppendLine($"  导入节点: {_totalImportedNodes} 个");
		stringBuilder.AppendLine("========================================");
		return stringBuilder.ToString();
	}

	private string subconverterParse(string base64str)
	{
		string text = Guid.NewGuid().ToString("N");
		string path = Utils.GetPath("subconverter\\temp_" + text + ".txt");
		try
		{
			if (!ConfigHandler.TcpClientCheck("127.0.0.1", 25500))
			{
				throw new Exception($"无法链接到subconverter(端口:{25500})，请确认是否启动！建议重新打开测速软件");
			}
			File.WriteAllText(path, base64str, Encoding.UTF8);
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://127.0.0.1:{25500}/sub?target=mixed&url=temp_{text}.txt&insert=false");
			obj.Method = "GET";
			obj.Timeout = 10000;
			obj.ReadWriteTimeout = 10000;
			obj.ContinueTimeout = 10000;
			using StreamReader streamReader = new StreamReader(obj.GetResponse().GetResponseStream(), Encoding.UTF8);
			return streamReader.ReadToEnd();
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

	public void UpdateSubscriptionProcess(Config config, Action<bool, string> update)
	{
		_config = config;
		_updateFunc = update;
		_totalSubscriptions = 0;
		_processedSubscriptions = 0;
		_successfulSubscriptions = 0;
		_totalImportedNodes = 0;
		_failedSubscriptions.Clear();
		int num = 0;
		_updateFunc(arg1: false, "开始更新节点信息...");
		if (config.subItem == null || config.subItem.Count <= 0)
		{
			_updateFunc(arg1: false, "订阅列表为空，请添加后再尝试");
			return;
		}
		_totalSubscriptions = config.subItem.Count;
		for (int i = 1; i <= config.subItem.Count; i++)
		{
			string id = config.subItem[i - 1].id.Trim();
			string text = config.subItem[i - 1].url.Trim();
			string remarks = config.subItem[i - 1].remarks ?? "未知订阅";
			string hashCode = "-->";
			if (!config.subItem[i - 1].enabled)
			{
				lock (_lockObject)
				{
					_processedSubscriptions++;
				}
				if (++num == config.subItem.Count)
				{
					_updateFunc(arg1: false, hashCode + "未启用任何订阅，请检查");
				}
				CheckAndReportCompletion();
				continue;
			}
			if (Utils.IsNullOrEmpty(id) || Utils.IsNullOrEmpty(text))
			{
				_updateFunc(arg1: false, hashCode + "[" + remarks + "] 订阅信息不完整，请检查");
				lock (_lockObject)
				{
					_failedSubscriptions.Add((remarks, "订阅地址为空"));
					_processedSubscriptions++;
				}
				CheckAndReportCompletion();
				continue;
			}
			DownloadHandle downloadHandle = new DownloadHandle();
			downloadHandle.UpdateCompleted += delegate(object sender2, DownloadHandle.ResultEventArgs args)
			{
				if (args.Success)
				{
					_updateFunc(arg1: false, hashCode + "[" + remarks + "] 获取网页数据成功");
					string text2 = Utils.Base64Decode(args.Msg);
					if (!Utils.IsNullOrEmpty(text2))
					{
						if (text2.IndexOf("vmess://") != -1 || text2.IndexOf("vless://") != -1 || text2.IndexOf("ss://") != -1 || text2.IndexOf("ssr://") != -1 || text2.IndexOf("trojan://") != -1 || text2.IndexOf("socks://") != -1 || text2.IndexOf("http://") != -1 || text2.IndexOf("https://") != -1 || text2.IndexOf("hysteria2://") != -1 || text2.IndexOf("hy2://") != -1 || text2.IndexOf("anytls://") != -1)
						{
							int num2 = MainFormHandler.Instance.AddBatchServers(config, text2, id);
							if (num2 <= 0)
							{
								_updateFunc(arg1: false, hashCode + "[" + remarks + "] 导入节点信息失败");
								lock (_lockObject)
								{
									_failedSubscriptions.Add((remarks, "导入节点信息失败"));
								}
							}
							else
							{
								_updateFunc(arg1: true, $"{hashCode}[{remarks}] 节点信息更新完成，导入 {num2} 个节点");
								lock (_lockObject)
								{
									_successfulSubscriptions++;
									_totalImportedNodes += num2;
								}
							}
							lock (_lockObject)
							{
								_processedSubscriptions++;
							}
							CheckAndReportCompletion();
							return;
						}
						text2 = "ss://YWVzLTI1Ni1nY206ZmFCQW9ENTRrODdVSkc3QDEuMS4xLjE6NjY2#%e5%8d%a0%e4%bd%8d%e8%8a%82%e7%82%b9" + Environment.NewLine + text2;
						text2 = Utils.Base64Encode(text2);
					}
					else
					{
						if (args.Msg.IndexOf("proxies:") == -1)
						{
							_updateFunc(arg1: false, hashCode + "[" + remarks + "] 解析失败，导入节点失败");
							lock (_lockObject)
							{
								_failedSubscriptions.Add((remarks, "解析失败"));
								_processedSubscriptions++;
							}
							CheckAndReportCompletion();
							return;
						}
						text2 = args.Msg;
					}
					try
					{
						text2 = subconverterParse(text2);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "出现异常");
						_updateFunc(arg1: false, hashCode + "[" + remarks + "] 解析失败！" + ex.Message);
						lock (_lockObject)
						{
							_failedSubscriptions.Add((remarks, ex.Message));
							_processedSubscriptions++;
						}
						CheckAndReportCompletion();
						return;
					}
					int num3 = MainFormHandler.Instance.AddBatchServers(config, text2, id);
					if (num3 <= 0)
					{
						_updateFunc(arg1: false, hashCode + "[" + remarks + "] 导入节点信息失败");
						lock (_lockObject)
						{
							_failedSubscriptions.Add((remarks, "导入节点信息失败"));
						}
					}
					else
					{
						_updateFunc(arg1: true, $"{hashCode}[{remarks}] 节点信息更新完成，导入 {num3} 个节点");
						lock (_lockObject)
						{
							_successfulSubscriptions++;
							_totalImportedNodes += num3;
						}
					}
					lock (_lockObject)
					{
						_processedSubscriptions++;
					}
					CheckAndReportCompletion();
				}
				else
				{
					_updateFunc(arg1: false, hashCode + "[" + remarks + "] 导入失败！" + args.Msg);
					lock (_lockObject)
					{
						_failedSubscriptions.Add((remarks, args.Msg));
						_processedSubscriptions++;
					}
					CheckAndReportCompletion();
				}
			};
			downloadHandle.Error += delegate(object sender2, ErrorEventArgs args)
			{
				_updateFunc(arg1: false, hashCode + "[" + remarks + "] 导入失败！" + args.GetException().Message);
				lock (_lockObject)
				{
					_failedSubscriptions.Add((remarks, args.GetException().Message));
					_processedSubscriptions++;
				}
				CheckAndReportCompletion();
			};
			downloadHandle.WebDownloadString(text);
		}
	}
}
