using System.Globalization;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Xml.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> An xliff document. </summary>
	public class XliffDocument : XliffBase
	{
		/// <summary> The namespace. </summary>
		public const string Namespace = "urn:oasis:names:tc:xliff:document:1.2";
		/// <summary> The namespace. </summary>
		public const string ElasNamespace = "urn:devutils:names:tc:xliff:document:1.2";

		/// <summary> Gets or sets a value indicating whether this object is dirty. </summary>
		///
		/// <value> true if this object is dirty, false if not. </value>
		public bool IsDirty { get; set; }

		/// <summary> Gets or sets the files. </summary>
		///
		/// <value> The files. </value>
		public XliffFileCollection Files { get; set; }

		/// <summary> Default constructor. </summary>
		public XliffDocument() : base(null)
		{
			Files = new XliffFileCollection(this);
		}

		/// <summary> Creates a group. </summary>
		///
		/// <param name="xmlReader"> The XML reader. </param>
		///
		/// <returns> The new group. </returns>
		internal protected virtual XliffGroup CreateGroup(XmlReader xmlReader)
		{
			var ret = new XliffGroup(xmlReader, this);
			return ret;
		}

		/// <summary> Creates a group. </summary>
		///
		/// <param name="id"> The identifier. </param>
		///
		/// <returns> The new group. </returns>
		internal protected virtual XliffGroup CreateGroup(string id)
		{
			var ret = new XliffGroup(id, this);
			return ret;
		}

		/// <summary> Creates transaction unit. </summary>
		///
		/// <param name="xmlReader"> The XML reader. </param>
		///
		/// <returns> The new transaction unit. </returns>
		internal protected virtual XliffTransUnit CreateTransUnit(XmlReader xmlReader)
		{
			var ret = new XliffTransUnit(xmlReader, this);
			return ret;
		}

		/// <summary> Creates transaction unit. </summary>
		///
		/// <param name="id"> The identifier. </param>
		///
		/// <returns> The new transaction unit. </returns>
		internal protected virtual XliffTransUnit CreateTransUnit(string id)
		{
			var ret = new XliffTransUnit(id, this);
			return ret;
		}

		/// <summary> Creates a file. </summary>
		///
		/// <param name="original">			  The original. </param>
		/// <param name="sourceLanguage"> Source language. </param>
		/// <param name="targetLanguage"> Target language. </param>
		/// <param name="datatype">			  The datatype. </param>
		///
		/// <returns> The new file. </returns>
		internal protected virtual XliffFile CreateFile(string original, CultureInfo sourceLanguage, CultureInfo targetLanguage, XliffDataType datatype)
		{
			var ret = new XliffFile(original, sourceLanguage, targetLanguage, datatype, this);
			return ret;
		}

		/// <summary> Creates a file. </summary>
		///
		/// <param name="xmlReader"> The XML reader. </param>
		///
		/// <returns> The new file. </returns>
		internal protected virtual XliffFile CreateFile(XmlReader xmlReader)
		{
			var ret = new XliffFile(xmlReader, this);
			return ret;
		}

		/// <summary> Creates a target. </summary>
		///
		/// <param name="xmlReader"> The XML reader. </param>
		///
		/// <returns> The new target. </returns>
		internal protected virtual XliffTarget CreateTarget(XmlReader xmlReader)
		{
			var ret = new XliffTarget(xmlReader, this);
			return ret;
		}

		/// <summary> Creates a target. </summary>
		///
		/// <returns> The new target. </returns>
		public virtual XliffTarget CreateTarget()
		{
			var ret = new XliffTarget(this);
			return ret;
		}

		/// <summary> Creates a source. </summary>
		///
		/// <param name="xmlReader"> The XML reader. </param>
		///
		/// <returns> The new source. </returns>
		internal protected virtual XliffSource CreateSource(XmlReader xmlReader)
		{
			var ret = new XliffSource(xmlReader, this);
			return ret;
		}

		/// <summary> Creates a source. </summary>
		///
		/// <returns> The new source. </returns>
		public virtual XliffSource CreateSource()
		{
			var ret = new XliffSource(this);
			return ret;
		}

		/// <summary> Loads the given file. </summary>
		///
		/// <param name="filename"> The filename to load. </param>
		public void Load(string filename)
		{
			Files.Clear();

			using (var xmlReader = new XmlTextReader(filename))
			{
				xmlReader.WhitespaceHandling = WhitespaceHandling.None;

				var depth = xmlReader.CreateDepthControl();

				xmlReader.ReadStartElement("xliff", Namespace);

				if (xmlReader.NodeType == XmlNodeType.None)
				{
					return;
				}

				do
				{
					var file = CreateFile(xmlReader);
					Files.Add(file);
				} while (depth.Above);
			}

			IsDirty = false;
		}

		/// <summary> Saves the given file. </summary>
		///
		/// <param name="filename"> The filename to load. </param>
		public void Save(string filename)
		{
			var ws = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				NewLineHandling = NewLineHandling.None
			};

			using (var xmlWriter = XmlWriter.Create(filename, ws))
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("xliff", Namespace);
				xmlWriter.WriteAttributeString("version", "1.2");
				xmlWriter.WriteAttributeString("xmlns", "elas", null, ElasNamespace);

				foreach (var item in Files)
				{
					xmlWriter.WriteStartElement("file");
					item.Write(xmlWriter);
					xmlWriter.WriteEndElement();
				}

				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}

			IsDirty = false;
		}
	}
}
