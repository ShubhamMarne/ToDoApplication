namespace WindowsMailService
{
    using System.Configuration;
    using System.Timers;
    using ToDoHelperLibrary;

    /// <summary>
    /// Class represents windows service to send email to users for their TODO task.
    /// NOTE : Once this windows service is installed and started it will wakeup for
    /// every 1 min(i.e. 60000 milisecond) and check is any email need to send to the user.
    /// If needed then send email one by one.
    /// To check eligibility for task for sending email, please check database query in DBQuery class in TODODesktopUtility.
    /// </summary>
    class WindowsEmailService
    {
        private readonly Timer _timer;

        /// <summary>
        /// Initialize WindowsEmailService instance.
        /// </summary>
        public WindowsEmailService() {
            // Use timer by using which service will wakeup for every minute and check email notification to send.
            _timer = new Timer(60000) { AutoReset = true };
            // Subscribe method for time elapsed.
            _timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Subscription method for windows service to send email.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Check email notification from database and send email.
            string HostAddress = ConfigurationManager.AppSettings["Host"].ToString();
            string FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
            string Pass = ConfigurationManager.AppSettings["Password"].ToString();
            string DatabaseConnectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            Helper.SendEmails(HostAddress, FromEmailid, Pass, DatabaseConnectionString);
        }

        /// <summary>
        /// Start Service.
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// Stop service.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }
    }
}
