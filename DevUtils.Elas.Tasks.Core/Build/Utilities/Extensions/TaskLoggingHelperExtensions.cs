using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.IO.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions
{
	/// <summary> A task logging helper extensions. </summary>
	public static class TaskLoggingHelperExtensions
	{
		/// <summary> A TaskLoggingHelper extension method that determine if we should build. </summary>
		///
		/// <param name="log">	  The log to act on. </param>
		/// <param name="source"> Source for the. </param>
		/// <param name="target"> Target for the. </param>
		///
		/// <returns> true if it succeeds, false if it fails. </returns>
		public static bool ShouldBuild(this TaskLoggingHelper log, FileInfo source, FileInfo target)
		{
			var ret = log.ShouldBuild(Enumerable.Repeat(source, 1), Enumerable.Repeat(target, 1));
			return ret;
		}
		/// <summary> A TaskLoggingHelper extension method that determine if we should build. </summary>
		///
		/// <param name="log">		 The log to act on. </param>
		/// <param name="sources"> The sources. </param>
		/// <param name="targets"> The targets. </param>
		///
		/// <returns> true if it succeeds, false if it fails. </returns>
		public static bool ShouldBuild(this TaskLoggingHelper log, IEnumerable<FileInfo> sources, IEnumerable<FileInfo> targets)
		{
			var targets2 = targets.ToArray();

			var notExists = targets2.FirstOrDefault(f => !f.Exists);
			if (notExists != null)
			{
				log.LogMessage(MessageImportance.Low, log.FormatString("Output file \"{0}\" does not exist.", notExists.GetDisplayPath()));
				return true;
			}

			foreach (var item in sources)
			{
				var trg = targets2.FirstOrDefault(f => f.LastWriteTimeUtc < item.LastWriteTimeUtc);
				if (trg != null)
				{
					log.LogMessage(MessageImportance.Low, log.FormatString("Input file \"{0}\" is newer than output file \"{1}\".", item.GetDisplayPath(), trg.GetDisplayPath()));
					return true;
				}
			}
			return false;
		}

		/// <summary> A TaskLoggingHelper extension method that logs error from aggregate exception. </summary>
		///
		/// <param name="log">							  The log to act on. </param>
		/// <param name="aggregateException"> Details of the exception. </param>
		public static void LogErrorFromAggregateException(this TaskLoggingHelper log, AggregateException aggregateException)
		{
			foreach (var item in aggregateException.InnerExceptions)
			{
				var ae = item as AggregateException;
				if (ae != null)
				{
					log.LogErrorFromAggregateException(ae);
				}
				else
				{
					log.LogErrorFromException(item);
				}
			}
		}
	}
}
