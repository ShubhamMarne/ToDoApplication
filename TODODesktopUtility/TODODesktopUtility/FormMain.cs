namespace TODODesktopUtility
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Windows.Forms;
    using ToDoHelperLibrary;

    /// <summary>
    /// Class represents Main form.
    /// </summary>
    public partial class FormMain : Form
    {
        private FormLogin _loginForm;
        
        DataSet dataSet = new DataSet();
        private string _name;
        public static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        /// <summary>
        /// Initialize instance for Main form
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loginForm"></param>
        public FormMain(string name, FormLogin loginForm)
        {
            InitializeComponent();
            // Subscribe event to update grid data.
            FormTask.UpdateGridDataEvent += new UpdateGridData(OnUpgradeGrid);
            lblWelcome.Text = "Welcome  " + name;
            _name = name;
            _loginForm = loginForm;
            // Fill grid
            FillGrid();
        }

        /// <summary>
        /// Form close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _loginForm.Close();
        }

        /// <summary>
        /// Click event for logout button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, EventArgs e)
        {
            _loginForm.Close();
        }

        /// <summary>
        /// Fill grid
        /// </summary>
        private void FillGrid()
        {
            DbCommunicator db = DbCommunicator.CreateInstance(_connectionString);
            try
            {
                log.Info("Get data to fill grid");
                db.OpenConnection();
                dataSet.Clear();
                dataSet = db.ExecuteQuery(DBQuery.GET_TASK_DATA, "task");
                dataGridView.DataSource = dataSet.Tables["task"];
            }
            catch (Exception ex)
            {
                log.Error("Exception message is : " + ex.Message);
                throw ex;
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
        /// Click event handler for add task button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddTask_Click(object sender, EventArgs e)
        {
            log.Info("Opening form task");
            FormTask formAddTask = new FormTask(_name);
            formAddTask.Show();
        }

        /// <summary>
        /// Subsriber method to upgrade grid data once task is added.
        /// </summary>
        private void OnUpgradeGrid()
        {
            log.Info("Subsriber method called to upgrade grid.");
            FillGrid();
        }

        /// <summary>
        /// Key up event handler for search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string searchQuery = "title like '%{0}%' or status like '%{0}%' ";
            string query = string.Format(searchQuery, txtSearch.Text.Trim());
            FillGrid();
            DataView dataView = dataSet.Tables["task"].DefaultView;
            dataView.RowFilter = query;
            dataGridView.DataSource = dataView.ToTable();
        }

        /// <summary>
        /// Double click event for row in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            log.Info("Updating task");
            DataGridViewRow selectedRow = dataGridView.Rows[e.RowIndex];
            FormTask formUpdateTask = new FormTask(Convert.ToInt16(selectedRow.Cells[0].Value.ToString()),
                selectedRow.Cells[1].Value.ToString(),
                selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[3].Value.ToString(),
                selectedRow.Cells[4].Value.ToString(), selectedRow.Cells[5].Value.ToString(),
                selectedRow.Cells[6].Value.ToString(), selectedRow.Cells[7].Value.ToString());
            formUpdateTask.Show();
            log.Info("Task updation completed.");
        }
    }
}
