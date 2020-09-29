namespace TODODesktopUtility
{
    using System;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using ToDoHelperLibrary;

    /// <summary>
    /// Class represents Form to add new user
    /// </summary>
    public partial class FormAddUser : Form
    {

        public static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        public FormAddUser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// click event handler for register button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            DbCommunicator db = DbCommunicator.CreateInstance(_connectionString);
            try
            {
                log.Info("Clicked on Register button on New User form.");
                //Validate Field on Add user form.
                bool isValid = FieldsValidator();
                if (true == isValid)
                {
                    db.OpenConnection();
                    string query = String.Format(DBQuery.INSERT_USER, txtNewUsername.Text, txtNewPassword.Text, txtEmail.Text);
                    int numberofRows = db.ExecuteNonQuery(query);
                    if (0 < numberofRows)
                    {
                        log.Info("New user added successfully.");
                        MessageBox.Show(txtNewUsername.Text + " user added successfully.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        log.Error("Failed to add new user");
                        MessageBox.Show("Failed to insert new user", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Someting went wrong...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error("Exception message is : " + ex.Message);
            }
            finally {
                if (null != db)
                {
                    db.CloseConnection();
                }
            }
        }

        /// <summary>
        /// Validate form fields
        /// </summary>
        /// <returns></returns>
        private bool FieldsValidator()
        {
            log.Info("Validating all fields on the New User form.");
            bool isValid = true;
            bool isEmail = Regex.IsMatch(txtEmail.Text, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase);
            if (String.IsNullOrWhiteSpace(txtNewUsername.Text)
                || String.IsNullOrWhiteSpace(txtNewPassword.Text)
                || String.IsNullOrWhiteSpace(txtConfirmPassword.Text)
                || String.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("All Fields are mandatory",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            else if ((txtNewUsername.Text.Length < 4 || txtNewUsername.Text.Length > 30))
            {
                MessageBox.Show("Username must be a minimum of 4 characters and maximum of 30 characters",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            else if ((txtNewPassword.Text.Length < 4 || txtNewPassword.Text.Length > 10))
            {
                MessageBox.Show("Password must be a minimum of 4 characters and maximum of 10 characters",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNewPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;
                isValid = false;
            }
            else if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Confirm Password is not same.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Text = string.Empty;
                isValid = false;
            }
            else if (!isEmail)
            {
                MessageBox.Show("Please enter valid email",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }

        /// <summary>
        /// Click event hanlder for Cancle button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
