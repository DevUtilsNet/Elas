using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace DevUtils.Elas.Tasks.Core.Reflection.Extensions
{
	internal static class MethodBaseExtensions
	{
		/// <summary>
		/// A MethodBase extension method that executes the preserve stack trace on a different thread, and
		/// waits for the result.
		/// </summary>
		/// <exception cref="TargetInvocationException"> Thrown when a Target Invocation error condition
		/// 																						 occurs. </exception>
		/// <param name="method">		  The method to act on. </param>
		/// <param name="target">		  Target for the. </param>
		/// <param name="invokeArgs"> A variable-length parameters list containing invoke arguments. </param>
		/// <returns> An object. </returns>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static object InvokePreserveStackTrace(this MethodBase method, object target, params object[] invokeArgs)
		{
			try
			{
				var ret = method.Invoke(target, invokeArgs);
				return ret;
			}
			catch (TargetInvocationException te)
			{
				if (te.InnerException == null)
				{
					throw;
				}

				var innerException = te.InnerException;

				var savestack = Delegate.CreateDelegate(typeof (ThreadStart), innerException, "InternalPreserveStackTrace", false, false) as ThreadStart;
				if (savestack != null)
				{
					savestack();
				}

				throw innerException; // -- now we can re-throw without trashing the stack
			}
		}
	}
}
