using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using nodesCatchNext.Base;
using nodesCatchNext.Mode;

namespace nodesCatchNext;

internal class Utils
{
	private static readonly string[] TunKeywords = new string[6] { "wintun", "tun", "tap", "wireguard", "clash", "warp" };

	public static bool TryDetectTunAdapter(out string adapterDisplayName)
	{
		adapterDisplayName = string.Empty;
		try
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface == null || networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Unknown)
				{
					continue;
				}
				string text = networkInterface.Description ?? string.Empty;
				string text2 = networkInterface.Name ?? string.Empty;
				string text3 = ((string.IsNullOrWhiteSpace(text2) || string.IsNullOrWhiteSpace(text)) ? (text2 + text).Trim() : (text2 + " (" + text + ")"));
				if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
				{
					adapterDisplayName = text3;
					return true;
				}
				string text4 = text.ToLowerInvariant();
				string text5 = text2.ToLowerInvariant();
				string[] tunKeywords = TunKeywords;
				foreach (string value in tunKeywords)
				{
					if ((!string.IsNullOrEmpty(text4) && text4.Contains(value)) || (!string.IsNullOrEmpty(text5) && text5.Contains(value)))
					{
						adapterDisplayName = text3;
						return true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("TryDetectTunAdapter failed: " + ex.Message);
		}
		return false;
	}

	public static string LoadResource(string res)
	{
		string result = string.Empty;
		try
		{
			using StreamReader streamReader = new StreamReader(res);
			result = streamReader.ReadToEnd();
		}
		catch (Exception ex)
		{
			Logger.Debug("LoadResource failed for '" + res + "': " + ex.Message);
		}
		return result;
	}

	public static T DeepCopy<T>(T obj)
	{
		if (obj == null)
		{
			return default(T);
		}
		return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		}));
	}

	public static void SetSecurityProtocol()
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		ServicePointManager.DefaultConnectionLimit = 256;
	}

	public static string UrlEncode(string url)
	{
		return HttpUtility.UrlEncode(url);
	}

	public static string UrlDecode(string url)
	{
		return HttpUtility.UrlDecode(url);
	}

	public static T FromJson<T>(string strJson)
	{
		try
		{
			return JsonConvert.DeserializeObject<T>(strJson);
		}
		catch (Exception ex)
		{
			Logger.Debug("FromJson failed: " + ex.Message);
			return default(T);
		}
	}

	public static int ToJsonFile(object obj, string filePath, bool nullValue = true)
	{
		try
		{
			using (StreamWriter textWriter = File.CreateText(filePath))
			{
				((!nullValue) ? new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				} : new JsonSerializer
				{
					Formatting = Formatting.Indented
				}).Serialize(textWriter, obj);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logger.Error("ToJsonFile failed for '" + filePath + "'", ex);
			return -1;
		}
	}

	public static string GetPath(string fileName)
	{
		string text = StartupPath();
		if (IsNullOrEmpty(fileName))
		{
			return text;
		}
		return Path.Combine(text, fileName);
	}

	public static string GetEmbedText(string res)
	{
		string result = string.Empty;
		try
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
			using StreamReader streamReader = new StreamReader(stream);
			result = streamReader.ReadToEnd();
		}
		catch
		{
		}
		return result;
	}

	public static string StartupPath()
	{
		return Application.StartupPath;
	}

	public static bool IsNullOrEmpty(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			return text.Equals("null");
		}
		return true;
	}

	public static void AddSubItem(ListViewItem i, string name, string text)
	{
		i.SubItems.Add(new ListViewItem.ListViewSubItem
		{
			Name = name,
			Text = text
		});
	}

	public static string Base64EncodeUrlSafe(string plainText)
	{
		try
		{
			return Base64Encode(plainText).Replace('+', '-').Replace('/', '_').Replace("=", "");
		}
		catch
		{
			return string.Empty;
		}
	}

	public static string Base64Encode(string plainText)
	{
		try
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static string ToJson(object obj)
	{
		string result = string.Empty;
		try
		{
			result = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
		}
		catch
		{
		}
		return result;
	}

	public static string Base64DecodeUrlSafe(string plainText)
	{
		try
		{
			if (IsNullOrEmpty(plainText))
			{
				return string.Empty;
			}
			plainText = plainText.Replace('-', '+').Replace('_', '/');
			return Base64Decode(plainText);
		}
		catch
		{
			return string.Empty;
		}
	}

	public static string Base64Decode(string plainText)
	{
		try
		{
			plainText = plainText.TrimEx().Replace(Environment.NewLine, "").Replace("\n", "")
				.Replace("\r", "")
				.Replace(" ", "");
			if (plainText.Length % 4 > 0)
			{
				plainText = plainText.PadRight(plainText.Length + 4 - plainText.Length % 4, '=');
			}
			byte[] bytes = Convert.FromBase64String(plainText);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static List<string> String2List(string str)
	{
		try
		{
			str = str.Replace(Environment.NewLine, "");
			return new List<string>(str.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries));
		}
		catch
		{
			return new List<string>();
		}
	}

	public static string GetGUID()
	{
		try
		{
			return Guid.NewGuid().ToString("D");
		}
		catch
		{
		}
		return string.Empty;
	}

	public static string GetClipboardData()
	{
		string result = string.Empty;
		try
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(DataFormats.UnicodeText))
			{
				result = dataObject.GetData(DataFormats.UnicodeText).ToString();
			}
			return result;
		}
		catch
		{
			return result;
		}
	}

	public static void SetClipboardData(string strData)
	{
		try
		{
			Clipboard.SetText(strData);
		}
		catch
		{
		}
	}

	public static bool IsIpv6(string ip)
	{
		if (IPAddress.TryParse(ip, out var address))
		{
			return address.AddressFamily switch
			{
				AddressFamily.InterNetwork => false, 
				AddressFamily.InterNetworkV6 => true, 
				_ => false, 
			};
		}
		return false;
	}

	public static string ToString(object obj)
	{
		try
		{
			return (obj == null) ? string.Empty : obj.ToString();
		}
		catch
		{
			return string.Empty;
		}
	}

	public static string ShowSaveFileDialog(string filter)
	{
		string result = "";
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = filter;
		saveFileDialog.FilterIndex = 1;
		saveFileDialog.RestoreDirectory = true;
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			result = saveFileDialog.FileName.ToString();
		}
		return result;
	}

	public static int ToInt(object obj)
	{
		try
		{
			return Convert.ToInt32(obj);
		}
		catch
		{
			return 0;
		}
	}

	public static void DedupServerList(List<VmessItem> source, out List<VmessItem> result, bool keepOlder)
	{
		List<VmessItem> list = new List<VmessItem>();
		if (!keepOlder)
		{
			source.Reverse();
		}
		foreach (VmessItem item in source)
		{
			if (!list.Exists((VmessItem i) => _isAdded(i, item)))
			{
				list.Add(item);
			}
		}
		if (!keepOlder)
		{
			list.Reverse();
		}
		result = list;
		static bool _isAdded(VmessItem o, VmessItem n)
		{
			if (o.configVersion == n.configVersion && o.configType == n.configType && o.address == n.address && o.port == n.port && o.id == n.id && o.alterId == n.alterId && o.security == n.security && o.network == n.network && o.headerType == n.headerType && o.requestHost == n.requestHost && o.path == n.path && o.remarks == n.remarks)
			{
				return o.streamSecurity == n.streamSecurity;
			}
			return false;
		}
	}
}
