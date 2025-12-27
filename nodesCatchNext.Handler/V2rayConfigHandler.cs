using System.Collections.Generic;
using System.Linq;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

internal class V2rayConfigHandler
{
	private static string SampleClient = "nodesCatchNext.Sample.SampleClientConfig.txt";

	public static int GenerateClientConfig(Config config, string fileName, bool blExport, out string msg)
	{
		try
		{
			if (config == null || config.index < 0 || config.vmess.Count <= 0 || config.index > config.vmess.Count - 1)
			{
				msg = "请先检查服务器设置";
				return -1;
			}
			msg = "初始化配置";
			string embedText = Utils.GetEmbedText(SampleClient);
			if (Utils.IsNullOrEmpty(embedText))
			{
				msg = "获取默认配置失败";
				return -1;
			}
			V2rayConfig v2rayConfig = Utils.FromJson<V2rayConfig>(embedText);
			if (v2rayConfig == null)
			{
				msg = "生成默认配置文件失败";
				return -1;
			}
			Utils.ToJsonFile(v2rayConfig, fileName, nullValue: false);
			msg = "生成配置文件成功！";
		}
		catch
		{
			msg = "生成默认配置文件失败";
			return -1;
		}
		return 0;
	}

	public static string GenerateClientSpeedtestConfigString(Config config, List<int> selecteds, out string msg)
	{
		try
		{
			if (config == null || config.index < 0 || config.vmess.Count <= 0 || config.index > config.vmess.Count - 1)
			{
				msg = "请先检查服务器设置";
				return "";
			}
			msg = "初始化配置";
			Config config2 = Utils.DeepCopy(config);
			string embedText = Utils.GetEmbedText(SampleClient);
			if (Utils.IsNullOrEmpty(embedText))
			{
				msg = "取得默认配置失败";
				return "";
			}
			V2rayConfig v2rayConfig = Utils.FromJson<V2rayConfig>(embedText);
			if (v2rayConfig == null)
			{
				msg = "生成默认配置文件失败";
				return "";
			}
			v2rayConfig.inbounds.Clear();
			int localPort = config2.GetLocalPort("speedtest");
			V2rayConfig obj = Utils.DeepCopy(v2rayConfig);
			foreach (int selected in selecteds)
			{
				if (config2.vmess[selected].configType != 2)
				{
					config2.index = selected;
					Inbounds inbounds = new Inbounds
					{
						listen = "127.0.0.1",
						port = localPort + selected,
						protocol = "http"
					};
					inbounds.tag = "http" + inbounds.port;
					v2rayConfig.inbounds.Add(inbounds);
					V2rayConfig v2rayConfig2 = Utils.DeepCopy(obj);
					outbound(config2, ref v2rayConfig2);
					v2rayConfig2.outbounds[0].tag = "proxy" + inbounds.port;
					v2rayConfig.outbounds.Add(v2rayConfig2.outbounds[0]);
					RulesItem item = new RulesItem
					{
						inboundTag = new List<string> { inbounds.tag },
						outboundTag = v2rayConfig2.outbounds[0].tag,
						type = "field"
					};
					v2rayConfig.routing.rules.Add(item);
				}
			}
			msg = "配置成功";
			return Utils.ToJson(v2rayConfig);
		}
		catch
		{
			msg = "生成默认配置文件失败";
			return "";
		}
	}

