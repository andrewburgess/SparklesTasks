using System;
using System.Collections.Specialized;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[TaskName("attach-db")] 
	public class AttachDatabaseTask : Task
	{
		[TaskAttribute("replace")]
		public bool ReplaceDB { get; set; }

		[TaskAttribute("db-name", Required = true)]
		public string DBName { get; set; }

		[TaskAttribute("mdf-path", Required = true)]
		public string MDFPath { get; set; }

		[TaskAttribute("ldf-path", Required = true)]
		public string LDFPath { get; set; }

		[TaskAttribute("owner")]
		public string Owner { get; set; }

		private string serverName = @"localhost";
		[TaskAttribute("server")]
		public string ServerName { get { return serverName; } set { serverName = value; } }

		protected override void ExecuteTask()
		{
			var server = new Server(ServerName);
			server.ConnectionContext.LoginSecure = true;
			server.ConnectionContext.Connect();

			File.Move(MDFPath, Path.Combine(server.Settings.DefaultFile, DBName + ".mdf"));
			File.Move(LDFPath, Path.Combine(server.Settings.DefaultFile, DBName + ".ldf"));

			var mdf = Path.Combine(server.Settings.DefaultFile, DBName + ".mdf");
			var ldf = Path.Combine(server.Settings.DefaultFile, DBName + ".ldf");

			if (!ReplaceDB)
			{
				if (server.Databases.Contains(DBName))
				{
					Log(Level.Info, DBName + " exists on " + ServerName + " and will not be replaced");
					return;
				}
			}

			if (server.Databases.Contains(DBName))
			{
				Log(Level.Info, "Removing existing database and replacing it");
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
				Log(Level.Info, "Database removed");
			}

			Log(Level.Info, "Attaching database " + DBName + " to " + ServerName);
			var sc = new StringCollection {mdf, ldf};

			if (string.IsNullOrEmpty(Owner))
				server.AttachDatabase(DBName, sc);
			else
				server.AttachDatabase(DBName, sc, Owner);

			Log(Level.Info, DBName + " attached to " + ServerName);

			if (server.ConnectionContext.IsOpen)
				server.ConnectionContext.Disconnect();
		}
	}
}