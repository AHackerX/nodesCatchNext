using Newtonsoft.Json;

namespace nodesCatchNext.Mode;

public class RealitySettings
{
	public string[] serverNames { get; set; }

	public string publicKey { get; set; }

	public string[] shortIds { get; set; }

	public string fingerprint { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string spiderX { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public bool? show { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string dest { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public int? xver { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string privateKey { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string minClientVer { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string maxClientVer { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public int? maxTimeDiff { get; set; }
}
