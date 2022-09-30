using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DALGenerator {

    public partial class frmMain: Form {

        private SqlConnection DbConn = new SqlConnection();
        private const string AppName = "DAL Generator";
        private const string AppRegistryKey = @"Software\JGhost\DALGenerator";
        private const string Key = "Sd6Ci36qT3t6VTUv/62TGQ==";
        private bool Running = false;

        #region public frmMain()
        public frmMain() {
            InitializeComponent();
        }
        #endregion

        #region private void Log(string Message, Color? color)
        private void Log(string Message, Color? color) {
            if (this.InvokeRequired) {
                this.Invoke((MethodInvoker) delegate {
                    _Log(Message, color);
                });
            } else {
                _Log(Message, color);
            }
        }
        private void _Log(string Message, Color? color) {
            txtLog.SelectionColor = color == null ? Color.Black : color.Value;
            txtLog.AppendText(Message + "\n");
            Control control = this.ActiveControl;
            txtLog.Focus();
            control.Focus();
            Application.DoEvents();
        }
        #endregion

        #region private void FillDbServersList()
        private void FillDbServersList() {
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            cbServers.Items.Clear();
            foreach (DataRow row in dt.Rows) {
                if (row["InstanceName"].ToString() == "")
                    cbServers.Items.Add(row["ServerName"]);
                else
                    cbServers.Items.Add(row["ServerName"] + "\\" + row["InstanceName"]);
            }
            if (cbServers.Items.Count > 0)
                cbServers.SelectedIndex = 0;

        }
        #endregion

        #region private void ReadDatabases()
        private void ReadDatabases() {
            using (SqlDataAdapter adapter = new SqlDataAdapter("sp_databases", DbConn)) {
                cbDatabase.Items.Clear();
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table != null && table.Rows.Count > 0) {
                    foreach (DataRow row in table.Rows) {
                        cbDatabase.Items.Add(row["database_name"].ToString());
                    }
                }
            }
            if (cbDatabase.Items.Count > 0)
                cbDatabase.SelectedIndex = 0;
            string database = Reg.Read(AppRegistryKey, "Database", "");
            if (cbDatabase.Items.IndexOf(database) > -1) {
                cbDatabase.SelectedIndex = cbDatabase.Items.IndexOf(database);
            }
        }
        #endregion

        #region private void frmMain_Load(...)
        private void frmMain_Load(object sender, EventArgs e) {
            this.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.Text = AppName;

            grpOperation.Enabled = false;
            /*
            // Read Network SQL Servers (It is damn slow)
            grpDbConn.Enabled = false;
            Log("Reading Network SQL Servers ...", Color.Blue);
            Application.DoEvents();
            FillDbServersList();
            */
            grpDbConn.Enabled = true;

            Reg.LoadWindowPos(AppRegistryKey, this);

            string[] PrevServers = Reg.Read(AppRegistryKey, "Servers", new string[0]);
            cbServers.Items.AddRange(PrevServers);
            cbServers.Text = Reg.Read(AppRegistryKey, "server", "");
            cbAuthentication.SelectedIndex = Reg.Read(AppRegistryKey, "Authentication", 0);
            edtUserName.Text = Reg.Read(AppRegistryKey, "user", "");
            string pass = Reg.Read(AppRegistryKey, "pass", "");
            try {
                if (pass != "") {
                    pass = Crypto.AES.Decrypt(pass, Key);
                }
            } catch { }
            edtPassword.Text = pass;

            edtNamespace.Text = Reg.Read(AppRegistryKey, "namespace", "");
            edtSaveFolder.Text = Reg.Read(AppRegistryKey, "SaveFolder", Application.StartupPath.AddTrailingBackSlashes());

            cbModels.Checked = Reg.Read(AppRegistryKey, "Create_Models", 0) == 1;
            cbDALSuffix.Checked = Reg.Read(AppRegistryKey, "DAL_as_Suffix", 0) == 1;
            cbCreateSolution.Checked = Reg.Read(AppRegistryKey, "CreateSolution", 0) == 1;
            cbSaveLogToFile.Checked = Reg.Read(AppRegistryKey, "SaveLog", 1) == 1;
            cbOmitDbo.Checked = Reg.Read(AppRegistryKey, "OmitDbo", 0) == 1;
        }
        #endregion

        #region private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
            string[] PrevServers = new string[cbServers.Items.Count];
            cbServers.Items.CopyTo(PrevServers, 0);
            Reg.Write(AppRegistryKey, "Servers", PrevServers);
            Reg.Write(AppRegistryKey, "server", cbServers.Text);

            Reg.Write(AppRegistryKey, "Authentication", cbAuthentication.SelectedIndex);
            Reg.Write(AppRegistryKey, "user", edtUserName.Text);
            Reg.Write(AppRegistryKey, "pass", Crypto.AES.Encrypt(edtPassword.Text, Key));

            Reg.Write(AppRegistryKey, "Database", cbDatabase.Text);
            Reg.Write(AppRegistryKey, "SaveFolder", edtSaveFolder.Text);
            Reg.Write(AppRegistryKey, "namespace", edtNamespace.Text);

            Reg.Write(AppRegistryKey, "Create_Models", cbModels.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "DAL_as_Suffix", cbDALSuffix.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "CreateSolution", cbCreateSolution.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "SaveLog", cbSaveLogToFile.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "OmitDbo", cbOmitDbo.Checked ? 1 : 0);

            Reg.SaveWindowPos(AppRegistryKey, this);
        }
        #endregion

        #region private void cbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        private void cbAuthentication_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbAuthentication.SelectedIndex == 0) {
                edtPassword.Enabled = true;
                edtUserName.Enabled = true;
                lblPassword.Enabled = true;
                lblUserName.Enabled = true;
            } else {
                edtPassword.Enabled = false;
                edtUserName.Enabled = false;
                lblPassword.Enabled = false;
                lblUserName.Enabled = false;
            }
        }
        #endregion

        #region private void btnConnect_Click(...)
        private void btnConnect_Click(object sender, EventArgs e) {
            try {
                if (DbConn.State != ConnectionState.Open) {
                    Log("Trying to connect to " + cbServers.Text + " ...", Color.Blue);
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = cbServers.Text;
                    if (cbAuthentication.SelectedIndex == 0) {
                        builder.UserID = edtUserName.Text;
                        builder.Password = edtPassword.Text;
                        builder.IntegratedSecurity = false;
                    } else {
                        builder.IntegratedSecurity = true;
                    }
                    builder.InitialCatalog = "master";
                    DbConn.ConnectionString = builder.ConnectionString;
                    DbConn.Open();
                    btnConnect.Text = "Disconnect";
                    Log("  Connected", Color.Black);
                    grpOperation.Enabled = true;
                    ReadDatabases();

                    cbServers.Enabled = false;
                    cbAuthentication.Enabled = false;
                    edtUserName.Enabled = false;
                    edtPassword.Enabled = false;
                    lblUserName.Enabled = false;
                    lblPassword.Enabled = false;
                    lblAuthentication.Enabled = false;
                    lblServers.Enabled = false;
                } else {
                    DbConn.Close();
                    btnConnect.Text = "Connect";
                    Log("Server Disconnected", Color.Black);
                    grpOperation.Enabled = false;

                    lblAuthentication.Enabled = true;
                    lblServers.Enabled = true;
                    cbServers.Enabled = true;
                    cbAuthentication.Enabled = true;
                    cbAuthentication_SelectedIndexChanged(null, null);

                }
            } catch (Exception ex) {
                Log(ex.Message, Color.Red);
            }
        }
        #endregion

        #region private void btnRun_Click
        private async void btnRun_Click(object sender, EventArgs e) {
            if (Running)
                return;
            Running = true;
            btnRun.Text = "Building";
            txtLog.Clear();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = cbServers.Text;
            if (cbAuthentication.SelectedIndex == 0) {
                builder.UserID = edtUserName.Text;
                builder.Password = edtPassword.Text;
                builder.IntegratedSecurity = false;

            } else {
                builder.IntegratedSecurity = true;
            }
            builder.InitialCatalog = cbDatabase.Text;

            string name_space = edtNamespace.Text.Trim();
            if (name_space == "") {
                name_space = cbDatabase.SelectedItem.ToString();
            }

            name_space = name_space.Replace("[", "").Replace("]", "").Replace("-", "_");
            if (char.IsDigit(name_space[0]) || char.IsSymbol(name_space[0])) {
                name_space = "_" + name_space;
            }
            edtSaveFolder.Text = edtSaveFolder.Text.AddTrailingBackSlashes();

            await Task.Run(() => {
                dal_base DAL;
                if (cbModels.Checked) {
                    DAL = new dal_with_model(edtSaveFolder.Text, name_space, cbDALSuffix.Checked, cbOmitDbo.Checked, cbCreateSolution.Checked, builder.ConnectionString, Log);
                } else {
                    DAL = new dal_simple(edtSaveFolder.Text, name_space, cbDALSuffix.Checked, cbOmitDbo.Checked, cbCreateSolution.Checked, builder.ConnectionString, Log);
                }
                DAL.build();
            });
            Log("Done", Color.Blue);

            if (cbSaveLogToFile.Checked)
                txtLog.SaveFile(edtSaveFolder.Text + "DALGenerator.log.doc");
            Running = false;
            btnRun.Text = "Run";
        }
        #endregion

        #region private void btnBrowse_Click(...)
        private void btnBrowse_Click(object sender, EventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            if (Directory.Exists(edtSaveFolder.Text)) {
                dialog.SelectedPath = edtSaveFolder.Text;
            }
            if (dialog.ShowDialog(this) == DialogResult.OK) {
                edtSaveFolder.Text = dialog.SelectedPath;
            }
        }
        #endregion

    }
}

