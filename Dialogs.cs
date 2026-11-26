using System.Windows.Forms;

namespace AIOSICT011.Forms
{
    // Two small pop-up windows: ask for one value, and ask for a rating.
    public static class Dialogs
    {
        public static string Prompt(string text, string title)
        {
            using (Form f = new Form())
            {
                UI.StyleForm(f, title, 340, 150);
                Label l = UI.Lbl(text, 15, 15);
                TextBox t = UI.Txt(15, 45, 295);
                Button ok = UI.Btn("OK", 130, 90, 85, UI.Teal);
                Button cancel = UI.Btn("Cancel", 225, 90, 85, UI.Red);
                ok.DialogResult = DialogResult.OK;
                cancel.DialogResult = DialogResult.Cancel;
                f.Controls.AddRange(new Control[] { l, t, ok, cancel });
                f.AcceptButton = ok;
                f.CancelButton = cancel;
                if (f.ShowDialog() == DialogResult.OK) return t.Text;
                return null;
            }
        }

        public static bool Rating(out int rating, out string feedback, out bool redFlag)
        {
            rating = 0;
            feedback = "";
            redFlag = false;

            using (Form f = new Form())
            {
                UI.StyleForm(f, "Rate this service", 360, 270);
                Label l1 = UI.Lbl("Rating (1-5):", 15, 20);
                NumericUpDown n = new NumericUpDown();
                n.Left = 130;
                n.Top = 17;
                n.Width = 60;
                n.Minimum = 1;
                n.Maximum = 5;
                n.Value = 5;
                Label l2 = UI.Lbl("Feedback:", 15, 60);
                TextBox t = UI.Txt(15, 88, 325);
                t.Multiline = true;
                t.Height = 65;
                t.MaxLength = 300;
                CheckBox c = new CheckBox();
                c.Text = "Report poor service (red flag)";
                c.Left = 15;
                c.Top = 165;
                c.AutoSize = true;
                Button ok = UI.Btn("Save", 150, 210, 90, UI.Teal);
                Button cancel = UI.Btn("Cancel", 250, 210, 90, UI.Red);
                ok.DialogResult = DialogResult.OK;
                cancel.DialogResult = DialogResult.Cancel;
                f.Controls.AddRange(new Control[] { l1, n, l2, t, c, ok, cancel });
                f.AcceptButton = ok;
                f.CancelButton = cancel;

                if (f.ShowDialog() != DialogResult.OK) return false;
                rating = (int)n.Value;
                feedback = t.Text.Trim();
                redFlag = c.Checked;
                return true;
            }
        }
    }
}