	private static int outbound(Config config, ref V2rayConfig v2rayConfig)
	{
		try
		{
			int num = config.configType();
			if (num != 1 && (uint)(num - 3) > 3u)
			{
				return -1;
			}
			Outbounds outbounds = v2rayConfig.outbounds[0];
			if (config.configType() == 1)
			{
				VnextItem vnextItem;
				if (outbounds.settings.vnext.Count <= 0)
				{
					vnextItem = new VnextItem();
					outbounds.settings.vnext.Add(vnextItem);
				}
				else
				{
					vnextItem = outbounds.settings.vnext[0];
				}
				vnextItem.address = config.address();
				vnextItem.port = config.port();
				UsersItem usersItem;
				if (vnextItem.users.Count <= 0)
				{
					usersItem = new UsersItem();
					vnextItem.users.Add(usersItem);
				}
				else
				{
					usersItem = vnextItem.users[0];
				}
				usersItem.id = config.id();
				usersItem.alterId = config.alterId();
				usersItem.email = "t@t.tt";
				usersItem.security = config.security();
				outbounds.mux.enabled = false;
				outbounds.mux.concurrency = -1;
				StreamSettings streamSettings = outbounds.streamSettings;
				boundStreamSettings(config, "out", ref streamSettings);
				outbounds.protocol = "vmess";
				outbounds.settings.servers = null;
			}
			else if (config.configType() == 3)
			{
				ServersItem serversItem;
				if (outbounds.settings.servers.Count <= 0)
				{
					serversItem = new ServersItem();
					outbounds.settings.servers.Add(serversItem);
				}
				else
				{
					serversItem = outbounds.settings.servers[0];
				}
				serversItem.address = config.address();
				serversItem.port = config.port();
				serversItem.password = config.id();
				if (Global.ssSecuritys.Contains(config.security()))
				{
					serversItem.method = config.security();
				}
				else
				{
					serversItem.method = "none";
				}
				serversItem.ota = false;
				serversItem.level = 1;
				outbounds.mux.enabled = false;
				outbounds.mux.concurrency = -1;
				outbounds.protocol = "shadowsocks";
				outbounds.settings.vnext = null;
			}
			else if (config.configType() == 4)
			{
				ServersItem serversItem2;
				if (outbounds.settings.servers.Count <= 0)
				{
					serversItem2 = new ServersItem();
					outbounds.settings.servers.Add(serversItem2);
				}
				else
				{
					serversItem2 = outbounds.settings.servers[0];
				}
				serversItem2.address = config.address();
				serversItem2.port = config.port();
				serversItem2.method = null;
				serversItem2.password = null;
				if (!Utils.IsNullOrEmpty(config.security()) && !Utils.IsNullOrEmpty(config.id()))
				{
					SocksUsersItem item = new SocksUsersItem
					{
						user = config.security(),
						pass = config.id(),
						level = 1
					};
					serversItem2.users = new List<SocksUsersItem> { item };
				}
				outbounds.mux.enabled = false;
				outbounds.mux.concurrency = -1;
				outbounds.protocol = "socks";
				outbounds.settings.vnext = null;
			}
			else if (config.configType() == 5)
			{
				VnextItem vnextItem2;
				if (outbounds.settings.vnext.Count <= 0)
				{
					vnextItem2 = new VnextItem();
					outbounds.settings.vnext.Add(vnextItem2);
				}
				else
				{
					vnextItem2 = outbounds.settings.vnext[0];
				}
				vnextItem2.address = config.address();
				vnextItem2.port = config.port();
				UsersItem usersItem2;
				if (vnextItem2.users.Count <= 0)
				{
					usersItem2 = new UsersItem();
					vnextItem2.users.Add(usersItem2);
				}
				else
				{
					usersItem2 = vnextItem2.users[0];
				}
				usersItem2.id = config.id();
				usersItem2.alterId = 0;
				usersItem2.flow = string.Empty;
				usersItem2.email = "t@t.tt";
				usersItem2.encryption = config.security();
				outbounds.mux.enabled = false;
				outbounds.mux.concurrency = -1;
				StreamSettings streamSettings2 = outbounds.streamSettings;
				boundStreamSettings(config, "out", ref streamSettings2);
				if (config.streamSecurity() == "reality" || (!string.IsNullOrWhiteSpace(config.flow()) && config.flow().Contains("vision") && (!string.IsNullOrWhiteSpace(config.publicKey()) || !string.IsNullOrWhiteSpace(config.shortId()))))
				{
					if (!Utils.IsNullOrEmpty(config.flow()))
					{
						usersItem2.flow = config.flow();
					}
					outbounds.mux.enabled = false;
					outbounds.mux.concurrency = -1;
				}
				else if (config.streamSecurity() == "xtls")
				{
					if (Utils.IsNullOrEmpty(config.flow()))
					{
						usersItem2.flow = "xtls-rprx-origin";
					}
					else
					{
						usersItem2.flow = config.flow().Replace("splice", "direct");
					}
					outbounds.mux.enabled = false;
					outbounds.mux.concurrency = -1;
				}
				outbounds.protocol = "vless";
				outbounds.settings.servers = null;
			}
			else if (config.configType() == 6)
			{
				ServersItem serversItem3;
				if (outbounds.settings.servers.Count <= 0)
				{
					serversItem3 = new ServersItem();
					outbounds.settings.servers.Add(serversItem3);
				}
				else
				{
					serversItem3 = outbounds.settings.servers[0];
				}
				serversItem3.address = config.address();
				serversItem3.port = config.port();
				serversItem3.password = config.id();
				serversItem3.flow = string.Empty;
				serversItem3.ota = false;
				serversItem3.level = 1;
				if (config.streamSecurity() == "xtls")
				{
					if (Utils.IsNullOrEmpty(config.flow()))
					{
						serversItem3.flow = "xtls-rprx-origin";
					}
					else
					{
						serversItem3.flow = config.flow().Replace("splice", "direct");
					}
					outbounds.mux.enabled = false;
					outbounds.mux.concurrency = -1;
				}
				outbounds.mux.enabled = false;
				outbounds.mux.concurrency = -1;
				StreamSettings streamSettings3 = outbounds.streamSettings;
				boundStreamSettings(config, "out", ref streamSettings3);
				outbounds.protocol = "trojan";
				outbounds.settings.vnext = null;
			}
		}
		catch
		{
		}
		return 0;
	}

