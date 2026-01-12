using System.Collections.Generic;

namespace nodesCatchNext;

internal class Global
{
	public static readonly IEnumerable<string> ssSecuritys = new HashSet<string>
	{
		"aes-128-gcm", "aes-192-gcm", "aes-256-gcm", "aes-128-cfb", "aes-192-cfb", "aes-256-cfb", "aes-128-ctr", "aes-192-ctr", "aes-256-ctr", "rc4-md5",
		"chacha20-ietf", "xchacha20", "chacha20-ietf-poly1305", "xchacha20-ietf-poly1305", "none", "plain",
		// Shadowsocks 2022 (blake3) 加密方式
		"2022-blake3-aes-128-gcm", "2022-blake3-aes-256-gcm", "2022-blake3-chacha20-poly1305"
	};

	public static bool reloadV2ray { get; set; }

	public static Job processJob { get; set; }
}
