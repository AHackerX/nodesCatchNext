using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Handler;

/// <summary>
/// 原生 Clash YAML 配置解析器，支持 VLESS Reality、Hysteria2 等协议
/// </summary>
public static class ClashYamlParser
{
    /// <summary>
    /// 解析 Clash YAML 配置，提取 proxies 节点
    /// </summary>
    public static List<VmessItem> ParseYaml(string yamlContent)
    {
        var result = new List<VmessItem>();
        if (string.IsNullOrEmpty(yamlContent))
            return result;

        // 找到 proxies: 部分
        int proxiesIndex = yamlContent.IndexOf("proxies:");
        if (proxiesIndex < 0)
            return result;

        string proxiesSection = yamlContent.Substring(proxiesIndex + 8); // 跳过 "proxies:"
        
        // 找到下一个顶级键（proxy-groups:, rules: 等）作为结束
        // 支持有换行和无换行两种格式
        string[] topLevelKeys = { "proxy-groups:", "rules:", "rule-providers:", "proxy-providers:" };
        int endIndex = proxiesSection.Length;
        foreach (var key in topLevelKeys)
        {
            // 尝试带换行的格式
            int idx = proxiesSection.IndexOf("\n" + key);
            if (idx > 0 && idx < endIndex)
                endIndex = idx;
            
            // 尝试不带换行的格式（紧凑格式）
            idx = proxiesSection.IndexOf(key);
            if (idx > 0 && idx < endIndex)
                endIndex = idx;
        }
        proxiesSection = proxiesSection.Substring(0, endIndex);

        // 按 "- name:" 分割每个代理节点
        var proxyBlocks = SplitProxyBlocks(proxiesSection);
        
        foreach (var block in proxyBlocks)
        {
            var item = ParseProxyBlock(block);
            if (item != null)
                result.Add(item);
        }

        return result;
    }

    private static List<string> SplitProxyBlocks(string proxiesSection)
    {
        var blocks = new List<string>();
        
        // 预处理：将紧凑格式 }- { 或 }-{ 转换为带换行的格式
        proxiesSection = Regex.Replace(proxiesSection, @"\}\s*-\s*\{", "}\n- {");
        
        // 预处理：确保 - { 前面有换行（处理 proxies:- { 这种情况）
        proxiesSection = Regex.Replace(proxiesSection, @"^-\s*\{", "\n- {");
        
        // 按行分割，处理每一行
        var lines = proxiesSection.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var currentBlock = new List<string>();
        bool inProxy = false;

        foreach (var line in lines)
        {
            string trimmed = line.TrimStart();
            
            // 检测单行 JSON 格式: - {name: xxx, server: xxx, ...}
            // 使用更智能的方式提取完整的 JSON 对象
            if (trimmed.StartsWith("- {"))
            {
                string jsonBlock = ExtractJsonBlock(trimmed);
                if (!string.IsNullOrEmpty(jsonBlock))
                {
                    blocks.Add(jsonBlock);
                    continue;
                }
            }
            
            // 检测多行格式新代理块开始
            if (trimmed.StartsWith("- name:") || trimmed.StartsWith("-  name:"))
            {
                if (inProxy && currentBlock.Count > 0)
                {
                    blocks.Add(string.Join("\n", currentBlock));
                }
                currentBlock.Clear();
                currentBlock.Add(line);
                inProxy = true;
            }
            else if (inProxy)
            {
                currentBlock.Add(line);
            }
        }

        if (currentBlock.Count > 0)
        {
            blocks.Add(string.Join("\n", currentBlock));
        }

        return blocks;
    }

    /// <summary>
    /// 从行中提取完整的 JSON 对象，正确处理嵌套花括号
    /// </summary>
    private static string ExtractJsonBlock(string line)
    {
        // 找到第一个 { 的位置
        int startIndex = line.IndexOf('{');
        if (startIndex < 0)
            return null;

        int braceCount = 0;
        bool inQuote = false;
        char quoteChar = '\0';

        for (int i = startIndex; i < line.Length; i++)
        {
            char c = line[i];

            // 处理引号（跳过转义的引号）
            if ((c == '"' || c == '\'') && (i == 0 || line[i - 1] != '\\'))
            {
                if (!inQuote)
                {
                    inQuote = true;
                    quoteChar = c;
                }
                else if (c == quoteChar)
                {
                    inQuote = false;
                    quoteChar = '\0';
                }
                continue;
            }

            // 不在引号内时计算花括号
            if (!inQuote)
            {
                if (c == '{')
                    braceCount++;
                else if (c == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        // 找到完整的 JSON 对象
                        return "- " + line.Substring(startIndex, i - startIndex + 1);
                    }
                }
            }
        }

