using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using AIOSICT011.Business;

namespace AIOSICT011.Forms
{
    // Admin screen: list customers, update their details, view their job history.
    // Clicking a row copies it into the text boxes (same idea as the DataGridView lesson).
    public class CustomersForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtPhone, txtAddress, txtEmail;
        private int selectedId = 0;

        public CustomersForm()
        {
            UI.StyleForm(this, "AIOS ICT011 - Customers", 920, 520);

            grid = UI.Grid(15, 15, 890, 250);
            grid.CellClick += grid_CellClick;

            Label lblName = UI.Lbl("Full name", 15, 290);
            txtName = UI.Txt(110, 290, 280);
            Label lblPhone = UI.Lbl("Phone", 15, 330);
            txtPhone = UI.Txt(110, 330, 280);
            Label lblAddress = UI.Lbl("Address", 450, 290);
            txtAddress = UI.Txt(530, 290, 280);
            Label lblEmail = UI.Lbl("Email", 450, 330);
            txtEmail = UI.Txt(530, 330, 280);

            Button btnUpdate = UI.Btn("Update Customer", 15, 400, 170, UI.Teal);
            Button btnHistory = UI.Btn("View History", 195, 400, 150, UI.Orange);
            Button btnClose = UI.Btn("Close", 805, 400, 100, UI.Navy);
            btnUpdate.Click += btnUpdate_Click;
            btnHistory.Click += btnHistory_Click;
            btnClose.Click += btnClose_Click;

            Controls.Add(grid);
            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblPhone);
            Controls.Add(txtPhone);
            Controls.Add(lblAddress);
            Controls.Add(txtAddress);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(btnUpdate);
            Controls.Add(btnHistory);
            Controls.Add(btnClose);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                grid.DataSource = ServiceManager.GetCustomers();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
        }

        // Copies the clicked row into the text boxes
        private void grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = grid.Rows[e.RowIndex];
            selectedId = Convert.ToInt32(row.Cells["CustomerID"].Value);
            txtName.Text = Convert.ToString(row.Cells["FullName"].Value);
            txtPhone.Text = Convert.ToString(row.Cells["Phone"].Value);
            txtAddress.Text = Convert.ToString(row.Cells["Address"].Value);
            txtEmail.Text = Convert.ToString(row.Cells["Email"].Value);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceManager.UpdateCustomer(selectedId, txtName.Text, txtPhone.Text, txtAddress.Text, txtEmail.Text);
                UI.Info("Customer updated.");
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

        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                UI.Warn("Select a customer first.");
                return;
            }

            try
            {
                using (GridViewerForm form = new GridViewerForm("Job history", ServiceManager.GetCustomerHistory(selectedId)))
                {
                    form.ShowDialog();
                }
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
