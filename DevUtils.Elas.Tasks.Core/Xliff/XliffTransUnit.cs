using System.Xml;
using DevUtils.Elas.Tasks.Core.Extensions;
using DevUtils.Elas.Tasks.Core.Xml.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary>  The <see cref="XliffTransUnit "/> class contains a <see cref="XliffSource"/>, <see cref="XliffTarget"/> and associated classes. </summary>
	public sealed class XliffTransUnit : XliffUnit
	{
		private string _note;
		private bool _absent;
		private bool _notTranslate;
		private XliffSource _source;
		private XliffTarget _target;

		/// <summary> is used to add localization-related comments to the XLIFF document. </summary>
		///
		/// <value> The note. </value>
		public string Note
		{
			get { return _note; }
			set
			{
				if (!StringExtensions.EqualsIgnoreNullOrEmpty(_note, value))
				{
					Dirty();
				}
				_note = value;
			}
		}

		/// <summary> Gets or sets a value indicating whether the not translate. </summary>
		///
		/// <value> true if not translate, false if not. </value>
		public bool NotTranslate
		{
			get { return _notTranslate; }
			set
			{
				if (_notTranslate != value)
				{
					Dirty();
				}
				_notTranslate = value;
			}
		}

		/// <summary> Gets or sets the source for the. </summary>
		///
		/// <value> The source. </value>
		public XliffSource Source
		{
			get { return _source; }
			set
			{
				if (_source != value)
				{
					Dirty();
				}
				_source = value;
			}
		}

		/// <summary> Gets or sets the Target for the. </summary>
		///
		/// <value> The target. </value>
		public XliffTarget Target
		{
			get { return _target; }
			set
			{
				if (_target != value)
				{
					Dirty();
				}
				_target = value;
			}
		}

		/// <summary> Gets or sets a value indicating whether the used. </summary>
		///
		/// <value> true if used, false if not. </value>
		public bool IsUsed { get; set; }

		/// <summary> Gets or sets the not used. </summary>
		///
		/// <value> The not used. </value>
		public bool Absent
		{
			get { return _absent; }
			set
			{
				if (_absent != value)
				{
					Dirty();
				}
				_absent = value;
			}
		}

		internal XliffTransUnit(string id, XliffDocument document)
			: base(id, document)
		{
			
		}

		internal XliffTransUnit(XmlReader xmlReader, XliffDocument document)
			: base(xmlReader, document)
		{
			xmlReader.CheckElement("trans-unit", XliffDocument.Namespace);
			NotTranslate = xmlReader.QueryAttribute("translate") == "no";
			var absent = xmlReader.GetAttribute("absent", XliffDocument.ElasNamespace);
			if (absent != null)
			{
				Absent = absent == "yes";
			}

			var depth = xmlReader.CreateDepthControl();

			xmlReader.Read();
			Source = Document.CreateSource(xmlReader);
			Source.SetParent(this);

			while (depth.Above)
			{
				if (xmlReader.IsStartElement("target", XliffDocument.Namespace))
				{
					Target = Document.CreateTarget(xmlReader);
					Target.SetParent(this);
				}

				if (xmlReader.IsStartElement("note", XliffDocument.Namespace))
				{
					Note = xmlReader.ReadElementContentAsString();
				}
			}
			xmlReader.Read();
		}

		internal override void Write(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("trans-unit");
			xmlWriter.WriteAttributeString("id", Id);
			xmlWriter.WriteAttributeString("translate", NotTranslate ? "no" : "yes");
			if (Absent)
			{
				xmlWriter.WriteAttributeString("absent", XliffDocument.ElasNamespace, "yes");
			}

			base.Write(xmlWriter);

			Source.Write(xmlWriter);
			Target.Write(xmlWriter);

			if (!string.IsNullOrEmpty(Note))
			{
				xmlWriter.WriteStartElement("note");
				xmlWriter.WriteString(Note);
				xmlWriter.WriteEndElement();
			}

			xmlWriter.WriteEndElement();
		}

		/// <summary> Used this object. </summary>
		public void Used()
		{
			IsUsed = true;
		}
	}
}