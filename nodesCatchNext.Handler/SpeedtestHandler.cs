using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

public class SpeedtestHandler
{
	private Config _config;

	private V2rayHandler _v2rayHandler;

	private List<int> _selecteds;

	private Action<int, string> _updateFunc;

	private Action<int, string> _updateMaxFunc;

	private Action<int, string> _updateHttpsDelayFunc;

	private Action<bool> _btStopTestStat;

	private CancellationTokenSource _cts;

	private int _pid;

	private Dictionary<int, VmessItem> _nodeSnapshot;

	public SpeedtestHandler(ref Config config, ref CancellationTokenSource cts, ref V2rayHandler v2rayHandler, List<int> selecteds, Action<int, string> update, Action<int, string> updateMax, Action<bool> btStopTestStat, int pid)
		: this(ref config, ref cts, ref v2rayHandler, selecteds, update, updateMax, null, btStopTestStat, pid)
	{
	}

	public SpeedtestHandler(ref Config config, ref CancellationTokenSource cts, ref V2rayHandler v2rayHandler, List<int> selecteds, Action<int, string> update, Action<int, string> updateMax, Action<int, string> updateHttpsDelay, Action<bool> btStopTestStat, int pid)
	{
		_config = config;
		_v2rayHandler = v2rayHandler;
		_selecteds = Utils.DeepCopy(selecteds);
		_updateFunc = update;
		_updateMaxFunc = updateMax;
		_updateHttpsDelayFunc = updateHttpsDelay;
		_btStopTestStat = btStopTestStat;
		_cts = cts;
		_pid = pid;
		_nodeSnapshot = new Dictionary<int, VmessItem>();
		foreach (int selected in _selecteds)
		{
			if (selected >= 0 && selected < _config.vmess.Count)
			{
				_nodeSnapshot[selected] = _config.vmess[selected];
			}
		}
	}

	public SpeedtestHandler(ref Config config, ref CancellationTokenSource cts, ref V2rayHandler v2rayHandler, List<int> selecteds, string actionType, Action<int, string> update, Action<int, string> updateMax, Action<bool> btStopTestStat, int pid)
		: this(ref config, ref cts, ref v2rayHandler, selecteds, actionType, update, updateMax, null, btStopTestStat, pid)
	{
	}

	public SpeedtestHandler(ref Config config, ref CancellationTokenSource cts, ref V2rayHandler v2rayHandler, List<int> selecteds, string actionType, Action<int, string> update, Action<int, string> updateMax, Action<int, string> updateHttpsDelay, Action<bool> btStopTestStat, int pid)
	{
		_config = config;
		_v2rayHandler = v2rayHandler;
		_selecteds = Utils.DeepCopy(selecteds);
		_updateFunc = update;
		_updateMaxFunc = updateMax;
		_updateHttpsDelayFunc = updateHttpsDelay;
		_btStopTestStat = btStopTestStat;
		_cts = cts;
		_pid = pid;
		_nodeSnapshot = new Dictionary<int, VmessItem>();
		foreach (int selected in _selecteds)
		{
			if (selected >= 0 && selected < _config.vmess.Count)
			{
				_nodeSnapshot[selected] = _config.vmess[selected];
			}
		}
		CancellationToken token = _cts.Token;
		switch (actionType)
		{
		case "realping":
		case "httpsdelay":
			if (_config.ThreadNum == 0)
			{
				Task.Run(delegate
				{
					RunRealPing(token);
				}, token);
			}
			else
			{
				Task.Run(delegate
				{
					RunRealPing2(token);
				}, token);
			}
			break;
		case "speedtest":
			if (_config.DownloadThreadNum == 0)
			{
				Task.Run(delegate
				{
					RunSpeedTest(token);
				}, token);
			}
			else
			{
				Task.Run(delegate
				{
					RunSpeedTest2(token);
				}, token);
			}
			break;
		}
	}

