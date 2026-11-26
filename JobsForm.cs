using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using AIOSICT011.Business;
using AIOSICT011.Models;

namespace AIOSICT011.Forms
{
    // The service workflow: Requested -> In Progress -> Completed -> Paid -> Closed
    // Provider: accept and complete. Customer: pay AIOS and rate. Admin: pay the provider.
    public class JobsForm : Form
    {
        private DataGridView grid;

        public JobsForm()
        {
            if (CurrentSession.IsAdmin)
                UI.StyleForm(this, "AIOS ICT011 - All Jobs", 1000, 520);
            else
                UI.StyleForm(this, "AIOS ICT011 - My Jobs", 1000, 520);

            grid = UI.Grid(15, 15, 970, 390);

            // provider buttons
            Button btnAccept = UI.Btn("Accept Job", 15, 425, 150, UI.Green);
            Button btnComplete = UI.Btn("Complete and Invoice", 175, 425, 200, UI.Teal);
            // customer buttons
            Button btnPay = UI.Btn("Pay Invoice (to AIOS)", 15, 425, 210, UI.Green);
            Button btnRate = UI.Btn("Rate / Report Service", 235, 425, 210, UI.Orange);
            // admin button
            Button btnPayProvider = UI.Btn("Pay Provider", 15, 425, 170, UI.Green);
            Button btnClose = UI.Btn("Close", 885, 425, 100, UI.Navy);

            btnAccept.Visible = CurrentSession.IsProvider;
            btnComplete.Visible = CurrentSession.IsProvider;
            btnPay.Visible = CurrentSession.IsCustomer;
            btnRate.Visible = CurrentSession.IsCustomer;
            btnPayProvider.Visible = CurrentSession.IsAdmin;

            btnAccept.Click += btnAccept_Click;
            btnComplete.Click += btnComplete_Click;
            btnPay.Click += btnPay_Click;
            btnRate.Click += btnRate_Click;
            btnPayProvider.Click += btnPayProvider_Click;
            btnClose.Click += btnClose_Click;

            Controls.Add(grid);
            Controls.Add(btnAccept);
            Controls.Add(btnComplete);
            Controls.Add(btnPay);
            Controls.Add(btnRate);
            Controls.Add(btnPayProvider);
            Controls.Add(btnClose);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                grid.DataSource = ServiceManager.GetJobsForCurrentUser();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
        }

        // Returns the selected job number, or 0 when nothing is selected
        private int GetSelectedJobId()
        {
            int jobId = UI.SelectedId(grid, "TransactionID");
            if (jobId == 0)
                UI.Warn("Select a job first.");
            return jobId;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            int jobId = GetSelectedJobId();
            if (jobId == 0) return;

            try
            {
                ServiceManager.AcceptJob(jobId);
                UI.Info("Job accepted. You are now marked as busy.");
                LoadData();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                UI.Warn(ex.Message);
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            int jobId = GetSelectedJobId();
            if (jobId == 0) return;

            string input = Dialogs.Prompt("Hours worked (for example 2.5):", "Complete job");
            if (input == null) return;   // cancelled

            decimal hours;
            if (!decimal.TryParse(input, out hours))
            {
                UI.Warn("Enter the hours as a number.");
                return;
            }

            try
            {
                decimal total = ServiceManager.CompleteJob(jobId, hours);
                UI.Info("Job completed. Invoice total: P" + total.ToString("0.00"));
                LoadData();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                UI.Warn(ex.Message);
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            int jobId = GetSelectedJobId();
            if (jobId == 0) return;

            try
            {
                ServiceManager.PayJob(jobId);
                UI.Info("Payment received by AIOS.");
                LoadData();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                UI.Warn(ex.Message);
            }
        }

        private void btnRate_Click(object sender, EventArgs e)
        {
            int jobId = GetSelectedJobId();
            if (jobId == 0) return;

            int rating;
            string feedback;
            bool redFlag;
            if (!Dialogs.Rating(out rating, out feedback, out redFlag)) return;   // cancelled

            try
            {
                bool blacklisted = ServiceManager.SubmitFeedback(jobId, rating, feedback, redFlag);
                if (blacklisted)
                    UI.Info("Feedback saved. This provider has been blacklisted.");
                else
                    UI.Info("Feedback saved.");
                LoadData();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                UI.Warn(ex.Message);
            }
        }

        private void btnPayProvider_Click(object sender, EventArgs e)
        {
            int jobId = GetSelectedJobId();
            if (jobId == 0) return;

            try
            {
                ServiceManager.PayProvider(jobId);
                UI.Info("Provider has been paid by AIOS.");
                LoadData();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                UI.Warn(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
