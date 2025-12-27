using System.Collections.Generic;

namespace nodesCatchNext.Mode;

public class Inboundsettings
{
	public string auth { get; set; }

	public bool udp { get; set; }

	public string ip { get; set; }

	public string address { get; set; }

	public List<UsersItem> clients { get; set; }

	public string decryption { get; set; }

	public bool allowTransparent { get; set; }
}
