using System;
using System.Data;
using System.Windows.Forms;

namespace AIOSICT011.Forms
{
    // Reusable pop-up that just shows a table (for example a customer's job history)
    public class GridViewerForm : Form
    {
        public GridViewerForm(string title, DataTable data)
        {
            UI.StyleForm(this, title, 900, 440);

            DataGridView grid = UI.Grid(15, 15, 870, 360);
            grid.DataSource = data;

            Button btnClose = UI.Btn("Close", 785, 390, 100, UI.Navy);
            btnClose.Click += btnClose_Click;

            Controls.Add(grid);
            Controls.Add(btnClose);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
