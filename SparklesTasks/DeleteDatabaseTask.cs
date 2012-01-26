using System;
using System.Data;
using System.IO;
using System.Threading;
using Microsoft.SqlServer.Management.Common;
using NAnt.Core;
using NAnt.Core.Attributes;
using Microsoft.SqlServer.Management.Smo;

namespace SparklesTasks
{
	[TaskName("delete-db")]
	public class DeleteDatabaseTask : Task
	{
		[TaskAttribute("db-name", Required = true)]
		public string DBName { get; set; }

		private string serverName = @"localhost\sql2008";
		[TaskAttribute("server")]
		public string ServerName { get { return serverName; } set { serverName = value; } }

		protected override void ExecuteTask()
		{
			var server = new Server(ServerName);
			server.ConnectionContext.LoginSecure = true;
			server.ConnectionContext.Connect();

			if (server.Databases.Contains(DBName) == false)
			{
				Log(Level.Warning, "Database " + DBName + " does not exist on the server");
			}
			else
			{

				if (Verbose)
					Log(Level.Info, "Dropping database " + DBName);
				try
				{
					server.Databases[DBName].Drop();
				}
				catch (Exception e)
				{
					if (Verbose)
						Log(Level.Info, "Killing connections to " + DBName);

					server.KillAllProcesses(DBName);
					server.Databases[DBName].Drop();
				}
				Log(Level.Info, DBName + " dropped");
			}

			if (server.ConnectionContext.IsOpen)
				server.ConnectionContext.Disconnect();
		}
	}
}