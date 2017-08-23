using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.PageMarkup
{
	/// <summary> The elas bind page markup and generated baml. This class cannot be inherited. </summary>
	public sealed class ElasBindPageMarkupWithGeneratedBaml : TaskExtension
	{
		/// <summary> Gets or sets source files. </summary>
		///
		/// <value> The source files. </value>
		public ITaskItem[] PageMarkup { get; set; }

		/// <summary> Gets or sets the generated baml. </summary>
		///
		/// <value> The generated baml. </value>
		public ITaskItem[] GeneratedBaml { get; set; }

		/// <summary> Gets or sets the full pathname of the intermediate output file. </summary>
		///
		/// <value> The full pathname of the intermediate output file. </value>
		[Required]
		public string IntermediateOutputPath { get; set; }

		/// <summary> Gets or sets the output page markup. </summary>
		///
		/// <value> The output page markup. </value>
		[Output]
		public ITaskItem[] OutputPageMarkup { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (PageMarkup == null || GeneratedBaml == null)
			{
				return;
			}

			var outputFiles = new List<ITaskItem>(PageMarkup.Length);

			var iop = Path.GetFullPath(IntermediateOutputPath);

			foreach (var item in GeneratedBaml)
			{
				var pp = NativeMethods.GetRelativePath(iop, FileAttributes.Directory, item.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath), FileAttributes.Normal);
				var xamlPath = Path.GetFullPath(Path.ChangeExtension(pp, ".xaml"));

				var pageMarkup =
					PageMarkup.Where(w => !outputFiles.Select(s => s.ItemSpec).Contains(w.ItemSpec) && Bind(w, xamlPath))
					          .Select(s => new TaskItem(s))
					          .FirstOrDefault();
				if (pageMarkup != null)
				{
					pageMarkup = new TaskItem(pageMarkup);
					pageMarkup.SetMetadata("ElasGeneratedBaml", item.ToString());

					outputFiles.Add(pageMarkup);
				}
			}

			OutputPageMarkup = outputFiles.ToArray();
		}

		#endregion

		private static bool Bind(ITaskItem pageMarkup, string pageMarkupFullPath)
		{
			var path = pageMarkup.GetMetadata("Link");
			if (string.IsNullOrEmpty(path))
			{
				path = pageMarkup.ToString();
			}

			path = Path.GetFullPath(path);

			var ret = path.Equals(pageMarkupFullPath, StringComparison.InvariantCultureIgnoreCase);
			return ret;
		}
	}
}
