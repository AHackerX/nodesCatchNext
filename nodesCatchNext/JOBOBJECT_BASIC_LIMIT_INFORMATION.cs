using System;

namespace nodesCatchNext;

internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
{
	public long PerProcessUserTimeLimit;

	public long PerJobUserTimeLimit;

	public uint LimitFlags;

	public UIntPtr MinimumWorkingSetSize;

	public UIntPtr MaximumWorkingSetSize;

	public uint ActiveProcessLimit;

	public UIntPtr Affinity;

	public uint PriorityClass;

	public uint SchedulingClass;
}
