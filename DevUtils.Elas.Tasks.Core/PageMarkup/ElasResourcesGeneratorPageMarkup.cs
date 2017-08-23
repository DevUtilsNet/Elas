using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks.Windows;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.PageMarkup
{
	/// <summary> The elas resources generator page markup. This class cannot be inherited. </summary>
	public sealed class ElasResourcesGeneratorPageMarkup : TaskExtension
	{
		/// <summary> Gets or sets the full pathname of the output file. </summary>
		///
		/// <value> The full pathname of the output file. </value>
		[Required]
		public string OutputPath { get; set; }

		/// <summary> Gets or sets the name of the assembly. </summary>
		///
		/// <value> The name of the assembly. </value>
		[Required]
		public string AssemblyName { get; set; }

		/// <summary> Gets or sets the resource files. </summary>
		///
		/// <value> The resource files. </value>
		public ITaskItem[] ResourceFiles { get; set; }

		/// <summary> Gets or sets the output resources files. </summary>
		///
		/// <value> The output resources files. </value>
		[Output]
		public ITaskItem[] OutputResourcesFiles { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (ResourceFiles == null)
			{
				return;
			}

			var resourcesGenerator = CreateTask<ResourcesGenerator>();

			var outputResourcesFiles = new List<ITaskItem>();

			foreach (var item in ResourceFiles.GroupBy(g => g.GetMetadata("Culture")))
			{
				if (string.IsNullOrEmpty(item.Key))
				{
					continue;
				}

				var outputResourcesFile = new TaskItem(Path.Combine(OutputPath, string.Format("{0}.g.{1}.resources", AssemblyName, item.Key)));
				outputResourcesFile.SetMetadata("Culture", item.Key);

				resourcesGenerator.OutputPath = OutputPath;
				resourcesGenerator.ResourceFiles = item.ToArray();
				resourcesGenerator.OutputResourcesFile = new ITaskItem[] { outputResourcesFile };
				resourcesGenerator.Execute();

				outputResourcesFiles.Add(outputResourcesFile);
			}

			OutputResourcesFiles = outputResourcesFiles.ToArray();
		}

		#endregion
	}
}
