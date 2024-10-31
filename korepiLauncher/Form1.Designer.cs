namespace korepiLauncher
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnDownloadSelected;
        private System.Windows.Forms.ComboBox cmbReleases;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel titleBar;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnGetHWID;
        private System.Windows.Forms.CheckBox chkLoadAccounts;
        private System.Windows.Forms.ComboBox cmbAccounts;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnDownloadSelected = new Button();
            cmbReleases = new ComboBox();
            lblStatus = new Label();
            progressBar = new ProgressBar();
            titleBar = new Panel();
            btnClose = new Button();
            btnMinimize = new Button();
            btnGetHWID = new Button();
            chkLoadAccounts = new CheckBox();
            cmbAccounts = new ComboBox();
            btnRunKp = new Button();
            pictureBox1 = new PictureBox();
            titleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnDownloadSelected
            // 
            btnDownloadSelected.BackColor = Color.MediumSlateBlue;
            btnDownloadSelected.BackgroundImage = (Image)resources.GetObject("btnDownloadSelected.BackgroundImage");
            btnDownloadSelected.BackgroundImageLayout = ImageLayout.Center;
            btnDownloadSelected.FlatAppearance.BorderSize = 0;
            btnDownloadSelected.FlatStyle = FlatStyle.Flat;
            btnDownloadSelected.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDownloadSelected.ForeColor = Color.White;
            btnDownloadSelected.ImageAlign = ContentAlignment.MiddleLeft;
            btnDownloadSelected.Location = new Point(16, 117);
            btnDownloadSelected.Margin = new Padding(4, 3, 4, 3);
            btnDownloadSelected.Name = "btnDownloadSelected";
            btnDownloadSelected.Size = new Size(36, 36);
            btnDownloadSelected.TabIndex = 1;
            btnDownloadSelected.UseVisualStyleBackColor = false;
            btnDownloadSelected.Click += BtnDownloadSelected_Click;
            btnDownloadSelected.MouseEnter += Btn_MouseEnter;
            btnDownloadSelected.MouseLeave += Btn_MouseLeave;
            // 
            // cmbReleases
            // 
            cmbReleases.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReleases.Location = new Point(15, 74);
            cmbReleases.Margin = new Padding(4, 3, 4, 3);
            cmbReleases.Name = "cmbReleases";
            cmbReleases.Size = new Size(175, 23);
            cmbReleases.TabIndex = 2;
            cmbReleases.SelectedIndexChanged += CmbReleases_SelectedIndexChanged; 
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 10F);
            lblStatus.Location = new Point(16, 214);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(47, 19);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Status";
            lblStatus.Click += LblStatus_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 184);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(437, 27);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 4;
            // 
            // titleBar
            // 
            titleBar.BackColor = Color.MediumSlateBlue;
            titleBar.Controls.Add(pictureBox1);
            titleBar.Controls.Add(btnClose);
            titleBar.Controls.Add(btnMinimize);
            titleBar.Dock = DockStyle.Top;
            titleBar.Location = new Point(0, 0);
            titleBar.Name = "titleBar";
            titleBar.Size = new Size(467, 30);
            titleBar.TabIndex = 0;
            titleBar.Paint += TitleBar_Paint;
            titleBar.MouseDown += titleBar_MouseDown;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.MediumSlateBlue;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(421, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(30, 24);
            btnClose.TabIndex = 0;
            btnClose.Text = "X";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += BtnClose_Click;
            // 
            // btnMinimize
            // 
            btnMinimize.BackColor = Color.MediumSlateBlue;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(391, 3);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(30, 24);
            btnMinimize.TabIndex = 1;
            btnMinimize.Text = "-";
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += BtnMinimize_Click;
            // 
            // btnGetHWID
            // 
            btnGetHWID.BackColor = Color.MediumSlateBlue;
            btnGetHWID.FlatAppearance.BorderSize = 0;
            btnGetHWID.FlatStyle = FlatStyle.Flat;
            btnGetHWID.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGetHWID.ForeColor = Color.White;
            btnGetHWID.Location = new Point(86, 117);
            btnGetHWID.Margin = new Padding(4, 3, 4, 3);
            btnGetHWID.Name = "btnGetHWID";
            btnGetHWID.Size = new Size(104, 36);
            btnGetHWID.TabIndex = 5;
            btnGetHWID.Text = "Copy HWID";
            btnGetHWID.UseVisualStyleBackColor = false;
            btnGetHWID.Click += BtnGetHWID_Click;
            btnGetHWID.MouseEnter += Btn_MouseEnter;
            btnGetHWID.MouseLeave += Btn_MouseLeave;
            // 
            // chkLoadAccounts
            // 
            chkLoadAccounts.Location = new Point(277, 51);
            chkLoadAccounts.Name = "chkLoadAccounts";
            chkLoadAccounts.Size = new Size(150, 17);
            chkLoadAccounts.TabIndex = 6;
            chkLoadAccounts.Text = "Load Accounts";
            chkLoadAccounts.CheckedChanged += ChkLoadAccounts_CheckedChanged;
            // 
            // cmbAccounts
            // 
            cmbAccounts.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAccounts.Enabled = false;
            cmbAccounts.Location = new Point(277, 74);
            cmbAccounts.Name = "cmbAccounts";
            cmbAccounts.Size = new Size(175, 23);
            cmbAccounts.TabIndex = 7;
            // 
            // btnRunKp
            // 
            btnRunKp.BackColor = Color.MediumSlateBlue;
            btnRunKp.FlatAppearance.BorderSize = 0;
            btnRunKp.FlatStyle = FlatStyle.Flat;
            btnRunKp.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRunKp.ForeColor = Color.White;
            btnRunKp.Location = new Point(277, 117);
            btnRunKp.Margin = new Padding(4, 3, 4, 3);
            btnRunKp.Name = "btnRunKp";
            btnRunKp.Size = new Size(174, 36);
            btnRunKp.TabIndex = 8;
            btnRunKp.Text = "Run Korepi";
            btnRunKp.UseVisualStyleBackColor = false;
            btnRunKp.Click += BtnRunKp_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(32, 32);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            pictureBox1.UseWaitCursor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(467, 246);
            Controls.Add(btnRunKp);
            Controls.Add(btnGetHWID);
            Controls.Add(titleBar);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Controls.Add(cmbReleases);
            Controls.Add(btnDownloadSelected);
            Controls.Add(chkLoadAccounts);
            Controls.Add(cmbAccounts);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "korepiLauncher";
            titleBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        // Enable dragging of the custom title bar
        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xA1, 0x2, 0);
            }
        }

        // Event handlers for custom title bar
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Button hover effects
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            (sender as Button).BackColor = System.Drawing.Color.DarkSlateBlue;
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            (sender as Button).BackColor = System.Drawing.Color.MediumSlateBlue;
        }

        private Button btnRunKp;
        private PictureBox pictureBox1;
    }
}

