using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using nodesCatchNext.Base;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

internal class ShareHandler
{
	private static readonly Regex UrlFinder = new Regex("ss://(?<base64>[A-Za-z0-9+-/=_]+)(?:#(?<tag>\\S+))?", RegexOptions.IgnoreCase);

	private static readonly Regex DetailsParser = new Regex("^((?<method>.+?):(?<password>.*)@(?<hostname>.+?):(?<port>\\d+?))$", RegexOptions.IgnoreCase);

	private static readonly Regex StdVmessUserInfo = new Regex("^(?<network>[a-z]+)(\\+(?<streamSecurity>[a-z]+))?:(?<id>[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})-(?<alterId>[0-9]+)$");

	public static string GetShareUrl(Config config, int index, bool changRemark = false)
	{
		try
		{
			string result = string.Empty;
			string remarks = (config.GetLocalPort() + index).ToString();
			string remarks2 = "";
			VmessItem vmessItem = config.vmess[index];
			if (changRemark)
			{
				remarks2 = vmessItem.remarks;
				vmessItem.remarks = remarks;
			}
			// 保存 requestHost 原始值（用于 SSR 类型）
			string originalRequestHost = vmessItem.requestHost;
			if (vmessItem.configType == 1)
			{
				result = Utils.ToJson(new VmessQRCode
				{
					v = vmessItem.configVersion.ToString(),
					ps = vmessItem.remarks.TrimEx(),
					add = vmessItem.address,
					port = vmessItem.port.ToString(),
					id = vmessItem.id,
					aid = vmessItem.alterId.ToString(),
					scy = vmessItem.security,
					net = vmessItem.network,
					type = vmessItem.headerType,
					host = vmessItem.requestHost,
					path = vmessItem.path,
					tls = vmessItem.streamSecurity,
					sni = vmessItem.sni
				});
				result = Utils.Base64Encode(result);
				result = string.Format("{0}{1}", "vmess://", result);
			}
			else if (vmessItem.configType == 3)
			{
				string text = string.Empty;
				string empty = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					text = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				result = vmessItem.security + ":" + vmessItem.id;
				result = Utils.Base64Encode(result).Replace("=", "");
				result += $"@{vmessItem.address}:{vmessItem.port}";
				if (!Utils.IsNullOrEmpty(vmessItem.network))
				{
					// 拼接完整的 plugin 参数
					StringBuilder pluginBuilder = new StringBuilder();
					pluginBuilder.Append(vmessItem.network);
					if (vmessItem.network == "v2ray-plugin" && !Utils.IsNullOrEmpty(vmessItem.headerType))
					{
						pluginBuilder.Append(";mode=").Append(vmessItem.headerType);
					}
					else if (vmessItem.network == "obfs-local" && !Utils.IsNullOrEmpty(vmessItem.headerType))
					{
						pluginBuilder.Append(";obfs=").Append(vmessItem.headerType);
					}
					if (!Utils.IsNullOrEmpty(vmessItem.requestHost))
					{
						if (vmessItem.network == "v2ray-plugin")
						{
							pluginBuilder.Append(";host=").Append(vmessItem.requestHost);
						}
						else if (vmessItem.network == "obfs-local")
						{
							pluginBuilder.Append(";obfs-host=").Append(vmessItem.requestHost);
						}
					}
					if (!Utils.IsNullOrEmpty(vmessItem.path))
					{
						pluginBuilder.Append(";path=").Append(vmessItem.path);
					}
					if (vmessItem.streamSecurity == "tls")
					{
						pluginBuilder.Append(";tls");
					}
					empty = "/?plugin=" + Utils.UrlEncode(pluginBuilder.ToString());
					result = string.Format("{0}{1}{2}{3}", "ss://", result, empty, text);
				}
				else
				{
					result = string.Format("{0}{1}{2}", "ss://", result, text);
				}
			}
			else if (vmessItem.configType == 9)
			{
				// 防御空值：如果 id 为空，跳过该节点
				if (Utils.IsNullOrEmpty(vmessItem.id))
				{
					result = string.Empty;
				}
				else
				{
					if (Utils.IsNullOrEmpty(vmessItem.requestHost))
					{
						vmessItem.requestHost = "nodesCatchNext";
					}
					// SSR 链接格式: address:port:protocol:method:obfs:base64(password)/?remarks=...&protoparam=...&obfsparam=...&group=...
					// 字段映射: network=protocol, security=method(cipher), headerType=obfs
					// 防御空值：使用默认值
					string network = Utils.IsNullOrEmpty(vmessItem.network) ? "origin" : vmessItem.network;
					string security = Utils.IsNullOrEmpty(vmessItem.security) ? "none" : vmessItem.security;
					string headerType = Utils.IsNullOrEmpty(vmessItem.headerType) ? "plain" : vmessItem.headerType;
					result = $"{vmessItem.address}:{vmessItem.port}:{network}:{security}:{headerType}:{Utils.Base64EncodeUrlSafe(vmessItem.id)}/?remarks={Utils.Base64EncodeUrlSafe(vmessItem.remarks)}&protoparam={Utils.Base64EncodeUrlSafe(vmessItem.streamSecurity)}&obfsparam={Utils.Base64EncodeUrlSafe(vmessItem.sni)}&group={Utils.Base64EncodeUrlSafe(vmessItem.requestHost)}";
					result = Utils.Base64EncodeUrlSafe(result);
					result = string.Format("{0}{1}", "ssr://", result);
				}
			}
			else if (vmessItem.configType == 4)
			{
				string arg = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					arg = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				result = ((vmessItem.security == null || !(vmessItem.security != "")) ? $"{vmessItem.address}:{vmessItem.port}" : $"{vmessItem.security}:{vmessItem.id}@{vmessItem.address}:{vmessItem.port}");
				result = Utils.Base64Encode(result);
				result = string.Format("{0}{1}{2}", "socks://", result, arg);
			}
			else if (vmessItem.configType == 7)
			{
				string arg2 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					arg2 = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				result = ((vmessItem.security == null || !(vmessItem.security != "")) ? $"{vmessItem.address}:{vmessItem.port}" : $"{vmessItem.security}:{vmessItem.id}@{vmessItem.address}:{vmessItem.port}");
				result = Utils.Base64Encode(result);
				result = string.Format("{0}{1}{2}", "http://", result, arg2);
			}
			else if (vmessItem.configType == 8)
			{
				string arg3 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					arg3 = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				result = ((vmessItem.security == null || !(vmessItem.security != "")) ? $"{vmessItem.address}:{vmessItem.port}" : $"{vmessItem.security}:{vmessItem.id}@{vmessItem.address}:{vmessItem.port}");
				result = Utils.Base64Encode(result);
				result = string.Format("{0}{1}{2}", "https://", result, arg3);
			}
			else if (vmessItem.configType == 6)
			{
				string text2 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					text2 = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				string text3 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.allowInsecure))
				{
					text3 = ((!(vmessItem.allowInsecure == "false")) ? "?allowInsecure=1" : "?allowInsecure=0");
					if (!Utils.IsNullOrEmpty(vmessItem.sni))
					{
						text3 = text3 + "&sni=" + Utils.UrlEncode(vmessItem.sni);
					}
				}
				else if (!Utils.IsNullOrEmpty(vmessItem.sni))
				{
					text3 = "?sni=" + Utils.UrlEncode(vmessItem.sni);
				}
				result = $"{vmessItem.id}@{GetIpv6(vmessItem.address)}:{vmessItem.port}";
				result = string.Format("{0}{1}{2}{3}", "trojan://", result, text3, text2);
			}
			else if (vmessItem.configType == 5)
			{
				string text4 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					text4 = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (!Utils.IsNullOrEmpty(vmessItem.flow))
				{
					dictionary.Add("flow", vmessItem.flow);
				}
				if (!Utils.IsNullOrEmpty(vmessItem.security))
				{
					dictionary.Add("encryption", vmessItem.security);
				}
				else
				{
					dictionary.Add("encryption", "none");
				}
				if (!Utils.IsNullOrEmpty(vmessItem.streamSecurity))
				{
					dictionary.Add("security", vmessItem.streamSecurity);
				}
				else if (!Utils.IsNullOrEmpty(vmessItem.flow) && vmessItem.flow.Contains("vision"))
				{
					dictionary.Add("security", "reality");
				}
				else
				{
					dictionary.Add("security", "none");
				}
				if (!Utils.IsNullOrEmpty(vmessItem.sni))
				{
					dictionary.Add("sni", vmessItem.sni);
				}
				if (!Utils.IsNullOrEmpty(vmessItem.streamSecurity) && (vmessItem.streamSecurity == "tls" || vmessItem.streamSecurity == "reality" || !string.IsNullOrWhiteSpace(vmessItem.fingerprint)))
				{
					if (!Utils.IsNullOrEmpty(vmessItem.allowInsecure))
					{
						if (vmessItem.allowInsecure == "false" || vmessItem.allowInsecure == "0")
						{
							dictionary.Add("insecure", "0");
							dictionary.Add("allowInsecure", "0");
						}
						else
						{
							dictionary.Add("insecure", "1");
							dictionary.Add("allowInsecure", "1");
						}
					}
					else
					{
						dictionary.Add("insecure", "0");
						dictionary.Add("allowInsecure", "0");
					}
				}
				if (!Utils.IsNullOrEmpty(vmessItem.publicKey))
				{
					dictionary.Add("pbk", Utils.UrlEncode(vmessItem.publicKey));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.shortId))
				{
					dictionary.Add("sid", Utils.UrlEncode(vmessItem.shortId));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.fingerprint))
				{
					dictionary.Add("fp", Utils.UrlEncode(vmessItem.fingerprint));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.network))
				{
					dictionary.Add("type", vmessItem.network);
				}
				else
				{
					dictionary.Add("type", "tcp");
				}
				switch (vmessItem.network)
				{
				case "tcp":
					if (!Utils.IsNullOrEmpty(vmessItem.headerType))
					{
						dictionary.Add("headerType", vmessItem.headerType);
					}
					else
					{
						dictionary.Add("headerType", "none");
					}
					if (!Utils.IsNullOrEmpty(vmessItem.requestHost))
					{
						dictionary.Add("host", Utils.UrlEncode(vmessItem.requestHost));
					}
					break;
				case "kcp":
					if (!Utils.IsNullOrEmpty(vmessItem.headerType))
					{
						dictionary.Add("headerType", vmessItem.headerType);
					}
					else
					{
						dictionary.Add("headerType", "none");
					}
					if (!Utils.IsNullOrEmpty(vmessItem.path))
					{
						dictionary.Add("seed", Utils.UrlEncode(vmessItem.path));
					}
					break;
				case "ws":
					if (!Utils.IsNullOrEmpty(vmessItem.requestHost))
					{
						dictionary.Add("host", Utils.UrlEncode(vmessItem.requestHost));
					}
					if (!Utils.IsNullOrEmpty(vmessItem.path))
					{
						dictionary.Add("path", Utils.UrlEncode(vmessItem.path));
					}
					break;
				case "h2":
				case "http":
					dictionary["type"] = "http";
					if (!Utils.IsNullOrEmpty(vmessItem.requestHost))
					{
						dictionary.Add("host", Utils.UrlEncode(vmessItem.requestHost));
					}
					if (!Utils.IsNullOrEmpty(vmessItem.path))
					{
						dictionary.Add("path", Utils.UrlEncode(vmessItem.path));
					}
					break;
				case "quic":
					if (!Utils.IsNullOrEmpty(vmessItem.headerType))
					{
						dictionary.Add("headerType", vmessItem.headerType);
					}
					else
					{
						dictionary.Add("headerType", "none");
					}
					dictionary.Add("quicSecurity", Utils.UrlEncode(vmessItem.requestHost));
					dictionary.Add("key", Utils.UrlEncode(vmessItem.path));
					break;
				case "grpc":
					if (!Utils.IsNullOrEmpty(vmessItem.path))
					{
						dictionary.Add("serviceName", Utils.UrlEncode(vmessItem.path));
						if (vmessItem.headerType == "gun" || vmessItem.headerType == "multi")
						{
							dictionary.Add("mode", Utils.UrlEncode(vmessItem.headerType));
						}
					}
					break;
				}
				string text5 = "?" + string.Join("&", dictionary.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value).ToArray());
				result = $"{vmessItem.id}@{GetIpv6(vmessItem.address)}:{vmessItem.port}";
				result = string.Format("{0}{1}{2}{3}", "vless://", result, text5, text4);
			}
			else if (vmessItem.configType == 11)
			{
				string text6 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					text6 = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				if (!Utils.IsNullOrEmpty(vmessItem.sni))
				{
					dictionary2.Add("sni", Utils.UrlEncode(vmessItem.sni));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.path))
				{
					dictionary2.Add("obfs", Utils.UrlEncode(vmessItem.path));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.requestHost))
				{
					dictionary2.Add("obfs-password", Utils.UrlEncode(vmessItem.requestHost));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.publicKey))
				{
					dictionary2.Add("pinSHA256", Utils.UrlEncode(vmessItem.publicKey));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.allowInsecure))
				{
					if (vmessItem.allowInsecure == "false" || vmessItem.allowInsecure == "0")
					{
						dictionary2.Add("insecure", "0");
					}
					else
					{
						dictionary2.Add("insecure", "1");
					}
				}
				string text7 = ((dictionary2.Count > 0) ? ("?" + string.Join("&", dictionary2.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value).ToArray())) : "");
				string arg4 = (Utils.IsNullOrEmpty(vmessItem.id) ? "" : (Utils.UrlEncode(vmessItem.id) + "@"));
				int num = ((vmessItem.port > 0) ? vmessItem.port : 443);
				result = $"{arg4}{GetIpv6(vmessItem.address)}:{num}";
				result = string.Format("{0}{1}{2}{3}", "hy2://", result, text7, text6);
			}
			else if (vmessItem.configType == 12)
			{
				string text8 = string.Empty;
				if (!Utils.IsNullOrEmpty(vmessItem.remarks))
				{
					text8 = "#" + Utils.UrlEncode(vmessItem.remarks);
				}
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				if (!Utils.IsNullOrEmpty(vmessItem.sni))
				{
					dictionary3.Add("sni", Utils.UrlEncode(vmessItem.sni));
				}
				if (!Utils.IsNullOrEmpty(vmessItem.allowInsecure))
				{
					if (vmessItem.allowInsecure == "false" || vmessItem.allowInsecure == "0")
					{
						dictionary3.Add("insecure", "0");
					}
					else
					{
						dictionary3.Add("insecure", "1");
					}
				}
				string text9 = ((dictionary3.Count > 0) ? ("/?" + string.Join("&", dictionary3.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value).ToArray())) : "");
				string arg5 = (Utils.IsNullOrEmpty(vmessItem.id) ? "" : (Utils.UrlEncode(vmessItem.id) + "@"));
				int num2 = ((vmessItem.port > 0) ? vmessItem.port : 443);
				result = $"{arg5}{GetIpv6(vmessItem.address)}:{num2}";
				result = string.Format("{0}{1}{2}{3}", "anytls://", result, text9, text8);
			}
			if (changRemark)
			{
				vmessItem.remarks = remarks2;
				vmessItem.requestHost = originalRequestHost;
			}
			return result;
		}
		catch
		{
			return "";
		}
	}

	private static string GetIpv6(string address)
	{
		if (!Utils.IsIpv6(address))
		{
			return address;
		}
		return "[" + address + "]";
	}

	public static VmessItem ImportFromClipboardConfig(string clipboardData, out string msg)
	{
		msg = string.Empty;
		VmessItem vmessItem = new VmessItem();
		try
		{
			string text = clipboardData.TrimEx();
			if (Utils.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.StartsWith("vmess://"))
			{
				if (text.IndexOf("?") > 0)
				{
					vmessItem = ResolveStdVmess(text) ?? ResolveVmess4Kitsunebi(text);
				}
				else
				{
					vmessItem.configType = 1;
					text = text.Substring("vmess://".Length);
					text = Utils.Base64Decode(text);
					VmessQRCode vmessQRCode = Utils.FromJson<VmessQRCode>(text);
					if (vmessQRCode == null)
					{
						msg = "转换配置文件失败";
						return null;
					}
					vmessItem.network = "tcp";
					vmessItem.headerType = "none";
					vmessItem.configVersion = Utils.ToInt(vmessQRCode.v);
					vmessItem.remarks = Utils.ToString(vmessQRCode.ps);
					vmessItem.address = Utils.ToString(vmessQRCode.add);
					vmessItem.port = Utils.ToInt(vmessQRCode.port);
					vmessItem.id = Utils.ToString(vmessQRCode.id);
					vmessItem.alterId = Utils.ToInt(vmessQRCode.aid);
					vmessItem.security = Utils.ToString(vmessQRCode.scy);
					if (!Utils.IsNullOrEmpty(vmessQRCode.scy))
					{
						vmessItem.security = vmessQRCode.scy;
					}
					else
					{
						vmessItem.security = "auto";
					}
					if (!Utils.IsNullOrEmpty(vmessQRCode.net))
					{
						vmessItem.network = vmessQRCode.net;
					}
					if (!Utils.IsNullOrEmpty(vmessQRCode.type))
					{
						vmessItem.headerType = vmessQRCode.type;
					}
					vmessItem.requestHost = Utils.ToString(vmessQRCode.host);
					vmessItem.path = Utils.ToString(vmessQRCode.path);
					vmessItem.streamSecurity = Utils.ToString(vmessQRCode.tls);
					vmessItem.sni = Utils.ToString(vmessQRCode.sni);
				}
			}
			else if (text.StartsWith("ss://"))
			{
				msg = "配置格式不正确";
				vmessItem = ResolveSip002(text);
				if (vmessItem == null)
				{
					vmessItem = ResolveSSLegacy(text);
				}
				if (vmessItem == null)
				{
					return null;
				}
				if (vmessItem.address.Length == 0 || vmessItem.port == 0 || vmessItem.security.Length == 0 || vmessItem.id.Length == 0)
				{
					return null;
				}
				vmessItem.configType = 3;
			}
			else if (text.StartsWith("socks://"))
			{
				msg = "配置格式不正确";
				vmessItem.configType = 4;
				text = text.Substring("socks://".Length);
				int num = text.IndexOf("#");
				if (num > 0)
				{
					try
					{
						vmessItem.remarks = Utils.UrlDecode(text.Substring(num + 1, text.Length - num - 1));
					}
					catch
					{
					}
					text = text.Substring(0, num);
				}
				if (text.IndexOf(":") == -1)
				{
					text = Utils.Base64Decode(text);
				}
				string[] array = text.Split('@');
				if (array.Length == 2)
				{
					string[] array2 = array[0].Split(':');
					int num2 = array[1].LastIndexOf(":");
					if (array2.Length != 2 || num2 < 0)
					{
						return null;
					}
					vmessItem.address = array[1].Substring(0, num2);
					vmessItem.port = Utils.ToInt(array[1].Substring(num2 + 1, array[1].Length - (num2 + 1)));
					vmessItem.security = array2[0];
					vmessItem.id = array2[1];
				}
				else
				{
					if (array.Length != 1)
					{
						return null;
					}
					string[] array3 = array[0].Split(':');
					if (array3.Length != 2)
					{
						return null;
					}
					vmessItem.address = array3[0];
					vmessItem.port = Utils.ToInt(array3[1]);
				}
			}
			else if (text.StartsWith("http://"))
			{
				msg = "配置格式不正确";
				vmessItem.configType = 7;
				text = text.Substring("http://".Length);
				int num3 = text.IndexOf("#");
				if (num3 > 0)
				{
					try
					{
						vmessItem.remarks = Utils.UrlDecode(text.Substring(num3 + 1, text.Length - num3 - 1));
					}
					catch
					{
					}
					text = text.Substring(0, num3);
				}
				if (text.IndexOf(":") == -1)
				{
					text = Utils.Base64Decode(text);
				}
				string[] array4 = text.Split('@');
				if (array4.Length == 2)
				{
					string[] array5 = array4[0].Split(':');
					int num4 = array4[1].LastIndexOf(":");
					if (array5.Length != 2 || num4 < 0)
					{
						return null;
					}
					vmessItem.address = array4[1].Substring(0, num4);
					vmessItem.port = Utils.ToInt(array4[1].Substring(num4 + 1, array4[1].Length - (num4 + 1)));
					vmessItem.security = array5[0];
					vmessItem.id = array5[1];
				}
				else
				{
					if (array4.Length != 1)
					{
						return null;
					}
					string[] array6 = array4[0].Split(':');
					if (array6.Length != 2)
					{
						return null;
					}
					vmessItem.address = array6[0];
					vmessItem.port = Utils.ToInt(array6[1]);
				}
			}
			else if (text.StartsWith("https://"))
			{
				msg = "配置格式不正确";
				vmessItem.configType = 8;
				text = text.Substring("https://".Length);
				int num5 = text.IndexOf("#");
				if (num5 > 0)
				{
					try
					{
						vmessItem.remarks = Utils.UrlDecode(text.Substring(num5 + 1, text.Length - num5 - 1));
					}
					catch
					{
					}
					text = text.Substring(0, num5);
				}
				if (text.IndexOf(":") == -1)
				{
					text = Utils.Base64Decode(text);
				}
				string[] array7 = text.Split('@');
				if (array7.Length == 2)
				{
					string[] array8 = array7[0].Split(':');
					int num6 = array7[1].LastIndexOf(":");
					if (array8.Length != 2 || num6 < 0)
					{
						return null;
					}
					vmessItem.address = array7[1].Substring(0, num6);
					vmessItem.port = Utils.ToInt(array7[1].Substring(num6 + 1, array7[1].Length - (num6 + 1)));
					vmessItem.security = array8[0];
					vmessItem.id = array8[1];
				}
				else
				{
					if (array7.Length != 1)
					{
						return null;
					}
					string[] array9 = array7[0].Split(':');
					if (array9.Length != 2)
					{
						return null;
					}
					vmessItem.address = array9[0];
					vmessItem.port = Utils.ToInt(array9[1]);
				}
			}
			else if (text.StartsWith("trojan://"))
			{
				msg = "配置格式不正确";
				vmessItem.configType = 6;
				Uri uri = new Uri(text);
				vmessItem.address = uri.IdnHost;
				vmessItem.port = uri.Port;
				vmessItem.id = Utils.UrlDecode(uri.UserInfo);
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
				vmessItem.sni = Utils.UrlDecode(nameValueCollection["sni"] ?? "");
				if (nameValueCollection["allowInsecure"] != null)
				{
					if (nameValueCollection["allowInsecure"] == "0")
					{
						vmessItem.allowInsecure = "false";
					}
					else
					{
						vmessItem.allowInsecure = "true";
					}
				}
				string text2 = uri.Fragment.Replace("#", "");
				if (Utils.IsNullOrEmpty(text2))
				{
					vmessItem.remarks = "NONE";
				}
				else
				{
					vmessItem.remarks = Utils.UrlDecode(text2);
				}
			}
			else if (text.StartsWith("ssr://"))
			{
				msg = "配置格式不正确";
				vmessItem = ResolveSSRLegacy(text);
			}
			else if (text.StartsWith("hy2://") || text.StartsWith("hysteria2://"))
			{
				msg = "配置格式不正确";
				vmessItem = ResolveHysteria2(text);
			}
			else if (text.StartsWith("anytls://"))
			{
				msg = "配置格式不正确";
				vmessItem = ResolveAnyTLS(text);
			}
			else
			{
				if (!text.StartsWith("vless://"))
				{
					msg = "非支持的协议格式";
					return null;
				}
				vmessItem = ResolveStdVLESS(text);
			}
		}
		catch
		{
			msg = "不是正确的配置，请检查";
			return null;
		}
		return vmessItem;
	}

	private static VmessItem ResolveVmess4Kitsunebi(string result)
	{
		VmessItem vmessItem = new VmessItem
		{
			configType = 1
		};
		result = result.Substring("vmess://".Length);
		int num = result.IndexOf("?");
		if (num > 0)
		{
			result = result.Substring(0, num);
		}
		result = Utils.Base64Decode(result);
		string[] array = result.Split('@');
		if (array.Length != 2)
		{
			return null;
		}
		string[] array2 = array[0].Split(':');
		string[] array3 = array[1].Split(':');
		if (array2.Length != 2 || array2.Length != 2)
		{
			return null;
		}
		vmessItem.address = array3[0];
		vmessItem.port = Utils.ToInt(array3[1]);
		vmessItem.security = array2[0];
		vmessItem.id = array2[1];
		vmessItem.network = "tcp";
		vmessItem.headerType = "none";
		vmessItem.remarks = "Alien";
		vmessItem.alterId = 0;
		return vmessItem;
	}

	private static VmessItem ResolveSip002(string result)
	{
		Uri uri;
		try
		{
			uri = new Uri(result);
		}
		catch (UriFormatException)
		{
			return null;
		}
		VmessItem vmessItem = new VmessItem
		{
			remarks = uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped),
			address = uri.IdnHost,
			port = uri.Port
		};
		string text = uri.GetComponents(UriComponents.UserInfo, UriFormat.Unescaped).Replace('-', '+').Replace('_', '/');
		string text2;
		try
		{
			text2 = Encoding.UTF8.GetString(Convert.FromBase64String(text.PadRight(text.Length + (4 - text.Length % 4) % 4, '=')));
		}
		catch (FormatException)
		{
			return null;
		}
		// 使用 Split 限制为 2 部分，支持 SS2022 格式（密码中可能包含冒号）
		// SS2022 格式: method:iPSK:uPSK 或 method:base64password
		string[] array = text2.Split(new char[1] { ':' }, 2);
		if (array.Length != 2)
		{
			return null;
		}
		vmessItem.security = array[0];
		vmessItem.id = array[1];
		NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
		if (nameValueCollection["plugin"] != null)
		{
			string plugin = Utils.UrlDecode(nameValueCollection["plugin"]);
			string[] pluginParts = plugin.Split(';');
			if (pluginParts.Length > 0)
			{
				vmessItem.network = pluginParts[0]; // 插件名称，如 "v2ray-plugin" 或 "obfs-local"
				for (int i = 1; i < pluginParts.Length; i++)
				{
					string[] kv = pluginParts[i].Split(new char[] { '=' }, 2);
					string key = kv[0].Trim();
					string value = kv.Length > 1 ? kv[1].Trim() : "";
					switch (key)
					{
						case "mode":
							vmessItem.headerType = value; // v2ray-plugin 的 mode
							break;
						case "host":
							vmessItem.requestHost = value;
							break;
						case "path":
							vmessItem.path = value;
							break;
						case "tls":
							vmessItem.streamSecurity = "tls";
							break;
						case "obfs":
							vmessItem.headerType = value; // obfs-local 的 obfs 类型
							break;
						case "obfs-host":
							vmessItem.requestHost = value;
							break;
					}
				}
				// v2ray-plugin 不带子参数时默认 mode 为 websocket
				if (vmessItem.network == "v2ray-plugin" && Utils.IsNullOrEmpty(vmessItem.headerType))
				{
					vmessItem.headerType = "websocket";
				}
				// obfs-local 默认 obfs 为 http
				if (vmessItem.network == "obfs-local" && Utils.IsNullOrEmpty(vmessItem.headerType))
				{
					vmessItem.headerType = "http";
				}
			}
		}
		return vmessItem;
	}

	private static VmessItem ResolveSSLegacy(string result)
	{
		Match match = UrlFinder.Match(result);
		if (!match.Success)
		{
			return null;
		}
		VmessItem vmessItem = new VmessItem();
		string text = match.Groups["base64"].Value.TrimEnd('/');
		string value = match.Groups["tag"].Value;
		if (!Utils.IsNullOrEmpty(value))
		{
			vmessItem.remarks = Utils.UrlDecode(value);
		}
		Match match2;
		try
		{
			match2 = DetailsParser.Match(Encoding.UTF8.GetString(Convert.FromBase64String(text.PadRight(text.Length + (4 - text.Length % 4) % 4, '='))));
		}
		catch (FormatException)
		{
			return null;
		}
		if (!match2.Success)
		{
			return null;
		}
		vmessItem.security = match2.Groups["method"].Value;
		vmessItem.id = match2.Groups["password"].Value;
		vmessItem.address = match2.Groups["hostname"].Value;
		vmessItem.port = int.Parse(match2.Groups["port"].Value);
		return vmessItem;
	}

	private static VmessItem ResolveSSRLegacy(string result)
	{
		VmessItem vmessItem = new VmessItem
		{
			configType = 9
		};
		result = result.Substring("ssr://".Length);
		result = Utils.Base64DecodeUrlSafe(result);
		if (result.IsNullOrEmpty())
		{
			return null;
		}
		string[] array = result.Split(new string[1] { "/?" }, StringSplitOptions.None);
		if (array.Length < 2)
		{
			return null;
		}
		string[] array2 = array[0].Split(':');
		string[] array3 = array[1].Split('&');
		vmessItem.address = array2[0];
		vmessItem.port = Utils.ToInt(array2[1]);
		// SSR 链接格式: address:port:protocol:method:obfs:password
		// 字段映射: network=protocol, security=method(cipher), headerType=obfs
		vmessItem.network = array2[2];      // protocol
		vmessItem.security = array2[3];     // method (cipher)
		vmessItem.headerType = array2[4];   // obfs
		vmessItem.id = Utils.Base64Decode(array2[5]);
		string[] array4 = null;
		string[] array5 = array3;
		for (int i = 0; i < array5.Length; i++)
		{
			array4 = array5[i].Split('=');
			switch (array4[0])
			{
			case "remarks":
				vmessItem.remarks = Utils.Base64DecodeUrlSafe(array4[1]) ?? "none";
				break;
			case "protoparam":
				vmessItem.streamSecurity = Utils.Base64DecodeUrlSafe(array4[1]);
				break;
			case "obfsparam":
				vmessItem.sni = Utils.Base64DecodeUrlSafe(array4[1]);
				break;
			case "group":
				vmessItem.requestHost = Utils.Base64DecodeUrlSafe(array4[1]);
				break;
			}
		}
		return vmessItem;
	}

	private static VmessItem ResolveStdVmess(string result)
	{
		VmessItem vmessItem = new VmessItem
		{
			configType = 1,
			security = "auto"
		};
		Uri uri = new Uri(result);
		vmessItem.address = uri.IdnHost;
		vmessItem.port = uri.Port;
		vmessItem.remarks = uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
		NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
		Match match = StdVmessUserInfo.Match(uri.UserInfo);
		if (!match.Success)
		{
			return null;
		}
		vmessItem.id = match.Groups["id"].Value;
		if (!int.TryParse(match.Groups["alterId"].Value, out var result2))
		{
			return null;
		}
		vmessItem.alterId = result2;
		if (match.Groups["streamSecurity"].Success)
		{
			vmessItem.streamSecurity = match.Groups["streamSecurity"].Value;
		}
		if (!(vmessItem.streamSecurity == "tls") && !string.IsNullOrWhiteSpace(vmessItem.streamSecurity))
		{
			return null;
		}
		vmessItem.network = match.Groups["network"].Value;
		switch (vmessItem.network)
		{
		case "tcp":
		{
			string headerType2 = nameValueCollection["type"] ?? "none";
			vmessItem.headerType = headerType2;
			break;
		}
		case "kcp":
			vmessItem.headerType = nameValueCollection["type"] ?? "none";
			break;
		case "ws":
		{
			string path2 = nameValueCollection["path"] ?? "/";
			string url2 = nameValueCollection["host"] ?? "";
			vmessItem.requestHost = Utils.UrlDecode(url2);
			vmessItem.path = path2;
			break;
		}
		case "http":
		case "h2":
		{
			vmessItem.network = "h2";
			string path3 = nameValueCollection["path"] ?? "/";
			string url3 = nameValueCollection["host"] ?? "";
			vmessItem.requestHost = Utils.UrlDecode(url3);
			vmessItem.path = path3;
			break;
		}
		case "quic":
		{
			string url = nameValueCollection["security"] ?? "none";
			string path = nameValueCollection["key"] ?? "";
			string headerType = nameValueCollection["type"] ?? "none";
			vmessItem.headerType = headerType;
			vmessItem.requestHost = Utils.UrlDecode(url);
			vmessItem.path = path;
			break;
		}
		default:
			return null;
		}
		return vmessItem;
	}

	private static VmessItem ResolveStdVLESS(string result)
	{
		VmessItem vmessItem = new VmessItem
		{
			configType = 5,
			security = "none"
		};
		Uri uri = new Uri(result);
		vmessItem.address = uri.IdnHost;
		vmessItem.port = uri.Port;
		vmessItem.remarks = uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
		vmessItem.id = Utils.UrlDecode(uri.UserInfo);
		NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
		vmessItem.flow = nameValueCollection["flow"] ?? "";
		vmessItem.security = nameValueCollection["encryption"] ?? "none";
		vmessItem.streamSecurity = nameValueCollection["security"] ?? "";
		vmessItem.sni = Utils.UrlDecode(nameValueCollection["sni"] ?? "");
		vmessItem.network = nameValueCollection["type"] ?? "tcp";
		vmessItem.publicKey = Utils.UrlDecode(nameValueCollection["pbk"] ?? nameValueCollection["publicKey"] ?? "");
		vmessItem.shortId = Utils.UrlDecode(nameValueCollection["sid"] ?? nameValueCollection["shortId"] ?? "");
		vmessItem.fingerprint = Utils.UrlDecode(nameValueCollection["fp"] ?? nameValueCollection["fingerprint"] ?? "");
		if (nameValueCollection["allowInsecure"] != null || nameValueCollection["insecure"] != null)
		{
			string text = nameValueCollection["allowInsecure"] ?? nameValueCollection["insecure"];
			if (text == "1" || text == "true")
			{
				vmessItem.allowInsecure = "true";
			}
			else
			{
				vmessItem.allowInsecure = "false";
			}
		}
		if ((!string.IsNullOrWhiteSpace(vmessItem.publicKey) || !string.IsNullOrWhiteSpace(vmessItem.shortId)) && string.IsNullOrWhiteSpace(vmessItem.streamSecurity))
		{
			vmessItem.streamSecurity = "reality";
		}
		switch (vmessItem.network)
		{
		case "tcp":
			vmessItem.headerType = nameValueCollection["headerType"] ?? "none";
			vmessItem.requestHost = Utils.UrlDecode(nameValueCollection["host"] ?? "");
			break;
		case "kcp":
			vmessItem.headerType = nameValueCollection["headerType"] ?? "none";
			vmessItem.path = Utils.UrlDecode(nameValueCollection["seed"] ?? "");
			break;
		case "ws":
		{
			vmessItem.requestHost = Utils.UrlDecode(nameValueCollection["host"] ?? "");
			string url = nameValueCollection["path"] ?? "/";
			vmessItem.path = Utils.UrlDecode(url);
			break;
		}
		case "h2":
		case "http":
			vmessItem.network = "h2";
			vmessItem.requestHost = Utils.UrlDecode(nameValueCollection["host"] ?? "");
			vmessItem.path = Utils.UrlDecode(nameValueCollection["path"] ?? "/");
			break;
		case "quic":
			vmessItem.headerType = nameValueCollection["headerType"] ?? "none";
			vmessItem.requestHost = nameValueCollection["quicSecurity"] ?? "none";
			vmessItem.path = Utils.UrlDecode(nameValueCollection["key"] ?? "");
			break;
		case "grpc":
			vmessItem.path = Utils.UrlDecode(nameValueCollection["serviceName"] ?? "");
			vmessItem.headerType = Utils.UrlDecode(nameValueCollection["mode"] ?? "gun");
			break;
		default:
			return null;
		}
		return vmessItem;
	}

	private static VmessItem ResolveHysteria2(string result)
	{
		VmessItem vmessItem = new VmessItem
		{
			configType = 11,
			network = "hysteria2",
			streamSecurity = "tls"
		};
		try
		{
			string text = result.Trim();
			if (text.EndsWith("/") && !text.Contains("?"))
			{
				text = text.TrimEnd('/');
			}
			else if (text.EndsWith("/") && text.Contains("?"))
			{
				int num = text.IndexOf("?");
				string text2 = text.Substring(0, num);
				string text3 = text.Substring(num);
				if (text2.EndsWith("/"))
				{
					text2 = text2.TrimEnd('/');
				}
				text = text2 + text3;
			}
			Uri uri = new Uri(text);
			vmessItem.address = uri.IdnHost;
			vmessItem.port = ((uri.Port > 0) ? uri.Port : 443);
			vmessItem.id = Utils.UrlDecode(uri.UserInfo);
			vmessItem.remarks = uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
			vmessItem.sni = Utils.UrlDecode(nameValueCollection["sni"] ?? "");
			vmessItem.path = Utils.UrlDecode(nameValueCollection["obfs"] ?? "");
			vmessItem.requestHost = Utils.UrlDecode(nameValueCollection["obfs-password"] ?? "");
			vmessItem.publicKey = Utils.UrlDecode(nameValueCollection["pinSHA256"] ?? "");
			if (nameValueCollection["insecure"] != null)
			{
				if (nameValueCollection["insecure"] == "0" || nameValueCollection["insecure"] == "false")
				{
					vmessItem.allowInsecure = "false";
				}
				else
				{
					vmessItem.allowInsecure = "true";
				}
			}
			if (string.IsNullOrEmpty(vmessItem.remarks))
			{
				vmessItem.remarks = "NONE";
			}
		}
		catch (Exception)
		{
			return null;
		}
		return vmessItem;
	}

	private static VmessItem ResolveAnyTLS(string result)
	{
		VmessItem vmessItem = new VmessItem
		{
			configType = 12,
			network = "anytls",
			streamSecurity = "tls"
		};
		try
		{
			string text = result.Trim();
			if (text.EndsWith("/") && !text.Contains("?"))
			{
				text = text.TrimEnd('/');
			}
			else if (text.Contains("/?"))
			{
				text = text.Replace("/?", "?");
			}
			Uri uri = new Uri(text);
			vmessItem.address = uri.IdnHost;
			vmessItem.port = ((uri.Port > 0) ? uri.Port : 443);
			vmessItem.id = Utils.UrlDecode(uri.UserInfo);
			vmessItem.remarks = uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
			vmessItem.sni = Utils.UrlDecode(nameValueCollection["sni"] ?? "");
			if (nameValueCollection["insecure"] != null)
			{
				if (nameValueCollection["insecure"] == "0" || nameValueCollection["insecure"] == "false")
				{
					vmessItem.allowInsecure = "false";
				}
				else
				{
					vmessItem.allowInsecure = "true";
				}
			}
			if (string.IsNullOrEmpty(vmessItem.remarks))
			{
				vmessItem.remarks = "NONE";
			}
		}
		catch (Exception)
		{
			return null;
		}
		return vmessItem;
	}
}
