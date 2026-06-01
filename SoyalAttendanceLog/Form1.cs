using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SoyalAttendanceLog
{
    public partial class Form1 : Form
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private readonly ISoyalDeviceClient _deviceClient = new FakeSoyalDeviceClient();
        private readonly ExcelExportService _excelExportService = new ExcelExportService();
        private readonly RawTcpService _rawTcpService = new RawTcpService();
        private readonly SoyalWebClient _soyalWebClient = new SoyalWebClient();
        private readonly AppConfigService _configService = new AppConfigService();

        private CancellationTokenSource _cts;
        private Button btnLoadRealLogs;
        private TextBox txtIp;
        private TextBox txtPort;
        private TextBox txtDeviceId;
        private Button btnTest;
        private Button btnStart;
        private Button btnExport;
        private DataGridView dgvLogs;
        private TextBox txtRawCommand;
        private TextBox txtRawResponse;
        private Button btnSendRaw;
        private Label lblTotalLogs;
        private Button btnClear;
        private Label lblStatus;
        private Button btnSaveConfig;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnFilter;
        private TextBox txtEmployeeUserId;
        private TextBox txtEmployeeName;
        private Button btnSaveEmployee;
        private Button btnSearchUser;
        private Button btnShowAll;
        private Label lblEmployees;
        private Label lblLastSync;
        private Button btnRefresh;
        private Button btnManageEmployees;
        private Label lblTodayLogs;
        private Label lblUniqueToday;
        private Label lblInvalidToday;
        public Form1()
        {
            InitializeComponent();
            BuildUi();
            LoadConfig();

            _databaseService.InitializeDatabase();
            LoadLogsToGrid();
        }
        private void LoadConfig()
        {
            var config = _configService.Load();

            txtIp.Text = config.Ip;
            txtPort.Text = config.Port;
            txtDeviceId.Text = config.DeviceId;
        }
        private void BuildUi()
        {
            Text = "SOYAL Attendance Manager";
            Width = 1200;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Top;
            mainPanel.Height = 285;
            mainPanel.Padding = new Padding(10);

            // =========================
            // Group 1: Device Connection
            // =========================
            var gbDevice = new GroupBox();
            gbDevice.Text = "Device Connection";
            gbDevice.Left = 10;
            gbDevice.Top = 10;
            gbDevice.Width = 560;
            gbDevice.Height = 80;

            var lblIp = new Label { Text = "IP:", Left = 15, Top = 35, Width = 30 };
            txtIp = new TextBox { Left = 45, Top = 30, Width = 130, Text = "192.168.1.143" };

            var lblPort = new Label { Text = "Port:", Left = 190, Top = 35, Width = 40 };
            txtPort = new TextBox { Left = 235, Top = 30, Width = 70, Text = "1621" };

            var lblDeviceId = new Label { Text = "Device ID:", Left = 320, Top = 35, Width = 70 };
            txtDeviceId = new TextBox { Left = 395, Top = 30, Width = 60, Text = "1" };

            btnTest = new Button { Text = "Test", Left = 470, Top = 28, Width = 70 };
            btnTest.Click += BtnTest_Click;

            gbDevice.Controls.Add(lblIp);
            gbDevice.Controls.Add(txtIp);
            gbDevice.Controls.Add(lblPort);
            gbDevice.Controls.Add(txtPort);
            gbDevice.Controls.Add(lblDeviceId);
            gbDevice.Controls.Add(txtDeviceId);
            gbDevice.Controls.Add(btnTest);

            // =========================
            // Group 2: Dashboard
            // =========================
            var gbDashboard = new GroupBox();
            gbDashboard.Text = "Dashboard";
            gbDashboard.Left = 590;
            gbDashboard.Top = 10;
            gbDashboard.Width = 560;
            gbDashboard.Height = 105;

            lblStatus = new Label
            {
                Left = 15,
                Top = 25,
                Width = 200,
                Text = "Status: Ready"
            };

            lblTotalLogs = new Label
            {
                Left = 300,
                Top = 25,
                Width = 150,
                Text = "Total Logs: 0"
            };

            lblEmployees = new Label
            {
                Left = 300,
                Top = 50,
                Width = 150,
                Text = "Employees: 0"
            };

            lblLastSync = new Label
            {
                Left = 15,
                Top = 50,
                Width = 200,
                Text = "Last Sync: -"
            };

            gbDashboard.Controls.Add(lblStatus);
            gbDashboard.Controls.Add(lblLastSync);
            gbDashboard.Controls.Add(lblTotalLogs);
            gbDashboard.Controls.Add(lblEmployees);

            lblTodayLogs = new Label
            {
                Left = 15,
                Top = 75,
                Width = 150,
                Text = "Today Logs: 0"
            };

            lblUniqueToday = new Label
            {
                Left = 220,
                Top = 75,
                Width = 180,
                Text = "Unique Today: 0"
            };

            lblInvalidToday = new Label
            {
                Left = 430,
                Top = 75,
                Width = 150,
                Text = "Invalid Today: 0"
            };

            gbDashboard.Controls.Add(lblTodayLogs);
            gbDashboard.Controls.Add(lblUniqueToday);
            gbDashboard.Controls.Add(lblInvalidToday);

            // =========================
            // Group 3: Actions
            // =========================
            var gbActions = new GroupBox();
            gbActions.Text = "Actions";
            gbActions.Left = 10;
            gbActions.Top = 125;
            gbActions.Width = 1140;
            gbActions.Height = 65;

            btnLoadRealLogs = new Button { Text = "Load Logs", Left = 15, Top = 25, Width = 110 };
            btnLoadRealLogs.Click += BtnLoadRealLogs_Click;

            btnStart = new Button { Text = "Auto Sync", Left = 135, Top = 25, Width = 110 };
            btnStart.Click += BtnStart_Click;

            

            btnExport = new Button { Text = "Export Excel", Left = 375, Top = 25, Width = 110 };
            btnExport.Click += BtnExport_Click;


            btnRefresh = new Button
            {
                Text = "Refresh",
                Left = 255,
                Top = 25,
                Width = 100
            };
            gbActions.Controls.Add(btnRefresh);

            btnRefresh.Click += BtnRefresh_Click;

            btnSaveConfig = new Button { Text = "Save Config", Left = 495, Top = 25, Width = 110 };
            btnSaveConfig.Click += BtnSaveConfig_Click;

            gbActions.Controls.Add(btnLoadRealLogs);
            gbActions.Controls.Add(btnStart);
           
            gbActions.Controls.Add(btnExport);
            gbActions.Controls.Add(btnSaveConfig);

            //btnManageEmployees = new Button
            //{
            //    Text = "Manage Employees",
            //    Left = 565,
            //    Top = 25,
            //    Width = 140
            //};
            //btnManageEmployees.Click += BtnManageEmployees_Click;

            //gbActions.Controls.Add(btnManageEmployees);

            // =========================
            // Group 4: Filter & Employee
            // =========================
            var gbFilterEmployee = new GroupBox();
            gbFilterEmployee.Text = "Filter & Employee";
            gbFilterEmployee.Left = 10;
            gbFilterEmployee.Top = 200;
            gbFilterEmployee.Width = 1140;
            gbFilterEmployee.Height = 75;

            var lblFrom = new Label { Text = "From:", Left = 15, Top = 32, Width = 40 };
            dtpFrom = new DateTimePicker
            {
                Left = 60,
                Top = 27,
                Width = 135,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd"
            };

            var lblTo = new Label { Text = "To:", Left = 210, Top = 32, Width = 30 };
            dtpTo = new DateTimePicker
            {
                Left = 240,
                Top = 27,
                Width = 135,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd"
            };

            btnFilter = new Button { Text = "Filter", Left = 390, Top = 25, Width = 80 };
            btnFilter.Click += BtnFilter_Click;

            var lblEmployeeUserId = new Label { Text = "User ID:", Left = 500, Top = 32, Width = 60 };
            txtEmployeeUserId = new TextBox { Left = 560, Top = 27, Width = 110 };

            var lblEmployeeName = new Label { Text = "Name:", Left = 685, Top = 32, Width = 45 };
            txtEmployeeName = new TextBox { Left = 730, Top = 27, Width = 180 };

            btnSaveEmployee = new Button { Text = "Save", Left = 920, Top = 25, Width = 70 };
            btnSaveEmployee.Click += BtnSaveEmployee_Click;

            btnSearchUser = new Button { Text = "Search", Left = 1000, Top = 25, Width = 70 };
            btnSearchUser.Click += BtnSearchUser_Click;

            btnShowAll = new Button { Text = "All", Left = 1080, Top = 25, Width = 45 };
            btnShowAll.Click += BtnShowAll_Click;

            gbFilterEmployee.Controls.Add(lblFrom);
            gbFilterEmployee.Controls.Add(dtpFrom);
            gbFilterEmployee.Controls.Add(lblTo);
            gbFilterEmployee.Controls.Add(dtpTo);
            gbFilterEmployee.Controls.Add(btnFilter);
            gbFilterEmployee.Controls.Add(lblEmployeeUserId);
            gbFilterEmployee.Controls.Add(txtEmployeeUserId);
            gbFilterEmployee.Controls.Add(lblEmployeeName);
            gbFilterEmployee.Controls.Add(txtEmployeeName);
            gbFilterEmployee.Controls.Add(btnSaveEmployee);
            gbFilterEmployee.Controls.Add(btnSearchUser);
            gbFilterEmployee.Controls.Add(btnShowAll);

            mainPanel.Controls.Add(gbDevice);
            mainPanel.Controls.Add(gbDashboard);
            mainPanel.Controls.Add(gbActions);
            mainPanel.Controls.Add(gbFilterEmployee);

            // =========================
            // Grid
            // =========================
            dgvLogs = new DataGridView();
            dgvLogs.Dock = DockStyle.Fill;
            dgvLogs.ReadOnly = true;
            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.AllowUserToDeleteRows = false;
            dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLogs.CellClick += DgvLogs_CellClick;

            Controls.Add(dgvLogs);
            Controls.Add(mainPanel);
        }
        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            LoadLogsToGrid();

            lblStatus.Text = "Status: Showing all logs";
        }
        private void BtnSaveConfig_Click(object sender, EventArgs e)
        {
            var config = new AppConfig
            {
                Ip = txtIp.Text.Trim(),
                Port = txtPort.Text.Trim(),
                DeviceId = txtDeviceId.Text.Trim()
            };

            _configService.Save(config);

            MessageBox.Show("Config saved");
        }
        private void BtnTest_Click(object sender, EventArgs e)
        {
            int port;

            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Port không hợp lệ");
                return;
            }

            bool ok = _deviceClient.TestConnection(txtIp.Text, port);

            MessageBox.Show(ok ? "Kết nối TCP thành công" : "Không kết nối được TCP");
        }
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogsToGrid();

            lblStatus.Text = "Status: Data Refreshed";
            lblLastSync.Text = "Last Action: " +
                               DateTime.Now.ToString("HH:mm:ss");
        }
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                MessageBox.Show("Auto sync đang chạy rồi");
                return;
            }

            _cts = new CancellationTokenSource();

            Task.Run(() => AutoLoadRealLogsLoop(_cts.Token));

            lblStatus.Text = "Status: Auto loading real logs...";
        }
        private void BtnLoadRealLogs_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Status: Loading real SOYAL logs...";

                var logs = _soyalWebClient.GetEventLogs(
                    txtIp.Text.Trim(),
                    txtDeviceId.Text.Trim());

                int inserted = 0;

                foreach (var log in logs)
                {
                    if (_databaseService.InsertLog(log))
                        inserted++;
                }

                LoadLogsToGrid();

                lblStatus.Text = "Status: Auto sync OK";
                lblLastSync.Text = "Last Sync: " + DateTime.Now.ToString("HH:mm:ss");
                UpdateDashboard();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Status: Load real logs failed";
                AppLogger.Error("Load real logs failed", ex);
                MessageBox.Show("Không lấy được dữ liệu từ máy SOYAL.\nChi tiết đã ghi vào logs/app.log");
            }
        }
        private void BtnFilter_Click(object sender, EventArgs e)
        {
            DateTime from = dtpFrom.Value.Date;
            DateTime to = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);

            var logs = _databaseService.GetLogsByDate(from, to);

            dgvLogs.DataSource = null;
            dgvLogs.DataSource = logs;

            lblTotalLogs.Text = "Total Logs: " + logs.Count;
            lblStatus.Text = "Status: Filtered logs";
        }
        

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel file (*.xlsx)|*.xlsx";
                dialog.FileName = "attendance_logs.xlsx";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var logs = GetCurrentGridLogs();
                    _excelExportService.Export(dialog.FileName, logs);

                    MessageBox.Show("Xuất Excel thành công");
                    AppLogger.Info("Export Excel OK");
                }


            }
        }
        private void DgvLogs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgvLogs.Rows[e.RowIndex];

            if (row.Cells["UserId"].Value != null)
            {
                txtEmployeeUserId.Text = row.Cells["UserId"].Value.ToString();
            }

            if (row.Cells["UserName"].Value != null)
            {
                txtEmployeeName.Text = row.Cells["UserName"].Value.ToString();
            }
        }
        private void AutoLoadRealLogsLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var logs = _soyalWebClient.GetEventLogs(
                        txtIp.Text.Trim(),
                        txtDeviceId.Text.Trim());

                    bool hasNewLog = false;

                    foreach (var log in logs)
                    {
                        if (_databaseService.InsertLog(log))
                        {
                            hasNewLog = true;
                        }
                    }

                    if (hasNewLog)
                    {
                        BeginInvoke(new Action(LoadLogsToGrid));
                    }

                    BeginInvoke(new Action(() =>
                    {
                        lblStatus.Text = "Status: Auto sync OK";
                        lblLastSync.Text = "Last Sync: " + DateTime.Now.ToString("HH:mm:ss");
                        UpdateDashboard();
                    }));
                }
                catch (Exception ex)
                {
                    AppLogger.Error("Auto sync failed", ex);

                    BeginInvoke(new Action(() =>
                    {
                        lblStatus.Text = "Status: Auto sync error - " + ex.Message;
                    }));
                }

                Thread.Sleep(5000);
            }
        }
        private void UpdateDashboard()
        {
            int totalLogs = _databaseService.CountLogs();
            int totalEmployees = _databaseService.CountEmployees();

            lblTotalLogs.Text = "Total Logs: " + totalLogs;
            lblEmployees.Text = "Employees: " + totalEmployees;
            lblTodayLogs.Text = "Today Logs: " + _databaseService.CountTodayLogs();
            lblUniqueToday.Text = "Unique Today: " + _databaseService.CountUniqueEmployeesToday();
            lblInvalidToday.Text = "Invalid Today: " + _databaseService.CountInvalidCardsToday();
        }
       
        private void BtnSendRaw_Click(object sender, EventArgs e)
        {
            int port;

            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Port không hợp lệ");
                return;
            }

            try
            {
                string response = _rawTcpService.SendHexCommand(
                    txtIp.Text.Trim(),
                    port,
                    txtRawCommand.Text.Trim());

                txtRawResponse.Text = response;
            }
            catch (Exception ex)
            {
                txtRawResponse.Text = ex.ToString();
            }
        }
        private void BtnSearchUser_Click(object sender, EventArgs e)
        {
            string userId = txtEmployeeUserId.Text.Trim();

            if (string.IsNullOrWhiteSpace(userId))
            {
                MessageBox.Show("Vui lòng nhập User ID cần tìm");
                return;
            }

            var logs = _databaseService.SearchLogsByUserId(userId);

            dgvLogs.DataSource = null;
            dgvLogs.DataSource = logs;

            lblTotalLogs.Text = "Total Logs: " + logs.Count;
            lblStatus.Text = "Status: Search User " + userId;
        }
        private void BtnSaveEmployee_Click(object sender, EventArgs e)
        {
            string userId = txtEmployeeUserId.Text.Trim();
            string userName = txtEmployeeName.Text.Trim();

            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show("Vui lòng nhập User ID và tên nhân viên");
                return;
            }

            _databaseService.SaveEmployee(userId, userName);
            _databaseService.UpdateLogUserName(userId, userName);

            MessageBox.Show("Đã lưu nhân viên và cập nhật log");

            LoadLogsToGrid();
        }
        private List<AttendanceLog> GetCurrentGridLogs()
        {
            var logs = new List<AttendanceLog>();

            foreach (DataGridViewRow row in dgvLogs.Rows)
            {
                if (row.IsNewRow)
                    continue;

                logs.Add(new AttendanceLog
                {
                    UserName = row.Cells["UserName"].Value?.ToString() ?? "",
                    UserId = row.Cells["UserId"].Value?.ToString() ?? "",
                    EventTime = Convert.ToDateTime(row.Cells["EventTime"].Value),
                    EventType = row.Cells["EventType"].Value?.ToString() ?? "",
                    VerifyType = row.Cells["VerifyType"].Value?.ToString() ?? ""
                });
            }

            return logs;
        }
        private void PollingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    int port;

                    if (!int.TryParse(txtPort.Text, out port))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    var logs = _deviceClient.ReadLogs(
                        txtIp.Text,
                        port,
                        txtDeviceId.Text);

                    bool hasNewLog = false;

                    foreach (var log in logs)
                    {
                        if (_databaseService.InsertLog(log))
                        {
                            hasNewLog = true;
                        }
                    }

                    if (hasNewLog)
                    {
                        BeginInvoke(new Action(LoadLogsToGrid));
                    }
                }
                catch
                {
                    // Tạm thời bỏ qua lỗi để app không bị tắt
                }

                Thread.Sleep(1000);
            }

        }
        //private void BtnManageEmployees_Click(object sender, EventArgs e)
        //{
        //    using (var form = new EmployeeForm(_databaseService))
        //    {
        //        form.ShowDialog();
        //    }

        //    LoadLogsToGrid();
        //}
        private void LoadLogsToGrid()
        {
            var logs = _databaseService.GetAllLogs();

            dgvLogs.DataSource = null;
            dgvLogs.DataSource = logs;


            // Ẩn cột kỹ thuật
            if (dgvLogs.Columns["Id"] != null)
                dgvLogs.Columns["Id"].Visible = false;

            if (dgvLogs.Columns["DeviceId"] != null)
                dgvLogs.Columns["DeviceId"].Visible = false;

            if (dgvLogs.Columns["Source"] != null)
                dgvLogs.Columns["Source"].Visible = false;

            if (dgvLogs.Columns["VerifyType"] != null)
                dgvLogs.Columns["VerifyType"].Visible = false;

            if (dgvLogs.Columns["LogIndex"] != null)
                dgvLogs.Columns["LogIndex"].Visible = false;

            // Đổi tên cột
            dgvLogs.Columns["UserName"].HeaderText = "Employee Name";
            dgvLogs.Columns["UserId"].HeaderText = "Fingerprint ID";
            dgvLogs.Columns["EventTime"].HeaderText = "Time";
            dgvLogs.Columns["EventType"].HeaderText = "Event";

            // Sắp thứ tự cột - đặt từ phải sang trái
            dgvLogs.Columns["EventType"].DisplayIndex = 3;
            dgvLogs.Columns["EventTime"].DisplayIndex = 2;
            dgvLogs.Columns["UserId"].DisplayIndex = 1;
            dgvLogs.Columns["UserName"].DisplayIndex = 0;

            UpdateDashboard();
        }
    }
}