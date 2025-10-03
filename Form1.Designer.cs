namespace jsontosql
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnSelectFolder;
        private Button btnManualSync;
        private Button btnStartTimer;
        private Button btnStopTimer;
        private Button btnExportJson;
        private Label lblFolderPath;
        private Label lblStatus;
        private ProgressBar progressBar;
        private TextBox txtLog;
        private GroupBox groupBoxSettings;
        private GroupBox groupBoxSync;
        private GroupBox groupBoxLog;

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
            this.btnSelectFolder = new Button();
            this.btnManualSync = new Button();
            this.btnStartTimer = new Button();
            this.btnStopTimer = new Button();
            this.btnExportJson = new Button();
            this.lblFolderPath = new Label();
            this.lblStatus = new Label();
            this.progressBar = new ProgressBar();
            this.txtLog = new TextBox();
            this.groupBoxSettings = new GroupBox();
            this.groupBoxSync = new GroupBox();
            this.groupBoxLog = new GroupBox();

            this.groupBoxSettings.SuspendLayout();
            this.groupBoxSync.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();

            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.btnSelectFolder);
            this.groupBoxSettings.Controls.Add(this.lblFolderPath);
            this.groupBoxSettings.Location = new Point(12, 12);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new Size(560, 80);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";

            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new Point(15, 25);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new Size(120, 30);
            this.btnSelectFolder.TabIndex = 0;
            this.btnSelectFolder.Text = "Select JSON Folder";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new EventHandler(this.btnSelectFolder_Click);

            // 
            // lblFolderPath
            // 
            this.lblFolderPath.AutoSize = true;
            this.lblFolderPath.Location = new Point(150, 32);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new Size(120, 15);
            this.lblFolderPath.TabIndex = 1;
            this.lblFolderPath.Text = "No folder selected";

            // 
            // groupBoxSync
            // 
            this.groupBoxSync.Controls.Add(this.btnManualSync);
            this.groupBoxSync.Controls.Add(this.btnStartTimer);
            this.groupBoxSync.Controls.Add(this.btnStopTimer);
            this.groupBoxSync.Controls.Add(this.btnExportJson);
            this.groupBoxSync.Controls.Add(this.lblStatus);
            this.groupBoxSync.Controls.Add(this.progressBar);
            this.groupBoxSync.Location = new Point(12, 108);
            this.groupBoxSync.Name = "groupBoxSync";
            this.groupBoxSync.Size = new Size(560, 100);
            this.groupBoxSync.TabIndex = 1;
            this.groupBoxSync.TabStop = false;
            this.groupBoxSync.Text = "Synchronization";

            // 
            // btnManualSync
            // 
            this.btnManualSync.Location = new Point(15, 25);
            this.btnManualSync.Name = "btnManualSync";
            this.btnManualSync.Size = new Size(100, 30);
            this.btnManualSync.TabIndex = 0;
            this.btnManualSync.Text = "Sync Now";
            this.btnManualSync.UseVisualStyleBackColor = true;
            this.btnManualSync.Click += new EventHandler(this.btnManualSync_Click);

            // 
            // btnStartTimer
            // 
            this.btnStartTimer.Location = new Point(130, 25);
            this.btnStartTimer.Name = "btnStartTimer";
            this.btnStartTimer.Size = new Size(100, 30);
            this.btnStartTimer.TabIndex = 1;
            this.btnStartTimer.Text = "Start Auto";
            this.btnStartTimer.UseVisualStyleBackColor = true;
            this.btnStartTimer.Click += new EventHandler(this.btnStartTimer_Click);

            // 
            // btnStopTimer
            // 
            this.btnStopTimer.Enabled = false;
            this.btnStopTimer.Location = new Point(245, 25);
            this.btnStopTimer.Name = "btnStopTimer";
            this.btnStopTimer.Size = new Size(100, 30);
            this.btnStopTimer.TabIndex = 2;
            this.btnStopTimer.Text = "Stop Auto";
            this.btnStopTimer.UseVisualStyleBackColor = true;
            this.btnStopTimer.Click += new EventHandler(this.btnStopTimer_Click);

            // 
            // btnExportJson
            // 
            this.btnExportJson.Location = new Point(360, 25);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new Size(100, 30);
            this.btnExportJson.TabIndex = 3;
            this.btnExportJson.Text = "Export JSON";
            this.btnExportJson.UseVisualStyleBackColor = true;
            this.btnExportJson.Click += new EventHandler(this.btnExportJson_Click);

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(15, 65);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(39, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready";

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(480, 30);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(65, 20);
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;

            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Controls.Add(this.txtLog);
            this.groupBoxLog.Location = new Point(12, 224);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new Size(560, 200);
            this.groupBoxLog.TabIndex = 2;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "Activity Log";

            // 
            // txtLog
            // 
            this.txtLog.Location = new Point(15, 25);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.Size = new Size(530, 160);
            this.txtLog.TabIndex = 0;

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(584, 441);
            this.Controls.Add(this.groupBoxLog);
            this.Controls.Add(this.groupBoxSync);
            this.Controls.Add(this.groupBoxSettings);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "JSON to SQLite Converter";

            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxSync.ResumeLayout(false);
            this.groupBoxSync.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}