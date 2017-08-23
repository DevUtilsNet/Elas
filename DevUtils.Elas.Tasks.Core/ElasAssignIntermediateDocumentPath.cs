using System;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary>
	/// The elas assign intermediate document path. This class cannot be inherited.
	/// </summary>
	public sealed class ElasAssignIntermediateDocumentPath : TaskExtension
	{
		/// <summary> Gets or sets the pathname of the root folder. </summary>
		/// <value> The pathname of the root folder. </value>
		[Required]
		public string RootFolder { get; set; }
		/// <summary> Gets the files. </summary>
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }
		/// <summary> Gets or sets the assigned files. </summary>
		/// <value> The assigned files. </value>
		[Output]
		public ITaskItem[] AssignedFiles { get; set; }

		#region Overrides of SafeTask

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			var withoutTargetPath = Files.Where(w => string.IsNullOrEmpty(w.GetMetadata("TargetPath"))).ToArray();
			if (withoutTargetPath.Length > 0)
			{
				var assignTargetPathTask = CreateTask<AssignTargetPath>();
				assignTargetPathTask.RootFolder = RootFolder;
				assignTargetPathTask.Files = withoutTargetPath;
				assignTargetPathTask.Execute();

				Files = Files.Except(withoutTargetPath).Concat(assignTargetPathTask.AssignedFiles).ToArray();
			}

			AssignedFiles =
				Files.Where(w => !string.Equals(w.GetMetadata("ElasIgnore"), "true", StringComparison.InvariantCultureIgnoreCase))
				     .Select(s =>
				             {
					             var tp = s.RequestMetadata("TargetPath");
					             var i = new TaskItem(s);
					             i.SetMetadata("ElasSourceItemSpec", tp);
					             i.SetMetadata("ElasIntermediateDocumentPath", tp + ".xlf");
					             return (ITaskItem) i;
				             }).ToArray();
		}

		#endregion
	}
}
