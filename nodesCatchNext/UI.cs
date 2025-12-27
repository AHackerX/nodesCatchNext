using System.Windows.Forms;

namespace nodesCatchNext;

internal class UI
{
	public static void Show(string msg, IWin32Window owner = null)
	{
		MessageBox.Show(owner, msg, "nodesCatchNext", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	public static void ShowWarning(string msg, IWin32Window owner = null)
	{
		MessageBox.Show(owner, msg, "nodesCatchNext", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}

	public static void ShowError(string msg, IWin32Window owner = null)
	{
		MessageBox.Show(owner, msg, "nodesCatchNext", MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	public static DialogResult ShowYesNo(string msg, IWin32Window owner = null)
	{
		return MessageBox.Show(owner, msg, "nodesCatchNext", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
	}
}
