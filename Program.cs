using System;
using System.Windows.Forms;
using AIOSICT011.DataAccess;
using AIOSICT011.Forms;
using AIOSICT011.Models;

namespace AIOSICT011
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Build the database and tables the first time the program runs
            try
            {
                Db.EnsureDatabaseCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The database could not be prepared. Check that SQL Server LocalDB is installed, " +
                                "or change the connection strings in Db.cs." + Environment.NewLine + Environment.NewLine + ex.Message,
                                "AIOS ICT011", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Login -> main menu -> (logout) -> login again, until the user exits
            bool again = true;
            while (again)
            {
                CurrentSession.LoggedOut = false;
                CurrentSession.LoggedInUser = null;

                DialogResult result;
                using (LoginForm login = new LoginForm())
                {
                    result = login.ShowDialog();
                }
                if (result != DialogResult.OK) break;

                using (MainForm main = new MainForm())
                {
                    main.ShowDialog();
                }
                again = CurrentSession.LoggedOut;
            }
        }
    }
}
