using System;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Extensions;
using DevUtils.Elas.Tasks.Core.Xml.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> The <see cref="XliffTarget"/> class contains the translation of the content of the sibling <see cref="XliffSource"/> class. </summary>
	public sealed class XliffTarget : XliffBase
	{
		private string _content;
		private XliffTargetState? _state;

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

		/// <summary> The status of a particular translation in a <see cref="XliffTarget"/>. </summary>
		///
		/// <value> The state. </value>
		public XliffTargetState? State
		{
			get { return _state; }
			set
			{
				if (_state != value)
				{
					Dirty();
				}
				_state = value;
			}
		}

		internal XliffTarget(XliffDocument document)
			: base(document)
		{
			
		}

		internal XliffTarget(XmlReader xmlReader, XliffDocument document)
			: base(document)
		{
			xmlReader.CheckElement("target", XliffDocument.Namespace);
			var state = xmlReader.GetAttribute("state");
			if (state != null)
			{
				State = state.EnumFromStringValue<XliffTargetState>();
			}

			Content = xmlReader.ReadElementContentAsString();
		}

		internal void Write(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("target");
			xmlWriter.WriteAttributeString("xml", "space", "http://www.w3.org/XML/1998/namespace", "preserve");

			if (State.HasValue)
			{
				xmlWriter.WriteAttributeString("state", State.GetStringValue());
			}

			xmlWriter.WriteString(Content);
			xmlWriter.WriteEndElement();
		}
	}
}