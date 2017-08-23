using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Extensions;
using DevUtils.Elas.Tasks.Core.Xml.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> The <see cref="XliffFile"/> class corresponds to a single extracted original document. </summary>
	public sealed class XliffFile : XliffBase
	{
		private DateTime? _date;
		private string _original;
		private CultureInfo _sourceLanguage;
		private CultureInfo _targetLanguage;
		private XliffDataType _dataType;

		/// <summary> Gets or sets the Date/Time of the date. </summary>
		///
		/// <value> The date. </value>
		public DateTime? Date
		{
			get { return _date; }
			set
			{
				if (_date != value)
				{
					//Dirty();
				}
				_date = value;
			}
		}

		/// <summary> The <see cref="Original"/> specifies the name of the original file from which the contents of a <see cref="XliffFile"/> class has been extracted. </summary>
		///
		/// <value> The original. </value>
		public string Original
		{
			get { return _original; }
			set
			{
				if (_original != value)
				{
					Dirty();
				}
				_original = value;
			}
		}

		/// <summary> The language for the <see cref="XliffSource"/> classes in the given <see cref="XliffFile"/> class. </summary>
		///
		/// <value> The source language. </value>
		public CultureInfo SourceLanguage
		{
			get { return _sourceLanguage; }
			set
			{
				if (!Equals(_sourceLanguage, value))
				{
					Dirty();
				}
				_sourceLanguage = value;
			}
		}

		/// <summary> The language for the <see cref="XliffTarget"/> class in the given <see cref="XliffFile"/> class. </summary>
		///
		/// <value> The target language. </value>
		public CultureInfo TargetLanguage
		{
			get { return _targetLanguage; }
			set
			{
				if (!Equals(_targetLanguage, value))
				{
					Dirty();
				}
				_targetLanguage = value;
			}
		}

		/// <summary>  The datatype attribute specifies the kind of text contained in the <see cref="XliffFile"/>.
		/// 					 Depending on that type, you may apply different processes to the data.
		/// 					 For example: <see cref="DataType"/>=<see cref="XliffDataType.Winres"/> specifies that the content is Windows resources which would allow using the
		/// 					 Win32 API in rendering the content. </summary>
		///
		/// <value> The type of the data. </value>
		public XliffDataType DataType
		{
			get { return _dataType; }
			set
			{
				if (_dataType != value)
				{
					Dirty();
				}
				_dataType = value;
			}
		}

		/// <summary> Gets or sets the units. </summary>
		///
		/// <value> The units. </value>
		public XliffUnitCollection Units { get; private set; }

		/// <summary> Default constructor. </summary>
		///
		/// <param name="original">			 	The <see cref="Original"/> specifies the name of the original
		/// 															file from which the contents of a <see cref="XliffFile"/> class
		/// 															has been extracted. </param>
		/// <param name="sourceLanguage">	The language for the <see cref="XliffSource"/> classes in the
		/// 															given <see cref="XliffFile"/> class. </param>
		/// <param name="targetLanguage">	The language for the <see cref="XliffTarget"/> class in the given
		/// 															<see cref="XliffFile"/> class. </param>
		/// <param name="dataType">			 	The datatype attribute specifies the kind of text contained in
		/// 															the <see cref="XliffFile"/>. Depending on that type, you may
		/// 															apply different processes to the data. For example:
		/// 															<see cref="DataType"/>=<see cref="XliffDataType.Winres"/>
		/// 															specifies that the content is Windows resources which would allow
		/// 															using the Win32 API in rendering the content. </param>
		/// <param name="document">			  The document. </param>
		internal XliffFile(string original, CultureInfo sourceLanguage, CultureInfo targetLanguage, XliffDataType dataType, XliffDocument document)
			: base(document)
		{
			Original = original;
			DataType = dataType;
			SourceLanguage = sourceLanguage;
			TargetLanguage = targetLanguage;

			Units = new XliffUnitCollection(this, Document);
		}

		internal XliffFile(XmlReader xmlReader, XliffDocument document)
			: base(document)
		{
			xmlReader.CheckElement("file", XliffDocument.Namespace);
			Original = xmlReader.QueryAttribute("original");
			SourceLanguage = new CultureInfo(xmlReader.QueryAttribute("source-language"));
			TargetLanguage = new CultureInfo(xmlReader.QueryAttribute("target-language"));
			DataType = xmlReader.QueryAttribute("datatype").EnumFromStringValue<XliffDataType>();

			var date = xmlReader.GetAttribute("date");
			if (date != null)
			{
				DateTime tmp;
				if (DateTime.TryParse(date, out tmp))
				{
					_date = tmp;
				}
			}

			var depth = xmlReader.CreateDepthControl();
			while (xmlReader.Read() && depth.Above)
			{
				if (xmlReader.IsStartElement("body", XliffDocument.Namespace))
				{
					Units = new XliffUnitCollection(xmlReader, this, Document);
					xmlReader.Read();
					break;
				}
			}
		}

		internal void Write(XmlWriter xmlWriter)
		{
			var executingAssembly = Assembly.GetExecutingAssembly();

			xmlWriter.WriteAttributeString("original", Original);
			xmlWriter.WriteAttributeString("source-language", SourceLanguage.Name);
			xmlWriter.WriteAttributeString("target-language", TargetLanguage.Name);
			xmlWriter.WriteAttributeString("datatype", DataType.GetStringValue());

			xmlWriter.WriteStartElement("header");
			xmlWriter.WriteStartElement("tool");
			xmlWriter.WriteAttributeString("tool-version", executingAssembly.GetName().Version.ToString());
			xmlWriter.WriteAttributeString("tool-name", executingAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product);
			xmlWriter.WriteAttributeString("tool-company", executingAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company);

			xmlWriter.WriteAttributeString("tool-id", executingAssembly.FullName);

			//if (_date.HasValue)
			//{
			//	xmlWriter.WriteAttributeString("date", _date.Value.ToString("O"));
			//}

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("body");

			Units.Write(xmlWriter);

			xmlWriter.WriteEndElement();
		}
	}
}