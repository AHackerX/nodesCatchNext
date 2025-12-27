namespace nodesCatchNext.Mode;

public class Inbounds
{
	public string tag { get; set; }

	public int port { get; set; }

	public string listen { get; set; }

	public string protocol { get; set; }

	public Sniffing sniffing { get; set; }

	public Inboundsettings settings { get; set; }

	public StreamSettings streamSettings { get; set; }
}
