using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.IO;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	/// <summary> The elas get cultured resource compile. This class cannot be inherited. </summary>
	public sealed class ElasGetCulturedResourceCompile : Core.TaskExtension
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
			if (Files == null || TargetCultures == null || TargetCultures.Length == 0)
			{
				return;
			}

			OutputFiles = Files.Select(s =>
			{
				var ret = TargetCultures.Select(s2 => CreateTargetTaskItem(s, s2.ItemSpec));
				return ret;
				// ReSharper disable once PossibleMultipleEnumeration
			}).SelectMany(s => s).ToArray();
		}

		private ITaskItem CreateTargetTaskItem(ITaskItem source, string culture)
		{
			var targetPath = source.RequestMetadata("TargetPath");

			var newTargetPath = Path2.ChangeFileNameWithoutExtension(targetPath, Path.GetFileNameWithoutExtension(targetPath) + "." + culture);

			var ret = source.CreateRelativeItem(Path.Combine(IntermediateOutputPath, newTargetPath));

			var outFileName = ret.RequestMetadata("ResourceOutputFileName");
			var newName = Path2.ChangeFileNameWithoutExtension(outFileName, Path.GetFileNameWithoutExtension(outFileName) + "." + culture);
			ret.SetMetadata("ResourceOutputFileName", newName);
			ret.SetMetadata("ElasTargetLanguage", culture);

			return ret;
		}
	}
}
