using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SparklesTasks
{
	[TaskName("movedir")]
	public class MoveDirectoryTask : Task
	{
		[TaskAttribute("from", Required = true)]
		public string SourceDirectory { get; set; }

		[TaskAttribute("to", Required = true)]
		public string DestinationDirectory { get; set; }

		protected override void ExecuteTask()
		{
			Log(Level.Info, string.Format("Moving {0} to {1}", SourceDirectory, DestinationDirectory));
			Directory.Move(SourceDirectory, DestinationDirectory);
		}						
	}
}