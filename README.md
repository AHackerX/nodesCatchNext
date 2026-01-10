<a name="top"></a>

<div align="center">

# 🚀 nodesCatchNext

[![Platform](https://img.shields.io/badge/platform-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white)](https://www.microsoft.com/windows)
[![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Release](https://img.shields.io/badge/release-v3.8-FF6B35?style=for-the-badge&logo=github&logoColor=white)](../../releases)
[![License](https://img.shields.io/badge/license-MIT-00C853?style=for-the-badge)](LICENSE)

**🎯 基于 [不良林 nodesCatch V2.0](https://bulianglin.com/archives/nodescatch.html) 二次开发的代理节点管理和测速工具**

*支持多种代理协议的导入、管理、测速和转换*

---

**[📥 下载最新版](../../releases)** · **[🐛 反馈问题](../../issues)** · **[💡 功能建议](../../issues)**

---

[✨ 功能特性](#-功能特性-1) · [🚀 快速开始](#-快速开始-1) · [📖 使用指南](#-使用指南-1) · [⚙️ 配置说明](#️-配置说明-1) · [🔄 更新日志](#-更新日志)

</div>

<br>

## 🎉 v3.8 新版本亮点

<div align="center">

| 🆕 新增功能 | 📝 描述 |
|:---:|:---|
| 📄 **原生 YAML 解析** | 无需 subconverter 即可直接导入 Clash/Mihomo YAML 配置文件 |
| 🔐 **完整协议支持** | 支持 VLESS Reality、Hysteria2、VMess、Trojan、SS 等协议的 YAML 导入 |
| 🛠️ **智能错误提示** | 根据错误代码提供具体解决建议，自动检测系统架构 |
| 📦 **开箱即用** | 减少对外部组件的依赖，提升用户体验 |

</div>

<br>

## 🌟 相比原版的改进

> 相比原版 nodesCatch V2.0，本项目进行了以下优化和改进

<table>
<tr>
<td width="50%" valign="top">

### 🚀 性能优化

| 优化项 | 说明 |
|:---:|:---|
| ⚡ 测速算法 | 优化多线程并发，提升效率 |
| 📦 内存占用 | 减少大量节点时的内存消耗 |
| 📂 配置读写 | 改进配置文件的读写性能 |
| 🛡 容错能力 | 增强配置文件的容错处理 |

</td>
<td width="50%" valign="top">

### ✨ 功能增强

| 新功能 | 说明 |
|:---:|:---|
| 🔄 Mihomo 核心 | 升级至最新版本 |
| 🔒 新协议 | 支持 VLESS、Hysteria 2、AnyTLS |
| 🌐 地区筛选 | 快速筛选指定地区节点 |

</td>
</tr>
</table>

<br>

### 📝 已知问题

| 问题 | 描述 | 状态 |
|:---:|:---|:---:|
| 🐛 **部分节点测速异常** | VLESS+Reality 协议节点 HTTPS 延迟可能无结果，但下载测速正常 | 🛑 无法解决 |

> 💡 如果您发现任何问题或有改进建议，欢迎 [提交 Issue](../../issues)！

<br>

---

<a name="-功能特性-1"></a>

## ✨ 功能特性

<table>
<tr>
<td width="50%" valign="top">

### 📦 节点管理

- ✅ **多协议支持**
  - VMess / VLESS / Trojan
  - Shadowsocks / ShadowsocksR
  - Hysteria / Hysteria 2
  - TUIC / WireGuard / AnyTLS

- 🔄 **导入方式**
  - 订阅 URL 自动更新
  - 分享链接快速导入
  - 拖拽文件批量导入

- 🔧 **批量操作**
  - 智能去重 / 批量删除
  - 关键词筛选 / 地区预设

</td>
<td width="50%" valign="top">

### ⚡ 节点测速

- 📡 **延迟测试**
  - HTTPS 连接延迟
  - 多线程并发测速

- 📥 **速度测试**
  - 实际下载速度
  - 快速模式筛选
  - 自定义测速 URL

- 📊 **结果管理**
  - 自动排序 / 导出结果
  - 移除低速 / 无效节点

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 🔄 协议转换

- 🔗 导出分享链接
- 📝 转换 Clash/Mihomo 配置
- 🔤 导出 Base64 订阅
- 🛠️ 集成 Subconverter

</td>
<td width="50%" valign="top">

### 💎 代理核心

- 🎯 Mihomo 核心支持
- 📜 实时运行日志
- ⚙️ 一键启停服务
- 🚀 推送节点到核心

</td>
</tr>
</table>

---

<a name="-快速开始-1"></a>

## 🚀 快速开始

<div align="center">

### 📥 下载安装

[![Download](https://img.shields.io/badge/下载最新版-v3.8-FF6B35?style=for-the-badge&logo=github)](https://github.com/AHackerX/nodesCatchNext/releases)

</div>

```
1️⃣ 下载压缩包并解压到任意目录
2️⃣ 双击运行 nodesCatchNext.exe（建议以管理员模式运行）
3️⃣ 开始导入节点并测速！
```

> ⚠️ **重要提示**
> - 测速时请关闭其他代理软件，尤其是 TUN（虚拟网卡）模式
> - 代理软件运行中可能导致测速结果失真

<br>

---

<a name="-使用指南-1"></a>

## 📖 使用指南

### 1️⃣ 导入节点

<table>
<tr>
<td width="50%" valign="top">

#### 🔗 方式一：分享链接导入

```
① 复制节点分享链接
   • vmess://...
   • ss://...
   • trojan://...
   • vless://...
   • hysteria2://...
   • anytls://...

② 右键菜单 → 从剪贴板导入
   或使用快捷键 Ctrl+V

③ 自动解析并添加到列表 ✅
```

</td>
<td width="50%" valign="top">

#### 📡 方式二：订阅 URL 导入

```
① 点击「订阅管理」按钮

② 添加订阅 URL
   • 填写订阅地址
   • 设置备注名称

③ 点击「更新订阅」

④ 自动获取所有节点 🎉
```

</td>
</tr>
</table>

<br>

### 2️⃣ 测速节点

| 步骤 | 操作 | 说明 |
|:---:|:---|:---|
| **1** | 选择节点 | 单选 / 多选 / `Ctrl+A` 全选 |
| **2** | 点击测速 | 右键菜单或点击「一键自动测速」 |
| **3** | 选择类型 | 🌐 HTTPS延迟 · 📥 下载速度 |
| **4** | 等待完成 | 结果自动显示在列表中 |

<br>

### 3️⃣ 导出节点

| 导出方式 | 操作 | 快捷键 |
|:---|:---|:---:|
| 🔗 **分享链接** | 右键 → 导出分享URL到剪贴板 | `Ctrl+C` |
| 📝 **Clash 配置** | 右键 → 导出Mihomo订阅文件 | - |
| 🔤 **Base64 订阅** | 右键 → 导出Base64通用订阅文件 | - |
| 📋 **订阅内容** | 右键 → 导出订阅内容到剪贴板 | - |

<br>

### ⌨️ 快捷键速查

| 快捷键 | 功能 | 快捷键 | 功能 |
|:---:|:---|:---:|:---|
| `Ctrl+R` | 测试 HTTPS 延迟 | `Ctrl+A` | 全选节点 |
| `Ctrl+T` | 测试下载速度 | `Ctrl+C` | 导出分享链接 |
| `Ctrl+E` | 一键测速选中节点 | `Ctrl+V` | 从剪贴板导入 |
| `Delete` | 删除选中节点 | `双击` | 编辑节点 |

---

<a name="️-配置说明-1"></a>

## ⚙️ 配置说明

### 📄 主配置文件

> `nodeConfig.json` - 存储所有节点、订阅和程序设置

<details>
<summary><b>📋 点击展开配置项说明</b></summary>

```json
{
  "localPort": 40000,              // 🔌 本地代理端口
  "ClashPort": "9090",             // 🎛️ Mihomo 外部控制端口
  "defAllowInsecure": true,        // 🔓 默认允许不安全连接
  "speedTestUrl": "...",           // 🌐 下载测速 URL
  "speedPingTestUrl": "...",       // 📡 延迟测试 URL
  "pingAble": true,                // ✅ 是否启用延迟测试
  "speedAble": true,               // ✅ 是否启用速度测试
  "fastMode": true,                // ⚡ 是否启用快速模式
  "Thread": "100",                 // 🧵 延迟测速线程数
  "vmess": [],                     // 📦 节点列表
  "subItem": []                    // 📡 订阅列表
}
```

</details>

<br>

### 📁 目录结构

```
📁 nodesCatchNext/
│
├── 🚀 nodesCatchNext.exe          # 主程序
├── 💎 mihomo-nodes.exe            # Mihomo 代理核心
├── 📚 Newtonsoft.Json.dll         # JSON 处理库（必要）
├── ⚙️ nodeConfig.json             # 主配置文件
│
├── 📁 config/                     # 运行时配置
│   ├── config.yaml                # Mihomo 配置
│   ├── cache.db                   # 缓存数据库
│   └── Country.mmdb               # GeoIP 数据库
│
└── 📁 subconverter/               # 订阅转换工具
    └── ...
```

> 💡 **提示**：如需更新 Mihomo 核心，请到 [Mihomo Releases](https://github.com/MetaCubeX/mihomo/releases) 下载对应版本

<br>

---

## 💡 使用技巧

<table>
<tr>
<td width="50%" valign="top">

### ⚡ 快速筛选节点

```
① 启用「快速模式」
② 设置较短超时时间（3-5秒）
③ 设置峰值速度阈值
④ 运行测速，快速淘汰慢节点
⑤ 对剩余节点完整测速
```

</td>
<td width="50%" valign="top">

### 🌍 地区预设筛选

```
① 勾选「关键词筛选」
② 点击 ▼ 按钮
③ 选择地区预设：
   🇭🇰 香港  🇹🇼 台湾
   🇯🇵 日本  🇺🇸 美国
   🇸🇬 新加坡 🇰🇷 韩国
④ 自动填入筛选关键词
```

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 🔧 批量操作

- `Ctrl` / `Shift` 多选节点
- 右键菜单批量操作
- 支持批量删除、测速、导出

</td>
<td width="50%" valign="top">

### ⚙️ 自定义测速 URL

- 点击「设置」按钮
- 修改延迟测速 URL
- 修改下载测速 URL
- 建议使用稳定服务器

</td>
</tr>
</table>

<br>

---

## ⚠️ 注意事项 & 系统要求

<table>
<tr>
<td width="50%" valign="top">

### 📋 注意事项

| 项目 | 说明 |
|:---:|:---|
| 🎯 **首次运行** | 自动创建配置文件 |
| 🔌 **端口占用** | 修改配置中的端口 |
| 🛡️ **防火墙** | 允许程序通过 |
| 💾 **配置备份** | 定期备份配置文件 |
| ⏱️ **测速频率** | 避免过于频繁 |
| ⚖️ **合法使用** | 遵守当地法规 |

</td>
<td width="50%" valign="top">

### 💻 系统要求

| 项目 | 要求 |
|:---:|:---|
| 🖥️ **操作系统** | Windows 7+ |
| 🔧 **.NET** | Framework 4.8+ |
| 🧠 **内存** | ≥ 100MB |
| 💾 **磁盘** | ≥ 50MB |

</td>
</tr>
</table>

---

## 🔄 更新日志

<details open>
<summary><b>🏷️ v3.8 (最新版本)</b></summary>

- ✨ 新增原生 Clash YAML 配置解析功能，无需依赖 subconverter 即可直接导入
- 🔐 支持 VLESS（包括 Reality）、VMess、Shadowsocks、Trojan、Hysteria2 等协议的 YAML 导入
- 🛠️ Mihomo 内核启动失败时，根据错误代码提供具体的解决建议
- 🔍 自动检测系统架构，提示下载对应版本（amd64/386）
- 📝 改进错误提示信息，明确告知用户缺少哪些文件或配置
- 🐛 修复反编译残留代码导致的编译错误

</details>

<details>
<summary><b>🏷️ v3.7</b></summary>

- ⚙️ 设置界面新增关闭程序时保存配置按钮
- 🔄 设置界面新增"订阅更新模式"，分为覆盖更新和添加更新
- 🎨 修改"允许直接关闭程序"和"记住窗口位置"设置逻辑
- 🐛 修复订阅Tab栏更新单个订阅会导致更新所有订阅的Bug
- ⚡ 调整下载测速逻辑，确保每个节点使用不同的API端口测速

</details>

<details>
<summary><b>🏷️ v3.5</b></summary>

- ✨ 新增 AnyTLS 协议支持
- 🎨 优化 UI 布局，合并控制面板
- 🌍 新增地区预设快速筛选
- 🔧 改进节点去重逻辑（包含备注字段）
- ⚙️ 将测速 URL 设置移至设置界面
- 🗑️ 移除冗余的内核选择功能

</details>

<details>
<summary><b>🏷️ v3.0</b></summary>

- 🔄 升级至最新版 Mihomo 核心
- ⚡ 初步支持 VLESS 和 Hysteria 2 协议
- 💾 优化配置文件管理

</details>

> 📋 查看完整更新日志：[GitHub Releases](../../releases)

<br>

---

## 📜 许可证 & 免责声明

<table>
<tr>
<td width="50%" valign="top">

### 📄 许可证

本项目基于 **MIT License** 开源

[![License](https://img.shields.io/badge/license-MIT-00C853?style=for-the-badge)](LICENSE)

</td>
<td width="50%" valign="top">

### ⚠️ 免责声明

- 本工具仅供学习和研究使用
- 使用产生的后果由使用者承担
- 请遵守当地法律法规
- 开发者不对任何问题负责
- 如本项目侵犯了您的权益，请联系 scratchqaq@gmail.com 进行删除

</td>
</tr>
</table>

<br>

---

## 🙏 鸣谢

<div align="center">

**本项目基于 [不良林](https://www.youtube.com/@bulianglin) 的 [nodesCatch V2.0](https://bulianglin.com/archives/nodescatch.html) 二次开发**

</div>

### 开源项目引用

| 项目 | 说明 |
|:---:|:---|
| [💎 Mihomo](https://github.com/MetaCubeX/mihomo) | Clash 分支，功能强大的代理核心 |
| [🚀 V2Ray](https://github.com/v2ray/v2ray-core) | 网络代理工具，支持多种协议 |
| [📱 v2rayN](https://github.com/2dust/v2rayN) | Windows 平台的 V2Ray 客户端 |
| [🔄 Subconverter](https://github.com/tindy2013/subconverter) | 订阅转换工具 |
| [📚 Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) | .NET 高性能 JSON 框架 |

<br>

---

<div align="center">

## ⭐ Star History

如果这个项目对你有帮助，请给个 **Star** 支持一下！

<br>

[![Star History Chart](https://api.star-history.com/svg?repos=AHackerX/nodesCatchNext&type=Date)](https://star-history.com/#AHackerX/nodesCatchNext&Date)

<br>

---

### 💖 感谢使用

Made with ❤️ by [AHackerX](https://github.com/AHackerX)

**[⬆️ 回到顶部](#top)**

</div>
