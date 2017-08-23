using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.Diagnostics
{
	/// <summary> A task logging trace listener. This class cannot be inherited. </summary>
	public sealed class TaskLoggingTraceListener : TraceListener
	{
		private static TaskLoggingTraceListener _instance;

		/// <summary> Gets or sets the task logging. </summary>
		///
		/// <value> The task logging. </value>
		public TaskLoggingHelper TaskLogging { get; set; }

		/// <summary> Gets the instance. </summary>
		///
		/// <value> The instance. </value>
		public static TaskLoggingTraceListener Instance
		{
			get { return _instance ?? (_instance = new TaskLoggingTraceListener()); }
		}

		/// <summary> When overridden in a derived class, writes the specified message to the listener you
		/// create in the derived class. </summary>
		///
		/// <param name="message"> A message to write. </param>
		public override void Write(string message)
		{
			throw new NotImplementedException();
		}

		/// <summary> When overridden in a derived class, writes a message to the listener you create in
		/// the derived class, followed by a line terminator. </summary>
		///
		/// <param name="message"> A message to write. </param>
		public override void WriteLine(string message)
		{
			throw new NotImplementedException();
		}

		/// <summary> Writes trace information, a message, and event information to the listener specific
		/// output. </summary>
		///
		/// <exception cref="ArgumentOutOfRangeException">	Thrown when one or more arguments are outside
		/// 																								the required range. </exception>
		///
		/// <param name="eventCache">	A <see cref="T:System.Diagnostics.TraceEventCache" /> object that
		/// 													contains the current process ID, thread ID, and stack trace
		/// 													information. </param>
		/// <param name="source">		 	A name used to identify the output, typically the name of the
		/// 													application that generated the trace event. </param>
		/// <param name="eventType"> 	One of the <see cref="T:System.Diagnostics.TraceEventType" /> values
		/// 													specifying the type of event that has caused the trace. </param>
		/// <param name="id">				  A numeric identifier for the event. </param>
		/// <param name="message">	  A message to write. </param>
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			TraceEvent(eventCache, source, eventType, id, message, null);
		}

		/// <summary> Writes trace information, a formatted array of objects and event information to the
		/// listener specific output. </summary>
		///
		/// <exception cref="ArgumentOutOfRangeException">	Thrown when one or more arguments are outside
		/// 																								the required range. </exception>
		///
		/// <param name="eventCache">	A <see cref="T:System.Diagnostics.TraceEventCache" /> object that
		/// 													contains the current process ID, thread ID, and stack trace
		/// 													information. </param>
		/// <param name="source">		 	A name used to identify the output, typically the name of the
		/// 													application that generated the trace event. </param>
		/// <param name="eventType"> 	One of the <see cref="T:System.Diagnostics.TraceEventType" /> values
		/// 													specifying the type of event that has caused the trace. </param>
		/// <param name="id">				  A numeric identifier for the event. </param>
		/// <param name="format">		 	A format string that contains zero or more format items, which
		/// 													correspond to objects in the <paramref name="args" /> array. </param>
		/// <param name="args">			  An object array containing zero or more objects to format. </param>
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format,
			params object[] args)
		{
			if (TaskLogging != null)
			{
				switch (eventType)
				{
					case TraceEventType.Critical:
					case TraceEventType.Error:
						{
							TaskLogging.LogError(source, id.ToString(CultureInfo.InvariantCulture), null, null, 0, 0, 0, 0, format, args);
							break;
						}
					case TraceEventType.Warning:
						{
							TaskLogging.LogWarning(source, id.ToString(CultureInfo.InvariantCulture), null, null, 0, 0, 0, 0, format, args);
							break;
						}
					case TraceEventType.Information:
						{
							TaskLogging.LogMessage(source, id.ToString(CultureInfo.InvariantCulture), null, null, 0, 0, 0, 0, MessageImportance.Normal, format, args);
							break;
						}
					case TraceEventType.Verbose:
						{
							TaskLogging.LogMessage(source, id.ToString(CultureInfo.InvariantCulture), null, null, 0, 0, 0, 0, MessageImportance.Low, format, args);
							break;
						}
					case TraceEventType.Start:
					case TraceEventType.Stop:
					case TraceEventType.Suspend:
					case TraceEventType.Resume:
					case TraceEventType.Transfer:
					default:
						throw new ArgumentOutOfRangeException("eventType");
				}
			}
		}
	}
}
