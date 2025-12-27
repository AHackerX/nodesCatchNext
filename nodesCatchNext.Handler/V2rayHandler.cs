using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

public class V2rayHandler
{
	private List<string> lstV2ray;

	private string coreUrl;

	private Process _process;

	public event ProcessDelegate ProcessEvent;

	public void LoadV2rayCore(Config config)
	{
		lstV2ray = new List<string> { "mihomo-nodes" };
		coreUrl = "https://github.com/MetaCubeX/mihomo/releases";
	}

	public void ShowMsg(bool updateToTrayTooltip, string msg)
	{
		this.ProcessEvent?.Invoke(updateToTrayTooltip, msg);
	}

	private void V2rayRestart()
	{
		V2rayStop();
		V2rayStart();
	}

	public void V2rayStart()
	{
		ShowMsg(updateToTrayTooltip: false, "启动服务内核...");
		try
		{
			string text = V2rayFindexe();
			if (text == "")
			{
				return;
			}
			Process process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = text,
					WorkingDirectory = Utils.StartupPath(),
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true,
					StandardOutputEncoding = Encoding.UTF8
				}
			};
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					string msg = e.Data + Environment.NewLine;
					ShowMsg(updateToTrayTooltip: false, msg);
				}
			};
			process.Start();
			process.PriorityClass = ProcessPriorityClass.High;
			process.BeginOutputReadLine();
			_process = process;
			if (process.WaitForExit(1000))
			{
				string text2 = process.StandardError.ReadToEnd();
				throw new Exception((!string.IsNullOrEmpty(text2)) ? text2 : ("进程启动后立即退出，退出代码: " + process.ExitCode));
			}
			Global.processJob.AddProcess(process.Handle);
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			ShowMsg(updateToTrayTooltip: true, message);
		}
	}

	public void V2rayStopPid(int pid)
	{
		try
		{
			Process processById = Process.GetProcessById(pid);
			KillProcess(processById);
		}
		catch (Exception)
		{
		}
	}

	private string V2rayFindexe()
	{
		string text = string.Empty;
		string fileName = "mihomo-nodes.exe";
		fileName = Utils.GetPath(fileName);
		if (File.Exists(fileName))
		{
			text = fileName;
		}
		if (Utils.IsNullOrEmpty(text))
		{
			string arch = Environment.Is64BitOperatingSystem ? "amd64" : "386";
			string msg = $"找不到Core，请下载对应版本：mihomo-windows-{arch}.zip\n下载地址: {coreUrl}\n下载后将 mihomo.exe 重命名为 mihomo-nodes.exe 放到程序目录";
			ShowMsg(updateToTrayTooltip: false, msg);
		}
		return text;
	}

	public void V2rayStop()
	{
		try
		{
			if (_process != null)
			{
				KillProcess(_process);
				_process.Dispose();
				_process = null;
				return;
			}
			foreach (string item in lstV2ray)
			{
				Process[] processesByName = Process.GetProcessesByName(item);
				foreach (Process process in processesByName)
				{
					if (process.MainModule.FileName == Utils.GetPath(item) + ".exe")
					{
						KillProcess(process);
					}
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void KillProcess(Process p)
	{
		try
		{
			p.CloseMainWindow();
			p.WaitForExit(100);
			if (!p.HasExited)
			{
				p.Kill();
				p.WaitForExit(100);
			}
		}
		catch (Exception)
		{
		}
	}

	public int ClashStart(string configStr)
	{
		ShowMsg(updateToTrayTooltip: false, "启动Mihomo内核(" + DateTime.Now.ToString() + ")...");
		try
		{
			string text = V2rayFindexe();
			if (text == "")
			{
				ShowMsg(updateToTrayTooltip: false, "错误：找不到 mihomo-nodes.exe，请将 Mihomo 内核文件放到程序目录下并重命名为 mihomo-nodes.exe");
				return -1;
			}
			Process process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = text,
					Arguments = configStr,
					WorkingDirectory = Utils.StartupPath(),
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true,
					StandardOutputEncoding = Encoding.UTF8,
					StandardErrorEncoding = Encoding.UTF8
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					string msg = e.Data + Environment.NewLine;
					ShowMsg(updateToTrayTooltip: false, msg);
				}
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					string msg = "[错误] " + e.Data + Environment.NewLine;
					ShowMsg(updateToTrayTooltip: false, msg);
				}
			};
			
			// 等待一小段时间检查进程是否正常启动
			System.Threading.Thread.Sleep(1000);
			if (process.HasExited)
			{
				string errorHint = "";
				switch (process.ExitCode)
				{
					case -1073741515: // 0xC0000135
						errorHint = "缺少必要的DLL文件，请安装 Visual C++ Redistributable";
						break;
					case -1073741701: // 0xC000007B
						errorHint = "程序架构不匹配，请确认下载了正确的版本（amd64/arm64）";
						break;
					case -1073740791: // 0xC0000409
						errorHint = "程序崩溃，可能是系统版本过低，建议使用 Windows 10 1809 或更高版本";
						break;
					default:
						errorHint = "请检查：1.系统版本是否过低 2.是否安装了VC++运行库 3.下载的内核架构是否正确(amd64)";
						break;
				}
				ShowMsg(updateToTrayTooltip: false, $"Mihomo内核启动失败！退出代码: {process.ExitCode} (0x{process.ExitCode:X})\n{errorHint}");
				return -1;
			}
			
			Global.processJob.AddProcess(process.Handle);
			ShowMsg(updateToTrayTooltip: false, $"启动成功！进程ID：{process.Id}");
			return process.Id;
		}
		catch (Exception ex)
		{
			ShowMsg(updateToTrayTooltip: false, "Mihomo内核启动异常：" + ex.Message);
			return -1;
		}
	}
}
