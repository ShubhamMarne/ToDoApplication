namespace ToDoHelperLibrary
{
    /// <summary>
    /// Class represents DB Queries used for communicating with database.
    /// </summary>
    public class DBQuery
    {
        /// <summary>
        /// Get user count 
        /// </summary>
        public static string GET_USER_COUNT= "SELECT COUNT(id) FROM user WHERE name = '{0}' AND password = '{1}'";

        /// <summary>
        /// Get task details
        /// </summary>
        public static string GET_TASK_DATA = "SELECT id, title, description, status, start_time, end_time, send_reminder, notification_time FROM task";

        /// <summary>
        /// Query to insert new task
        /// </summary>
        public static string INSERT_TASK = "INSERT INTO task(`title`, `description`, `start_time`, `end_time`, `status`, `send_reminder`, `notification_time`, `user_id`)"
            +" values ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', (SELECT id FROM user WHERE name = '{7}'))";

        /// <summary>
        /// Update task
        /// </summary>
        public static string UPDATE_TASK = "UPDATE task SET `title` = '{0}', `description` = '{1}', `start_time` = '{2}', `end_time` = '{3}', `status` = '{4}', "+
            " `send_reminder` = {5}, `notification_time` = '{6}' WHERE id = {7}";

        /// <summary>
        /// Get email details to send email using windows service
        /// </summary>
        public static string GET_EMAIL_DETAILS = "SELECT t.title, t.description, u.email FROM task t  "+
            " INNER JOIN user u "+
            " ON u.id = t.user_id AND (NOW() < DATE_SUB(t.end_time, INTERVAL t.notification_time MINUTE) and "+
            " (DATE_SUB(t.end_time, INTERVAL t.notification_time MINUTE)) < DATE_ADD(NOW(), INTERVAL 1 MINUTE)) "+
            " AND t.status != 'completed' AND t.send_reminder = 1";

        /// <summary>
        /// Query to insert new user.
        /// </summary>
        public static string INSERT_USER = "INSERT INTO user(`name`, `password`, `email`) VALUES('{0}', '{1}', '{2}')";
    }
}
