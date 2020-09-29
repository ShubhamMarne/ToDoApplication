namespace TODODesktopUtility
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;
    using ToDoHelperLibrary;

    //Delegate for LoginSuccess
    public delegate void LoginSuccess();

    /// <summary>
    /// Base Login Form
    /// </summary>
    public partial class FormLogin : Form
    {
        #region Class Members

        //Event for successful login
        public event LoginSuccess LoginSuccessEvent;
        public static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize instance of <see cref="FormLogin">
        /// </summary>
        public FormLogin()
        {
            InitializeComponent();
            // Subscribe method for LoginSuccess event.
            LoginSuccessEvent += new LoginSuccess(OnLoginSuccess);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Event hanlder for login button to check authentication for user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BtnLogin_Click(object sender, EventArgs e)
        {
            log.Info("Clicked on login button on login form.");
            log.Info("Validating inputs.");
            bool isValid = ValidateFields();
            if (true == isValid)
            {
                //Send login request
                log.Info("Authenticating user");
                Login();
            }
            else
            {
                //Do nothing
            }
        }
        
        /// <summary>
        /// Check login status
        /// </summary>
        public void Login() {
            DbCommunicator db = DbCommunicator.CreateInstance(_connectionString);
            try
            {
                log.Info("Checking login for user.");
                db.OpenConnection();
                string query = String.Format(DBQuery.GET_USER_COUNT, txtUserName.Text, txtPassword.Text);
                int numberOfRows = db.ExecuteScalar(query);
                if (0 < numberOfRows)
                {
                    log.Info("Successful login for user" + txtUserName.Text);
                    // On login success open form main.
                    LoginSuccessEvent();
                }
                else
                {
                    log.Error("Login failed for " + txtUserName.Text);
                    MessageBox.Show("Invalid credentails. \nPlease check username/password.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error("Exception message is : " + ex.Message);
                Application.Exit();
            }
            finally {
                if (null != db)
                {
                    db.CloseConnection();
                }
            }     
        }

        /// <summary>
        /// Event handler for cancel button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BtnCancel_Click(object sender, EventArgs e)
        {
            log.Info("Login Form Cancel button clicked.");
            Application.Exit();
        }

        /// <summary>
        /// Validate input fields on the login form.
        /// </summary>
        private bool ValidateFields()
        {
            bool isValid = true;
            if (String.IsNullOrEmpty(txtUserName.Text)
                || String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("All Fields are mandatory",
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
        /// Reset Username and Password Fields
        /// </summary>
        public void ResetControls()
        {
            txtPassword.Text = txtUserName.Text = string.Empty;
            this.ActiveControl = txtUserName;
        }

        /// <summary>
        /// Close application once form gets closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Close Application
            log.Info("Close Application.");
            Application.Exit();
        }

        /// <summary>
        /// Subscriber method for Login success.
        /// </summary>
        public void OnLoginSuccess() {
            // Hide login form
            this.Hide();
            // Show Main Form
            FormMain form = new FormMain(txtUserName.Text, this);
            form.Show();
        }

        /// <summary>
        /// Click event handler for New User button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewUser_Click(object sender, EventArgs e)
        {
            FormAddUser addUser = new FormAddUser();
            addUser.Show();
        }
        
        #endregion
    }
}
