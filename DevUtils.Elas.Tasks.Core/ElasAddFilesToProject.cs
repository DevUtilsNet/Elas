using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary> The elas add files to project. This class cannot be inherited. </summary>
	public sealed class ElasAddFilesToProject : TaskExtension
	{
		/// <summary> Gets the files. </summary>
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			foreach (var item in Files)
			{
				item.EnsureInProject();
			}
		}
	}
}
