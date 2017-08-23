using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.IO.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary> The elas clear read only. This class cannot be inherited. </summary>
	public sealed class ElasClearReadOnly : TaskExtension
	{
		/// <summary> Gets or sets the files. </summary>
		///
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			foreach (var item in Files.Select(s => new FileInfo(s.ItemSpec)))
			{
				item.DTEClearReadOnly();
			}
		}

		#endregion
	}
}