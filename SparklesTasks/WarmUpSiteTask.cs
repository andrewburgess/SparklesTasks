using System;
using System.Net;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[TaskName("warm-up-site")]
	public class WarmUpSiteTask : Task
	{
		[TaskAttribute("url", Required = true)]
		public string Url { get; set; }

		protected override void ExecuteTask()
		{
			Log(Level.Info, "Warming up " + Url);
			var retries = 20;
			for (var i = 0; i < retries; i++)
			{
				try
				{
					var req = (HttpWebRequest)WebRequest.Create(Url);
					req.AllowAutoRedirect = false;
					req.Timeout = (1000 * 60 * 5); //5 minutes
					var rsp = (HttpWebResponse)req.GetResponse();

					if (rsp.StatusCode == HttpStatusCode.OK)
					{
						Log(Level.Info, string.Format("{0} responded: {1}", Url, rsp.StatusDescription));
						return;
					}
					else
					{
						Log(Level.Warning, string.Format("{0} responded: {1}", Url, rsp.StatusDescription));
					}
				}
				catch (WebException we)
				{
					Log(Level.Warning, string.Format("IIS not ready. Retry #{0}...", i + 1));
				}
			}

			Log(Level.Error, string.Format("{0} never responded OK", Url));
			throw new ApplicationException("Can't contact server");
		}
	}
}