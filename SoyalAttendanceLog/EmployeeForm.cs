using System;
using System.Windows.Forms;

namespace SoyalAttendanceLog
{
    public partial class EmployeeForm : Form
    {
        private readonly DatabaseService _databaseService;

        private TextBox txtUserId;
        private TextBox txtUserName;
        private Button btnSave;

        public EmployeeForm(DatabaseService databaseService)
        {
            InitializeComponent();

            _databaseService = databaseService;

            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Manage Employees";
            Width = 420;
            Height = 220;
            StartPosition = FormStartPosition.CenterParent;

            var lblUserId = new Label
            {
                Text = "Fingerprint ID:",
                Left = 20,
                Top = 30,
                Width = 100
            };

            txtUserId = new TextBox
            {
                Left = 130,
                Top = 25,
                Width = 220
            };

            var lblUserName = new Label
            {
                Text = "Employee Name:",
                Left = 20,
                Top = 70,
                Width = 100
            };

            txtUserName = new TextBox
            {
                Left = 130,
                Top = 65,
                Width = 220
            };

            btnSave = new Button
            {
                Text = "Save Employee",
                Left = 130,
                Top = 110,
                Width = 140
            };
            btnSave.Click += BtnSave_Click;

            Controls.Add(lblUserId);
            Controls.Add(txtUserId);
            Controls.Add(lblUserName);
            Controls.Add(txtUserName);
            Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string userId = txtUserId.Text.Trim();
            string userName = txtUserName.Text.Trim();

            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show("Please enter Fingerprint ID and Employee Name");
                return;
            }

            _databaseService.SaveEmployee(userId, userName);
            _databaseService.UpdateLogUserName(userId, userName);

            MessageBox.Show("Employee saved");

            Close();
        }
    }
}