using System;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions;
using DevUtils.Elas.Tasks.Core.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary> An application domain isolated task extension. </summary>
	public abstract class AppDomainIsolatedTaskExtension : AppDomainIsolatedTask
	{
		#region Overrides of AppDomainIsolatedTask

		/// <summary>
		/// Runs the task.
		/// </summary>
		/// <returns>
		/// true if successful; otherwise, false.
		/// </returns>
		public override bool Execute()
		{
			TaskLoggingTraceListener.Instance.TaskLogging = Log;
			try
			{
				try
				{
					TryExecute();
					return true;
				}
				catch (XmlException e)
				{
					var file = e.SourceUri;
					if (Uri.IsWellFormedUriString(file, UriKind.Absolute))
					{
						var uri = new Uri(file);
						file = uri.LocalPath;
					}
					throw new TaskException(file, e.LineNumber, e.LinePosition, e.Message, e);
				}
			}
			catch (TaskException e)
			{
				Log.LogError(
					e.Subcategory,
					e.ErrorCode,
					e.HelpKeyword,
					e.File,
					e.LineNumber,
					e.ColumnNumber,
					e.EndLineNumber,
					e.EndColumnNumber,
					e.Message);
			}
			catch (AggregateException e)
			{
				Log.LogErrorFromAggregateException(e);
				return false;
			}
			catch (Exception e)
			{
				Log.LogErrorFromException(e);
			}
			finally
			{
				TaskLoggingTraceListener.Instance.TaskLogging = null;
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Try execute.
		/// </summary>
		protected abstract void TryExecute();

		/// <summary> Initializes the task. </summary>
		/// <param name="task"> The task. </param>
		protected void InitializeTask(ITask task)
		{
			task.BuildEngine = BuildEngine;
			task.HostObject = HostObject;
		}
		/// <summary> Creates the task. </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <returns> The new task. </returns>
		protected T CreateTask<T>() where T : ITask, new()
		{
			var ret = new T();
			InitializeTask(ret);
			return ret;
		}
	}
}
