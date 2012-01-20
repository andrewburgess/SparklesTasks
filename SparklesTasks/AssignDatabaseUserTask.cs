using System.Data;
using System.Data.SqlClient;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[TaskName("db-assign-user")]
	public class AssignDatabaseUserTask : Task
	{
		[TaskAttribute("db-name", Required = true)]
		public string DBName { get; set; }

		[TaskAttribute("db-user", Required = true)]
		public string DBUser { get; set; }

		private string serverName = @"localhost";
		[TaskAttribute("server")]
		public string ServerName { get { return serverName; } set { serverName = value; } }

		protected override void ExecuteTask()
		{
			var cstr = string.Format(@"Data Source={0};Database={1};Trusted_Connection=Yes;", ServerName, DBName);
			var conn = new SqlConnection(cstr);

			var cmd = new SqlCommand();
			cmd.CommandText = string.Format(@"EXEC sp_changedbowner {0}", DBUser);
			cmd.CommandType = CommandType.Text;
			cmd.Connection = conn;

			conn.Open();
			if (Verbose)
				Log(Level.Info, string.Format("Assigning user {0} to database {1}", DBUser, DBName));
			cmd.ExecuteNonQuery();
			conn.Close();
		}
	}
}