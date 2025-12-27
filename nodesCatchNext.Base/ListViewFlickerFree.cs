using System;
using System.Drawing;
using System.Windows.Forms;

namespace nodesCatchNext.Base;

internal class ListViewFlickerFree : ListView
{
	public ListViewFlickerFree()
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		UpdateStyles();
	}

	public void AutoResizeColumns()
	{
		try
		{
			SuspendLayout();
			Graphics graphics = CreateGraphics();
			AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			for (int i = 0; i < base.Columns.Count; i++)
			{
				ColumnHeader columnHeader = base.Columns[i];
				int val = columnHeader.Width;
				string text = "";
				Font font = base.Items[0].SubItems[0].Font;
				foreach (ListViewItem item in base.Items)
				{
					font = item.SubItems[i].Font;
					string text2 = item.SubItems[i].Text;
					if (text2.Length > text.Length)
					{
						text = text2;
					}
				}
				int val2 = (int)graphics.MeasureString(text, font).Width;
				columnHeader.Width = Math.Max(val, val2);
			}
			ResumeLayout();
		}
		catch
		{
		}
	}
}
