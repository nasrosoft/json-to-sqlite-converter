using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Timers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace jsontosql
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer syncTimer;
        private string jsonFolderPath = "";
        private string sqlitePath = "data.sqlite";
        private BackgroundWorker syncWorker;

        public MainForm()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            // Setup timer for hourly sync
            syncTimer = new System.Timers.Timer(3600000); // 1 hour in milliseconds
            syncTimer.Elapsed += OnTimerElapsed;
            syncTimer.AutoReset = true;

            // Setup background worker
            syncWorker = new BackgroundWorker();
            syncWorker.WorkerReportsProgress = true;
            syncWorker.DoWork += SyncWorker_DoWork;
            syncWorker.ProgressChanged += SyncWorker_ProgressChanged;
            syncWorker.RunWorkerCompleted += SyncWorker_RunWorkerCompleted;

            // Initialize SQLite database
            InitializeDatabase();

            // Load settings if they exist
            LoadSettings();
        }

        private void InitializeDatabase()
        {
            try
            {
                string connectionString = $"Data Source={sqlitePath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Database will be created automatically if it doesn't exist
                    LogMessage("SQLite database initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error initializing database: {ex.Message}");
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select folder containing JSON files";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    jsonFolderPath = dialog.SelectedPath;
                    lblFolderPath.Text = $"Folder: {jsonFolderPath}";
                    SaveSettings();
                    LogMessage($"JSON folder selected: {jsonFolderPath}");
                }
            }
        }

        private void btnManualSync_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(jsonFolderPath))
            {
                MessageBox.Show("Please select a JSON folder first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartSync();
        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(jsonFolderPath))
            {
                MessageBox.Show("Please select a JSON folder first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            syncTimer.Start();
            btnStartTimer.Enabled = false;
            btnStopTimer.Enabled = true;
            LogMessage("Automatic sync started (every hour)");
        }

        private void btnStopTimer_Click(object sender, EventArgs e)
        {
            syncTimer.Stop();
            btnStartTimer.Enabled = true;
            btnStopTimer.Enabled = false;
            LogMessage("Automatic sync stopped");
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select folder to export JSON files";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToJson(dialog.SelectedPath);
                }
            }
        }

        private void ExportToJson(string exportFolderPath)
        {
            try
            {
                btnExportJson.Enabled = false;
                progressBar.Value = 0;
                progressBar.Visible = true;
                lblStatus.Text = "Exporting to JSON...";

                string connectionString = $"Data Source={sqlitePath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Get all table names
                    var tableNames = new List<string>();
                    string getTablesQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";

                    using (var command = new SQLiteCommand(getTablesQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }

                    LogMessage($"Found {tableNames.Count} tables to export");

                    // Export each table
                    for (int i = 0; i < tableNames.Count; i++)
                    {
                        string tableName = tableNames[i];
                        progressBar.Value = (i * 100) / tableNames.Count;

                        LogMessage($"Exporting table: {tableName}");
                        ExportTableToJson(connection, tableName, exportFolderPath);
                    }

                    progressBar.Value = 100;
                    lblStatus.Text = $"Export completed: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    LogMessage($"All tables exported to: {exportFolderPath}");

                    MessageBox.Show($"Successfully exported {tableNames.Count} tables to JSON files!",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error during export: {ex.Message}");
                MessageBox.Show($"Export failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExportJson.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void ExportTableToJson(SQLiteConnection connection, string tableName, string exportFolderPath)
        {
            try
            {
                string selectQuery = $"SELECT * FROM [{tableName}]";
                var records = new List<Dictionary<string, object>>();

                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    // Get column names
                    var columnNames = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }

                    // Read all records
                    while (reader.Read())
                    {
                        var record = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = columnNames[i];
                            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                            // Convert back from SQLite storage format
                            if (value != null)
                            {
                                // Try to parse JSON strings back to objects/arrays
                                if (value is string stringValue &&
                                    (stringValue.StartsWith("{") || stringValue.StartsWith("[")))
                                {
                                    try
                                    {
                                        value = JToken.Parse(stringValue);
                                    }
                                    catch
                                    {
                                        // If parsing fails, keep as string
                                    }
                                }
                                // Convert boolean integers back to booleans
                                else if (value is long longValue && (longValue == 0 || longValue == 1))
                                {
                                    // Check if this was originally a boolean by column context
                                    // For now, keep as integer to avoid false conversions
                                }
                            }

                            record[columnName] = value;
                        }

                        records.Add(record);
                    }
                }

                // Create JSON file
                string jsonFilePath = Path.Combine(exportFolderPath, $"{tableName}.json");
                string jsonContent = JsonConvert.SerializeObject(records, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat
                    });

                File.WriteAllText(jsonFilePath, jsonContent, Encoding.UTF8);
                LogMessage($"Exported {records.Count} records to {tableName}.json");
            }
            catch (Exception ex)
            {
                LogMessage($"Error exporting table {tableName}: {ex.Message}");
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Invoke((Action)(() => StartSync()));
        }

        private void StartSync()
        {
            if (!syncWorker.IsBusy)
            {
                btnManualSync.Enabled = false;
                progressBar.Value = 0;
                progressBar.Visible = true;
                lblStatus.Text = "Syncing...";
                syncWorker.RunWorkerAsync();
            }
        }

        private void SyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json");
                syncWorker.ReportProgress(0, $"Found {jsonFiles.Length} JSON files");

                string connectionString = $"Data Source={sqlitePath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    for (int i = 0; i < jsonFiles.Length; i++)
                    {
                        string jsonFile = jsonFiles[i];
                        string fileName = Path.GetFileNameWithoutExtension(jsonFile);

                        syncWorker.ReportProgress((i * 100) / jsonFiles.Length,
                            $"Processing {fileName}...");

                        ProcessJsonFile(connection, jsonFile, fileName);
                    }
                }

                syncWorker.ReportProgress(100, "Sync completed successfully");
            }
            catch (Exception ex)
            {
                syncWorker.ReportProgress(0, $"Error during sync: {ex.Message}");
            }
        }

        private void ProcessJsonFile(SQLiteConnection connection, string jsonFilePath, string tableName)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);

                // Parse JSON - handle both arrays and single objects
                JToken jsonToken = JToken.Parse(jsonContent);
                List<JObject> records = new List<JObject>();

                if (jsonToken is JArray jsonArray)
                {
                    records.AddRange(jsonArray.Cast<JObject>());
                }
                else if (jsonToken is JObject jsonObject)
                {
                    records.Add(jsonObject);
                }

                if (records.Count == 0) return;

                // Create table based on first record structure
                CreateTableFromJson(connection, tableName, records.First());

                // Insert/Update records
                foreach (var record in records)
                {
                    UpsertRecord(connection, tableName, record);
                }
            }
            catch (Exception ex)
            {
                syncWorker.ReportProgress(0, $"Error processing {tableName}: {ex.Message}");
            }
        }

        private void CreateTableFromJson(SQLiteConnection connection, string tableName, JObject sample)
        {
            var columns = new List<string>();

            foreach (var property in sample.Properties())
            {
                string columnName = property.Name.Replace(" ", "_").Replace("-", "_");
                string sqlType = GetSqliteType(property.Value);
                columns.Add($"[{columnName}] {sqlType}");
            }

            // Add an ID column if not present
            if (!sample.Properties().Any(p => p.Name.ToLower() == "id"))
            {
                columns.Insert(0, "[id] INTEGER PRIMARY KEY AUTOINCREMENT");
            }

            string createTableSql = $@"
                CREATE TABLE IF NOT EXISTS [{tableName}] (
                    {string.Join(",\n                    ", columns)}
                )";

            using (var command = new SQLiteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private string GetSqliteType(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Integer:
                    return "INTEGER";
                case JTokenType.Float:
                    return "REAL";
                case JTokenType.Boolean:
                    return "INTEGER";
                case JTokenType.Date:
                    return "TEXT";
                case JTokenType.Array:
                case JTokenType.Object:
                    return "TEXT"; // Store as JSON string
                default:
                    return "TEXT";
            }
        }

        private void UpsertRecord(SQLiteConnection connection, string tableName, JObject record)
        {
            var properties = record.Properties().ToList();
            var columns = properties.Select(p => $"[{p.Name.Replace(" ", "_").Replace("-", "_")}]").ToList();
            var parameters = properties.Select(p => $"@{p.Name.Replace(" ", "_").Replace("-", "_")}").ToList();

            string sql = $@"
                INSERT OR REPLACE INTO [{tableName}] 
                ({string.Join(", ", columns)}) 
                VALUES ({string.Join(", ", parameters)})";

            using (var command = new SQLiteCommand(sql, connection))
            {
                foreach (var property in properties)
                {
                    string paramName = $"@{property.Name.Replace(" ", "_").Replace("-", "_")}";
                    object value = GetParameterValue(property.Value);
                    command.Parameters.AddWithValue(paramName, value);
                }

                command.ExecuteNonQuery();
            }
        }

        private object GetParameterValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Null:
                    return DBNull.Value;
                case JTokenType.Boolean:
                    return (bool)token ? 1 : 0;
                case JTokenType.Array:
                case JTokenType.Object:
                    return token.ToString();
                default:
                    return token.Value<object>();
            }
        }

        private void SyncWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
            {
                LogMessage(e.UserState.ToString());
            }
        }

        private void SyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnManualSync.Enabled = true;
            progressBar.Visible = false;
            lblStatus.Text = $"Last sync: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            LogMessage("Sync operation completed");
        }

        private void LogMessage(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke((Action)(() =>
                {
                    txtLog.AppendText(logEntry + Environment.NewLine);
                    txtLog.ScrollToCaret();
                }));
            }
            else
            {
                txtLog.AppendText(logEntry + Environment.NewLine);
                txtLog.ScrollToCaret();
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new
                {
                    JsonFolderPath = jsonFolderPath,
                    SqlitePath = sqlitePath
                };

                File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                LogMessage($"Error saving settings: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists("settings.json"))
                {
                    string settingsJson = File.ReadAllText("settings.json");
                    dynamic settings = JsonConvert.DeserializeObject(settingsJson);

                    if (settings.JsonFolderPath != null)
                    {
                        jsonFolderPath = settings.JsonFolderPath;
                        lblFolderPath.Text = $"Folder: {jsonFolderPath}";
                    }

                    if (settings.SqlitePath != null)
                    {
                        sqlitePath = settings.SqlitePath;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error loading settings: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            syncTimer?.Stop();
            syncTimer?.Dispose();
            SaveSettings();
            base.OnFormClosing(e);
        }
    }
}