using System;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Xml.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary>  The <see cref="XliffSource"/> class is used to delimit a unit of text that could be a paragraph, 
	/// 					 a title, a menu item, a caption, etc. The content of the <see cref="XliffSource"/> is generally the translatable text, 
	/// 					 depending upon the translate attribute of the parent <see cref="XliffTransUnit"/>. </summary>
	public sealed class XliffSource : XliffBase
	{
		private string _content;

		internal XliffSource(XliffDocument document)
			: base(document)
		{
			
		}

		internal XliffSource(XmlReader xmlReader, XliffDocument document)
			: base(document)
		{
			xmlReader.CheckElement("source", XliffDocument.Namespace);
			Content = xmlReader.ReadElementContentAsString();
		}

		/// <summary> Text, Zero, one or more of the following elements: <g/>, <x/>, <bx/>, <ex/>, <bpt/> , <ept/>, <ph/>, <it/> , <mrk/>, in any order. </summary>
		///
		/// <value> The content. </value>
		public String Content
		{
			get { return _content; }
			set
			{
				if (_content != value)
				{
					Dirty();
				}
				_content = value;
			}
		}

		internal void Write(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("source");
			xmlWriter.WriteAttributeString("xml", "space", "http://www.w3.org/XML/1998/namespace", "preserve");
			xmlWriter.WriteString(Content);
			xmlWriter.WriteEndElement();
		}
	}
}