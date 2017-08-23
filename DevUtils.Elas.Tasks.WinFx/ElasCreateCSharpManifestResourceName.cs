using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elas.Tasks.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace Elas.Tasks.WinFx
{
	public class ElasCreateCSharpManifestResourceName : SafeTask
	{
		private readonly List<ITaskItem> _targetFiles = new List<ITaskItem>();

		/// <summary>Gets or sets the root namespace to use for naming.</summary>
		/// <returns>The root namespace to use for naming.</returns>
		public String RootNamespace { get; set; }

		[Required]
		public ITaskItem[] SourceFiles { get; set; }

		[Output]
		public ITaskItem[] TargetFiles
		{
			get { return _targetFiles.ToArray(); }
		}

		protected override void TryExecute()
		{
			var originalItems = new List<TaskItem>();

			foreach (var item in SourceFiles)
			{
				var taskItem = new TaskItem(item.GetMetadata("TargetPath"));
				if (originalItems.Any(a => String.Equals(a.ItemSpec, taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}
				originalItems.Add(taskItem);
			}

			var task = new CreateCSharpManifestResourceName { ResourceFiles = originalItems.Cast<ITaskItem>().ToArray(), RootNamespace = RootNamespace, BuildEngine = BuildEngine };

			task.Execute();

			foreach (var item in SourceFiles)
			{
				var link = item.GetMetadata("TargetPath");
				var linkTaskItem = task.ResourceFilesWithManifestResourceNames.First(f => String.Equals(f.ItemSpec, link, StringComparison.OrdinalIgnoreCase));

				var taskItem = new TaskItem(item);
				var culture = Path.GetExtension(Path.GetFileNameWithoutExtension(item.ItemSpec));
				taskItem.SetMetadata("ManifestResourceName", linkTaskItem.GetMetadata("ManifestResourceName") + culture);
				_targetFiles.Add(taskItem);
			}
		}
	}
}
