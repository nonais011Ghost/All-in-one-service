using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using AIOSICT011.Business;

namespace AIOSICT011.Forms
{
    // Reports with filter and sort. The report is loaded into a DataTable and shown through its DataView,
    // so the filter text and the sort column can change what the grid shows (Lesson 11).
    public class ReportsForm : Form
    {
        private ComboBox cboReport, cboColumn, cboSort;
        private TextBox txtFilter;
        private CheckBox chkDesc;
        private DataGridView grid;
        private DataView view;

        public ReportsForm()
        {
            UI.StyleForm(this, "AIOS ICT011 - Reports and Statistics", 1000, 600);

            Label lblReport = UI.Lbl("Report:", 15, 15);
            cboReport = new ComboBox();
            cboReport.Left = 75;
            cboReport.Top = 15;
            cboReport.Width = 230;
            cboReport.DropDownStyle = ComboBoxStyle.DropDownList;
            cboReport.Items.AddRange(ServiceManager.ReportNames);
            cboReport.SelectedIndex = 0;

            Button btnShow = UI.Btn("Show", 320, 11, 90, UI.Teal);
            btnShow.Click += btnShow_Click;

            Label lblColumn = UI.Lbl("Filter column:", 15, 58);
            cboColumn = new ComboBox();
            cboColumn.Left = 115;
            cboColumn.Top = 58;
            cboColumn.Width = 150;
            cboColumn.DropDownStyle = ComboBoxStyle.DropDownList;
            cboColumn.SelectedIndexChanged += FilterOrSortChanged;

            Label lblContains = UI.Lbl("contains", 275, 58);
            txtFilter = UI.Txt(340, 58, 150);
            txtFilter.TextChanged += FilterOrSortChanged;

            Label lblSort = UI.Lbl("Sort by:", 510, 58);
            cboSort = new ComboBox();
            cboSort.Left = 570;
            cboSort.Top = 58;
            cboSort.Width = 150;
            cboSort.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSort.SelectedIndexChanged += FilterOrSortChanged;

            chkDesc = new CheckBox();
            chkDesc.Text = "Descending";
            chkDesc.Left = 735;
            chkDesc.Top = 61;
            chkDesc.AutoSize = true;
            chkDesc.CheckedChanged += FilterOrSortChanged;

            grid = UI.Grid(15, 105, 965, 430);
            Button btnClose = UI.Btn("Close", 880, 550, 100, UI.Navy);
            btnClose.Click += btnClose_Click;

            Controls.Add(lblReport);
            Controls.Add(cboReport);
            Controls.Add(btnShow);
            Controls.Add(lblColumn);
            Controls.Add(cboColumn);
            Controls.Add(lblContains);
            Controls.Add(txtFilter);
            Controls.Add(lblSort);
            Controls.Add(cboSort);
            Controls.Add(chkDesc);
            Controls.Add(grid);
            Controls.Add(btnClose);

            LoadReport();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        // Loads the chosen report and fills the two column lists
        private void LoadReport()
        {
            try
            {
                DataTable table = ServiceManager.GetReport(cboReport.Text);
                view = table.DefaultView;

                cboColumn.Items.Clear();
                cboSort.Items.Clear();
                foreach (DataColumn col in table.Columns)
                {
                    cboColumn.Items.Add(col.ColumnName);
                    cboSort.Items.Add(col.ColumnName);
                }

                txtFilter.Clear();
                chkDesc.Checked = false;
                grid.DataSource = view;
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

        private void FilterOrSortChanged(object sender, EventArgs e)
        {
            ApplyFilterAndSort();
        }

        // Filters the rows by the text and sorts them by the chosen column
        private void ApplyFilterAndSort()
        {
            if (view == null) return;

            try
            {
                string text = txtFilter.Text.Trim();
                text = text.Replace("'", "''");   // a quote in the text would break the filter

                if (text.Length == 0 || cboColumn.SelectedItem == null)
                {
                    view.RowFilter = "";
                }
                else
                {
                    view.RowFilter = "Convert([" + cboColumn.SelectedItem + "], 'System.String') LIKE '%" + text + "%'";
                }

                if (cboSort.SelectedItem != null)
                {
                    string direction = "ASC";
                    if (chkDesc.Checked) direction = "DESC";
                    view.Sort = "[" + cboSort.SelectedItem + "] " + direction;
                }
            }
            catch (Exception)
            {
                view.RowFilter = "";
                UI.Warn("That filter could not be applied. Try different text.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
