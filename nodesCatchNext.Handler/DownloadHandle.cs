using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Timers;

namespace nodesCatchNext.Handler;

internal class DownloadHandle
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

	private ManualResetEvent allDone = new ManualResetEvent(initialState: false);

	private WebClient client;

	private long MaxSpeed;

	private long aveSpeed;

	private System.Timers.Timer timer;

	private System.Timers.Timer timer2;

	private bool fastMode;

	private int fmSecond;

	private int fmMax;

	private int fmAve;

	private DateTime startTime;

	private DateTime lastUpdate;

	private DateTime lastUIUpdate;

	private long lastBytes;

	private long bytes;

	private double percentage;

	private string speedSecond = "";

	public event EventHandler<ResultEventArgs> UpdateCompleted;

	public event ErrorEventHandler Error;

	public void DownloadFileAsync(string url, WebProxy webProxy, int downloadTimeout, bool mode, int second, int max, int ave)
	{
		allDone.Reset();
		fastMode = mode;
		fmSecond = second;
		fmMax = max;
		fmAve = ave;
		MaxSpeed = 0L;
		aveSpeed = 0L;
		Utils.SetSecurityProtocol();
		ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
		client = new NoKeepAliveWebClient();
		client.Proxy = webProxy;
		client.DownloadProgressChanged += client_DownloadProgressChanged;
		client.DownloadDataCompleted += client_DownloadDataCompleted;
		timer = new System.Timers.Timer(downloadTimeout * 1000);
		timer.Elapsed += delegate
		{
			client.CancelAsync();
		};
		timer.Start();
		if (fastMode)
		{
			timer2 = new System.Timers.Timer(fmSecond * 1000);
			timer2.AutoReset = false;
			timer2.Elapsed += delegate
			{
				if (percentage < (double)fmAve || MaxSpeed < fmMax)
				{
					client.CancelAsync();
				}
			};
			timer2.Start();
		}
		this.UpdateCompleted?.Invoke(this, new ResultEventArgs(success: false, "正在下载..."));
		Uri address = new Uri(url);
		startTime = DateTime.Now;
		client.DownloadDataAsync(address);
		allDone.WaitOne();
	}

	private void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
	{
		if (aveSpeed == 0L && e.Error != null)
		{
			this.UpdateCompleted(this, new ResultEventArgs(success: false, e.Error.Message));
		}
		else
		{
			if (bytes - lastBytes > 0 && (bytes - lastBytes) / 1024 > MaxSpeed)
			{
				MaxSpeed = (bytes - lastBytes) / 1024;
			}
			string text = ((double)(MaxSpeed + 1) / 1024.0).ToString("f2") + " MB/s";
			if (aveSpeed > 0)
			{
				TimeSpan timeSpan = DateTime.Now - startTime;
				aveSpeed = (long)((double)(aveSpeed / 1024) / timeSpan.TotalSeconds);
			}
			string text2 = ((double)(aveSpeed + 1) / 1024.0).ToString("f2") + " MB/s";
			this.UpdateCompleted(this, new ResultEventArgs(success: false, text2 + "|" + text));
		}
		client.Dispose();
		timer.Dispose();
		if (fastMode)
		{
			timer2.Dispose();
		}
		lastBytes = 0L;
		bytes = 0L;
		MaxSpeed = 0L;
		aveSpeed = 0L;
		speedSecond = "";
		percentage = 0.0;
		allDone.Set();
	}

	private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		bytes = e.BytesReceived;
		if (lastBytes == 0L)
		{
			lastUpdate = DateTime.Now;
			lastUIUpdate = DateTime.Now;
			lastBytes = bytes;
			return;
		}
		DateTime now = DateTime.Now;
		if ((now - lastUpdate).Seconds == 1)
		{
			long num = (bytes - lastBytes) / 1024;
			if (num > MaxSpeed)
			{
				MaxSpeed = num;
			}
			if (num > 1024)
			{
				speedSecond = ((double)num / 1024.0).ToString("f2") + " MB/s";
			}
			else
			{
				speedSecond = num + " KB/s";
			}
			lastBytes = bytes;
			lastUpdate = now;
		}
		aveSpeed = e.BytesReceived;
		percentage = (double)e.BytesReceived / (double)e.TotalBytesToReceive * 100.0;
		if ((now - lastUIUpdate).TotalMilliseconds >= 200.0)
		{
			if (percentage >= 1.0)
			{
				this.UpdateCompleted(this, new ResultEventArgs(success: false, percentage.ToString("#") + "%|" + speedSecond));
				lastUIUpdate = now;
			}
			else if (speedSecond != "")
			{
				this.UpdateCompleted(this, new ResultEventArgs(success: false, "正在下载...|" + speedSecond));
				lastUIUpdate = now;
			}
		}
	}

	public void WebDownloadString(string url, WebProxy proxy = null)
	{
		_ = string.Empty;
		System.Timers.Timer timer = new System.Timers.Timer(20000.0);
		try
		{
			Utils.SetSecurityProtocol();
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
			WebClient ws = new WebClient();
			ws.Headers.Add("User-Agent", "mihomo/1.19.12");
			if (proxy != null)
			{
				ws.Proxy = proxy;
			}
			else
			{
				ws.Proxy = WebRequest.GetSystemWebProxy();
			}
			ws.DownloadStringCompleted += Ws_DownloadStringCompleted;
			timer.Elapsed += delegate
			{
				ws.CancelAsync();
			};
			timer.Start();
			ws.Encoding = Encoding.UTF8;
			ws.DownloadStringAsync(new Uri(url));
		}
		catch (Exception exception)
		{
			timer.Dispose();
			this.Error?.Invoke(this, new ErrorEventArgs(exception));
		}
	}

	private void Ws_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
	{
		try
		{
			if (e.Error == null || Utils.IsNullOrEmpty(e.Error.ToString()))
			{
				string result = e.Result;
				this.UpdateCompleted?.Invoke(this, new ResultEventArgs(success: true, result));
				return;
			}
			throw e.Error;
		}
		catch (Exception exception)
		{
			this.Error?.Invoke(this, new ErrorEventArgs(exception));
		}
	}
}
