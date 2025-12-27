using System.Collections.Generic;

namespace nodesCatchNext.Mode;

public class ServersItem
{
	public string email { get; set; }

	public string address { get; set; }

	public string method { get; set; }

	public bool ota { get; set; }

	public string password { get; set; }

	public int port { get; set; }

	public int level { get; set; }

	public List<SocksUsersItem> users { get; set; }

	public string flow { get; set; }
}
