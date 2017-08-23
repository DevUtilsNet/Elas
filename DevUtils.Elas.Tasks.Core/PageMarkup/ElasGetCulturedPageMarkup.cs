using System.IO;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.PageMarkup
{
	/// <summary> The elas get cultured page markup. This class cannot be inherited. </summary>
	public sealed class ElasGetCulturedPageMarkup : TaskExtension
	{
		/// <summary> Gets or sets source files. </summary>
		///
		/// <value> The source files. </value>
		public ITaskItem[] PageMarkup { get; set; }

		/// <summary> Gets or sets target cultures. </summary>
		///
		/// <value> The target cultures. </value>
		public ITaskItem[] TargetCultures { get; set; }

		/// <summary> Gets or sets the output files. </summary>
		///
		/// <value> The output files. </value>
		[Output]
		public ITaskItem[] OutputFiles { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (PageMarkup == null || TargetCultures == null)
			{
				return;
			}

			var index = 0;
			OutputFiles = new ITaskItem[PageMarkup.Length * TargetCultures.Length];

			foreach (var item in PageMarkup)
			{
				var originalBaml = item.RequestMetadata("ElasGeneratedBaml");
				var extension = Path.GetExtension(originalBaml);
				var withoutExtension = Path2.GetFileNameWithDirectoryWithoutExtension(originalBaml);

				foreach (var item2 in TargetCultures)
				{
					var targetCulture = item2.ItemSpec;

					var outputFile = new TaskItem(item)
					{
						ItemSpec = withoutExtension + "." + targetCulture + extension
					};

					outputFile.SetMetadata("Link", originalBaml);
					outputFile.SetMetadata("Culture", targetCulture);
					outputFile.SetMetadata("ElasTargetLanguage", targetCulture);
					outputFile.SetMetadata("ElasIntermediateDocumentPath", item.RequestMetadata("ElasIntermediateDocumentPath"));
					OutputFiles[index++] = outputFile;
				}
			}
		}

		#endregion
	}
}
