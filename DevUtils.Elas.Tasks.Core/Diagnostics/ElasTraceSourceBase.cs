using System.Diagnostics;

namespace DevUtils.Elas.Tasks.Core.Diagnostics
{
	/// <summary> The elas trace source base. </summary>
	public abstract class ElasTraceSourceBase : TraceSource
	{
		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="name"> The name. </param>
		protected ElasTraceSourceBase(string name) 
			: base(name, SourceLevels.Verbose)
		{
			Listeners.Add(TaskLoggingTraceListener.Instance);
		}
	}
}