using System;

namespace nodesCatchNext;

internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
{
	public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;

	public IO_COUNTERS IoInfo;

	public UIntPtr ProcessMemoryLimit;

	public UIntPtr JobMemoryLimit;

	public UIntPtr PeakProcessMemoryUsed;

	public UIntPtr PeakJobMemoryUsed;
}
