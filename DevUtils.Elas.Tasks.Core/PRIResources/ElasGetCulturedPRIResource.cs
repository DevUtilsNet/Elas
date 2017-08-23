using System;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core.PRIResources
{
	/// <summary> The elas include cultured pri resource. </summary>
	public class ElasGetCulturedPRIResource : TaskExtension
	{
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

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null || TargetCultures == null)
			{
				return;
			}

			var files = Files.Where(w =>
				string.Equals(w.GetMetadata(MSBuildWellKnownItemMetadates.Extension), ".resw", StringComparison.InvariantCultureIgnoreCase)).Select(s =>
			{
				var ret = TargetCultures.Select(s2 => CreateTargetTaskItem(s, s2.ItemSpec));
				return ret;
				// ReSharper disable once PossibleMultipleEnumeration
			}).SelectMany(s => s).ToArray();

			OutputFiles = files;
		}

		private ITaskItem CreateTargetTaskItem(ITaskItem source, string culture)
		{
			var targetPath = source.RequestMetadata("TargetPath");

			var targetDir = Path.GetDirectoryName(targetPath);

			if (Path.GetFileName(targetDir).IsValidCultureName())
			{
				targetDir = Path.GetDirectoryName(targetDir);
			}

			targetPath = Path.Combine(Path.Combine(targetDir, culture), Path.GetFileName(targetPath));
			var ret = source.CreateRelativeItem(Path.Combine(IntermediateOutputPath, targetPath));

			ret.SetMetadata("Link", targetPath);
			ret.SetMetadata("Culture", culture);
			ret.SetMetadata("ElasTargetLanguage", culture);

			return ret;
		}
	}
}
