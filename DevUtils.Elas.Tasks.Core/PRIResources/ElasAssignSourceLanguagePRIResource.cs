using System.Globalization;
using System.IO;
using DevUtils.Elas.Tasks.Core.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.PRIResources
{
	/// <summary> The elas assign source language pri resource. This class cannot be inherited. </summary>
	public sealed class ElasAssignSourceLanguagePRIResource : TaskExtension
	{
		/// <summary> Gets or sets the resources. </summary>
		/// <value> The resources. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Gets or sets the default language. </summary>
		///
		/// <value> The default language. </value>
		public string DefaultLanguage { get; set; }

		/// <summary> Gets or sets the assigned files. </summary>
		/// <value> The assigned files. </value>
		[Output]
		public ITaskItem[] AssignedFiles { get; set; }

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			var index = 0;
			AssignedFiles = new ITaskItem[Files.Length];

			foreach (var item in Files)
			{
				var targetDir = Path.GetDirectoryName(item.ToString());
				var culture = Path.GetFileName(targetDir);

				if (!culture.IsValidCultureName())
				{
					if (string.IsNullOrEmpty(DefaultLanguage))
					{
						culture = CultureInfo.CurrentCulture.Name;

						Log.LogWarning(
							Log.FormatString(
								"The DefaultLanguage property is either missing from the project file or does not have a value. " +
								"The fallback language is set to the Visual Studio language: {0}.",
								culture));
					}
					else
					{
						culture = DefaultLanguage;
					}
				}

				var newItem = new TaskItem(item);

				newItem.SetMetadata("ElasSourceLanguage", culture);

				AssignedFiles[index++] = newItem;
			}
		}
	}
}