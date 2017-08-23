using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Elas.Tasks.Common
{
	public class ElasGetInputs : SafeTask
	{
		private readonly List<ITaskItem> _inputFiles = new List<ITaskItem>();

		[Required]
		public ITaskItem[] SourceFiles { get; set; }

		[Required]
		public String OutputPath { get; set; }

		[Output]
		public ITaskItem[] InputFiles
		{
			get { return _inputFiles.ToArray(); }
		}

		protected override void TryExecute()
		{
			foreach (var item in SourceFiles)
			{
				var itemFullPathComponents = Path.GetFullPath(item.ItemSpec).Split('\\');
				var outputFullPathComponents = Path.GetFullPath(OutputPath).Split('\\');

				var index = 0;
				for (var count = Math.Min(itemFullPathComponents.Length, outputFullPathComponents.Length); index < count; ++index)
				{
					if (String.Compare(itemFullPathComponents[index], outputFullPathComponents[index], StringComparison.OrdinalIgnoreCase) != 0)
					{
						break;
					}
				}

				var filePath = String.Join("$", itemFullPathComponents, index, itemFullPathComponents.Length - index);
				var taskItem = new TaskItem(Path.Combine(OutputPath, filePath));
				item.CopyMetadataTo(taskItem);
				taskItem.SetMetadata("SourcePath", item.ToString());
				_inputFiles.Add(taskItem);
			}
		}
	}
}
