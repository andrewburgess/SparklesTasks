using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

		[Function("file-contains")]
		public static bool FileContains(string path, string test)
		{
			var file = File.OpenRead(path);
			var reader = new StreamReader(file);
			foreach (var line in reader.ReadToEnd().Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.RemoveEmptyEntries))
			{
				var match = Regex.Match(line, test, RegexOptions.IgnoreCase);
				if (match.Success)
				{
					reader.Close();
					file.Close();
					return true;
				}
			}

			reader.Close();
			file.Close();
			return false;
		}
	}
}
