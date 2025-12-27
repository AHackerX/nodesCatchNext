using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace nodesCatchNext;

public class Job : IDisposable
{
	private IntPtr handle = IntPtr.Zero;

	private bool disposed;

	public Job()
	{
		handle = CreateJobObject(IntPtr.Zero, null);
		IntPtr intPtr = IntPtr.Zero;
		JOBOBJECT_BASIC_LIMIT_INFORMATION basicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
		{
			LimitFlags = 8192u
		};
		JOBOBJECT_EXTENDED_LIMIT_INFORMATION structure = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
		{
			BasicLimitInformation = basicLimitInformation
		};
		try
		{
			int num = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
			if (!SetInformationJobObject(handle, JobObjectInfoType.ExtendedLimitInformation, intPtr, (uint)num))
			{
				throw new Exception($"Unable to set information.  Error: {Marshal.GetLastWin32Error()}");
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public bool AddProcess(IntPtr processHandle)
	{
		return AssignProcessToJobObject(handle, processHandle);
	}

	public bool AddProcess(int processId)
	{
		return AddProcess(Process.GetProcessById(processId).Handle);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;
			if (handle != IntPtr.Zero)
			{
				CloseHandle(handle);
				handle = IntPtr.Zero;
			}
		}
	}

	~Job()
	{
		Dispose(disposing: false);
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	private static extern IntPtr CreateJobObject(IntPtr a, string lpName);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CloseHandle(IntPtr hObject);
}
