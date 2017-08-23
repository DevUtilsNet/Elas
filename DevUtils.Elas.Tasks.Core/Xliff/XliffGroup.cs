using System.Xml;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary>  The <see cref="XliffGroup"/> class specifies a set of elements that should be processed together. 
	/// 					 For example: all the items of a menu, etc. Note that a <see cref="XliffGroup"/> class can contain other <see cref="XliffGroup"/> class. 
	/// 					 The <see cref="XliffGroup"/> class can be used to describe the hierarchy of the file. </summary>
	public sealed class XliffGroup : XliffUnit
	{
		/// <summary> Gets or sets the units. </summary>
		///
		/// <value> The units. </value>
		public XliffUnitCollection Units { get; private set; }

		internal XliffGroup(string id, XliffDocument document)
			: base(id, document)
		{
			Units = new XliffUnitCollection(this, Document);
		}

		internal XliffGroup(XmlReader xmlReader, XliffDocument document)
			: base(xmlReader, document)
		{
			Units = new XliffUnitCollection(xmlReader, this, Document);
		}

		internal override void Write(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("group");
			xmlWriter.WriteAttributeString("id", Id);

			base.Write(xmlWriter);

			Units.Write(xmlWriter);

			xmlWriter.WriteEndElement();
		}
	}
}