	private static int boundStreamSettings(Config config, string iobound, ref StreamSettings streamSettings)
	{
		try
		{
			streamSettings.network = config.network();
			string text = config.requestHost();
			string text2 = config.sni();
			if (string.IsNullOrWhiteSpace(text2) && config.network() == "ws" && !string.IsNullOrWhiteSpace(text))
			{
				text2 = Utils.String2List(text)[0];
			}
			if (config.streamSecurity() == "tls")
			{
				streamSettings.security = config.streamSecurity();
				TlsSettings tlsSettings = new TlsSettings
				{
					allowInsecure = config.allowInsecure()
				};
				if (!string.IsNullOrWhiteSpace(text2))
				{
					tlsSettings.serverName = text2;
				}
				else if (!string.IsNullOrWhiteSpace(text))
				{
					tlsSettings.serverName = Utils.String2List(text)[0];
				}
				streamSettings.tlsSettings = tlsSettings;
			}
			if (config.streamSecurity() == "xtls")
			{
				streamSettings.security = config.streamSecurity();
				TlsSettings tlsSettings2 = new TlsSettings
				{
					allowInsecure = config.allowInsecure()
				};
				if (!string.IsNullOrWhiteSpace(text2))
				{
					tlsSettings2.serverName = text2;
				}
				else if (!string.IsNullOrWhiteSpace(text))
				{
					tlsSettings2.serverName = Utils.String2List(text)[0];
				}
				streamSettings.xtlsSettings = tlsSettings2;
			}
			if (config.streamSecurity() == "reality" || (!string.IsNullOrWhiteSpace(config.flow()) && config.flow().Contains("vision") && (!string.IsNullOrWhiteSpace(config.publicKey()) || !string.IsNullOrWhiteSpace(config.shortId()))))
			{
				if (config.streamSecurity() != "reality")
				{
					streamSettings.security = "reality";
				}
				else
				{
					streamSettings.security = config.streamSecurity();
				}
				RealitySettings realitySettings = new RealitySettings();
				string text3 = text2;
				if (string.IsNullOrWhiteSpace(text3) && !string.IsNullOrWhiteSpace(text))
				{
					text3 = Utils.String2List(text)[0];
				}
				if (string.IsNullOrWhiteSpace(text3))
				{
					text3 = "www.microsoft.com";
				}
				realitySettings.serverNames = new string[1] { text3 };
				string text4 = config.shortId();
				if (!string.IsNullOrWhiteSpace(text4))
				{
					realitySettings.shortIds = new string[1] { text4 };
				}
				else
				{
					realitySettings.shortIds = new string[1] { "" };
				}
				string text5 = config.publicKey();
				if (!string.IsNullOrWhiteSpace(text5))
				{
					realitySettings.publicKey = text5;
				}
				string fingerprint = config.vmess[config.index].fingerprint;
				if (!string.IsNullOrWhiteSpace(fingerprint))
				{
					realitySettings.fingerprint = fingerprint;
				}
				else
				{
					realitySettings.fingerprint = "chrome";
				}
				streamSettings.realitySettings = realitySettings;
			}
			switch (config.network())
			{
			case "ws":
			{
				WsSettings wsSettings = new WsSettings();
				string text8 = config.path();
				if (!string.IsNullOrWhiteSpace(text))
				{
					wsSettings.headers = new Headers
					{
						Host = text
					};
				}
				if (!string.IsNullOrWhiteSpace(text8))
				{
					wsSettings.path = text8;
				}
				streamSettings.wsSettings = wsSettings;
				break;
			}
			case "h2":
			{
				HttpSettings httpSettings = new HttpSettings();
				if (!string.IsNullOrWhiteSpace(text))
				{
					httpSettings.host = Utils.String2List(text);
				}
				httpSettings.path = config.path();
				streamSettings.httpSettings = httpSettings;
				break;
			}
			case "quic":
			{
				QuicSettings quicSettings = new QuicSettings
				{
					security = text,
					key = config.path(),
					header = new Header
					{
						type = config.headerType()
					}
				};
				streamSettings.quicSettings = quicSettings;
				if (config.streamSecurity() == "tls")
				{
					if (!string.IsNullOrWhiteSpace(text2))
					{
						streamSettings.tlsSettings.serverName = text2;
					}
					else
					{
						streamSettings.tlsSettings.serverName = config.address();
					}
				}
				break;
			}
			case "grpc":
			{
				GrpcSettings grpcSettings = new GrpcSettings();
				grpcSettings.serviceName = config.path();
				grpcSettings.multiMode = config.headerType() == "multi";
				streamSettings.grpcSettings = grpcSettings;
				break;
			}
			default:
			{
				if (!config.headerType().Equals("http"))
				{
					break;
				}
				TcpSettings tcpSettings = new TcpSettings
				{
					header = new Header
					{
						type = config.headerType()
					}
				};
				if (iobound.Equals("out"))
				{
					string embedText = Utils.GetEmbedText("nodesCatchNext.Sample.SampleHttprequest.txt");
					string[] value = text.Split(',');
					string text6 = string.Join("\",\"", value);
					embedText = embedText.Replace("$requestHost$", "\"" + text6 + "\"");
					string text7 = "/";
					if (!Utils.IsNullOrEmpty(config.path()))
					{
						string[] value2 = config.path().Split(',');
						text7 = string.Join("\",\"", value2);
					}
					embedText = embedText.Replace("$requestPath$", "\"" + text7 + "\"");
					tcpSettings.header.request = Utils.FromJson<object>(embedText);
				}
				else
				{
					iobound.Equals("in");
				}
				streamSettings.tcpSettings = tcpSettings;
				break;
			}
			}
		}
		catch
		{
		}
		return 0;
	}
}