        return null;
    }

    private static VmessItem ParseProxyBlock(string block)
    {
        try
        {
            var props = ParseYamlProperties(block);
            if (!props.ContainsKey("type") || !props.ContainsKey("name"))
                return null;

            string type = props["type"].ToLower();
            var item = new VmessItem
            {
                remarks = CleanName(props.GetValueOrDefault("name", "")),
                address = props.GetValueOrDefault("server", ""),
                port = ParseInt(props.GetValueOrDefault("port", "0"))
            };

            switch (type)
            {
                case "vless":
                    return ParseVless(item, props);
                case "vmess":
                    return ParseVmess(item, props);
                case "ss":
                case "shadowsocks":
                    return ParseShadowsocks(item, props);
                case "trojan":
                    return ParseTrojan(item, props);
                case "hysteria2":
                case "hy2":
                    return ParseHysteria2(item, props);
                case "socks5":
                case "socks":
                    return ParseSocks(item, props);
                case "http":
                    return ParseHttp(item, props);
                case "ssr":
                case "shadowsocksr":
                    return ParseShadowsocksR(item, props);
                case "anytls":
                    return ParseAnyTLS(item, props);
                default:
                    return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, string> ParseYamlProperties(string block)
    {
        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        string trimmedBlock = block.Trim();
        
        // 检测单行格式: - {name: xxx, server: xxx, ...} 或 {name: xxx, ...}
        if (trimmedBlock.StartsWith("- {"))
        {
            trimmedBlock = trimmedBlock.Substring(2).Trim(); // 移除 "- "
        }
        
        if (trimmedBlock.StartsWith("{") && trimmedBlock.EndsWith("}"))
        {
            return ParseInlineYaml(trimmedBlock);
        }
        
        // 多行格式解析
        var lines = block.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var sectionStack = new List<string>();
        int lastIndent = -1;

        foreach (var line in lines)
        {
            // 计算缩进
            int indent = 0;
            foreach (char c in line)
            {
                if (c == ' ') indent++;
                else if (c == '\t') indent += 2;
                else break;
            }

            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                continue;

            // 处理 "- name: xxx" 格式（列表项开始）
            if (trimmed.StartsWith("- "))
            {
                trimmed = trimmed.Substring(2).Trim();
                sectionStack.Clear();
                lastIndent = indent;
            }
            else if (indent <= lastIndent && sectionStack.Count > 0)
            {
                // 缩进减少，退出子节
                while (sectionStack.Count > 0 && indent <= lastIndent)
                {
                    sectionStack.RemoveAt(sectionStack.Count - 1);
                    lastIndent -= 2;
                }
            }

            // 检测子节（如 reality-opts:, ws-opts:, headers:）
            if (trimmed.EndsWith(":") && !trimmed.Contains(": "))
            {
                string sectionName = trimmed.TrimEnd(':');
                sectionStack.Add(sectionName);
                lastIndent = indent;
                continue;
            }

            // 解析 key: value
            int colonIndex = trimmed.IndexOf(':');
            if (colonIndex > 0)
            {
                string key = trimmed.Substring(0, colonIndex).Trim();
                string value = trimmed.Substring(colonIndex + 1).Trim();
                
                // 移除引号
                value = value.Trim('\'', '"');
                
                // 构建完整的键名
                string fullKey = key;
                if (sectionStack.Count > 0)
                {
                    fullKey = string.Join(".", sectionStack) + "." + key;
                }
                
                props[fullKey] = value;
                
                // 同时保存简单键名（用于兼容）
                if (!props.ContainsKey(key))
                {
                    props[key] = value;
                }
            }
        }

        return props;
    }

    /// <summary>
    /// 解析单行 YAML 格式: {name: xxx, server: xxx, port: 443, type: http, ...}
    /// 也支持 JSON 格式: {"name":"xxx", "server":"xxx", ...}
    /// </summary>
    private static Dictionary<string, string> ParseInlineYaml(string inline)
    {
        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        // 移除首尾花括号
        string content = inline.Trim().TrimStart('{').TrimEnd('}').Trim();
        
        if (string.IsNullOrEmpty(content))
            return props;

        // 使用状态机解析，处理引号内的逗号和冒号
        var pairs = SplitInlineYamlPairs(content);
        
        foreach (var pair in pairs)
        {
            int colonIndex = FindKeyValueSeparator(pair);
            if (colonIndex > 0)
            {
                string key = pair.Substring(0, colonIndex).Trim();
                string value = pair.Substring(colonIndex + 1).Trim();
                
                // 移除 key 的引号（JSON 格式）
                key = key.Trim('"', '\'');
                
                // 移除 value 的引号
                value = value.Trim('"', '\'');
                
                props[key] = value;
            }
        }

        return props;
    }

    /// <summary>
    /// 分割单行 YAML 的键值对，正确处理引号内的逗号
    /// </summary>
    private static List<string> SplitInlineYamlPairs(string content)
    {
        var pairs = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuote = false;
        char quoteChar = '\0';
        int braceDepth = 0;

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];

            // 处理引号
            if ((c == '"' || c == '\'') && (i == 0 || content[i - 1] != '\\'))
            {
                if (!inQuote)
                {
                    inQuote = true;
                    quoteChar = c;
                }
                else if (c == quoteChar)
                {
                    inQuote = false;
                    quoteChar = '\0';
                }
                current.Append(c);
                continue;
            }

            // 处理嵌套花括号（如 headers: {Host: xxx}）
            if (!inQuote)
            {
                if (c == '{') braceDepth++;
                else if (c == '}') braceDepth--;
            }

            // 逗号分隔（不在引号内且不在嵌套花括号内）
            if (c == ',' && !inQuote && braceDepth == 0)
            {
                string pair = current.ToString().Trim();
                if (!string.IsNullOrEmpty(pair))
                {
                    pairs.Add(pair);
                }
                current.Clear();
                continue;
            }

            current.Append(c);
        }

        // 添加最后一个键值对
        string lastPair = current.ToString().Trim();
        if (!string.IsNullOrEmpty(lastPair))
        {
            pairs.Add(lastPair);
        }

        return pairs;
    }

    /// <summary>
    /// 找到键值分隔符（冒号），跳过引号内的冒号
    /// </summary>
    private static int FindKeyValueSeparator(string pair)
    {
        bool inQuote = false;
        char quoteChar = '\0';

        for (int i = 0; i < pair.Length; i++)
        {
            char c = pair[i];

            if ((c == '"' || c == '\'') && (i == 0 || pair[i - 1] != '\\'))
            {
                if (!inQuote)
                {
                    inQuote = true;
                    quoteChar = c;
                }
                else if (c == quoteChar)
                {
                    inQuote = false;
                    quoteChar = '\0';
                }
                continue;
            }

            if (c == ':' && !inQuote)
            {
                return i;
            }
        }

        return -1;
    }

    private static VmessItem ParseVless(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 5; // VLESS
        item.id = props.GetValueOrDefault("uuid", "");
        item.security = props.GetValueOrDefault("encryption", "none");
        item.network = props.GetValueOrDefault("network", "tcp");
        item.flow = props.GetValueOrDefault("flow", "");
        
        // TLS 设置
        bool tls = ParseBool(props.GetValueOrDefault("tls", "false"));
        string security = props.GetValueOrDefault("security", "");
        
        // Reality 设置
        string publicKey = props.GetValueOrDefault("reality-opts.public-key", "");
        string shortId = props.GetValueOrDefault("reality-opts.short-id", "");
        
        if (!string.IsNullOrEmpty(publicKey))
        {
            item.streamSecurity = "reality";
            item.publicKey = publicKey;
            item.shortId = shortId;
            item.sni = props.GetValueOrDefault("servername", "");
            item.fingerprint = props.GetValueOrDefault("client-fingerprint", "chrome");
        }
        else if (tls || security == "tls")
        {
            item.streamSecurity = "tls";
            item.sni = props.GetValueOrDefault("servername", props.GetValueOrDefault("sni", ""));
            item.fingerprint = props.GetValueOrDefault("client-fingerprint", "");
        }

        // skip-cert-verify
        if (ParseBool(props.GetValueOrDefault("skip-cert-verify", "false")))
        {
            item.allowInsecure = "true";
        }

        // 网络设置
        ParseNetworkSettings(item, props);

        return item;
    }

    private static VmessItem ParseVmess(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 1; // VMess
        item.id = props.GetValueOrDefault("uuid", "");
        item.alterId = ParseInt(props.GetValueOrDefault("alterId", "0"));
        item.security = props.GetValueOrDefault("cipher", "auto");
        item.network = props.GetValueOrDefault("network", "tcp");

        bool tls = ParseBool(props.GetValueOrDefault("tls", "false"));
        if (tls)
        {
            item.streamSecurity = "tls";
            item.sni = props.GetValueOrDefault("servername", props.GetValueOrDefault("sni", ""));
        }

        if (ParseBool(props.GetValueOrDefault("skip-cert-verify", "false")))
        {
            item.allowInsecure = "true";
        }

        ParseNetworkSettings(item, props);

        return item;
    }

    private static VmessItem ParseShadowsocks(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 3; // Shadowsocks
        item.id = props.GetValueOrDefault("password", "");
        item.security = props.GetValueOrDefault("cipher", props.GetValueOrDefault("method", ""));
        
        string plugin = props.GetValueOrDefault("plugin", "");
        if (!string.IsNullOrEmpty(plugin))
        {
            item.network = plugin;
        }

        return item;
    }

    private static VmessItem ParseTrojan(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 6; // Trojan
        item.id = props.GetValueOrDefault("password", "");
        item.streamSecurity = "tls";
        item.sni = props.GetValueOrDefault("sni", props.GetValueOrDefault("servername", ""));
        item.network = props.GetValueOrDefault("network", "tcp");

        if (ParseBool(props.GetValueOrDefault("skip-cert-verify", "false")))
        {
            item.allowInsecure = "true";
        }

        ParseNetworkSettings(item, props);

        return item;
    }

    private static VmessItem ParseHysteria2(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 11; // Hysteria2
        item.network = "hysteria2";
        item.streamSecurity = "tls";
        
        // password 或 auth
        item.id = props.GetValueOrDefault("password", props.GetValueOrDefault("auth", ""));
        item.sni = props.GetValueOrDefault("sni", "");
        
        // obfs 设置
        string obfs = props.GetValueOrDefault("obfs", "");
        if (!string.IsNullOrEmpty(obfs))
        {
            item.path = obfs; // obfs type
            item.requestHost = props.GetValueOrDefault("obfs-password", ""); // obfs password
        }

        if (ParseBool(props.GetValueOrDefault("skip-cert-verify", "false")))
        {
            item.allowInsecure = "true";
        }

        return item;
    }

    private static VmessItem ParseSocks(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 4; // Socks
        item.security = props.GetValueOrDefault("username", "");
        item.id = props.GetValueOrDefault("password", "");
        return item;
    }

    private static VmessItem ParseHttp(VmessItem item, Dictionary<string, string> props)
    {
        bool tls = ParseBool(props.GetValueOrDefault("tls", "false"));
        item.configType = tls ? 8 : 7; // HTTPS or HTTP
        item.security = props.GetValueOrDefault("username", "");
        item.id = props.GetValueOrDefault("password", "");
        return item;
    }

    private static VmessItem ParseShadowsocksR(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 9; // SSR
        item.id = props.GetValueOrDefault("password", "");
        item.security = props.GetValueOrDefault("cipher", "");
        
        // SSR 特有参数 - 字段映射与 ssr:// 链接格式一致
        item.network = props.GetValueOrDefault("protocol", "");           // protocol
        item.headerType = props.GetValueOrDefault("obfs", "");            // obfs
        item.streamSecurity = props.GetValueOrDefault("protocol-param", ""); // protocol-param
        item.sni = props.GetValueOrDefault("obfs-param", "");             // obfs-param
        
        return item;
    }

    private static VmessItem ParseAnyTLS(VmessItem item, Dictionary<string, string> props)
    {
        item.configType = 12; // AnyTLS
        item.network = "anytls";
        item.streamSecurity = "tls";
        item.id = props.GetValueOrDefault("password", "");
        item.sni = props.GetValueOrDefault("sni", "");
        
        if (ParseBool(props.GetValueOrDefault("skip-cert-verify", "false")))
        {
            item.allowInsecure = "true";
        }
        
        return item;
    }

    private static void ParseNetworkSettings(VmessItem item, Dictionary<string, string> props)
    {
        switch (item.network?.ToLower())
        {
            case "ws":
            case "websocket":
                item.network = "ws";
                item.path = props.GetValueOrDefault("ws-opts.path", props.GetValueOrDefault("path", "/"));
                item.requestHost = props.GetValueOrDefault("ws-opts.headers.Host", props.GetValueOrDefault("host", ""));
                break;
            case "grpc":
                item.path = props.GetValueOrDefault("grpc-opts.grpc-service-name", props.GetValueOrDefault("serviceName", ""));
                item.headerType = props.GetValueOrDefault("grpc-opts.grpc-mode", "gun");
                break;
            case "h2":
            case "http":
                item.network = "h2";
                item.path = props.GetValueOrDefault("h2-opts.path", props.GetValueOrDefault("path", "/"));
                item.requestHost = props.GetValueOrDefault("h2-opts.host", props.GetValueOrDefault("host", ""));
                break;
            case "tcp":
            default:
                item.network = "tcp";
                item.headerType = props.GetValueOrDefault("headerType", "none");
                break;
        }
    }

    private static string CleanName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "NONE";
        // URL 解码 + 号
        return name.Replace("+", " ").Trim();
    }

    private static int ParseInt(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        return 0;
    }

    private static bool ParseBool(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;
        value = value.ToLower();
        return value == "true" || value == "1" || value == "yes";
    }

    private static string GetValueOrDefault(this Dictionary<string, string> dict, string key, string defaultValue = "")
    {
        return dict.TryGetValue(key, out string value) ? value : defaultValue;
    }
}
