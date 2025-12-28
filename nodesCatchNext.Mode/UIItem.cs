using System;
using System.Collections.Generic;
using System.Drawing;

namespace nodesCatchNext.Mode;

[Serializable]
public class UIItem
{
	public Size mainSize { get; set; }

	public Point mainLocation { get; set; }

	public Dictionary<string, int> mainLvColWidth { get; set; }

	/// <summary>
	/// 列可见性配置，key为列名，value为是否显示
	/// </summary>
	public Dictionary<string, bool> mainLvColVisible { get; set; }

	/// <summary>
	/// 列顺序配置，按顺序存储列名
	/// </summary>
	public List<string> mainLvColOrder { get; set; }
}
