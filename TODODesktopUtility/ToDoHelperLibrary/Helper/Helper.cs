namespace ToDoHelperLibrary
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Net.Mail;

    /// <summary>
    /// Class represents Helper methods for TODO Utility.
    /// </summary>
    public class Helper
    {
        public static string Subject = "Reminder for your TODO : "+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Send email for each user with task.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        private static void Send(string email, string title, string desc, string HostAddress, string FromEmailid, string Pass)
        {
            string Message = "Please complete your TODO task as follows : "+ "<br>"
                + " Title : "+ title
                + "<br>"
                + " Description : "+ desc;

            //creating the object of MailMessage  
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailid); 
            mailMessage.Subject = Subject;   
            mailMessage.Body = Message; 
            mailMessage.IsBodyHtml = true;

            string[] ToEmailsMultiple = email.Split(',');
            foreach (string ToEmail in ToEmailsMultiple)
            {
                mailMessage.To.Add(new MailAddress(ToEmail));
            }

            SmtpClient smtp = new SmtpClient();  // creating object 
            smtp.Host = HostAddress;

            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = mailMessage.From.Address;
            NetworkCred.Password = Pass;
            smtp.UseDefaultCredentials = true;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = NetworkCred;
            smtp.Send(mailMessage); //Send email
        }

        /// <summary>
        /// Send emails 
        /// This method is called by windows service after evert 1 min.
        /// This method is responsible for checking notification from database,
        /// and send notification for each task if valid as per time.
        /// </summary>
        public static void SendEmails(string HostAddress, string FromEmailid, string Pass, string databaseConnectionString)
        {
            DbCommunicator db = DbCommunicator.CreateInstance(databaseConnectionString);
            try
            {
                db.OpenConnection();
                DataSet data = db.ExecuteQuery(DBQuery.GET_EMAIL_DETAILS, "taskDetails");
                // Iterate data from dataset and send email as notification which contains title, description.
                foreach (DataRow item in data.Tables["taskDetails"].Rows)
                {
                    string email = item["email"].ToString();
                    string title = item["title"].ToString();
                    string desc = item["description"].ToString();
                    if (false == String.IsNullOrEmpty(email))
                    {
                        // send email
                        Send(email, title, desc, HostAddress, FromEmailid, Pass);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db != null)
                {
                    db.CloseConnection();
                }
            }
        }
    }

    /// <summary>
    /// Constant values for task status.
    /// </summary>
    public class Status
    {
        public const string YET_TO_BE_DONE = "yet to be done";
        public const string IN_PROGRESS = "in progess";
        public const string COMPLETED = "completed";
    }

}
