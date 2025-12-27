using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

public class SubconverHandler
{
	private List<string> lstSubconver;

	private string coreUrl = "https://github.com/asdlokj1qpi233/subconverter";

	private Process _process;

	public event ProcessDelegate ProcessEvent;

	public void LoadSubconverCore(Config config)
	{
		lstSubconver = new List<string> { "subconverter" };
		coreUrl = "https://github.com/asdlokj1qpi233/subconverter";
	}

	public void ShowMsg(bool updateToTrayTooltip, string msg)
	{
		this.ProcessEvent?.Invoke(updateToTrayTooltip, msg);
	}

	public void SubconverStopPid(int pid)
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

	private string SubconverFindexe()
	{
		string text = string.Empty;
		string fileName = "subconverter\\subconverter.exe";
		fileName = Utils.GetPath(fileName);
		if (File.Exists(fileName))
		{
			text = fileName;
		}
		if (Utils.IsNullOrEmpty(text))
		{
			string msg = "找不到subconverter，下载地址: " + coreUrl;
			ShowMsg(updateToTrayTooltip: false, msg);
		}
		return text;
	}

	public void SubconverStop()
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
			foreach (string item in lstSubconver)
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

	public int SubconverStartNew()
	{
		ShowMsg(updateToTrayTooltip: false, "启动Subconverter服务(" + DateTime.Now.ToString() + ")...");
		try
		{
			string text = SubconverFindexe();
			if (text == "")
			{
				return -1;
			}
			Process process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = text,
					WorkingDirectory = Utils.StartupPath(),
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					StandardOutputEncoding = Encoding.UTF8
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					string msg = e.Data + Environment.NewLine;
					ShowMsg(updateToTrayTooltip: false, msg);
				}
			};
			_process = process;
			Global.processJob.AddProcess(process.Handle);
			ShowMsg(updateToTrayTooltip: false, $"启动成功！进程ID：{process.Id}");
			return process.Id;
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			ShowMsg(updateToTrayTooltip: false, message);
			return -1;
		}
	}
}
