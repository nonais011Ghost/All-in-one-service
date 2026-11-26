using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using AIOSICT011.Business;
using AIOSICT011.Models;

namespace AIOSICT011.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private int attempts = 0;   // counts wrong passwords

        public LoginForm()
        {
            UI.StyleForm(this, "AIOS ICT011 - Login", 400, 330);

            Label lblTitle = new Label();
            lblTitle.Text = "All-In-One Service";
            lblTitle.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
            lblTitle.ForeColor = UI.Navy;
            lblTitle.AutoSize = true;
            lblTitle.Left = 70;
            lblTitle.Top = 25;

            Label lblSub = UI.Lbl("Sign in to continue", 135, 70);
            Label lblUser = UI.Lbl("Username", 40, 120);
            txtUser = UI.Txt(130, 120, 220);
            Label lblPass = UI.Lbl("Password", 40, 165);
            txtPass = UI.Txt(130, 165, 220);
            txtPass.UseSystemPasswordChar = true;

            Button btnLogin = UI.Btn("Login", 130, 215, 100, UI.Teal);
            Button btnRegister = UI.Btn("Register", 245, 215, 105, UI.Orange);
            Button btnExit = UI.Btn("Exit", 130, 265, 220, UI.Navy);

            btnLogin.Click += btnLogin_Click;
            btnRegister.Click += btnRegister_Click;
            btnExit.Click += btnExit_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblSub);
            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
            Controls.Add(btnExit);

            AcceptButton = btnLogin;   // pressing Enter clicks Login
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                User user = ServiceManager.Login(txtUser.Text, txtPass.Text);

                if (user == null)
                {
                    attempts++;
                    if (attempts >= 3)
                    {
                        UI.Warn("Too many failed attempts. The program will close.");
                        DialogResult = DialogResult.Cancel;
                        Close();
                        return;
                    }
                    UI.Warn("Invalid username or password. Attempts left: " + (3 - attempts));
                    txtPass.Clear();
                    txtPass.Focus();
                    return;
                }

                CurrentSession.LoggedInUser = user;
                DialogResult = DialogResult.OK;
                Close();
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm form = new RegisterForm(false))
            {
                form.ShowDialog();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
