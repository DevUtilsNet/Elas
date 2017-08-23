using System;
using Microsoft.Build.Utilities;

namespace Elas.Tasks.Common
{
	/// <summary>
	/// Safe task.
	/// </summary>
	public abstract class SafeTask : Task
	{
		/// <summary>
		/// When overridden in a derived class, executes the task.
		/// </summary>
		/// <returns>
		/// true if the task successfully executed; otherwise, false.
		/// </returns>
		public override bool Execute()
		{
			try
			{
				TryExecute();
			}
			catch (Exception e)
			{
				Log.LogErrorFromException(e);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Try execute.
		/// </summary>
		protected abstract void TryExecute();
	}
}
