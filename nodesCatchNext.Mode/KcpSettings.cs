namespace nodesCatchNext.Mode;

public class KcpSettings
{
	public int mtu { get; set; }

	public int tti { get; set; }

	public int uplinkCapacity { get; set; }

	public int downlinkCapacity { get; set; }

	public bool congestion { get; set; }

	public int readBufferSize { get; set; }

	public int writeBufferSize { get; set; }

	public Header header { get; set; }

	public string seed { get; set; }
}
