using System;

namespace nodesCatchNext.Mode;

[Serializable]
public class VmessItem
{
	public int configVersion { get; set; }

	public string address { get; set; }

	public int port { get; set; }

	public string id { get; set; }

	public int alterId { get; set; }

	public string security { get; set; }

	public string network { get; set; }

	public string remarks { get; set; }

	public string headerType { get; set; }

	public string requestHost { get; set; }

	public string path { get; set; }

	public string streamSecurity { get; set; }

	public string allowInsecure { get; set; }

	public int configType { get; set; }

	public string httpsDelay { get; set; }

	public string testResult { get; set; }

	public string MaxSpeed { get; set; }

	public string lastTestTime { get; set; }

	public string subid { get; set; }

	public string flow { get; set; }

	public string plugin { get; set; }

	public string sni { get; set; }

	public string publicKey { get; set; }

	public string shortId { get; set; }

	public string fingerprint { get; set; }

	public VmessItem()
	{
		configVersion = 1;
		address = string.Empty;
		port = 0;
		id = string.Empty;
		alterId = 0;
		security = string.Empty;
		network = string.Empty;
		remarks = string.Empty;
		headerType = string.Empty;
		requestHost = string.Empty;
		path = string.Empty;
		streamSecurity = string.Empty;
		allowInsecure = string.Empty;
		configType = 1;
		httpsDelay = string.Empty;
		testResult = string.Empty;
		lastTestTime = string.Empty;
		subid = string.Empty;
		flow = string.Empty;
		plugin = string.Empty;
		publicKey = string.Empty;
		shortId = string.Empty;
		fingerprint = string.Empty;
		sni = string.Empty;
	}

	public string getSubRemarks(Config config)
	{
		string empty = string.Empty;
		if (Utils.IsNullOrEmpty(subid))
		{
			return empty;
		}
		foreach (SubItem item in config.subItem)
		{
			if (item.id.EndsWith(subid))
			{
				return item.remarks;
			}
		}
		if (subid.Length <= 4)
		{
			return subid;
		}
		return subid.Substring(0, 4);
	}

	public string getItemId()
	{
		return Utils.Base64Encode($"{address}{port}{requestHost}{path}");
	}
}
