using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary> The elas validate intermediate document. This class cannot be inherited. </summary>
	public sealed class ElasValidateIntermediateDocument : TaskExtension
	{
		class Extractor : XliffDocument
		{
			public struct XliffTargetInfo
			{
				public int LineNumber;
				public int LinePosition;
				public XliffTarget Target;
			}

			public struct XliffFileInfo
			{
				public int LineNumber;
				public int LinePosition;
				public XliffFile File;
			}

			public struct XliffTransUnitInfo
			{
				public int LineNumber;
				public int LinePosition;
				public XliffTransUnit TransUnit;
			}

			public List<XliffFileInfo> XliffFileInfos { get; private set; }
			public List<XliffTargetInfo> XliffTargetsInfos { get; private set; }
			public List<XliffTransUnitInfo> XliffTransUnitInfos { get; private set; }

			public Extractor()
			{
				XliffFileInfos = new List<XliffFileInfo>();
				XliffTargetsInfos = new List<XliffTargetInfo>();
				XliffTransUnitInfos = new List<XliffTransUnitInfo>();
			}

			/// <summary> Creates transaction unit. </summary>
			///
			/// <param name="xmlReader"> The XML reader. </param>
			///
			/// <returns> The new transaction unit. </returns>
			protected internal override XliffTransUnit CreateTransUnit(XmlReader xmlReader)
			{
				var lineInfo = (IXmlLineInfo)xmlReader;
				var lineNumber = lineInfo.LineNumber;
				var linePosition = lineInfo.LinePosition;

				var ret = base.CreateTransUnit(xmlReader);

				XliffTransUnitInfos.Add(new XliffTransUnitInfo
				{
					LineNumber = lineNumber,
					LinePosition = linePosition,
					TransUnit = ret
				});

				return ret;
			}

			protected internal override XliffFile CreateFile(XmlReader xmlReader)
			{
				var lineInfo = (IXmlLineInfo)xmlReader;
				var lineNumber = lineInfo.LineNumber;
				var linePosition = lineInfo.LinePosition;

				var ret = base.CreateFile(xmlReader);

				XliffFileInfos.Add(new XliffFileInfo
				{
					LineNumber = lineNumber,
					LinePosition = linePosition,
					File = ret
				});

				return ret;
				
			}

			protected internal override XliffTarget CreateTarget(XmlReader xmlReader)
			{
				var lineInfo = (IXmlLineInfo)xmlReader;
				var lineNumber = lineInfo.LineNumber;
				var linePosition = lineInfo.LinePosition;

				var ret = base.CreateTarget(xmlReader);

				XliffTargetsInfos.Add(new XliffTargetInfo
				{
					LineNumber = lineNumber, 
					LinePosition = linePosition, 
					Target = ret
				});

				return ret;
			}
		}

		private static readonly XmlSchema ElasSchema;
		private static readonly XmlSchema XliffSchema;

		/// <summary> Gets the files. </summary>
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Gets or sets target cultures. </summary>
		/// <value> The target cultures. </value>
		public ITaskItem[] TargetCultures { get; set; }

		static ElasValidateIntermediateDocument()
		{
			XliffSchema = XmlSchema.Read(new XmlTextReader(new StringReader(Properties.Resources.xliff_core_1_2_transitional)), null);
			ElasSchema = XmlSchema.Read(new XmlTextReader(new StringReader(Properties.Resources.elas_xliff_extension_1_2_transitional)), null);
		}

		private void ValidateSchema(string file)
		{
			var xmlDocument = new XmlDocument();
			var settings = new XmlReaderSettings();

			settings.Schemas.Add(ElasSchema);
			settings.Schemas.Add(XliffSchema);

			settings.ValidationType = ValidationType.Schema;
			settings.ValidationEventHandler += (s, a) => OnValidationEventHandler(file, s, a);
			using (var reader = XmlReader.Create(file, settings))
			{
				xmlDocument.Load(reader);
			}
		}

		private void OnValidationEventHandler(string file, object sender, ValidationEventArgs args)
		{
			var li = (IXmlLineInfo) sender;
			switch (args.Severity)
			{
				case XmlSeverityType.Error:
				case XmlSeverityType.Warning:
				{
					Log.LogWarning("Validation", "XsdSchema", null, file, li.LineNumber, li.LinePosition, 0, 0, args.Message);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void LogWarning(string file, int lineNumber, int columnNumber, string message, params object[] messageArgs)
		{
			LogWarning("Validation", null, null, file, lineNumber, columnNumber, 0, 0, message, messageArgs);
		}

		private void LogError(string file, int lineNumber, int columnNumber, string message, params object[] messageArgs)
		{
			LogError("Validation", null, null, file, lineNumber, columnNumber, 0, 0, message, messageArgs);
		}

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null || TargetCultures == null)
			{
				return;
			}

			var cultures = TargetCultures.Select(s => new CultureInfo(s.ToString())).ToArray();

			foreach (var item in Files)
			{
				var file = item.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath);

				ValidateSchema(file);

				var xliffDocument = new Extractor();

				xliffDocument.Load(file);

				foreach (var item2 in xliffDocument.XliffFileInfos.Where(w => w.File.Units.Any() && !cultures.Contains(w.File.TargetLanguage)))
				{
					LogWarning(file, item2.LineNumber, item2.LinePosition, "Unused culture \"{0}\".", item2.File.TargetLanguage.ToString());
				}

				var count = 0;

				foreach (var item2 in xliffDocument.XliffTargetsInfos
					.Where(w => w.Target != null && cultures.Contains(w.Target.SelectParent().OfType<XliffFile>().First().TargetLanguage)))
				{
					if (item2.Target.State == null || 
							(item2.Target.State != XliffTargetState.Final
							&& item2.Target.State != XliffTargetState.Translated 
							&& item2.Target.State != XliffTargetState.SignedOff))
					{
						var unit = (XliffTransUnit)item2.Target.Parent;

						LogWarning(file, item2.LineNumber, item2.LinePosition, "The Translation unit with id=\"{0}\" has Target with not \"final\", \"translated\" or \"signed-off\" state.", unit.Id);

						if (count++ > 100)
						{
							break;
						}
					}
				}

				count = 0;

				foreach (var item2 in xliffDocument.XliffFileInfos.Where(w => Equals(w.File.SourceLanguage, w.File.TargetLanguage)))
				{
					LogError(file, item2.LineNumber, item2.LinePosition, "In Translation File where original=\"{0}\", source and target languages cannot be the same.", item2.File.Original);

					if (count++ > 100)
					{
						break;
					}
				}

				count = 0;

				foreach (var item2 in xliffDocument.XliffTransUnitInfos.Where(w => w.TransUnit.Absent))
				{
					LogWarning(file, item2.LineNumber, item2.LinePosition,  "Unused translation unit with id=\"{0}\".", item2.TransUnit.Id);

					if (count++ > 100)
					{
						break;
					}
				}
			}
		}
	}
}
