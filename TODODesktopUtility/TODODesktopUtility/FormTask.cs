namespace TODODesktopUtility
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using ToDoHelperLibrary;

    // Delegate to Upgrade grid data.
    public delegate void UpdateGridData();

    /// <summary>
    /// Form responsible for New task.
    /// </summary>
    public partial class FormTask : Form
    {
        public static event UpdateGridData UpdateGridDataEvent;
        public static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        private int _taskId = 0;
        private string _name;

        /// <summary>
        /// Initialize instance for add new task
        /// </summary>
        /// <param name="name"></param>
        public FormTask(string name)
        {
            InitializeComponent();
            _name = name;
            // Settings for form for Adding task.
            this.Text = "Add Task";
            btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(AddTaskEventHanlder);
            FillStatus();
            clderStartTime.Format = DateTimePickerFormat.Custom;
            clderStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            cldrEndTime.Format = DateTimePickerFormat.Custom;
            cldrEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            clderStartTime.MinDate = DateTime.Now;
            cldrEndTime.MinDate = DateTime.Now;
        }

        /// <summary>
        /// Initialize instance for updating task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sendReminder"></param>
        /// <param name="notificationTime"></param>
        public FormTask(int id, string title, string desc, string status, 
            string startTime, string endTime, string sendReminder, string notificationTime)
        {
            InitializeComponent();
            // Settings for form for Updating task.
            this.Text = "Update Task";
            btnAdd.Text = "Update";
            clderStartTime.Format = DateTimePickerFormat.Custom;
            clderStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            cldrEndTime.Format = DateTimePickerFormat.Custom;
            cldrEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.btnAdd.Click += new System.EventHandler(UpdateTaskEventHanlder);
            FillStatus();
            _taskId = id;
            txtTitle.Text = title;
            txtItemDesc.Text = desc;
            clderStartTime.Text = DateTime.Parse(startTime).ToString();
            cldrEndTime.Text = DateTime.Parse(endTime).ToString();
            if ("1" == sendReminder) {
                radioYes.Checked = true;
            } else {
                radioNo.Checked = true;
            }
            txtNotificationTime.Text = notificationTime;
            cmbStatus.Text = status;
        }

        /// <summary>
        ///  Add new task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTaskEventHanlder(object sender, EventArgs e)
        {
            DbCommunicator db = DbCommunicator.CreateInstance(_connectionString);
            try
            {
                log.Info("Adding new task");
                // Fields validation in add task form.
                bool validationStatus = FieldValidation();
                if (true == validationStatus)
                {
                    log.Info("Successful validation");
                    db.OpenConnection();
                    int isReminder = (true == radioYes.Checked) ? 1 : 0;
                    string query = String.Format(DBQuery.INSERT_TASK, txtTitle.Text, txtItemDesc.Text,
                        DateTime.Parse(clderStartTime.Text).ToString("yyyy-MM-dd HH:mm:ss"),
                        DateTime.Parse(cldrEndTime.Text).ToString("yyyy-MM-dd HH:mm:ss"),
                        cmbStatus.SelectedItem,
                        isReminder,
                        txtNotificationTime.Text,
                        _name
                        );
                    log.Info("Query to add new task is : "+query);
                    int numberOfRows = db.ExecuteNonQuery(query);
                    if (0 < numberOfRows)
                    {
                        log.Info("New task added successfully with title " + txtTitle.Text);
                        MessageBox.Show("Task added successfully. You will be notified " + 
                            txtNotificationTime.Text + " min(s) before the task",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateGridDataEvent();
                    }
                    else
                    {
                        log.Info("Failed to add new task.");
                        MessageBox.Show("Failed to add todo task record in database.",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception message is : "+ ex.Message);
                MessageBox.Show("Failed to add todo task record in database.",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                if (null != db)
                {
                    db.CloseConnection();
                }
            }
        }

        /// <summary>
        /// Update task event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTaskEventHanlder(object sender, EventArgs e)
        {
            DbCommunicator db = DbCommunicator.CreateInstance(_connectionString);
            try
            {
                log.Info("Updating task ");
                // Fields validation in add task form.
                bool validationStatus = FieldValidation();
                if (true == validationStatus)
                {
                    db.OpenConnection();
                    int isReminder = (true == radioYes.Checked) ? 1 : 0;
                    string query = String.Format(DBQuery.UPDATE_TASK, txtTitle.Text, txtItemDesc.Text,
                        clderStartTime.Text,
                        cldrEndTime.Text,
                        cmbStatus.SelectedItem,
                        isReminder,
                        txtNotificationTime.Text, 
                        _taskId);
                    log.Info("Query to update task is : "+query);
                    int numberOfRows = db.ExecuteNonQuery(query);
                    if (0 < numberOfRows)
                    {
                        MessageBox.Show("Task updated successfully. You will be notified " +
                            txtNotificationTime.Text + " min(s) before the task",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        log.Info("Task updated successfully.");
                        // Fire event to upgrade grid data.
                        UpdateGridDataEvent();
                    }
                    else
                    {
                        log.Info("Failed to update task.");
                        MessageBox.Show("Failed to update todo task record in database.",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update todo task record in database.",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Info("Exception message is : "+ex.Message);
            }
            finally
            {
                if (null != db)
                {
                    db.CloseConnection();
                }
            }
        }

        /// <summary>
        /// Validation for form fields
        /// </summary>
        /// <returns></returns>
        private bool FieldValidation() {
            log.Info("Validating form fields");
            if (true == String.IsNullOrEmpty(txtTitle.Text) ||
                (false == radioNo.Checked && false == radioYes.Checked) ||
                true == String.IsNullOrEmpty(txtNotificationTime.Text))
            {
                MessageBox.Show("Please enter all mandatory fields.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (false == Regex.IsMatch(txtNotificationTime.Text, "^[0-9]+$") || 
                (Convert.ToInt16(txtNotificationTime.Text) < 5 || Convert.ToInt16(txtNotificationTime.Text) > 30)) {
                    MessageBox.Show("Notification time should numberic value and should be inbetween 5 to 30.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                log.Info("Form fields Validation completed successfully.");
                return true;
            }
        }

        /// <summary>
        /// Close event handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Fill status combox for task
        /// </summary>
        private void FillStatus()
        {
            ArrayList status = new ArrayList();
            
            status.Add(Status.YET_TO_BE_DONE);
            status.Add(Status.IN_PROGRESS);
            status.Add(Status.COMPLETED);

            cmbStatus.DataSource = new BindingSource(status, null);
        }

        /// <summary>
        /// Value change event handler for start time
        /// Once value is changed for start time then set same date as min date for end time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clderStartTime_ValueChanged(object sender, EventArgs e)
        {
            cldrEndTime.MinDate = clderStartTime.Value;
        }
    }
}
