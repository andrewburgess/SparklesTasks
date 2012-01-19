using System;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Web.Administration;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[FunctionSet("sparkles", "Sparkles")]
	public class SparklesFunctions : FunctionSetBase
	{
		public SparklesFunctions(Project project, PropertyDictionary properties) : base(project, properties)
		{
		}

		[Function("iis-site-exists")]
		public static bool IISSiteExists(string siteName)
		{
			var manager = new ServerManager();
			return manager.Sites.Any(site => site.Name.ToLower() == siteName.ToLower());
		}

		[Function("db-exists")]
		public static bool DBExists(string database)
		{
			var server = new Server("localhost");
			server.ConnectionContext.LoginSecure = true;
			server.ConnectionContext.Connect();

			return server.Databases.Contains(database);
		}

		[Function("db-exists")]
		public static bool DBExists(string serverName, string database)
		{
			var server = new Server(serverName);
			server.ConnectionContext.LoginSecure = true;
			server.ConnectionContext.Connect();

			return server.Databases.Contains(database);
		}
	}
}
