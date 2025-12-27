using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using nodesCatchNext.Base;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Forms;

public class SubSettingControl : UserControl
{
	private IContainer components;

	private GroupBox grbMain;

	private CheckBox chkEnabled;

	private TextBox txtUrl;

	private TextBox txtRemark;

	private Label label2;

	private Label label1;

	private Button btnDelete;

	public SubItem subItem { get; set; }

	public event ChangeEventHandler OnButtonClicked;

	public SubSettingControl()
	{
		InitializeComponent();
	}

	private void SubSettingControl_Load(object sender, EventArgs e)
	{
		base.Height = grbMain.Height;
		BindingSub();
	}

	private void BindingSub()
	{
		if (subItem != null)
		{
			txtRemark.Text = subItem.remarks.ToString();
			txtUrl.Text = subItem.url.ToString();
			chkEnabled.Checked = subItem.enabled;
		}
	}

	private void EndBindingSub()
	{
		if (subItem != null)
		{
			subItem.remarks = txtRemark.Text.TrimEx();
			subItem.url = txtUrl.Text.TrimEx();
			subItem.enabled = chkEnabled.Checked;
		}
	}

	private void txtRemark_Leave(object sender, EventArgs e)
	{
		EndBindingSub();
		MainForm.Instance?.InitSubscriptionTabs();
	}

	private void btnDelete_Click(object sender, EventArgs e)
	{
		if (subItem != null && MainForm.config?.subItem != null)
		{
			string text = (string.IsNullOrWhiteSpace(subItem.remarks) ? "该订阅" : ("订阅\"" + subItem.remarks + "\""));
			if (MessageBox.Show("确定要删除 " + text + " 吗？", "删除订阅", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				MainForm.config.subItem.Remove(subItem);
				MainForm.Instance?.InitSubscriptionTabs();
				this.OnButtonClicked?.Invoke(sender, e);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.grbMain = new System.Windows.Forms.GroupBox();
		this.btnDelete = new System.Windows.Forms.Button();
		this.chkEnabled = new System.Windows.Forms.CheckBox();
		this.txtUrl = new System.Windows.Forms.TextBox();
		this.txtRemark = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.grbMain.SuspendLayout();
		base.SuspendLayout();
		this.grbMain.Controls.Add(this.btnDelete);
		this.grbMain.Controls.Add(this.chkEnabled);
		this.grbMain.Controls.Add(this.txtUrl);
		this.grbMain.Controls.Add(this.txtRemark);
		this.grbMain.Controls.Add(this.label2);
		this.grbMain.Controls.Add(this.label1);
		this.grbMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grbMain.Location = new System.Drawing.Point(0, 0);
		this.grbMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.grbMain.Name = "grbMain";
		this.grbMain.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.grbMain.Size = new System.Drawing.Size(540, 98);
		this.grbMain.TabIndex = 0;
		this.grbMain.TabStop = false;
		this.btnDelete.Location = new System.Drawing.Point(390, 58);
		this.btnDelete.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.btnDelete.Name = "btnDelete";
		this.btnDelete.Size = new System.Drawing.Size(68, 20);
		this.btnDelete.TabIndex = 28;
		this.btnDelete.Text = "删除订阅";
		this.btnDelete.UseVisualStyleBackColor = true;
		this.btnDelete.Click += new System.EventHandler(btnDelete_Click);
		this.chkEnabled.AutoSize = true;
		this.chkEnabled.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.chkEnabled.Location = new System.Drawing.Point(400, 32);
		this.chkEnabled.Name = "chkEnabled";
		this.chkEnabled.Size = new System.Drawing.Size(48, 16);
		this.chkEnabled.TabIndex = 26;
		this.chkEnabled.Text = "启用";
		this.chkEnabled.UseVisualStyleBackColor = true;
		this.chkEnabled.Leave += new System.EventHandler(txtRemark_Leave);
		this.txtUrl.Location = new System.Drawing.Point(58, 58);
		this.txtUrl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.txtUrl.Name = "txtUrl";
		this.txtUrl.Size = new System.Drawing.Size(330, 21);
		this.txtUrl.TabIndex = 3;
		this.txtUrl.Leave += new System.EventHandler(txtRemark_Leave);
		this.txtRemark.Location = new System.Drawing.Point(58, 30);
		this.txtRemark.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.txtRemark.Name = "txtRemark";
		this.txtRemark.Size = new System.Drawing.Size(330, 21);
		this.txtRemark.TabIndex = 2;
		this.txtRemark.Leave += new System.EventHandler(txtRemark_Leave);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(22, 62);
		this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(41, 12);
		this.label2.TabIndex = 1;
		this.label2.Text = "网址：";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(22, 32);
		this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(41, 12);
		this.label1.TabIndex = 0;
		this.label1.Text = "备注：";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.grbMain);
		base.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		base.Name = "SubSettingControl";
		base.Size = new System.Drawing.Size(540, 98);
		base.Load += new System.EventHandler(SubSettingControl_Load);
		this.grbMain.ResumeLayout(false);
		this.grbMain.PerformLayout();
		base.ResumeLayout(false);
	}
}
