<a name="top"></a>

<div align="center">

# 🚀 nodesCatchNext

**基于不良林nodesCatch V2.0 二次开发的代理节点管理和测速工具**

[![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Release](https://img.shields.io/badge/release-v1.0-orange.svg)](../../releases)

支持多种代理协议的导入、管理、测速和转换

[功能特性](#功能特性) • [快速开始](#快速开始) • [使用指南](#使用指南) • [配置说明](#配置说明) • [常见问题](#常见问题)

</div>

---

## 🎉 相比原版的改进

相比原版 nodesCatch V2.0，本项目进行了以下优化和改进：

<table>
<tr>
<td width="50%">

### 🚀 性能优化

- ⚡ **测速性能提升**
  - 优化了多线程并发测速算法
  - 提升了大量节点的测速效率
  - 减少了内存占用

- 💾 **配置管理优化**
  - 改进了配置文件的读写性能
  - 优化了节点数据的存储结构
  - 增强了配置文件的容错能力

</td>
<td width="50%">

### ✨ 功能增强

- 🔄 **核心支持**
  - 升级至最新版 Mihomo 核心
  - 初步支持Vless和Hysteria 2协议节点测速
  - 支持更多新特性

- 📊 **用户体验**
  - 新增TLS RTT测速
  - 改进了日志显示机制
  - 增强了错误提示信息

</td>
</tr>
<tr>

</td>
</tr>
</table>

### 📝 目前已知Bug

- 🐛 **部分节点无法测速**：部分Hysteria 2和Vless协议节点无法测速
- 🐛 **TLS RTT测速异常**：TLS RTT测速结果与HTTPS测速近乎一致

> 💡 **提示**：如果您在使用过程中发现任何问题或有改进建议，欢迎提交 Issue 或 Pull Request！

---

<a name="功能特性"></a>

## ✨ 功能特性

### 📦 节点管理
- ✅ 支持多种代理协议：VMess、Shadowsocks、VLESS、Trojan、SOCKS、HTTP、Hysteria 2、Vless 等
- 🔄 从订阅 URL 自动导入和更新节点
- 📋 从分享链接快速导入节点
- 🔧 批量管理节点（删除、去重）
- 📊 节点列表查看（地址、端口、协议、加密方式等详细信息）

### ⚡ 节点测速
- 📡 **Ping 测试**：检测节点延迟
- 🌐 **HTTPS 延迟测试**：测试实际 HTTPS 连接延迟
- 🔐 **TLS RTT 测试**：测试 TLS 握手往返时间
- 📥 **下载速度测试**：测试节点实际下载速度
- 🚄 支持多线程并发测速，快速评估节点质量
- ⏱️ 快速模式：设置时间限制快速筛选可用节点

### 🔄 协议转换
- 🔗 导出为分享链接（支持各种协议格式）
- 📝 转换为 Clash 配置文件
- 🔤 导出为 Base64 编码的订阅内容
- 🛠️ 订阅转换功能（集成 Subconverter）

### 💎 代理核心管理
- 🎯 **Mihomo（Clash 分支）**：现代化的代理核心，性能优越
- 📜 实时显示核心运行日志
- ⚙️ 一键启动/停止代理服务

---

<a name="快速开始"></a>

## 🚀 快速开始

### 📌 启动程序

双击运行 `nodesCatchNext.exe` 即可启动程序。

---

<a name="使用指南"></a>

## 📖 使用指南

### 1️⃣ 导入节点

<table>
<tr>
<td width="50%">

**方式一：从分享链接导入** 🔗

1. 复制代理节点的分享链接
   - `vmess://...`
   - `ss://...`
   - `trojan://...` 等
2. 在主界面点击 **从剪贴板导入**
3. 程序自动解析并添加到列表 ✅

</td>
<td width="50%">

**方式二：从订阅 URL 导入** 📡

1. 点击 **订阅设置** 按钮
2. 添加订阅 URL 并设置备注
3. 点击 **更新订阅**
4. 自动获取并导入所有节点 🎉

</td>
</tr>
</table>

### 2️⃣ 测速节点

```
1. 在节点列表中选择要测速的节点（支持多选）
2. 点击"测速"按钮
3. 选择测速类型：
   ⚡ 实时 Ping - 快速测试延迟
   📥 下载测速 - 测试实际速度和延迟
4. 等待测速完成，结果显示在列表中
```

### 3️⃣ 导出节点

| 导出方式 | 说明 |
|---------|------|
| 🔗 **导出分享链接** | 选中节点后右键选择"复制分享链接" |
| 📝 **导出 Clash 配置** | 点击"导出为 Clash 配置"保存为 `.yaml` 文件 |
| 📤 **导出订阅内容** | 点击"导出订阅"生成 Base64 编码的订阅内容 |

---

<a name="配置说明"></a>

## ⚙️ 配置说明

### 📄 nodeConfig.json

> 主配置文件，存储所有节点、订阅和程序设置，位于程序根目录。

<details>
<summary><b>点击查看配置项说明</b></summary>

```json
{
  "localPort": 40000,              // 🔌 本地代理端口
  "externalControllerPort": 40001, // 🎛️ 外部控制器端口
  "coreType": 1,                   // 💎 核心类型（Mihomo）
  "defAllowInsecure": true,        // 🔓 默认允许不安全连接
  "speedTestUrl": "...",           // 🌐 速度测试 URL
  "speedPingTestUrl": "...",       // 📡 Ping 测试 URL
  "pingAble": true,                // ✅ 是否启用 Ping
  "speedAble": true,               // ✅ 是否启用速度测试
  "fastMode": false,               // ⚡ 是否启用快速模式
  "Thread": "100",                 // 🧵 并发线程数
  "DownloadThread": "5",           // 📥 下载线程数
  "vmess": [],                     // 📦 节点列表
  "subItem": []                    // 📡 订阅列表
}
```

</details>

### 📁 config/ 目录

| 文件 | 说明 | 图标 |
|------|------|:---:|
| `config.yaml` | Clash/Mihomo 配置文件 | ⚙️ |
| `temp.yaml` | 临时生成的配置文件 | 📝 |
| `cache.db` | 缓存数据库 | 💾 |
| `Country.mmdb` | IP 地理位置数据库（GeoIP） | 🌍 |

---

## 📂 目录结构

```
📁 nodesCatchNext/
├── 🚀 nodesCatchNext.exe       # 主程序
├── 💎 mihomo-nodes.exe         # Mihomo 代理核心（如需更新，请自行到[Mihomo releases](https://github.com/MetaCubeX/mihomo/releases)下载对应系统版本）
├── 📚 Newtonsoft.Json.dll      # JSON 处理库（必要文件，请勿删除）
├── ⚙️ nodeConfig.json          # 主配置文件
├── 📁 config/                  # 配置文件目录
│   ├── ⚙️ config.yaml          # Clash 配置
│   ├── 📝 temp.yaml            # 临时配置
│   ├── 💾 cache.db             # 缓存数据库
│   └── 🌍 Country.mmdb         # GeoIP 数据库
└── 📁 subconverter/            # 订阅转换工具
    ├── 📁 base/                # 基础配置
    ├── 📁 config/              # 转换规则
    ├── 📁 profiles/            # 配置文件
    ├── 📁 rules/               # 分流规则
    └── 📁 snippets/            # 代码片段
```

---

## 💡 使用技巧

<table>
<tr>
<td width="50%">

### ⚡ 快速筛选可用节点

1. 导入大量节点后，启用 **快速模式**
2. 设置较短的超时时间（3-5 秒）
3. 运行测速，快速淘汰不可用节点
4. 对剩余节点进行完整测速

</td>
<td width="50%">

### 🎯 自定义测速 URL

1. 打开 `nodeConfig.json`
2. 修改配置项：
   - `speedTestUrl`
   - `speedPingTestUrl`
3. 建议使用稳定的测速服务器
4. 重启程序生效 🔄

</td>
</tr>
<tr>
<td width="50%">

### 🔧 批量操作

- ⌨️ 使用 `Ctrl` 或 `Shift` 键多选节点
- 🗑️ 支持批量删除、批量测速
- 📋 右键菜单提供更多批量操作

</td>
<td width="50%">

### 📡 订阅管理

- 🔄 定期更新订阅以获取最新节点
- 📚 可以设置多个订阅源
- 📝 建议为每个订阅添加备注

</td>
</tr>
</table>

---

## ⚠️ 注意事项

> 使用前请仔细阅读以下注意事项

| 项目 | 说明 |
|------|------|
| 🎯 **首次运行** | 程序会自动创建配置文件和必要目录 |
| 🔌 **端口占用** | 如果默认端口（40000）被占用，请在配置文件中修改 `localPort` |
| 🛡️ **防火墙** | 首次运行可能需要允许程序通过防火墙 |
| 💎 **代理核心** | 确保 `mihomo-nodes.exe` 与主程序在同一目录 |
| 💾 **配置备份** | 建议定期备份 `nodeConfig.json` 避免节点数据丢失 |
| ⏱️ **测速频率** | 避免过于频繁的测速，可能被服务器识别为异常流量 |
| ⚖️ **合法使用** | 请遵守当地法律法规，仅用于合法用途 |

---

## 💻 系统要求

| 项目 | 要求 |
|------|------|
| 🖥️ **操作系统** | Windows 7 及以上 |
| 🔧 **.NET Framework** | 4.8 或更高版本 |
| 🧠 **内存** | 至少 100MB 可用内存 |
| 💾 **磁盘空间** | 至少 50MB 可用空间 |

---

<a name="常见问题"></a>

## ❓ 常见问题

<details>
<summary><b>❌ Q: 节点导入失败？</b></summary>

**A:** 请检查分享链接格式是否正确，确保协议头（如 `vmess://`、`ss://`）完整。

✅ **解决方案：**
- 检查链接是否完整复制
- 确认链接格式符合标准
- 尝试手动添加协议头

</details>

<details>
<summary><b>⏱️ Q: 测速一直超时？</b></summary>

**A:** 可能是网络问题或节点失效

✅ **建议：**
- 🔍 检查本地网络连接
- 🔄 尝试更换测速 URL
- ⏰ 增加超时时间设置
- 🌐 确认网络环境正常

</details>

<details>
<summary><b>🚫 Q: 启动核心失败？</b></summary>

**A:** 请逐项检查以下内容：

✅ **检查清单：**
- [ ] 端口是否被占用
- [ ] `mihomo-nodes.exe` 是否存在
- [ ] 配置文件是否正确
- [ ] 查看日志窗口的错误信息
- [ ] 尝试更换端口号

</details>

<details>
<summary><b>🔧 Q: 如何更改本地代理端口？</b></summary>

**A:** 修改配置文件中的端口设置

📝 **步骤：**
1. 打开 `nodeConfig.json`
2. 找到 `localPort` 字段
3. 修改为目标端口（如 `7890`）
4. 保存文件并重启程序 ✅

</details>

---

## 🔄 更新日志

请查看 [GitHub Releases](../../releases) 页面获取最新版本和更新说明。

---

## 📜 许可证

本项目仅供学习和研究使用，请勿用于非法用途。

[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## ⚖️ 免责声明

> **重要提醒**

```
⚠️  本工具仅作为技术研究和学习使用
⚠️  使用本工具产生的任何后果由使用者自行承担
⚠️  请遵守当地法律法规，合理合法使用网络资源
⚠️  开发者不对使用本工具造成的任何问题负责
```

---

## 🙏 鸣谢

本项目基于 [**不良林**](https://www.youtube.com/@bulianglin) 的 [nodesCatch V2.0](https://bulianglin.com/archives/nodescatch.html) 进行二次开发。

### 开源项目引用

本项目使用了以下优秀的开源项目，在此表示感谢：

| 项目 | 说明 | 链接 |
|------|------|------|
| 💎 **Mihomo** | Clash 的一个分支，功能强大的代理核心 | [GitHub](https://github.com/MetaCubeX/mihomo) |
| 🚀 **V2Ray** | 网络代理工具，支持多种协议 | [GitHub](https://github.com/v2ray/v2ray-core) |
| 📱 **v2rayN** | Windows 平台的 V2Ray 客户端 | [GitHub](https://github.com/2dust/v2rayN) |
| 🔄 **Subconverter** | 订阅转换工具，支持多种配置格式转换 | [GitHub](https://github.com/tindy2013/subconverter) |
| 📚 **Newtonsoft.Json** | .NET 平台的高性能 JSON 框架 | [GitHub](https://github.com/JamesNK/Newtonsoft.Json) |

感谢所有开源项目作者的无私贡献！

---

<div align="center">

### 💖 感谢使用

**如果觉得这个项目有帮助，请给个 ⭐ Star 吧！**

Made with ❤️ by [AHackerX](https://github.com/AHackerX)

[⬆️ 回到顶部](#top)

</div>
