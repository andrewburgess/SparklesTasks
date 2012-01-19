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
	[TaskName("restore-db")]
	public class RestoreDatabaseTask : Task
	{
		[TaskAttribute("replace")]
		public bool ReplaceDB { get; set; }

		[TaskAttribute("db-name", Required = true)]
		public string DBName { get; set; }

		[TaskAttribute("backup-path", Required = true)]
		public string BackupPath { get; set; }

		private string serverName = @"localhost";
		[TaskAttribute("server")]
		public string ServerName { get { return serverName; } set { serverName = value; } }

		protected override void ExecuteTask()
		{
			var server = new Server(ServerName);
			server.ConnectionContext.LoginSecure = true;
			server.ConnectionContext.Connect();

			if (!ReplaceDB)
			{
				if (server.Databases.Contains(DBName))
				{
					Log(Level.Info, DBName + " exists on " + ServerName + " and will not be replaced");
					return;
				}
			}

			Log(Level.Info, "Restoring database " + DBName + " to " + ServerName);
			var restore = new Restore();
			restore.Database = DBName;
			restore.ReplaceDatabase = ReplaceDB;
			restore.Action = RestoreActionType.Database;
			restore.Devices.AddDevice(BackupPath, DeviceType.File);

			var table = restore.ReadFileList(server);
			restore.RelocateFiles.Add(new RelocateFile(table.Rows[0][0].ToString(), Path.Combine(server.Settings.DefaultFile, DBName + ".mdf")));
			restore.RelocateFiles.Add(new RelocateFile(table.Rows[1][0].ToString(), Path.Combine(server.Settings.DefaultFile, DBName + "_Log.ldf")));

			restore.PercentComplete += UpdatePercent;
			restore.Complete += RestoreCompleted;

			restore.SqlRestore(server);

			if (server.ConnectionContext.IsOpen)
				server.ConnectionContext.Disconnect();
		}

		private void UpdatePercent(object sender, PercentCompleteEventArgs e)
		{
			if (Verbose) Log(Level.Info, string.Format("Restore {0}% Complete", e.Percent));
		}

		protected void RestoreCompleted(object sender, ServerMessageEventArgs serverMessageEventArgs)
		{
			Log(Level.Info, DBName + " restored");
		}
	}
}