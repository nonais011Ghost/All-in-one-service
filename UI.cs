using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AIOSICT011.Forms
{
    // Small helpers so every form looks the same and the form code stays short.
    public static class UI
    {
        public static readonly Color Navy = Color.FromArgb(11, 60, 93);
        public static readonly Color Teal = Color.FromArgb(29, 154, 166);
        public static readonly Color Orange = Color.FromArgb(242, 140, 40);
        public static readonly Color Green = Color.FromArgb(46, 139, 87);
        public static readonly Color Red = Color.FromArgb(200, 50, 40);
        public static readonly Color Light = Color.FromArgb(232, 244, 248);

        public static void StyleForm(Form f, string title, int width, int height)
        {
            f.Text = title;
            f.ClientSize = new Size(width, height);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.BackColor = Light;
            f.FormBorderStyle = FormBorderStyle.FixedSingle;
            f.MaximizeBox = false;
            f.Font = new Font("Segoe UI", 9.5f);
        }

        public static Label Lbl(string text, int x, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.Left = x;
            l.Top = y + 3;
            l.AutoSize = true;
            return l;
        }

        public static TextBox Txt(int x, int y, int width)
        {
            TextBox t = new TextBox();
            t.Left = x;
            t.Top = y;
            t.Width = width;
            return t;
        }

        public static Button Btn(string text, int x, int y, int width, Color back)
        {
            Button b = new Button();
            b.Text = text;
            b.Left = x;
            b.Top = y;
            b.Width = width;
            b.Height = 34;
            b.BackColor = back;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            return b;
        }

        public static ComboBox Combo(int x, int y, int width, DataTable data, string displayMember, string valueMember)
        {
            ComboBox c = new ComboBox();
            c.Left = x;
            c.Top = y;
            c.Width = width;
            c.DropDownStyle = ComboBoxStyle.DropDownList;
            c.DisplayMember = displayMember;
            c.ValueMember = valueMember;
            c.DataSource = data;
            return c;
        }

        public static DataGridView Grid(int x, int y, int width, int height)
        {
            DataGridView g = new DataGridView();
            g.Left = x;
            g.Top = y;
            g.Width = width;
            g.Height = height;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.BackgroundColor = Color.White;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Navy;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Light;
            return g;
        }

        // Selected value of a combo box that is bound to a DataTable (0 if nothing selected).
        public static int ComboValue(ComboBox c)
        {
            if (c.SelectedValue == null) return 0;
            return Convert.ToInt32(c.SelectedValue);
        }

        // Value of a column in the highlighted grid row (0 if no row is selected).
        public static int SelectedId(DataGridView g, string column)
        {
            if (g.CurrentRow == null) return 0;
            return Convert.ToInt32(g.CurrentRow.Cells[column].Value);
        }

        public static void Warn(string message)
        {
            MessageBox.Show(message, "AIOS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Info(string message)
        {
            MessageBox.Show(message, "AIOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
