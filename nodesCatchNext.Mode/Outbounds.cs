namespace nodesCatchNext.Mode;

public class Outbounds
{
	public string tag { get; set; }

	public string protocol { get; set; }

	public Outboundsettings settings { get; set; }

	public StreamSettings streamSettings { get; set; }

	public Mux mux { get; set; }
}