	public void RunSpeedTest(CancellationToken ct)
	{
		int downloadTimeout = 10;
		int externalControllerPort = _config.externalControllerPort;
		string speedTestUrl = _config.speedTestUrl;
		int localPort = _config.GetLocalPort("speedtest");
		string uri = "http://127.0.0.1:" + externalControllerPort + "/proxies/GLOBAL";
		WebProxy webProxy = new WebProxy("127.0.0.1", 40000);
		foreach (int selected in _selecteds)
		{
			int originalIndex = selected;
			int currentNodeIndex = GetCurrentNodeIndex(originalIndex);
			if (currentNodeIndex < 0)
			{
				continue;
			}
			DownloadHandle downloadHandle = new DownloadHandle();
			downloadHandle.UpdateCompleted += delegate(object sender2, DownloadHandle.ResultEventArgs args)
			{
				int currentNodeIndex4 = GetCurrentNodeIndex(originalIndex);
				if (currentNodeIndex4 >= 0 && currentNodeIndex4 < _config.vmess.Count)
				{
					string[] array = args.Msg.Split('|');
					if (array.Length > 1)
					{
						_updateMaxFunc(currentNodeIndex4, array[1]);
					}
					_updateFunc(currentNodeIndex4, array[0]);
				}
			};
			downloadHandle.Error += delegate(object sender2, ErrorEventArgs args)
			{
				int currentNodeIndex4 = GetCurrentNodeIndex(originalIndex);
				if (currentNodeIndex4 >= 0 && currentNodeIndex4 < _config.vmess.Count)
				{
					_updateFunc(currentNodeIndex4, args.GetException().Message);
				}
			};
			try
			{
				if (ct.IsCancellationRequested)
				{
					throw new OperationCanceledException();
				}
				try
				{
					ServicePointManager.FindServicePoint(new Uri(speedTestUrl)).CloseConnectionGroup("");
				}
				catch
				{
				}
				string text = (localPort + currentNodeIndex).ToString();
				if (ConfigHandler.sendReq("{\"name\":\"" + text + "\"}", uri, "PUT") == "204")
				{
					Thread.Sleep(500);
					downloadHandle.DownloadFileAsync(speedTestUrl, webProxy, downloadTimeout, _config.fastMode, int.Parse(_config.FMSecond), (int)(double.Parse(_config.FMmax) * 1024.0), int.Parse(_config.FMave));
					try
					{
						ServicePointManager.FindServicePoint(new Uri(speedTestUrl)).CloseConnectionGroup("");
					}
					catch
					{
					}
				}
				else
				{
					int currentNodeIndex2 = GetCurrentNodeIndex(originalIndex);
					if (currentNodeIndex2 >= 0 && currentNodeIndex2 < _config.vmess.Count)
					{
						_updateFunc(currentNodeIndex2, "切换节点失败");
					}
				}
			}
			catch (Exception)
			{
				int currentNodeIndex3 = GetCurrentNodeIndex(originalIndex);
				if (currentNodeIndex3 >= 0 && currentNodeIndex3 < _config.vmess.Count)
				{
					_updateFunc(currentNodeIndex3, "测速被取消");
				}
			}
		}
		_btStopTestStat(obj: false);
		_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "测速执行完成！");
	}

	public void RunSpeedTest2(CancellationToken ct)
	{
		_ = string.Empty;
		int timeout = 10;
		_ = _config.externalControllerPort;
		string speedTestUrl = _config.speedTestUrl;
		int httpPort = _config.GetLocalPort("speedtest");
		int num = int.Parse(_config.DownloadThread);
		if (!ThreadPool.SetMinThreads(num, num))
		{
			_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "线程设置失败！将由系统默认分配线程");
		}
		List<Action> list = new List<Action>();
		foreach (int selected in _selecteds)
		{
			int originalIndex = selected;
			list.Add(delegate
			{
				try
				{
					if (ct.IsCancellationRequested)
					{
						throw new OperationCanceledException();
					}
					int currentNodeIndex = GetCurrentNodeIndex(originalIndex);
					if (currentNodeIndex >= 0)
					{
						DownloadHandle downloadHandle = new DownloadHandle();
						downloadHandle.UpdateCompleted += delegate(object sender2, DownloadHandle.ResultEventArgs args)
						{
							int currentNodeIndex3 = GetCurrentNodeIndex(originalIndex);
							if (currentNodeIndex3 >= 0 && currentNodeIndex3 < _config.vmess.Count)
							{
								string[] array = args.Msg.Split('|');
								if (array.Length > 1)
								{
									_updateMaxFunc(currentNodeIndex3, array[1]);
								}
								_updateFunc(currentNodeIndex3, array[0]);
							}
						};
						downloadHandle.Error += delegate(object sender2, ErrorEventArgs args)
						{
							int currentNodeIndex3 = GetCurrentNodeIndex(originalIndex);
							if (currentNodeIndex3 >= 0 && currentNodeIndex3 < _config.vmess.Count)
							{
								_updateFunc(currentNodeIndex3, args.GetException().Message);
							}
						};
						downloadHandle.DownloadFileAsync(webProxy: new WebProxy("127.0.0.1", httpPort + currentNodeIndex), url: speedTestUrl, downloadTimeout: timeout, mode: _config.fastMode, second: int.Parse(_config.FMSecond), max: (int)(double.Parse(_config.FMmax) * 1024.0), ave: int.Parse(_config.FMave));
					}
				}
				catch (Exception)
				{
					int currentNodeIndex2 = GetCurrentNodeIndex(originalIndex);
					if (currentNodeIndex2 >= 0 && currentNodeIndex2 < _config.vmess.Count)
					{
						_updateFunc(currentNodeIndex2, "测速被取消");
					}
				}
			});
		}
		Parallel.Invoke(new ParallelOptions
		{
			MaxDegreeOfParallelism = num
		}, list.ToArray());
		if (!ThreadPool.SetMinThreads(Environment.ProcessorCount, Environment.ProcessorCount))
		{
			_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "线程设置失败！将由系统默认分配线程");
		}
		if (_pid > 0)
		{
			_v2rayHandler.V2rayStopPid(_pid);
		}
		_btStopTestStat(obj: false);
		_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "测速执行完成！");
	}

	public void RunRealPing2(CancellationToken ct)
	{
		try
		{
			_ = string.Empty;
			int httpPort = _config.GetLocalPort("speedtest");
			int num = (int)(Convert.ToDouble(_config.Timeout) * 1000.0);
			int extendedTimeout = num * 2;
			int num2 = int.Parse(_config.Thread);
			if (!ThreadPool.SetMinThreads(num2, num2))
			{
				_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "线程设置失败！将由系统默认分配线程");
			}
			List<Action> list = new List<Action>();
			foreach (int selected in _selecteds)
			{
				int originalIndex = selected;
				list.Add(delegate
				{
					try
					{
						if (ct.IsCancellationRequested)
						{
							throw new OperationCanceledException();
						}
						int currentNodeIndex = GetCurrentNodeIndex(originalIndex);
						if (currentNodeIndex >= 0)
						{
							if (_updateHttpsDelayFunc != null)
							{
								_updateHttpsDelayFunc(currentNodeIndex, "正在测速...");
							}
							_ = httpPort;
							bool flag2 = currentNodeIndex >= 0 && currentNodeIndex < _config.vmess.Count && _config.vmess[currentNodeIndex].configType == 11;
							WebProxy webProxy = new WebProxy("127.0.0.1", httpPort + currentNodeIndex);
							int responseTime = -1;
							GetRealPingTime2(_config.speedPingTestUrl, extendedTimeout + 1000, webProxy, out responseTime);
							if (responseTime == -1 && flag2 && _config.ThreadNum == 0)
							{
								try
								{
									int num3 = httpPort + currentNodeIndex;
									string url = $"http://{_config.externalController}/proxies/{num3}/delay?timeout={extendedTimeout}&url={_config.speedPingTestUrl}";
									Match match = Regex.Match(GetRealPingTime(url, extendedTimeout + 1000), ".*delay.+:([0-9]{1,4})}");
									if (match.Success)
									{
										responseTime = int.Parse(match.Groups[1].Value);
									}
								}
								catch
								{
								}
							}
							if (currentNodeIndex >= 0 && currentNodeIndex < _config.vmess.Count)
							{
								string arg = FormatOut2(responseTime, "ms");
								if (_updateHttpsDelayFunc != null)
								{
									_updateHttpsDelayFunc(currentNodeIndex, arg);
								}
							}
						}
					}
					catch
					{
						int currentNodeIndex2 = GetCurrentNodeIndex(originalIndex);
						if (currentNodeIndex2 >= 0 && currentNodeIndex2 < _config.vmess.Count && _updateHttpsDelayFunc != null)
						{
							_updateHttpsDelayFunc(currentNodeIndex2, "测速被取消");
						}
					}
				});
			}
			Parallel.Invoke(new ParallelOptions
			{
				MaxDegreeOfParallelism = num2
			}, list.ToArray());
		}
		catch (Exception)
		{
		}
		finally
		{
			if (!ThreadPool.SetMinThreads(Environment.ProcessorCount, Environment.ProcessorCount))
			{
				_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "线程设置失败！将由系统默认分配线程");
			}
			if (_pid > 0)
			{
				_v2rayHandler.V2rayStopPid(_pid);
			}
			_btStopTestStat(obj: false);
			_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "测速执行完成！");
		}
	}

	public void RunRealPing(CancellationToken ct)
	{
		try
		{
			int httpPort = _config.GetLocalPort("speedtest");
			int timeOut = (int)(Convert.ToDouble(_config.Timeout) * 1000.0);
			int num = int.Parse(_config.Thread);
			if (!ThreadPool.SetMinThreads(num, num))
			{
				_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "线程设置失败！将由系统默认分配线程");
			}
			List<Task> list = new List<Task>();
			foreach (int selected in _selecteds)
			{
				int originalIndex = selected;
				list.Add(Task.Run(delegate
				{
					try
					{
						if (ct.IsCancellationRequested)
						{
							throw new OperationCanceledException();
						}
						int currentNodeIndex = GetCurrentNodeIndex(originalIndex);
						if (currentNodeIndex >= 0)
						{
							if (_updateHttpsDelayFunc != null)
							{
								_updateHttpsDelayFunc(currentNodeIndex, "正在测速...");
							}
							int num2 = httpPort + currentNodeIndex;
							string url = $"http://{_config.externalController}/proxies/{num2}/delay?timeout={timeOut}&url={_config.speedPingTestUrl}";
							string realPingTime = GetRealPingTime(url, timeOut + 1);
							string arg = FormatOut(realPingTime, "ms");
							if (currentNodeIndex >= 0 && currentNodeIndex < _config.vmess.Count && _updateHttpsDelayFunc != null)
							{
								_updateHttpsDelayFunc(currentNodeIndex, arg);
							}
						}
					}
					catch
					{
						int currentNodeIndex2 = GetCurrentNodeIndex(originalIndex);
						if (currentNodeIndex2 >= 0 && currentNodeIndex2 < _config.vmess.Count && _updateHttpsDelayFunc != null)
						{
							_updateHttpsDelayFunc(currentNodeIndex2, "测速被取消");
						}
					}
				}));
			}
			Task.WaitAll(list.ToArray());
		}
		catch (Exception)
		{
		}
		finally
		{
			_btStopTestStat(obj: false);
			_v2rayHandler.ShowMsg(updateToTrayTooltip: false, "测速执行完成！");
		}
	}

	public string GetRealPingTime(string url, int timeOut)
	{
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
			obj.Timeout = timeOut;
			using HttpWebResponse httpWebResponse = (HttpWebResponse)obj.GetResponse();
			using Stream stream = httpWebResponse.GetResponseStream();
			using StreamReader streamReader = new StreamReader(stream, Encoding.Default);
			return streamReader.ReadToEnd();
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string FormatOut(string time, string unit)
	{
		Match match = Regex.Match(time, ".*delay.+:([0-9]{1,4})}");
		if (!match.Success)
		{
			if (time.IndexOf("504") != -1)
			{
				return "超时";
			}
			if (time.IndexOf("503") != -1)
			{
				return "无法连接";
			}
			return time;
		}
		return match.Groups[1].Value + unit;
	}

	private string GetRealPingTime2(string url, int timeout, WebProxy webProxy, out int responseTime)
	{
		string result = string.Empty;
		responseTime = -1;
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Timeout = timeout;
			httpWebRequest.Proxy = webProxy;
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
			httpWebRequest.KeepAlive = false;
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.MaximumAutomaticRedirections = 3;
			httpWebRequest.ReadWriteTimeout = timeout;
			if (httpWebRequest.ServicePoint != null)
			{
				httpWebRequest.ServicePoint.ConnectionLeaseTimeout = 0;
				httpWebRequest.ServicePoint.MaxIdleTime = 0;
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			if (httpWebResponse.StatusCode != HttpStatusCode.OK && httpWebResponse.StatusCode != HttpStatusCode.NoContent)
			{
				result = httpWebResponse.StatusDescription;
			}
			stopwatch.Stop();
			responseTime = (int)stopwatch.Elapsed.TotalMilliseconds;
			httpWebResponse.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Status == WebExceptionStatus.Timeout) ? "超时" : ((ex.Status == WebExceptionStatus.ConnectFailure) ? "连接失败" : ((ex.Status != WebExceptionStatus.ProtocolError) ? ex.Message : ((!(ex.Response is HttpWebResponse httpWebResponse2)) ? "协议错误" : ("HTTP " + (int)httpWebResponse2.StatusCode + ": " + httpWebResponse2.StatusDescription)))));
		}
		catch (Exception ex2)
		{
			result = ex2.Message;
		}
		return result;
	}

	private string FormatOut2(object time, string unit)
	{
		if (time.ToString().Equals("-1"))
		{
			return "超时";
		}
		return $"{time}{unit}".PadLeft(8, ' ');
	}

	private bool IsNodeStillValid(int index)
	{
		if (!_nodeSnapshot.ContainsKey(index))
		{
			return false;
		}
		if (index < 0 || index >= _config.vmess.Count)
		{
			return false;
		}
		return _config.vmess[index] == _nodeSnapshot[index];
	}

	private int GetCurrentNodeIndex(int originalIndex)
	{
		if (!_nodeSnapshot.ContainsKey(originalIndex))
		{
			return -1;
		}
		VmessItem vmessItem = _nodeSnapshot[originalIndex];
		for (int i = 0; i < _config.vmess.Count; i++)
		{
			if (_config.vmess[i] == vmessItem)
			{
				return i;
			}
		}
		return -1;
	}
}
