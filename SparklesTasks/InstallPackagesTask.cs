using System;
using System.Net;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[TaskName("install-packages")]
	public class InstallPackagesTask : Task
	{
		[TaskAttribute("url", Required = true)]
		public string Url { get; set; }


		private string runtimePath = "PackageInstaller/InstallerRunTimeServices.aspx";
		[TaskAttribute("runtime-path")]
		public string RuntimePath { get { return runtimePath; } set { runtimePath = value; } }

		protected override void ExecuteTask()
		{
			Log(Level.Info, "Installing packages...");

			var fullUrl = string.Format("{0}/{1}?Invoke=InstallAllPackages", Url, RuntimePath);
			var req = (HttpWebRequest) WebRequest.Create(fullUrl);
			req.AllowAutoRedirect = false;
			req.Timeout = (1000*60*10); //10 minutes

			var rsp = (HttpWebResponse) req.GetResponse();

			if (rsp.StatusCode == HttpStatusCode.OK)
			{
				Log(Level.Info, "{0} responded: {1}", Url, rsp.StatusDescription);
			}
			else
			{
				Log(Level.Error, "{0} did not respond with OK (" + rsp.StatusDescription + ")");
			}
		}
	}
}