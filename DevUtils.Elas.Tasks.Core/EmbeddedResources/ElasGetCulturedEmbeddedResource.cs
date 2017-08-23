using System;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;

namespace DevUtils.Elas.Tasks.Core.EmbeddedResources
{
	/// <summary>
	/// The elas add cultured embedded resource. This class cannot be inherited.
	/// </summary>
	public sealed class ElasGetCulturedEmbeddedResource : Core.TaskExtension
	{
		/// <summary> Gets or sets the pathname of the root folder. </summary>
		/// <value> The pathname of the root folder. </value>
		[Required]
		public string RootFolder { get; set; }
		/// <summary> Gets or sets the resources. </summary>
		/// <value> The resources. </value>
		public ITaskItem[] Files { get; set; }
		/// <summary> Gets or sets target cultures. </summary>
		/// <value> The target cultures. </value>
		public ITaskItem[] TargetCultures { get; set; }
		/// <summary> Gets or sets the included files. </summary>
		/// <value> The included files. </value>
		[Output]
		public ITaskItem[] OutputFiles { get; set; }
		/// <summary> Gets or sets the full pathname of the intermediate output file. </summary>
		/// <value> The full pathname of the intermediate output file. </value>
		[Required]
		public string IntermediateOutputPath { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (Files == null || TargetCultures == null)
			{
				return;
			}

			var files = Files.Where(w => 
				string.Equals(w.GetMetadata(MSBuildWellKnownItemMetadates.Extension), ".resx", StringComparison.InvariantCultureIgnoreCase)).Select(s =>
				{
					var ret = TargetCultures.Select(s2 => CreateTargetTaskItem(s, s2.ItemSpec));
					return ret;
// ReSharper disable once PossibleMultipleEnumeration
				}).SelectMany(s => s).ToArray();

			var assignTargetPathTask = CreateTask<AssignTargetPath>();
			assignTargetPathTask.Files = files;
			assignTargetPathTask.RootFolder = RootFolder;
			assignTargetPathTask.Execute();

			files = assignTargetPathTask.AssignedFiles;

			var assignCultureTask = CreateTask<AssignCulture>();
			assignCultureTask.Files = files;
			assignCultureTask.Execute();
			OutputFiles = assignCultureTask.AssignedFilesWithCulture;

			foreach (var item in OutputFiles)
			{
				item.SetMetadata("ElasTargetLanguage", item.RequestMetadata("Culture"));
			}
		}

		private ITaskItem CreateTargetTaskItem(ITaskItem source, string culture)
		{
			var extension = source.RequestMetadata(MSBuildWellKnownItemMetadates.Extension);
			var targetPath = Path.ChangeExtension(source.RequestMetadata("TargetPath"), culture) + extension;
			var ret = source.CreateRelativeItem(Path.Combine(IntermediateOutputPath, targetPath));

			ret.SetMetadata("Link", targetPath);
			return ret;
		}

		#endregion
	}
}
