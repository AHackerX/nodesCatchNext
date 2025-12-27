using System.Collections.Generic;

namespace nodesCatchNext.Mode;

public class VnextItem
{
	public string address { get; set; }

	public int port { get; set; }

	public List<UsersItem> users { get; set; }
}
