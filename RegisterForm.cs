using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using AIOSICT011.Business;

namespace AIOSICT011.Forms
{
    // Registers a Customer or a Service Provider.
    // The extra provider fields are shown only when "Service Provider" is chosen.
    public class RegisterForm : Form
    {
        private RadioButton rbCustomer;
        private RadioButton rbProvider;
        private TextBox txtName, txtPhone, txtAddress, txtEmail, txtReg, txtRate, txtUser, txtPass, txtConfirm;
        private ComboBox cboRegion, cboService;
        private Label lblEmail, lblReg, lblService, lblRate;

        public RegisterForm(bool adminMode)
        {
            if (adminMode)
                UI.StyleForm(this, "AIOS ICT011 - Register a New User", 470, 560);
            else
                UI.StyleForm(this, "AIOS ICT011 - Register", 470, 560);

            Label lblTitle = new Label();
            lblTitle.Text = "Registration";
            lblTitle.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblTitle.ForeColor = UI.Navy;
            lblTitle.AutoSize = true;
            lblTitle.Left = 20;
            lblTitle.Top = 10;

            rbCustomer = new RadioButton();
            rbCustomer.Text = "Customer";
            rbCustomer.Left = 20;
            rbCustomer.Top = 55;
            rbCustomer.AutoSize = true;
            rbCustomer.Checked = true;
            rbCustomer.CheckedChanged += rbCustomer_CheckedChanged;

            rbProvider = new RadioButton();
            rbProvider.Text = "Service Provider";
            rbProvider.Left = 140;
            rbProvider.Top = 55;
            rbProvider.AutoSize = true;

            Label lblName = UI.Lbl("Full name", 20, 95);
            txtName = UI.Txt(170, 95, 260);
            Label lblPhone = UI.Lbl("Phone", 20, 133);
            txtPhone = UI.Txt(170, 133, 260);
            Label lblAddress = UI.Lbl("Address", 20, 171);
            txtAddress = UI.Txt(170, 171, 260);
            Label lblRegion = UI.Lbl("Region", 20, 209);
            cboRegion = UI.Combo(170, 209, 260, ServiceManager.GetRegions(false), "RegionName", "RegionID");

            // customer only
            lblEmail = UI.Lbl("Email", 20, 247);
            txtEmail = UI.Txt(170, 247, 260);

            // provider only
            lblReg = UI.Lbl("Business reg. no.", 20, 247);
            txtReg = UI.Txt(170, 247, 260);
            lblService = UI.Lbl("Service type", 20, 285);
            cboService = UI.Combo(170, 285, 260, ServiceManager.GetServiceTypes(false), "TypeName", "ServiceTypeID");
            lblRate = UI.Lbl("Hourly rate (P)", 20, 323);
            txtRate = UI.Txt(170, 323, 120);

            Label lblUser = UI.Lbl("Username", 20, 375);
            txtUser = UI.Txt(170, 375, 260);
            Label lblPass = UI.Lbl("Password", 20, 413);
            txtPass = UI.Txt(170, 413, 260);
            txtPass.UseSystemPasswordChar = true;
            Label lblConfirm = UI.Lbl("Confirm password", 20, 451);
            txtConfirm = UI.Txt(170, 451, 260);
            txtConfirm.UseSystemPasswordChar = true;

            Button btnRegister = UI.Btn("Register", 170, 500, 120, UI.Teal);
            Button btnClose = UI.Btn("Close", 300, 500, 130, UI.Navy);
            btnRegister.Click += btnRegister_Click;
            btnClose.Click += btnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(rbCustomer);
            Controls.Add(rbProvider);
            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblPhone);
            Controls.Add(txtPhone);
            Controls.Add(lblAddress);
            Controls.Add(txtAddress);
            Controls.Add(lblRegion);
            Controls.Add(cboRegion);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblReg);
            Controls.Add(txtReg);
            Controls.Add(lblService);
            Controls.Add(cboService);
            Controls.Add(lblRate);
            Controls.Add(txtRate);
            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(btnRegister);
            Controls.Add(btnClose);

            ShowFieldsForType();
        }

        private void rbCustomer_CheckedChanged(object sender, EventArgs e)
        {
            ShowFieldsForType();
        }

        // Shows the fields that belong to the chosen type of user
        private void ShowFieldsForType()
        {
            bool isCustomer = rbCustomer.Checked;

            lblEmail.Visible = isCustomer;
            txtEmail.Visible = isCustomer;

            lblReg.Visible = !isCustomer;
            txtReg.Visible = !isCustomer;
            lblService.Visible = !isCustomer;
            cboService.Visible = !isCustomer;
            lblRate.Visible = !isCustomer;
            txtRate.Visible = !isCustomer;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbCustomer.Checked)
                {
                    string membershipId = ServiceManager.RegisterCustomer(
                        txtName.Text, txtPhone.Text, txtAddress.Text, txtEmail.Text,
                        UI.ComboValue(cboRegion), txtUser.Text, txtPass.Text, txtConfirm.Text);

                    UI.Info("Customer registered. Your membership ID is " + membershipId);
                }
                else
                {
                    decimal rate = 0;
                    decimal.TryParse(txtRate.Text, out rate);   // stays 0 when it is not a number

                    ServiceManager.RegisterProvider(
                        txtName.Text, txtPhone.Text, txtAddress.Text, UI.ComboValue(cboRegion),
                        txtReg.Text, UI.ComboValue(cboService), rate,
                        txtUser.Text, txtPass.Text, txtConfirm.Text);

                    UI.Info("Service provider registered.");
                }
                ClearFields();
            }
            catch (SqlException ex)
            {
                UI.Warn("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                UI.Warn(ex.Message);   // validation messages
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtReg.Clear();
            txtRate.Clear();
            txtUser.Clear();
            txtPass.Clear();
            txtConfirm.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
