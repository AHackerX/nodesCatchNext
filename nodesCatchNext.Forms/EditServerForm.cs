using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using nodesCatchNext.Mode;

namespace nodesCatchNext.Forms;

public class EditServerForm : Form
{
	private IContainer components;

	private GroupBox groupBox1;

	private Label lblRemarks;

	private TextBox txtRemarks;

	private Label lblAddress;

	private TextBox txtAddress;

	private Label lblPort;

	private TextBox txtPort;

	private Label lblId;

	private TextBox txtId;

	private Label lblAlterId;

	private TextBox txtAlterId;

	private Label lblSecurity;

	private ComboBox cmbSecurity;

	private Label lblNetwork;

	private ComboBox cmbNetwork;

	private Label lblHeaderType;

	private TextBox txtHeaderType;

	private Label lblRequestHost;

	private TextBox txtRequestHost;

	private Label lblPath;

	private TextBox txtPath;

	private Label lblStreamSecurity;

	private ComboBox cmbStreamSecurity;

	private Label lblSni;

	private TextBox txtSni;

	private Label lblFingerprint;

	private TextBox txtFingerprint;

	private Label lblFlow;

	private TextBox txtFlow;

	private Label lblConfigType;

	private TextBox txtConfigType;

	private Button btnOK;

	private Button btnCancel;

	private VmessItem vmessItem;

	private int serverIndex;

	public VmessItem ResultItem => vmessItem;

	public int ServerIndex => serverIndex;

