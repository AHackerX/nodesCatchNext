using System.Collections.Generic;

namespace nodesCatchNext.Mode;

public class V2rayConfig
{
	public Policy policy;

	public Log log { get; set; }

	public List<Inbounds> inbounds { get; set; }

	public List<Outbounds> outbounds { get; set; }

	public Stats stats { get; set; }

	public API api { get; set; }

	public object dns { get; set; }

	public Routing routing { get; set; }
}
