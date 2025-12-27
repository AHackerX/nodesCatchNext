using System;
using System.Threading;
using System.Windows.Forms;
using nodesCatchNext.Forms;

namespace nodesCatchNext;

internal static class Program
{
	private static Mutex mutex;

	[STAThread]
	private static void Main()
	{
		mutex = new Mutex(initiallyOwned: true, "Global\\nodesCatchNext_SingleInstance_Mutex_3F8A9D2B", out var createdNew);
		if (!createdNew)
		{
			MessageBox.Show("nodesCatchNext 已经在运行中！\n\n请检查任务栏或系统托盘。", "程序已运行", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.Run(new MainForm());
		}
		finally
		{
			if (mutex != null)
			{
				mutex.ReleaseMutex();
				mutex.Dispose();
			}
		}
	}
}
