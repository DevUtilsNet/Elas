using System;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions;
using DevUtils.Elas.Tasks.Core.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary>
	/// Safe task.
	/// </summary>
	public abstract class TaskExtension : Task
	{
		private int _errorsCount;

		/// <summary> Gets or sets a value indicating whether the treat warnings as errors. </summary>
		///
		/// <value> true if treat warnings as errors, false if not. </value>
		public bool TreatWarningsAsErrors { get; set; }

		/// <summary> When overridden in a derived class, executes the task. </summary>
		/// <returns> true if the task successfully executed; otherwise, false. </returns>
		public override bool Execute()
		{
			TaskLoggingTraceListener.Instance.TaskLogging = Log;
			try
			{
				try
				{
					TryExecute();
					var ret = _errorsCount == 0;
					return ret;
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

		/// <summary> Logs a warning. </summary>
		///
		/// <param name="subcategory">		 The subcategory. </param>
		/// <param name="errorCode">			 The error code. </param>
		/// <param name="helpKeyword">		 The help keyword. </param>
		/// <param name="file">						 The file. </param>
		/// <param name="lineNumber">			 The line number. </param>
		/// <param name="columnNumber">		 The column number. </param>
		/// <param name="endLineNumber">	 The end line number. </param>
		/// <param name="endColumnNumber"> The end column number. </param>
		/// <param name="message">				 The message. </param>
		/// <param name="messageArgs">			A variable-length parameters list containing message arguments. </param>
		protected void LogWarning(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
		{
			if (TreatWarningsAsErrors)
			{
				LogError(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, "Warning as Error: " + message, messageArgs);
			}
			else
			{
				Log.LogWarning(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);
			}
		}

		/// <summary> Logs an error. </summary>
		///
		/// <param name="subcategory">		 The subcategory. </param>
		/// <param name="errorCode">			 The error code. </param>
		/// <param name="helpKeyword">		 The help keyword. </param>
		/// <param name="file">						 The file. </param>
		/// <param name="lineNumber">			 The line number. </param>
		/// <param name="columnNumber">		 The column number. </param>
		/// <param name="endLineNumber">	 The end line number. </param>
		/// <param name="endColumnNumber"> The end column number. </param>
		/// <param name="message">				 The message. </param>
		/// <param name="messageArgs">			A variable-length parameters list containing message arguments. </param>
		protected void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
		{
			++_errorsCount;
			Log.LogError(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);
		}

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
		protected T CreateTask<T>() where T : ITask, new ()
		{
			var ret = new T();
			InitializeTask(ret);
			return ret;
		}
	}
}
