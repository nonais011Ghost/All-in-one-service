using System;
using System.Drawing;
using System.Windows.Forms;
using AIOSICT011.Models;

namespace AIOSICT011.Forms
{
    // Main menu. Buttons that a role may not use are disabled (role-based access).
    public class MainForm : Form
    {
        public MainForm()
        {
            UI.StyleForm(this, "AIOS ICT011 - Main Menu", 440, 470);
            User user = CurrentSession.LoggedInUser;

            Label lblTitle = new Label();
            lblTitle.Text = "All-In-One Service";
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.ForeColor = UI.Navy;
            lblTitle.AutoSize = true;
            lblTitle.Left = 20;
            lblTitle.Top = 15;

            Label lblWelcome = UI.Lbl("Welcome, " + user.Username + "  (" + user.Role + ")", 22, 55);

            Button btnCatalog = UI.Btn("Service Provider Catalogue", 70, 100, 300, UI.Teal);
            Button btnJobs = UI.Btn("My Jobs", 70, 150, 300, UI.Teal);
            if (CurrentSession.IsAdmin)
                btnJobs.Text = "All Jobs";

            Button btnCustomers = UI.Btn("Manage Customers", 70, 200, 300, UI.Orange);
            Button btnReports = UI.Btn("Reports and Statistics", 70, 250, 300, UI.Orange);
            Button btnRegister = UI.Btn("Register a New User", 70, 300, 300, UI.Orange);
            Button btnLogout = UI.Btn("Logout", 70, 370, 300, UI.Navy);

            // only the admin may use these three
            btnCustomers.Enabled = CurrentSession.IsAdmin;
            btnReports.Enabled = CurrentSession.IsAdmin;
            btnRegister.Enabled = CurrentSession.IsAdmin;

            btnCatalog.Click += btnCatalog_Click;
            btnJobs.Click += btnJobs_Click;
            btnCustomers.Click += btnCustomers_Click;
            btnReports.Click += btnReports_Click;
            btnRegister.Click += btnRegister_Click;
            btnLogout.Click += btnLogout_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblWelcome);
            Controls.Add(btnCatalog);
            Controls.Add(btnJobs);
            Controls.Add(btnCustomers);
            Controls.Add(btnReports);
            Controls.Add(btnRegister);
            Controls.Add(btnLogout);
        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {
            using (ProviderCatalogForm form = new ProviderCatalogForm())
            {
                form.ShowDialog();
            }
        }

        private void btnJobs_Click(object sender, EventArgs e)
        {
            using (JobsForm form = new JobsForm())
            {
                form.ShowDialog();
            }
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            using (CustomersForm form = new CustomersForm())
            {
                form.ShowDialog();
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            using (ReportsForm form = new ReportsForm())
            {
                form.ShowDialog();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm form = new RegisterForm(true))
            {
                form.ShowDialog();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            CurrentSession.LoggedOut = true;   // Program.cs shows the login screen again
            Close();
        }
    }
}