	public EditServerForm(VmessItem item, int index)
	{
		vmessItem = item;
		serverIndex = index;
		InitializeComponent();
		LoadServerInfo();
		try
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
			if (File.Exists(text))
			{
				base.Icon = new Icon(text);
			}
		}
		catch
		{
		}
		base.AcceptButton = btnOK;
		base.Shown += delegate
		{
			btnOK.Focus();
		};
	}

	private void LoadServerInfo()
	{
		if (vmessItem != null)
		{
			txtRemarks.Text = vmessItem.remarks;
			txtAddress.Text = vmessItem.address;
			txtPort.Text = vmessItem.port.ToString();
			txtId.Text = vmessItem.id;
			txtAlterId.Text = vmessItem.alterId.ToString();
			txtHeaderType.Text = vmessItem.headerType;
			txtRequestHost.Text = vmessItem.requestHost;
			txtPath.Text = vmessItem.path;
			txtSni.Text = vmessItem.sni;
			txtFingerprint.Text = vmessItem.fingerprint;
			txtFlow.Text = vmessItem.flow;
			string text = ((EConfigType)vmessItem.configType/*cast due to .constrained prefix*/).ToString();
			txtConfigType.Text = text;
			cmbSecurity.Text = vmessItem.security;
			cmbNetwork.Text = vmessItem.network;
			cmbStreamSecurity.Text = vmessItem.streamSecurity;
		}
	}

	private bool SaveServerInfo()
	{
		if (string.IsNullOrWhiteSpace(txtAddress.Text))
		{
			MessageBox.Show("地址不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		if (!int.TryParse(txtPort.Text, out var result) || result <= 0 || result > 65535)
		{
			MessageBox.Show("端口必须是1-65535之间的数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		vmessItem.remarks = txtRemarks.Text.Trim();
		vmessItem.address = txtAddress.Text.Trim();
		vmessItem.port = result;
		vmessItem.id = txtId.Text.Trim();
		if (int.TryParse(txtAlterId.Text, out var result2))
		{
			vmessItem.alterId = result2;
		}
		vmessItem.security = cmbSecurity.Text;
		vmessItem.network = cmbNetwork.Text;
		vmessItem.headerType = txtHeaderType.Text.Trim();
		vmessItem.requestHost = txtRequestHost.Text.Trim();
		vmessItem.path = txtPath.Text.Trim();
		vmessItem.streamSecurity = cmbStreamSecurity.Text;
		vmessItem.sni = txtSni.Text.Trim();
		vmessItem.fingerprint = txtFingerprint.Text.Trim();
		vmessItem.flow = txtFlow.Text.Trim();
		return true;
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		if (SaveServerInfo())
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
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
		this.components = new System.ComponentModel.Container();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.lblRemarks = new System.Windows.Forms.Label();
		this.txtRemarks = new System.Windows.Forms.TextBox();
		this.lblAddress = new System.Windows.Forms.Label();
		this.txtAddress = new System.Windows.Forms.TextBox();
		this.lblPort = new System.Windows.Forms.Label();
		this.txtPort = new System.Windows.Forms.TextBox();
		this.lblId = new System.Windows.Forms.Label();
		this.txtId = new System.Windows.Forms.TextBox();
		this.lblAlterId = new System.Windows.Forms.Label();
		this.txtAlterId = new System.Windows.Forms.TextBox();
		this.lblSecurity = new System.Windows.Forms.Label();
		this.cmbSecurity = new System.Windows.Forms.ComboBox();
		this.lblNetwork = new System.Windows.Forms.Label();
		this.cmbNetwork = new System.Windows.Forms.ComboBox();
		this.lblHeaderType = new System.Windows.Forms.Label();
		this.txtHeaderType = new System.Windows.Forms.TextBox();
		this.lblRequestHost = new System.Windows.Forms.Label();
		this.txtRequestHost = new System.Windows.Forms.TextBox();
		this.lblPath = new System.Windows.Forms.Label();
		this.txtPath = new System.Windows.Forms.TextBox();
		this.lblStreamSecurity = new System.Windows.Forms.Label();
		this.cmbStreamSecurity = new System.Windows.Forms.ComboBox();
		this.lblSni = new System.Windows.Forms.Label();
		this.txtSni = new System.Windows.Forms.TextBox();
		this.lblFingerprint = new System.Windows.Forms.Label();
		this.txtFingerprint = new System.Windows.Forms.TextBox();
		this.lblFlow = new System.Windows.Forms.Label();
		this.txtFlow = new System.Windows.Forms.TextBox();
		this.lblConfigType = new System.Windows.Forms.Label();
		this.txtConfigType = new System.Windows.Forms.TextBox();
		this.btnOK = new System.Windows.Forms.Button();
		this.btnCancel = new System.Windows.Forms.Button();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.lblConfigType);
		this.groupBox1.Controls.Add(this.txtConfigType);
		this.groupBox1.Controls.Add(this.lblRemarks);
		this.groupBox1.Controls.Add(this.txtRemarks);
		this.groupBox1.Controls.Add(this.lblAddress);
		this.groupBox1.Controls.Add(this.txtAddress);
		this.groupBox1.Controls.Add(this.lblPort);
		this.groupBox1.Controls.Add(this.txtPort);
		this.groupBox1.Controls.Add(this.lblId);
		this.groupBox1.Controls.Add(this.txtId);
		this.groupBox1.Controls.Add(this.lblAlterId);
		this.groupBox1.Controls.Add(this.txtAlterId);
		this.groupBox1.Controls.Add(this.lblSecurity);
		this.groupBox1.Controls.Add(this.cmbSecurity);
		this.groupBox1.Controls.Add(this.lblNetwork);
		this.groupBox1.Controls.Add(this.cmbNetwork);
		this.groupBox1.Controls.Add(this.lblHeaderType);
		this.groupBox1.Controls.Add(this.txtHeaderType);
		this.groupBox1.Controls.Add(this.lblRequestHost);
		this.groupBox1.Controls.Add(this.txtRequestHost);
		this.groupBox1.Controls.Add(this.lblPath);
		this.groupBox1.Controls.Add(this.txtPath);
		this.groupBox1.Controls.Add(this.lblStreamSecurity);
		this.groupBox1.Controls.Add(this.cmbStreamSecurity);
		this.groupBox1.Controls.Add(this.lblSni);
		this.groupBox1.Controls.Add(this.txtSni);
		this.groupBox1.Controls.Add(this.lblFingerprint);
		this.groupBox1.Controls.Add(this.txtFingerprint);
		this.groupBox1.Controls.Add(this.lblFlow);
		this.groupBox1.Controls.Add(this.txtFlow);
		this.groupBox1.Font = new System.Drawing.Font("Microsoft YaHei", 9f);
		this.groupBox1.Location = new System.Drawing.Point(12, 12);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(740, 580);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "节点信息";
		int num = 25;
		int num2 = 120;
		int num3 = 580;
		int num4 = 36;
		int num5 = 35;
		int num6 = 0;
		this.lblConfigType.AutoSize = true;
		this.lblConfigType.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblConfigType.Name = "lblConfigType";
		this.lblConfigType.Text = "类型:";
		this.txtConfigType.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtConfigType.Name = "txtConfigType";
		this.txtConfigType.Size = new System.Drawing.Size(num3, 23);
		this.txtConfigType.ReadOnly = true;
		this.txtConfigType.BackColor = System.Drawing.SystemColors.Control;
		num6++;
		this.lblRemarks.AutoSize = true;
		this.lblRemarks.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblRemarks.Name = "lblRemarks";
		this.lblRemarks.Text = "别名:";
		this.txtRemarks.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtRemarks.Name = "txtRemarks";
		this.txtRemarks.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblAddress.AutoSize = true;
		this.lblAddress.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblAddress.Name = "lblAddress";
		this.lblAddress.Text = "地址:";
		this.txtAddress.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtAddress.Name = "txtAddress";
		this.txtAddress.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblPort.AutoSize = true;
		this.lblPort.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblPort.Name = "lblPort";
		this.lblPort.Text = "端口:";
		this.txtPort.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtPort.Name = "txtPort";
		this.txtPort.Size = new System.Drawing.Size(100, 23);
		num6++;
		this.lblId.AutoSize = true;
		this.lblId.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblId.Name = "lblId";
		this.lblId.Text = "用户ID:";
		this.txtId.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtId.Name = "txtId";
		this.txtId.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblAlterId.AutoSize = true;
		this.lblAlterId.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblAlterId.Name = "lblAlterId";
		this.lblAlterId.Text = "额外ID:";
		this.txtAlterId.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtAlterId.Name = "txtAlterId";
		this.txtAlterId.Size = new System.Drawing.Size(100, 23);
		num6++;
		this.lblSecurity.AutoSize = true;
		this.lblSecurity.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblSecurity.Name = "lblSecurity";
		this.lblSecurity.Text = "加密方式:";
		this.cmbSecurity.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.cmbSecurity.Name = "cmbSecurity";
		this.cmbSecurity.Size = new System.Drawing.Size(150, 25);
		this.cmbSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.cmbSecurity.Items.AddRange(new object[14]
		{
			"auto", "aes-128-gcm", "chacha20-poly1305", "none", "zero", "aes-256-gcm", "aes-128-cfb", "aes-256-cfb", "chacha20", "chacha20-ietf",
			"xchacha20", "2022-blake3-aes-128-gcm", "2022-blake3-aes-256-gcm", "2022-blake3-chacha20-poly1305"
		});
		num6++;
		this.lblNetwork.AutoSize = true;
		this.lblNetwork.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblNetwork.Name = "lblNetwork";
		this.lblNetwork.Text = "传输协议:";
		this.cmbNetwork.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.cmbNetwork.Name = "cmbNetwork";
		this.cmbNetwork.Size = new System.Drawing.Size(150, 25);
		this.cmbNetwork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.cmbNetwork.Items.AddRange(new object[7] { "tcp", "kcp", "ws", "h2", "quic", "grpc", "httpupgrade" });
		num6++;
		this.lblHeaderType.AutoSize = true;
		this.lblHeaderType.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblHeaderType.Name = "lblHeaderType";
		this.lblHeaderType.Text = "伪装类型:";
		this.txtHeaderType.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtHeaderType.Name = "txtHeaderType";
		this.txtHeaderType.Size = new System.Drawing.Size(150, 23);
		num6++;
		this.lblRequestHost.AutoSize = true;
		this.lblRequestHost.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblRequestHost.Name = "lblRequestHost";
		this.lblRequestHost.Text = "伪装域名:";
		this.txtRequestHost.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtRequestHost.Name = "txtRequestHost";
		this.txtRequestHost.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblPath.AutoSize = true;
		this.lblPath.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblPath.Name = "lblPath";
		this.lblPath.Text = "路径:";
		this.txtPath.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtPath.Name = "txtPath";
		this.txtPath.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblStreamSecurity.AutoSize = true;
		this.lblStreamSecurity.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblStreamSecurity.Name = "lblStreamSecurity";
		this.lblStreamSecurity.Text = "传输安全:";
		this.cmbStreamSecurity.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.cmbStreamSecurity.Name = "cmbStreamSecurity";
		this.cmbStreamSecurity.Size = new System.Drawing.Size(150, 25);
		this.cmbStreamSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.cmbStreamSecurity.Items.AddRange(new object[3] { "", "tls", "reality" });
		num6++;
		this.lblSni.AutoSize = true;
		this.lblSni.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblSni.Name = "lblSni";
		this.lblSni.Text = "SNI:";
		this.txtSni.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtSni.Name = "txtSni";
		this.txtSni.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblFingerprint.AutoSize = true;
		this.lblFingerprint.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblFingerprint.Name = "lblFingerprint";
		this.lblFingerprint.Text = "指纹:";
		this.txtFingerprint.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtFingerprint.Name = "txtFingerprint";
		this.txtFingerprint.Size = new System.Drawing.Size(num3, 23);
		num6++;
		this.lblFlow.AutoSize = true;
		this.lblFlow.Location = new System.Drawing.Point(num, num5 + num6 * num4);
		this.lblFlow.Name = "lblFlow";
		this.lblFlow.Text = "流控:";
		this.txtFlow.Location = new System.Drawing.Point(num2, num5 + num6 * num4 - 3);
		this.txtFlow.Name = "txtFlow";
		this.txtFlow.Size = new System.Drawing.Size(num3, 23);
		this.btnOK.Location = new System.Drawing.Point(280, 610);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(90, 32);
		this.btnOK.TabIndex = 1;
		this.btnOK.Text = "确定";
		this.btnOK.UseVisualStyleBackColor = true;
		this.btnOK.Click += new System.EventHandler(btnOK_Click);
		this.btnCancel.Location = new System.Drawing.Point(400, 610);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(90, 32);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "取消";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(776, 660);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.btnCancel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "EditServerForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "编辑节点";
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
