using System.Collections.Generic;

namespace nodesCatchNext.Mode;

public class Outboundsettings
{
	public List<VnextItem> vnext { get; set; }

	public List<ServersItem> servers { get; set; }

	public Response response { get; set; }
}
