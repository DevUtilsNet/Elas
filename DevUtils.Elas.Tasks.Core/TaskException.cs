using System;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary> Exception for signalling task errors. This class cannot be inherited. </summary>
	public sealed class TaskException : Exception
	{
		/// <summary> Gets or sets the subcategory. </summary>
		///
		/// <value> The subcategory. </value>
		public string Subcategory { get; private set; }

		/// <summary> Gets or sets the error code. </summary>
		///
		/// <value> The error code. </value>
		public string ErrorCode { get; private set; }

		/// <summary> Gets or sets the help keyword. </summary>
		///
		/// <value> The help keyword. </value>
		public string HelpKeyword { get; private set; }

		/// <summary> Gets or sets the file. </summary>
		///
		/// <value> The file. </value>
		public string File { get; private set; }

		/// <summary> Gets or sets the line number. </summary>
		///
		/// <value> The line number. </value>
		public int LineNumber { get; private set; }

		/// <summary> Gets or sets the column number. </summary>
		///
		/// <value> The column number. </value>
		public int ColumnNumber { get; private set; }

		/// <summary> Gets or sets the end line number. </summary>
		///
		/// <value> The end line number. </value>
		public int EndLineNumber { get; private set; }

		/// <summary> Gets or sets the end column number. </summary>
		///
		/// <value> The end column number. </value>
		public int EndColumnNumber { get; private set; }

		/// <summary> Default constructor. </summary>
		public TaskException()
		{
			
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="message"> The message. </param>
		public TaskException(string message)
			: this(null, null, null, null, 0, 0, 0, 0, message, null)
		{
			
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="file">		 The file. </param>
		/// <param name="message"> The message. </param>
		public TaskException(string file, string message)
			: this(null, null, null, file, 0, 0, 0, 0, message, null)
		{

		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="message">			  The message. </param>
		/// <param name="innerException"> The inner exception. </param>
		public TaskException(string message, Exception innerException)
			: this(null, null, null, null, 0, 0, 0, 0, message, innerException)
		{
			
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="file">					  The file. </param>
		/// <param name="message">			  The message. </param>
		/// <param name="innerException"> The inner exception. </param>
		public TaskException(string file, string message, Exception innerException)
			: this(null, null, null, file, 0, 0, 0, 0, message, innerException)
		{

		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="file">					  The file. </param>
		/// <param name="lineNumber">		  The line number. </param>
		/// <param name="columnNumber">	  The column number. </param>
		/// <param name="message">			  The message. </param>
		/// <param name="innerException"> The inner exception. </param>
		public TaskException(string file, int lineNumber, int columnNumber, string message, Exception innerException)
			: this(null, null, null, file, lineNumber, columnNumber, 0, 0, message, innerException)
		{
			
		}

		/// <summary> Constructor. </summary>
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
		/// <param name="innerException">  The inner exception. </param>
		public TaskException(
			string subcategory, 
			string errorCode, 
			string helpKeyword, 
			string file, 
			int lineNumber, 
			int columnNumber, 
			int endLineNumber, 
			int endColumnNumber, 
			string message,
			Exception innerException)
			: base(message, innerException)
		{
			Subcategory = subcategory;
			ErrorCode = errorCode;
			HelpKeyword = helpKeyword;
			File = file;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			EndLineNumber = endLineNumber;
			EndColumnNumber = endColumnNumber;
		}
	}
}
