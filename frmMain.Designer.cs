namespace DALGenerator {
    partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnConnect = new System.Windows.Forms.Button();
            this.edtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.cbServers = new System.Windows.Forms.ComboBox();
            this.lblAuthentication = new System.Windows.Forms.Label();
            this.lblServers = new System.Windows.Forms.Label();
            this.cbAuthentication = new System.Windows.Forms.ComboBox();
            this.edtUserName = new System.Windows.Forms.TextBox();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.grpOperation = new System.Windows.Forms.GroupBox();
            this.cbSaveLogToFile = new System.Windows.Forms.CheckBox();
            this.cbCreateSolution = new System.Windows.Forms.CheckBox();
            this.cbDALSuffix = new System.Windows.Forms.CheckBox();
            this.edtNamespace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbModels = new System.Windows.Forms.CheckBox();
            this.cbDatabase = new System.Windows.Forms.ComboBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.edtSaveFolder = new System.Windows.Forms.TextBox();
            this.lblDestFolder = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.grpDbConn = new System.Windows.Forms.GroupBox();
            this.cbOmitDbo = new System.Windows.Forms.CheckBox();
            this.grpLog.SuspendLayout();
            this.grpOperation.SuspendLayout();
            this.grpDbConn.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(455, 144);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(88, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // edtPassword
            // 
            this.edtPassword.Location = new System.Drawing.Point(202, 147);
            this.edtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.edtPassword.Name = "edtPassword";
            this.edtPassword.PasswordChar = '*';
            this.edtPassword.Size = new System.Drawing.Size(179, 23);
            this.edtPassword.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(199, 127);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(63, 16);
            this.lblPassword.TabIndex = 7;
            this.lblPassword.Text = "Password";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(20, 127);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(71, 16);
            this.lblUserName.TabIndex = 6;
            this.lblUserName.Text = "User Name";
            // 
            // cbServers
            // 
            this.cbServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbServers.FormattingEnabled = true;
            this.cbServers.Location = new System.Drawing.Point(7, 43);
            this.cbServers.Margin = new System.Windows.Forms.Padding(4);
            this.cbServers.Name = "cbServers";
            this.cbServers.Size = new System.Drawing.Size(535, 24);
            this.cbServers.TabIndex = 0;
            // 
            // lblAuthentication
            // 
            this.lblAuthentication.AutoSize = true;
            this.lblAuthentication.Location = new System.Drawing.Point(7, 73);
            this.lblAuthentication.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAuthentication.Name = "lblAuthentication";
            this.lblAuthentication.Size = new System.Drawing.Size(89, 16);
            this.lblAuthentication.TabIndex = 3;
            this.lblAuthentication.Text = "Authentication";
            // 
            // lblServers
            // 
            this.lblServers.AutoSize = true;
            this.lblServers.Location = new System.Drawing.Point(7, 23);
            this.lblServers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServers.Name = "lblServers";
            this.lblServers.Size = new System.Drawing.Size(83, 16);
            this.lblServers.TabIndex = 2;
            this.lblServers.Text = "Server Name";
            // 
            // cbAuthentication
            // 
            this.cbAuthentication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAuthentication.FormattingEnabled = true;
            this.cbAuthentication.Items.AddRange(new object[] {
            "SqlServer Authentication",
            "Windows Authentication"});
            this.cbAuthentication.Location = new System.Drawing.Point(7, 92);
            this.cbAuthentication.Margin = new System.Windows.Forms.Padding(4);
            this.cbAuthentication.Name = "cbAuthentication";
            this.cbAuthentication.Size = new System.Drawing.Size(535, 24);
            this.cbAuthentication.TabIndex = 1;
            this.cbAuthentication.SelectedIndexChanged += new System.EventHandler(this.cbAuthentication_SelectedIndexChanged);
            // 
            // edtUserName
            // 
            this.edtUserName.Location = new System.Drawing.Point(24, 147);
            this.edtUserName.Margin = new System.Windows.Forms.Padding(4);
            this.edtUserName.Name = "edtUserName";
            this.edtUserName.Size = new System.Drawing.Size(171, 23);
            this.edtUserName.TabIndex = 2;
            // 
            // grpLog
            // 
            this.grpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Location = new System.Drawing.Point(7, 456);
            this.grpLog.Margin = new System.Windows.Forms.Padding(4);
            this.grpLog.Name = "grpLog";
            this.grpLog.Padding = new System.Windows.Forms.Padding(4);
            this.grpLog.Size = new System.Drawing.Size(549, 361);
            this.grpLog.TabIndex = 1;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(7, 25);
            this.txtLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(535, 329);
            this.txtLog.TabIndex = 8;
            this.txtLog.Text = "";
            // 
            // grpOperation
            // 
            this.grpOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOperation.Controls.Add(this.cbOmitDbo);
            this.grpOperation.Controls.Add(this.cbSaveLogToFile);
            this.grpOperation.Controls.Add(this.cbCreateSolution);
            this.grpOperation.Controls.Add(this.cbDALSuffix);
            this.grpOperation.Controls.Add(this.edtNamespace);
            this.grpOperation.Controls.Add(this.label1);
            this.grpOperation.Controls.Add(this.cbModels);
            this.grpOperation.Controls.Add(this.cbDatabase);
            this.grpOperation.Controls.Add(this.lblDatabase);
            this.grpOperation.Controls.Add(this.btnBrowse);
            this.grpOperation.Controls.Add(this.edtSaveFolder);
            this.grpOperation.Controls.Add(this.lblDestFolder);
            this.grpOperation.Controls.Add(this.btnRun);
            this.grpOperation.Location = new System.Drawing.Point(7, 199);
            this.grpOperation.Margin = new System.Windows.Forms.Padding(4);
            this.grpOperation.Name = "grpOperation";
            this.grpOperation.Padding = new System.Windows.Forms.Padding(4);
            this.grpOperation.Size = new System.Drawing.Size(549, 257);
            this.grpOperation.TabIndex = 3;
            this.grpOperation.TabStop = false;
            this.grpOperation.Text = "Operation";
            // 
            // cbSaveLogToFile
            // 
            this.cbSaveLogToFile.AutoSize = true;
            this.cbSaveLogToFile.Location = new System.Drawing.Point(304, 208);
            this.cbSaveLogToFile.Margin = new System.Windows.Forms.Padding(4);
            this.cbSaveLogToFile.Name = "cbSaveLogToFile";
            this.cbSaveLogToFile.Size = new System.Drawing.Size(115, 20);
            this.cbSaveLogToFile.TabIndex = 11;
            this.cbSaveLogToFile.Text = "Save Log to file";
            this.cbSaveLogToFile.UseVisualStyleBackColor = true;
            // 
            // cbCreateSolution
            // 
            this.cbCreateSolution.AutoSize = true;
            this.cbCreateSolution.Location = new System.Drawing.Point(304, 184);
            this.cbCreateSolution.Margin = new System.Windows.Forms.Padding(4);
            this.cbCreateSolution.Name = "cbCreateSolution";
            this.cbCreateSolution.Size = new System.Drawing.Size(139, 20);
            this.cbCreateSolution.TabIndex = 10;
            this.cbCreateSolution.Text = "Create Solution File";
            this.cbCreateSolution.UseVisualStyleBackColor = true;
            // 
            // cbDALSuffix
            // 
            this.cbDALSuffix.AutoSize = true;
            this.cbDALSuffix.Location = new System.Drawing.Point(8, 184);
            this.cbDALSuffix.Margin = new System.Windows.Forms.Padding(4);
            this.cbDALSuffix.Name = "cbDALSuffix";
            this.cbDALSuffix.Size = new System.Drawing.Size(269, 20);
            this.cbDALSuffix.TabIndex = 9;
            this.cbDALSuffix.Text = "DAL as Namespace Suffix Instead of Prefix";
            this.cbDALSuffix.UseVisualStyleBackColor = true;
            // 
            // edtNamespace
            // 
            this.edtNamespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtNamespace.Location = new System.Drawing.Point(8, 96);
            this.edtNamespace.Margin = new System.Windows.Forms.Padding(4);
            this.edtNamespace.Name = "edtNamespace";
            this.edtNamespace.Size = new System.Drawing.Size(535, 23);
            this.edtNamespace.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 72);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(297, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Namespace (will be used as project name as well)";
            // 
            // cbModels
            // 
            this.cbModels.AutoSize = true;
            this.cbModels.Location = new System.Drawing.Point(8, 208);
            this.cbModels.Margin = new System.Windows.Forms.Padding(4);
            this.cbModels.Name = "cbModels";
            this.cbModels.Size = new System.Drawing.Size(186, 20);
            this.cbModels.TabIndex = 6;
            this.cbModels.Text = "Generate Model Classes too";
            this.cbModels.UseVisualStyleBackColor = true;
            // 
            // cbDatabase
            // 
            this.cbDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDatabase.FormattingEnabled = true;
            this.cbDatabase.Location = new System.Drawing.Point(7, 41);
            this.cbDatabase.Margin = new System.Windows.Forms.Padding(4);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new System.Drawing.Size(535, 24);
            this.cbDatabase.TabIndex = 0;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(10, 21);
            this.lblDatabase.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(61, 16);
            this.lblDatabase.TabIndex = 5;
            this.lblDatabase.Text = "Database";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(424, 152);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "…";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // edtSaveFolder
            // 
            this.edtSaveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtSaveFolder.Location = new System.Drawing.Point(8, 152);
            this.edtSaveFolder.Margin = new System.Windows.Forms.Padding(4);
            this.edtSaveFolder.Name = "edtSaveFolder";
            this.edtSaveFolder.Size = new System.Drawing.Size(440, 23);
            this.edtSaveFolder.TabIndex = 1;
            // 
            // lblDestFolder
            // 
            this.lblDestFolder.AutoSize = true;
            this.lblDestFolder.Location = new System.Drawing.Point(8, 128);
            this.lblDestFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDestFolder.Name = "lblDestFolder";
            this.lblDestFolder.Size = new System.Drawing.Size(76, 16);
            this.lblDestFolder.TabIndex = 1;
            this.lblDestFolder.Text = "Save Folder";
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(455, 152);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(91, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // grpDbConn
            // 
            this.grpDbConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDbConn.Controls.Add(this.btnConnect);
            this.grpDbConn.Controls.Add(this.edtPassword);
            this.grpDbConn.Controls.Add(this.lblPassword);
            this.grpDbConn.Controls.Add(this.lblUserName);
            this.grpDbConn.Controls.Add(this.cbServers);
            this.grpDbConn.Controls.Add(this.lblAuthentication);
            this.grpDbConn.Controls.Add(this.lblServers);
            this.grpDbConn.Controls.Add(this.cbAuthentication);
            this.grpDbConn.Controls.Add(this.edtUserName);
            this.grpDbConn.Location = new System.Drawing.Point(7, 7);
            this.grpDbConn.Margin = new System.Windows.Forms.Padding(4);
            this.grpDbConn.Name = "grpDbConn";
            this.grpDbConn.Padding = new System.Windows.Forms.Padding(4);
            this.grpDbConn.Size = new System.Drawing.Size(549, 186);
            this.grpDbConn.TabIndex = 0;
            this.grpDbConn.TabStop = false;
            this.grpDbConn.Text = "Database Connection";
            // 
            // cbOmitDbo
            // 
            this.cbOmitDbo.AutoSize = true;
            this.cbOmitDbo.Location = new System.Drawing.Point(8, 232);
            this.cbOmitDbo.Margin = new System.Windows.Forms.Padding(4);
            this.cbOmitDbo.Name = "cbOmitDbo";
            this.cbOmitDbo.Size = new System.Drawing.Size(242, 20);
            this.cbOmitDbo.TabIndex = 12;
            this.cbOmitDbo.Text = "Don\'t Add dbo Schema to Namespace";
            this.cbOmitDbo.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(564, 828);
            this.Controls.Add(this.grpOperation);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpDbConn);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(580, 683);
            this.Name = "frmMain";
            this.Text = "DAL Generator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpLog.ResumeLayout(false);
            this.grpOperation.ResumeLayout(false);
            this.grpOperation.PerformLayout();
            this.grpDbConn.ResumeLayout(false);
            this.grpDbConn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAuthentication;
        private System.Windows.Forms.Label lblServers;
        private System.Windows.Forms.ComboBox cbAuthentication;
        private System.Windows.Forms.TextBox edtUserName;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.ComboBox cbServers;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox edtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.GroupBox grpOperation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox edtSaveFolder;
        private System.Windows.Forms.Label lblDestFolder;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ComboBox cbDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.CheckBox cbModels;
        private System.Windows.Forms.TextBox edtNamespace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpDbConn;
        private System.Windows.Forms.CheckBox cbDALSuffix;
        private System.Windows.Forms.CheckBox cbCreateSolution;
        private System.Windows.Forms.CheckBox cbSaveLogToFile;
        private System.Windows.Forms.CheckBox cbOmitDbo;
    }
}