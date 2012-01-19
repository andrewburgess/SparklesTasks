using System.Diagnostics;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[TaskName("open-url")]
	public class OpenURLTask : Task
	{
		[TaskAttribute("url", Required = true)]
		public string URL { get; set; }

		protected override void ExecuteTask()
		{
			Process.Start(URL);
		}
	}
}