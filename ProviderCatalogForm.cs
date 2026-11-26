using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using AIOSICT011.Business;
using AIOSICT011.Models;

namespace AIOSICT011.Forms
{
    // Catalogue of service providers: search and filter by service type, region and availability.
    // Customers can request the selected provider. The admin can change availability.
    public class ProviderCatalogForm : Form
    {
        private TextBox txtSearch;
        private ComboBox cboService;
        private ComboBox cboRegion;
        private CheckBox chkAvailable;
        private DataGridView grid;

        public ProviderCatalogForm()
        {
            UI.StyleForm(this, "AIOS ICT011 - Service Provider Catalogue", 920, 540);

            Label lblSearch = UI.Lbl("Search:", 15, 15);
            txtSearch = UI.Txt(70, 15, 140);
            Label lblService = UI.Lbl("Service:", 225, 15);
            cboService = UI.Combo(285, 15, 130, ServiceManager.GetServiceTypes(true), "TypeName", "ServiceTypeID");
            Label lblRegion = UI.Lbl("Region:", 430, 15);
            cboRegion = UI.Combo(485, 15, 190, ServiceManager.GetRegions(true), "RegionName", "RegionID");

            chkAvailable = new CheckBox();
            chkAvailable.Text = "Available only";
            chkAvailable.Left = 690;
            chkAvailable.Top = 18;
            chkAvailable.AutoSize = true;

            Button btnSearch = UI.Btn("Search", 810, 11, 95, UI.Teal);
            btnSearch.Click += btnSearch_Click;

            grid = UI.Grid(15, 60, 890, 370);
            grid.CellFormatting += grid_CellFormatting;

            Button btnRequest = UI.Btn("Request Selected Provider", 15, 450, 230, UI.Green);
            Button btnToggle = UI.Btn("Toggle Availability", 15, 450, 170, UI.Orange);
            Button btnClose = UI.Btn("Close", 805, 450, 100, UI.Navy);
            btnRequest.Click += btnRequest_Click;
            btnToggle.Click += btnToggle_Click;
            btnClose.Click += btnClose_Click;

            // only customers can request, only the admin can change availability
            btnRequest.Visible = CurrentSession.IsCustomer;
            btnToggle.Visible = CurrentSession.IsAdmin;

            Controls.Add(lblSearch);
            Controls.Add(txtSearch);
            Controls.Add(lblService);
            Controls.Add(cboService);
            Controls.Add(lblRegion);
            Controls.Add(cboRegion);
            Controls.Add(chkAvailable);
            Controls.Add(btnSearch);
            Controls.Add(grid);
            Controls.Add(btnRequest);
            Controls.Add(btnToggle);
            Controls.Add(btnClose);

            AcceptButton = btnSearch;
            LoadData();
        }

        // Fills the grid using the search box and the filters
        private void LoadData()
        {
            try
            {
                grid.DataSource = ServiceManager.SearchProviders(txtSearch.Text.Trim(),
                    UI.ComboValue(cboService), UI.ComboValue(cboRegion), chkAvailable.Checked);
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // Colours the Status column
        private void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;
            if (grid.Columns[e.ColumnIndex].Name != "Status") return;

            string status = e.Value.ToString();
            if (status == "Blacklisted")
                e.CellStyle.ForeColor = Color.Red;
            else if (status == "Available")
                e.CellStyle.ForeColor = Color.Green;
            else
                e.CellStyle.ForeColor = Color.DarkOrange;
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            int providerId = UI.SelectedId(grid, "ProviderID");
            if (providerId == 0)
            {
                UI.Warn("Select a provider first.");
                return;
            }

            try
            {
                int jobId = ServiceManager.RequestJob(CurrentSession.LoggedInUser.PersonID, providerId);
                UI.Info("Service requested. Your job number is " + jobId);
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

        private void btnToggle_Click(object sender, EventArgs e)
        {
            int providerId = UI.SelectedId(grid, "ProviderID");
            if (providerId == 0)
            {
                UI.Warn("Select a provider first.");
                return;
            }

            try
            {
                ServiceManager.ToggleAvailability(providerId);
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
